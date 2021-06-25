using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    public class DeviceManager : IDeviceManager
    {
        public DeviceManager()
        {
            Status = DeviceStatus.Uninitialised;
        }

        private System.Windows.Threading.DispatcherTimer _statusTimer;

        private void KillTimer()
        {
            if (_statusTimer != null)
            {
                _statusTimer.Stop();
                _statusTimer = null;
            }
        }
        
        public delegate void StatusChangeEvent();
        public event StatusChangeEvent OnStatusChange;

        #region IDeviceManager

        public string DeviceName { get; private set; }
        public DeviceStatus Status { get; private set; }
        public List<KeyValuePair<string, bool>> StatusFlags
        {
            get
            {
                List<KeyValuePair<string, bool>> statusFlags = new List<KeyValuePair<string, bool>>();

                switch (Status)
                {
                    case DeviceStatus.Running:
                        statusFlags.Add(new KeyValuePair<string, bool>("Warp Drive Online", true));
                        statusFlags.Add(new KeyValuePair<string, bool>("Impulse Engines Online", true));
                        statusFlags.Add(new KeyValuePair<string, bool>("Shields Ready", true));
                        statusFlags.Add(new KeyValuePair<string, bool>("Tea Maker Operational", true));
                        break;
                    case DeviceStatus.Error:
                        statusFlags.Add(new KeyValuePair<string, bool>("System Foobared", true));
                        statusFlags.Add(new KeyValuePair<string, bool>("Sympathy Required", true));
                        break;
                    case DeviceStatus.Uninitialised:
                        break;
                    case DeviceStatus.Initialised:
                        statusFlags.Add(new KeyValuePair<string, bool>("Warp Drive Online", false));
                        statusFlags.Add(new KeyValuePair<string, bool>("Impulse Engines Online", false));
                        statusFlags.Add(new KeyValuePair<string, bool>("Shields Ready", false));
                        statusFlags.Add(new KeyValuePair<string, bool>("Tea Maker Operational", true));
                        break;
                }
                return statusFlags;
            }
        }

        public void Initialise()
        {
            if (Status == DeviceStatus.Uninitialised)
            {
                Status = DeviceStatus.Initialised;
            }

            DeviceName = "Death Star";
        }

        public void Start()
        {
            if (Status == DeviceStatus.Initialised)
            {
                Status = DeviceStatus.Starting;
                // Simulate a real device with a simple timer
                _statusTimer = new System.Windows.Threading.DispatcherTimer(
                    new TimeSpan(0, 0, 3), 
                    System.Windows.Threading.DispatcherPriority.Normal,
                    delegate 
                    {
                        KillTimer();
                        Status = DeviceStatus.Running; 
                        _statusTimer = null; 
                        if (OnStatusChange != null)
                        {
                            OnStatusChange();
                        }
                    }, 
                    System.Windows.Threading.Dispatcher.CurrentDispatcher);
                
            }
        }

        public void Stop()
        {
            if (Status == DeviceStatus.Running)
            {
                Status = DeviceStatus.Initialised;
                if (OnStatusChange != null)
                {
                    OnStatusChange();
                }
            }
        }
        
        public void Terminate()
        {
            KillTimer();
            Stop();
            Status = DeviceStatus.Uninitialised;
        }

        #endregion
    }
}
