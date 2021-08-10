using DailyWallpaper.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

using System.Reflection;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Threading;



namespace DailyWallpaper.View
{
    public partial class OptionsView : Form
    {
        private TimerHelper _timerHelper;
        private string textFromHoursTextBox;
        private ConfigIni _ini;
        private bool consRunning = false;
        private bool useTextBoxWriter = false;
        private bool setWallpaperSucceed = false;
        private ConsWindow _consWindow;
        private NotifyIcon _notifyIcon;
        private bool iStextFromFileNew = true;
        private HashCalc.HashCalcForm _hashWin;
        private Tools.ShutdownTimer.Shutdown _shutdownTimer = null;

        public OptionsView()
        {
            InitializeComponent();
            _ini = ConfigIni.GetInstance();
            UpdateTranslation();

            WindowState = FormWindowState.Minimized;
            ShowInTaskbar = false;
            FormClosing += OptionsForm_FormClosing;
            Resize += OptionsForm_Resize;
            Icon = Properties.Resources.icon32x32;

            _notifyIcon.Icon = Properties.Resources.icon32x32;
            _notifyIcon.Text = 
                string.Format(TranslationHelper.Get("Icon_ToolTip"), ProjectInfo.GetVerSion());

            _notifyIcon.Visible = true;
            // _notifyIcon.BalloonTipClicked += notifyIcon_BalloonTipClicked;
            _notifyIcon.MouseUp += notifyIcon_MouseUp;
            
            SeveralFormInit();
            InitializeCheckedAndTimer();
            TryToUseGithubInCN();
        }

        private void SeveralFormInit()
        {
            _consWindow = ConsWindow.GetInstance(Properties.Resources.icon32x32);
            _consWindow.FormClosing += _viewWindow_FormClosing;
            // _consWindow.Load += new System.EventHandler(_viewWindow_Load);
            _consWindow.clearButton.Click += new EventHandler(clearButton_Click);
            _consWindow.saveToFileButton.Click += new EventHandler(saveToFileButton_Click);

            _hashWin = new HashCalc.HashCalcForm();
            _hashWin.selfFromClosing = false;
            _hashWin.FormClosing += _hashWindow_FormClosing;
            _shutdownTimer = new Tools.ShutdownTimer.Shutdown();
            _shutdownTimer.FormClosing += _shutdownTimer_FormClosing;
        }
        private void LaterCheckUpdate()
        {
            var _updateTimer = new System.Timers.Timer
            {
                Interval = 1000 * 60 * 5, // 5mins LATER,
                AutoReset = true,
                Enabled = true
            };
            // _timer.
            _updateTimer.Elapsed += _updateTimer_Elapsed;
            _updateTimer.Start();
        }
        public void _updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CheckUpdate(click: false);
            ((System.Timers.Timer)sender).Enabled = false;
        }

        void UpdateTranslation()
        {
            Icon_dailyWallpaperTitleToolStripMenuItem.Text = ProjectInfo.exeName
                + " v" + ProjectInfo.GetVerSion() + " by " + ProjectInfo.author;
            Icon_HashCalc.Text = TranslationHelper.Get("Icon_HashCalc");
            Icon_IssueAndFeedback.Text = TranslationHelper.Get("Icon_IssueAndFeedback");

            Icon_ChangeWallpaper.Text = TranslationHelper.Get("Icon_ChangeWallpaper");
            Icon_ChangeWallpaper.ToolTipText = TranslationHelper.Get("Icon_ChangeWallpaperTit");
            
            var defFont = Icon_HashCalc.Font; // default font so little.
            Icon_ChangeWallpaper.Font = new Font(defFont.Name, defFont.Size + 1, FontStyle.Bold);
            Icon_ChangeWallpaper.ShowShortcutKeys = true;
            Icon_ChangeWallpaper.ShortcutKeyDisplayString =
                TranslationHelper.Get("TrayIcon_ShortcutKeys");

            Icon_ChangeWallpaper.TextAlign = ContentAlignment.MiddleCenter;
            Icon_ChangeWallpaper.AutoSize = true;

            Icon_AutoChangeWallpaper.Text = "    "
                + TranslationHelper.Get("Icon_AutoChangeWallpaper");
            Icon_AutoChangeWallpaper.TextAlign = ContentAlignment.MiddleRight;
            //_Icon_EveryHoursAutoChangeMenuItem.DisplayStyle

            // _Icon_EveryHoursAutoChangeMenuItem

            // ooo
            Icon_AutoChangeWallpaper.DropDownItems.AddRange(new ToolStripItem[] {
            CustomHoursTextboxWithButtonAndUnit() 
            });

            Icon_Bing.Text =
                    TranslationHelper.Get("Icon_Bing");
            /*Icon_Bing.ToolTipText =
                    string.Format(TranslationHelper.Get("Icon_FeatureTit"), 
                    TranslationHelper.Get("Icon_Bing"));*/

            Icon_AlwaysDownLoadBingPicture.Text =
                    "    " + TranslationHelper.Get("Icon_AlwaysDownLoadBingPicture");
            Icon_AlwaysDownLoadBingPicture.Click +=
                    Icon_AlwaysDownLoadBingPicture_Click;

            Icon_BingNotAddWaterMark.Text =
                    "    " + TranslationHelper.Get("Icon_BingNotAddWaterMark");
            Icon_BingNotAddWaterMark.Click += Icon_BingNotAddWaterMark_Click;

            Icon_SkipToday.Text =
                    "    " + TranslationHelper.Get("Icon_SkipToday");
            Icon_SkipToday.Click += Icon_SkipToday_Click;

            Icon_LocalPath.Text = TranslationHelper.Get("Icon_LocalPath");
            /*Icon_LocalPath.ToolTipText =
                    string.Format(TranslationHelper.Get("Icon_FeatureTit"),
                    TranslationHelper.Get("Icon_LocalPath"));*/

            Icon_LocalPathSetting.Text = TranslationHelper.Get("Icon_LocalPathSetting");
            Icon_LocalPathSetting.Click += _Icon_LocalPathSettingMenuItem_Click;
            Icon_LocalPathSetting.ToolTipText = _ini.Read("localPathSetting", "Local") ?? "NULL";

            Icon_Spotlight.Text =
                    TranslationHelper.Get("Icon_Spotlight");
            /*Icon_Spotlight.ToolTipText =
                    string.Format(TranslationHelper.Get("Icon_FeatureTit"),
                    TranslationHelper.Get("Icon_Spotlight"));*/

            Icon_DisableShortcutKeys.Text =
                    TranslationHelper.Get("Icon_DisableShortcutKeys");


            // TODO
            Icon_Options.Text = TranslationHelper.Get("Icon_Options");

            Icon_DonateAndSupport.Text = TranslationHelper.Get("Icon_DonateAndSupport");
            Icon_DonateAndSupport.Click +=
                    (e, s) => {
                        Process.Start(ProjectInfo.DonationUrl);
                        ShowNotification("", TranslationHelper.Get("Notify_ThanksForDonation"));
                    };

            // open notepad++ / notepad
            Icon_Notepad.Text =
                    TranslationHelper.Get("Icon_Notepad");
            Icon_Notepad.ShortcutKeyDisplayString = "(Middle)";
                  
            Icon_Notepad.Click +=
                    Icon_Notepad_Click;

            Icon_Toolbox.Text =
                    TranslationHelper.Get("Icon_Toolbox");

            // Help and DropDownItems


            Icon_Help.Text = TranslationHelper.Get("Icon_Help");

            Icon_OpenOfficialWebsite.Text = TranslationHelper.Get("Icon_OpenOfficialWebsite");
            Icon_OpenOfficialWebsite.ToolTipText = ProjectInfo.OfficalWebSite;
            Icon_OpenOfficialWebsite.Click += ((e, s) =>
            {
                Process.Start(ProjectInfo.OfficalWebSite);
            });

            Icon_CheckUpdate.Text = TranslationHelper.Get("Icon_CheckUpdate");

            Icon_OpenConsole.Text = TranslationHelper.Get("Icon_ShowLog");
            Icon_About.Text = TranslationHelper.Get("Icon_About");
            Icon_RunAtStartup.Text = TranslationHelper.Get("Icon_RunAtStartup");
            Icon_Quit.Text = TranslationHelper.Get("Icon_Quit");
        }
        public void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(() => { DailyWallpaperConsSetWallpaper(silent: true);
                _ini.UpdateIniItem("TimerSetWallpaper", "true", "LOG");
            });
        }

        private void DailyWallpaperConsSetWallpaper(bool silent = false)
        {

            if (consRunning)
            {
                return;
            }
            consRunning = true;
            // _notifyIcon.
            try
            {
                if (IsNoneSelected())
                {
                    if (!silent)
                        ShowNotification("", TranslationHelper.Get("Notify_AtLeastSelectOneFeature"), 
                            isError: true);
                    return;
                }
                _notifyIcon.Icon = Properties.Resources.icon32x32_timer;
                bool res;
                if (useTextBoxWriter)
                {
                    iStextFromFileNew = false;
                    res = DailyWallpaperCons.GetInstance().ShowDialog(true, _consWindow.textWriter);
                }
                else
                {
                    iStextFromFileNew = true;
                    res = DailyWallpaperCons.GetInstance().ShowDialog();
                }

                System.Threading.Thread.Sleep(500);
                if (!res)
                {
                    setWallpaperSucceed = false;
                    if (!silent) 
                        ShowNotification("",
                            string.Format(TranslationHelper.Get("Notify_SetWallpaper_Failed"), Environment.NewLine));

                }
                else
                {
                    setWallpaperSucceed = true;
                    if (!silent)
                        ShowNotification("",
                            string.Format(TranslationHelper.Get("Notify_SetWallpaper_Succeed"),
                            Environment.NewLine + $"{_ini.Read("WALLPAPER", "LOG")}") );
                }
            }
            catch
            {

            }
            finally
            {
                consRunning = false;
                ChangeIconStatus();
            }
        }
        /// <summary>
        /// timeout unit: milliseconds
        /// </summary>
        /// <param name="title"></param>
        /// <param name="content">
        ///             The content you want to show
        /// </param>
        /// <param name="isError"></param>
        /// <param name="timeout"></param>
        /// <param name="clickEvent"></param>
        /// <param name="closeEvent"></param>
        /// <example> ShowNotification("","Show me something."); </example>
        public void ShowNotification(string title, string content, bool isError = false, int timeout = 5000,
            Action clickEvent = null, Action closeEvent = null)
        {
            _notifyIcon.ShowBalloonTip(timeout, title, content, isError ? ToolTipIcon.Error : ToolTipIcon.Info);
            _notifyIcon.BalloonTipClicked += OnIconOnBalloonTipClicked;
            _notifyIcon.BalloonTipClosed += OnIconOnBalloonTipClosed;

            void OnIconOnBalloonTipClicked(object sender, EventArgs e)
            {
                clickEvent?.Invoke();
                _notifyIcon.BalloonTipClicked -= OnIconOnBalloonTipClicked;
            }

            void OnIconOnBalloonTipClosed(object sender, EventArgs e)
            {
                closeEvent?.Invoke();
                _notifyIcon.BalloonTipClosed -= OnIconOnBalloonTipClosed;
            }
        }
        private bool IsNoneSelected()
        {
            if (Icon_Bing.Checked == false &&
                Icon_LocalPath.Checked == false &&
                Icon_Spotlight.Checked == false
                )
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// try to use github in CN, update CN/Global URL background.
        /// </summary>
        void TryToUseGithubInCN()
        {
            void updateUrlTips(bool success, string msg)
            {
                // MessageBox.Show(msg);
                if (success)
                {
                    Icon_OpenOfficialWebsite.ToolTipText = ProjectInfo.OfficalWebSite;
                }
                else
                {
                    // ShowNotification("Use backup gitee", msg);
                }
            };
            // ProjectInfo.TestConnect(updateUrlTips, "https://www.google.com");
            ProjectInfo.TestConnect(updateUrlTips, "https://www.github.com");
        }

        private void hoursTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) // && (e.KeyChar != '.')
            {
                e.Handled = true;
            }
            // only allow one decimal point
            /*if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }*/
        }

        // Press Enter Key when focus on hoursTextBox      
        private void hoursTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //Enter key is down
                //Capture the text
                if (sender is TextBox box)
                {
                    if (int.TryParse(box.Text, out int result))
                    {
                        textFromHoursTextBox = box.Text;
                        _ini.UpdateIniItem("Timer", textFromHoursTextBox);
                        _timerHelper.SetTimer(result * 60, SetTimerAfter);
                    }
                }
                _notifyIcon.ContextMenuStrip.Close();
            }
        }
        private void h12RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h12RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 12.ToString());
                hoursTextBox.Enabled = false;
                _notifyIcon.ContextMenuStrip.Close();
                _timerHelper.SetTimer(12 * 60, SetTimerAfter);
            }
        }
        private void h24RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h24RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 24.ToString());
                hoursTextBox.Enabled = false;
                _notifyIcon.ContextMenuStrip.Close();
                _timerHelper.SetTimer(24 * 60, SetTimerAfter);
            }
        }
        private void h6RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h6RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 6.ToString());
                hoursTextBox.Enabled = false;
                _notifyIcon.ContextMenuStrip.Close();
                _timerHelper.SetTimer(6 * 60, SetTimerAfter);
            }
        }
        private void customRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (customRadioButton.Checked)
            {
                hoursTextBox.Enabled = true;
                if (!_ini.Read("Timer").Equals(textFromHoursTextBox))
                {
                    _ini.UpdateIniItem("Timer", textFromHoursTextBox.ToString());
                }
                int.TryParse(textFromHoursTextBox, out int res);
                _timerHelper.SetTimer(res * 60, SetTimerAfter);
            }

        }

        private void ChangeIconStatus()
        {
            if (Icon_DisableShortcutKeys.Checked)
            {
                _notifyIcon.Icon = Properties.Resources.icon32x32_ban;
            }
            else if (Icon_RunAtStartup.Checked)
            {
                _notifyIcon.Icon = Properties.Resources.icon32x32_good;
            }
            else if (!Icon_RunAtStartup.Checked)
            {
                _notifyIcon.Icon = Properties.Resources.icon32x32_exclamation;
            }
            else
            {
                _notifyIcon.Icon = Properties.Resources.icon32x32;
            }
            if (consRunning)
            {
                _notifyIcon.Icon = Properties.Resources.icon32x32_timer;
            }
        }

        private void Icon_ChangeWallpaper_Click(object sender, EventArgs e)
        {
            Task.Run(() => DailyWallpaperConsSetWallpaper());
        }
        
        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            var wallpaper = _ini.Read("WALLPAPER", "LOG");
            if (File.Exists(wallpaper) && setWallpaperSucceed)
            {
                // string p = @"C:\tmp\this path contains spaces, and,commas\target.txt";
                string args = string.Format("/e, /select, \"{0}\"", wallpaper);
                ProcessStartInfo info = new ProcessStartInfo
                {
                    FileName = "explorer",
                    Arguments = args
                };
                Process.Start(info);
            }
            /*if (!setWallpaperSucceed)
            {
                Process.Start(ProjectInfo.logFile);
            }*/
        }

        private void Icon_AlwaysDownLoadBingPicture_Click(object sender, EventArgs e)
        {
            var item = Icon_AlwaysDownLoadBingPicture;
            if (item.Checked)
            {
                item.Checked = false;
                _ini.UpdateIniItem("alwaysDLBingWallpaper", "no", "Online");
            }
            else
            {
                item.Checked = true;
                _ini.UpdateIniItem("alwaysDLBingWallpaper", "yes", "Online");
                _notifyIcon.ContextMenuStrip.Show();
            }
        }
        private void Icon_BingNotAddWaterMark_Click(object sender, EventArgs e)
        {
            var item = Icon_BingNotAddWaterMark;
            if (item.Checked)
            {
                item.Checked = false;
                _ini.UpdateIniItem("bingNotWMK", "no", "Online");
            }
            else
            {
                item.Checked = true;
                _ini.UpdateIniItem("bingNotWMK", "yes", "Online");
                _notifyIcon.ContextMenuStrip.Show();
            }
        }

        private void _shutdownTimer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _shutdownTimer.Hide();
            }
        }

        private void _hashWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _hashWin.Hide();
            }
        }

        private void _viewWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                if (!consRunning)
                {
                    useTextBoxWriter = false;
                }
                //_consWindow.WindowState = FormWindowState.Minimized;
                //_consWindow.ShowInTaskbar = false;
                _consWindow.Hide();
                // _consWindow.textBoxCons.Text = "";
            }
        }

        private void _viewWindow_Load(object sender, EventArgs e)
        {

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _consWindow.textBoxCons.Text = "";
        }

        private void saveToFileButton_Click(object sender, EventArgs e)
        {
            if (_consWindow.textBoxCons.Text.Length < 1)
            {
                return;
            }
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog.Filter = "Txt files (*.txt)|*.txt";
                // saveFileDialog.FilterIndex = 2;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = ProjectInfo.exeName + "_" +
                DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); //+ ".txt"
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        // Code to write the stream goes here.
                        byte[] byteArray = System.Text.Encoding.Default.GetBytes(_consWindow.textBoxCons.Text);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }
            }
        }

        private void Icon_OpenConsole_Click(object sender, EventArgs e)
        {
            // Process.Start(ProjectInfo.logFile);
            if (consRunning)
            {
                if (useTextBoxWriter)
                {
                    _consWindow.Show();
                    return;
                }
                else
                {
                    ShowNotification("",
                        "You cannot redirect stdout/stderr " +
                        "while the console program is running.", true);
                    return;
                }

            }
            useTextBoxWriter = true;
            _consWindow.Show();
            if (iStextFromFileNew)
            {
                if (File.Exists(ProjectInfo.logFile))
                {
                    var textBoxCons = File.ReadAllText(ProjectInfo.logFile);
                    _consWindow.textBoxCons.Text = textBoxCons;
                    // _consWindow.textBoxCons
                    _consWindow.textBoxCons.Select(_consWindow.textBoxCons.TextLength, 0);//光标定位到文本最后
                    _consWindow.textBoxCons.ScrollToCaret();
                }
            }
        }

        private void Icon_LocalPath_Click(object sender, EventArgs e)
        {
            if (Icon_LocalPath.Checked)
            {
                Icon_LocalPath.Checked = false;
                Icon_LocalPathSetting.Visible = false;
                _ini.UpdateIniItem("localPath", "no", "Local");
                _notifyIcon.ContextMenuStrip.Hide();
            }
            else
            {
                Icon_LocalPath.Checked = true;
                Icon_LocalPathSetting.Visible = true;
                _ini.UpdateIniItem("localPath", "yes", "Local");
                _notifyIcon.ContextMenuStrip.Show();
            }
        }
        private void _Icon_LocalPathSettingMenuItem_Click(object sender, EventArgs e)
        {

            /*OpenFileDialog folderBrowser = new OpenFileDialog();
            // Set validate names and check file exists to false otherwise windows will
            // not let you select "Folder Selection."
            folderBrowser.ValidateNames = false;
            folderBrowser.CheckFileExists = false;
            folderBrowser.CheckPathExists = true;
            // Always default to Folder Selection.
            folderBrowser.FileName = "Folder Selection.";
            if (folderBrowser.ShowDialog() == DialogResult.OK)
            {
                string folderPath = Path.GetDirectoryName(folderBrowser.FileName);
                MessageBox.Show($"folderPath: {folderPath}");
                // ...
            }*/
            /*  Note that you need to install the Microsoft.WindowsAPICodePack.Shell package 
                through NuGet before you can use this CommonOpenFileDialog
                
                VS->Tools->NuGet Package manager->Program Package Manager Terminal->
                Type: Install-Package Microsoft.WindowsAPICodePack-Shell    Enter 
                using Microsoft.WindowsAPICodePack.Dialogs;
            */

            using (var dialog = new CommonOpenFileDialog())
            {
                var localPathSetting = _ini.Read("localPathSetting", "Local");
                var deskTopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (!_ini.EqualsIgnoreCase("localPathSetting", "null", "Local") && Directory.Exists(localPathSetting))
                {
                    dialog.InitialDirectory = localPathSetting;
                }
                else
                {
                    dialog.InitialDirectory = deskTopPath;
                }
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.Multiselect = false;
                dialog.Title = TranslationHelper.Get("Icon_LocalPathSetting");

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    // MessageBox.Show("You selected: " + dialog.FileName);
                    _ini.UpdateIniItem("localPathSetting", dialog.FileName, "Local");
                    Icon_LocalPathSetting.ToolTipText = dialog.FileName;
                    ShowNotification("", $"{TranslationHelper.Get("Notify_LocalPathSetting")} {dialog.FileName}");
                }
            }
        }

        private void Icon_Spotlight_Click(object sender, EventArgs e)
        {
            var it = Icon_Spotlight;
            if (it.Checked)
            {
                it.Checked = false;
                _ini.UpdateIniItem("Spotlight", "no", "Online");
            }
            else
            {
                it.Checked = true;
                _notifyIcon.ContextMenuStrip.Show();
                _ini.UpdateIniItem("Spotlight", "yes", "Online");
            }
        }

        /// <summary>
        /// get item from sender, should be learnt.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            /*            if (e.Button == MouseButtons.Left)
                        {
                            MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                            mi.Invoke(_notifyIcon, null);
                        }*/
        }

        private void Icon_Notepad_Click(object sender, EventArgs e)
        {
            try
            {
                var notePadppPathIni = _ini.Read("NOTEPADPPPATH", "Local");
                if (!string.IsNullOrEmpty(notePadppPathIni) && File.Exists(notePadppPathIni))
                {
                    Process.Start(notePadppPathIni);
                    return;
                }
                string notePadppPath = null;
                void GetNotePadppPath(bool res, string pathOrMsg)
                {
                    if (res)
                    {
                        notePadppPath = pathOrMsg;
                    }
                    else
                    {
                        // NO FOUND.
                    }
                }
                Task.Run(() =>
                {
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"I:\", GetNotePadppPath);
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"C:\Program Files\", GetNotePadppPath);
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"C:\Program Files (x86)\", GetNotePadppPath);
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"C:\", GetNotePadppPath);
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"D:\", GetNotePadppPath);
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"E:\", GetNotePadppPath);
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"F:\", GetNotePadppPath);
                    if (string.IsNullOrEmpty(notePadppPath)) ScanDirsFindNotepadPP(@"G:\", GetNotePadppPath);

                    if (!string.IsNullOrEmpty(notePadppPath))
                    {
                        /*var p = new Process();
                        p.StartInfo.FileName = notePadppPath;
                        p.StartInfo.UseShellExecute = false;
                        p.Start();*/
                        Process.Start(notePadppPath);
                        _ini.UpdateIniItem("NOTEPADPPPATH", notePadppPath, "Local");
                    }
                    else
                    {
                        Process.Start("notepad.exe");
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ScanDirsFindNotepadPP(string path, Action<bool, string> action)
        {
            try
            {
                var defPath = @"C:\Program Files\Notepad++\notepad++.exe";
                if (File.Exists(defPath))
                {
                    action(true, defPath);
                    return;
                }
                defPath = @"C:\Program Files(x86)\Notepad++\notepad++.exe";
                if (File.Exists(defPath))
                {
                    action(true, defPath);
                    return;
                }
                if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
                {
                    action(false, "Starting directory is a null or not exist.");
                    return;
                    // throw new ArgumentException("Starting directory is a null reference or an empty string: path");
                }
                foreach (var d in Directory.EnumerateDirectories(path, "*Notepad*", SearchOption.AllDirectories))
                {
                    if (d.Contains("Notepad++"))
                    {
                        var npexe = Path.Combine(d, "notepad++.exe");
                        if (File.Exists(npexe))
                        {
                            action(true, npexe);
                            // action(false, $">>>FOUND NOTDPAD++exe {npexe}");
                            return;
                        }
                    }
                    ScanDirsFindNotepadPP(d, action);
                }
            }
            catch (UnauthorizedAccessException) { }
            // action(false, $"NO FOUND AT {path}");
            return;
        }

        private void AddDivIntoPanel(Panel panel,
                                    RadioButton radioButton,
                                    int height,
                                    string unitText,
                                    int hours = 0,
                                    string buttonStr = null
                                    )
        {
            radioButton.AutoSize = true;
            radioButton.Location = new Point(3, height - 2);
            var unitLabel = new Label();
            unitLabel.AutoSize = true;
            var column = (int)radioButton.Font.Size;
            if (hours == 0)
            {
                radioButton.Name = "customRadioButton";
                radioButton.Text = !string.IsNullOrEmpty(buttonStr) ? buttonStr : "Custom";
                hoursTextBox.Name = "hoursTextBox";
                hoursTextBox.Width = 28;
                hoursTextBox.Location = new System.Drawing.Point(radioButton.Right + column, height);
                hoursTextBox.TextAlign = HorizontalAlignment.Center;
                hoursTextBox.KeyDown += hoursTextBox_KeyDown;
                hoursTextBox.KeyPress += hoursTextBox_KeyPress;
                //hoursTextBox.TextChanged += hoursTextBox_TextChanged;
                unitLabel.Name = "customUnitLabel";
                unitLabel.Location = new Point(hoursTextBox.Right + column, height);
                unitLabel.Text = "  " + unitText;
                panel.Controls.Add(hoursTextBox);

            }
            else
            {
                //radioButton.Width -= 50;
                radioButton.Name = "radioButton" + hours.ToString();
                unitLabel.Name = "unitLabel" + hours.ToString();
                unitLabel.Location = new Point(radioButton.Right + column, height);
                unitLabel.Text = hours.ToString() + "  " + unitText;
            }
            // MessageBox.Show(radioButton.Width.ToString()); default is 104
            panel.Controls.Add(radioButton);
            panel.Controls.Add(unitLabel);
        }
        private ToolStripControlHost CustomHoursTextboxWithButtonAndUnit()
        {
            var backColor = SystemColors.Window;
            hoursTextBox = new TextBox();

            h12RadioButton = new RadioButton();
            h24RadioButton = new RadioButton();
            h6RadioButton = new RadioButton();
            customRadioButton = new RadioButton();
            h6RadioButton.CheckedChanged += h6RadioButton_CheckedChanged;
            h12RadioButton.CheckedChanged += h12RadioButton_CheckedChanged;
            h24RadioButton.CheckedChanged += h24RadioButton_CheckedChanged;
            customRadioButton.CheckedChanged += customRadioButton_CheckedChanged;

            var panel = new Panel();
            panel.SuspendLayout(); // IS NOT DIFF ?
            var unit = TranslationHelper.Get("Icon_Unit");
            AddDivIntoPanel(panel, h6RadioButton, 5, unit, 6);
            AddDivIntoPanel(panel, h12RadioButton, 35, unit, 12);
            AddDivIntoPanel(panel, h24RadioButton, 65, unit, 24);
            AddDivIntoPanel(panel, customRadioButton, 95, unit, buttonStr: TranslationHelper.Get("Icon_Custom"));
            panel.Name = "combine";
            panel.AutoSize = true;
            // panel.Size = new System.Drawing.Size(241, 37);
            //MessageBox.Show(_Icon_Every24HoursMenuItem.); // 32, 19
            // panel.TabIndex = 7;
            var panelHost = new ToolStripControlHost(panel)
            {
                BackColor = backColor
            };

            return panelHost;
        }
        private void InitializeCheckedAndTimer()
        {
            _timerHelper = TimerHelper.GetInstance(233, timer_Elapsed);
            textFromHoursTextBox = "3";
            if (_ini.EqualsIgnoreCase("bing", "yes", "Online"))
            {
                Icon_Bing.Checked = true;
                Icon_AlwaysDownLoadBingPicture.Visible = true;
                Icon_BingNotAddWaterMark.Visible = true;
            }
            else
            {
                Icon_Bing.Checked = false;
                Icon_AlwaysDownLoadBingPicture.Visible = false;
                Icon_BingNotAddWaterMark.Visible = false;
            }

            if (_ini.EqualsIgnoreCase("alwaysDLBingWallpaper", "yes", "Online"))
            {
                Icon_AlwaysDownLoadBingPicture.Checked = true;
            }


            if (_ini.EqualsIgnoreCase("bingNotWMK", "yes", "Online"))
            {
                Icon_BingNotAddWaterMark.Checked = true;
            }
            else
            {
                Icon_BingNotAddWaterMark.Checked = false;
            }

            Icon_SkipToday.Checked = false;
            if (DateTime.TryParse(_ini.Read("bingSkipToday", "Online"), out DateTime ret))
            {
                if (ret.DayOfYear.Equals(DateTime.Today.DayOfYear))
                {
                    Icon_SkipToday.Checked = true;
                }
            }

            if (AutoStartupHelper.IsAutorun())
            {
                Icon_RunAtStartup.Checked = true;
                _notifyIcon.Icon = Properties.Resources.icon32x32_good;
            }
            else
            {
                Icon_RunAtStartup.Checked = false;
                _notifyIcon.Icon = Properties.Resources.icon32x32_exclamation;
            }


            if (_ini.EqualsIgnoreCase("Spotlight", "yes", "Online"))
            {
                Icon_Spotlight.Checked = true;
            } 
            else if(_ini.EqualsIgnoreCase("Spotlight", "no", "Online"))
            {
                Icon_Spotlight.Checked = false;
            }

            if (_ini.EqualsIgnoreCase("localPath", "yes", "Local"))
            {
                Icon_LocalPath.Checked = true;
                Icon_LocalPathSetting.Visible = true;
            }
            else
            {
                Icon_LocalPath.Checked = false;
                Icon_LocalPathSetting.Visible = false;
            }

            if (_ini.EqualsIgnoreCase("UseShortcutKeys", "yes"))
            {
                Icon_DisableShortcutKeys.Checked = false;
            }

            string timerStr = _ini.Read("Timer");
            hoursTextBox.Enabled = false;
            if (timerStr.Equals("12"))
            {
                h12RadioButton.Checked = true;
            }
            else if (timerStr.Equals("24"))
            {
                h24RadioButton.Checked = true;
            }
            else if (timerStr.Equals("6"))
            {
                h6RadioButton.Checked = true;
            }
            else
            {
                customRadioButton.Checked = true;
                hoursTextBox.Enabled = true;
                hoursTextBox.Text = timerStr;
                textFromHoursTextBox = timerStr;
            }

            if (int.TryParse(timerStr, out int res))
            {
                if (_ini.EqualsIgnoreCase("TimerSetWallpaper", "true", "LOG"))
                {
                    _timerHelper.SetTimer(res * 60, SetTimerAfter);
                }
                else
                {
                    if (!DateTime.TryParse(_ini.Read("appStartTime", "LOG"), out DateTime appStartTime))
                    {
                        // first time.
                        appStartTime = DateTime.Now;
                    }

                    if (!DateTime.TryParse(_ini.Read("appExitTime", "LOG"), out DateTime appExitTime))
                    {
                        // first time.
                        appExitTime = DateTime.Now;
                    }

                    var timeDiff = (int)(appExitTime - appStartTime).TotalMinutes;
                    _ini.UpdateIniItem("LastExitSubStartTimeDiff", $"{timeDiff}mins", "Log");
                    if (timeDiff > res * 60)
                    {
                        // something wrong. never mind.
                        _timerHelper.SetTimer(res * 60, SetTimerAfter);
                        _ini.UpdateIniItem("TimerNotWorkProperly ", $"{DateTime.Now.ToString()}");
                    }
                    else
                    {
                        // rest time = 60 * res - lastTimeNotFinishe.
                        _timerHelper.SetTimer(res * 60 - timeDiff, SetTimerAfter);
                    }

                }
            }
            else
            {
                _ini.UpdateIniItem("Timer", "24");
                _timerHelper.SetTimer(24 * 60, SetTimerAfter);
            }
            // 30 mins period.
            UpdateExitIniTimer();
            _ini.UpdateIniItem("appStartTime", DateTime.Now.ToString(), "LOG");
            LaterCheckUpdate();
            
            void LaterSetWallpaperWhenStart() 
            {
                Thread.Sleep(1000 * 60 * 1); // 1min later
                Task.Run(() => {
                    DailyWallpaperConsSetWallpaper(silent: true);
                    _ini.UpdateIniItem("TimerSetWallpaper", "true", "LOG");
                });
            }
            new Thread(LaterSetWallpaperWhenStart).Start();
        }

        private void UpdateExitIniTimer()
        {
            var _updateTimer = new System.Timers.Timer
            {
                Interval = 1000 * 60 * 30, // 30mins,
                AutoReset = true,
                Enabled = true
            };
            // _timer.
            _updateTimer.Elapsed += exitIniTimerElapsed;
            _updateTimer.Start();
        }
        public void exitIniTimerElapsed(object sender, ElapsedEventArgs e)
        {
            _ini.UpdateIniItem("appExitTime", DateTime.Now.ToString(), "LOG");
        }


        void SetTimerAfter(int mins)
        {
            var nextTime = DateTime.Now.AddMinutes(mins).ToString();
            _ini.UpdateIniItem("NEXTAutoChageWallpaperTime", nextTime, "LOG");
            Icon_NextTime.Text = "NextTime: " + nextTime;
        }
        
        private RadioButton h12RadioButton;
        private RadioButton h24RadioButton;
        private RadioButton h6RadioButton;
        private RadioButton customRadioButton;
        private TextBox hoursTextBox;
        

        private void panelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void Icon_Quit_Click(object sender, EventArgs e)
        {
            _ini.UpdateIniItem("appExitTime", DateTime.Now.ToString(), "LOG");
 /*           _cefWindow.Dispose();
            _consWindow.Dispose();
            _notifyIcon.Dispose();*/
            Application.Exit();
        }

        private void Icon_DonateAndSupport_Click(object sender, EventArgs e)
        {

        }

        private void Icon_OpenOfficialWebsite_Click(object sender, EventArgs e)
        {

        }

        private void CheckUpdate(bool click = true)
        {
            void getResult(bool res, bool downloaded, string msg)
            {
                // if downloaded is true, msg will become filepath.
                if (click && !downloaded)
                {
                    ShowNotification("", msg);
                }
                if (click && !res && !downloaded)
                {
                    Process.Start(ProjectInfo.OfficalLatest);
                }
                if (downloaded && res || (click && downloaded && !res)) // true true = new download, false true = already downloaded.
                {
                    ShowNotification("",
                                "Update Downloaded, click me to install.",
                                timeout: 20000,
                                clickEvent:
                                () => { 
                                    Process.Start(msg);
                                    Icon_Quit.PerformClick();
                                });
                }
            }
            if (click)
                ShowNotification("", "Checking update.");
            ProjectInfo.CheckForUpdates(getResult);
        }
        private void Icon_CheckUpdate_Click(object sender, EventArgs e)
        {
            CheckUpdate();
        }

        private void Icon_IssueAndFeedback_Click(object sender, EventArgs e)
        {
            Process.Start(ProjectInfo.NewIssue);
        }

        private void Icon_About_Click(object sender, EventArgs e)
        {
            new View.AboutForm().Show();
        }

        private void Icon_RunAtStartup_Click(object sender, EventArgs e)
        {
            // May be unsuccessful due to permissions
            var next_action = !AutoStartupHelper.IsAutorun();
            if (next_action)
            {
                _ini.UpdateIniItem("RunAtStartUp", "yes");
                // Force Update ShortCut: delete and create.
                AutoStartupHelper.CreateAutorunShortcut();
            }
            else
            {
                _ini.UpdateIniItem("RunAtStartUp", "no");
                AutoStartupHelper.RemoveAutorunShortcut();
            }
            // actually
            Icon_RunAtStartup.Checked = AutoStartupHelper.IsAutorun();
            if (Icon_RunAtStartup.Checked)
            {
                // Only succeed will ShowNotification.
                System.Threading.Thread.Sleep(300);
                ShowNotification("", string.Format(TranslationHelper.Get("Notify_RunAtStartup"),
                    Environment.NewLine));
            }
            ChangeIconStatus();
        }

        private void Icon_DisableShortcutKeys_Click(object sender, EventArgs e)
        {
            if (Icon_DisableShortcutKeys.Checked)
            {
                Icon_DisableShortcutKeys.Checked = false;
                Icon_ChangeWallpaper.ShowShortcutKeys = true;
                _ini.UpdateIniItem("UseShortcutKeys", "yes");
            }
            else
            {
                Icon_DisableShortcutKeys.Checked = true;
                Icon_ChangeWallpaper.ShowShortcutKeys = false;
                _ini.UpdateIniItem("UseShortcutKeys", "no");
            }
            ChangeIconStatus();
        }

        private void Icon_Options_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = FormWindowState.Normal;
            ShowInTaskbar = true;
            // notifyIcon.Visible = false;
        }
        private void OptionsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;
                // ShowInTaskbar = false;
                // Hide();
            }
        }

        private void OptionsForm_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                // mynotifyicon.Visible = true;
                // mynotifyicon.ShowBalloonTip(500);
                this.Hide();
            }
            else if (FormWindowState.Normal == this.WindowState)
            {
                // mynotifyicon.Visible = false;
            }
        }

        private void Icon_HashCalc_Click(object sender, EventArgs e)
        {
            // new ShowFormInThreadMode().ShowForm(ShowFormInThreadMode.ShowHashCalcForm); // only can use showdialog.
            // try to close the form, no error.
            // _hashWin.Show();
            new HashCalc.HashCalcForm().Show();
        }


        private void Icon_Bing_Click(object sender, EventArgs e)
        {
            if (Icon_Bing.Checked)
            {
                Icon_Bing.Checked = false;
                Icon_AlwaysDownLoadBingPicture.Visible = false;
                Icon_BingNotAddWaterMark.Visible = false;
                /*Icon_BingSetting.Visible = false;
                Icon_BingSetting.Enabled = false;*/
                _ini.UpdateIniItem("bing", "no", "Online");
                _notifyIcon.ContextMenuStrip.Hide();
            }
            else
            {
                Icon_Bing.Checked = true;
                _ini.UpdateIniItem("bing", "yes", "Online");
                Icon_AlwaysDownLoadBingPicture.Visible = true;
                Icon_BingNotAddWaterMark.Visible = true;
                /*Icon_BingSetting.Visible = true;
                Icon_BingSetting.Enabled = true;*/
                _notifyIcon.ContextMenuStrip.Show();
            }

        }

        private void Icon_AutoChangeWallpaper_Click(object sender, EventArgs e)
        {

        }

        // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/input-mouse/how-to-distinguish-between-clicks-and-double-clicks?view=netdesktop-5.0
        // There is a solution, but I don't want to use. Complicated my code.
        private void _notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                Icon_Notepad.PerformClick();
            }
        }

        private void _notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (_ini.EqualsIgnoreCase("UseShortcutKeys", "yes"))
                {
                    Task.Run(() => DailyWallpaperConsSetWallpaper());
                }
            }
        }

        private void geminiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new GeminiForm().Show();
        }

        private void videoEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Tools.VideoEditorForm().Show();
        }

        private void shutdownTimerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //new Tools.ShutdownTimer.Shutdown().Show();
            _shutdownTimer.Show();
    }

        private void dateCalculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new DateCnt().Show();
        }

        private void Icon_SkipToday_Click(object sender, EventArgs e)
        {
            var it = Icon_SkipToday;
            if (it.Checked)
            {
                it.Checked = false;
                _ini.UpdateIniItem("bingSkipToday", "NULL", "Online");
            }
            else
            {
                it.Checked = true;
                _ini.UpdateIniItem("bingSkipToday", DateTime.Today.ToString("yyyy-MM-dd"), "Online");
                Icon_ChangeWallpaper.PerformClick();
            }
        }
    }
}
