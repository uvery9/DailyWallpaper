using DailyWallpaper.HashCalc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// https://docs.microsoft.com/en-us/dotnet/desktop/winforms/controls/how-to-make-thread-safe-calls-to-windows-forms-controls?view=netframeworkdesktop-4.8
/// </summary>
namespace DailyWallpaper.Helpers
{
    class ShowFormInThreadMode
    {
        public delegate void ShowElement();
        public void ShowForm(ShowElement ui)
        {
            var mySTAThread = new Thread(new ThreadStart(ui));
            try
            {
                mySTAThread.SetApartmentState(ApartmentState.STA);
            }
            catch (ThreadStateException ex)
            {
                MessageBox.Show("STA Failed", ex.Message);
            }
            mySTAThread.Start();

        }

        public static void ShowHashCalcForm()
        {
            // MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
            var hashCalcForm = new HashCalcForm();
            hashCalcForm.ShowDialog();
            // myForm.BringToFront();
        }

        /*
         * 
         * new Thread(new ThreadStart(ShowLoaderForm)).Start();
         *
         *   public void ShowLoaderForm() {
         *   new LoaderForm().Show();
         *   }
         * 
         * Two problems with this:
        * 
        * one: Form.Show() doesn't block, so the thread continues 
        *         to the end ot the method and exits, taking the form with it.
        * two: You would run into issues with cross threading, and the
        *         main thread, which is where events will be raised 
        *         (such as repaint) would be busy, so there would 
        *         be no refreshing of your form.
        * There are ways of doing a "splash" screen that are 
        * documented on code project and MSDN (Creating a Splash Screen). 
        * It's not that hard to fine some info about the accepted way to do splash screens.
         */
    }
}
