using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.Helpers
{
    class ProjectInfo
    {
        public static bool connectToWorld = false;
        public static string author = "Jared DC";
        public static string email = "jared.dcx@gmail.com";
        public static string exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public static string executingLocation = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        public static string logFile = new System.IO.FileInfo(exeName + ".log.txt").FullName;
        private static string officalWebSiteGlobal = "https://github.com/JaredDC/DailyWallpaper";
        private static string officalWebSiteCHN = "https://gitee.com/imtvip/DailyWallpaper";
        private static string newIssueCHN = "https://gitee.com/imtvip/DailyWallpaper/issues/new";
        private static string newIssueGlobal = "https://github.com/JaredDC/DailyWallpaper/issues/new";
        private static string donationUrlGlobal   = "https://github.com/JaredDC/DailyWallpaper/tree/master/Donate";
        private static string donationUrlCHN = "https://gitee.com/imtvip/DailyWallpaper/tree/master/Donate";
        private static Color backColor = 
            Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(243)))), ((int)(((byte)(214)))));

        /// <summary>
        /// return result will block the program.
        /// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=net-5.0
        /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run?view=net-5.0
        /// NEED TO FIX: WHEN SOMEBODY USE VPN/SOMETHING, (S)HE CAN VISIT THR WEBSIT(SERVER), BUT THIS METHOD CAN'T.
        /// </summary>
        public static void TestConnectUsingSocket(Action<bool, string> updateFunc, string server = "www.google.com", int port = 80)
        {
            bool innerRun()
            {
                try
                {
                    Socket s = null;
                    IPHostEntry hostEntry = null;

                    // Get host related information.
                    hostEntry = Dns.GetHostEntry(server);

                    // Loop through the AddressList to obtain the supported AddressFamily. This is to avoid
                    // an exception that occurs when the host IP Address is not compatible with the address family
                    // (typical in the IPv6 case).
                    foreach (IPAddress address in hostEntry.AddressList)
                    {
                        IPEndPoint ipe = new IPEndPoint(address, port);
                        Socket tempSocket =
                            new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                        tempSocket.Connect(ipe);

                        if (tempSocket.Connected)
                        {
                            s = tempSocket;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    if (s == null)
                    {
                        connectToWorld = false;
                        updateFunc(false, $"Can not conneted to {server}.");
                        return false;
                    }
                    connectToWorld = true;
                    updateFunc(true, $"Conneted to {server}");
                    return true;
                }
                catch (Exception e)
                {
                    connectToWorld = false;
                    updateFunc(false, $"Can not conneted to {server} with error: {e.Message}.");
                    return false;
                }
            };
            // Wait(); or .Result will block the program.
            Task.Run(() => { return innerRun(); });
        }

        public static string DonationUrl { 
            get {
                if (!connectToWorld && CultureInfo.CurrentUICulture.Equals(CultureInfo.GetCultureInfo("zh-CN"))) {
                    return donationUrlCHN;
                } else
                {
                    return donationUrlGlobal;
                }
            } 
        }
        
        public static string OfficalWebSite { get {
                if (!connectToWorld && CultureInfo.CurrentUICulture.Equals(CultureInfo.GetCultureInfo("zh-CN")))
                {
                    return officalWebSiteCHN;
                }
                else
                {
                    return officalWebSiteGlobal;
                }
            } 
        }
        public static string NewIssue
        {
            get
            {
                if (!connectToWorld && CultureInfo.CurrentUICulture.Equals(CultureInfo.GetCultureInfo("zh-CN")))
                {
                    return newIssueCHN;
                }
                else
                {
                    return newIssueGlobal;
                }
            }
        }
    }
}
