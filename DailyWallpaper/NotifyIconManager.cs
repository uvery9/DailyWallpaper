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
            textFromHoursTextBox = "3";
            TrayIconInitializeComponent();
            ActionRegister();
            InitializeCheckedAndTimer();
        }

        // call back by timer.
        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (_ini.GetCfgFromIni()["UseShortcutKeys"].ToLower().Equals("yes"))
            {
                //DailyWallpaperConsSetWallpaper();
                // Will NOT wait
                Task.Run(async () => await DailyWallpaperConsSetWallpaperAsync());
            }
        }
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Task.Run(async () => await DailyWallpaperConsSetWallpaperAsync()).Wait();
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
                if (sender is TextBox box)
                {
                    int result;
                    if (int.TryParse(box.Text, out result))
                    {
                        textFromHoursTextBox = box.Text;
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
        private void h6RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h6RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 6.ToString());
                hoursTextBox.Enabled = false;
                notifyIcon.ContextMenuStrip.Close();
                _timerHelper.SetTimer(6 * 60);
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
            if (consRunning)
            {
                notifyIcon.Icon = Properties.Resources.icon32x32_timer;
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

        private bool IsNoneSelected()
        {
            if (_Icon_BingMenuItem.Checked == false &&
                _Icon_LocalPathMenuItem.Checked == false &&
                _Icon_SpotlightMenuItem.Checked == false              
                )
            {
                return true;
            } else
            {
                return false;
            }            
        }

        private async Task DailyWallpaperConsSetWallpaperAsync()
        {
            await Task.Run(() => DailyWallpaperConsSetWallpaper());
        }
        private void DailyWallpaperConsSetWallpaper() {

            if (consRunning)
            {
                return;
            }
            consRunning = true;
            // notifyIcon.
            try
            {
                if (IsNoneSelected())
                {
                    ShowNotification("", TranslationHelper.Get("Notify_AtLeastSelectOneFeature"), isError: true);
                    return;
                }
                notifyIcon.Icon = Properties.Resources.icon32x32_timer;
                bool res;
                if (useTextBoxWriter)
                {
                    iStextFromFileNew = false;
                    res = DailyWallpaperCons.GetInstance().ShowDialog(true, _viewWindow.textWriter);
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
                    ShowNotification("",
                        string.Format(TranslationHelper.Get("Notify_SetWallpaper_Failed"), Environment.NewLine));

                }
                else
                {
                    setWallpaperSucceed = true;
                    ShowNotification("",
                        string.Format(TranslationHelper.Get("Notify_SetWallpaper_Succeed"),
                        Environment.NewLine + $"{_ini.Read("WALLPAPER", "LOG")}")
                        );
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
        private void _Icon_ChangeWallpaperMenuItem_Click(object sender, EventArgs e)
        {
            // DailyWallpaperConsSetWallpaper();
            // NOT WAIT
            Task.Run(async () => await DailyWallpaperConsSetWallpaperAsync());
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
            var wallpaper = _ini.Read("WALLPAPER", "LOG");
            if (File.Exists(wallpaper) && setWallpaperSucceed)
            {
                // string p = @"C:\tmp\this path contains spaces, and,commas\target.txt";
                string args = string.Format("/e, /select, \"{0}\"", wallpaper);
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = "explorer";
                info.Arguments = args;
                Process.Start(info);
            }
            /*if (!setWallpaperSucceed)
            {
                Process.Start(ProjectInfo.logFile);
            }*/
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
                _Icon_BingAddWaterMarkMenuItem.Visible = false;
                _ini.UpdateIniItem("bing", "no", "Online");
            }
            else
            {
                _Icon_BingMenuItem.Checked = true;
                _ini.UpdateIniItem("bing", "yes", "Online");
                _Icon_AlwaysDownLoadBingPictureMenuItem.Visible = true;
                _Icon_BingAddWaterMarkMenuItem.Visible = true;
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
        private void _Icon_BingAddWaterMarkMenuItem_Click(object sender, EventArgs e)
        {
            var item = _Icon_BingAddWaterMarkMenuItem;
            if (item.Checked)
            {
                item.Checked = false;
                _ini.UpdateIniItem("bingWMK", "no", "Online");
            }
            else
            {
                item.Checked = true;
                _ini.UpdateIniItem("bingWMK", "yes", "Online");
                notifyIcon.ContextMenuStrip.Show();
            }
        }
 
        private void _Icon_IssueAndFeedbackMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(ProjectInfo.NewIssue);

        }

        
        private void _cefWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                _cefWindow.Hide();
            }
        }
        private void _viewWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                if (!consRunning) {
                    useTextBoxWriter = false;
                }
                //_viewWindow.WindowState = FormWindowState.Minimized;
                //_viewWindow.ShowInTaskbar = false;
                _viewWindow.Hide();
                // _viewWindow.textBoxCons.Text = "";
            }
        }

        private void _viewWindow_Load(object sender, EventArgs e)
        {
            
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            _viewWindow.textBoxCons.Text = "";
        }

        private void saveToFileButton_Click(object sender, EventArgs e)
        {
            if (_viewWindow.textBoxCons.Text.Length < 1)
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
                        byte[] byteArray = System.Text.Encoding.Default.GetBytes(_viewWindow.textBoxCons.Text);
                        stream.Write(byteArray, 0, byteArray.Length);
                    }
                }
            }
        }

        private void _Icon_ShowLogMenuItem_Click(object sender, EventArgs e)
        {
            // Process.Start(ProjectInfo.logFile);
            if (consRunning)
            {
                if (useTextBoxWriter)
                {
                    _viewWindow.Show();
                    return;
                } else
                {
                    ShowNotification("", 
                        "You cannot redirect stdout/stderr " + 
                        "while the console program is running.", true);
                    return;
                }
                
            }
            useTextBoxWriter = true;
            _viewWindow.Show();
            if (iStextFromFileNew)
            {
                if (File.Exists(ProjectInfo.logFile))
                {
                    var textBoxCons = File.ReadAllText(ProjectInfo.logFile);
                    _viewWindow.textBoxCons.Text = textBoxCons;
                    // _viewWindow.textBoxCons
                    _viewWindow.textBoxCons.Select(_viewWindow.textBoxCons.TextLength, 0);//光标定位到文本最后
                    _viewWindow.textBoxCons.ScrollToCaret();
                }
            }
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
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    // MessageBox.Show("You selected: " + dialog.FileName);
                    _ini.UpdateIniItem("localPathSetting", dialog.FileName, "Local");
                    ShowNotification("", $"{TranslationHelper.Get("Notify_LocalPathSetting")} {dialog.FileName}");
                }
            }
        }

        private void _Icon_QuitMenuItem_Click(object sender, EventArgs e)
        {
            _ini.UpdateIniItem("appExitTime", DateTime.Now.ToString(),"LOG");
            _cefWindow.Dispose();
            _viewWindow.Dispose();
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
            /*            if (e.Button == MouseButtons.Left)
                        {
                            MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                            mi.Invoke(notifyIcon, null);
                        }*/
        }

        
    }
}
