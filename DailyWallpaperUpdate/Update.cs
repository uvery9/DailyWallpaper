using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace DailyWallpaperUpdate
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent(); 
            var li = ParseXml();
            if (li.Count == 3)
            {
                targetTextBox.Text = li[0];
                zipFileTextBox.Text = li[1];
                unzipPathTextBox.Text = li[2];
            }
        }

        private List<string> ParseXml()
        {
            var xmlFile = Assembly.GetExecutingAssembly().GetName().Name + ".exe.xml";
            var xmlPath = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), xmlFile);
            if (!File.Exists(xmlPath))
                return new List<string>() { "DailyWallpaper.exe", "DailyWallpaper.Protable-latest.zip", "Some Path"};
            updateConsTextBox.AppendText("> Found: " + xmlFile + " at :"+ Environment.NewLine 
                + "    "+ new FileInfo(xmlPath).DirectoryName + Environment.NewLine);
            var doc = new XmlDocument();
            doc.Load(xmlPath);
            var li = new List<string>();
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                var text = node.InnerText;
                // updateConsTextBox.AppendText(text + Environment.NewLine);
                li.Add(text);
            }
            return li;
        }

        private void UnzipToPath(string zipFile, string unzipPath)
        {
            try
            {
                if (!Directory.Exists(unzipPath))
                    Directory.CreateDirectory(unzipPath);
                // unzip zip to path
            }
            catch (UnauthorizedAccessException)
            { }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            var zipFile = zipFileTextBox.Text;
            if (!File.Exists(zipFile))
                return;
            var target = targetTextBox.Text;
            updateConsTextBox.AppendText("\r\n> Try to kill process: " + target + Environment.NewLine);
            foreach (Process proc in Process.GetProcesses())
            {
                if (target.ToLower().Contains(proc.ProcessName.ToLower()))
                {
                    // updateConsTextBox.AppendText("> Kill: " + targetTextBox.Text + Environment.NewLine);
                    updateConsTextBox.AppendText("\r\n> Kill: " + proc.ProcessName + Environment.NewLine);
                    proc.Kill();
                    break;
                }
            }
            var unzipPath = unzipPathTextBox.Text;
            updateConsTextBox.AppendText($"\r\n> Unzip {new FileInfo(zipFile).Name} to:\r\n    {unzipPath}" + Environment.NewLine);
            UnzipToPath(zipFile, unzipPath);
        }
    }
}
