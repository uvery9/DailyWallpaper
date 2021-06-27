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
    partial class NotifyIconManager
    {
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
                    "    " + TranslationHelper.Get("Icon_AutoChangeWallpaper"),
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

            _Icon_AlwaysDownLoadBingPictureMenuItem = ToolStripMenuItemWithHandler(
                    "    " + TranslationHelper.Get("Icon_AlwaysDownLoadBingPicture"),
                    _Icon_AlwaysDownLoadBingPictureMenuItem_Click);

            _Icon_BingAddWaterMarkMenuItem = ToolStripMenuItemWithHandler(
                    "    " + TranslationHelper.Get("Icon_BingAddWaterMark"),
                    _Icon_BingAddWaterMarkMenuItem_Click);
            

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
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_AlwaysDownLoadBingPictureMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_BingAddWaterMarkMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_LocalPathMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_LocalPathSettingMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_SpotlightMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_DisableShortcutKeysMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_OptionsMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());           
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_HelpMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_RunAtStartUpMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_DonateAndSupportMenuItem);
            notifyIcon.ContextMenuStrip.Items.Add(_Icon_QuitMenuItem);
            _viewWindow = View.LogWindow.GetInstance(Properties.Resources.icon32x32);
            _viewWindow.FormClosing += _viewWindow_FormClosing;
            _viewWindow.Load += new System.EventHandler(_viewWindow_Load);
            _writerFile = new StreamWriter(ProjectInfo.logFile);
            Console.SetOut(_writerFile);
            Console.SetError(_writerFile);
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
                radioButton.Text = !string.IsNullOrEmpty(buttonStr) ? buttonStr : "Custom";
                hoursTextBox.Name = "hoursTextBox";
                hoursTextBox.Width = 28;
                hoursTextBox.Location = new System.Drawing.Point(radioButton.Right + column, height);
                hoursTextBox.TextAlign = HorizontalAlignment.Center;
                hoursTextBox.KeyDown += hoursTextBox_KeyDown;
                hoursTextBox.KeyPress += hoursTextBox_KeyPress;
                //hoursTextBox.TextChanged += hoursTextBox_TextChanged;
                unitLabel.Name = "customUnitLabel";
                unitLabel.Location = new System.Drawing.Point(hoursTextBox.Right + column, height);
                unitLabel.Text = "  " + unitText;
                panel.Controls.Add(hoursTextBox);

            }
            else
            {
                //radioButton.Width -= 50;
                radioButton.Name = "radioButton" + hours.ToString();
                unitLabel.Name = "unitLabel" + hours.ToString();
                unitLabel.Location = new System.Drawing.Point(radioButton.Right + column, height);
                unitLabel.Text = hours.ToString() + "  " + unitText;
            }
            // MessageBox.Show(radioButton.Width.ToString()); default is 104
            panel.Controls.Add(radioButton);
            panel.Controls.Add(unitLabel);
        }
        private ToolStripControlHost CustomHoursTextboxWithButtonAndUnit(string buttonStr, string unitText, bool custom = false)
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
            AddDivIntoPanel(panel, customRadioButton, 95, unit, buttonStr: TranslationHelper.Get("Icon_Custom"));
            panel.Name = "combine";
            panel.AutoSize = true;
            // panel.Size = new System.Drawing.Size(241, 37);
            //MessageBox.Show(_Icon_Every24HoursMenuItem.); // 32, 19
            // panel.TabIndex = 7;
            var panelHost = new ToolStripControlHost(panel);
            panelHost.BackColor = backColor;

            return panelHost;
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
        public static NotifyIconManager GetInstance()
        {
            return _instance ?? (_instance = new NotifyIconManager());
        }

        public void Dispose()
        {
            notifyIcon.Visible = false;
            _timerHelper.Dispose();
            _exitTimeHelper.Dispose();
            notifyIcon.Dispose();
        }
        private void ActionRegister()
        {
            notifyIcon.ContextMenuStrip.Opening += ContextMenuStrip_Opening;
            notifyIcon.ContextMenuStrip.Closing += ContextMenuStrip_Closing;
            notifyIcon.DoubleClick += notifyIcon_DoubleClick;
            notifyIcon.BalloonTipClicked += notifyIcon_BalloonTipClicked;
            notifyIcon.MouseUp += notifyIcon_MouseUp;
        }
        private void InitializeCheckedAndTimer()
        {
            var startFeatures = _ini.GetCfgFromIni();
            if (startFeatures["bing"].ToLower().Equals("yes"))
            {
                _Icon_BingMenuItem.Checked = true;
                _Icon_AlwaysDownLoadBingPictureMenuItem.Visible = true;
                _Icon_BingAddWaterMarkMenuItem.Visible = true;
            }

            if (startFeatures["alwaysDLBingWallpaper"].ToLower().Equals("yes"))
            {
                _Icon_AlwaysDownLoadBingPictureMenuItem.Checked = true;
            }
            if (_ini.Read("bingWMK", "Online").ToLower().Equals("yes"))
            {
                _Icon_BingAddWaterMarkMenuItem.Checked = true;
            }

            if (AutoStartupHelper.IsAutorun())
            {
                _Icon_RunAtStartUpMenuItem.Checked = true;
                notifyIcon.Icon = Properties.Resources.icon32x32_good;
            }
            else
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

            if (int.TryParse(timerStr, out int res))
            {
                if (_ini.Read("TimerSetWallpaper", "LOG").ToLower().Equals("true"))
                {
                    _timerHelper.SetTimer(res * 60);
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
                    // var localPathMtime = new FileInfo(this.path).LastWriteTime;
                    var timeDiff = (int)(appExitTime - appStartTime).TotalMinutes;
                    _ini.UpdateIniItem("LastExitSubStartTimeDiff", $"{timeDiff}mins");
                    if (timeDiff > res * 60)
                    {
                        // something wrong. never mind.
                        _timerHelper.SetTimer(res * 60);
                        _ini.UpdateIniItem("TimerNotWorkProperly ", $"{DateTime.Now.ToString()}");
                    }
                    else
                    {
                        // rest time = 60 * res - lastTimeNotFinishe.
                        _timerHelper.SetTimer(res * 60 - timeDiff);
                    }

                }
            }
            else
            {
                _ini.UpdateIniItem("Timer", "24");
                _timerHelper.SetTimer(24 * 60);
            }
            // 1min dueTime, 30 mins period.
            _exitTimeHelper = new System.Threading.Timer(exitTimeHelperCallback, null, 1000 * 60, 1000 * 60 * 30);
            _ini.UpdateIniItem("appStartTime", DateTime.Now.ToString(), "LOG");
        }
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
        private ToolStripMenuItem _Icon_AlwaysDownLoadBingPictureMenuItem;
        private ToolStripMenuItem _Icon_BingAddWaterMarkMenuItem;
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
        private TimerHelper _timerHelper;
        private System.Threading.Timer _exitTimeHelper;
        private bool setWallpaperSucceed = false;
        private View.LogWindow _viewWindow;
        private StreamWriter _writerFile;

    }
}
