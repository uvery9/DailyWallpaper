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
using System.Windows.Controls;
using static DailyWallpaper.Gemini;
// using System.Linq;

namespace DailyWallpaper
{
    public partial class GeminiForm : Form, IDisposable
    {
        private Gemini gemini;
        private TextBoxCons _console;
        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private CancellationTokenSource _source = null;
        private bool deletePermanently = false;

        // Speed up the next scan
        private string analyzePath = null;
        private bool scanRes = false;
        private List<string> folderFilter;
        private string regexFilter;
        private Regex regex;
        
        private List<string> targetFolder1History = new List<string>();
        private List<string> targetFolder2History = new List<string>();
        private string targetFolder1 = null;
        private string targetFolder2 = null;
        private List<string> filesList1;
        private List<string> filesList2;

        private List<GeminiFileStruct> geminiFileStructList1;
        private List<GeminiFileStruct> geminiFileStructList2;
        private long minimumFileLimit = 0;
        private List<Task> _tasks = new List<Task>();

        private enum FilterMode : int
        {
            REGEX_FIND,
            REGEX_PROTECT,
            GEN_FIND,
            GEN_PROTECT,

        }
        private FilterMode filterMode;
        public GeminiForm()
        {
            InitializeComponent();
            targetFolder1TextBox.KeyDown += targetFolder1_KeyDown;
            targetFolder2TextBox.KeyDown += targetFolder2_KeyDown;
            folderFilterTextBox.KeyDown += folderFilterTextBox_KeyDown;
            Icon = Properties.Resources.GE32X32;
            gemini = new Gemini();
            _console = new TextBoxCons(new ConsWriter(tbConsole));
            _console.WriteLine(gemini.helpString);

            
            // init targetfolder 1&2
            targetFolder1TextBox.Text = desktopPath;
            targetFolder2TextBox.Text = "";
            var init = gemini.ini.Read("TargetFolder1", "Gemini");
            if (Directory.Exists(init))
            {
                UpdateTextAndIniFile("TargetFolder1", init, ref targetFolder1, 
                    targetFolder1History, targetFolder1TextBox, updateIni: false); 
            }

            init = gemini.ini.Read("TargetFolder2", "Gemini");
            if (Directory.Exists(init))
            {
                UpdateTextAndIniFile("TargetFolder2", init, ref targetFolder2,
                    targetFolder2History, targetFolder2TextBox, updateIni: false);
            }

            btnStop.Enabled = false;
            // default: send to RecycleBin
            deleteOrRecycleBin.Checked = false;
            DeleteOrRecycleBin(deletePermanently: false);
            listOrLog.Checked = true;
            folderFilter = new List<string>();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            SetUpFilterMode();
            regexCheckBox.CheckedChanged += new EventHandler(regexCheckBox_CheckedChanged);
            // _console.WriteLine("You could always TYPE help in folder filter textbox and press ENTER.");
            InitFileSameMode();
            filesList1 = new List<string>();
            filesList2 = new List<string>();
            geminiFileStructList1 = new List<GeminiFileStruct>();
            geminiFileStructList2 = new List<GeminiFileStruct>();
        }
        /// <summary>
        /// bind to tbTargetFolderHistory
        /// </summary>
        private void SetUpFilterMode()
        {
            var fimode = gemini.ini.Read("FilterMode");
            if (!string.IsNullOrEmpty(fimode))
            {
                if (fimode.Equals("GEN_FIND"))
                {
                    regexCheckBox.Checked = false;
                    modeCheckBox.Checked = true;
                    filterMode = FilterMode.GEN_FIND;
                }
                else if (fimode.Equals("REGEX_PROTECT"))
                {
                    regexCheckBox.Checked = true;
                    modeCheckBox.Checked = false;
                    filterMode = FilterMode.REGEX_PROTECT;
                }
                else if (fimode.Equals("REGEX_FIND"))
                {
                    regexCheckBox.Checked = true;
                    modeCheckBox.Checked = true;
                    filterMode = FilterMode.REGEX_FIND;
                }
                else
                {
                    regexCheckBox.Checked = false;
                    modeCheckBox.Checked = false;
                    filterMode = FilterMode.GEN_PROTECT;
                }
            }
            _console.WriteLine($"\r\n Start with FilterMode: {filterMode}");
            UpdateFilterExampleText(filterMode);
        }
        private void BindHistory(System.Windows.Forms.TextBox tb, List<string> list)
        {
            tb.AutoCompleteCustomSource.Clear();
            tb.AutoCompleteCustomSource.AddRange(list.ToArray());
            tb.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            tb.AutoCompleteSource = AutoCompleteSource.CustomSource;
        }
        private void btnSelectTargetFolder1_Click(object sender, EventArgs e)
        {
            SelectFolder("TargetFolder1", targetFolder1TextBox,
            ref targetFolder1, targetFolder1History);
        }

        private void btnSelectTargetFolder2_Click(object sender, EventArgs e)
        {
            SelectFolder("TargetFolder2", targetFolder2TextBox,
            ref targetFolder2, targetFolder2History);
        }

        private void SelectFolder(string keyInIni, System.Windows.Forms.TextBox tx, 
            ref string targetFolder, List<string> targetFolderHistory)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                if (Directory.Exists(tx.Text))
                {
                    dialog.InitialDirectory = tx.Text;
                }
                else
                {
                    dialog.InitialDirectory = desktopPath;
                }
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Select Target Folders";

                // maybe add some log
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    var path = dialog.FileName;
                    if (!UpdateTextAndIniFile(keyInIni, path, ref targetFolder, targetFolderHistory, tx))
                    {
                        return;
                    }
                    tx.Text = path;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteSlected();
        }

        private void DeleteSlected()
        {

        }

        private async void StartAnalyze(bool delete = false)
        {
            if (!SetFolderFilter(folderFilterTextBox.Text, print: true))
            {
                return;
            }
            _source = new CancellationTokenSource();
            var token = _source.Token;
            btnStop.Enabled = true;
            btnAnalyze.Enabled = false;
            
            var _task = Task.Run(() =>
            {
                RecurseScanDir(targetFolder1TextBox.Text, filesList1, token);
                _console.WriteLine();
                RecurseScanDir(targetFolder2TextBox.Text, filesList2, token);
                FileList2GeminiFileStructList(filesList1, geminiFileStructList1, token);
                FileList2GeminiFileStructList(filesList2, geminiFileStructList2, token);
                foreach(var gs in geminiFileStructList1)
                {
                    _console.WriteLine(gs.ToString());
                    _console.WriteLine();
                }
                geminiFileStructList2 =  Gemini.ForceGetHashGeminiFileStructList(geminiFileStructList2, token,
                    fileSHA1CheckBox.Checked, fileMD5CheckBox.Checked).Result;
                foreach (var gs in geminiFileStructList2)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    _console.WriteLine(gs.ToString());
                    _console.WriteLine();
                }
            }, _source.Token);
            try
            {
                _tasks.Add(_task);
                await _task;
                // No Error, filesList is usable
                scanRes = true;
            }
            catch (OperationCanceledException e)
            {
                scanRes = false;
                _console.WriteLine($"\r\n>>> RecurseScanDir throw exception message: {e.Message}");
            }
            catch (Exception e)
            {
                scanRes = false;
                _console.WriteLine($"\r\n RecurseScanDir throw exception message: {e.Message}");
                _console.WriteLine($"\r\n#----^^^  PLEASE CHECK, TRY TO CONTACT ME WITH THIS LOG.  ^^^----#");
            }
            finally
            {
                _console.WriteLine(">>> Analyse is over.");
            }
            btnAnalyze.Enabled = true;

        }
        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            SetMinimumFileLimit();
            StartAnalyze();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_source != null)
            {
                _source.Cancel();
                //_source.Dispose();
                //_source = null;
            }
            _console.WriteLine("Stop...");
            btnAnalyze.Enabled = true;
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
        private void RecurseScanDir(string path, List<string> filesList, CancellationToken token)
        {
            //token.ThrowIfCancellationRequested();
            // DO NOT KNOW WHY D: DOESNOT WORK WHILE D:\ WORK.
            if (!Directory.Exists(path))
            {
                _console.WriteLine("Invalid directory path: {0}", path);
                return;
            }

            analyzePath = path;

            _console.WriteLine($"\r\n#---- Started Analyze Operation ----#\r\n");
            if (scanRes && filesList.Count > 0)
            {
                foreach (var folder in filesList)
                {
                    _console.WriteLine($"found ###  {folder}");
                }
                return;
            }
            if (filterMode == FilterMode.GEN_FIND && folderFilter.Count > 0)
            {
                FindFilesWithFindMode(path, filesList, token, re: false);
            }
            else if (filterMode == FilterMode.REGEX_FIND && regex != null)
            {
                FindFilesWithFindMode(path, filesList, token, re: true);
            }
            else
            {
                FindFilesWithProtectMode(path, filesList, token);
            }
            _console.WriteLine($"\r\n#---- Finished Analyze Operation ----#\r\n");
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
            if (mode == FilterMode.REGEX_PROTECT)
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
            if (mode == FilterMode.REGEX_FIND)
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

        /// <summary>
        /// TEST CASE: 
        /// 1)games 2)D:\games 3)games,Steam\logs 4)D:\games,Steam\logs
        /// </summary>

        private void FindFilesWithFindMode(string path, List<string> filesList, CancellationToken token, bool re = false)
        {
            if (String.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Starting directory is a null reference or an empty string: path");
            }
            try
            {
                
                foreach (var d in Directory.EnumerateDirectories(path))
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    // FUCK THE $RECYCLE.BIN
                    if (d.ToLower().Contains("$RECYCLE.BIN".ToLower()))
                    {
                        continue;
                    }
                    FindFilesWithFindMode(d, filesList, token, re);
                }
                if (re)
                {
                    if (regex.IsMatch(path))
                    {
                        FindFilesInDir(path, filesList, token);
                        return;
                    }
                }
                else
                {
                    foreach (var filter in folderFilter)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        if (path.Contains(filter))
                        {
                            FindFilesInDir(path, filesList, token);
                            continue;
                        }
                    }
                    return;
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (Exception)
            {
                throw;
            }
        }
        //calcSHA1: fileSHA1CheckBox.Checked,
        // calcMD5: fileMD5CheckBox.Checked

        void FileList2GeminiFileStructList(List<string> filesList, List<GeminiFileStruct> gList, CancellationToken token)
        {
            if (filesList.Count > 0)
            {
                _console.WriteLine(">>> Start to analyze files.");
                foreach (var f in filesList)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    gList.Add(Gemini.FillGeminiFileStruct(f));
                }
                _console.WriteLine(">>> Analyse finished.");
            }
            else
            {
                _console.WriteLine("No files.");
            }
        }

        private void FindFilesInDir(string dir, List<string> filesList, CancellationToken token)
        {
            try
            {
                foreach (var fi in Directory.EnumerateFiles(dir))
                {
                    filesList.Add(fi);
                    _console.WriteLine($"print >>>  {fi}");
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (DirectoryNotFoundException) { }
        }

        private void FindFilesWithProtectMode(string dir, List<string> filesList, CancellationToken token)
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
                    FindFilesWithProtectMode(d, filesList, token);
                }
                FindFilesInDir(dir, filesList, token);
            }
            catch (UnauthorizedAccessException) { }
        }

        /// <summary>
        /// check if Controlled Or NotExist
        /// </summary>
        private bool IsControlled(string path, bool print = true)
        {
            if (gemini.IsControlledFolder(path)){
                if (print)
                {
                    _console.WriteLine($"\r\nThe folder is CONTROLLED, please re-select:\r\n   {path}");
                    _console.WriteLine("\r\nYou could Type \" list controlled \" in the \r\n" + 
                        "\"Folder Filter\" and Type ENTER" +
                        " to see all the controlled folders.");
                }
                return true;
            }
            return false;
        }
        
        private bool UpdateTextAndIniFile(string keyInIni, string path, ref string targetFolder, 
            List<string> targetFolderHistory, System.Windows.Forms.TextBox tx = null,
            bool updateIni = true, bool print = true)
        {
            if (IsControlled(path))
            {
                return false;
            }
            if (!Directory.Exists(path))
            {
                if (print)
                {
                    _console.WriteLine($"\r\nThe {keyInIni} folder dose NOT EXIST, please re-select:\r\n   {path}");
                }
                return false;
            }
            // DirectoryIn
            path = Path.GetFullPath(path);
            if (tx != null)
            {
                tx.Text = path;
                targetFolderHistory.Add(path);
                BindHistory(tx, targetFolderHistory);
            }
            targetFolder = path;
            if (updateIni)
            {
                gemini.ini.UpdateIniItem(keyInIni, path, "Gemini");
            }
            if (print)
            {
                _console.WriteLine($"\r\nYou have selected {keyInIni} folder:\r\n  {path}");
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
                if (sender is System.Windows.Forms.TextBox box)
                {
                    IsCmdInTextBox(folderFilterTextBox, box.Text);
                }
            }
        }

        private void UpdateREAndModeCheckBox(FilterMode mode)
        {
            if (mode == FilterMode.GEN_FIND)
            {
                regexCheckBox.Checked = false;
                modeCheckBox.Checked = true;
            }
            if (mode == FilterMode.GEN_PROTECT)
            {
                regexCheckBox.Checked = false;
                modeCheckBox.Checked = false;
            }
            if (mode == FilterMode.REGEX_FIND)
            {
                regexCheckBox.Checked = true;
                modeCheckBox.Checked = true;
            }
            if (mode == FilterMode.REGEX_PROTECT)
            {
                regexCheckBox.Checked = true;
                modeCheckBox.Checked = false;
            }
        }
        private void UpdateIniAndTextBox()
        {    
            _console.WriteLine($"\r\n >>> FilterMode: {filterMode}");
            UpdateFilterExampleText(filterMode);
            gemini.ini.UpdateIniItem("FilterMode", filterMode.ToString());
        }
        private bool IsCmdInTextBox(System.Windows.Forms.TextBox box, string cmd)
        {
            cmd = cmd.Trim();

            // command mode
            bool useCommand = false;
            
            if (cmd.ToLower().Equals("list controlled"))
            {
                _console.WriteLine("\r\nThe following is a list of controlled folders:");
                foreach (var f in gemini.GetAllControlledFolders())
                {
                    _console.WriteLine(f);
                }
                useCommand = true;
            }
            if (cmd.ToLower().Equals("find"))
            {
                useCommand = true;
                FilterMode fimode = filterMode;
                if (regexCheckBox.Checked)
                {
                    if (filterMode == FilterMode.REGEX_FIND)
                    {
                        fimode = FilterMode.REGEX_PROTECT;
                    }
                    if (filterMode == FilterMode.REGEX_PROTECT)
                    {
                        fimode = FilterMode.REGEX_FIND;
                    }
                }
                else
                {
                    if (filterMode == FilterMode.GEN_FIND)
                    {
                        fimode = FilterMode.GEN_PROTECT;
                    }
                    if (filterMode == FilterMode.GEN_PROTECT)
                    {
                        fimode = FilterMode.GEN_FIND;
                    }
                }
                filterMode = fimode;
                UpdateREAndModeCheckBox(filterMode);
            }
            if (cmd.ToLower().Equals("re"))
            {
                useCommand = true;
                FilterMode fimode = filterMode;
                if (modeCheckBox.Checked)
                {
                    if (filterMode == FilterMode.REGEX_FIND)
                    {
                        fimode = FilterMode.GEN_FIND;
                    }
                    if (filterMode == FilterMode.GEN_FIND)
                    {
                        fimode = FilterMode.REGEX_FIND;
                    }
                }
                else
                {
                    if (filterMode == FilterMode.GEN_PROTECT)
                    {
                        fimode = FilterMode.REGEX_PROTECT;
                    }
                    if (filterMode == FilterMode.REGEX_PROTECT)
                    {
                        fimode = FilterMode.GEN_PROTECT;
                    }
                }
                filterMode = fimode;
                UpdateREAndModeCheckBox(filterMode);
            }
            if (cmd.ToLower().Equals("help"))
            {
                _console.WriteLine(gemini.helpString);
                useCommand = true;
            }

            // recover
            if (useCommand)
            {
                box.Text = "";
                return true;
            }
            return false;
        }
        private void targetFolder1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is System.Windows.Forms.TextBox box)
                {
                    var path = box.Text;
                    path = path.Trim();
                    if (!UpdateTextAndIniFile("TargetFolder1", path, ref targetFolder1, targetFolder1History))
                    {
                        return;
                    }
                }
            }
        }

        private void targetFolder2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is System.Windows.Forms.TextBox box)
                {
                    var path = box.Text;
                    path = path.Trim();
                    if (!UpdateTextAndIniFile("TargetFolder2", path, ref targetFolder2, targetFolder2History))
                    {
                        return;
                    }
                }
            }
        }

        private void DeleteOrRecycleBin(bool deletePermanently = false)
        {
            if (!deletePermanently)
            {
                btnDelete.Text = "RecycleBin";
            } else
            {
                btnDelete.Text = "Delete Permanently";
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

            if (listOrLog.Checked && (filesList1.Count < 1 || filesList2.Count < 1))
            {
                _console.WriteLine("You SHOULD scan one folder first.");
                return;
            }
            if (listOrLog.Checked && !Directory.Exists(targetFolder1))
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
                var name = new DirectoryInfo(targetFolder1).Name + "-" +
                    new DirectoryInfo(targetFolder2).Name;
                
                // E:, D: -> D-Disk
                // need TEST here
                if (name.Contains(":"))
                {
                    name = name.Split(':')[0] + "-Disk";
                }
                if (listOrLog.Checked)
                {
                    saveFileDialog.FileName = "Gemini-List_" + name + "_" +
                                         DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); //+ ".txt"
                } else
                {
                    saveFileDialog.FileName = "Gemini-Log_" + name + "_" +
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
                            dataAsBytes = filesList1.SelectMany(s =>
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

        private bool SetFolderFilter(string text, bool print = false)
        {
            _console.WriteLine($">>> Using: {filterMode}");
            string filter = text;
            if (string.IsNullOrEmpty(filter))
            {
                folderFilter = new List<string>();
                regexFilter = "";
                regex = null;
                _console.WriteLine(">>> But there is no valid filter value.");
                return true;
            }
            folderFilter = new List<string>();
            regexFilter = "";
            if (regexCheckBox.Checked)
            {
                regexFilter = filter;
                try
                {
                    regex = new Regex(regexFilter, RegexOptions.IgnoreCase | RegexOptions.Compiled);
                }
                catch (Exception e)
                {
                    _console.WriteLine($"\r\n!!! filter ERROR: {regexFilter} illegal");
                    _console.WriteLine($"\r\n!!! ERROR: {e.Message}");
                    regex = null;
                    return false;
                } 
                if (print)
                {
                    _console.WriteLine($"\r\nYou have set the regex filter: \" {regexFilter} \"");
                }
                return true;
            }
            if (filter.Contains("，"))
            {
                if (print) _console.WriteLine("\r\n>>> WARNING: Chinese comma(full-width commas) in the filter <<<\r\n");
            }
            filter = filter.Trim();
            var filterList = filter.Split(',');
            if (filterList.Length < 1)
            {
                return false;
            }
            if (print) _console.WriteLine("\r\nYou have set the following general filter(s):");
            foreach (var ft in filterList)
            {
                if (print) _console.WriteLine($" {ft} ");
                folderFilter.Add(ft);
            }
            return true;
        }
        private void folderFilterTextBox_TextChanged(object sender, EventArgs e)
        {
            // DONOTHING
        }

        private void UpdateFilterExampleText(FilterMode mode)
        {
            if (mode == FilterMode.GEN_FIND)
            {
                filterExample.Text = " Using General Find mode";
            }
            if (mode == FilterMode.GEN_PROTECT)
            {
                filterExample.Text = " Using General Protect mode";
            }
            if (mode == FilterMode.REGEX_FIND)
            {
                filterExample.Text = " Using Regex Find mode";
            }
            if (mode == FilterMode.REGEX_PROTECT)
            {
                filterExample.Text = " Using Regex Protect mode";
            }
        }
        private void regexCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (regexCheckBox.Checked)
            {
                if (filterMode == FilterMode.GEN_FIND)
                {
                    filterMode = FilterMode.REGEX_FIND;
                }
                if (filterMode == FilterMode.GEN_PROTECT)
                {
                    filterMode = FilterMode.REGEX_PROTECT;
                }
            }
            else
            {
                if (filterMode == FilterMode.REGEX_PROTECT)
                {
                    filterMode = FilterMode.GEN_PROTECT;
                }
                if (filterMode == FilterMode.REGEX_FIND)
                {
                    filterMode = FilterMode.GEN_FIND;
                }
            }
            UpdateIniAndTextBox();

        }

        private void modeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (modeCheckBox.Checked)
            {
                if (filterMode == FilterMode.GEN_PROTECT)
                {
                    filterMode = FilterMode.GEN_FIND;
                }
                if (filterMode == FilterMode.REGEX_PROTECT)
                {
                    filterMode = FilterMode.REGEX_FIND;
                }
            }
            else
            {
                if (filterMode == FilterMode.GEN_FIND)
                {
                    filterMode = FilterMode.GEN_PROTECT;
                }
                if (filterMode == FilterMode.REGEX_FIND)
                {
                    filterMode = FilterMode.REGEX_PROTECT;
                }
            }
            UpdateIniAndTextBox();
        }

        private void targetFolder1_DragDrop(object sender, DragEventArgs e)
        {
            targetFolder_DragDrop(sender, e, "TargetFolder1", ref targetFolder1, targetFolder1History,
                targetFolder1TextBox);
        }
        private void targetFolder2_DragDrop(object sender, DragEventArgs e)
        {
            targetFolder_DragDrop(sender, e, "TargetFolder2", ref targetFolder2, targetFolder2History,
                targetFolder2TextBox);
        }

        private void targetFolder_DragDrop(object sender, DragEventArgs e, string keyInIni,
            ref string targetFolder, List<string> targetFolderHistory, System.Windows.Forms.TextBox tx)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                // DirectoryInfo().
                if (filePaths.Length == 1)
                {
                    var path = filePaths[0];
                    if (Directory.Exists(path))
                    {
                        if (Directory.Exists(path))
                        {
                            UpdateTextAndIniFile(keyInIni, path, ref targetFolder,
                                targetFolderHistory, tx);
                        }
                    }
                }
                else
                {
                    _console.WriteLine("\r\nAttention: File or multiple folders are not allowed!");
                }
                
            }
        }

        private void targetFolder1_2_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void InitFileSameMode()
        {
            // if no ini, just filename and filesize.
            fileNameCheckBox.Checked = true;
            fileSizeCheckBox.Checked = true;
            fileExtNameCheckBox.Checked = false;
            fileMD5CheckBox.Checked = false;
            fileSHA1CheckBox.Checked = false;
            
            ReadFileSameModeFromIni("SameFileName", fileNameCheckBox);
            ReadFileSameModeFromIni("SameFileExtName", fileExtNameCheckBox);
            ReadFileSameModeFromIni("SameFileSize", fileSizeCheckBox);
            ReadFileSameModeFromIni("SameFileMD5", fileMD5CheckBox);
            ReadFileSameModeFromIni("SameFileSHA1", fileSHA1CheckBox);

            fileNameCheckBox.Click += fileNameCheckBox_Click;
            fileExtNameCheckBox.Click += fileExtNameCheckBox_Click;
            fileSizeCheckBox.Click += fileSizeCheckBox_Click;
            fileMD5CheckBox.Click += fileMD5CheckBox_Click;
            fileSHA1CheckBox.Click += fileSHA1CheckBox_Click;

            ReadIgnoreFileFromIni();
        }

        private void ReadIgnoreFileFromIni()
        {
            minimumFileLimit = 1024 * 1024; // 1MB
            ignoreFileSizecomboBox.SelectedIndex = 2;
            ignoreFileSizeTextBox.Text = "1";
            ignoreFileCheckBox.Checked = false;
            ignoreFileSizeTextBox.Enabled = false;

            if (int.TryParse(gemini.ini.Read("ignoreFileIndex", "Gemini"), out int retIndex)){
                if (int.TryParse(gemini.ini.Read("ignoreFileTextBox", "Gemini"), out int retNum))
                {
                    minimumFileLimit = retNum * 1024 ^ retIndex;
                    ignoreFileSizecomboBox.SelectedIndex = retIndex;
                    ignoreFileSizeTextBox.Text = retNum.ToString();
                }
            }
            if (gemini.ini.EqualsIgnoreCase("ignoreFileEnabled", "true", "Gemini"))
            {
                ignoreFileCheckBox.Checked = true;
            }
            ignoreFileSizeTextBox.Enabled = ignoreFileCheckBox.Checked;
        }

        private void ReadFileSameModeFromIni(string key, System.Windows.Forms.CheckBox cb)
        {
            if (gemini.ini.EqualsIgnoreCase(key, "true", "Gemini"))
            {
                cb.Checked = true;
            }

            if (gemini.ini.EqualsIgnoreCase(key, "false", "Gemini"))
            {
                cb.Checked = false;
            }
        }
        private void FileSameModeClick(string key, System.Windows.Forms.CheckBox cb)
        {
            if (cb.Checked)
            {

            }
            else
            {

            }
            gemini.ini.UpdateIniItem(key, cb.Checked.ToString(), "Gemini");
        }
        private void fileNameCheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileName", fileNameCheckBox);
        }

        private void fileExtNameCheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileExtName", fileExtNameCheckBox);
        }

        private void fileSizeCheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileSize", fileSizeCheckBox);
        }

        private void fileMD5CheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileMD5", fileMD5CheckBox);
        }

        private void fileSHA1CheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileSHA1", fileSHA1CheckBox);
        }

        private void ignoreFileSizecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetMinimumFileLimit();
        }

        private void SetMinimumFileLimit()
        {
            minimumFileLimit = 0;
            if (ignoreFileCheckBox.Checked)
            {
                ignoreFileSizeTextBox.Enabled = true;
                if (int.TryParse(ignoreFileSizeTextBox.Text, out int ret))
                {
                    switch (ignoreFileSizecomboBox.SelectedIndex)
                    {
                        case 0:
                            minimumFileLimit = ret * 1;
                            break;
                        case 1:
                            minimumFileLimit = ret * 1024;
                            break;
                        case 2:
                            minimumFileLimit = ret * 1024 * 1024;
                            break;
                        case 3:
                            minimumFileLimit = ret * 1024 * 1024 * 1024;
                            break;
                        default:
                            minimumFileLimit = ret * 1024 * 1024;
                            break;
                    }
                }
            }
            else
            {
                ignoreFileSizeTextBox.Enabled = false;
            }
            gemini.ini.UpdateIniItem("ignoreFileEnabled", ignoreFileCheckBox.Checked.ToString(), "Gemini");
            gemini.ini.UpdateIniItem("ignoreFileIndex", ignoreFileSizecomboBox.SelectedIndex.ToString(), "Gemini");
            gemini.ini.UpdateIniItem("ignoreFileTextBox", ignoreFileSizeTextBox.Text, "Gemini");
        }
        private void ignoreFileCheckBox_Click(object sender, EventArgs e)
        {
            SetMinimumFileLimit();
        }

        private void GeminiForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            targetFolder1TextBox.AllowDrop = false;
            targetFolder2TextBox.AllowDrop = false;

            btnStop.PerformClick();
            btnStop.PerformClick();
            btnStop.PerformClick();

            Hide();

            Task.Run(() =>
            {   
                // MessageBox.Show("Start WaitAll.");
                Task.WaitAll(_tasks.ToArray());
                // MessageBox.Show("finished.");
                e.Cancel = false;
            }
            );
        }
    }
}