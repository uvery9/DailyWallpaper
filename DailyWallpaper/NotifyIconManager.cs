using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using DailyWallpaper.Helpers;
using System.Drawing;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Diagnostics;
using System.Timers;
namespace DailyWallpaper
{
    public partial class NotifyIconManager : IDisposable
    {
        private NotifyIconManager() {
            _ini = ConfigIni.GetInstance();
            _components = new System.ComponentModel.Container();
            notifyIcon = new System.Windows.Forms.NotifyIcon(_components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.icon32x32,
                Text = string.Format(TranslationHelper.Get("Icon_ToolTip"),
                                Application.ProductVersion),
                Visible = true,
            };
            _timerHelper = TimerHelper.GetInstance(233, timer_Elapsed);
            textFromHoursTextBox = "72";
            TrayIconInitializeComponent();
            ActionRegister();
            InitializeCheckedAndTimer();
        }

        // call back by timer.
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (_ini.GetCfgFromIni()["UseShortcutKeys"].ToLower().Equals("yes"))
            {
                DailyWallpaperConsSetWallpaper();
            }
        }
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DailyWallpaperConsSetWallpaper();
            _ini.UpdateIniItem("TimerSetWallpaper", "true", "LOG");
            // _ini.UpdateIniItem("TimerSetWallpaper", "false", "LOG");
        }
        private void exitTimeHelperCallback(object state)
        {
            _ini.UpdateIniItem("appExitTime", DateTime.Now.ToString(), "LOG");
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
                if (sender is TextBox)
                {
                    int result;
                    if (int.TryParse(((TextBox)sender).Text, out result))
                    {
                        textFromHoursTextBox = ((TextBox)sender).Text;
                        _ini.UpdateIniItem("Timer", textFromHoursTextBox);
                    }
                }
                notifyIcon.ContextMenuStrip.Close();
            }
        }
        private void h12RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h12RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 12.ToString());
                hoursTextBox.Enabled = false;
                notifyIcon.ContextMenuStrip.Close();
                _timerHelper.SetTimer(12 * 60);
            }
        }
        private void h24RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h24RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 24.ToString());
                hoursTextBox.Enabled = false;
                notifyIcon.ContextMenuStrip.Close();
                _timerHelper.SetTimer(24 * 60);
            }
        }
        private void h48RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h48RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 48.ToString());
                hoursTextBox.Enabled = false;
                notifyIcon.ContextMenuStrip.Close();
                _timerHelper.SetTimer(48 * 60);
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
                _timerHelper.SetTimer(res * 60);
            }
           
        }

        private void ChangeIconStatus()
        {
            if (_Icon_DisableShortcutKeysMenuItem.Checked)
            {
                notifyIcon.Icon = Properties.Resources.icon32x32_ban;
            }
            else if (_Icon_RunAtStartUpMenuItem.Checked)
            {
                notifyIcon.Icon = Properties.Resources.icon32x32_good;
            } else if (!_Icon_RunAtStartUpMenuItem.Checked)
            {
                notifyIcon.Icon = Properties.Resources.icon32x32_exclamation;
            } else
            {
                notifyIcon.Icon = Properties.Resources.icon32x32;
            }
        }
        
        private void _Icon_RunAtStartupMenuItem_Click(object sender, EventArgs e)
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
            _Icon_RunAtStartUpMenuItem.Checked = AutoStartupHelper.IsAutorun();
            if (_Icon_RunAtStartUpMenuItem.Checked)
            {
                // Only succeed will ShowNotification.
                System.Threading.Thread.Sleep(300);
                ShowNotification("", string.Format(TranslationHelper.Get("Notify_RunAtStartup"),
                    Environment.NewLine));
            }
            ChangeIconStatus();
        }
     
        private async void DailyWallpaperConsSetWallpaper() {

            notifyIcon.Icon = Properties.Resources.icon32x32_timer;
            bool res = await DailyWallpaperCons.ShowDialog();
            System.Threading.Thread.Sleep(500);
            if (!res)
            {
                ShowNotification("", TranslationHelper.Get("Notify_SetWallpaper_Failed"));
            } else
            {
                ShowNotification("",
                    $"{TranslationHelper.Get("Notify_SetWallpaper_Succeed")} " +
                    $"{_ini.Read("WALLPAPER", "LOG")}");
            }
            ChangeIconStatus();
        }
        private void _Icon_ChangeWallpaperMenuItem_Click(object sender, EventArgs e)
        {
            DailyWallpaperConsSetWallpaper();
        }

        private void _Icon_DisableShortcutKeysMenuItem_Click(object sender, EventArgs e)
        {
            if (_Icon_DisableShortcutKeysMenuItem.Checked)
            {
                _Icon_DisableShortcutKeysMenuItem.Checked = false;
                _Icon_ChangeWallpaperMenuItem.ShowShortcutKeys = true;
                _ini.UpdateIniItem("UseShortcutKeys", "yes");
            }
            else
            {
                _Icon_DisableShortcutKeysMenuItem.Checked = true;
                _Icon_ChangeWallpaperMenuItem.ShowShortcutKeys = false;
                _ini.UpdateIniItem("UseShortcutKeys", "no");
            }
            ChangeIconStatus();
        }
        private void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Donothing
            e.Cancel = false;
        }
        // Has BUG.
        private void ContextMenuStrip_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ChangeIconStatus();
            //ToolStripDropDownClosingEventHandler
            if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
            {
                if (NotCloseMenu)
                {
                    e.Cancel = true;
                    NotCloseMenu = false;
                }
                else
                {
                    // e.Cancel = false; // who set this flag to false?? everytime?
                }
            }
        }

        
         private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            // Process.Start("explorer.exe", @"/select,""full-path-to-your-file""");
            //_ini.Read();
            var wallpaper = _ini.Read("WALLPAPER", "LOG");
            if (File.Exists(wallpaper))
            {
                // string p = @"C:\tmp\this path contains spaces, and,commas\target.txt";
                string args = string.Format("/e, /select, \"{0}\"", wallpaper);
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "explorer";
                info.Arguments = args;
                Process.Start(info);
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
            notifyIcon.ShowBalloonTip(timeout, title, content, isError ? ToolTipIcon.Error : ToolTipIcon.Info);
            notifyIcon.BalloonTipClicked += OnIconOnBalloonTipClicked;
            notifyIcon.BalloonTipClosed += OnIconOnBalloonTipClosed;

            void OnIconOnBalloonTipClicked(object sender, EventArgs e)
            {
                clickEvent?.Invoke();
                notifyIcon.BalloonTipClicked -= OnIconOnBalloonTipClicked;
            }

            void OnIconOnBalloonTipClosed(object sender, EventArgs e)
            {
                closeEvent?.Invoke();
                notifyIcon.BalloonTipClosed -= OnIconOnBalloonTipClosed;
            }
        }

        private void _Icon_BingMenuItem_Click(object sender, EventArgs e)
        {
            if (_Icon_BingMenuItem.Checked)
            {
                _Icon_BingMenuItem.Checked = false;
                _Icon_AlwaysDownLoadBingPictureMenuItem.Visible = false;
                _ini.UpdateIniItem("bingChina", "no", "Online");
            }
            else
            {
                _Icon_BingMenuItem.Checked = true;
                _ini.UpdateIniItem("bingChina", "yes", "Online");
                _Icon_AlwaysDownLoadBingPictureMenuItem.Visible = true;
                notifyIcon.ContextMenuStrip.Show();
            }
        }
        private void _Icon_AlwaysDownLoadBingPictureMenuItem_Click(object sender, EventArgs e)
        {
            var item = _Icon_AlwaysDownLoadBingPictureMenuItem;
            if (item.Checked)
            {
                item.Checked = false;
                _ini.UpdateIniItem("alwaysDLBingWallpaper", "no", "Online");
            }
            else
            {
                item.Checked = true;
                _ini.UpdateIniItem("alwaysDLBingWallpaper", "yes", "Online");
                notifyIcon.ContextMenuStrip.Show();
            }
        }

        private void _Icon_IssueAndFeedbackMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(ProjectInfo.NewIssue);

        }

        private void _Icon_ShowLogMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(ProjectInfo.logFile);
        }

        private void _Icon_LocalPathMenuItem_Click(object sender, EventArgs e)
        {
            if (_Icon_LocalPathMenuItem.Checked)
            {
                _Icon_LocalPathMenuItem.Checked = false;
                _Icon_LocalPathSettingMenuItem.Visible = false;
                _ini.UpdateIniItem("localPath", "no", "Local");
            }
            else
            {
                _Icon_LocalPathMenuItem.Checked = true;
                _Icon_LocalPathSettingMenuItem.Visible = true;
                _ini.UpdateIniItem("localPath", "yes", "Local");
                notifyIcon.ContextMenuStrip.Show();
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
                string pathSelected = null;
                var localPathSetting = _ini.GetCfgFromIni()["localPathSetting"];
                var deskTopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (!localPathSetting.ToLower().Equals("null") && Directory.Exists(localPathSetting))
                {
                    dialog.InitialDirectory = localPathSetting;
                } else
                {
                    dialog.InitialDirectory = deskTopPath;
                }
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.Multiselect = false;
                dialog.Title = TranslationHelper.Get("Icon_LocalPathSetting");
                // maybe add some log TODO
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    // MessageBox.Show("You selected: " + dialog.FileName);
                    pathSelected = dialog.FileName;
                }

                if (!string.IsNullOrEmpty(pathSelected))
                {
                    // Update to config.ini
                    _ini.UpdateIniItem("localPathSetting", pathSelected, "Local");
                    ShowNotification("", $"{TranslationHelper.Get("Notify_LocalPathSetting")} {pathSelected}");
                }
            }
        }

        private void _Icon_QuitMenuItem_Click(object sender, EventArgs e)
        {
            _ini.UpdateIniItem("appExitTime", DateTime.Now.ToString(),"LOG");
            notifyIcon.Dispose();
            Application.Exit();
        }

        private void _Icon_SpotlightMenuItem_Click(object sender, EventArgs e)
        {
            if (_Icon_SpotlightMenuItem.Checked)
            {
                _Icon_SpotlightMenuItem.Checked = false;
                _ini.UpdateIniItem("Spotlight", "no", "Online");
            }
            else
            {
                _Icon_SpotlightMenuItem.Checked = true;
                notifyIcon.ContextMenuStrip.Show();
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
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        
    }
}
