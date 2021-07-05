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
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    var result = MessageBox.Show("Only allow one instance. \r\nPress \"Cancel\": restart.", "Confirmation", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.Cancel)
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
                // OptionsView
                // new ViewManager()
                var opv = new OptionsView();
                Application.Run(opv);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "DailyWallpaper");
            }
        }
        
    }
}
