using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper
{
    public class DWApplicationContext : ApplicationContext
    {
        public DWApplicationContext()
        {
            _deviceManager = new DeviceManager();
            _viewManager = new ViewManager(_deviceManager);

            _deviceManager.OnStatusChange += _viewManager.OnStatusChange;

            _deviceManager.Initialise();
        }

        private ViewManager _viewManager;
        private DeviceManager _deviceManager;

        // Called from the Dispose method of the base class
        protected override void Dispose(bool disposing)
        {
            if ((_deviceManager != null) && (_viewManager != null))
            {
                _deviceManager.OnStatusChange -= _viewManager.OnStatusChange;
            }
            if (_deviceManager != null)
            {
                _deviceManager.Terminate();
            }
            _deviceManager = null;
            _viewManager = null;
        }
    }
}
