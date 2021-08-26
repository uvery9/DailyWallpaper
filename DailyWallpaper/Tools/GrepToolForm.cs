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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.Tools
{
    // TODO:
    // 1. find grep.exe, show url and return if no found.
    // 2. remember folder, self defined grep.exe, args, string.
    // 3. fix grep utf-8
    public partial class GrepToolForm : Form
    {
        private CancellationTokenSource _source;
        private Process proc;
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

        private void Finished(Color c, string msg)
        {
            LogWithColor(consRichTextBox, c,
                    msg);
            consRichTextBox.ScrollToCaret();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            LogWithColor(consRichTextBox, Color.Green, "Start...");
            LogWithColor(consRichTextBox, Color.Blue, "Usage: $GREP -I -i -r \"SOMEWORD\" $TARGETFOLDER");
            var grepBin = grepLocationTextBox.Text;
            var targetFolder = targetFolderTextBox.Text;
            if (!File.Exists(grepBin))
            {
                Finished(Color.Red, $"grep.exe must exist:\r\n    {grepBin}\r\nFinished with error...");
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
            _source = new CancellationTokenSource();
            Task.Run(() =>
            {
                try
                {
                    proc = new Process();
                    proc.StartInfo = new ProcessStartInfo(grepBin)
                    {
                        WorkingDirectory = targetFolder,
                        Arguments = arguments,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    };
                    bool line = false;
                    proc.OutputDataReceived += (_, data) =>
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
                    proc.ErrorDataReceived += (_, data) =>
                    {
                        var printStr = data.Data;
                        if (!help)
                            printStr += Environment.NewLine;
                        LogWithColor(consRichTextBox, Color.Red, printStr);
                    };
                    proc.EnableRaisingEvents = true;
                    proc.Start();
                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();
                    proc.WaitForExit();
                    if (_source.Token.IsCancellationRequested) // Not Work
                    {
                        proc.Kill();
                        _source.Token.ThrowIfCancellationRequested();
                    }                        
                    Finished(Color.Green, "Finished...");
                }
                catch (InvalidOperationException ex) {
                    if (!ex.ToString().Contains("Process.Kill"))
                        throw;
                    Finished(Color.DarkOrange, "Finished with cancel...");
                    // Finished(Color.DarkOrange, ex.Message);
                }
                catch (Exception ex)
                {
                    // kill grep process
                    LogWithColor(consRichTextBox, Color.Red, ex.ToString());
                    Finished(Color.Red, "Finished with error...");
                }
                finally
                {
                    LogWithColor(consRichTextBox, Color.Black,
                    "-------------------------------------------------------");
                }
                    
            }, _source.Token);

        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            if (stringTextBox.Text.Contains("$"))
                stringTextBox.Clear();
            if (targetFolderTextBox.Text.Contains("$"))
                targetFolderTextBox.Clear();
            if (_source != null)
            {
                _source.Cancel();
            }
            if (proc!= null && !proc.HasExited)
                proc.Kill();
            else
                consRichTextBox.Clear();

        }


    }
}
