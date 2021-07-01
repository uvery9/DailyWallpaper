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
using System.Text.RegularExpressions;
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
        private List<string> folderFilter;
        private string regexFilter;
        private Regex regex;
        private List<string> tbTargetFolderHistory = new List<string>();
        private enum FilterMode: int
        {
            REGEX_FIND,
            REGEX_PROTECT,
            GEN_FIND,
            GEN_PROTECT,

        }
        private FilterMode filterMode;
        public CleanEmptyFoldersForm()
        {
            InitializeComponent();
            this.tbTargetFolder.KeyDown += tbTargetFolder_KeyDown;
            folderFilterTextBox.KeyDown += folderFilterTextBox_KeyDown;
            Icon = Properties.Resources.icon32x32;
            _cef = new CleanEmptyFolders();
            _console = new CEFTextWriter(new ControlWriter(tbConsole));
            _console.WriteLine(_cef.helpString);
            var init = _cef.ini.Read("CleanEmptyFoldersPath", "LOG");
            if (Directory.Exists(init))
            {
                UpdateTextAndIniFile(init, updateIni : false);
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
            listOrLog.Checked = true;
            folderFilter = new List<string>();
            this.MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            SetUpFilterMode();
            this.regexCheckBox.CheckedChanged += new System.EventHandler(this.regexCheckBox_CheckedChanged);
        }
        /// <summary>
        /// bind to tbTargetFolderHistory
        /// </summary>
        private void SetUpFilterMode()
        {
            var fimode = _cef.ini.Read("FilterMode");
            if (!string.IsNullOrEmpty(fimode))
            {
                if (fimode.Equals("GEN_FIND"))
                {
                    regexCheckBox.Checked = false;
                    filterMode = FilterMode.GEN_FIND;
                }
                else if (fimode.Equals("REGEX_PROTECT"))
                {
                    regexCheckBox.Checked = true;
                    filterMode = FilterMode.REGEX_PROTECT;
                }
                else if (fimode.Equals("REGEX_FIND"))
                {
                    regexCheckBox.Checked = true;
                    filterMode = FilterMode.REGEX_FIND;
                }
                else
                {
                    regexCheckBox.Checked = false;
                    filterMode = FilterMode.GEN_PROTECT;
                }               
            }
        }
        private void BindHistory(TextBox tb, List<string> list)
        {
            tb.AutoCompleteCustomSource.Clear();
            tb.AutoCompleteCustomSource.AddRange(list.ToArray());
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
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
            PrintDir(delete: true);
        }

        private void PrintDir(bool delete = false)
        {
            SetFolderFilter(folderFilterTextBox.Text, print: true);
            _source = new CancellationTokenSource();
            btnStop.Enabled = true;
            btnClean.Enabled = false;
            btnPrint.Enabled = false;
            RecurseScanDir(_cef.targetFolderPath, _source.Token, delete);
        }
        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDir();

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
        /// <summary>
        /// TODO: When folder To much, try to not use Recurse
        /// </summary>
        private async void RecurseScanDir(string path, CancellationToken token, bool delete = false)
        {
            //token.ThrowIfCancellationRequested();
            // DO NOT KNOW WHY D: DOESNOT WORK WHILE D:\ WORK.
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
                    else
                    {
                        _console.WriteLine();
                        _console.WriteLine($"Found {cnt} empty folder(s).");
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
                _console.WriteLine();
                _console.WriteLine($"\r\n#---- Finished {option} Operation ----#");
            }
            catch (OperationCanceledException e)
            {
                scanRes = false;
                _console.WriteLine($"\r\nRecurseScanDir throw exception message: {e.Message}");
            }
            catch (Exception ex)
            {
                scanRes = false;
                _console.WriteLine($"\r\nRecurseScanDir throw exception message: {ex.Message}");
                _console.WriteLine($"\r\n#----^^^  PLEASE CHECK, TRY TO CONTACT ME WITH THIS LOG.  ^^^----#");
            }

            btnClean.Enabled = true;
            btnPrint.Enabled = true;
            btnStop.Enabled = false;
        }

        private bool FolderFilter(string path, FilterMode mode)
        {              
            if (mode == FilterMode.GEN_PROTECT)
            {
                if (folderFilter.Count > 0)
                {
                    foreach (var filter in folderFilter)
                    {
                        if (path.Contains(filter))
                        {
                            return true;
                        }
                    }
                }
            }
            else if (mode == FilterMode.REGEX_PROTECT)
            {
                if (regex == null)
                {
                    return false;
                }
                if (regex.IsMatch(path))
                {
                    return true;
                }
            }
            else if (mode == FilterMode.GEN_FIND)
            {
                if (folderFilter.Count > 0)
                {
                    foreach (var filter in folderFilter)
                    {
                        if (!path.Contains(filter))
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                if (regex == null)
                {
                    return false;
                }
                if (!regex.IsMatch(path))
                {
                    return true;
                }
            }
            return false;
        }

        private void DeleteEmptyDirs(string dir, ref int cnt, CancellationToken token, bool delete = false,
            bool deletePermanently = false)
        {
            if (String.IsNullOrEmpty(dir))
            {
                throw new ArgumentException("Starting directory is a null reference or an empty string: dir");
            }
            try
            {
                foreach (var d in Directory.EnumerateDirectories(dir))
                {
                    // FUCK THE $RECYCLE.BIN
                    if (d.ToLower().Contains("$RECYCLE.BIN".ToLower())) {
                        continue;
                    }
                    if (FolderFilter(d, filterMode))
                    {
                        continue;
                    }                   
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
        private bool IsControlled(string path, bool print = true)
        {
            if (_cef.IsControlledFolder(path)){
                if (print)
                {
                    _console.WriteLine($"\r\nThe folder is CONTROLLED, please re-select:\r\n   {path}");
                    _console.WriteLine("\r\nYou could Type \" list controlled \" in the Folder Filter and Type ENTER" +
                        " to see all the controlled folders.");
                }
                return true;
            }
            return false;
        }


        private void tbTargetFolder_TextChanged(object sender, EventArgs e)
        {
            // DONOTHING
        }

        private void CleanEmptyFoldersForm_Load(object sender, EventArgs e)
        {

        }
        private void cefWindow_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
        
        private bool UpdateTextAndIniFile(string path, bool updateIni = true, bool print = true)
        {
            if (IsControlled(path))
            {
                return false;
            }
            if (!Directory.Exists(path))
            {
                if (print)
                {
                    _console.WriteLine($"\r\nThe folder dose NOT EXIST, please re-select:\r\n   {path}");
                }
                return false;
            }
            // DirectoryIn
            path = Path.GetFullPath(path);
            tbTargetFolderHistory.Add(path);
            BindHistory(tbTargetFolder, tbTargetFolderHistory);
            _cef.targetFolderPath = path;
            tbTargetFolder.Text = path;
            if (updateIni)
            {
                _cef.ini.UpdateIniItem("CleanEmptyFoldersPath", path, "LOG");
            }
            if (print)
            {
                _console.WriteLine($"\r\nYou have selected this folder:\r\n  {path}");
            }
            return true;
        }

        /// <summary>
        /// Should TEST
        /// </summary>
        private void folderFilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox box)
                {
                    IsCmdInTextBox(folderFilterTextBox, box.Text);
                }
            }
        }

        private bool IsCmdInTextBox(TextBox box, string cmd)
        {
            cmd = cmd.Trim();

            // command mode
            bool useCommand = false;
            if (cmd.ToLower().Equals("list controlled"))
            {
                _console.WriteLine("\r\nThe following is a list of controlled folders:");
                foreach (var f in _cef.GetAllControlledFolders())
                {
                    _console.WriteLine(f);
                }
                useCommand = true;
            }
            if (cmd.ToLower().Equals("mode protect"))
            {
                
                useCommand = true;
                if (regexCheckBox.Checked) 
                {
                    filterMode = FilterMode.REGEX_PROTECT;
                }
                else
                {
                    filterMode = FilterMode.GEN_PROTECT;
                }
                _console.WriteLine($"\r\nmode: {filterMode}");
                _cef.ini.UpdateIniItem("FilterMode", filterMode.ToString());
            }
            if (cmd.ToLower().Equals("mode find"))
            {
                useCommand = true;
                if (regexCheckBox.Checked)
                {
                    filterMode = FilterMode.REGEX_FIND;
                }
                else
                {
                    filterMode = FilterMode.GEN_FIND;
                }
                _console.WriteLine($"\r\nmode: {filterMode}");
                _cef.ini.UpdateIniItem("FilterMode", filterMode.ToString());
            }

            // recover
            if (useCommand)
            {
                box.Text = "";
                return true;
            }
            return false;
        }
        private void tbTargetFolder_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox box)
                {
                    var path = box.Text;
                    path = path.Trim();
                    if (UpdateTextAndIniFile(path))
                    {
                        PrintDir();
                    }
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

            if (listOrLog.Checked && emptyFolderList.Count < 1)
            {
                _console.WriteLine("You SHOULD scan one folder first.");
                return;
            }
            if (listOrLog.Checked && !Directory.Exists(_cef.targetFolderPath))
            {
                return;
            }
            if (!listOrLog.Checked && tbConsole.Text.Length < 1)
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
                var name = new DirectoryInfo(_cef.targetFolderPath).Name;
                
                // E:, D: -> D-Disk
                // need TEST here
                if (name.Contains(":"))
                {
                    name = name.Split(':')[0] + "-Disk";
                }
                if (listOrLog.Checked)
                {
                    saveFileDialog.FileName = "EmptyFolders-List_" + name + "_" +
                                         DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); //+ ".txt"
                } else
                {
                    saveFileDialog.FileName = "EmptyFolders-Log_" + name + "_" +
                                         DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); //+ ".txt"
                }
                

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        // Code to write the stream goes here.
                        byte[] dataAsBytes = null;

                        if (listOrLog.Checked)
                        {
                            dataAsBytes = emptyFolderList.SelectMany(s =>
                            System.Text.Encoding.Default.GetBytes(s + Environment.NewLine)).ToArray();
                        }
                        else
                        {
                            dataAsBytes = System.Text.Encoding.Default.GetBytes(tbConsole.Text);
                        }
                        stream.Write(dataAsBytes, 0, dataAsBytes.Length);
                    }
                }
            }

        }

        private void listOrLog_CheckedChanged(object sender, EventArgs e)
        {
            if (listOrLog.Checked)
            {
                saveList2File.Text = "Save list to File";
            }
            else
            {
                saveList2File.Text = "Save log to File";
            }
        }

        private void filterExample_TextChanged(object sender, EventArgs e)
        {

        }

        private void SetFolderFilter(string text, bool print = false)
        {
            _console.WriteLine($">>> Using: {filterMode}");
            string filter = text;
            if (string.IsNullOrEmpty(filter))
            {
                folderFilter = new List<string>();
                regexFilter = "";
                return;
            }
            folderFilter = new List<string>();
            regexFilter = "";
            if (regexCheckBox.Checked)
            {
                regexFilter = filter;
                regex = new Regex(regexFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                if (print)
                {
                    _console.WriteLine($"\r\nYou have set the regex filter: \" {regexFilter} \"");
                }
                return;
            }
            if (filter.Contains("，"))
            {
                if (print) _console.WriteLine("\r\n>>> WARNING: Chinese comma(full-width commas) in the filter <<<\r\n");
            }
            filter = filter.Trim();
            var filterList = filter.Split(',');
            if (filterList.Length < 1)
            {
                return;
            }
            if (print) _console.WriteLine("\r\nYou have set the following protection filter(s):");
            foreach (var ft in filterList)
            {
                if (print) _console.WriteLine($" {ft} ");
                folderFilter.Add(ft);
            }
        }
        private void folderFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            // DONOTHING
        }

        private void regexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (regexCheckBox.Checked)
            {
                this.filterExample.Text = " Using regular expression";
            }
            else
            {
                this.filterExample.Text = " Such as: equal,freedom,Pictures";
            }
            if (filterMode == FilterMode.GEN_FIND)
            {
                if (regexCheckBox.Checked)
                {
                    filterMode = FilterMode.REGEX_FIND;
                }
                
            }
            else if (filterMode == FilterMode.REGEX_PROTECT)
            {
                if (!regexCheckBox.Checked)
                {
                    filterMode = FilterMode.GEN_PROTECT;
                }
                
            }
            else if (filterMode == FilterMode.REGEX_FIND)
            {
                if (!regexCheckBox.Checked)
                {
                    filterMode = FilterMode.GEN_FIND;
                }
                
            }
            else
            {
                if (regexCheckBox.Checked)
                {
                    filterMode = FilterMode.REGEX_FIND;
                }
                
            }
            _console.WriteLine($"\r\n MODE: {filterMode}");
            _cef.ini.UpdateIniItem("FilterMode", filterMode.ToString());

        }
    }
}
