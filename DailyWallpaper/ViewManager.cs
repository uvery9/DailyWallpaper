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
    public class ViewManager
    {
        public ViewManager(IDeviceManager deviceManager)
        {
            System.Diagnostics.Debug.Assert(deviceManager != null);

            _deviceManager = deviceManager;

            var notifyIconHelper = NotifyIconManager.GetInstance();
            _notifyIcon = notifyIconHelper.notifyIcon;
             // _notifyIcon.MouseUp += notifyIcon_MouseUp;

             _aboutViewModel = new DailyWallpaperWpfLib.ViewModel.AboutViewModel();
            _statusViewModel = new DailyWallpaperWpfLib.ViewModel.StatusViewModel();

            _statusViewModel.Icon = AppIcon;
            _aboutViewModel.Icon = _statusViewModel.Icon;

            _hiddenWindow = new System.Windows.Window();
            _hiddenWindow.Hide();
            
        }

        System.Windows.Media.ImageSource AppIcon
        {
            get
            {
                System.Drawing.Icon icon = (_deviceManager.Status == DeviceStatus.Running) ? Properties.Resources.icon32x32 : Properties.Resources.icon32x32_ban;
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle, 
                    System.Windows.Int32Rect.Empty, 
                    System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
        }

        // This allows code to be run on a GUI thread
        private System.Windows.Window _hiddenWindow;

        
        // The Windows system tray class
        private NotifyIcon _notifyIcon;  
        IDeviceManager _deviceManager;
        private DailyWallpaperWpfLib.View.AboutView _aboutView;
        private DailyWallpaperWpfLib.ViewModel.AboutViewModel _aboutViewModel;
        private DailyWallpaperWpfLib.View.StatusView _statusView;
        private DailyWallpaperWpfLib.ViewModel.StatusViewModel _statusViewModel;

            

        private void DisplayStatusMessage(string text)
        {
            _hiddenWindow.Dispatcher.Invoke(delegate
            {
                _notifyIcon.BalloonTipText = _deviceManager.DeviceName + ": " + text;
                // The timeout is ignored on recent Windows
                _notifyIcon.ShowBalloonTip(3000);
            });
        }

        /*private void UpdateStatusView()
        {
            if ((_statusViewModel != null) && (_deviceManager != null))
            {
                List<KeyValuePair<string, bool>> flags = _deviceManager.StatusFlags;
                List<KeyValuePair<string, string>> statusItems = flags.Select(n => new KeyValuePair<string, string>(n.Key, n.Value.ToString())).ToList();
                statusItems.Insert(0, new KeyValuePair<string, string>("Device", _deviceManager.DeviceName));
                statusItems.Insert(1, new KeyValuePair<string, string>("Status", _deviceManager.Status.ToString()));
                _statusViewModel.SetStatusFlags(statusItems);
            }
        }*/

        

        public void OnStatusChange()
        {
            // UpdateStatusView();

            switch (_deviceManager.Status)
            {
                case DeviceStatus.Initialised:
                    _notifyIcon.Text = _deviceManager.DeviceName + ": Ready";
                    _notifyIcon.Icon = Properties.Resources.icon32x32_ban;
                    DisplayStatusMessage("Idle");
                    break;
                case DeviceStatus.Running:
                    _notifyIcon.Text = _deviceManager.DeviceName + ": Running";
                    _notifyIcon.Icon = Properties.Resources.icon32x32;
                    DisplayStatusMessage("Running");
                    break;
                case DeviceStatus.Starting:
                    _notifyIcon.Text = _deviceManager.DeviceName + ": Starting";
                    _notifyIcon.Icon = Properties.Resources.icon32x32_ban;
                    DisplayStatusMessage("Starting");
                    break;
                case DeviceStatus.Uninitialised:
                    _notifyIcon.Text = _deviceManager.DeviceName + ": Not Ready";
                    _notifyIcon.Icon = Properties.Resources.icon32x32_ban;
                    break;
                case DeviceStatus.Error:
                    _notifyIcon.Text = _deviceManager.DeviceName + ": Error Detected";
                    _notifyIcon.Icon = Properties.Resources.icon32x32_ban;
                    break;
                default:
                    _notifyIcon.Text = _deviceManager.DeviceName + ": -";
                    _notifyIcon.Icon = Properties.Resources.icon32x32_ban;
                    break;
            }
            System.Windows.Media.ImageSource icon = AppIcon;
            if (_aboutView != null)
            {
                _aboutView.Icon = AppIcon;
            }
            if (_statusView != null)
            {
                _statusView.Icon = AppIcon;
            }
        }




        
        /*private void ShowStatusView()
        {
            if (_statusView == null)
            {
                _statusView = new DailyWallpaperWpfLib.View.StatusView();
                _statusView.DataContext = _statusViewModel;

                _statusView.Closi ng += ((arg_1, arg_2) => _statusView = null);
                _statusView.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
                _statusView.Show();
                //UpdateStatusView();
            }
            else
            {
                _statusView.Activate();
            }
            _statusView.Icon = AppIcon;
        }*/

        /*private void showStatusItem_Click(object sender, EventArgs e)
        {
            ShowStatusView();
        }*/

        private void ShowAboutView()
        {
            if (_aboutView == null)
            {
                _aboutView = new DailyWallpaperWpfLib.View.AboutView();
                _aboutView.DataContext = _aboutViewModel;
                _aboutView.Closing += ((arg_1, arg_2) => _aboutView = null);
                _aboutView.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;

                _aboutView.Show();
            }
            else
            {
                _aboutView.Activate();
            }
            _aboutView.Icon = AppIcon;

            _aboutViewModel.AddVersionInfo("Hardware", _deviceManager.DeviceName);
            _aboutViewModel.AddVersionInfo("Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            _aboutViewModel.AddVersionInfo("Serial Number", "142573462354");
        }
        
        

        

    }
}
