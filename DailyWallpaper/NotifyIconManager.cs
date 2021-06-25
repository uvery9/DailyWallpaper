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

namespace DailyWallpaper
{
    class NotifyIconManager:IDisposable
    {
        private static NotifyIconManager _instance;
        public System.Windows.Forms.NotifyIcon notifyIcon;
        private ToolStripMenuItem _Icon_RunAtStartUpMenuItem;
        private ToolStripMenuItem _Icon_ChangeWallpaperMenuItem;
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
            TrayIconInitializeComponent();
            ActionRegister();
        }

        private void TrayIconInitializeComponent()
        {
            if (notifyIcon.ContextMenuStrip.Items.Count != 0)
            {
                return;
            }
            var _Icon_TitleMenuItem = new ToolStripMenuItem(Application.ProductName + " by " + ProjectInfo.author) { Enabled = false };

            _Icon_ChangeWallpaperMenuItem = ToolStripMenuItemWithHandler(
                    TranslationHelper.Get("Icon_ChangeWallpaper"),
                    TranslationHelper.Get("Icon_ChangeWallpaperTit"),
                    _Icon_ChangeWallpaperMenuItem_Click);
            var defFont = Control.DefaultFont;
            _Icon_ChangeWallpaperMenuItem.Font = new Font(defFont.Name, defFont.Size + 2, FontStyle.Bold);
            _Icon_ChangeWallpaperMenuItem.ShowShortcutKeys = true;
            _Icon_ChangeWallpaperMenuItem.ShortcutKeyDisplayString = 
                TranslationHelper.Get("TrayIcon_ShortcutKeys"); // "Alt + F"
            // _Icon_ChangeWallpaperMenuItem.ShortcutKeyDisplayString.

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
                    (e, s) => { });
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
            InitializeAllChecked();
            // notifyIcon.KeyPreview = true;
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
            } else if(!_Icon_RunAtStartUpMenuItem.Checked)
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

        private async void DailyWallpaperConsSetWallpaper(){

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
            System.Diagnostics.Process.Start(ProjectInfo.NewIssue);

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
