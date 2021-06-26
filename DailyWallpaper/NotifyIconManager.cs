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

namespace DailyWallpaper
{
    class NotifyIconManager : IDisposable
    {
        private static NotifyIconManager _instance;
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private ToolStripMenuItem _Icon_RunAtStartUpMenuItem;
        private ToolStripMenuItem _Icon_ChangeWallpaperMenuItem;
        private ToolStripMenuItem _Icon_EveryHoursAutoChangeMenuItem;
        private RadioButton h12RadioButton;
        private RadioButton h24RadioButton;
        private RadioButton h48RadioButton;
        private RadioButton customRadioButton;
        private TextBox hoursTextBox;
        private string textFromHoursTextBox;
        private ToolStripMenuItem _Icon_BingMenuItem;
        private ToolStripMenuItem _Icon_LocalPathMenuItem;
        private ToolStripMenuItem _Icon_LocalPathSettingMenuItem;
        private ToolStripMenuItem _Icon_SpotlightMenuItem;
        private ToolStripMenuItem _Icon_DisableShortcutKeysMenuItem;
        private ToolStripMenuItem _Icon_OptionsMenuItem;
        private ToolStripMenuItem _Icon_DonateAndSupportMenuItem;
        private ToolStripMenuItem _Icon_HelpMenuItem;
        private ToolStripMenuItem _Icon_OpenOfficialWebsiteMenuItem;
        private ToolStripMenuItem _Icon_CheckUpdateMenuItem;
        private ToolStripMenuItem _Icon_IssueAndFeedbackMenuItem;
        private ToolStripMenuItem _Icon_ShowLogMenuItem;
        private ToolStripMenuItem _Icon_AboutMenuItem;
        private ToolStripMenuItem _Icon_QuitMenuItem;
        private bool NotCloseMenu = false;
        private System.ComponentModel.IContainer _components;
        private ConfigIni _ini;
        //            _ini.RunAtStartup();
        private NotifyIconManager() {
            _ini = ConfigIni.GetInstance();
            _ini.UpdateIniItem("appStartTime", DateTime.Now.ToString(), "LOG");
            _components = new System.ComponentModel.Container();
            notifyIcon = new System.Windows.Forms.NotifyIcon(_components)
            {
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Properties.Resources.icon32x32,
                Text = string.Format(TranslationHelper.Get("Icon_ToolTip"),
                                Application.ProductVersion),
                Visible = true,
            };
            textFromHoursTextBox = "72";
            TrayIconInitializeComponent();
            ActionRegister();
            InitializeAllChecked();
        }

        private void TrayIconInitializeComponent()
        {
            if (notifyIcon.ContextMenuStrip.Items.Count != 0)
            {
                return;
            }
            // notifyIcon.ContextMenuStrip.ShowCheckMargin = true;
            //notifyIcon.ContextMenuStrip.RenderMode = ToolStripRenderMode.System;


            var contextMenuStripTitle = Application.ProductName + " by " + ProjectInfo.author;
            var _Icon_TitleMenuItem = new ToolStripMenuItem(contextMenuStripTitle) { Enabled = false };

            _Icon_ChangeWallpaperMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_ChangeWallpaper"),
                    TranslationHelper.Get("Icon_ChangeWallpaperTit"),
                    _Icon_ChangeWallpaperMenuItem_Click);
            var defFont = Control.DefaultFont;
            _Icon_ChangeWallpaperMenuItem.Font = new Font(defFont.Name, defFont.Size + 1, FontStyle.Bold);
            _Icon_ChangeWallpaperMenuItem.ShowShortcutKeys = true;
            _Icon_ChangeWallpaperMenuItem.ShortcutKeyDisplayString =
                TranslationHelper.Get("TrayIcon_ShortcutKeys"); // "Alt + F"
            // _Icon_ChangeWallpaperMenuItem.ShortcutKeyDisplayString.

            _Icon_EveryHoursAutoChangeMenuItem = ToolStripMenuItemWithHandler(
                    "    " + TranslationHelper.Get("Icon_AutoChange"),
                    eventHandler: null);
            _Icon_EveryHoursAutoChangeMenuItem.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            //_Icon_EveryHoursAutoChangeMenuItem.DisplayStyle

            _Icon_EveryHoursAutoChangeMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                CustomHoursTextboxWithButtonAndUnit(
                    TranslationHelper.Get("Icon_Custom"),
                    TranslationHelper.Get("Icon_Unit"))
            });

            // BingWaterMark Flag
            // alwaysDownload in options
            _Icon_BingMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_Bing"),
                    TranslationHelper.Get("Icon_BingTit"),
                    _Icon_BingMenuItem_Click);

            _Icon_LocalPathMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_LocalPath"),
                    TranslationHelper.Get("Icon_LocalPathTit"),
                    _Icon_LocalPathMenuItem_Click);

            _Icon_LocalPathSettingMenuItem = ToolStripMenuItemWithHandler(
                    "    " + TranslationHelper.Get("Icon_LocalPathSetting"),
                    TranslationHelper.Get("Icon_LocalPathSettingTit"),
                    _Icon_LocalPathSettingMenuItem_Click);
            _Icon_LocalPathSettingMenuItem.Visible = false;

            _Icon_SpotlightMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_Spotlight"),
                    TranslationHelper.Get("Icon_SpotlightTit"),
                    _Icon_SpotlightMenuItem_Click);

            _Icon_DisableShortcutKeysMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_DisableShortcutKeys"),
                    _Icon_DisableShortcutKeysMenuItem_Click);


            // TODO
            _Icon_OptionsMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_Options"),
                    (e, s) => { });

            _Icon_DonateAndSupportMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_DonateAndSupport"),
                    ProjectInfo.DonationUrl,
                    (e, s) => {
                        System.Diagnostics.Process.Start(ProjectInfo.DonationUrl);
                        ShowNotification("", TranslationHelper.Get("Notify_ThanksForDonation"));
                    });

            // Help and DropDownItems
            _Icon_HelpMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_Help"),
                    (e, s) => { });

            _Icon_OpenOfficialWebsiteMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_OpenOfficialWebsite"),
                    ProjectInfo.OfficalWebSite,
                    (e, s) => { System.Diagnostics.Process.Start(ProjectInfo.OfficalWebSite); });
            _Icon_CheckUpdateMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_CheckUpdate"),
                    (e, s) => { });

            _Icon_IssueAndFeedbackMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_IssueAndFeedback"),
                    ProjectInfo.NewIssue,
                    _Icon_IssueAndFeedbackMenuItem_Click);
            _Icon_ShowLogMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_ShowLog"),
                    _Icon_ShowLogMenuItem_Click);
            _Icon_AboutMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_About"),
                    (e, s) => { }); //ShowAboutView();

            _Icon_HelpMenuItem.DropDownItems.AddRange(new ToolStripItem[] {
                _Icon_OpenOfficialWebsiteMenuItem,
                _Icon_CheckUpdateMenuItem,
                _Icon_IssueAndFeedbackMenuItem,
                new ToolStripSeparator(),
                _Icon_ShowLogMenuItem,
                _Icon_AboutMenuItem}
            );
            _Icon_RunAtStartUpMenuItem = ToolStripMenuItemWithHandler(
                        TranslationHelper.Get("Icon_RunAtStartup"),
                        _Icon_RunAtStartupMenuItem_Click);
            _Icon_QuitMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_Quit"),
                    _Icon_QuitMenuItem_Click);

            notifyIcon.ContextMenuStrip.Items.Add(_Icon_TitleMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_ChangeWallpaperMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_EveryHoursAutoChangeMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_BingMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_LocalPathMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_LocalPathSettingMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_SpotlightMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_DisableShortcutKeysMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_OptionsMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_DonateAndSupportMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_HelpMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_RunAtStartUpMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_QuitMenuItem);
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
            }
        }
        private void h24RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h24RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 24.ToString());
                hoursTextBox.Enabled = false;
                notifyIcon.ContextMenuStrip.Close();
            }
        }
        private void h48RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (h48RadioButton.Checked)
            {
                _ini.UpdateIniItem("Timer", 48.ToString());
                hoursTextBox.Enabled = false;
                notifyIcon.ContextMenuStrip.Close();
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
            }
           
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
            radioButton.Location = new System.Drawing.Point(3, height - 2);
            var unitLabel = new Label();
            unitLabel.AutoSize = true;
            var column = (int)radioButton.Font.Size;
            if (hours == 0)
            {
                radioButton.Name = "customRadioButton";
                radioButton.Text = !string.IsNullOrEmpty(buttonStr)? buttonStr:"Custom";
                hoursTextBox.Name = "hoursTextBox";
                hoursTextBox.Width = 28;
                hoursTextBox.Location = new System.Drawing.Point(radioButton.Right + column, height);
                hoursTextBox.TextAlign = HorizontalAlignment.Center;
                hoursTextBox.KeyDown += hoursTextBox_KeyDown;
                hoursTextBox.KeyPress += hoursTextBox_KeyPress;
                //hoursTextBox.TextChanged += hoursTextBox_TextChanged;
                unitLabel.Name = "customUnitLabel";
                unitLabel.Location = new System.Drawing.Point(hoursTextBox.Right + column, height);
                unitLabel.Text = unitText;
                panel.Controls.Add(hoursTextBox);

            }
            else
            {
                //radioButton.Width -= 50;
                radioButton.Name = "radioButton" + hours.ToString();
                unitLabel.Name = "unitLabel" + hours.ToString();
                unitLabel.Location = new System.Drawing.Point(radioButton.Right + column, height);
                unitLabel.Text = hours.ToString() + "  "+ unitText;
            }
            // MessageBox.Show(radioButton.Width.ToString()); default is 104
            panel.Controls.Add(radioButton);
            panel.Controls.Add(unitLabel);
        }
        private ToolStripControlHost CustomHoursTextboxWithButtonAndUnit(string buttonStr, string unitText, bool custom=false)
        {
            var backColor = SystemColors.Window;
            hoursTextBox = new System.Windows.Forms.TextBox();
            
            h12RadioButton = new RadioButton();
            h24RadioButton = new RadioButton();
            h48RadioButton = new RadioButton();
            customRadioButton = new RadioButton();
            h12RadioButton.CheckedChanged += h12RadioButton_CheckedChanged;
            h24RadioButton.CheckedChanged += h24RadioButton_CheckedChanged;
            h48RadioButton.CheckedChanged += h48RadioButton_CheckedChanged;
            customRadioButton.CheckedChanged += customRadioButton_CheckedChanged;

            var panel = new System.Windows.Forms.Panel();
            panel.SuspendLayout(); // IS NOT DIFF ?
            var unit = TranslationHelper.Get("Icon_Unit");
            AddDivIntoPanel(panel, h12RadioButton, 5, unit, 12);
            AddDivIntoPanel(panel, h24RadioButton, 35, unit, 24);
            AddDivIntoPanel(panel, h48RadioButton, 65, unit, 48);
            AddDivIntoPanel(panel, customRadioButton, 95, unit, buttonStr:TranslationHelper.Get("Icon_Custom"));
            panel.Name = "combine";
            panel.AutoSize = true;
            // panel.Size = new System.Drawing.Size(241, 37);
            //MessageBox.Show(_Icon_Every24HoursMenuItem.); // 32, 19
            // panel.TabIndex = 7;
            var panelHost = new ToolStripControlHost(panel);
            panelHost.BackColor = backColor;

            return panelHost;
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
        private void InitializeAllChecked()
        {
            var startFeatures = _ini.GetCfgFromIni();
            if (startFeatures["bingChina"].ToLower().Equals("yes"))
            {
                _Icon_BingMenuItem.Checked = true;
            }
            if (AutoStartupHelper.IsAutorun())
            {
                _Icon_RunAtStartUpMenuItem.Checked = true;
                notifyIcon.Icon = Properties.Resources.icon32x32_good;
            } else
            {
                _Icon_RunAtStartUpMenuItem.Checked = false;
                notifyIcon.Icon = Properties.Resources.icon32x32_exclamation;
            }


            if (startFeatures["Spotlight"].ToLower().Equals("yes"))
            {
                _Icon_SpotlightMenuItem.Checked = true;
            }
            if (startFeatures["localPath"].ToLower().Equals("yes"))
            {
                _Icon_LocalPathMenuItem.Checked = true;
                _Icon_LocalPathSettingMenuItem.Visible = true;
            }
            if (startFeatures["UseShortcutKeys"].ToLower().Equals("yes"))
            {
                _Icon_DisableShortcutKeysMenuItem.Checked = false;
            }

            string timerStr = startFeatures["Timer"];
            hoursTextBox.Enabled = false;
            if (timerStr.Equals("12"))
            {
                h12RadioButton.Checked = true;
            } 
            else if (timerStr.Equals("24"))
            {
                h24RadioButton.Checked = true;
            }
            else if (timerStr.Equals("48"))
            {
                h48RadioButton.Checked = true;
            }
            else
            {
                customRadioButton.Checked = true;
                hoursTextBox.Enabled = true;
                hoursTextBox.Text = timerStr;
                textFromHoursTextBox = timerStr;
            }
        }

        private delegate void HoursHandler(int num);
        private void UpdateEveryHoursInConfig(int hours)
        {

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
        private void ActionRegister()
        {
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.ContextMenuStrip.Closing += ContextMenuStrip_Closing;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
        }


        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            if (_ini.GetCfgFromIni()["UseShortcutKeys"].ToLower().Equals("yes"))
            {
                DailyWallpaperConsSetWallpaper();
            }
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
                    $"{_ini.Read("wallpaperInLog", "LOG")}");
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

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
            {
                item.Click += eventHandler;
            }
            item.Checked = false;
            return item;
        }

        private ToolStripMenuItem ToolStripMenuItemWithHandler(string displayText, string tooltipText, EventHandler eventHandler)
        {
            var item = new ToolStripMenuItem(displayText);
            if (eventHandler != null)
            {
                item.Click += eventHandler;
            }
            if (!String.IsNullOrEmpty(tooltipText))
            {
                item.ToolTipText = tooltipText;
            }
            item.Checked = false;
            return item;
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
        public void ShowNotification(
            string title,
            string content,
            bool isError = false,
            int timeout = 5000,
            Action clickEvent = null,
            Action closeEvent = null)
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
                _ini.UpdateIniItem("bingChina", "no", "Online");
            }
            else
            {
                _Icon_BingMenuItem.Checked = true;
                _ini.UpdateIniItem("bingChina", "yes", "Online");
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
                var lastLocalPathSetting = _ini.GetCfgFromIni()["lastLocalPathSetting"];
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
                    if (!pathSelected.Equals(lastLocalPathSetting)) { 
                        _ini.UpdateIniItem("lastLocalPathSetting", pathSelected, "LOG");
                    }
                }
            }
        }

        private void _Icon_QuitMenuItem_Click(object sender, EventArgs e)
        {
            _ini.UpdateIniItem("appExitTime", DateTime.Now.ToString(),"LOG");
            Application.Exit();
        }

        // Application.Run(new OpenFileDialogForm());

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
        private void notifyIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                MethodInfo mi = typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic);
                mi.Invoke(notifyIcon, null);
            }
        }

        public static NotifyIconManager GetInstance()
        {
            return _instance ?? (_instance = new NotifyIconManager());
        }

        public void Dispose()
        {
            notifyIcon.Visible = false;
            notifyIcon.Dispose();
        }
    }
}
