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
using System.IO.Compression;

namespace DailyWallpaperUpdate
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent();
            Icon = Properties.Resources.Update;
            var li = ParseXml();
            if (li.Count == 3)
            {
                targetTextBox.Text = li[0];
                zipFileTextBox.Text = li[1];
                unzipPathTextBox.Text = li[2];
                if (File.Exists(zipFileTextBox.Text))
                {
                    readOnlyCheckBox.Checked = true;
                    SetReadOnly(true);
                }
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

        private void UnzipToPath(string zipPath, string extractPath)
        {
            try
            {
                // Normalizes the path.
                extractPath = Path.GetFullPath(extractPath);
                if (!Directory.Exists(extractPath))
                    Directory.CreateDirectory(extractPath);
                // ZipFile.ExtractToDirectory(zipPath, extractPath); can not overwrite.
                
                // Ensures that the last character on the extraction path
                // is the directory separator char.
                // Without this, a malicious zip file could try to traverse outside of the expected
                // extraction path.
                if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                    extractPath += Path.DirectorySeparatorChar;

                using (var archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (var file in archive.Entries)
                    {
                        // if (file.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))

                        // Gets the full path to ensure that relative segments are removed.
                        updateConsTextBox.AppendText($". {file.Name}" + Environment.NewLine);
                        string dest = Path.GetFullPath(Path.Combine(extractPath, file.FullName));
                        if (File.Exists(dest))
                            File.Delete(dest);

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (dest.StartsWith(extractPath, StringComparison.Ordinal))
                            file.ExtractToFile(dest);
                    }
                }
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
            var zipPath = zipFileTextBox.Text;
            var extractPath = unzipPathTextBox.Text;
            if (!File.Exists(zipPath))
                return;
            var target = targetTextBox.Text;
            var targetPath = Path.Combine(extractPath, target);
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

            AppendText($"Unzip { new FileInfo(zipPath).Name} to:\r\n    { extractPath}\r\n");

            UnzipToPath(zipPath, extractPath);

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
