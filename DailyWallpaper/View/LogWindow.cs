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
    
    public partial class LogWindow : Form
    {
        public TextWriter _writer = null;

        private static LogWindow _instance;
        public static LogWindow GetInstance(Icon icon)
        {
            return _instance ?? (_instance = new LogWindow(icon));
        }

        public LogWindow ()
        {
            InitializeComponent();
        }

        private LogWindow(Icon icon)
        {
            InitializeComponent();
            this.Icon = icon;
            _writer = new TextBoxStreamWriter(textBox1);

        }

        private void LogWindow_Load(object sender, EventArgs e)
        {

        }
        public void SetConsToTextBox(bool set = true)
        {
            if (set)
            {
                Console.Out.Flush();
                Console.Error.Flush();
                textBox1.Text = "";
                Console.SetOut(_writer);
                Console.SetError(_writer);
            }
            else
            {
                _writer.Flush();
                Console.Out.Close();
                Console.Error.Close();
                
                // redirect stderr to default.
                var standardError = new StreamWriter(Console.OpenStandardError());
                standardError.AutoFlush = true;
                Console.SetError(standardError);

                // redirect stdout to default.
                var standardOutput = new StreamWriter(Console.OpenStandardOutput());
                standardOutput.AutoFlush = true;
                Console.SetOut(standardOutput);
            }
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
