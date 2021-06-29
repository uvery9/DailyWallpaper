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
    public class ViewManager: ApplicationContext
    {
        public ViewManager()
        {

             var notifyIconHelper = NotifyIconManager.GetInstance();
            _notifyIcon = notifyIconHelper.notifyIcon;

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
                System.Drawing.Icon icon = Properties.Resources.icon32x32;
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
        private DailyWallpaperWpfLib.View.AboutView _aboutView;
        private DailyWallpaperWpfLib.ViewModel.AboutViewModel _aboutViewModel;
        private DailyWallpaperWpfLib.ViewModel.StatusViewModel _statusViewModel;

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

            _aboutViewModel.AddVersionInfo("Hardware", "DeviceName");
            _aboutViewModel.AddVersionInfo("Version", Assembly.GetExecutingAssembly().GetName().Version.ToString());
            _aboutViewModel.AddVersionInfo("Serial Number", "142573462354");
        }


        protected override void Dispose(bool disposing)
        {
            _notifyIcon.Dispose();
        }


    }
}
