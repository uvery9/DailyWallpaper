using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.Tools
{
    public partial class VideoEditorForm : Form
    {
        private TextBoxCons _console;
        private readonly string helpStr = "ffmpeg";
        public VideoEditorForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.Ve32x32;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            

            _console = new TextBoxCons(new ConsWriter(videoEditorConsTextBox));
            _console.WriteLine(helpStr);
            // _console.WriteLine($"CurrentThread ID: {Thread.CurrentThread.ManagedThreadId}");
            
        }
    }
}
