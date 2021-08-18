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
            AppendText("Found: " + xmlFile + " at :\r\n    " 
                + new FileInfo(xmlPath).DirectoryName);

            var doc = new XmlDocument();
            doc.Load(xmlPath);
            var li = new List<string>();
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                var text = node.InnerText;
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

        private void AppendText(string s)
        {
            updateConsTextBox.AppendText($"\r\n> {s}" + Environment.NewLine);
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            var zipFile = zipFileTextBox.Text;
            if (!File.Exists(zipFile))
                return;
            var target = targetTextBox.Text;
            AppendText("Try to kill process: " + target);
            foreach (Process proc in Process.GetProcesses())
            {
                if (target.ToLower().Contains(proc.ProcessName.ToLower()))
                {
                    AppendText("Kill: " + proc.ProcessName);
                    proc.Kill();
                    break;
                }
            }
            var unzipPath = unzipPathTextBox.Text;
            AppendText($"Unzip { new FileInfo(zipFile).Name} to:\r\n    { unzipPath}");

            UnzipToPath(zipFile, unzipPath);

            AppendText($"restart {target} ...");
            target = Path.Combine(unzipPath, target);
            if (File.Exists(target))
            {
                Process.Start(target);
                AppendText($"restarted :\r\n    {target}");
            }
                
        }
    }
}
