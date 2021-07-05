using System;
using System.Diagnostics;
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
        private static string officalLatestGlobal = "https://github.com/JaredDC/DailyWallpaper/releases/latest";
        private static string newIssueCHN = "https://gitee.com/imtvip/DailyWallpaper/issues/new";
        private static string newIssueGlobal = "https://github.com/JaredDC/DailyWallpaper/issues/new";
        private static string donationUrlGlobal   = "https://github.com/JaredDC/DailyWallpaper/tree/master/Donate";
        private static string donationUrlCHN = "https://gitee.com/imtvip/DailyWallpaper/tree/master/Donate";
       /* private static Color backColor = 
            Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(243)))), ((int)(((byte)(214)))));*/

        public static void TestConnect(Action<bool, string> updateFunc, string server = "https://www.google.com", int port = 80, bool useProxy = true)
        {
            if (useProxy)
            {
                TestConnectUsingProxy(updateFunc, server);
            }
            else
            {
                TestConnectUsingSocket(updateFunc, server, port);
            }
        }

        private static void TestConnectUsingProxy(Action<bool, string> updateFunc, string server = "https://www.google.com")
        {
            bool innerRun()
            {
                
                try
                {
                    var webReq = (HttpWebRequest)WebRequest.Create(server);
                    webReq.Timeout = 3000;
                    // MessageBox.Show(string.Format("Proxy: {0}", webReq.Proxy.GetProxy(webReq.RequestUri)));
                    // webReq.Proxy = proxy;  USE SYSTEM PROXY
                    // new WebProxy(Global.Loopback, Global.httpPort);
                    var timer = new Stopwatch();
                    timer.Start();
                    string msg = null;
                    using (var webReqResp = (HttpWebResponse)webReq.GetResponse())
                    {
                        if (webReqResp.StatusCode != HttpStatusCode.OK
                        && webReqResp.StatusCode != HttpStatusCode.NoContent)
                        {
                            msg = ", descri: ";
                            msg += webReqResp.StatusDescription;
                        }
                    }
                    timer.Stop();
                    var responseTime = timer.Elapsed.Milliseconds;
                    connectToWorld = true;
                    string show = msg ?? ""; // string show = string.IsNullOrEmpty(msg) ? "" : msg;
                    updateFunc(true, $"Conneted to {server}, response: {responseTime} ms{show}");
                    return true;
                }
                catch (Exception e)
                {
                    connectToWorld = false;
                    updateFunc(false, $"Can not conneted to {server} with error: {e.Message}.");
                    return false;
                }
            };
            Task.Run(() => { return innerRun(); });
        }

        /// <summary>
        /// return result will block the program.
        /// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.socket?view=net-5.0
        /// https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task.run?view=net-5.0
        /// NEED TO FIX: WHEN SOMEBODY USE VPN/SOMETHING, (S)HE CAN VISIT THR WEBSIT(SERVER), BUT THIS METHOD CAN'T.
        /// </summary>
        private static void TestConnectUsingSocket(Action<bool, string> updateFunc, string server = "https://www.google.com", int port = 80)
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
            Task.Run(() => { return innerRun(); });
            /* Wait(); or .Result will block the program.
             *   task.Result is accessing the property's get accessor blocks the calling thread 
             *  until the asynchronous operation is complete; it is equivalent to calling the 
             *  Wait method. Once the result of an operation is available, it is stored and 
             *  is returned immediately on subsequent calls to the Result property. Note that, 
             *  if an exception occurred during the operation of the task, or if the task 
             *  has been cancelled, the Result property does not return a value. Instead, 
             *  attempting to access the property value throws an AggregateException exception. 
             *  The only difference is that the await will not block. Instead, it will asynchronously 
             *  wait for the Task to complete and then resume
             */
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
        public static string OfficalLatest
        {
            get
            {
                if (!connectToWorld && CultureInfo.CurrentUICulture.Equals(CultureInfo.GetCultureInfo("zh-CN")))
                {
                    return officalLatestGlobal;
                }
                else
                {
                    return officalLatestGlobal;
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
