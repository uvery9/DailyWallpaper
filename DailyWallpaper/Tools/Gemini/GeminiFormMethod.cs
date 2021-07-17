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
using Button = System.Windows.Forms.Button;
using ListView = System.Windows.Forms.ListView;
using ListViewItem = System.Windows.Forms.ListViewItem;
using System.Text;

// TODO: Use linq more.
/*decimal total = 0;
foreach (Account account in myAccounts)
{
    if (account.Status == "active")
    {
        total += account.Balance;
    }
}
you could just write:
decimal total = (from account in myAccounts
                 where account.Status == "active"
                 select account.Balance).Sum();
*/


namespace DailyWallpaper
{
    partial class GeminiForm
    {
        private enum LoadFileStep
        {
            NO_LOAD,
            STEP_1_ALL_FILES,
            STEP_2_FILES_TO_STRUCT,
            STEP_3_FAST_COMPARE,
            STEP_4_COMPARE_HASH,
            DEFAULT,
            ERROR
        }
        private void LoadGeminiFileFileToListView(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                {
                    return;
                }
                var ret = LoadListFromFile(path);
                var op = ret.Item1;
                var listFromFile = ret.Item2;
                
                if (op == LoadFileStep.ERROR)
                {
                    CWriteLine("!!! LoadFileStep.ERROR");
                    return;
                }
                var folders = new List<string>();
                if (op == LoadFileStep.STEP_1_ALL_FILES)
                {
                    var sl = (List<string>)listFromFile;
                    folders = sl;
                    targetFolder1TextBox.Text = FileList2MaxCommonPathInTextBox(folders);
                    StartAnalyzeStep(op, sL: sl);
                }
                else
                {
                    var gfl = (List<GeminiFileCls>)listFromFile;
                    folders =
                        (from i in gfl
                        select i.fullPath).ToList();
                    targetFolder1TextBox.Text = FileList2MaxCommonPathInTextBox(folders);
                    StartAnalyzeStep(op, gfL: gfl);
                }
                
            }
            catch (Exception ex)
            {
                CWriteLine($"xml -> Struct failed: {ex.Message}");
            }
        }

        // https://stackoverflow.com/questions/68407568/is-there-a-better-way-fastest-to-get-the-longest-common-folder-path/68408555#68408555
        private string FileList2MaxCommonPathInTextBox(List<string> folders) 
        {
            string result;
            try
            {
                var minPathLength = folders.Min(x => x.Length);

                var maxCommonPath = new StringBuilder();
                var currentCommonPath = new StringBuilder();
                for (int i = 0; i < minPathLength; i++)
                {
                    var boolAllSame = true;
                    var c = folders[0][i];
                    boolAllSame = folders.All(x => x[i] == c);

                    if (boolAllSame)
                    {
                        currentCommonPath.Append(c);
                        if (c == '\\')
                        {
                            maxCommonPath.Append(currentCommonPath.ToString());
                            currentCommonPath = new StringBuilder();
                        }
                    }
                    else
                        break;
                }
                result = maxCommonPath.ToString();
            }
            catch (Exception ee)
            {
                result = "";
                CWriteLine("! FileList2MaxCommonPathInTextBox: " + ee.Message);
            }
            CWriteLine(">>> The folder from xml is: " + result);
            return result;
        }
        private Tuple<LoadFileStep, object> LoadListFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Tuple.Create(LoadFileStep.ERROR, (object)null);
            }
            /*
             *  step1-allfiles_1.xml
             *  step1-allfiles_2.xml
             *  step2-filesToStruct_1.xml
             *  step2-filesToStruct_2.xml
             *  step3-FastCompare.xml
             *  step4-CompareHash.xml
             *  step5-RegrpAndColor.xml // color can't be write into xml, delete it.
             */
            object ret;
            LoadFileStep op;
            var pathLow = path.ToLower();
            try
            {
                    if (pathLow.Contains("step1".ToLower()))
                {
                    ret = ReadFromXmlFile<List<string>>(path);
                    op = LoadFileStep.STEP_1_ALL_FILES;
                }
                else if (pathLow.Contains("step2".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileCls>>(path);
                    op = LoadFileStep.STEP_2_FILES_TO_STRUCT;
                }
                else if (pathLow.Contains("step3".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileCls>>(path);
                    op = LoadFileStep.STEP_3_FAST_COMPARE;
                }
                else if (pathLow.Contains("step4".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileCls>>(path);
                    op = LoadFileStep.STEP_4_COMPARE_HASH;
                }
                else
                {
                    ret =
                    ReadFromXmlFile<List<GeminiFileCls>>(path);
                    op = LoadFileStep.DEFAULT;
                }
            }
            catch (Exception ex)
            {
                CWriteLine($"Can't read xml file to List<T>: {ex.Message}");
                op = LoadFileStep.ERROR;
                ret = null;
            }
            return Tuple.Create(op, ret);
        }

        private bool IsSkip(LoadFileStep op, LoadFileStep opcmp)
        {
            return op >= opcmp;
        }
        private async void StartAnalyzeStep(LoadFileStep op = LoadFileStep.NO_LOAD, 
            List<string> sL = null, List<GeminiFileCls> gfL = null)
        {
            _source = new CancellationTokenSource();
            var token = _source.Token;
            ListViewOperate(resultListView, ListViewOP.CLEAR);
            deleteList = new List<string>();
            EnableButton(btnStop, true);
            EnableButton(btnAnalyze, false);
            var limit = SetMinimumFileLimit();
            var f1 = targetFolder1TextBox.Text;
            string name;
            if (string.IsNullOrEmpty(f1) || !Directory.Exists(f1))
            {
                name = "";
            }
            else
            {
                name = new DirectoryInfo(f1).Name;
            }
            var fldname = string.Join("_", name.Split(Path.GetInvalidFileNameChars()));
            void SaveOpHisRes(bool ret, string msg)
            {
                if (ret)
                    CWriteLine(">>>  Save to file, restore progress by \"Load ListView from file\":\r\n    " + msg);
            }
            try
            {
                var _task = Task.Run(() =>
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    // Get all files from folder1/2
                    var filesList1 = new List<string>();
                    var filesList2 = new List<string>();
                    SetProgressBarVisible(geminiProgressBar, true);
                    
                    if (!IsSkip(op, LoadFileStep.STEP_1_ALL_FILES))
                    {
                        bool fld1 = false;
                        bool fld2 = false;
                        var t1 = targetFolder1TextBox.Text;
                        var t2 = targetFolder2TextBox.Text;
                        CWriteLine($">>> Start Analyze Operation...");
                        CWriteLine($">>> Because it is a recursive search, \r\n" +
                            "  Program don't know the progress, please wait patiently...");
                        SetSummaryBoxText("Please wait patiently...", 1);

                        if (!string.IsNullOrEmpty(t2) && Directory.Exists(t2))
                        {
                            fld2 = true;
                            RecurseScanDir(t2, ref filesList2, token);
                            CWriteLine($">>> Found {filesList2.Count} file(s) in: {t2}");
                        }

                        if (!string.IsNullOrEmpty(t1) && Directory.Exists(t1) && !t1.Equals(t2))
                        {
                            fld1 = true;
                            RecurseScanDir(t1, ref filesList1, token);
                            CWriteLine($">>> Found {filesList1.Count} file(s) in: {t1}");
                        }

                        if (!fld2 && !fld1)
                        {
                            CWriteLine("!!! Two folder invalid.");
                            return;
                        }
                        SaveOperationHistory("step1_allfiles_2.xml", filesList2);
                    }
                    else
                    {
                        CWriteLine($">>> Start Analyze Operation...");
                        CWriteLine($">>> Load FileList from file...");
                        CWriteLine($">>> Skip {LoadFileStep.STEP_1_ALL_FILES}... ");
                        filesList1 = sL;
                        filesList2 = new List<string>();
                    }
                    if (op <= LoadFileStep.STEP_1_ALL_FILES)
                        SaveOperationHistory($"step1-allfiles_1-{fldname}.xml", filesList1);

                    var geminiFileClsList1 = new List<GeminiFileCls>();
                    var geminiFileClsList2 = new List<GeminiFileCls>();
                    SetProgressBarVisible(geminiProgressBar, true);
                    if (!IsSkip(op, LoadFileStep.STEP_2_FILES_TO_STRUCT))
                    {
                        // get files info exclude HASH.(FASTER) 
                        FileList2GeminiFileClsList(filesList1, ref geminiFileClsList1, token);
                        FileList2GeminiFileClsList(filesList2, ref geminiFileClsList2, token);
                        SaveOperationHistory("step2_filesToStruct_2.xml", geminiFileClsList2);
                    }
                    else
                    {
                        // get files info exclude HASH.(FASTER) 
                        geminiFileClsList1 = gfL;
                        geminiFileClsList2 = new List<GeminiFileCls>();
                        CWriteLine($">>> Skip {LoadFileStep.STEP_2_FILES_TO_STRUCT}... ");
                    }
                    if (op <= LoadFileStep.STEP_2_FILES_TO_STRUCT)
                        SaveOperationHistory($"step2-filesToStruct_1-{fldname}.xml", geminiFileClsList1);
                    
                    List<GeminiFileCls> sameListNoDup;
                    var mode = SetCompareMode();
                    if (!IsSkip(op, LoadFileStep.STEP_3_FAST_COMPARE))
                    {
                        CWriteLine(">>> Start Fast Compare...");
                        // compare folders and themselves, return duplicated files list.
                        sameListNoDup = ComparerTwoFolderGetList(geminiFileClsList1,
                                geminiFileClsList2, mode, limit, token, geminiProgressBar).Result;
                        CWriteLine(">>> Fast Compare finished...");
                    }
                    else
                    {
                        sameListNoDup = gfL;
                        CWriteLine($">>> Skip {LoadFileStep.STEP_3_FAST_COMPARE}... ");
                        if (ignoreFileCheckBox.Checked)
                        {
                            sameListNoDup =
                                (from i in sameListNoDup
                                 where i.size > limit
                                 select i).ToList();
                        }
                    }

                    if (op <= LoadFileStep.STEP_3_FAST_COMPARE)
                        SaveOperationHistory($"step3-FastCompare-{fldname}.xml", sameListNoDup, SaveOpHisRes);
                    
                    if (fileMD5CheckBox.Checked || fileSHA1CheckBox.Checked)
                    {
                        if (!IsSkip(op, LoadFileStep.STEP_4_COMPARE_HASH))
                        {
                            CWriteLine($">>> Update HASH for {sameListNoDup.Count:N0} file(s)...");
                            SetProgressBarVisible(geminiProgressBar, true);
                            sameListNoDup =
                            UpdateHashInGeminiFileClsList(sameListNoDup, token,
                                alwaysCalculateHashToolStripMenuItem.Checked).Result;
                            SaveOperationHistory($"step4-CompareHashForLittleFiles-{fldname}.xml", 
                                sameListNoDup, SaveOpHisRes);
                            var sameListNoDupHash = new List<GeminiFileCls>();
                            var sameListNoDupBigFiles = new List<GeminiFileCls>();
                            foreach (var sl in sameListNoDup)
                            {
                                if (sl.bigFile)
                                {
                                    sameListNoDupBigFiles.Add(sl);
                                }
                                else
                                {
                                    sameListNoDupHash.Add(sl);
                                }
                            }
                            CWriteLine($">>> Found " +
                                    $"{sameListNoDupBigFiles.Count:N0} bigfile(s), do not calculate the hash value...");
                            if (sameListNoDupBigFiles.Count < 30)
                            {
                                CWriteLine($">>> Update HASH for remaining " +
                                    $"{sameListNoDupBigFiles.Count:N0} bigfile(s)...");
                                sameListNoDupBigFiles =
                                    UpdateHashInGeminiFileClsList(sameListNoDupBigFiles,
                                    token, true).Result;
                                // UpdateHash For BigFiles.
                            }
                            sameListNoDupHash.AddRange(sameListNoDupBigFiles);
                            sameListNoDup = sameListNoDupHash;
                            CWriteLine(">>> Update HASH finished.");
                        }
                        else
                        {
                            sameListNoDup = gfL;
                            CWriteLine($">>> Skip {LoadFileStep.STEP_4_COMPARE_HASH}... ");

                        }
                        if (op <= LoadFileStep.STEP_4_COMPARE_HASH)
                            SaveOperationHistory($"step4-CompareHash.xml-{fldname}", sameListNoDup, SaveOpHisRes);
                    }

                    // Color by Group.
                    CWriteLine(">>> Regroup and ReColor for ListView...");
                    geminiFileClsListForLV = ListReColorByGroup(sameListNoDup, mode, token);

                    CWriteLine(">>> Update to ListView...");
                    UpdateListView(resultListView, ref geminiFileClsListForLV, token);

                    timer.Stop();
                    CWriteLine($">>> Cost time: {GetTimeStringMsOrS(timer.Elapsed)}");

                }, _source.Token);

                _tasks.Add(_task);
                await _task;
                // No Error, filesList is usable
                scanRes = true;
                /*WriteToXmlFile("GeminiListLatest.xml",
                        geminiFileClsListForLV);*/
                SaveOperationHistory($"GeminiListLatest-{fldname}.xml", geminiFileClsListForLV, SaveOpHisRes);
            }
            catch (OperationCanceledException e)
            {
                scanRes = false;
                CWriteLine($"\r\n>>> OperationCanceledException: {e.Message}");
            }
            catch (AggregateException e)
            {
                CWriteLine($"\r\n>>> AggregateException[Cancel exception]: {e.Message}");
            }
            catch (Exception e)
            {
                scanRes = false;
                CWriteLine($"\r\n RecurseScanDir throw exception message: {e}");
                CWriteLine($"\r\n#----^^^  PLEASE CHECK, TRY TO CONTACT ME WITH THIS LOG.  ^^^----#");
            }
            finally
            {
                geminiProgressBar.Visible = false;
                GetCurrentScanStatus();
                CWriteLine(">>> Analyse is over.");
            }
            btnAnalyze.Enabled = true;

        }

        private delegate void EnableButtonDelegate(Button b, bool enable);

        [DebuggerStepThrough]
        private void EnableButton(Button b, bool enable)
        {
            if (b.InvokeRequired) //  && b.IsHandleCreated
            {
                var f = new EnableButtonDelegate(EnableButton);
                b.Invoke(f, new object[] { b, enable });
            }
            else
            {
                b.Enabled = enable;
            } 
        }

        private enum ListViewOP
        {
            ADD,
            ADDRANGE,
            CLEAR,
            UPDATE_CHECK,
            UPDATE_CHECK_INTHELOOP,
            UPDATE_CHECK_BY_INDEX,
            CEF_INTHELOOP,
            SORT
        }


        [DebuggerStepThrough]
        private void CWriteLine(object msg)
        {
            if (msg == null)
            {
                return;
            }
            if (msg.GetType().Equals("".GetType()))
            {
                // _console.WriteLine(msg.GetType());
                _console.WriteLine(msg);
            }
            else
            {
                _console.WriteLine(msg.ToString());
            }
            
        }

        [DebuggerStepThrough]
        private void CWriteLineFast(object msg)
        {
            if (msg == null)
            {
                return;
            }
            if (msg.GetType().Equals("".GetType()))
            {
                // _console.WriteLine(msg.GetType());
                tbConsole.Text += msg + Environment.NewLine;
            }
            else
            {
                tbConsole.Text += msg.ToString() + Environment.NewLine;
            }

        }


        private delegate void ListViewOperateLoopDelegate(System.Windows.Forms.ListView liv,
            ListViewOP op, List<GeminiFileCls> gfl = null,
            Action<bool, List<GeminiFileCls>, string> actionLoop = null, 
            CancellationToken token = default);

        private void ListViewOperateLoop(ListView liv,
            ListViewOP op, List<GeminiFileCls> gfl = null,
            Action<bool, List<GeminiFileCls>, string> actionLoop = null, CancellationToken token = default)
        {
            if (liv.InvokeRequired)
            {
                var addDele = new ListViewOperateLoopDelegate(ListViewOperateLoop);
                liv.Invoke(addDele, new object[] { liv, op, gfl, actionLoop, token });
            }
            else
            {
                if (op == ListViewOP.UPDATE_CHECK_INTHELOOP)
                {
                    var updateCheckLoopTask = Task.Run(() => { 
                        try
                        {
                            var tmpL = new List<GeminiFileCls>();
                            foreach (var it in liv.Items)
                            {
                                var lvi = (ListViewItem)it;
                                var fullPathLV = lvi.SubItems["fullPath"].Text;
                                foreach (var gf in gfl)
                                {
                                    if (token.IsCancellationRequested)
                                    {
                                        token.ThrowIfCancellationRequested();
                                    }
                                    var tmp = gf;
                                    if (gf.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                                    {
                                        tmp.Checked = lvi.Checked;
                                        tmpL.Add(tmp);
                                        break;
                                    }
                                }
                            }
                            actionLoop(true, tmpL, "");
                        }
                        catch(Exception ex)
                        {
                            actionLoop(false, null, ex.Message);
                        }
                    });
                    _tasks.Add(updateCheckLoopTask);

                }
                else if (op == ListViewOP.UPDATE_CHECK_BY_INDEX)
                {
                    var updateCheckIndexTask = Task.Run(() => { 
                        try
                        {
                            var tmpL = new List<GeminiFileCls>();
                            foreach (var gf in gfl)
                            {
                                if (token.IsCancellationRequested)
                                {
                                    token.ThrowIfCancellationRequested();
                                }
                                var tmp = gf;
                                var lvi = liv.Items[tmp.index];
                                var fullPathLV = lvi.SubItems["fullPath"].Text;
                                if (gf.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                                {
                                    tmp.Checked = lvi.Checked;
                                    tmpL.Add(tmp);
                                }
                                else
                                {
                                    actionLoop(false, tmpL, "Not Equal");
                                }
                            }
                            actionLoop(true, tmpL, "");
                        }
                        catch(Exception ex)
                        {
                            actionLoop(false, null, ex.Message);
                        }
                    });
                    _tasks.Add(updateCheckIndexTask);

                }
                // DOES NOT WORK.
                /*else if (op == ListViewOP.UPDATE_INDEX_AFTER_SORTED)
                {
                    var tmpL = new List<GeminiFileCls>();
                    var updateIndex = Task.Run();
                    updateIndex.ContinueWith((t) =>
                     {
                         if (t.Exception != null) throw t.Exception;
                         if (t.IsCompleted)
                         {
                             Debug.WriteLine("IsCompleted");
                             CWriteLine("IsCompleted");
                             actionLoop(true, tmpL, "");
                         }
                    });
                    _tasks.Add(updateIndex);
                }*/
            }
        }

        // https://stackoverflow.com/questions/17746013/how-to-change-order-of-columns-of-listview
        // https://docs.microsoft.com/en-us/troubleshoot/dotnet/csharp/sort-listview-by-column
        
        


        public static void SetListViewText(ListView lv, int index, string name, string text)
        {
            if (lv.InvokeRequired)
            {
                lv.Invoke(new MethodInvoker(delegate () { SetListViewText(lv, index, name , text); }));
            }
            else
            {
                lv.Items[index].SubItems[name].Text = text;
            }
        }


        /*public static void InvokeClearListViewItems(ListView listView)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new MethodInvoker(delegate () { InvokeClearListViewItems(listView); }));
            }
            else
            {
                listView.Items.Clear();
            }
        }*/

        private delegate ListView.ListViewItemCollection GetItems(ListView lstview);
        private ListView.ListViewItemCollection GetListViewItems(ListView lstview)
        {
            if (lstview.InvokeRequired)
            {
                return (ListView.ListViewItemCollection)
                    lstview.Invoke(new GetItems(GetListViewItems), new object[] { lstview });
                
            }
            else
            {
                var temp = new ListView.ListViewItemCollection(new ListView());
                foreach (ListViewItem item in lstview.Items)
                    temp.Add((ListViewItem)item.Clone());
                // return temp;
                return lstview.Items;
            }
        }


        private void ConvertToCEFMode(bool cef = false)
        {
            ListViewOperate(resultListView, ListViewOP.CLEAR);
            cefScanRes = false;
            if (cef)
            {
                targetFolder2TextBox.Text = "Now in Clean Empty Folders mode";
                targetFolder2TextBox.Enabled = false;
                targetFolder2TextBox.TextAlign = HorizontalAlignment.Center;
                btnSelectTargetFolder2.Enabled = false;

                fileMD5CheckBox.Enabled = false;
                fileNameCheckBox.Enabled = false;
                fileExtNameCheckBox.Enabled = false;
                fileSHA1CheckBox.Enabled = false;
                updateButton.Enabled = false;
                ignoreFileCheckBox.Enabled = false;
                ignoreFileSizecomboBox.Enabled = false;
                ignoreFileSizeTextBox.Enabled = false;
                cleanUpButton.Enabled = false;

                alwaysCalculateHashToolStripMenuItem.Enabled = false;
                notProtectFilesInGrpToolStripMenuItem.Enabled = false;
                autocleanEmptyFoldersToolStripMenuItem.Enabled = false;

                godsChoiceToolStripMenuItem.Enabled = false;

                geminiProgressBar.Visible = false;
                nameColumnHeader.Width = (int)(1.5 * nameColumnHeaderWidth);
                modifiedTimeColumnHeader.Width = (int)(1.5 * modifiedTimeColumnHeaderWidth);
                targetFolderFilterTextBox.Enabled = false;

            }
            else
            {
                targetFolder2TextBox.Text = "";
                targetFolder2TextBox.Enabled = true;
                targetFolder2TextBox.TextAlign = default;
                btnSelectTargetFolder2.Enabled = true;

                fileMD5CheckBox.Enabled = true;
                fileNameCheckBox.Enabled = true;
                fileExtNameCheckBox.Enabled = true;
                fileSHA1CheckBox.Enabled = true;
                updateButton.Enabled = true;
                ignoreFileCheckBox.Enabled = true;
                ignoreFileSizecomboBox.Enabled = true;
                ignoreFileSizeTextBox.Enabled = true;
                cleanUpButton.Enabled = true;

                alwaysCalculateHashToolStripMenuItem.Enabled = true;
                notProtectFilesInGrpToolStripMenuItem.Enabled = true;
                autocleanEmptyFoldersToolStripMenuItem.Enabled = true;

                godsChoiceToolStripMenuItem.Enabled = true;

                geminiProgressBar.Visible = true;

                nameColumnHeader.Width = nameColumnHeaderWidth;
                modifiedTimeColumnHeader.Width = modifiedTimeColumnHeaderWidth;
                targetFolderFilterTextBox.Enabled = true;
            }
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

        private void UpdateCheckedInDelGFL(List<GeminiFileCls> gfl, List<string> delList, GeminiFileCls item)
        {
            item.Checked = false;
            foreach (var it in delList)
            {
                if (item.fullPath.ToLower().Equals(it.ToLower()))
                {
                    item.Checked = true;
                }
            }
            gfl.Add(item);
        }
        private void DeleteCEF()
        {
            if (!cefScanRes)
            {
                btnAnalyze.PerformClick();
            }
            var deleteCEF = Task.Run(() => {
                foreach (var item in resultListView.Items)
                {
                    var it = (System.Windows.Forms.ListViewItem)item;
                    var fullPathLV = it.SubItems["name"].Text;
                    if (it.Checked && Directory.Exists(fullPathLV))
                    {
                        CWriteLine($"delete ###  {fullPathLV}");
                        FileSystem.DeleteDirectory(fullPathLV, UIOption.OnlyErrorDialogs,
                        deleteOrRecycleBin.Checked ?
                        RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin,
                        UICancelOption.DoNothing);
                    }
                }
                var tmpL = new List<GeminiCEFStruct>();
                foreach (var item in geminiCEFStructList)
                {
                    if (Directory.Exists(item.fullPath))
                    {
                        tmpL.Add(item);
                    }
                }
                geminiCEFStructList = tmpL;
                UpdateEmptyFoldersToLV(geminiCEFStructList, _source.Token);
                UpdateCEFChecked(geminiCEFStructList);
                cefScanRes = false;
            });
            _tasks.Add(deleteCEF);
        }
        private void EmptyJudge(string dir)
        {
            if (!Directory.Exists(dir))
            {
                return;
            }
            var entries = Directory.EnumerateFileSystemEntries(dir);
            if (!entries.Any())
            {
                try
                {
                    FileSystem.DeleteDirectory(dir, UIOption.OnlyErrorDialogs,
                        deleteOrRecycleBin.Checked ?
                        RecycleOption.DeletePermanently : RecycleOption.SendToRecycleBin,
                        UICancelOption.DoNothing);
                    CWriteLine($"...... Delete empty folder:  {dir}");
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
            }
        }

        private void EmptyJudgeCEF(string dir)
        {
            var entries = Directory.EnumerateFileSystemEntries(dir);
            if (!entries.Any())
            {
                try
                {
                    string lastMtime = "";
                    try
                    {
                        lastMtime = new FileInfo(dir).LastWriteTime.ToString();
                    }
                    catch (Exception ex)
                    {
                        lastMtime = ex.Message;
                    }
                    var geminiCEF = new GeminiCEFStruct();
                    geminiCEF.name = dir;
                    geminiCEF.fullPath = dir;
                    geminiCEF.Checked = false;
                    geminiCEF.lastMtime = lastMtime;
                    geminiCEFStructList.Add(geminiCEF);
                    CWriteLine($"... Found empty folder: {dir}");
                }
                catch (UnauthorizedAccessException) { }
                catch (DirectoryNotFoundException) { }
            }
        }

        void FileList2GeminiFileClsList(List<string> filesList,
            ref List<GeminiFileCls> gList, CancellationToken token)
        {
            gList = new List<GeminiFileCls>();
            if (filesList.Count > 0)
            {
                SetProgressMessage(geminiProgressBar, 0);
                CWriteLine(">>> Start collecting all files...");
                int i = 0;
                foreach (var f in filesList)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    gList.Add(new GeminiFileCls(f));
                    i++;
                    if (i % 100 == 0)
                    {
                        _mutex.WaitOne();
                        SetProgressMessage(geminiProgressBar, (int)((double)i / filesList.Count * 100));
                        _mutex.ReleaseMutex();
                    }
                }
                CWriteLine(">>> All files collected.");
            }
        }

        private void FindFilesInDir(string dir, List<string> filesList, CancellationToken token)
        {
            try
            {
                foreach (var fi in Directory.EnumerateFiles(dir))
                {
                    filesList.Add(fi);
                    // CWriteLine($"print >>>  {fi}");
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
                    CWriteLine($"\r\nThe folder is CONTROLLED, please re-select:\r\n   {path}");
                    CWriteLine("\r\nYou could Type \" list controlled \" in the \r\n" +
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
                    CWriteLine($"\r\nThe {keyInIni} folder dose NOT EXIST, please re-select:\r\n   {path}");
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
                CWriteLine($">>> You have selected {keyInIni} folder:\r\n  {path}");
            }
            return true;
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
            CWriteLine($"\r\n >>> FilterMode: {filterMode}");
            UpdateFilterExampleText(filterMode);
            gemini.ini.UpdateIniItem("FilterMode", filterMode.ToString(), "Gemini");
        }


        private delegate void ListViewOperateDelegate(System.Windows.Forms.ListView liv, ListViewOP op,
            ListViewItem item = null, bool ischeck = false,
            ListViewItem[] items = null, Action<bool, string> action = null);
        private void ListViewOperate(ListView liv, ListViewOP op,
            ListViewItem item = null, bool ischeck = false,
            ListViewItem[] items = null, Action <bool, string> action = null
           )
        {
            if (liv.InvokeRequired)
            {
                var func = new ListViewOperateDelegate(ListViewOperate);
                liv.Invoke(func, new object[] { liv, op, item, ischeck, items, action });
            }
            else
            {
                if (op == ListViewOP.ADD)
                {
                    liv.Items.Add(item);
                }
                if (op == ListViewOP.CLEAR)
                {
                    liv.Items.Clear();
                }
                if (op == ListViewOP.UPDATE_CHECK)
                {
                    item.Checked = ischeck;
                }
                if (op == ListViewOP.ADDRANGE)
                {
                    liv.Items.AddRange(items);
                }
                if (op == ListViewOP.SORT)
                {
                    liv.BeginUpdate();
                    liv.Sort();
                    liv.EndUpdate();
                }
            }
        }

        private enum MultipleSelectOperations
        {
            CHECK_ALL,
            UNCHECK_ALL,
            REVERSE_ELECTION
        };

        private void MultipleSelectOpAction(
            ListView liv, MultipleSelectOperations op, bool force = false)
        {
            if (_source == null)
            {
                _source = new CancellationTokenSource();
            }
            if (liv.Items.Count < 1)
            {
                CWriteLine("!!! ANALYZE First.");
                return;
            }
            /*liv.BeginUpdate();
            if (op == MultipleSelectOperations.CHECK_ALL)
            {
                liv.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = true);
            }
            else if(op == MultipleSelectOperations.UNCHECK_ALL)
            {
                liv.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = false);
            }
            else if (op == MultipleSelectOperations.REVERSE_ELECTION)
            {
                liv.Items.OfType<ListViewItem>().ToList().ForEach(item => item.Checked = !item.Checked);
            }
            liv.EndUpdate();

            // update geminiFileClsListForLV
            geminiFileClsListForLVUndo = geminiFileClsListForLV;
            undoToolStripMenuItem.Enabled = true;
            ConvertGeminiFileClsListAndListView(ref geminiFileClsListForLV, liv, 
                toListView: false, token: _source.Token);*/
            geminiFileClsListForLVUndo = BackUpForUndoRedo(
                 geminiFileClsListForLV, undoToolStripMenuItem);

            var updatedList = new List<GeminiFileCls>();
            var selectList = new List<GeminiFileCls>();
            var fldFilter = StringToFilter(targetFolderFilterTextBox.Text, !force);
            var tpl = GetGFLbyTheFilter(geminiFileClsListForLV, fldFilter);
            if (fldFilter.Count > 0)
            {
                selectList = tpl.Item1;
                if(!force)
                    updatedList.AddRange(tpl.Item2);
            }
            else
            {
                selectList = tpl.Item2;
            }
            if (force)
                selectList = geminiFileClsListForLV;
            if (op == MultipleSelectOperations.CHECK_ALL)
            {
                selectList.ForEach(item => item.Checked = true);
            }
            else if (op == MultipleSelectOperations.UNCHECK_ALL)
            {
                selectList.ForEach(item => item.Checked = false);
            }
            else if (op == MultipleSelectOperations.REVERSE_ELECTION)
            {
                selectList.ForEach(item => item.Checked = !item.Checked);
            }
            Debug.WriteLine(selectList[0].Checked);
            Debug.WriteLine(geminiFileClsListForLV[0].Checked); // why change me, FU.

            updatedList.AddRange(selectList);

            // update geminiFileClsListForLV
            // Debug.WriteLine("1." + geminiFileClsListForLV[0].Checked); // why change me, FU.
            geminiFileClsListForLV = updatedList; // can remove.
            var cnt =
                (from i in geminiFileClsListForLV
                 where i.Checked == true
                 select i).Count();
            if (!force)
            {
                CWriteLine($">>> {op} Selected {cnt:N0} file(s).");
                SetSummaryBoxText($"{op} Selected {cnt:N0} file(s).", cnt);
            }
                
            /*geminiFileClsListForLV[0].fullPath = "TEST....";
            CWriteLine("2.uodo." + geminiFileClsListForLVUndo[0].fullPath); // why change me, FU.
            CWriteLine("3." + geminiFileClsListForLV[0].fullPath); // why change me, FU.*/
            ConvertGeminiFileClsListAndListView(ref geminiFileClsListForLV, liv,
                toListView: true, token: _source.Token);
        }

        private void ConvertGeminiFileClsListAndListView(ref List<GeminiFileCls> rgfL,
            ListView lv, bool toListView = true, bool updateIndex = false, CancellationToken token = default, 
            Action<bool, string> action = null)
        {
            if (toListView)
            {
                foreach (var gf in rgfL)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    Application.DoEvents();
                    if (lv.Items.Count < 1)
                    {
                        return;
                    }
                    var it = lv.Items[gf.index];
                    var fullPathLV = it.SubItems["fullPath"].Text;
                    if (File.Exists(gf.fullPath) && fullPathLV.Equals(gf.fullPath))
                    {
                        ListViewOperate(lv, ListViewOP.UPDATE_CHECK, it, gf.Checked);
                    }
                }
                if (action != null)
                    action(true, "Succeed");
            }
            else
            {
                if (updateIndex)
                {
                    var tmpL = new List<GeminiFileCls>();
                    int index = 0;
                    foreach (ListViewItem lvi in lv.Items)
                    {
                        /*CWriteLine(liv.Items.IndexOf(lvi));*/
                        // how to update index in SubItems
                        lvi.SubItems["index"].Text = index.ToString();
                        lvi.SubItems[0].Text = index.ToString();
                        var fullPathLV = lvi.SubItems["fullPath"].Text;
                        foreach (var gf in rgfL)
                        {
                            if (token.IsCancellationRequested)
                            {
                                token.ThrowIfCancellationRequested();
                            }
                            Application.DoEvents();
                            if (gf.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                            {
                                var tmp = gf;
                                tmp.index = index;
                                tmpL.Add(tmp);
                                break;
                            }
                        }
                        index++;
                    }
                    rgfL = tmpL;
                }
                else
                {
                    var tmpL = new List<GeminiFileCls>();
                    foreach (ListViewItem item in lv.Items)
                    {
                        var fullPath = item.SubItems["fullPath"].Text;
                        foreach (var gl in rgfL)
                        {
                            if (token.IsCancellationRequested)
                            {
                                token.ThrowIfCancellationRequested();
                            }
                            Application.DoEvents();
                            if (fullPath.Equals(gl.fullPath))
                            {
                                var glt = gl;
                                glt.Checked = item.Checked;
                                tmpL.Add(glt);
                            }
                        }
                    }
                    rgfL = tmpL;
                    Debug.WriteLine(">>> Succeed convert lv to gfL.");
                }

            }
        }

        // not good enough
        /*private void MultipleSelectOperationsAction(
            ListView liv, MultipleSelectOperations op)
        {
            if (liv.Items.Count < 1)
            {
                return;
            }
            geminiFileClsListForLVUndo = geminiFileClsListForLV;
            undoToolStripMenuItem.Enabled = true;
            var mpTask = Task.Run(() => {
                try
                {
                    if (op == MultipleSelectOperations.REVERSE_ELECTION)
                    {
                        geminiFileClsListForLV = UpdateGFLChecked(geminiFileClsListForLV)
                            ?? geminiFileClsListForLV;
                    }
                    var tmpGfl = new List<GeminiFileCls>();
                    foreach (var item in geminiFileClsListForLV)
                    {
                        var tmp = item;
                        if (op == MultipleSelectOperations.REVERSE_ELECTION)
                        {
                            if (item.Checked)
                            {
                                tmp.Checked = false;
                            }
                            else
                            {
                                tmp.Checked = true;
                            }
                        }
                        else if (op == MultipleSelectOperations.CHECK_ALL)
                        {
                            tmp.Checked = true;
                        }
                        else if (op == MultipleSelectOperations.UNCHECK_ALL)
                        {
                            tmp.Checked = false;
                        }
                        tmpGfl.Add(tmp);
                    }
                    if (tmpGfl.Count < 1)
                    {
                        return;
                    }
                    geminiFileClsListForLV = tmpGfl;
                    RestoreListViewChoiceInvoke(liv, geminiFileClsListForLV, _source.Token);
                }
                catch (Exception ex)
                {
                    CWriteLine($"{ex}");
                    CWriteLine($"{ex.Message}");
                }
            });
            _tasks.Add(mpTask);
        }*/

        private void MultipleSelectOperationsActionCEF(MultipleSelectOperations op)
        {
            var mpCEFTask = Task.Run(() => {
                if (resultListView.Items.Count < 1)
                {
                    return;
                }
                undoToolStripMenuItem.Enabled = true;
                // geminiCEFStructListRedo
                if (op == MultipleSelectOperations.REVERSE_ELECTION)
                {
                    geminiCEFStructList = UpdateCEFChecked(geminiCEFStructList)
                        ?? geminiCEFStructList;
                }
                var tmpGfl = new List<GeminiCEFStruct>();
                if (geminiCEFStructList.Count < 1)
                {
                    return;
                }
                foreach (var item in geminiCEFStructList)
                {
                    var tmp = item;
                    if (op == MultipleSelectOperations.REVERSE_ELECTION)
                    {
                        if (item.Checked)
                        {
                            tmp.Checked = false;
                        }
                        else
                        {
                            tmp.Checked = true;
                        }
                    }
                    else if (op == MultipleSelectOperations.CHECK_ALL)
                    {
                        tmp.Checked = true;
                    }
                    else if (op == MultipleSelectOperations.UNCHECK_ALL)
                    {
                        tmp.Checked = false;
                    }
                    tmpGfl.Add(tmp);
                }
                if (tmpGfl.Count < 1)
                {
                    return;
                }
                geminiCEFStructList = tmpGfl;
                RestoreCEFListViewChoice(geminiCEFStructList, _source.Token);
            });
            _tasks.Add(mpCEFTask);
        }

        private bool GeminiFileClsListREForEach(GeminiFileCls item, 
            Regex rege, bool find = true)
        {
            bool ret = !find;
            if (rege.IsMatch(item.fullPath))
            {
                ret = find;
            }
            return ret;
        }

        private Tuple<List<GeminiFileCls>, List<GeminiFileCls>> 
            GetGFLbyTheFilter(
            List<GeminiFileCls> gfL, List<string> fldFilter)
        {
            var gflIn = new List<GeminiFileCls>();
            var gflNotIn = new List<GeminiFileCls>();
            foreach (var item in gfL) // why always change the value in gfL???
            {
                bool inSide = false;
                foreach (var fil in fldFilter)
                {
                    if (item.fullPath.Contains(fil))
                    {
                        gflIn.Add(item);
                        inSide = true;
                        break;
                    }
                    Application.DoEvents();
                }
                if (!inSide)
                    gflNotIn.Add(item);
            }
            // CWriteLine("gflNotIn.Count = " + gflNotIn.Count);
            /*CWriteLine($"total.cnt={gfL.Count}");
            CWriteLine($"in.cnt={gflIn.Count}");
            CWriteLine($"Notin.cnt={gflNotIn.Count}");*/
            return Tuple.Create(gflIn, gflNotIn);
        }

        private bool GeminiFileClsListGeneralForEach(GeminiFileCls item, List<string> filter, 
            bool find = true)
        {
            bool ret = !find;
            foreach (var it in filter)
            {
                if (item.fullPath.ToLower().Contains(it.ToLower()))
                {
                    ret = find;
                    break;
                }
            }
            return ret;
        }

        private void RestoreCEFListViewChoice(List<GeminiCEFStruct> gcefl, CancellationToken token)
        {
            if (resultListView.Items.Count > 0)
            {
                foreach (var item in resultListView.Items)
                {
                    var it = (System.Windows.Forms.ListViewItem)item;
                    var fullPathLV = it.SubItems["name"].Text;
                    foreach (var gcef in gcefl)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        if (Directory.Exists(gcef.fullPath) && fullPathLV.Equals(gcef.fullPath))
                        {
                            it.Checked = gcef.Checked;
                        }
                    }
                }
            }
        }

        private delegate void RestoreListViewChoiceInvokeDele(ListView liv, List<GeminiFileCls> gfl, 
            CancellationToken token, bool indexChange = false, Action<bool, string> action = default);


         private void RestoreListViewChoice(List<GeminiFileCls> gfl, ListView liv,
            CancellationToken token)
        {
            ConvertGeminiFileClsListAndListView(ref gfl,
                        liv, toListView: true, token: token);
        }

        private void RestoreListViewChoiceInvoke(ListView liv, List<GeminiFileCls> gfl, 
            CancellationToken token, bool indexChange = false, Action<bool, string> action = default)
        {
            if (liv.InvokeRequired)
            {
                var f = new RestoreListViewChoiceInvokeDele(RestoreListViewChoiceInvoke);
                liv.Invoke(f, new object[] { liv, gfl, token, indexChange, action });
            }
            else
            {
                var restoreTask = Task.Run(() => {
                try
                {
                    if (indexChange)
                    {
                        throw new CustomAttributeFormatException(">>> Clean-UP Restore Mode.");
                    }
                    foreach (var gf in gfl)
                    {
                        if (token.IsCancellationRequested)
                        {
                            token.ThrowIfCancellationRequested();
                        }
                        if (liv.Items.Count < 1)
                        {
                            return;
                        }
                        var it = liv.Items[gf.index];
                        var fullPathLV = it.SubItems["fullPath"].Text;
                        if (File.Exists(gf.fullPath) && fullPathLV.Equals(gf.fullPath))
                        {
                            ListViewOperate(liv, ListViewOP.UPDATE_CHECK, it, gf.Checked);
                        }
                    }
                    action(true, "Fast Mode Finished");
                        // CWriteLine("FAST No Exception..........");
                }
                catch (CustomAttributeFormatException ex)
                {
                    Debug.WriteLine($"{ex.Message}");
                    if (liv.Items.Count < 1)
                    {
                        return;
                    }
                    try
                    {
                        foreach (var gf in gfl)
                        {
                            // foreach (ListViewItem item in ListViewName.Items) block program.

                            foreach (ListViewItem it in GetListViewItems(liv))
                            {
                                var fullPathLV = it.SubItems["fullPath"].Text;
                                if (token.IsCancellationRequested)
                                {
                                    token.ThrowIfCancellationRequested();
                                }
                                if (File.Exists(gf.fullPath) && fullPathLV.Equals(gf.fullPath))
                                {
                                    ListViewOperate(liv, ListViewOP.UPDATE_CHECK, it, gf.Checked);
                                }
                            }
                        }                       
                        Debug.WriteLine("Slow Mode: " + ex.Message);
                        action(true, "Slow Mode: " + ex.Message);
                    }
                    catch (Exception exr)
                    {
                        // CWriteLine("! Error: " + exr.Message);
                        Debug.WriteLine("! Error: " + exr);
                        action(false, "! Error: " + exr.Message);
                    }
                }
                catch (Exception ext)
                {
                        Debug.WriteLine("! ext: " + ext.Message);
                }
                finally
                {
                        Debug.WriteLine("---------finally: ");
                }
                }); 
                _tasks.Add(restoreTask);

            }

        }

        private void UpdateListViewForCleanUP(ListView liv,
            ref List<GeminiFileCls> gfL, CancellationToken token)
        {

        }

        private void UpdateListView(ListView liv,
            ref List<GeminiFileCls> gfL, CancellationToken token)
        {
            ListViewOperate(liv, ListViewOP.CLEAR);
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            var tmpL = new List<GeminiFileCls>();
            if (gfL.Count > 0)
            {
                int index = 0;
                var items = new List<ListViewItem>();
                foreach (var gf in gfL)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    var item = new ListViewItem(index.ToString())
                    {
                        BackColor = gf.color
                    };
                    AddSubItem(item, "name", gf.name);
                    AddSubItem(item, "lastMtime", gf.lastMtime);
                    AddSubItem(item, "extName", gf.extName);
                    AddSubItem(item, "sizeStr", gf.sizeStr);
                    AddSubItem(item, "dir", gf.dir);
                    AddSubItem(item, "HASH", gf.hash ?? "");
                    AddSubItem(item, "fullPath", gf.fullPath);
                    AddSubItem(item, "size", gf.size.ToString());
                    AddSubItem(item, "index", index.ToString());
                    items.Add(item);
                    var tmp = gf;
                    tmp.index = index;
                    index++;
                    tmpL.Add(tmp);
                }
                gfL = tmpL;
                if (items.Count > 0)
                {
                    ListViewOperate(liv, ListViewOP.ADDRANGE, items: items.ToArray());
                }
            }
            SetSummaryBoxText($"Summay: Found {gfL.Count:N0} duplicate file(s).", gfL.Count);
        }

    }
}