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
using DailyWallpaper.Tools;
using System.Diagnostics;
using System.Collections;
using static DailyWallpaper.Tools.Gemini;
using System.Drawing;
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

        private bool scanRes = false;
        private List<string> folderFilter;
        private string regexFilter;
        private Regex regex;

        private List<string> targetFolder1History = new List<string>();
        private List<string> targetFolder2History = new List<string>();
        private List<string> filesList1;
        private List<string> filesList2;

        private List<GeminiFileStruct> geminiFileStructList1;
        private List<GeminiFileStruct> geminiFileStructList2;
        private List<GeminiFileStruct> sameListNoDup;
        private long minimumFileLimit = 0;
        private List<Task> _tasks = new List<Task>();
        private Mutex _mutex;
        private Mutex _mutexPb;
        private ListViewColumnSorter lvwColumnSorter;
        private List<string> deleteList;

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

            // System.Windows.Forms.TextBox.CheckForIllegalCrossThreadCalls = false;
            // _console.WriteLine(gemini.helpString);


            // init targetfolder 1&2
            targetFolder1TextBox.Text = desktopPath;
            targetFolder2TextBox.Text = "";
            /*var init = gemini.ini.Read("TargetFolder1", "Gemini");
            if (Directory.Exists(init))
            {
                UpdateTextAndIniFile("TargetFolder1", init,
                    targetFolder1History, targetFolder1TextBox, updateIni: false);
            }

            init = gemini.ini.Read("TargetFolder2", "Gemini");
            if (Directory.Exists(init))
            {
                UpdateTextAndIniFile("TargetFolder2", init,
                    targetFolder2History, targetFolder2TextBox, updateIni: false);
            }*/

            btnStop.Enabled = false;
            // default: send to RecycleBin
            deleteOrRecycleBin.Checked = false;
            DeleteOrRecycleBin(deletePermanently: false);

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
            InitSaveLogOrListToFile();
            _mutex = new Mutex();
            _mutexPb = new Mutex();

            // Create an instance of a ListView column sorter and assign it
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            resultListView.ListViewItemSorter = lvwColumnSorter;
        }

        private void InitSaveLogOrListToFile()
        {
            listOrLog.Checked = false;
            saveList2File.Text = "Save log to File";
            listOrLog.Click += (e, s) =>
            {
                if (listOrLog.Checked)
                {
                    saveList2File.Text = "Save list to File";
                }
                else
                {
                    saveList2File.Text = "Save log to File";
                }
            };
            saveList2File.Click += new EventHandler(saveList2File_Click);
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
                targetFolder1History);
        }

        private void btnSelectTargetFolder2_Click(object sender, EventArgs e)
        {
            SelectFolder("TargetFolder2", targetFolder2TextBox,
            targetFolder2History);
        }

        private void SelectFolder(string keyInIni, System.Windows.Forms.TextBox tx,
            List<string> targetFolderHistory)
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
                    if (!UpdateTextAndIniFile(keyInIni, path, targetFolderHistory, tx))
                    {
                        return;
                    }
                    tx.Text = path;
                }
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if(deleteList == null)
            {
                _console.WriteLine($"\r\n!!! You should ANALYZE first.");
                return;
            }

            _console.WriteLine($"\r\n=== You have selected {deleteList.Count} file(s).");
            if (deleteList.Count < 1)
            {
                return;
            }
            /*foreach (var item in deleteList)
            {
                _console.WriteLine("FKU:" + item);
            }*/

        }


        private static string GetTimeStringMsOrS(TimeSpan t)
        {
            string hashCostTime;
            if (t.TotalSeconds > 1)
            {
                hashCostTime = t.TotalSeconds.ToString("f2") + "s";
            }
            else
            {
                hashCostTime = t.TotalMilliseconds.ToString("f3") + "ms";
            }
            return hashCostTime;
        }
        private CompareMode SetCompareMode()
        {
            CompareMode mode = CompareMode.NameAndSize;

            if (fileSHA1CheckBox.Checked)
            {
                mode = CompareMode.SHA1;
            }

            if (fileMD5CheckBox.Checked)
            {
                mode = CompareMode.MD5;
            }

            if (fileExtNameCheckBox.Checked)
            {
                mode = CompareMode.ExtAndSize;
            }

            if (fileNameCheckBox.Checked)
            {
                mode = CompareMode.NameAndSize;
            }

            return mode;
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
            geminiProgressBar.Visible = true;
            var limit = SetMinimumFileLimit();
            try
            {
                var _task = Task.Run(() =>
                {
                    var timer = new Stopwatch();
                    timer.Start();
                        // Get all files from folder1/2
                        bool fld1 = false;
                    bool fld2 = false;
                    var t1 = targetFolder1TextBox.Text;
                    var t2 = targetFolder2TextBox.Text;

                    if (!string.IsNullOrEmpty(t2) && Directory.Exists(t2))
                    {
                        fld2 = true;
                        RecurseScanDir(t2, ref filesList2, token);
                        _console.WriteLine($">>> Found {filesList2.Count} file(s) in: {t2}");
                    }

                    if (!string.IsNullOrEmpty(t1) && Directory.Exists(t1) && !t1.Equals(t2))
                    {
                        fld1 = true;
                        RecurseScanDir(t1, ref filesList1, token);
                        _console.WriteLine($">>> Found {filesList1.Count} file(s) in: {t1}");
                    }

                    if (!fld2 && !fld1)
                    {
                        _console.WriteLine("!!! Two folder invalid.");
                        return;
                    }

                        // get files info exclude HASH.(FASTER) 
                    FileList2GeminiFileStructList(filesList1, ref geminiFileStructList1, token);
                    FileList2GeminiFileStructList(filesList2, ref geminiFileStructList2, token);

                        // compare folders and themselves, return duplicated files list.
                        _console.WriteLine(">>> Start comparing...");
                    CompareMode mode = SetCompareMode();
                    sameListNoDup = ComparerTwoFolderGetList(geminiFileStructList1,
                            geminiFileStructList2, mode, limit, token, geminiProgressBar).Result;
                    _console.WriteLine(">>> Compare finished...");
                        // group by size

                        _console.WriteLine(">>> Show to ListView...");
                    GeminiList2Group(sameListNoDup, token);
                    timer.Stop();
                    string hashCostTime = GetTimeStringMsOrS(timer.Elapsed);
                    _console.WriteLine($">>> Cost time: {hashCostTime}");

                }, _source.Token);

                _tasks.Add(_task);
                await _task;
                // No Error, filesList is usable
                scanRes = true;
            }
            catch (OperationCanceledException e)
            {
                scanRes = false;
                _console.WriteLine($"\r\n>>> OperationCanceledException: {e.Message}");
            }
            catch (AggregateException e)
            {
                _console.WriteLine($"\r\n>>> AggregateException[Cancel exception]: {e.Message}");
            }
            catch (Exception e)
            {
                scanRes = false;
                // _console.WriteLine($"\r\n RecurseScanDir throw exception message: {e.Message}");
                _console.WriteLine($"\r\n RecurseScanDir throw exception message: {e}");
                _console.WriteLine($"\r\n#----^^^  PLEASE CHECK, TRY TO CONTACT ME WITH THIS LOG.  ^^^----#");
            }
            finally
            {
                geminiProgressBar.Visible = false;
                _console.WriteLine(">>> Analyse is over.");
            }
            btnAnalyze.Enabled = true;

        }

        private async Task<List<GeminiFileStruct>> ComparerTwoFolderGetList(List<GeminiFileStruct> l1,
            List<GeminiFileStruct> l2, CompareMode mode, long limit = 0, CancellationToken token = default,
            System.Windows.Forms.ProgressBar pb = null)
        {

            if (ignoreFileCheckBox.Checked)
            {
                var limited =
                    from i in l1
                    where i.size > limit
                    select i;
                l1 = limited.ToList();
            }

            if (ignoreFileCheckBox.Checked)
            {
                var limited =
                    from i in l2
                    where i.size > limit
                    select i;
                l2 = limited.ToList();
            }

            var sameList = new List<GeminiFileStruct>();
            void retAction(bool res, List<GeminiFileStruct> ret)
            {
                _mutex.WaitOne();
                if (res && ret.Count > 1)
                {
                    sameList.AddRange(ret);
                }
                _mutex.ReleaseMutex();
            }
            var cnt1 = l1.Count;
            var cnt2 = l2.Count;
            long totalCmpCnt = cnt1 * cnt1 + cnt1 * cnt2 + cnt2 * cnt2;

            double percent = 0.0;
            void ProgressAction(long i) // percent in file.
            {
                // FIX ERROR: System.InvalidOperationException
                _mutexPb.WaitOne();
                percent += (double)i / totalCmpCnt * 100;
                var percentInt = (int)percent;
                if (percentInt > 99)
                    percentInt = 100;
/*                if (pb.IsHandleCreated)
                {
                    pb.Invoke(new Action(() =>
                    {*/
                        // pb.Value = percentInt;
                        SetProgressMessage(percentInt, pb);

/*                    }));
                }*/
                _mutexPb.ReleaseMutex();
            }
            var totalProgess = new Progress<long>(ProgressAction);
            await Task.Run(async () => await ComparerTwoList(l1,
                l1, mode, token, retAction, totalProgess));
            await Task.Run(async () => await ComparerTwoList(l2,
                l2, mode, token, retAction, totalProgess));
            await Task.Run(async () => await ComparerTwoList(l1,
                l2, mode, token, retAction, totalProgess));


            return sameList.Distinct().ToList();
        }


        private delegate void AddItemToListViewCallback(GeminiFileStruct gf, Color c);
        private void AddItemToListView(GeminiFileStruct gf, Color c)
        {
            if (InvokeRequired)
            {
                var f = new AddItemToListViewCallback(AddItemToListView);
                Invoke(f, new object[] { gf, c });
            }
            else
            {
                var item = new System.Windows.Forms.ListViewItem(gf.name);
                item.BackColor = c;
                AddSubItem(item, "lastMtime", gf.lastMtime);
                AddSubItem(item, "extName", gf.extName);
                AddSubItem(item, "sizeStr", gf.sizeStr);
                AddSubItem(item, "fullPath", gf.fullPath);
                resultListView.Items.Add(item);
            }
        }
        public static void AddSubItem(System.Windows.Forms.ListViewItem i, string name, string text)
        {
            i.SubItems.Add(new System.Windows.Forms.ListViewItem.ListViewSubItem() 
                { Name = name, Text = text });
        }

        private void AddGroupTitleToListView()
        {

        }
        private void GeminiList2Group(List<GeminiFileStruct> listNoDup,
            CancellationToken token, bool printToCons = false)
        {
            if (listNoDup.Count > 0)
            {
                SetText($"Summay: Found {listNoDup.Count:N0} duplicate files.", summaryTextBox);
                // same size to one group
                // check HASH before.
                var duplicateGrp =
                    from i in listNoDup
                    orderby i.fullPath // descending
                    group i by i.size into grp
                    where grp.Count() > 1
                    select grp;
                int index = 0;
                int j = 0;
                resultListView.Items.Clear();
                foreach (var item in duplicateGrp)
                {
                    index++;
                    if (printToCons) _console.WriteLine($"\r\n>>>>> Group Size[{index}] {item.Key}");
                    // update Group name

                    /*var groupHash =
                        from i in item
                        select i;*/
                    var ext =
                        from i in item
                        group i by i.extName into grp
                        select grp;
                    foreach (var it in ext)
                    {
                        j++;
                        if (printToCons) _console.WriteLine($">>> Group Ext[{j}] {it.Key}");
                        var color = Color.White;
                        if (j % 2 == 1)
                        {
                            color = Color.FromArgb(250, 234, 192);
                        }
                        foreach (var t in it)
                        {
                            if (token.IsCancellationRequested)
                            {
                                token.ThrowIfCancellationRequested();
                            }
                            if (printToCons) _console.WriteLine("<file>\r\n" + t + "\r\n</file>\r\n");
                            AddItemToListView(t, color);
                        }
                        if (printToCons) _console.WriteLine($">>> Group Ext[{j}] {it.Key}");
                        // update list name
                    }
                    if (printToCons) _console.WriteLine($">>> Group Size[{index}] {item.Key}\r\n");
                }

                _console.WriteLine($">>> Summay: Found {listNoDup.Count:n0} duplicate files.");
            }
            else
            {
                // summaryTextBox.Text = ;
                SetText($">>> Summay: Found No duplicate files.", summaryTextBox);
            }

        }

        delegate void SetTextCallBack(string text, System.Windows.Forms.TextBox tb);
        private void SetText(string text, System.Windows.Forms.TextBox tb)
        {
            if (summaryTextBox.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText);
                Invoke(stcb, new object[] { text, tb });
            }
            else
            {
                tb.Text = text;
            }
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            btnClear.PerformClick();
            resultListView.Items.Clear();
            geminiProgressBar.Visible = true;
            deleteList = new List<string>();
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
            summaryTextBox.Text = "";
            geminiProgressBar.Value = 0;
        }

        private static bool allChecked = false;
        private void btnSelectAllOrNot_Click(object sender, EventArgs e)
        {
            if (resultListView.Items.Count > 1)
            {
                if (allChecked)
                {
                    foreach (var item in resultListView.Items)
                    {
                        ((System.Windows.Forms.ListViewItem)item).Checked = false;
                    }
                    allChecked = false;
                }
                else
                {
                    foreach (var item in resultListView.Items)
                    {
                        ((System.Windows.Forms.ListViewItem)item).Checked = true;
                    }
                    allChecked = true;
                }
            }
        }

        // Thanks to João Angelo
        // https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
        /// <summary>
        /// TODO: When folder To much, try to not use Recurse
        /// </summary>
        private void RecurseScanDir(string path, ref List<string> filesList, CancellationToken token)
        {
            //token.ThrowIfCancellationRequested();
            // DO NOT KNOW WHY D: DOESNOT WORK WHILE D:\ WORK.
            filesList = new List<string>();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                _console.WriteLine("Invalid directory path: {0}", path);
                return;
            }

            // _console.WriteLine($"\r\n#---- Started Analyze Operation ----#\r\n");
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

        void FileList2GeminiFileStructList(List<string> filesList,
            ref List<GeminiFileStruct> gList, CancellationToken token)
        {
            gList = new List<GeminiFileStruct>();
            if (filesList.Count > 0)
            {
                _console.WriteLine(">>> Start collecting all files...");
                foreach (var f in filesList)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    gList.Add(Gemini.FillGeminiFileStruct(f));
                }
                _console.WriteLine(">>> All files collected.");
            }
        }

        private void FindFilesInDir(string dir, List<string> filesList, CancellationToken token)
        {
            try
            {
                foreach (var fi in Directory.EnumerateFiles(dir))
                {
                    filesList.Add(fi);
                    // _console.WriteLine($"print >>>  {fi}");
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
                    if (d.ToLower().Contains("$RECYCLE.BIN".ToLower()))
                    {
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
            if (gemini.IsControlledFolder(path))
            {
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

        private bool UpdateTextAndIniFile(string keyInIni, string path,
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
                    if (!UpdateTextAndIniFile("TargetFolder1", path, targetFolder1History))
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
                    if (!UpdateTextAndIniFile("TargetFolder2", path, targetFolder2History))
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
            }
            else
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
            if (!listOrLog.Checked && tbConsole.Text.Length < 1)
            {
                return;
            }

            using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {

                var saveDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "LOG");
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                saveFileDialog.InitialDirectory = saveDir;
                saveFileDialog.Filter = "Txt files (*.txt)|*.txt";
                // saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                string t1;
                string t2;
                var f1 = targetFolder1TextBox.Text;
                var f2 = targetFolder2TextBox.Text;
                if (string.IsNullOrEmpty(f1) || !Directory.Exists(f1))
                {
                    t1 = "NONE";
                }
                else
                {
                    t1 = new DirectoryInfo(f1).Name;
                }
                if (string.IsNullOrEmpty(f2) || !Directory.Exists(f2))
                {
                    t2 = "NONE";
                }
                else
                {
                    t2 = new DirectoryInfo(f2).Name;
                }

                var name = t1 + "-" + t2;
                // E:, D: -> D-Disk
                // need TEST here
                name = name.Replace(":", "_");
                if (listOrLog.Checked)
                {
                    saveFileDialog.FileName = "Gemini-List_" + name + "_" +
                                         DateTime.Now.ToString("yyyy-MM-dd_HH-mm"); //+ ".txt"
                }
                else
                {
                    saveFileDialog.FileName = "Gemini-Log_" + name + "_" +
                                         DateTime.Now.ToString("yyyy-MM-dd_HH-mm"); //+ ".txt"
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

        private bool SetFolderFilter(string text, bool print = false)
        {
            _console.WriteLine($">>> Using: {filterMode}");
            string filter = text;
            folderFilter = new List<string>();
            if (string.IsNullOrEmpty(filter))
            {
                regexFilter = "";
                regex = null;
                _console.WriteLine(">>> But there is no valid filter value.");
                return true;
            }
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
            targetFolder_DragDrop(sender, e, "TargetFolder1", targetFolder1History,
                targetFolder1TextBox);
        }
        private void targetFolder2_DragDrop(object sender, DragEventArgs e)
        {
            targetFolder_DragDrop(sender, e, "TargetFolder2", targetFolder2History,
                targetFolder2TextBox);
        }

        private void targetFolder_DragDrop(object sender, DragEventArgs e, string keyInIni,
            List<string> targetFolderHistory, System.Windows.Forms.TextBox tx)
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
                            UpdateTextAndIniFile(keyInIni, path,
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

        private void InitFileSameMode()
        {
            // FIX CFG
            fileSizeCheckBox.Checked = true;
            fileSizeCheckBox.Enabled = false;

            // if no ini, just filename and filesize.
            fileNameCheckBox.Checked = true;
            fileExtNameCheckBox.Checked = false;
            fileMD5CheckBox.Checked = false;
            fileSHA1CheckBox.Checked = false;

            ReadFileSameModeFromIni("SameFileName", fileNameCheckBox);
            ReadFileSameModeFromIni("SameFileExtName", fileExtNameCheckBox);
            ReadFileSameModeFromIni("SameFileMD5", fileMD5CheckBox);
            ReadFileSameModeFromIni("SameFileSHA1", fileSHA1CheckBox);

            fileNameCheckBox.Click += fileNameCheckBox_Click;
            fileExtNameCheckBox.Click += fileExtNameCheckBox_Click;
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

            if (gemini.ini.EqualsIgnoreCase("ignoreFileEnabled", "true", "Gemini"))
            {
                ignoreFileCheckBox.Checked = true;
            }

            if (int.TryParse(gemini.ini.Read("ignoreFileIndex", "Gemini"), out int retIndex))
            {
                if (int.TryParse(gemini.ini.Read("ignoreFileTextBox", "Gemini"), out int retNum))
                {
                    minimumFileLimit = retNum * 1024 ^ retIndex;
                    ignoreFileSizecomboBox.SelectedIndex = retIndex;
                    ignoreFileSizeTextBox.Text = retNum.ToString();
                }
            }
            ignoreFileSizeTextBox.Enabled = ignoreFileCheckBox.Checked;

            ignoreFileCheckBox.Click += new EventHandler(ignoreFileCheckBox_Click);
            ignoreFileSizecomboBox.SelectedIndexChanged += new EventHandler(ignoreFileSizecomboBox_SelectedIndexChanged);
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
        private void FileSameModeClick(string key, System.Windows.Forms.CheckBox cb, 
            string conflictKey = null, System.Windows.Forms.CheckBox cbConflict = null)
        {
            if (cb.Checked)
            {
                cbConflict.Checked = false;
            }
            else
            {

            }
            gemini.ini.UpdateIniItem(key, cb.Checked.ToString(), "Gemini");
            gemini.ini.UpdateIniItem(conflictKey, cbConflict.Checked.ToString(), "Gemini");
        }

        private void fileNameCheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileName", fileNameCheckBox, "SameFileExtName", fileExtNameCheckBox);
        }

        private void fileExtNameCheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileExtName", fileExtNameCheckBox, "SameFileName", fileNameCheckBox);
        }

        private void fileMD5CheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileMD5", fileMD5CheckBox, "SameFileSHA1", fileSHA1CheckBox);
        }

        private void fileSHA1CheckBox_Click(object sender, EventArgs e)
        {
            FileSameModeClick("SameFileSHA1", fileSHA1CheckBox, "SameFileMD5", fileMD5CheckBox);
        }


        private void ignoreFileSizecomboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetMinimumFileLimit();
        }

        private long SetMinimumFileLimit()
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
            return minimumFileLimit;
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

        private void alwaysOnTopCheckBox_Click(object sender, EventArgs e)
        {
            //if (alwaysOnTopCheckBox.Checked)
            if (alwaysOnTopCheckBox.Checked)
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
        }
        private void resultListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // https://stackoverflow.com/questions/17746013/how-to-change-order-of-columns-of-listview
        // https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/sort-listview-by-column
        private void resultListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine if clicked column is already the column that is being sorted.
            if (e.Column == lvwColumnSorter.SortColumn)
            {
                // Reverse the current sort direction for this column.
                if (lvwColumnSorter.Order == SortOrder.Ascending)
                {
                    lvwColumnSorter.Order = SortOrder.Descending;
                }
                else
                {
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }
            }
            else
            {
                // Set the column number that is to be sorted; default to ascending.
                lvwColumnSorter.SortColumn = e.Column;
                lvwColumnSorter.Order = SortOrder.Ascending;
            }

            // Perform the sort with these new sort options.
            resultListView.Sort();
        }

        private delegate void DelSetPro(int pro, System.Windows.Forms.ProgressBar proBar);
        private void SetProgressMessage(int pro, System.Windows.Forms.ProgressBar proBar)
        {
            if (InvokeRequired)
            {
                if (proBar.IsHandleCreated)
                {
                    DelSetPro setPro = new DelSetPro(SetProgressMessage);
                    Invoke(setPro, new object[] { pro, proBar });
                }
            }
            else
            {
                proBar.Value = Convert.ToInt32(pro);
            }
        }

        private void geminiProgressBar_Click(object sender, EventArgs e)
        {

        }

        private void resultListView_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Handles the ItemCheck event. The method uses the CurrentValue
            // property of the ItemCheckEventArgs to retrieve and tally the  
            // price of the menu items selected.  

            var txet = resultListView.Items[e.Index].SubItems["fullPath"].Text;
            if (e.CurrentValue == CheckState.Unchecked)
            {
                deleteList.Add(txet);
                // _console.WriteLine($"Add {txet}");
            }
            else if ((e.CurrentValue == CheckState.Checked))
            {
                try
                {
                    deleteList.Remove(txet);
                }
                catch
                {
                    _console.WriteLine("NO EXIT");
                }
                // _console.WriteLine($"Remove {txet}");
            }
        }
    }
}