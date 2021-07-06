using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.View
{
    
    public partial class ConsWindow : Form
    {
        public TextWriter textWriter = null;

        private static ConsWindow _instance;
        public static ConsWindow GetInstance(Icon icon)
        {
            return _instance ?? (_instance = new ConsWindow(icon));
        }

        public ConsWindow ()
        {
            InitializeComponent();
        }

        private ConsWindow(Icon icon)
        {
            InitializeComponent();
            this.Icon = icon;
            textWriter = new TextBoxStreamWriter(textBoxCons);

        }

        private void LogWindow_Load(object sender, EventArgs e)
        {

        }
    }
    internal class TextBoxStreamWriter : TextWriter
    {
        TextBox _output = null;

        public TextBoxStreamWriter(TextBox output)
        {
            _output = output;
        }

        public override void Write(char value)
        {
            base.Write(value);
            _output.AppendText(value.ToString());

        }

        public override Encoding Encoding
        {
            get { return System.Text.Encoding.Default; }
        }
    }
}
