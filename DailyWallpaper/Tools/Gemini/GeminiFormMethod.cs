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
                    _console.WriteLine("!!! LoadFileStep.ERROR");
                    return;
                }               
                if (op == LoadFileStep.STEP_1_ALL_FILES)
                    StartAnalyzeStep(op, sL: (List<string>)listFromFile);
                else
                    StartAnalyzeStep(op, gfL: (List<GeminiFileStruct>)listFromFile);

                /*_console.WriteLine(">>> Loading xml file...");
                geminiFileStructListForLV =
                        ReadFromXmlFile<List<GeminiFileStruct>>(path);
                var _updateFromFileTask = Task.Run(() =>
                {
                    var token = _source.Token;
                    _console.WriteLine(">>> Recolor...");
                    geminiFileStructListForLV = ListReColorByGroup(geminiFileStructListForLV,
                        SetCompareMode(), token);
                    _console.WriteLine(">>> Load file, update to ListView...");
                    UpdateListView(resultListView, ref geminiFileStructListForLV, token);
                    RestoreListViewChoiceInvoke(resultListView, geminiFileStructListForLV, _source.Token);
                    _console.WriteLine(">>> Load xml file finished!");
                }, _source.Token);
                _tasks.Add(_updateFromFileTask);*/
            }
            catch (Exception ex)
            {
                _console.WriteLine($"xml -> Struct failed: {ex.Message}");
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
                _console.WriteLine($"Can't read xml file to List<T>: {ex.Message}");
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
                        _console.WriteLine($">>> Start Analyze Operation...");
                        _console.WriteLine($">>> Because it is a recursive search, \r\n" +
                            "  Program don't know the progress, please wait patiently...");
                        SetText(summaryTextBox, "Please wait patiently...", themeColor);
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
                        SaveOperationHistory("step-1_allfiles_2.xml", filesList2);
                    }
                    else
                    {
                        _console.WriteLine($">>> Start Analyze Operation...");
                        _console.WriteLine($">>> Load FileList from file..., \r\n");
                        _console.WriteLine($">>> Skip {LoadFileStep.STEP_1_ALL_FILES}... ");
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
                        _console.WriteLine($">>> Skip {LoadFileStep.STEP_2_FILES_TO_STRUCT}... ");
                    }
                    if (op <= LoadFileStep.STEP_2_FILES_TO_STRUCT)
                        SaveOperationHistory("step-2-filesToStruct_1.xml", geminiFileStructList1);
                    
                    List<GeminiFileStruct> sameListNoDup;
                    var mode = SetCompareMode();
                    if (!IsSkip(op, LoadFileStep.STEP_3_FAST_COMPARE))
                    {
                        _console.WriteLine(">>> Start Fast Compare...");
                        // compare folders and themselves, return duplicated files list.
                        sameListNoDup = ComparerTwoFolderGetList(geminiFileStructList1,
                                geminiFileStructList2, mode, limit, token, geminiProgressBar).Result;
                        _console.WriteLine(">>> Fast Compare finished...");
                    }
                    else
                    {
                        sameListNoDup = gfL;
                        _console.WriteLine($">>> Skip {LoadFileStep.STEP_3_FAST_COMPARE}... ");
                    }
                    if (op <= LoadFileStep.STEP_3_FAST_COMPARE)
                        SaveOperationHistory("step-3-FastCompare.xml", sameListNoDup);
                    
                    if (fileMD5CheckBox.Checked || fileSHA1CheckBox.Checked)
                    {
                        if (!IsSkip(op, LoadFileStep.STEP_4_COMPARE_HASH))
                        {
                            _console.WriteLine($">>> Update HASH for {sameListNoDup.Count:N0} file(s)...");
                            sameListNoDup =
                            UpdateHashInGeminiFileStructList(sameListNoDup).Result;
                            _console.WriteLine(">>> Update HASH finished.");
                        }
                        else
                        {
                            sameListNoDup = gfL;
                            _console.WriteLine($">>> Skip {LoadFileStep.STEP_4_COMPARE_HASH}... ");

                        }
                        if (op <= LoadFileStep.STEP_4_COMPARE_HASH)
                            SaveOperationHistory("step-4-CompareHash.xml", sameListNoDup);
                    }

                    // Color by Group.
                    _console.WriteLine(">>> ListView Color...");
                    geminiFileStructListForLV = ListReColorByGroup(sameListNoDup, mode, token);

                    _console.WriteLine(">>> Update to ListView...");
                    UpdateListView(resultListView, ref geminiFileStructListForLV, token);

                    timer.Stop();
                    _console.WriteLine($">>> Cost time: {GetTimeStringMsOrS(timer.Elapsed)}");

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
                _console.WriteLine($"\r\n>>> OperationCanceledException: {e.Message}");
            }
            catch (AggregateException e)
            {
                _console.WriteLine($"\r\n>>> AggregateException[Cancel exception]: {e.Message}");
            }
            catch (Exception e)
            {
                scanRes = false;
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


        private delegate void ListViewOperateLoopDelegate(System.Windows.Forms.ListView liv,
            ListViewOP op, List<GeminiFileStruct> gfl = null,
            Action<bool, List<GeminiFileStruct>> actionLoop = null);

        private void ListViewOperateLoop(System.Windows.Forms.ListView liv,
            ListViewOP op, List<GeminiFileStruct> gfl = null,
            Action<bool, List<GeminiFileStruct>> actionLoop = null)
        {
            if (liv.InvokeRequired)
            {
                var addDele = new ListViewOperateLoopDelegate(ListViewOperateLoop);
                liv.Invoke(addDele, new object[] { liv, op, gfl, actionLoop });
            }
            else
            {
                if (op == ListViewOP.UPDATE_CHECK_INTHELOOP)
                {
                    var tmpL = new List<GeminiFileStruct>();
                    foreach (var it in liv.Items)
                    {
                        var lvi = ((System.Windows.Forms.ListViewItem)it);
                        var fullPathLV = lvi.SubItems["fullPath"].Text;
                        foreach (var gf in gfl)
                        {
                            var tmp = gf;
                            if (gf.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                            {
                                tmp.Checked = lvi.Checked;
                                tmpL.Add(tmp);
                                break;
                            }
                        }
                    }
                    actionLoop(true, tmpL);

                }
                if (op == ListViewOP.UPDATE_CHECK_BY_INDEX)
                {
                    var tmpL = new List<GeminiFileStruct>();
                    foreach (var gf in gfl)
                    {
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
                            actionLoop(false, tmpL);
                        }
                    }
                    actionLoop(true, tmpL);

                }
                if (op == ListViewOP.UPDATE_INDEX_AFTER_SORTED)
                {
                    // HUGE BUGS. INDEX FCK CHAOTIC..
                    var updateIndex = Task.Run(() =>
                    {
                        var tmpL = new List<GeminiFileStruct>();
                        int index = 0;
                        foreach (var it in liv.Items)
                        {
                            var lvi = ((System.Windows.Forms.ListViewItem)it);
                            /*_console.WriteLine(liv.Items.IndexOf(lvi));*/
                            // how to update index in SubItems
                            lvi.SubItems["index"].Text = index.ToString();
                            lvi.SubItems[0].Text = index.ToString();
                            var fullPathLV = lvi.SubItems["fullPath"].Text;
                            foreach (var gf in gfl)
                            {
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
                        actionLoop(true, tmpL);
                    });
                    _tasks.Add(updateIndex);
                }
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
                        _console.WriteLine($"delete ###  {fullPathLV}");
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
                    _console.WriteLine($"...... Delete empty folder:  {dir}");
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
                    _console.WriteLine($"... Found empty folder: {dir}");
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
                _console.WriteLine(">>> Start collecting all files...");
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
                _console.WriteLine($">>> You have selected {keyInIni} folder:\r\n  {path}");
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
            _console.WriteLine($"\r\n >>> FilterMode: {filterMode}");
            UpdateFilterExampleText(filterMode);
            gemini.ini.UpdateIniItem("FilterMode", filterMode.ToString(), "Gemini");
        }


        private delegate void ListViewOperateDelegate(System.Windows.Forms.ListView liv, ListViewOP op,
            System.Windows.Forms.ListViewItem item = null, bool ischeck = false, 
            System.Windows.Forms.ListViewItem[] items = null, Action<bool, string> action = null);
        private void ListViewOperate(System.Windows.Forms.ListView liv, ListViewOP op,
            System.Windows.Forms.ListViewItem item = null, bool ischeck = false, 
            System.Windows.Forms.ListViewItem[] items = null, Action <bool, string> action = null
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
                    /*Task.Run(() => { 
                    liv.BeginUpdate();
                    liv.Sort();
                    liv.EndUpdate();
                    action(true, "Finished");
                    });*/

                    /*liv.Sort();*/

                    liv.BeginUpdate();
                    liv.Sort();
                    liv.EndUpdate();
                }
            }
        }

        private enum MultipleSelectOperations
        {
            SELECT_ALL,
            UNSELECT_ALL,
            REVERSE_ELECTION
        };
        private void MultipleSelectOperationsAction(
            System.Windows.Forms.ListView liv, MultipleSelectOperations op)
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
                        else if (op == MultipleSelectOperations.SELECT_ALL)
                        {
                            tmp.Checked = true;
                        }
                        else if (op == MultipleSelectOperations.UNSELECT_ALL)
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
                    _console.WriteLine($"{ex}");
                    _console.WriteLine($"{ex.Message}");
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
                    else if (op == MultipleSelectOperations.SELECT_ALL)
                    {
                        tmp.Checked = true;
                    }
                    else if (op == MultipleSelectOperations.UNSELECT_ALL)
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

        private delegate void RestoreListViewChoiceInvokeDele(System.Windows.Forms.ListView liv,
            List<GeminiFileStruct> gfl, CancellationToken token, bool indexChange = false);
        private void RestoreListViewChoiceInvoke(System.Windows.Forms.ListView liv,
            List<GeminiFileStruct> gfl, CancellationToken token, bool indexChange = false)
        {
            if (liv.InvokeRequired)
            {
                var f = new RestoreListViewChoiceInvokeDele(RestoreListViewChoiceInvoke);
                liv.Invoke(f, new object[] { liv, gfl, token, indexChange });
            }
            else
            {
                try
                {
                    if (indexChange)
                    {
                        throw new Exception(">>> Clean-UP Restore Mode.");
                    }
                    // _console.WriteLine("FAST..........");
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
                    // _console.WriteLine("FAST No Exception..........");
                }
                catch (Exception ex)
                {
                    _console.WriteLine($"{ex.Message}");
                    if (liv.Items.Count < 1)
                    {
                        return;
                    }
                    foreach (var item in liv.Items)
                    {
                        var it = (System.Windows.Forms.ListViewItem)item;
                        var fullPathLV = it.SubItems["fullPath"].Text;
                        foreach (var gf in gfl)
                        {
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
                }
            }

        }

    }
}