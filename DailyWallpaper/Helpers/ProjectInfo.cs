using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace DailyWallpaper.Helpers
{
    class ProjectInfo
    {
        public static bool connectToWorld = false;
        public static string author = "Jared DC";
        public static string email = "jared.dcx@gmail.com";
        public static string exeName = Assembly.GetExecutingAssembly().GetName().Name;
        public static string executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        public static string logFile = new FileInfo(exeName + ".log.txt").FullName;
        private static string officalWebSiteGlobal = "https://github.com/JaredDC/DailyWallpaper";
        private static string officalWebSiteCHN = "https://gitee.com/imtvip/DailyWallpaper";
        private static string officalLatestGlobal = "https://github.com/JaredDC/DailyWallpaper/releases/latest";
        private static string newIssueCHN = "https://gitee.com/imtvip/DailyWallpaper/issues/new";
        private static string newIssueGlobal = "https://github.com/JaredDC/DailyWallpaper/issues/new";
        private static string donationUrlGlobal   = "https://github.com/JaredDC/DailyWallpaper/tree/master/Donate";
        private static string donationUrlCHN = "https://gitee.com/imtvip/DailyWallpaper/tree/master/Donate";
        private static string gitHubAPI = "https://api.github.com/repos/JaredDC/DailyWallpaper/releases/latest";
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
                } 
                else
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
        
        public static Version GetVerSion()
        {
            try
            {
                var file = Path.Combine(Path.GetDirectoryName(
                                            Assembly.GetExecutingAssembly().Location), "GIT-VERSION");
                if (File.Exists(file))
                {
                    using (var fs = new StreamReader(file))
                    {
                        var input = fs.ReadLine();
                        var stripped = Regex.Replace(input, @"[^0-9\.]", "");
                        return new Version(stripped);
                    }
                }
                return new Version("1.5.0.2");
            }
            catch
            {
                return new Version("1.4.0.4");
            }
        }
        private static Version StringOrFile2Version(string input)
        {
            var stripped = Regex.Replace(input, @"[^0-9\.]", "");
            return new Version(stripped);
        }
        public static void CheckForUpdates(Action<bool, bool, string> action, bool force = false)
        {
            _ = Task.Run(() =>
              {
                  try
                  {
                      var json = DownloadJson(gitHubAPI);
                      string tag_name = (string)json["tag_name"];
                      if (string.IsNullOrEmpty(tag_name))
                      {
                          action(false, false, "No tag_name");
                          return;
                      }
                      if (force)
                      {
                          var zipUrl = (string)json["assets"][2]["browser_download_url"];
                          if (string.IsNullOrEmpty(zipUrl))
                          {
                              action(false, false, "No zipUrl");
                              return;
                          }
                          DownloadFileAsync(zipUrl, tag_name, action);
                          return;
                      }
                      else
                      {
                          var gitHubVersion = StringOrFile2Version(tag_name);
                          var currentVersion = GetVerSion();
                          if (currentVersion < gitHubVersion)
                          {
                              var msiUrl = (string)json["assets"][0]["browser_download_url"];
                              if (string.IsNullOrEmpty(msiUrl))
                              {
                                  action(false, false, "No msiUrl");
                                  return;
                              }
                              DownloadFileAsync(msiUrl, tag_name, action);
                              return;
                          }
                          else
                          {
                              action(true, false, "No update.");
                              return;
                          }
                      }
                      
                  }
                  catch (Exception e)
                  {
                      action(false, false, "CheckForUpdates Exception: " + e.Message);
                  }
              });
        }
        static void ws_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {

                var wx = (WebClientEx)sender;
                if (e.Cancelled)
                {
                    wx.action(false, false, "User Cancelled.");
                    ((WebClientEx)sender).Dispose();
                    return;
                }
                if (e.Error == null)
                {
                    // Clean old .completed txt
                    var completedDir = Path.GetDirectoryName(wx.completed);
                    foreach (var completedTxt in Directory.EnumerateFiles(completedDir, "*.completed", SearchOption.TopDirectoryOnly))
                    {
                        File.Delete(Path.Combine(completedDir, completedTxt));
                    }
                    using (FileStream fs = File.Create(wx.completed))
                    {
                        byte[] info = new UTF8Encoding(true).GetBytes($"Downloaded {wx.file} at {DateTime.Now}.");
                        fs.Write(info, 0, info.Length);
                    }
                    // downloaded, return file.
                    wx.action(true, true, wx.file);
                }
                else
                {
                    wx.action(false, false, $"{e.Error}");
                }  
        }

        public static void DownloadFileAsync(string url, string tagName, Action<bool, bool, string> action, WebProxy webProxy = null)
        {
            WebClientEx ws = new WebClientEx();
            
            var gitHubFileName = "DailyWallpaper.Installer-latest.msi";
            if (url.ToLower().EndsWith(".zip"))
                gitHubFileName = "DailyWallpaper.Protable-latest.zip";
            var savePath = Path.Combine(Path.GetDirectoryName(
                            Assembly.GetExecutingAssembly().Location), gitHubFileName);
            ws.completed = savePath + "." + tagName + ".completed";
            ws.file = savePath;
            ws.action = action;
            if (File.Exists(ws.completed))
            {
                action(false, true, ws.file);
                return;
            }
            if (webProxy != null)
            {
                ws.Proxy = webProxy;// new WebProxy(Global.Loopback, Global.httpPort);
            }
            ws.DownloadFileCompleted += ws_DownloadFileCompleted;
            ws.DownloadFileAsync(new Uri(url), savePath);
/*            }
            catch
            {

            }*/
            /*catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
            }*/
        }

        private static dynamic DownloadJson(string url)
        {
            var web = new WebClientEx(15 * 1000)
            {
                Proxy = WebRequest.DefaultWebProxy,
                Credentials = CredentialCache.DefaultCredentials
            };
            web.Headers.Add(HttpRequestHeader.UserAgent, "Wget/1.9.1");

            var response =
                web.DownloadDataStream(url);

            var json = JsonConvert.DeserializeObject<dynamic>(new StreamReader(response).ReadToEnd());
            return json;
        }
    }
    internal class WebClientEx : WebClient
    {
        public string completed; 
        public string file;
        public Action<bool, bool, string> action;
        public WebClientEx() : this(60 * 1000)
        {
        }

        public WebClientEx(int timeout)
        {
            Timeout = timeout;
        }

        public int Timeout { get; set; }

        /*protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address);

            request.Timeout = Timeout;

            return request;
        }*/

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request;
            request = (HttpWebRequest)base.GetWebRequest(address);
            request.Timeout = Timeout;
            request.ReadWriteTimeout = Timeout;
            //request.AllowAutoRedirect = false;
            //request.AllowWriteStreamBuffering = true;

            request.ServicePoint.BindIPEndPointDelegate = (servicePoint, remoteEndPoint, retryCount) =>
            {
                if (remoteEndPoint.AddressFamily == AddressFamily.InterNetworkV6)
                    return new IPEndPoint(IPAddress.IPv6Any, 0);
                else
                    return new IPEndPoint(IPAddress.Any, 0);
            };

            return request;
        }

        public MemoryStream DownloadDataStream(string address)
        {
            var buffer = DownloadData(address);

            return new MemoryStream(buffer);
        }

        public MemoryStream DownloadDataStream(Uri address)
        {
            var buffer = DownloadData(address);

            return new MemoryStream(buffer);
        }
    }
}
