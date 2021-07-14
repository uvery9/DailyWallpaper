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
                if (op == LoadFileStep.STEP_1_ALL_FILES)
                    StartAnalyzeStep(op, sL: (List<string>)listFromFile);
                else
                    StartAnalyzeStep(op, gfL: (List<GeminiFileStruct>)listFromFile);
            }
            catch (Exception ex)
            {
                CWriteLine($"xml -> Struct failed: {ex.Message}");
            }
        }
        private Tuple<LoadFileStep, object> LoadListFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return Tuple.Create(LoadFileStep.ERROR, (object)null);
            }
            /*
             *  step-1-allfiles_1.xml
             *  step-1-allfiles_2.xml
             *  step-2-filesToStruct_1.xml
             *  step-2-filesToStruct_2.xml
             *  step-3-FastCompare.xml
             *  step-4-CompareHash.xml
             *  step-5-RegrpAndColor.xml // color can't be write into xml, delete it.
             */
            object ret;
            LoadFileStep op;
            var pathLow = path.ToLower();
            try
            {
                    if (pathLow.Contains("step-1-".ToLower()))
                {
                    ret = ReadFromXmlFile<List<string>>(path);
                    op = LoadFileStep.STEP_1_ALL_FILES;
                }
                else if (pathLow.Contains("step-2-".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileStruct>>(path);
                    op = LoadFileStep.STEP_2_FILES_TO_STRUCT;
                }
                else if (pathLow.Contains("step-3-".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileStruct>>(path);
                    op = LoadFileStep.STEP_3_FAST_COMPARE;
                }
                else if (pathLow.Contains("step-4-".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileStruct>>(path);
                    op = LoadFileStep.STEP_4_COMPARE_HASH;
                }
                else
                {
                    ret =
                    ReadFromXmlFile<List<GeminiFileStruct>>(path);
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
            List<string> sL = null, List<GeminiFileStruct> gfL = null)
        {
            _source = new CancellationTokenSource();
            var token = _source.Token;
            EnableButton(btnStop, true);
            EnableButton(btnAnalyze, false);
            SetProgressBarVisible(geminiProgressBar, true);
            var limit = SetMinimumFileLimit();
            try
            {
                var _task = Task.Run(() =>
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    // Get all files from folder1/2
                    
                    if(!IsSkip(op, LoadFileStep.STEP_1_ALL_FILES))
                    {
                        bool fld1 = false;
                        bool fld2 = false;
                        var t1 = targetFolder1TextBox.Text;
                        var t2 = targetFolder2TextBox.Text;
                        CWriteLine($">>> Start Analyze Operation...");
                        CWriteLine($">>> Because it is a recursive search, \r\n" +
                            "  Program don't know the progress, please wait patiently...");
                        SetText(summaryTextBox, "Please wait patiently...", themeColor);
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
                        SaveOperationHistory("step-1_allfiles_2.xml", filesList2);
                    }
                    else
                    {
                        CWriteLine($">>> Start Analyze Operation...");
                        CWriteLine($">>> Load FileList from file..., \r\n");
                        CWriteLine($">>> Skip {LoadFileStep.STEP_1_ALL_FILES}... ");
                        filesList1 = sL;
                        filesList2 = new List<string>();
                    }
                    if (op <= LoadFileStep.STEP_1_ALL_FILES)
                        SaveOperationHistory("step-1-allfiles_1.xml", filesList1);

                    SetProgressBarVisible(geminiProgressBar, true);
                    if (!IsSkip(op, LoadFileStep.STEP_2_FILES_TO_STRUCT))
                    {
                        // get files info exclude HASH.(FASTER) 
                        FileList2GeminiFileStructList(filesList1, ref geminiFileStructList1, token);
                        FileList2GeminiFileStructList(filesList2, ref geminiFileStructList2, token);
                        SaveOperationHistory("step-2_filesToStruct_2.xml", geminiFileStructList2);
                    }
                    else
                    {
                        // get files info exclude HASH.(FASTER) 
                        geminiFileStructList1 = gfL;
                        geminiFileStructList2 = new List<GeminiFileStruct>();
                        CWriteLine($">>> Skip {LoadFileStep.STEP_2_FILES_TO_STRUCT}... ");
                    }
                    if (op <= LoadFileStep.STEP_2_FILES_TO_STRUCT)
                        SaveOperationHistory("step-2-filesToStruct_1.xml", geminiFileStructList1);
                    
                    List<GeminiFileStruct> sameListNoDup;
                    var mode = SetCompareMode();
                    if (!IsSkip(op, LoadFileStep.STEP_3_FAST_COMPARE))
                    {
                        CWriteLine(">>> Start Fast Compare...");
                        // compare folders and themselves, return duplicated files list.
                        sameListNoDup = ComparerTwoFolderGetList(geminiFileStructList1,
                                geminiFileStructList2, mode, limit, token, geminiProgressBar).Result;
                        CWriteLine(">>> Fast Compare finished...");
                    }
                    else
                    {
                        sameListNoDup = gfL;
                        CWriteLine($">>> Skip {LoadFileStep.STEP_3_FAST_COMPARE}... ");
                    }
                    if (op <= LoadFileStep.STEP_3_FAST_COMPARE)
                        SaveOperationHistory("step-3-FastCompare.xml", sameListNoDup);
                    
                    if (fileMD5CheckBox.Checked || fileSHA1CheckBox.Checked)
                    {
                        if (!IsSkip(op, LoadFileStep.STEP_4_COMPARE_HASH))
                        {
                            CWriteLine($">>> Update HASH for {sameListNoDup.Count:N0} file(s)...");
                            sameListNoDup =
                            UpdateHashInGeminiFileStructList(sameListNoDup, 
                                alwaysCalculateHashToolStripMenuItem.Checked).Result;
                            SaveOperationHistory("step-4-CompareHashForLittleFiles.xml", sameListNoDup);
                            int bigFileCnt = 0;
                            var sameListNoDupHash = new List<GeminiFileStruct>();
                            var sameListNoDupBigFiles = new List<GeminiFileStruct>();
                            foreach (var sl in sameListNoDup)
                            {
                                if (sl.bigFile)
                                {
                                    bigFileCnt++;
                                    sameListNoDupBigFiles.Add(sl);
                                }
                                else
                                {
                                    sameListNoDupHash.Add(sl);
                                }
                            }
                            if (bigFileCnt < 20)
                            {
                                CWriteLine($">>> Update HASH for remaining bigfile(s): " +
                                    $"{sameListNoDupBigFiles.Count:N0}...");
                                sameListNoDupBigFiles =
                                    UpdateHashInGeminiFileStructList(sameListNoDupBigFiles, true).Result;
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
                            SaveOperationHistory("step-4-CompareHash.xml", sameListNoDup);
                    }

                    // Color by Group.
                    CWriteLine(">>> ListView Color...");
                    geminiFileStructListForLV = ListReColorByGroup(sameListNoDup, mode, token);

                    CWriteLine(">>> Update to ListView...");
                    UpdateListView(resultListView, ref geminiFileStructListForLV, token);

                    timer.Stop();
                    CWriteLine($">>> Cost time: {GetTimeStringMsOrS(timer.Elapsed)}");

                }, _source.Token);

                _tasks.Add(_task);
                await _task;
                // No Error, filesList is usable
                scanRes = true;
                /*WriteToXmlFile("GeminiListLatest.xml",
                        geminiFileStructListForLV);*/
                SaveOperationHistory("GeminiListLatest.xml", geminiFileStructListForLV);
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
                CWriteLine(">>> Analyse is over.");
            }
            btnAnalyze.Enabled = true;

        }

        private void loadListViewFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    if (cleanEmptyFolderModeToolStripMenuItem.Checked)
                    {

                    }
                    else
                    {
                        LoadGeminiFileFileToListView(dialog.FileName);
                    }
                }
            }
        }


        private delegate void EnableButtonDelegate(Button b, bool enable);

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
            UPDATE_INDEX_AFTER_SORTED,
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

        private delegate void ListViewOperateLoopDelegate(System.Windows.Forms.ListView liv,
            ListViewOP op, List<GeminiFileStruct> gfl = null,
            Action<bool, List<GeminiFileStruct>, string> actionLoop = null, 
            CancellationToken token = default);

        private void ListViewOperateLoop(ListView liv,
            ListViewOP op, List<GeminiFileStruct> gfl = null,
            Action<bool, List<GeminiFileStruct>, string> actionLoop = null, CancellationToken token = default)
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
                            var tmpL = new List<GeminiFileStruct>();
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
                            var tmpL = new List<GeminiFileStruct>();
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
                else if (op == ListViewOP.UPDATE_INDEX_AFTER_SORTED)
                {
                    var tmpL = new List<GeminiFileStruct>();
                    var updateIndex = Task.Run(() =>
                    {
                        try
                        {
                            int index = 0;
                            foreach (ListViewItem lvi in liv.Items)
                            {
                                /*CWriteLine(liv.Items.IndexOf(lvi));*/
                                // how to update index in SubItems
                                lvi.SubItems["index"].Text = index.ToString();
                                lvi.SubItems[0].Text = index.ToString();
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
                                        tmp.index = index;
                                        tmpL.Add(tmp);
                                        break;
                                    }
                                }
                                index++;
                            }
                            actionLoop(true, tmpL, "Succeed in UPDATE_INDEX_AFTER_SORTED.");
                        }
                        catch (Exception ee)
                        {
                            Debug.WriteLine(ee);
                            actionLoop(false, null, ee.Message);
                        }
                    });
                    _tasks.Add(updateIndex);
                }
                // DOES NOT WORK.
                /*else if (op == ListViewOP.UPDATE_INDEX_AFTER_SORTED)
                {
                    var tmpL = new List<GeminiFileStruct>();
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


        public static void InvokeClearListViewItems(ListView listView)
        {
            if (listView.InvokeRequired)
            {
                listView.Invoke(new MethodInvoker(delegate () { InvokeClearListViewItems(listView); }));
            }
            else
            {
                listView.Items.Clear();
            }
        }

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
                protectFilesInGrpToolStripMenuItem.Enabled = false;
                autocleanEmptyFoldersToolStripMenuItem.Enabled = false;

                godsChoiceToolStripMenuItem.Enabled = false;

                geminiProgressBar.Visible = false;
                nameColumnHeader.Width = (int)(1.5 * nameColumnHeaderWidth);
                modifiedTimeColumnHeader.Width = (int)(1.5 * modifiedTimeColumnHeaderWidth);

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
                protectFilesInGrpToolStripMenuItem.Enabled = true;
                autocleanEmptyFoldersToolStripMenuItem.Enabled = true;

                godsChoiceToolStripMenuItem.Enabled = true;

                geminiProgressBar.Visible = true;

                nameColumnHeader.Width = nameColumnHeaderWidth;
                modifiedTimeColumnHeader.Width = modifiedTimeColumnHeaderWidth;
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

        private void UpdateCheckedInDelGFL(List<GeminiFileStruct> gfl, List<string> delList, GeminiFileStruct item)
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

        void FileList2GeminiFileStructList(List<string> filesList,
            ref List<GeminiFileStruct> gList, CancellationToken token)
        {
            gList = new List<GeminiFileStruct>();
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
                    gList.Add(Gemini.FillGeminiFileStruct(f));
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
            ListView liv, MultipleSelectOperations op)
        {
            liv.BeginUpdate();
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

            // update geminiFileStructListForLV
            void UpdateFunc(bool ret, List<GeminiFileStruct> gfl, ListView lv, string msg)
            {
                if(ret)
                {
                    geminiFileStructListForLV = gfl;
                    /*CWriteLine("SUCCEED.........");
                    CWriteLine(gfl[0].Checked);*/
                }
                else
                {
                    CWriteLine("ConvertGeminiFileStructListAndListView: " + msg);
                }
            }
            ConvertGeminiFileStructListAndListView(geminiFileStructListForLV, liv, false, action: UpdateFunc);


        }

        private void ConvertGeminiFileStructListAndListView(List<GeminiFileStruct> gfL,
            ListView lv, bool toListView = true, 
            Action<bool, List<GeminiFileStruct>, ListView, string> action = default)
        {
            if (toListView)
            {

            }
            else
            {
                Task.Run(() =>
                {
                    // Cause System.InvalidOperationException
                    try
                    {
                        var tmpL = new List<GeminiFileStruct>();
                        foreach (ListViewItem item in lv.Items)
                        {
                            var fullPath = item.SubItems["fullPath"].Text;
                            foreach (var gl in gfL)
                            {
                                var glt = gl;
                                if (fullPath.Equals(gl.fullPath))
                                {
                                    glt.Checked = item.Checked;
                                    tmpL.Add(glt);
                                }
                            }
                        }
                        action(true, tmpL, null, ">>> Succeed convert lv to gfL.");
                    }
                    catch (Exception ee)
                    {
                        action(false, null, null, ee.Message);
                        Debug.WriteLine(ee);
                    }
                    
                });
            }
        }

        // not good enough
        private void MultipleSelectOperationsAction(
            ListView liv, MultipleSelectOperations op)
        {
            if (liv.Items.Count < 1)
            {
                return;
            }
            geminiFileStructListForLVUndo = geminiFileStructListForLV;
            undoToolStripMenuItem.Enabled = true;
            var mpTask = Task.Run(() => {
                try
                {
                    if (op == MultipleSelectOperations.REVERSE_ELECTION)
                    {
                        geminiFileStructListForLV = UpdateGFLChecked(geminiFileStructListForLV)
                            ?? geminiFileStructListForLV;
                    }
                    var tmpGfl = new List<GeminiFileStruct>();
                    foreach (var item in geminiFileStructListForLV)
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
                    geminiFileStructListForLV = tmpGfl;
                    RestoreListViewChoiceInvoke(liv, geminiFileStructListForLV, _source.Token);
                }
                catch (Exception ex)
                {
                    CWriteLine($"{ex}");
                    CWriteLine($"{ex.Message}");
                }
            });
            _tasks.Add(mpTask);
        }

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

        private void GeminiFileStructListRE(List<GeminiFileStruct> gfL,
            GeminiFileStruct item, Regex rege, bool find = true)
        {
            item.Checked = !find;
            if (rege.IsMatch(item.fullPath))
            {
                item.Checked = find;
            }
            gfL.Add(item);
        }

        private void GeminiFileStructListGeneral(List<GeminiFileStruct> gfL,
            GeminiFileStruct item, List<string> filter, bool find = true)
        {
            item.Checked = !find;
            foreach (var it in filter)
            {
                if (item.fullPath.ToLower().Contains(it.ToLower()))
                {
                    item.Checked = find;
                    break;
                }
            }
            gfL.Add(item);
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

        private delegate void RestoreListViewChoiceInvokeDele(ListView liv, List<GeminiFileStruct> gfl, 
            CancellationToken token, bool indexChange = false, Action<bool, string> action = default);
        
        private void RestoreListViewChoiceInvoke(ListView liv, List<GeminiFileStruct> gfl, 
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
            ref List<GeminiFileStruct> gfL, CancellationToken token)
        {

        }

        private void UpdateListView(ListView liv,
            ref List<GeminiFileStruct> gfL, CancellationToken token)
        {
            ListViewOperate(liv, ListViewOP.CLEAR);
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            var tmpL = new List<GeminiFileStruct>();
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
                SetText(summaryTextBox, $"Summay: Found {gfL.Count:N0} duplicate files.", themeColor);
            }
            else
            {
                SetText(summaryTextBox, $"Summay: Found No duplicate files.", Color.ForestGreen);
            }
        }

    }
}