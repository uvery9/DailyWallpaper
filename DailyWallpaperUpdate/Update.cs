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
using Microsoft.WindowsAPICodePack.Dialogs;

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
                readOnlyCheckBox.Checked = true;
                SetReadOnly(true);
            }
        }

        private List<string> ParseXml()
        {
            var xmlFile = Assembly.GetExecutingAssembly().GetName().Name + ".exe.xml";
            var xmlPath = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), xmlFile);
            if (!File.Exists(xmlPath))
                return new List<string>() { "DailyWallpaper.exe", "DailyWallpaper.Protable-latest.zip", "Some Path" };
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
            var unzipPath = unzipPathTextBox.Text;
            if (!File.Exists(zipFile))
                return;
            var target = targetTextBox.Text;
            var targetPath = Path.Combine(unzipPath, target);
            if (File.Exists(targetPath))
            {
                AppendText("Try to kill process: " + target);
                var targetWithoutExt = Path.GetFileNameWithoutExtension(target).ToLower();
                foreach (Process proc in Process.GetProcesses())
                {
                    if (targetWithoutExt.Equals(proc.ProcessName.ToLower()))
                    {
                        AppendText("Kill: " + proc.ProcessName);
                        proc.Kill();
                        break;
                    }
                }
            }

            AppendText($"Unzip { new FileInfo(zipFile).Name} to:\r\n    { unzipPath}");

            UnzipToPath(zipFile, unzipPath);

            AppendText($"restart {target} ...");
            if (File.Exists(targetPath))
            {
                Process.Start(targetPath);
                AppendText($"restarted :\r\n    {targetPath}");
            }
        }

        private void SetReadOnly(bool ro = false)
        {
            targetTextBox.ReadOnly = ro;
            zipFileTextBox.ReadOnly = ro;
            unzipPathTextBox.ReadOnly = ro;
            zipFileButton.Enabled = !ro;
            unzipPathButton.Enabled = !ro;
        }

        private void readOnlyCheckBox_Click(object sender, EventArgs e)
        {
            if (readOnlyCheckBox.Checked)
            {
                SetReadOnly(true);
            }
            else
            {
                SetReadOnly(false);
            }
        }

        private void zipFileButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                string zipDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                foreach (var file in Directory.EnumerateFiles(Path.GetDirectoryName(Assembly.
                    GetExecutingAssembly().Location)))
                {
                    if (file.ToLower().EndsWith(".zip"))
                    {
                        zipDir = new FileInfo(file).DirectoryName;
                        break;
                    }
                }
                dialog.InitialDirectory = zipDir;
                dialog.IsFolderPicker = false;
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Select zip file"; // "XML files (*.xml)|*.xml";
                dialog.Filters.Add(new CommonFileDialogFilter("zip file", "*.zip"));
                // maybe add some log

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    zipFileTextBox.Text = dialog.FileName;
                }
            }
        }

        private void unzipPathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                dialog.InitialDirectory = Path.GetDirectoryName(Assembly.
                    GetExecutingAssembly().Location);
                dialog.IsFolderPicker = true;
                dialog.Multiselect = false;
                dialog.Title = "Select Unzip folder"; 

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    unzipPathTextBox.Text = dialog.FileName;
                }
            }
        }
    }
}
