using System;
using System.Collections.Generic;
using System.Linq;
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
            string mutexName = System.Reflection.Assembly.GetExecutingAssembly().GetType().GUID.ToString();
            // Use the assembly GUID as the name of the mutex which we use to detect if an application instance is already running
            using (System.Threading.Mutex mutex = new System.Threading.Mutex(false, mutexName, out bool createdNew))
            {
                if (!createdNew)
                {
                    // MessageBox.Show("Only allow one instance", "Tips", MessageBoxButtons.OK);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                try
                {
                    Application.Run(new ViewManager());
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message, "Error");
                }
            }
        }
        
    }
}
