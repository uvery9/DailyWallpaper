using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DailyWallpaper
{
    class CEFTextWriter : TextWriter
    {
        private IEnumerable<TextWriter> writers;
        public CEFTextWriter(IEnumerable<TextWriter> writers)
        {
            this.writers = writers.ToList();
        }
        public CEFTextWriter(params TextWriter[] writers)
        {
            this.writers = writers;
        }

        public override void Write(char value)
        {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Write(string value)
        {
            foreach (var writer in writers)
                writer.Write(value);
        }

        public override void Flush()
        {
            foreach (var writer in writers)
                writer.Flush();
        }

        public override void Close()
        {
            foreach (var writer in writers)
                writer.Close();
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }
    public class ControlWriter : TextWriter
    {
        private TextBox textbox;
        public ControlWriter(TextBox textbox)
        {
            this.textbox = textbox;
        }

        public override void Write(char value)
        {
            string s = "";
            s += value;
            textbox.AppendText(s);
        }

        public override void Write(string value)
        {
            textbox.AppendText(value);
        }

        public override void WriteLine(string value)
        {
            // two long cause problem, maybe.
            if (textbox.Text.Length > 10000)
            {
                var tmp = textbox.Text;
                textbox.Text = tmp.Substring(8000, tmp.Length);
            }
            textbox.AppendText(value + NewLine);

            textbox.Update();
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }
}
