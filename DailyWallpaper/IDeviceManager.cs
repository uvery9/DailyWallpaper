using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    /*
     * A simple dummy device with some simple commands to control its state
     */
    public interface IDeviceManager
    {
        string DeviceName { get; }
        DeviceStatus Status { get; }
        List<KeyValuePair<string, bool>> StatusFlags { get; }
        void Initialise();
        void Start();
        void Stop();
        void Terminate();
    }
}
