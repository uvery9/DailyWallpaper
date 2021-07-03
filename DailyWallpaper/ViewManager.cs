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

        protected override void Dispose(bool disposing)
        {
            _notifyIcon.Dispose();
        }


    }
}
