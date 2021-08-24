using Microsoft.WindowsAPICodePack.Dialogs;
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

namespace DailyWallpaper.Tools
{
    public partial class GrepToolForm : Form
    {
        public GrepToolForm()
        {
            InitializeComponent();
            grepLocationTextBox.Text = 
                @"C:\Program Files\Git\usr\bin\grep.exe";
            argsTextBox.Text = "-I -i -r";
            // stringTextBox.Text = "SOMEWORD";
        }

        private void grepLocationButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                string grepDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (File.Exists(grepLocationTextBox.Text))
                    grepDir = new FileInfo(grepLocationTextBox.Text).DirectoryName;
                dialog.InitialDirectory = grepDir;
                dialog.IsFolderPicker = false;
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Select grep binary";
                dialog.Filters.Add(new CommonFileDialogFilter("exe file", "*.exe"));
                dialog.Filters.Add(new CommonFileDialogFilter("All file", "*.*"));

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    grepLocationTextBox.Text = dialog.FileName;
                }
            }
        }

        private void targetFolderBrowserButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog())
            {
                string targetDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (Directory.Exists(targetFolderTextBox.Text))
                    targetDir = Path.GetFullPath(targetFolderTextBox.Text);
                dialog.InitialDirectory = targetDir;
                dialog.IsFolderPicker = true;
                dialog.EnsureFileExists = true;
                dialog.Multiselect = false;
                dialog.Title = "Select grep target folder";

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {
                    targetFolderTextBox.Text = dialog.FileName;
                }
            }

        }

        private void LogWithColor(RichTextBox rtb, Color color, string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (InvokeRequired)
            {
                Invoke(new Action(() => LogWithColor(rtb, color, text)));
                return;
            }
            rtb.SelectionColor = color;
            rtb.AppendText(text);
            rtb.AppendText(Environment.NewLine);
        }

        
        private void runButton_Click(object sender, EventArgs e)
        {
            LogWithColor(consRichTextBox, Color.Green, "Start...");
            LogWithColor(consRichTextBox, Color.Blue, "Usage: $GREP -I -i -r \"SOMEWORD\" $TARGETFOLDER");
            var grepBin = grepLocationTextBox.Text;
            var targetFolder = targetFolderTextBox.Text;
            if (!File.Exists(grepBin))
            {
                LogWithColor(consRichTextBox, Color.Red, 
                    $"grep.exe must exist:\r\n    {grepBin}\r\nFinished with error...");
                return;
            }
            grepBin = Path.GetFullPath(grepBin);
            string arguments = " --help";
            bool help = true;
            if (Directory.Exists(targetFolder) && !string.IsNullOrEmpty(stringTextBox.Text))
            {
                arguments = " " + argsTextBox.Text + $" \"{stringTextBox.Text}\" " + $"\"{targetFolder}\"";
                targetFolder = Path.GetFullPath(targetFolder);
                help = false;
            }
            else
            {
                if (!Directory.Exists(targetFolder))
                {
                    LogWithColor(consRichTextBox, Color.Red, "Error: You have to specify $TARGETFOLDER");
                    targetFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    targetFolderTextBox.Text = "$TARGETFOLDER";
                }
                if (string.IsNullOrEmpty(stringTextBox.Text))
                {
                    LogWithColor(consRichTextBox, Color.Red, "Error: You have to specify $STRING");
                    stringTextBox.Text = "$STRING";
                }
            }
            LogWithColor(consRichTextBox, Color.Blue,
                Environment.NewLine + $"\"{grepBin}\"" + arguments);
            consRichTextBox.ScrollToCaret();
            Task.Run(() =>
            {
                try
                {
                    using (var p = new Process())
                    {
                        p.StartInfo = new ProcessStartInfo(grepBin)
                        {
                            WorkingDirectory = targetFolder,
                            Arguments = arguments,
                            RedirectStandardError = true,
                            RedirectStandardOutput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };
                        bool line = false;
                        p.OutputDataReceived += (_, data) =>
                        {
                            var printStr = data.Data;
                            if (!help) {
                                line = !line;
                                // printStr.Replace(targetFolder, "");
                                printStr += Environment.NewLine;
                            }
                            if (!line)
                                LogWithColor(consRichTextBox, Color.Black, printStr);
                            else
                                LogWithColor(consRichTextBox, Color.Brown, printStr);
                        };
                        p.ErrorDataReceived += (_, data) =>
                        {
                            var printStr = data.Data;
                            if (!help)
                                printStr += Environment.NewLine;
                            LogWithColor(consRichTextBox, Color.Red, printStr);
                        };
                        p.EnableRaisingEvents = true;
                        p.Start();
                        p.BeginOutputReadLine();
                        p.BeginErrorReadLine();
                        p.WaitForExit();
                    }
                    LogWithColor(consRichTextBox, Color.Green, "Finished...");
                }
                catch (Exception ex)
                {

                    // throw;
                    LogWithColor(consRichTextBox, Color.Red, ex.ToString());
                    LogWithColor(consRichTextBox, Color.Red, "Finished with error...");
                }
                finally
                {
                    LogWithColor(consRichTextBox, Color.Black,
                    "-------------------------------------------------------");
                }
                    
            });

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            consRichTextBox.Clear();
            if (stringTextBox.Text.Contains("$"))
                stringTextBox.Clear();
            if (targetFolderTextBox.Text.Contains("$"))
                targetFolderTextBox.Clear();
        }


    }
}
