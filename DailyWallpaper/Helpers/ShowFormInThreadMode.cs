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
                // MessageBox.Show("STA Failed", ex.Message);
            }
            mySTAThread.Start();

        }

        public static void ShowHashCalcForm()
        {
            MessageBox.Show(Thread.CurrentThread.ManagedThreadId.ToString());
            var myForm = new HashCalcForm();
            myForm.ShowDialog();
            // myForm.BringToFront();
        }
    }
}
