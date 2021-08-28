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
    // 2. fix grep utf-8

    public partial class GrepToolForm : Form
    {
        private CancellationTokenSource _source;
        private Process proc;
        private ConfigIni m_ini = ConfigIni.GetInstance();
        private bool clearable;
        private CancellationTokenSource cancelSrc;

        public GrepToolForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.gt32x32;
            argsTextBox.Text = "-I -i -r";
            // stringTextBox.Text = "SOMEWORD";
            var grepBin = m_ini.Read("GrepBin", "GrepTool");
            var targetFolder = m_ini.Read("TargetFolder", "GrepTool");
            var args = m_ini.Read("Args", "GrepTool");
            var string_ = m_ini.Read("String", "GrepTool");
            if (!string.IsNullOrEmpty(grepBin) && grepBin.ToLower().Contains("grep"))
                grepLocationTextBox.Text = grepBin;
            else
                FindGrepBin(@"C:\", @"bin\grep.exe");
            if (!string.IsNullOrEmpty(targetFolder))
                targetFolderTextBox.Text = targetFolder;
            if (!string.IsNullOrEmpty(args))
                argsTextBox.Text = args;
            if (!string.IsNullOrEmpty(string_))
                stringTextBox.Text = string_;
            clearable = true;
        }

        private void FindGrepBin(string path, string target)
        {
            if (!Directory.Exists(path))
                return;
            LogWithColor(consRichTextBox, Color.Purple, $"{DateTime.Now:> HH:mm:ss}: " + 
                $"Start finding all \"{target}\" in \"{path}\"");
            LogWithColor(consRichTextBox, Color.Purple, $"{DateTime.Now:> HH:mm:ss}: " +
                $"Please wait patiently, Button \"Locate\" to Cancel...");
            Task.Run(() =>
            {
                /*try
                {
                    var enumDirs = Directory.EnumerateDirectories(path, "*bin*", SearchOption.AllDirectories);
                    foreach (var item in enumDirs)
                    {
                        Debug.WriteLine("==> " + item);
                    }
                }
                catch { }*/
                cancelSrc = new CancellationTokenSource();
                var resList = Helpers.FindFile.FindIgnoreCaseAsync(path, target, cancelSrc.Token).Result;
                var cnt = resList.Count;
                if (cnt < 1)
                {
                    LogWithColor(consRichTextBox, Color.DarkOrange, $"{DateTime.Now:> HH:mm:ss}: No Found.");
                    return;
                }
                else if (cnt == 1)
                {
                    var grepBin = resList[0];
                    m_ini.UpdateIniItem("GrepBin", grepBin, "GrepTool");
                    grepLocationTextBox.Text = grepBin;
                }
                else
                {
                    foreach (var res in resList)
                    {
                        LogWithColor(consRichTextBox, Color.Purple, ">>>    " + res);
                    }
                }
                LogWithColor(consRichTextBox, Color.Purple, $"{DateTime.Now.ToString("> HH:mm:ss")}: End.");
            });
        }

        private void grepLocateButton_Click(object sender, EventArgs e)
        {
            if (cancelSrc != null)
            {
                cancelSrc.Cancel();
                LogWithColor(consRichTextBox, Color.DarkOrange, $"{DateTime.Now:> HH:mm:ss}: Cancel auto finding grep.exe.");
            }
                
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
                    m_ini.UpdateIniItem("GrepBin", dialog.FileName, "GrepTool");
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
            rtb.ScrollToCaret();
            rtb.SelectionColor = color;
            rtb.AppendText(text);
            rtb.AppendText(Environment.NewLine);
        }

        private void Finished(Color c, string msg)
        {
            LogWithColor(consRichTextBox, c,
                    msg);
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
                m_ini.UpdateIniItem("TargetFolder", targetFolder, "GrepTool");
                m_ini.UpdateIniItem("Args", argsTextBox.Text, "GrepTool");
                m_ini.UpdateIniItem("String", stringTextBox.Text, "GrepTool");
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
            if (clearable)
            {
                stringTextBox.Clear();
                targetFolderTextBox.Clear();
                clearable = false; // first
            }
                
            if (_source != null)
            {
                _source.Cancel();
            }
            if (proc!= null && !proc.HasExited)
                proc.Kill();
            else
                consRichTextBox.Clear();

        }

        private void grepLocationTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (sender is TextBox box)
                {
                    var path = box.Text.Trim();
                    if (File.Exists(path))
                    {
                        m_ini.UpdateIniItem("GrepBin", path, "GrepTool");
                        LogWithColor(consRichTextBox, Color.Black, $"Update GrepBin = \r\n    {path}");
                    }
                        
                }
            }
        }
    }
}
