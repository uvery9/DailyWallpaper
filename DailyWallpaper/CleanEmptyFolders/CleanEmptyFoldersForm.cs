using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;
using DailyWallpaper.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
// using System.Linq;

namespace DailyWallpaper
{
    public partial class CleanEmptyFoldersForm : Form, IDisposable
    {
        private CleanEmptyFolders _cef;
        private CEFTextWriter _console;
        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private CancellationTokenSource _source;
        private bool deletePermanently = false;

        // Speed up the next scan
        private List<string> emptyFolderList;
        private string printPath = null;
        private bool scanRes = false;

        public CleanEmptyFoldersForm()
        {
            InitializeComponent();
            this.tbTargetFolder.KeyDown += tbTargetFolder_KeyDown;
            Icon = Properties.Resources.icon32x32;
            _cef = new CleanEmptyFolders();
            _console = new CEFTextWriter(new ControlWriter(tbConsole));
            _console.WriteLine(_cef.helpString);
            var init = _cef.ini.Read("CleanEmptyFoldersPath", "LOG");
            if (Directory.Exists(init))
            {
                tbTargetFolder.Text = init;
            } else
            {
                tbTargetFolder.Text = desktopPath;
            }
            _source = new CancellationTokenSource();
            btnStop.Enabled = false;
            // default: send to RecycleBin
            deleteOrRecycleBin.Checked = false;
            DeleteOrRecycleBin(deletePermanently: false);
            emptyFolderList = new List<string>();
            saveList2File.Enabled = false;
        }

        private void btnSelectOutFolder_Click(object sender, EventArgs e)
        {
            /*  Note that you need to install the Microsoft.WindowsAPICodePack.Shell package 
                through NuGet before you can use this CommonOpenFileDialog
                
                VS->Tools->NuGet Package manager->Program Package Manager Terminal->
                Type: Install-Package Microsoft.WindowsAPICodePack-Shell    Enter 
                using Microsoft.WindowsAPICodePack.Dialogs;
            */

            using (var dialog = new CommonOpenFileDialog())
            {
                if (Directory.Exists(tbTargetFolder.Text))
                {
                    dialog.InitialDirectory = tbTargetFolder.Text;
                }
                else
                {
                    dialog.InitialDirectory = desktopPath;
                }    
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Clean Empty Folders";

                // maybe add some log
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    var path = dialog.FileName;
                    if (!UpdateTextAndIniFile(path))
                    {
                        return;
                    }
                    tbTargetFolder.Text = path;
                }
            }
        }

        private void btnClean_Click(object sender, EventArgs e)
        {
            _source = new CancellationTokenSource();
            btnStop.Enabled = true;
            btnClean.Enabled = false;
            btnPrint.Enabled = false;
            RecurseScanDir(_cef.targetFolderPath, _source.Token, true);
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            _source = new CancellationTokenSource();
            btnStop.Enabled = true;
            btnClean.Enabled = false;
            btnPrint.Enabled = false;
            RecurseScanDir(_cef.targetFolderPath, _source.Token, false);

        }
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_source != null)
            {
                _source.Cancel();
                //_source.Dispose();
                //_source = null;
            }
            btnClean.Enabled = true;
            btnPrint.Enabled = true;
            btnStop.Enabled = false;
        }
        private void btnClear_Click(object sender, EventArgs e)
        {

            tbConsole.Clear();
            // Invoke(new Action(Program.RunClear));
        }

        // Thanks to João Angelo
        // https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
        private async void RecurseScanDir(string path, CancellationToken token, bool delete = false)
        {
            //token.ThrowIfCancellationRequested();
            if (!Directory.Exists(path))
            {
                _console.WriteLine("Invalid directory path: {0}", path);
                return;
            }
            string option = "Delete";
            if (!delete)
            {
                option = "Print";
                printPath = path;
                scanRes = false;
                emptyFolderList = new List<string>();
            }

            _console.WriteLine($"#---- Started  {option} Operation ----#");
            _console.WriteLine();
            _console.WriteLine($"The following folders will be deleted:\r\n");
            var task = Task.Run(() =>
            {
                // Were we already canceled?
                // token.ThrowIfCancellationRequested();
                try
                {
                    // Set a variable to the My Documents path.
                    // string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 

                    if (scanRes && path.Equals(printPath))
                    {
                        if (emptyFolderList.Count > 0)
                        {
                            foreach (var folder in emptyFolderList)
                            {
                                _console.WriteLine($"delete ###  {folder}");
                                FileSystem.DeleteDirectory(folder, UIOption.OnlyErrorDialogs,
                                deletePermanently ?
                                RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin,
                                UICancelOption.DoNothing);
                            }
                        }
                        return;
                    }
                    int cnt = 0;
                    DeleteEmptyDirs(path, ref cnt, token, delete, deletePermanently);                  
                    if (cnt == 0)
                    {
                        _console.WriteLine("[NOTHING]");
                        _console.WriteLine();
                        _console.WriteLine("The folder is clean.");
                    }
                }
                catch (OperationCanceledException e)
                {
                    throw e;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }, token); // Pass same token to Task.Run.
            try
            {
                await task;

                // No Error, emptyFolderList is usable
                scanRes = true;
                if (emptyFolderList.Count > 1)
                {
                    saveList2File.Enabled = true;
                }
                _console.WriteLine();
                _console.WriteLine($"\r\n#---- Finished {option} Operation ----#");
            }
            catch (OperationCanceledException e)
            {
                scanRes = false;
                saveList2File.Enabled = false;
                _console.WriteLine($"\r\nRecurseScanDir throw exception message: {e.Message}");
            }
            catch (Exception ex)
            {
                scanRes = false;
                saveList2File.Enabled = false;
                _console.WriteLine($"\r\nRecurseScanDir throw exception message: {ex.Message}");
                _console.WriteLine($"\r\n#----^^^  PLEASE CHECK, TRY TO CONTACT ME WITH THIS LOG.  ^^^----#");
            }

            btnClean.Enabled = true;
            btnPrint.Enabled = true;
            btnStop.Enabled = false;
        }

          private void DeleteEmptyDirs(string dir, ref int cnt, CancellationToken token, bool delete = false,
            bool deletePermanently = false)
        {
            if (String.IsNullOrEmpty(dir))
            {
                throw new ArgumentException(
                                    "Starting directory is a null reference or an empty string",
                                    "dir");
            }
            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    DeleteEmptyDirs(d, ref cnt, token, delete, deletePermanently);
                }
                var entries = Directory.EnumerateFileSystemEntries(dir);

                if (!entries.Any())
                {
                    try
                    {
                        cnt++;
                        // Directory.Delete(dir);
                        emptyFolderList.Add(dir);
                        if (delete)
                        {
                            _console.WriteLine($"deleted >>>  {dir}");
                            FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs,
                                deletePermanently ?
                                RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin,
                                UICancelOption.DoNothing);
                        }
                        else
                        {
                            _console.WriteLine($"print >>>  {dir}");
                        }
                    }
                    catch (UnauthorizedAccessException) { }
                    catch (DirectoryNotFoundException) { }
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        /// <summary>
        /// check if Controlled Or NotExist
        /// </summary>
        private bool IsControlledOrNotExist(string path)
        {
            if (_cef.IsControlledFolder(path)){
                _console.WriteLine($"\r\nThe folder is CONTROLLED, please re-select:\r\n   {path}");
                _console.WriteLine("\r\nYou could Type \" list controlled \" in the Target Folder and Type ENTER" + 
                    " to see all the controlled folders.");
                return true;
            }
            if (!Directory.Exists(path))
            {
                _console.WriteLine($"\r\nThe folder dose NOT EXIST, please re-select:\r\n   {path}");
                return true;
            }
            return false;
        }


        private void tbTargetFolder_TextChanged(object sender, EventArgs e)
        {
            _cef.targetFolderPath = tbTargetFolder.Text;
            //var newPath = "path change to:" + tbTargetFolder.Text + "\r\n";
            //this.tbConsole.Text += newPath;
        }

        private void CleanEmptyFoldersForm_Load(object sender, EventArgs e)
        {

        }
        private void cefWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        
        private bool UpdateTextAndIniFile(string path)
        {
            if (IsControlledOrNotExist(path))
            {
                return false;
            }
            _cef.targetFolderPath = path;
            _cef.ini.UpdateIniItem("CleanEmptyFoldersPath", path, "LOG");
            _console.WriteLine($"\r\nYou have selected this folder:\r\n  {path}");
            return true;
        }
        private void tbTargetFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox box)
                {
                    string path = box.Text;
                    path = path.Trim();
                    while (path.EndsWith("\\"))
                    {
                        path = path.Substring(0, path.Length - 1);
                    }
                    if (path.ToLower().Equals("list controlled"))
                    {
                        _console.WriteLine("\r\nThe following is a list of controlled folders:");
                        foreach (var f in _cef.GetAllControlledFolders())
                        {
                            _console.WriteLine(f);
                        }
                        return;
                    }
                    UpdateTextAndIniFile(path);
                }
            }
        }

        private void DeleteOrRecycleBin(bool deletePermanently = false)
        {
            if (!deletePermanently)
            {
                deletePermanently = false;
                btnClean.Text = "RecycleBin";
            } else
            {
                deletePermanently = true;
                btnClean.Text = "Delete Permanently";
            }
            
        }
        private void deleteOrRecycleBin_CheckedChanged(object sender, EventArgs e)
        {
            if (deleteOrRecycleBin.Checked == true)
            {
                DeleteOrRecycleBin(deletePermanently: true);
            }
            else
            {
                DeleteOrRecycleBin(deletePermanently: false);
            }
        }

        private void saveList2File_Click(object sender, EventArgs e)
        {

            if (emptyFolderList.Count < 1)
            {
                return;
            }
            if (!Directory.Exists(_cef.targetFolderPath))
            {
                return;
            }
            using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                saveFileDialog.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog.Filter = "Txt files (*.txt)|*.txt";
                // saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = "EmptyFolderIn_" + new DirectoryInfo(_cef.targetFolderPath).Name 
                    + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); //+ ".txt"

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        // Code to write the stream goes here.
                        byte[] dataAsBytes = emptyFolderList.SelectMany(s =>
                            System.Text.Encoding.Default.GetBytes(s + Environment.NewLine)).ToArray();
                        // byte[] byteArray = System.Text.Encoding.Default.GetBytes(tbConsole.Text);
                        stream.Write(dataAsBytes, 0, dataAsBytes.Length);
                    }
                }
            }

        }
    }
}
