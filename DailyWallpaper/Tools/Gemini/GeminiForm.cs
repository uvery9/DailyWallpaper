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
using System.Security.Cryptography;
using System.Reflection;
using ListViewItem = System.Windows.Forms.ListViewItem;
using ListView = System.Windows.Forms.ListView;
// using System.Linq;

namespace DailyWallpaper
{
    public partial class GeminiForm : Form, IDisposable
    {
        private Gemini gemini;
        private TextBoxCons _console;
        private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        private CancellationTokenSource _source = new CancellationTokenSource();
        private CancellationTokenSource _sourceCEF;
        private CancellationTokenSource reIndexTokenSrc = null;
        private CancellationTokenSource hashTokenSrc = null;

        private bool scanRes = false;
        private bool cefScanRes = false;
        private List<string> pathFilter = new List<string>();
        private string regexFilter;
        private Regex regex;
        private int nameColumnHeaderWidth = 0;
        private int modifiedTimeColumnHeaderWidth = 0;
        private List<GeminiCEFCls> geminiCEFClsList = new List<GeminiCEFCls>();

        private List<string> targetFolder1History = new List<string>();
        private List<string> targetFolder2History = new List<string>();

        private long minimumFileLimit = 0;
        private List<Task> _tasks = new List<Task>();
        private Mutex _mutex;
        private Mutex _mutexPb;
        private ListViewColumnSorter lvwColumnSorter;
        private List<string> deleteList = new List<string>();
        private List<GeminiFileCls> geminiFileClsListForLV = new List<GeminiFileCls>();
        private List<GeminiFileCls> geminiFileClsListForLVUndo = new List<GeminiFileCls>();
        private List<GeminiFileCls> geminiFileClsListForLVRedo = new List<GeminiFileCls>();
        private Color themeColor = Color.FromArgb(250, 234, 192);
        private Color themeColorClean = Color.ForestGreen;
        // private System.Windows.Forms.ToolTip m_lvToolTip = new System.Windows.Forms.ToolTip();
        private enum FilterMode : int
        {
            REGEX_FIND,
            REGEX_PROTECT,
            GEN_FIND,
            GEN_PROTECT,
        }
        private FilterMode filterMode;

        // TODO, F2 rename
        // TODO FIX CLICK-CHECK EVENT UPDATE GFL.
        // https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/use-combobox-edit-listview#verify-that-it-works
        // https://docs.microsoft.com/zh-cn/previous-versions/dotnet/netframework-3.0/ms171728(v=vs.85)?redirectedfrom=MSDN
        
        private void IgnoreThreadingEx()
        {
            CheckForIllegalCrossThreadCalls = false;
        }
        public GeminiForm()
        {
            InitializeComponent();
            targetFolder1TextBox.KeyDown += targetFolder1_KeyDown;
            targetFolder2TextBox.KeyDown += targetFolder2_KeyDown;
            folderFilterTextBox.KeyDown += folderFilterTextBox_KeyDown;
            Icon = Properties.Resources.GE32X32;
            gemini = new Gemini();
            _console = new TextBoxCons(new ConsWriter(tbConsole));

            IgnoreThreadingEx();

            // init targetfolder 1&2
            targetFolder1TextBox.Text = desktopPath;
            targetFolder2TextBox.Text = "";
            var init = gemini.ini.Read("TargetFolder1", "Gemini");
            if (Directory.Exists(init))
            {
                UpdateTextAndIniFile("TargetFolder1", init,
                    targetFolder1History, targetFolder1TextBox, updateIni: false);
            }

            /*init = gemini.ini.Read("TargetFolder2", "Gemini");
            if (Directory.Exists(init))
            {
                UpdateTextAndIniFile("TargetFolder2", init,
                    targetFolder2History, targetFolder2TextBox, updateIni: false);
            }*/

            // default: send to RecycleBin
            deleteOrRecycleBin.Checked = false;
            if (gemini.ini.EqualsIgnoreCase("RecycleBin", "true", "Gemini"))
            {
                deleteOrRecycleBin.Checked = true;
                btnDelete.Text = "Delete";
            }
            // auto delete empty folder after remove.
            autocleanEmptyFoldersToolStripMenuItem.Checked = true;
            notProtectFilesInGrpToolStripMenuItem.Checked = false;
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;

            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            SetUpFilterModeAndRegClick();
            // CWriteLine("You could always TYPE help in folder filter textbox and press ENTER.");
            InitFileSameMode();
            _mutex = new Mutex();
            _mutexPb = new Mutex();

            // Create an instance of a ListView column sorter and assign it
            // to the ListView control.
            lvwColumnSorter = new ListViewColumnSorter();
            resultListView.ListViewItemSorter = lvwColumnSorter;

            InitCEFOrDuplicateMode();
        }

        private void InitCEFOrDuplicateMode()
        {
            cleanEmptyFolderModeToolStripMenuItem.Checked = false;
            if (gemini.ini.EqualsIgnoreCase("GeminiMode", "CEF", "Gemini"))
            {
                cleanEmptyFolderModeToolStripMenuItem.Checked = true;
            }
            nameColumnHeaderWidth = nameColumnHeader.Width;
            modifiedTimeColumnHeaderWidth = modifiedTimeColumnHeader.Width;
            ConvertToCEFMode(cleanEmptyFolderModeToolStripMenuItem.Checked);
            /*if (!cleanEmptyFolderModeToolStripMenuItem.Checked)
                m_lvToolTip.SetToolTip(resultListView, "Gemini");*/
        }

        /// <summary>
        /// bind to tbTargetFolderHistory
        /// </summary>
        private void SetUpFilterModeAndRegClick()
        {
            var fimode = gemini.ini.Read("FilterMode", "Gemini");
            if (!string.IsNullOrEmpty(fimode))
            {
                if (fimode.Equals("GEN_PROTECT"))
                {
                    regexCheckBox.Checked = false;
                    modeCheckBox.Checked = false;
                    filterMode = FilterMode.GEN_PROTECT;
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
                    modeCheckBox.Checked = true;
                    filterMode = FilterMode.GEN_FIND;
                }
            }
            else
            {
                regexCheckBox.Checked = false;
                modeCheckBox.Checked = true;
                filterMode = FilterMode.GEN_FIND;
            }
            UpdateFilterExampleText(filterMode);
            regexCheckBox.Click += new EventHandler(regexCheckBox_Click);
            modeCheckBox.Click += new EventHandler(modeCheckBox_Click);
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


        
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                DeleteCEF();
                return;
            }
            try
            {
                _source = new CancellationTokenSource();
                redoToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
                deleteList.Clear();
                // resultListView.Invoke(
                    //new MethodInvoker(delegate () {
                var checkedItems = resultListView.CheckedItems;
                if (checkedItems != null)
                {
                    string deleteFiles = "";
                    foreach (ListViewItem item in checkedItems)
                    {
                        if (_source.Token.IsCancellationRequested)
                        {
                            _source.Token.ThrowIfCancellationRequested();
                        }
                        var fullPathLV = item.SubItems["fullPath"].Text;
                        if (File.Exists(fullPathLV))
                        {
                            deleteList.Add(fullPathLV);
                            deleteFiles += "....." + fullPathLV + "\r\n";

                        }
                        Application.DoEvents();
                    }
                    CWriteLine("... will delete" + deleteFiles.Substring
                        (Math.Max(0, deleteFiles.Length - 2500)));
                }
                //})
                    //);
                CWriteLine($"\r\n=== You have selected {deleteList.Count} file(s).");
                SetSummaryBoxText($"Selected {deleteList.Count} file(s).", deleteList.Count);
                if (deleteList.Count < 1)
                {
                    return;
                }
                var taskDel = Task.Run(() => 
                {
                    try
                    {
                        // update Checked in GeminiFileCls;
                        var delGflChecked = new List<GeminiFileCls>();
                        CWriteLine($">>> updating Checked in GeminiFileCls...");
                        /*foreach (var item in geminiFileClsListForLV)
                        {
                            if (_source.Token.IsCancellationRequested)
                            {
                                _source.Token.ThrowIfCancellationRequested();
                            }
                            UpdateCheckedInDelGFL(delGflChecked, deleteList, item);
                        }*/

                        geminiFileClsListForLV.ForEach(a => a.Checked = false);
                        geminiFileClsListForLV.Where(x => deleteList.Contains(x.fullPath)).
                            ToList().ForEach(a =>a.Checked = true);

                        /*
                        * TODO: 
                        *   HOW COULD I PASS Anonymous Types ???
                        *   Func<TSource, TKey> keySelector;
                        *   var v2 = new { hash = "10086", size = 10086};
                        */

                        // group GeminiFileClsList
                        var delGflGrp = GeminiFileClsList2IEnumerableGroup(geminiFileClsListForLV, SetCompareMode());

                        // begin delete files, and prevent all files in the group from being deleted
                        int k = 0;
                        int j = 0;
                        bool notice = true;
                        var emptyFolderList = new List<string>();
                        CWriteLine(">>> updating prevent files in the group...");
                        string deletedFiles = "";
                        string preventFiles = "";
                        foreach (var item in delGflGrp)
                        {
                            if (_source.Token.IsCancellationRequested)
                            {
                                _source.Token.ThrowIfCancellationRequested();
                            }
                            // Prevent all files in the group from being deleted
                            if (!notProtectFilesInGrpToolStripMenuItem.Checked)
                            {
                                if ((from i in item
                                        where i.Checked == true
                                        select i).Count().Equals(item.Count))
                                {
                                    k++;
                                    if (k < 100)
                                    {
                                        CWriteLine($"!! [{k}] Prevent all files in the group from being deleted.");
                                    }
                                    preventFiles += $"!! [{k}] Prevent all files in the group from being deleted.\r\n";
                                    continue;
                                }
                            }

                            int hashEmpty = 0;
                            foreach (var it in item)
                            {
                                if (_source.Token.IsCancellationRequested)
                                {
                                    _source.Token.ThrowIfCancellationRequested();
                                }
                                if (it.Checked && File.Exists(it.fullPath))
                                {
                                    if (!string.IsNullOrEmpty(it.hash)
                                    && it.hash.ToLower().Contains("NotCounting".ToLower()))
                                    {
                                        hashEmpty++;
                                        CWriteLine(
                                            $"! [{hashEmpty}] Protect file without valid hash from being deleted: \r\n    {it.fullPath}");
                                        continue;
                                    }
                                    j++;
                                    if (j < 100)
                                    {
                                        CWriteLine($"...... Deleting file: {it.fullPath}");
                                    }
                                    else
                                    {
                                        if (notice)
                                        {
                                            CWriteLine($"...... Waiting for deleting file.");
                                            notice = false;
                                        }   
                                    }
                                    deletedFiles += $"...... Deleting file: {it.fullPath}\r\n";
                                    emptyFolderList.Add(Path.GetDirectoryName(it.fullPath));
                                    FileSystem.DeleteFile(it.fullPath, UIOption.OnlyErrorDialogs,
                                                    deleteOrRecycleBin.Checked ?
                                                    RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin,
                                                    UICancelOption.DoNothing);
                                }
                            }
                        }
                        var executingLocation = Path.GetDirectoryName(Assembly.
                            GetExecutingAssembly().Location);
                        var hisdir = Path.Combine(executingLocation, "Gemini.History");
                        if (!Directory.Exists(hisdir))
                        {
                            Directory.CreateDirectory(hisdir);
                        }
                        if (!string.IsNullOrEmpty(preventFiles) || !string.IsNullOrEmpty(deletedFiles))
                            File.WriteAllText(Path.Combine(hisdir, "deleted.txt"), 
                                preventFiles + deletedFiles);

                        // Clean Empty folders
                        if (autocleanEmptyFoldersToolStripMenuItem.Checked && emptyFolderList.Count > 0)
                        {
                            foreach (var dir in emptyFolderList)
                            {
                                EmptyJudge(dir);
                            }
                        }
                        CWriteLine($">>> Delete Finished.");

                        // clean non-existent file in geminiFileClsListForLV
                        //   update ListView and the checked in LV.
                    
                        cleanUpButton.Invoke(new MethodInvoker(delegate () {
                            cleanUpButton.PerformClick();
                        }));

                    }
                    catch (Exception ex)
                    {
                        CWriteLine("btnDelete_Click:" + ex.Message);
                        Debug.WriteLine("btnDelete_Click:" + ex);
                    }
                    }, _source.Token);
                _tasks.Add(taskDel);
                // taskDel.Wait();
                // await taskDel;
            }
            catch (UnauthorizedAccessException) { }
            catch (FileNotFoundException) { }
            catch (Exception ex)
            {
                CWriteLine($"!!! Error occur when deleting files/empty folders: {ex.Message}");
            }
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
        private GeminiCompareMode SetCompareMode()
        {
            GeminiCompareMode mode = GeminiCompareMode.NameAndSize;

            if (fileSHA1CheckBox.Checked || fileMD5CheckBox.Checked)
            {
                mode = GeminiCompareMode.HASH;
            }
            else if (fileNameCheckBox.Checked)
            {
                mode = GeminiCompareMode.NameAndSize;
            }
            else if (fileExtNameCheckBox.Checked)
            {
                mode = GeminiCompareMode.ExtAndSize;
            }
            return mode;
        }
       

        
        private async Task<List<GeminiFileCls>> ComparerTwoFolderGetList(List<GeminiFileCls> l1,
            List<GeminiFileCls> l2, GeminiCompareMode mode, long limit = 0, CancellationToken token = default,
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

            var sameList = new List<GeminiFileCls>();
            void retAction(bool res, List<GeminiFileCls> ret)
            {
                _mutex.WaitOne();
                if (res && ret.Count > 1)
                {
                    sameList.AddRange(ret);
                }
                _mutex.ReleaseMutex();
            }
            long cnt1 = l1.Count;
            long cnt2 = l2.Count;
            long totalCmpCnt = cnt1 * cnt1 + cnt1 * cnt2 + cnt2 * cnt2;

            string tip = "";
            if (ignoreFileCheckBox.Checked)
            {
                tip = $" larger than {Len2Str(limit)}";
            }
            CWriteLine($">>> folder1{tip}: {cnt1:N0}");
            CWriteLine($">>> folder2{tip}: {cnt2:N0}");
            CWriteLine($">>> about {totalCmpCnt:N0} times (x1*x1+x2*x2+x1*x2)...");

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
                SetProgressMessage(pb, percentInt);

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
      



        private async Task UpdateHash(List<GeminiFileCls> gfL,
            GeminiFileCls gf)
        {
            /*void ProgressActionD(double i) // percent in file.
            {
                _mutexPb.WaitOne();
                var percentInt = (int)i;
                if (percentInt > 100)
                    percentInt = 100;
                SetProgressMessage(percentInt, pb);
                _mutexPb.ReleaseMutex();
            }
            var totalProgessDouble = new Progress<double>(ProgressActionD);*/
            // DONT REPORT HERE.

            if (fileMD5CheckBox.Checked)
            {
                void getRes(bool res, string who, string md5, string costTimeOrMsg)
                {
                    if (res)
                    {
                        gf.hash = md5;
                    }
                }
                await ComputeHashAsync(
                    MD5.Create(), gf.fullPath, _source.Token, "MD5", getRes);
            }
            else if (fileSHA1CheckBox.Checked)
            {
                void getRes(bool res, string who, string sha1, string costTimeOrMsg)
                {
                    if (res)
                    {
                        gf.hash = sha1;
                    }
                }
                await ComputeHashAsync(
                    SHA1.Create(), gf.fullPath, _source.Token, "SHA1", getRes);
            }
            gfL.Add(gf);
        }

        private IEnumerable<List<GeminiFileCls>>
            GFL2IEnumGrpAnonymous<T>(List<GeminiFileCls> gfl,
            Func<GeminiFileCls, T> keySelector)
            => from i in gfl
               where File.Exists(i.fullPath)
               orderby i.size descending
               group i by keySelector(i) into grp
               where grp.Count() > 1
               select grp.ToList();

        private IEnumerable<List<GeminiFileCls>> GeminiFileClsList2IEnumerableGroup(
            List<GeminiFileCls> gfl, GeminiCompareMode mode)
        {
            IEnumerable<List<GeminiFileCls>> duplicateGrp = null;
            if (mode == GeminiCompareMode.HASH)
            {
                duplicateGrp = GFL2IEnumGrpAnonymous(gfl, i => i.hash);
            }
            else if (mode == GeminiCompareMode.ExtAndSize)
            {
                duplicateGrp = GFL2IEnumGrpAnonymous(gfl, i => new { i.size, i.extName });
            }
            else
            {
                duplicateGrp = GFL2IEnumGrpAnonymous(gfl, i => new { i.size });
            }
            return duplicateGrp;
        }

        /*private IEnumerable<List<GeminiFileCls>> GeminiFileClsList2IEnumerableGroupBP(
            List<GeminiFileCls> gfl, GeminiCompareMode mode)
        {
            IEnumerable<List<GeminiFileCls>> duplicateGrp = null;
            if (mode == GeminiCompareMode.HASH)
            {
                duplicateGrp =
                from i in gfl
                where File.Exists((i.fullPath))
                orderby i.name
                group i by new { i.hash } into grp
                where grp.Count() > 1
                select grp.ToList();
            }
            else if (mode == GeminiCompareMode.ExtAndSize)
            {
                duplicateGrp =
                   from i in gfl
                   where File.Exists((i.fullPath))
                   orderby i.name
                   group i by new { i.size, i.extName } into grp
                   where grp.Count() > 1
                   select grp.ToList();
            }
            else
            {
                duplicateGrp =
                   from i in gfl
                   where File.Exists((i.fullPath))
                   orderby i.name
                   group i by i.size into grp
                   where grp.Count() > 1
                   select grp.ToList();
            }
            return duplicateGrp;
        }*/

        private List<GeminiFileCls> ListReColorByGroup(List<GeminiFileCls> gfl, GeminiCompareMode mode,
            CancellationToken token)
        {
            var duplicateGrp = GeminiFileClsList2IEnumerableGroup(gfl, mode);
            var tmpHash = new List<GeminiFileCls>();

            long j = 0;
            foreach (var item in duplicateGrp)
            {
                j++;
                Color color = Color.White;
                // var color = Color.White;
                if (j % 3 == 1)
                {
                    // color = themeColor;
                    color = themeColor;
                }
                if (j % 3 == 2)
                {
                    // color = Color.LightBlue;
                    color = Color.White;
                }
                if (j % 3 == 0)
                {
                    // color = Color.LightBlue;
                    color = Color.LightBlue;
                }

                foreach (var it in item)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    var tmp = it;
                    tmp.color = color;
                    tmpHash.Add(tmp);
                }
            }
            return tmpHash;
        }

        private async Task<List<GeminiFileCls>> UpdateHashInGeminiFileClsList(
            List<GeminiFileCls> gfL, CancellationToken token, bool updateBigFile)
        {
            var tmp = new List<GeminiFileCls>();
            int i = 0;
            foreach (var it in gfL)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                i++;
                int hashSizeLimit = 100;
                if (!File.Exists(it.fullPath))
                {
                    continue;
                }
                if (it.size < hashSizeLimit * 1024 * 1024 || updateBigFile)
                {
                    await UpdateHash(tmp, it);
                }
                else
                {
                    var bp = it;
                    bp.hash = $">{hashSizeLimit}MB,NotCounting.YouCouldEnableAlwaysCalculateHash";
                    bp.bigFile = true;
                    tmp.Add(bp);
                }
                SetProgressMessage(geminiProgressBar, (int)((double)i / gfL.Count * 100));
            }
            return tmp;
        }

        public static void AddSubItem(System.Windows.Forms.ListViewItem i, string name, string text)
        {
            i.SubItems.Add(new System.Windows.Forms.ListViewItem.ListViewSubItem()
            { Name = name, Text = text });
        }

        private void AddGroupTitleToListView()
        {

        }

        delegate void SetTextCallBack(System.Windows.Forms.TextBox tb, string text, Color c = default);
        private void SetText(System.Windows.Forms.TextBox tb, string text, Color c = default)
        {
            if (tb.InvokeRequired)
            {
                SetTextCallBack stcb = new SetTextCallBack(SetText);
                tb.Invoke(stcb, new object[] { tb, text, c });
            }
            else
            {
                SetProgressBarVisible(geminiProgressBar, false);
                tb.Text = text;
                tb.BackColor = c;
            }
        }

        private void SetSummaryBoxText(string text, int cnt)
        {
            if (summaryTextBox.InvokeRequired)
            {
                summaryTextBox.Invoke(new MethodInvoker(delegate () 
                    { SetSummaryBoxText(text, cnt); 
                    }
                    ));
            }
            else
            {
                SetProgressBarVisible(geminiProgressBar, false);
                summaryTextBox.Text = text;
                var c = cnt == 0 ? themeColorClean : themeColor;
                summaryTextBox.BackColor = c;
            }
        }

        

        private void ScanEmptyDirsProtectMode(string dir, CancellationToken token)
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
                    ScanEmptyDirsProtectMode(d, token);
                }
                EmptyJudgeCEF(dir, print: true);
            }
            catch (UnauthorizedAccessException) { }
        }

        /// <summary>
        /// TEST CASE: 
        /// 1)games 2)D:\games 3)games,Steam\logs 4)D:\games,Steam\logs
        /// </summary>

        private void ScanEmptyDirsFindMode(string path, CancellationToken token, bool re = false)
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
                    ScanEmptyDirsFindMode(d, token, re);
                }
                if (re)
                {
                    if (regex.IsMatch(path))
                    {
                        EmptyJudgeCEF(path, print:true);
                        return;
                    }
                }
                else
                {
                    foreach (var filter in pathFilter)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        if (path.Contains(filter))
                        {
                            EmptyJudgeCEF(path, print: true);
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

        private void UpdateEmptyFoldersToLV(List<GeminiCEFCls> gcefl, CancellationToken token)
        {
            ListViewOperate(resultListView, ListViewOP.CLEAR);
            if (gcefl.Count < 1)
            {
                SetSummaryBoxText($"Summay: Found No duplicate files.", 0);
                CWriteLine(">>> The folder is CLEAN.");
                return;
            }
            var items = new List<ListViewItem>();
            foreach (var gcef in gcefl)
            {
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                var item = new ListViewItem();
                AddSubItem(item, "name", gcef.fullPath);
                AddSubItem(item, "lastMtime", gcef.lastMtime);
                items.Add(item);
            }
            if (items.Count > 0)
            {
                ListViewOperate(resultListView, ListViewOP.ADDRANGE, items: items.ToArray());
            }
            CWriteLine($">>> Found {geminiCEFClsList.Count:N0} empty folder(s).");
            SetSummaryBoxText($"Summay: Found {geminiCEFClsList.Count:N0} duplicate files.",
                geminiCEFClsList.Count);
        }

        // Thanks to João Angelo
        // https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
        private async void RecurseScanDirCEF(string path, CancellationToken token)
        {
            //token.ThrowIfCancellationRequested();
            // DO NOT KNOW WHY D: DOESNOT WORK WHILE D:\ WORK.
            try
            {
                var taskCef = Task.Run(() =>
                {
                    // Were we already canceled?
                    // token.ThrowIfCancellationRequested();
                    cefScanRes = false;
                    CWriteLine($">>> Start Analyze Operation...");
                    CWriteLine($">>> Because it is a recursive search, \r\n" +
                        "  Program don't know the progress, please wait patiently...");

                    SetSummaryBoxText("Please wait patiently...", 1);
                    // Set a variable to the My Documents path.
                    // string docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); 

                    if (filterMode == FilterMode.GEN_FIND && pathFilter.Count > 0)
                    {
                        ScanEmptyDirsFindMode(path, token, re: false);
                    }
                    else if (filterMode == FilterMode.REGEX_FIND && regex != null)
                    {
                        ScanEmptyDirsFindMode(path, token, re: true);
                    }
                    else
                    {
                        ScanEmptyDirsProtectMode(path, token);
                    }

                }, token);
                _tasks.Add(taskCef);
                await taskCef;
                cefScanRes = true;
                UpdateEmptyFoldersToLV(geminiCEFClsList, token);
                selectAllToolStripMenuItem.PerformClick();
                CWriteLine($">>> Finished Analyze Operation");
            }
            catch (OperationCanceledException e)
            {
                cefScanRes = false;
                CWriteLine($"\r\n>>> RecurseScanDir throw exception message: {e.Message}");
            }
            catch (Exception e)
            {
                cefScanRes = false;
                CWriteLine($"\r\n RecurseScanDir throw exception message: {e.Message}");
                CWriteLine($"\r\n#----^^^  PLEASE CHECK, TRY TO CONTACT ME WITH THIS LOG.  ^^^----#");
            }
            finally
            {
                btnDelete.Enabled = true;
                btnAnalyze.Enabled = true;
            }
        }
        private void AnalyzeEmptyFolder(string path)
        {
            _source = new CancellationTokenSource();
            btnStop.Enabled = true;
            btnDelete.Enabled = false;
            btnAnalyze.Enabled = false;
            if (!UpdateTextAndIniFile("TargetFolder1", path, targetFolder1History))
            {
                CWriteLine("Invalid directory path: {0}" + path);
                return;
            }
            if (!SetFolderFilter(folderFilterTextBox.Text, print: true))
            {
                return;
            }
            geminiCEFClsList.Clear();
            RecurseScanDirCEF(path, _source.Token);
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                ListViewOperate(resultListView, ListViewOP.CLEAR);
                AnalyzeEmptyFolder(targetFolder1TextBox.Text);
            }
            else
            {
                // btnClear.PerformClick();
                StartAnalyzeStep();
                redoToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_source != null)
            {
                _source.Cancel();
            }
            if (reIndexTokenSrc != null)
            {
                reIndexTokenSrc.Cancel();
            }
            if (hashTokenSrc != null)
            {
                hashTokenSrc.Cancel();
            }
            CWriteLine("Stop...");
            geminiProgressBar.Value = 0;
            EnableButtonsForIndexUpdate(true);
        }
        private void btnClear_Click(object sender, EventArgs e)
        {
            tbConsole.Clear();
            SetText(summaryTextBox, "", SystemColors.Control);
            summaryTextBox.Text = "";
            geminiProgressBar.Value = 0;
        }

        // CheckFileIfLocked(@"e:\bd_download\Adobe_Audition_CC.7z");
        private void CheckFileIfLocked(string path)
        {
            try
            {
                if (!File.Exists(path))
                {
                    return;
                }
                var pl = FileLockUtil.WhoIsLocking(path);
                if (pl.Count < 1)
                {
                    return;
                }
                (from i in pl
                    select i.ProcessName).Distinct().ToList().ForEach(it =>
                    CWriteLine($"{path} occupied by program " + it));            
            }
            catch (Exception ex)
            {
                CWriteLine(ex.Message);
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
                CWriteLine("Invalid directory path: {0}" + path);
                return;
            }

            // CWriteLine($"\r\n#---- Started Analyze Operation ----#\r\n");
            if (cefScanRes && filesList.Count > 0)
            {
                foreach (var folder in filesList)
                {
                    CWriteLine($"found ###  {folder}");
                }
                return;
            }
            /*if (filterMode == FilterMode.GEN_FIND && pathFilter.Count > 0)
            {
                FindFilesWithFindMode(path, filesList, token, re: false);
            }
            else if (filterMode == FilterMode.REGEX_FIND && regex != null)
            {
                FindFilesWithFindMode(path, filesList, token, re: true);
            }
            else
            {*/
            FindFilesWithProtectMode(path, filesList, token);
            /*}*/
        }

        private bool FolderFilter(string path, FilterMode mode)
        {
            if (mode == FilterMode.GEN_PROTECT)
            {
                if (pathFilter.Count > 0)
                {
                    foreach (var filter in pathFilter)
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
            return false;
        }

        /// <summary>
        /// TEST CASE: 
        /// 1)games 2)D:\games 3)games,Steam\logs 4)D:\games,Steam\logs
        /// </summary>

        /*private void FindFilesWithFindMode(string path, List<string> filesList, CancellationToken token, bool re = false)
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
                    foreach (var filter in pathFilter)
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
        }*/
        //calcSHA1: fileSHA1CheckBox.Checked,
        // calcMD5: fileMD5CheckBox.Checked

        


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

        
        private bool IsCmdInTextBox(System.Windows.Forms.TextBox box, string cmd)
        {
            cmd = cmd.Trim();

            // command mode
            bool useCommand = false;

            if (cmd.ToLower().Equals("list controlled"))
            {
                CWriteLine("\r\nThe following is a list of controlled folders:");
                foreach (var f in gemini.GetAllControlledFolders())
                {
                    CWriteLine(f);
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
                CWriteLine(gemini.helpString);
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

        private void saveLogToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tbConsole.Text.Length < 1)
            {
                return;
            }
            using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                var saveDir = Path.Combine(Path.GetDirectoryName(Assembly.
                    GetExecutingAssembly().Location), "Gemini.UserOperation");
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
                    t1 = "";
                }
                else
                {
                    t1 = new DirectoryInfo(f1).Name;
                }
                if (string.IsNullOrEmpty(f2) || !Directory.Exists(f2))
                {
                    t2 = "";
                }
                else
                {
                    t2 = "-" + new DirectoryInfo(f2).Name;
                }
                var name = t1 + t2;
                name = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
                
                saveFileDialog.FileName = "Gemini-log-" +
                                        DateTime.Now.ToString("yyyy-MM-dd_HH-mm") + "-" + name + ".txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        var dataAsBytes = System.Text.Encoding.Default.GetBytes(tbConsole.Text);
                        stream.Write(dataAsBytes, 0, dataAsBytes.Length);
                    }
                }
            }

        }

        private List<string> StringToFilter(string filter, bool print = true)
        {
            var fldFilter = new List<string>();
            if (string.IsNullOrEmpty(filter))
            {
                return fldFilter;
            }
            if (filter.Contains("，"))
            {
                if (print) CWriteLine("! Filter: Chinese comma(full-width commas) in the filter.");
            }
            filter = filter.Trim();
            var filterList = filter.Split(',');
            if (filterList.Length < 1)
            {
                return fldFilter;
            }
            if (print) CWriteLine(". You have set the following general filter(s):");
            foreach (var ft in filterList)
            {
                if (print) CWriteLine($" {ft} ");
                fldFilter.Add(ft);
            }
            return fldFilter;
        }
        /// <summary>
        ///             if (!SetFolderFilter(folderFilterTextBox.Text, print: true))
        /// </summary>
        /// <param name="text"></param>
        /// <param name="print"></param>
        /// <returns></returns>
        private bool SetFolderFilter(string filter, bool print = false)
        {
            pathFilter = new List<string>();
            if (string.IsNullOrEmpty(filter))
            {
                regexFilter = "";
                regex = null;
                CWriteLine($">>> Using: {filterMode}, but there is no valid filter value.");
                return true;
            }
            CWriteLine($">>> Using: {filterMode}");
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
                    CWriteLine($"\r\n!!! filter ERROR: {regexFilter} illegal");
                    CWriteLine($"\r\n!!! ERROR: {e.Message}");
                    regex = null;
                    return false;
                }
                if (print)
                {
                    CWriteLine($"\r\nYou have set the regex filter: \" {regexFilter} \"");
                }
                return true;
            }
            else
            {
                pathFilter = StringToFilter(filter, print);
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
        private void regexCheckBox_Click(object sender, EventArgs e)
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

        private void modeCheckBox_Click(object sender, EventArgs e)
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
            if (!cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                targetFolder_DragDrop(sender, e, "TargetFolder2", targetFolder2History,
                targetFolder2TextBox);
            }
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
                        UpdateTextAndIniFile(keyInIni, path,
                            targetFolderHistory, tx);
                    }
                }
                else
                {
                    CWriteLine("\r\nAttention: File or multiple folders are not allowed!");
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

        private long SetMinimumFileLimit(bool justPrint = false)
        {
            minimumFileLimit = 0;
            if (ignoreFileCheckBox.Checked)
            {
                ignoreFileSizeTextBox.Enabled = true;
                if (int.TryParse(ignoreFileSizeTextBox.Text, out int ret))
                {
                    string unit;
                    switch (ignoreFileSizecomboBox.SelectedIndex)
                    {
                        case 0:
                            minimumFileLimit = ret * 1;
                            unit = "B";
                            break;
                        case 1:
                            minimumFileLimit = ret * 1024;
                            unit = "KB";
                            break;
                        case 2:
                            minimumFileLimit = ret * 1024 * 1024;
                            unit = "MB";
                            break;
                        case 3:
                            minimumFileLimit = ret * 1024 * 1024 * 1024;
                            unit = "GB";
                            break;
                        default:
                            minimumFileLimit = ret * 1024 * 1024;
                            unit = "MB";
                            break;
                    }
                    if (justPrint)
                    {
                        CWriteLine($"---- Ignore files less than {ret}{unit}");
                        return 0;
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

        private void GetCurrentScanStatus()
        {
            CWriteLine(">>> YOU HAVE CHOSEN THE FOLLOWING MODE, CHECK IF IT'S WHAT YOU WANT: ");
            if (fileNameCheckBox.Checked)
            {
                CWriteLine("---- Same File Name mode.");
            }
            else if (fileExtNameCheckBox.Checked)
            {
                CWriteLine("---- Same File extension mode.");
            }
            if (fileMD5CheckBox.Checked)
            {
                CWriteLine("---- Same MD5 mode.");
            }
            else if (fileSHA1CheckBox.Checked)
            {
                CWriteLine("---- Same SHA1 mode.");
            }
            SetMinimumFileLimit(justPrint: true);
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

        private void alwaysOnTopMenu_Click(object sender, EventArgs e)
        {
            //if (alwaysOnTopCheckBox.Checked)
            var it = alwaysOnTopToolStripMenuItem;
            if (!it.Checked)
            {
                it.Checked = true;
                TopMost = true;
            }
            else
            {
                it.Checked = false;
                TopMost = false;
            }
        }
        private void resultListView_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


        private delegate void SetProgressMessageDelegate(
            System.Windows.Forms.ProgressBar proBar, int value);
        private void SetProgressMessage(System.Windows.Forms.ProgressBar proBar, int value)
        {
            if (InvokeRequired)
            {
                if (proBar.IsHandleCreated)
                {
                    SetProgressMessageDelegate setPro = new SetProgressMessageDelegate(SetProgressMessage);
                    Invoke(setPro, new object[] { proBar, value });
                }
            }
            else
            {
                proBar.Value = Convert.ToInt32(value);
            }
        }

        private delegate void SetProgressBarVisibleDelegate(
            System.Windows.Forms.ProgressBar proBar, bool visible);
        private void SetProgressBarVisible(System.Windows.Forms.ProgressBar proBar, bool visible)
        {
            if (InvokeRequired)
            {
                if (proBar.IsHandleCreated)
                {
                    var setvi = new SetProgressBarVisibleDelegate(SetProgressBarVisible);
                    Invoke(setvi, new object[] { proBar, visible });
                }
            }
            else
            {
                proBar.Visible = visible;
            }
        }

        private void geminiProgressBar_Click(object sender, EventArgs e)
        {

        }

        private List<GeminiCEFCls> UpdateCEFCheckedFromLV(List<GeminiCEFCls> gcefl)
        {
            if (resultListView.Items.Count > 0 && gcefl.Count > 0)
            {
                var tmpL = new List<GeminiCEFCls>();
                foreach (var item in resultListView.Items)
                {
                    var it = ((ListViewItem)item);
                    var fullPathLV = it.SubItems["name"].Text;
                    foreach (var gcef in gcefl)
                    {
                        var tmp = gcef;
                        if (gcef.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                        {
                            tmp.Checked = it.Checked;
                            tmpL.Add(tmp);
                            break;
                        }
                    }
                }
                return tmpL;
            }
            return null;
        }

        private List<GeminiFileCls> UpdateGFLChecked(List<GeminiFileCls> gfl)
        {
            var tmpL = new List<GeminiFileCls>();
            void UpdateGeminiFileCls(bool res, List<GeminiFileCls> gf, string msg)
            {
                if (res)
                {
                    tmpL = gf;
                }
                else
                {
                    CWriteLine(msg);
                }
            }
            try
            {
                ListViewOperateLoop(resultListView, ListViewOP.UPDATE_CHECK_BY_INDEX, gfl,
                    UpdateGeminiFileCls);
                if (tmpL.Count == gfl.Count)
                {
                    return tmpL;
                }
            }
            catch
            {
                tmpL = new List<GeminiFileCls>();
                ListViewOperateLoop(resultListView, ListViewOP.UPDATE_CHECK_INTHELOOP, gfl, 
                    UpdateGeminiFileCls);
            }
            return tmpL;
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                MultipleSelectOperationsActionCEF(MultipleSelectOperations.CHECK_ALL);
            }
            else
            {
                // MultipleSelectOperationsAction(resultListView, MultipleSelectOperations.CHECK_ALL);
                MultipleSelectOpAction(resultListView, MultipleSelectOperations.CHECK_ALL);
            }
        }

        private void unselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                MultipleSelectOperationsActionCEF(MultipleSelectOperations.UNCHECK_ALL);
            }
            else
            {
                // MultipleSelectOperationsAction(resultListView, MultipleSelectOperations.UNCHECK_ALL);
                MultipleSelectOpAction(resultListView, MultipleSelectOperations.UNCHECK_ALL);
            }
        }

        private void reverseElectionToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                MultipleSelectOperationsActionCEF(MultipleSelectOperations.REVERSE_ELECTION);
            }
            else
            {
                // MultipleSelectOperationsAction(resultListView, MultipleSelectOperations.REVERSE_ELECTION);
                MultipleSelectOpAction(resultListView, MultipleSelectOperations.REVERSE_ELECTION);
            }
        }


        private void saveResultListToFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cleanEmptyFolderModeToolStripMenuItem.Checked && geminiCEFClsList.Count < 1)
            {
                CWriteLine("--- No Empty folder, do not need to save to file.");
                return;
            }
                
            if (!cleanEmptyFolderModeToolStripMenuItem.Checked && geminiFileClsListForLV.Count < 1)
            {
                CWriteLine("--- No file, do not need to save to file.");
                return;
            }
            using (var saveFileDialog = new System.Windows.Forms.SaveFileDialog())
            {
                var saveDir = Path.Combine(Path.GetDirectoryName(Assembly.
                    GetExecutingAssembly().Location), "Gemini.UserOperation");
                if (!Directory.Exists(saveDir))
                {
                    Directory.CreateDirectory(saveDir);
                }
                saveFileDialog.InitialDirectory = saveDir;
                saveFileDialog.Filter = "XML files (*.xml)|*.xml";
                // saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                string t1;
                string t2;
                var f1 = targetFolder1TextBox.Text;
                var f2 = targetFolder2TextBox.Text;
                if (string.IsNullOrEmpty(f1) || !Directory.Exists(f1))
                {
                    t1 = "";
                }
                else
                {
                    t1 = new DirectoryInfo(f1).Name;
                }
                if (string.IsNullOrEmpty(f2) || !Directory.Exists(f2))
                {
                    t2 = "";
                }
                else
                {
                    t2 = "-"+ new DirectoryInfo(f2).Name;
                }

                var name = t1 + t2;
                name = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
                string header;
                if (cleanEmptyFolderModeToolStripMenuItem.Checked)
                {
                    header = "GeminiCEF-";
                }
                else
                {
                    header = "Gemini-";
                }
                saveFileDialog.FileName = header + DateTime.Now.ToString("yyyy-M-dd_HH-mm") + 
                                        "-" + name + ".xml";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Write the list of salesman objects to file.
                    /*var saveTask = Task.Run(() => {*/

                    if (cleanEmptyFolderModeToolStripMenuItem.Checked)
                    {
                        Task.Run(() => {
                            WriteToXmlFile(saveFileDialog.FileName,
                            geminiCEFClsList);
                        });
                    }
                    else
                    {
                        Task.Run(() => {
                            WriteToXmlFile(saveFileDialog.FileName,
                            geminiFileClsListForLV);
                        });
                    }                   
                    /*});
                    _tasks.Add(saveTask);*/
                    // WriteToJsonFile<List<GeminiFileCls>>(@"GeminiFileCls.json", geminiFileClsListForLV);

                    /*// Read the list of salesman objects from the file back into a variable.
                    List<GeminiFileCls> geminiFileClsListForLVFromFile = 
                        ReadFromXmlFile<List<GeminiFileCls>>("GeminiFileCls.xml");*/
                }
            }
        }

        private void usageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CWriteLine(gemini.helpString);
        }

        private void cleanUpButton_Click(object sender, EventArgs e)
        {
            if (resultListView.Items.Count > 0)
            {
                resultListView.Items.OfType<ListViewItem>().ToList().
                    ForEach(item => item.ForeColor = Color.Black);
            }
            var taskCleanUp = Task.Run(() => {
                try
                {              
                redoToolStripMenuItem.Enabled = false;
                undoToolStripMenuItem.Enabled = false;
                // custQuery is an IEnumerable<IGrouping<string, Customer>>
                if (geminiFileClsListForLV.Count < 1)
                {
                    CWriteLine("!!! ANALYZE First.");
                    return;
                }
                var duplicateGrp = GeminiFileClsList2IEnumerableGroup(geminiFileClsListForLV,
                    SetCompareMode());
                int cnt = 0;
                foreach (var item in duplicateGrp)
                {
                    foreach (var it in item)
                    {
                        cnt++;
                    }
                }
                if (cnt < geminiFileClsListForLV.Count)
                {
                    CWriteLine(
                        $">>> Remove {geminiFileClsListForLV.Count - cnt} " +
                        "items from ListView [ nonexistent + non-repeating ].");
                    int findex;
                    try
                    {
                        findex = resultListView.FocusedItem.Index;
                    }
                    catch
                    {
                        findex = 0;
                    }
                    var index = Math.Max(findex - 2, 0);
                    geminiFileClsListForLV = ListReColorByGroup(geminiFileClsListForLV,
                        SetCompareMode(), _source.Token);
                    UpdateListView(resultListView, ref geminiFileClsListForLV, _source.Token);
                    void GetRet(bool ret, string msg)
                    {
                       if (ret)
                       {
                            if (resultListView.Items.Count > 0 && deleteKey)
                            {
                                deleteKey = false;
                                resultListView.FocusedItem = resultListView.Items[index];
                                resultListView.SelectedIndices.Clear();
                                resultListView.Items[index].Selected = true;
                                resultListView.Select();
                                resultListView.EnsureVisible(index); 
                                // This is the trick, EnsureVisible() is just as important.
                            }
                            CWriteLine($">>> Clean-UP Finished.");
                            Debug.WriteLine($">>> Clean-UP Finished: {msg}");
                       }
                    }
                    /*RestoreListViewChoiceInvoke(resultListView, 
                        geminiFileClsListForLV, , indexChange: true, action: GetRet);*/

                    // geminiFileClsListForLV will not be modify, if toListView is true.
                    ConvertGeminiFileClsListAndListView(ref geminiFileClsListForLV,
                        resultListView, toListView: true, token: _source.Token, action: GetRet);
                }
                else
                {
                    CWriteLine(">>> All exist, no need to clean up.");
                }
            }
            catch (Exception ex)
            {
                CWriteLine(ex.ToString());
            }
             });
            _tasks.Add(taskCleanUp);
        }

        private void deleteOrRecycleBin_Click(object sender, EventArgs e)
        {
            if (deleteOrRecycleBin.Checked)
            {
                btnDelete.Text = "Delete";
            }
            else
            {
                btnDelete.Text = "RecycleBin";
            }
            gemini.ini.UpdateIniItem("RecycleBin", deleteOrRecycleBin.Checked.ToString(), "Gemini");
        }

       
        private void updateButton_Click(object sender, EventArgs e)
        {
            MultipleSelectOpAction(resultListView, MultipleSelectOperations.UNCHECK_ALL, force: true);
            Task.Run(() => {
                CWriteLine($">>> Update start with {filterMode}...");
                geminiFileClsListForLVUndo = BackUpForUndoRedo(
                 geminiFileClsListForLV, undoToolStripMenuItem);
                SetFolderFilter(folderFilterTextBox.Text);
                var updatedList = new List<GeminiFileCls>();
                var selectList = new List<GeminiFileCls>();
                var fldFilter = StringToFilter(targetFolderFilterTextBox.Text);
                var tpl = GetGFLbyTheFilter(geminiFileClsListForLV, fldFilter);
                if (fldFilter.Count > 0)
                {
                    selectList = tpl.Item1;
                    updatedList.AddRange(tpl.Item2);
                }
                else
                {
                    selectList = tpl.Item2;
                }
                if (pathFilter.Count > 0)
                {
                    selectList.ForEach(it => {
                        it.Checked = GeminiFileClsListGeneralForEach(it, pathFilter,
                            find: filterMode == FilterMode.GEN_FIND);
                    });                   
                }
                else if (regex != null)
                {
                    selectList.ForEach(it => {
                        it.Checked = GeminiFileClsListREForEach(it, regex,
                            find: filterMode == FilterMode.REGEX_FIND); // FilterMode.REGEX_PROTECT
                    });
                }
                updatedList.AddRange(selectList);

                geminiFileClsListForLV = ListReColorByGroup(updatedList, SetCompareMode(), _source.Token);
                RestoreListViewChoice(geminiFileClsListForLV, resultListView, _source.Token);

                var cnt =
                    (from i in geminiFileClsListForLV
                     where i.Checked == true
                     select i).Count();
                CWriteLine($">>> {filterMode} selectd {cnt:N0} file(s).");
                SetSummaryBoxText($"{filterMode} selectd {cnt:N0} file(s)", cnt);
            });
        }

        private void resultListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var focusedItem = resultListView.FocusedItem;
                if (focusedItem == null)
                {
                    return;
                }
                /*if (focusedItem.SubItems["name"].Bounds.Contains(e.Location))
                {
                    var filePath = focusedItem.SubItems["fullPath"].Text;
                    if (File.Exists(filePath))
                    {
                        ShellContextMenu scm = new ShellContextMenu();
                        FileInfo[] files = new FileInfo[1];
                        files[0] = new FileInfo(filePath);
                        scm.ShowContextMenu(files, Cursor.Position);
                    }
                }
                else if (focusedItem.SubItems["dir"].Bounds.Contains(e.Location))
                {
                    listViewContextMenuStrip.Show(Cursor.Position);
                }
                else
                {

                }*/
            }
            if (e.Button == MouseButtons.Left)
            {
                /*var focusedItem = resultListView.FocusedItem;
                if (focusedItem == null)
                {
                    return;
                }
                if (focusedItem.SubItems["dir"].Bounds.Contains(e.Location))
                {
                    string itemInfo = focusedItem.SubItems["dir"].Text;
                    // new System.Windows.Forms.ToolTip().SetToolTip(e.Item.ListView, itemInfor);
                    new System.Windows.Forms.ToolTip().SetToolTip(focusedItem.ListView, itemInfo);
                }*/
            }
        }

        /// <summary>
        /// https://stackoverflow.com/questions/17916183/handle-click-on-a-sub-item-of-listview
        /// </summary>
        /*
         private void listView_Click(object sender, EventArgs e)
        {
            Point mousePos = listView.PointToClient(Control.MousePosition);
            ListViewHitTestInfo hitTest = listView.HitTest(mousePos);
            int columnIndex = hitTest.Item.SubItems.IndexOf(hitTest.SubItem);
        }
         */
        private void resultListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (!cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                if (e.Button == MouseButtons.Left)
                {
                    var focusedItem = resultListView.FocusedItem;
                    if (focusedItem == null)
                    {
                        return;
                    }
                    if (focusedItem.SubItems["dir"].Bounds.Contains(e.Location))
                    {
                        var filePath = focusedItem.SubItems["fullPath"].Text;
                        if (File.Exists(filePath))
                        {
                            // combine the arguments together
                            // it doesn't matter if there is a space after ','
                            string argument = "/select, \"" + filePath + "\"";
                            Process.Start("explorer.exe", argument);
                        }
                    }
                    // THE FIRST ANONYMOUS ITEM MUST USE INDEX, I PERFET SUBITEMS["NAME"]
                    else if (focusedItem.SubItems["name"].Bounds.Contains(e.Location))
                    {
                        try
                        {
                            var filePath = focusedItem.SubItems["fullPath"].Text;
                            if (File.Exists(filePath))
                            {
                                // open file.
                                Process.Start(filePath);
                            }

                        }
                        catch (Exception ex)
                        {
                            CWriteLine(ex.Message);
                        }
                        
                    }
                    else
                    {
                        // DONOTHING.
                    }

                    // DOESN'T WORK, HIT.SUBITEM AND HIT.ITEM IS NULL.
                    /*Point mousePosition = resultListView.PointToClient(System.Windows.Forms.Control.MousePosition);
                    ListViewHitTestInfo hit = resultListView.HitTest(mousePosition);
                    // hit.Item.SubItems["fullPath"].Text
                    int columnindex = hit.Item.SubItems.IndexOf(hit.SubItem);
                    if (resultListView.Columns[columnindex].Name == "dirColumnHeader")
                    {
                        var filePath = hit.Item.SubItems["fullPath"].Text;
                        if (File.Exists(filePath))
                        {
                            // combine the arguments together
                            // it doesn't matter if there is a space after ','
                            string argument = "/select, \"" + filePath + "\"";
                            Process.Start("explorer.exe", argument);
                        }
                    }*/
                }
            }            
        }

        
        private List<GeminiFileCls> BackUpForUndoRedo(List<GeminiFileCls> gfl,
            ToolStripMenuItem tm)
        {
            /*var li =
                (from i in gfl
                select i).ToList();*/
            tm.Enabled = true;
            return gfl.ConvertAll(gf => gf.Clone());
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cleanEmptyFolderModeToolStripMenuItem.Checked)
            {

            }
            else
            {
                if (geminiFileClsListForLVUndo.Count > 0)
                {
                    geminiFileClsListForLVRedo = BackUpForUndoRedo(
                        geminiFileClsListForLV, redoToolStripMenuItem);
                    geminiFileClsListForLV = geminiFileClsListForLVUndo;
                    RestoreListViewChoice(geminiFileClsListForLV, resultListView, _source.Token);
                    undoToolStripMenuItem.Enabled = false;
                }
            }

        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (cleanEmptyFolderModeToolStripMenuItem.Checked)
            {

            }
            else
            {
                if (geminiFileClsListForLVRedo.Count > 0)
                {
                    geminiFileClsListForLVUndo = BackUpForUndoRedo(
                        geminiFileClsListForLV, undoToolStripMenuItem);
                    geminiFileClsListForLV = geminiFileClsListForLVRedo;
                    RestoreListViewChoice(geminiFileClsListForLV, resultListView, _source.Token);
                    redoToolStripMenuItem.Enabled = false;
                    undoToolStripMenuItem.Enabled = false;
                }
            }
        }


        private void copyFullPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileOrDirectory(FileOP.COPY_FULLPATH);
        }

        private void notProtectFilesInGrpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = notProtectFilesInGrpToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
            }
            else
            {
                it.Checked = true;
            }
        }

        private void ignoreFileSizeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SetMinimumFileLimit();
            }
        }

        private void ignoreFileSizeTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // !char.IsControl(e.KeyChar) allow Enter.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) // && (e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void autocleanEmptyFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = autocleanEmptyFoldersToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
            }
            else
            {
                it.Checked = true;
            }
        }

        // Keep only one for each group
        private void godsChoiceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (resultListView.Items.Count < 1)
            {
                CWriteLine("!!! ANALYZE First.");
                return;
            }

            try
            {
                var fldFilter = StringToFilter(targetFolderFilterTextBox.Text, true);
                geminiFileClsListForLVUndo = BackUpForUndoRedo(
                    geminiFileClsListForLV, undoToolStripMenuItem);
                MultipleSelectOpAction(resultListView, MultipleSelectOperations.UNCHECK_ALL, force: true);

                var updatedList = new List<GeminiFileCls>();
                var selectList = new List<GeminiFileCls>();
                var godsChoiceList = new List<GeminiFileCls>();
                var tpl = GetGFLbyTheFilter(geminiFileClsListForLV, fldFilter); // may block program
                if (fldFilter.Count > 0)
                {
                    selectList = tpl.Item1;
                    updatedList.AddRange(tpl.Item2);
                    // CWriteLine("tpl.Item2.Count" + tpl.Item2.Count);
                }
                else
                {
                    selectList = tpl.Item2;
                }
                var delGflGrp = GeminiFileClsList2IEnumerableGroup(selectList,
                    SetCompareMode()); // Can not clear, linq will access the GFL later.

                int cntInLoop = 0;
                foreach (var item in delGflGrp)
                {
                    bool first = true;
                    foreach (var it in item)
                    {
                        var tmp = it;
                        if (first)
                        {
                            tmp.Checked = false;
                            first = false;
                        }
                        else
                        {
                            tmp.Checked = true;
                            cntInLoop++;
                        }
                        godsChoiceList.Add(tmp);
                    }
                }
                if (godsChoiceList.Count < 1)
                {
                    CWriteLine($">>> God chose 0 file(s).");
                    SetSummaryBoxText($"God chose 0 file(s).", 0);
                    return;
                }
                updatedList.AddRange(godsChoiceList);
                geminiFileClsListForLV = updatedList;
                var cnt =
                    (from i in geminiFileClsListForLV
                        where i.Checked == true
                        select i).Count();
                // CWriteLine($">>> loop: God chose {cntInLoop:N0} file(s).");
                CWriteLine($">>> God chose {cnt:N0} file(s).");
                SetSummaryBoxText($"God chose {cnt:N0} file(s).", cnt);
                RestoreListViewChoice(geminiFileClsListForLV, resultListView, _source.Token);
            }
            catch (UnauthorizedAccessException) { }
            catch (FileNotFoundException) { }
            catch (Exception ex)
            {
                CWriteLine($"! GodsChoice: {ex.Message}");
            }

        }

        private void alwaysCalculateHashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = alwaysCalculateHashToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
            }
            else
            {
                it.Checked = true;
            }
        }

        // TODO: ONLY SHOW WHEN HOVER OVER THE DIRECTORY.
        private void resultListView_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            if (!cleanEmptyFolderModeToolStripMenuItem.Checked)
            {
                var subI = e.Item.SubItems;
                
                //use cursor points.
                var p = e.Item.ListView.PointToClient(Cursor.Position);
                // e.Item.ListView.Columns.IndexOf(e.Item.ListView.Items.);
                //var where = e.Item.ListView;
                //var sp = new Point(p.X + 25, p.Y + 10);
                // var duration = 3000;
                // when I move the cursor, will no cause disappear.
                if (subI["dir"].Bounds.Contains(p))
                {
                    string itemInfo = subI["dir"].Text;
                    new System.Windows.Forms.ToolTip().SetToolTip(e.Item.ListView, itemInfo);
                    // m_lvToolTip.Show(itemInfo, where, sp, duration);
                }
                /*else if (subI["HASH"].Bounds.Contains(p))
                {
                    string itemInfo = subI["HASH"].Text;
                    m_lvToolTip.Show(itemInfo, where, sp, duration);
                }*/
                else if (subI["name"].Bounds.Contains(p))
                {
                    string itemInfo = subI["name"].Text;
                    // m_lvToolTip.Show(itemInfo, where, sp, duration);
                    new System.Windows.Forms.ToolTip().SetToolTip(e.Item.ListView, itemInfo);
                }
                else
                {
                    // don't know where it's. HIDE, may cover by subitem.
                    //m_lvToolTip.Show("", e.Item.ListView);
                    new System.Windows.Forms.ToolTip().SetToolTip(e.Item.ListView, "");
                }
            }
        }



        private void cleanEmptyFolderModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = cleanEmptyFolderModeToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;

            }
            else
            {
                it.Checked = true;
            }
            ConvertToCEFMode(it.Checked);
            gemini.ini.UpdateIniItem("GeminiMode", it.Checked ? "CEF" : "Duplicate", "Gemini");
        }

        private void modeSelectButton_Click(object sender, EventArgs e)
        {
            cleanEmptyFolderModeToolStripMenuItem.PerformClick();
        }


        private void CalcHashMenuClick()
        {
            var multi = resultListView.SelectedItems;
            if (multi.Count < 1)
            {
                return;
            }
            bool md5 = fileMD5CheckBox.Checked;
            var s = md5 ? "MD5" : "SHA1";
            var total = multi.Count;
            CWriteLine($">>> Start calculating hash[{s}] for {total} file(s)");

            SetProgressBarVisible(geminiProgressBar, true);
            hashTokenSrc = new CancellationTokenSource();
            var token = hashTokenSrc.Token;
            
            var fullPathList = new List<string>();
            foreach (ListViewItem item in multi)
            {
                fullPathList.Add(item.SubItems["fullPath"].Text);
            }
            
            void UpdateHashInLV(string fullPath, string hash)
            {
                resultListView.Items.OfType<ListViewItem>().ToList().ForEach(item =>
                {
                    var fullP = item.SubItems["fullPath"].Text;
                    /*var hash =
                            (from i in geminiFileClsListForLV
                            where i.fullPath.Equals(fullP)
                            select i.hash).ToList()[0];*/
                    if (fullPath.Equals(fullP))
                    {
                        if (!string.IsNullOrEmpty(hash) && !hash.ToLower().Contains("not"))
                        {
                            item.SubItems["HASH"].Text = hash;
                            item.ForeColor = Color.Blue;
                        }
                        return;
                    }
                }
                );
            }

            Task.Run(async () =>
            {
                try
                {
                    int cnt = 0;
                    SetProgressBarVisible(geminiProgressBar, true);
                    foreach (var fullPath in fullPathList)
                    {
                        if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
                        {
                            break;
                        }
                        void getRes(bool res, string who, string _hash, string costTimeOrMsg)
                        {
                            if (res)
                            {
                                geminiFileClsListForLV.ForEach(i => {
                                    if (i.fullPath.Equals(fullPath))
                                    {
                                        i.hash = _hash;
                                        return;
                                    }
                                });
                                UpdateHashInLV(fullPath, _hash);
                                cnt++;
                            }
                        }
                        var pbint = (int)((double)cnt / total * 100);
                        if (pbint > 100)
                        {
                            CWriteLine("! Bug: pbint: " + pbint);
                            pbint = 100;
                        }
                        SetProgressMessage(geminiProgressBar, pbint);
                        if (md5)
                        {
                            await ComputeHashAsync(
                                MD5.Create(), fullPath, token, "MD5", getRes);
                        }
                        else
                        {
                            await ComputeHashAsync(
                                SHA1.Create(), fullPath, token, "SHA1", getRes);
                        }
                    }
                    SetText(summaryTextBox, $"Updated hash[{s}] for {total} file(s)", themeColorClean);
                    CWriteLine($">>> Updated hash[{s}] for {total} file(s)");
                }
                catch (Exception ee)
                {
                    CWriteLine("ComputeHashAsync: " + ee.Message);
                    SetText(summaryTextBox, $"Updated {s} failed.", themeColor);
                }
                finally
                {
                    SetProgressBarVisible(geminiProgressBar, false);
                }

            });
        }

        private void CalcHashMenuClick(string fullPath, ListViewItem item, int total,
            bool md5 = true, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath))
            {
                return;
            }
            void getRes(bool res, string who, string _hash, string costTimeOrMsg)
            {
                if (res)
                {
                    item.SubItems["HASH"].Text = _hash;
                    item.ForeColor = Color.Blue;
                    geminiFileClsListForLV.ForEach(i => {
                        if (i.fullPath.Equals(fullPath))
                        {
                            i.hash = _hash;
                        }
                    });
                    /*if (cnt == total)
                    {*/
                    var s = fileMD5CheckBox.Checked ? "MD5" : "SHA1";
                    SetProgressBarVisible(geminiProgressBar, false);
                    SetText(summaryTextBox, $"Updated {s}", Color.ForestGreen);
                    /*}*/
                    /*var s = md5 ? "MD5" : "SHA1";
                    CWriteLine($".. Update {s} [{_hash}] -> {fullPath}");*/
                }
            }

            Task.Run(async () =>
            {
                try 
                {
                    SetProgressBarVisible(geminiProgressBar, true);
                    void ProgressActionD(double i)
                    {
                        SetProgressMessage(geminiProgressBar, (int)i);
                    }
                    var progessDouble = new Progress<double>(ProgressActionD);
                    if (new FileInfo(fullPath).Length < 100 * 1024 * 1024) // Too fast.
                    {
                        progessDouble = null;
                    }
                    if (md5)
                    {
                        await ComputeHashAsync(
                            MD5.Create(), fullPath, token, "MD5", getRes, progessDouble);
                    }
                    else
                    {
                        await ComputeHashAsync(
                            SHA1.Create(), fullPath, token, "SHA1", getRes, progessDouble);
                    }
                    
                }
                catch (Exception ee)
                {
                    CWriteLine("ComputeHashAsync: " + ee.Message);
                }
                    
            });

        }

        private enum FileOP
        {
            CUT,
            COPY,
            RENAME,
            DELETE,
            OPEN,
            OPENDIR,
            PROPERTIES,
            COPY_FILENAME,
            COPY_FULLPATH
        }

        private void openDirectoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileOrDirectory(FileOP.OPENDIR);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileOrDirectory(FileOP.OPEN);
        }
        
        private void OpenFileOrDirectory(FileOP op)
        {
            string fullPath;
            string fileName;
            try
            {
                fullPath = resultListView.FocusedItem.SubItems["fullPath"].Text;
                fileName = resultListView.FocusedItem.SubItems["name"].Text;
            }
            catch
            {
                fullPath = null;
                fileName = null;
            }
            if (string.IsNullOrEmpty(fullPath))
            {
                return;
            }
            if (File.Exists(fullPath))
            {
                try
                {
                    if (op == FileOP.OPEN)
                    {
                        Process.Start(fullPath);
                    }
                    else if (op == FileOP.OPENDIR)
                    {
                        string argument = "/select, \"" + fullPath + "\"";
                        Process.Start("explorer.exe", argument);
                    } 
                    else if(op == FileOP.DELETE)
                    {
                        FileSystem.DeleteFile(fullPath, UIOption.OnlyErrorDialogs,
                            RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                        CWriteLine($"... Send {fullPath} to RecycleBin");
                        cleanUpButton.PerformClick();
                    }
                    else if (op == FileOP.PROPERTIES)
                    {
                        // Thanks to
                        // https://stackoverflow.com/questions/1936682/how-do-i-display-a-files-properties-dialog-from-c
                        ShowProperties.ShowFileProperties(fullPath);
                    }
                    else if (op == FileOP.COPY_FILENAME)
                    {
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            Clipboard.SetText(fileName);
                            CWriteLine("... Copied file name to Clipboard.");
                        }
                    }
                    else if (op == FileOP.COPY_FULLPATH)
                    {
                        Clipboard.SetText(fullPath);
                        CWriteLine("... Copied full path to Clipboard.");
                    }


                }
                catch (Exception ex)
                {
                    CWriteLine($"!!! {ex.Message}");
                }
                            
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileOrDirectory(FileOP.DELETE);
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*Based off of the services.msc, the page comes from filemgmt.dll and is called ServicePageGeneral. 
             * While the COM components are registered, I cannot find any documentation for the CLSID in question, 
             * nor for any of the other strings present in filemgmt.dll.
              This does not rule out the possibility that there exists an established API, 
            or a command line option to show the dialog, but I certainly can't find one.
              Further substantiating the case that the dialog is not reusable, Process Explorer and SQL 
            Server Configuration Manager both re-implement the dialog, rather than showing the services.msc version.
              Related: How do I open properties box for individual services from command line or link?*/
            OpenFileOrDirectory(FileOP.PROPERTIES);
        }

        // unselect and select all will flush.


        [DebuggerStepThrough]
        private void EnableButtonsForIndexUpdate(bool enable)
        {
            EnableButton(btnAnalyze, enable);
            EnableButton(modeSelectButton, enable);
            EnableButton(cleanUpButton, enable);
            EnableButton(btnDelete, enable);
            selectAllToolStripMenuItem.Enabled = enable;
            godsChoiceToolStripMenuItem.Enabled = enable;
            unselectAllToolStripMenuItem.Enabled = enable;
            reverseElectionToolStripMenuItem.Enabled = enable;
            sorting = !enable;
        }

        private bool sorting = false;
        private void resultListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sorting)
            {
                CWriteLine("! The Index is being updated in the background\r\n" +
                    "and can no longer be sorted, please try again later");
                
                return;
            }
            CWriteLine(">>> Sort and update Index in the background...");
            sorting = true;
            EnableButtonsForIndexUpdate(false);
            if (!cleanEmptyFolderModeToolStripMenuItem.Checked)
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

                // Update Index string in resultListView, index in GeminiFileCls.


                // quick
                ListViewOperate((ListView)sender, ListViewOP.SORT);

                /*void UpdateGFL(bool res, List<GeminiFileCls> gfl, string msg)
                {
                    if (res)
                    {
                        geminiFileClsListForLV = gfl;
                        CWriteLine(">>> Index background update completed");
                    }
                    else
                    {
                        CWriteLine(msg);
                    }
                    sorting = false;
                    EnableButtonsForIndexUpdate(true);
                }*/
                // Update Index string in resultListView, index in GeminiFileCls.
                // very slow, block the program.
                // Task.Run here not work. Must inside the Invoked.
                reIndexTokenSrc = new CancellationTokenSource();

                /*ListViewOperateLoop(resultListView, ListViewOP.UPDATE_INDEX_AFTER_SORTED,
                    geminiFileClsListForLV, UpdateGFL, token: reIndexTokenSrc.Token);*/

                ConvertGeminiFileClsListAndListView(ref geminiFileClsListForLV, resultListView,
                    toListView: false, updateIndex: true, token: reIndexTokenSrc.Token);
                sorting = false;
                EnableButtonsForIndexUpdate(true);
                CWriteLine(">>> Index background update completed");
            }
        }

        private void resultListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            
        }

        private void copyFileNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileOrDirectory(FileOP.COPY_FILENAME);
        }

        private bool deleteKey = false;
        private bool selectAll = true;
        private void resultListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.F2 && resultListView.SelectedItems.Count > 0)
            {
                resultListView.LabelEdit = true;
                resultListView.SelectedItems[0].BeginEdit();
            }
            else if (e.KeyData == Keys.F5)
            {
                cleanUpButton.PerformClick();
            }
            else if (e.KeyData == Keys.Delete)
            {
                deleteKey = true;
                deleteToolStripMenuItem.PerformClick();
            }
            else if (e.KeyData == (Keys.A | Keys.Control))
            {
                if (selectAll)
                    selectAllToolStripMenuItem.PerformClick();
                else
                    unselectAllToolStripMenuItem.PerformClick();
                selectAll = !selectAll;

            }
            else if (e.KeyData == (Keys.G | Keys.Control))
            {
                godsChoiceToolStripMenuItem.PerformClick();
            }
            else if (e.KeyData == (Keys.R | Keys.Control))
            {
                reverseElectionToolStripMenuItem.PerformClick();
            }
            else if (e.KeyData == (Keys.S | Keys.Control))
            {
                saveResultListToFileToolStripMenuItem.PerformClick();
            }
            else if (e.KeyData == (Keys.L | Keys.Control))
            {
                loadListViewFromFileToolStripMenuItem.PerformClick();
            }
            else if (e.KeyData == (Keys.Z | Keys.Control))
            {
                undoToolStripMenuItem.PerformClick();
            }
            else if (e.KeyData == (Keys.Y | Keys.Control))
            {
                redoToolStripMenuItem.PerformClick();
            }
        }

        private void resultListView_DragDrop(object sender, DragEventArgs e)
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
                        UpdateTextAndIniFile("TargetFolder1", path,
                            targetFolder1History, targetFolder1TextBox);
                        btnAnalyze.PerformClick();
                    }
                    else if (File.Exists(path))
                    {
                        if (path.ToLower().EndsWith(".xml"))
                            LoadGeminiFileFileToListView(path);
                        else
                        {
                            UpdateTextAndIniFile("TargetFolder1", Path.GetDirectoryName(path),
                                targetFolder1History, targetFolder1TextBox);
                            CWriteLine(">>> Press button Analyze.");
                        }
                            
                    }
                }
                else
                {
                    CWriteLine("\r\nAttention: File or multiple folders are not allowed!");
                }

            }
        }

        private string SelectXmlFileFromFolder()
        {
            string ret = "";
            using (var dialog = new CommonOpenFileDialog())
            {
                var saveDir = Path.Combine(Path.GetDirectoryName(Assembly.
                    GetExecutingAssembly().Location), "Gemini.UserOperation");
                if (!Directory.Exists(saveDir))
                {
                    saveDir = desktopPath;
                }
                dialog.InitialDirectory = saveDir;
                dialog.IsFolderPicker = false;
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Select xml file"; // "XML files (*.xml)|*.xml";
                dialog.Filters.Add(new CommonFileDialogFilter("Xml file", "*.xml"));
                // maybe add some log

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                   ret = dialog.FileName;
                }
            }
            return ret;
        }
        private void loadListViewFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var xmlFile = SelectXmlFileFromFolder();
            if (!string.IsNullOrEmpty(xmlFile) && File.Exists(xmlFile))
                LoadGeminiFileFileToListView(xmlFile);
        }
        private void GeminiForm_Load(object sender, EventArgs e)
        {

        }

        private void resultListView_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }

        private void calcHashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // CheckedItems of SelectedItems ??? FocusedItem
            /*try
            {
                // var multi = resultListView.CheckedItems;
                var multi = resultListView.SelectedItems;
                if (multi.Count < 1)
                {
                    return;
                }
                var s = fileMD5CheckBox.Checked ? "MD5" : "SHA1";
                CWriteLine($"Start calculating hash[{s}] for {multi.Count} file(s)");
                SetProgressBarVisible(geminiProgressBar, true);
                hashTokenSrc = new CancellationTokenSource();
                foreach (ListViewItem item in multi)
                {
                    CalcHashMenuClick(item.SubItems["fullPath"].Text, item, multi.Count,
                        fileMD5CheckBox.Checked, hashTokenSrc.Token);
                }
            }
            catch (Exception ee)
            {
                CWriteLine(ee.Message);
            }*/
            CalcHashMenuClick();

        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cleanUpButton.PerformClick();
        }

        private void clearFilterbutton_Click(object sender, EventArgs e)
        {
            targetFolderFilterTextBox.Text = "";
            folderFilterTextBox.Text = "";
        }

        private void justThisFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var fld = resultListView.FocusedItem.SubItems["fullPath"].Text;
                targetFolderFilterTextBox.Text = Path.GetDirectoryName(fld);
            }
            catch (Exception ee)
            {
                CWriteLine("Just this folder: Check if ListView has item, " + ee.Message);
            }
        }

        private void selectFilesInThisFolderFirstToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var fld = resultListView.FocusedItem.SubItems["fullPath"].Text;
                folderFilterTextBox.Text = Path.GetDirectoryName(fld);
                targetFolderFilterTextBox.Text = "";
                modeCheckBox.Checked = true;
                updateButton.PerformClick();
            }
            catch (Exception ee)
            {
                CWriteLine("Just this folder: Check if ListView has item, " + ee.Message);
            }
        }

        private void protectThisFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void updateHashFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (geminiFileClsListForLV.Count < 1)
                return;
            var xmlFile = SelectXmlFileFromFolder();
            if (string.IsNullOrEmpty(xmlFile) || !File.Exists(xmlFile))
                return;
            try 
            {
                var gfl = ReadFromXmlFile<List<GeminiFileCls>>(xmlFile);
                var gflHash = 
                    gfl.Where(x => (!x.hash.ToLower().Contains("not") && 
                        !string.IsNullOrEmpty(x.hash)));
                if (gflHash.Count() < 1)
                    return;

                // update hash in geminiFileClsListForLV
                geminiFileClsListForLV.ForEach(d => { d.hash = 
                    gflHash.Where(sd => sd.fullPath.Equals(d.fullPath)).FirstOrDefault().hash;
                    });

                // update hash in ListView
                geminiFileClsListForLV.ForEach(i =>
                {
                    resultListView.Items.OfType<ListViewItem>().ToList().ForEach(item =>
                    {
                        var fullP = item.SubItems["fullPath"].Text;
                        /*var hash =
                                (from i in geminiFileClsListForLV
                                where i.fullPath.Equals(fullP)
                                select i.hash).ToList()[0];*/
                        if (i.fullPath.Equals(fullP))
                        {
                            if (!string.IsNullOrEmpty(i.hash) && !i.hash.ToLower().Contains("not"))
                            {
                                item.SubItems["HASH"].Text = i.hash;
                                item.ForeColor = Color.Blue;
                            }
                            return;
                        }
                    }
                    );
                });
            }
            catch (Exception ex)
            {
                CWriteLine($"Can't read xml file to List<T>: {ex.Message}");
            }            
        }






            
    }
}