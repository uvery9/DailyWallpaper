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

namespace DailyWallpaper
{
    partial class GeminiForm
    {
        private enum LoadFileStep
        {
            NO_LOAD,
            STEP_1_ALLFILES,
            STEP_2_FILESTOSTRUCT,
            STEP_3_FASTCOMPARE,
            STEP_4_COMPAREHASH,
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
                if (op == LoadFileStep.STEP_1_ALLFILES)
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
                    op = LoadFileStep.STEP_1_ALLFILES;
                }
                else if (pathLow.Contains("step-2-".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileStruct>>(path);
                    op = LoadFileStep.STEP_2_FILESTOSTRUCT;
                }
                else if (pathLow.Contains("step-3-".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileStruct>>(path);
                    op = LoadFileStep.STEP_3_FASTCOMPARE;
                }
                else if (pathLow.Contains("step-4-".ToLower()))
                {
                    ret = ReadFromXmlFile<List<GeminiFileStruct>>(path);
                    op = LoadFileStep.STEP_4_COMPAREHASH;
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
                    
                    if(!IsSkip(op, LoadFileStep.STEP_1_ALLFILES))
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
                        _console.WriteLine($">>> Skip {LoadFileStep.STEP_1_ALLFILES}... ");
                        filesList1 = sL;
                        filesList2 = new List<string>();
                    }
                    if (op <= LoadFileStep.STEP_1_ALLFILES)
                        SaveOperationHistory("step-1-allfiles_1.xml", filesList1);

                    SetProgressBarVisible(geminiProgressBar, true);
                    if (!IsSkip(op, LoadFileStep.STEP_2_FILESTOSTRUCT))
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
                        _console.WriteLine($">>> Skip {LoadFileStep.STEP_2_FILESTOSTRUCT}... ");
                    }
                    if (op <= LoadFileStep.STEP_2_FILESTOSTRUCT)
                        SaveOperationHistory("step-2-filesToStruct_1.xml", geminiFileStructList1);
                    
                    List<GeminiFileStruct> sameListNoDup;
                    var mode = SetCompareMode();
                    if (!IsSkip(op, LoadFileStep.STEP_3_FASTCOMPARE))
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
                        _console.WriteLine($">>> Skip {LoadFileStep.STEP_3_FASTCOMPARE}... ");
                    }
                    if (op <= LoadFileStep.STEP_3_FASTCOMPARE)
                        SaveOperationHistory("step-3-FastCompare.xml", sameListNoDup);
                    
                    if (fileMD5CheckBox.Checked || fileSHA1CheckBox.Checked)
                    {
                        if (!IsSkip(op, LoadFileStep.STEP_4_COMPAREHASH))
                        {
                            _console.WriteLine($">>> Update HASH for {sameListNoDup.Count:N0} file(s)...");
                            sameListNoDup =
                            UpdateHashInGeminiFileStructList(sameListNoDup).Result;
                            _console.WriteLine(">>> Update HASH finished.");
                        }
                        else
                        {
                            sameListNoDup = gfL;
                            _console.WriteLine($">>> Skip {LoadFileStep.STEP_4_COMPAREHASH}... ");

                        }
                        if (op <= LoadFileStep.STEP_4_COMPAREHASH)
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
            UPDATE_INDEX_AFTER_SORTED
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
                }
            }

        }


        private delegate void ListViewOperateDelegate(System.Windows.Forms.ListView liv, ListViewOP op,
            System.Windows.Forms.ListViewItem item = null, bool ischeck = false, 
            System.Windows.Forms.ListViewItem[] items = null);
        private void ListViewOperate(System.Windows.Forms.ListView liv, ListViewOP op,
            System.Windows.Forms.ListViewItem item = null, bool ischeck = false, 
            System.Windows.Forms.ListViewItem[] items = null 
           )
        {
            if (liv.InvokeRequired)
            {
                var addDele = new ListViewOperateDelegate(ListViewOperate);
                liv.Invoke(addDele, new object[] { liv, op, item, ischeck, items });
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
            }
        }
    }
}