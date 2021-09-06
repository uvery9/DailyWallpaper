using DailyWallpaper.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper
{

    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        [STAThread]
        static void Main()
        {
            string mutexName = System.Reflection.Assembly.GetExecutingAssembly()
                .GetType().GUID
                .ToString();
            // Use the assembly GUID as the name of the mutex which we use to detect if an application instance is already running
            using (Mutex mutex = new Mutex(false, mutexName, out bool createdNew))
            {
                if (!createdNew)
                {

                    var locale = System.Globalization.CultureInfo.CurrentUICulture;
                    // private static readonly CultureInfo CurrentCultureInfo = CultureInfo.GetCultureInfo("zh-CN");
                    // private static readonly CultureInfo CurrentCultureInfo = CultureInfo.GetCultureInfo("en");
                    var buttonOK = MessageBoxButtons.OK.ToString();
                    if (locale.ToString().ToLower().Contains("zh-cn"))
                        buttonOK = "确定";
                    var name = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
                    var result = MessageBox.Show($"Only allow one instance. \r\nPress \"{buttonOK}\" to restart.",
                        $"{name}", MessageBoxButtons.OKCancel); // Confirmation
                    if (result == DialogResult.OK)
                    {
                        foreach (Process proc in Process.GetProcesses())
                        {
                            if (proc.ProcessName.Equals(Process.GetCurrentProcess().ProcessName) 
                                && proc.Id != Process.GetCurrentProcess().Id)
                            {
                                proc.Kill();
                                break;
                            }
                        }
                        // Wait for process to close
                        Thread.Sleep(1000);
                        // MessageBox.Show("RESTART...");
                        RunDailyWallpaper();
                        return; // MUST RETURN.
                    }
                    else
                    {
                        return;
                    }
                }
                RunDailyWallpaper();
            }
        }

        static void RunDailyWallpaper()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            try
            {
                var opv = new TrayView();
                Application.Run(opv);
                // opv.firstInit = false;
            }
            catch (Exception exc)
            {
                MessageBox.Show("Program.cs: " + exc, "DailyWallpaper");
            }
        }
        
    }
}
