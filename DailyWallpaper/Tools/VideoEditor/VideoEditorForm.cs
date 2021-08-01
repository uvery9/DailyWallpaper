using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.Tools
{
    public partial class VideoEditorForm : Form
    {
        private TextBoxCons _console;
        private readonly string helpStr = "ffmpeg";
        private string ffmpeg = null;
        public VideoEditorForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.Ve32x32;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            

            _console = new TextBoxCons(new ConsWriter(videoEditorConsTextBox));
            _console.WriteLine(helpStr);
            FindFFmpegExe();
            // _console.WriteLine($"CurrentThread ID: {Thread.CurrentThread.ManagedThreadId}");

        }


        private void FindFFmpegExe()
        {
            try
            {
                var _ini = ConfigIni.GetInstance();
                var ffmpegPathIni = _ini.Read("FFMPEGPATH", "Local");
                void FindFFmpegAsync(string ffmpegPathAsync)
                {
                    if (!string.IsNullOrEmpty(ffmpegPathAsync) && File.Exists(ffmpegPathAsync))
                    {
                        /*var p = new Process();
                        p.StartInfo.FileName = ffmpegPath;
                        p.StartInfo.UseShellExecute = false;
                        p.Start();*/
                        _ini.UpdateIniItem("FFMPEGPATH", ffmpegPathAsync, "Local");
                        // _console.WriteLine($"ffmpegPath = {ffmpegPathAsync}");
                         _console.WriteLine($"use ffmpeg at: \r\n    {ffmpegPathAsync}");
                        ffmpeg = ffmpegPathAsync;
                    }
                    else
                    {
                        _ini.UpdateIniItem("FFMPEGPATH", "YOU DON'T HAVE FFMPEG.", "Local");
                        _console.WriteLine("YOU DON'T HAVE FFMPEG.");
                    }
                }
                if (!string.IsNullOrEmpty(ffmpegPathIni) && File.Exists(ffmpegPathIni))
                {
                    FindFFmpegAsync(ffmpegPathIni);
                    return;
                }
                string ffmpegPath = null;
                void UpdateFFmpegPath(bool res, string pathOrMsg)
                {
                    if (res && File.Exists(pathOrMsg))
                    {
                        ffmpegPath = pathOrMsg;
                    }
                    else
                    {
                        // NO FOUND.
                        // _console.WriteLine(pathOrMsg);
                    }
                }
                Task.Run(() =>
                {
                    try 
                    {
                        var userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                        //_console.WriteLine($"userProfile: {userProfile}");
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"I:\", UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(userProfile, UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"C:\Program Files\", UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"C:\Program Files (x86)\", UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"C:\", UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"D:\", UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"E:\", UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"F:\", UpdateFFmpegPath);
                        if (string.IsNullOrEmpty(ffmpegPath)) ScanDirsFindFFmpeg(@"G:\", UpdateFFmpegPath);
                        FindFFmpegAsync(ffmpegPath);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                    }

                });
            }
            catch (Exception ex)
            {
                // MessageBox.Show(ex.Message);
                Debug.WriteLine(ex.Message);
            }
        }
        private void ScanDirsFindFFmpeg(string path, Action<bool, string> action)
        {
            try
            {
                if (String.IsNullOrEmpty(path) || !Directory.Exists(path))
                {
                    action(false, $"Starting directory is a null or not exist: {path}");
                    return;
                    // throw new ArgumentException("Starting directory is a null reference or an empty string: path");
                }
                //var dirs = from dir in
                //     Directory.EnumerateDirectories(path, "*mpeg*",
                //        SearchOption.AllDirectories)
                //           select dir;
                foreach (var d in Directory.EnumerateDirectories(path, "*mpeg*", SearchOption.AllDirectories))
                {
                    if (d.ToLower().Contains("ffmpeg"))
                    {
                        //_console.WriteLine(d);
                        var npexe = Path.Combine(d, @"bin\ffmpeg.exe");
                        if (File.Exists(npexe))
                        {
                            action(true, npexe);
                            // action(false, $">>>FOUND NOTDPAD++exe {npexe}");
                            return;
                        }
                    }
                    ScanDirsFindFFmpeg(d, action);
                }
            }
            catch (UnauthorizedAccessException) { }
            // action(false, $"NO FOUND AT {path}");
            return;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ffmpeg) || !File.Exists(ffmpeg))
            {
                return;
            }

            try
            {
                // https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.redirectstandardoutput?view=net-5.0
                using (Process p = new Process())
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.RedirectStandardOutput = true;
                    string eOut = null;
                    p.StartInfo.RedirectStandardError = true;
                    p.ErrorDataReceived += new DataReceivedEventHandler((senderIn, eIn) =>
                    { eOut += eIn.Data; });
                    p.StartInfo.FileName = ffmpeg;
                    p.StartInfo.Arguments = "-h";
                    p.Start();

                    // To avoid deadlocks, use an asynchronous read operation on at least one of the streams.  
                    p.BeginErrorReadLine();
                    string output = p.StandardOutput.ReadToEnd();
                    p.WaitForExit();

                    _console.WriteLine($"output = {output}");
                    // _console.WriteLine($"The last 50 characters in the output stream are:\n'{output.Substring(output.Length - 50)}'");
                    _console.WriteLine($"\r\nError stream: \r\n{eOut}");
                }

            }
            catch (Exception ex)
            {
                _console.WriteLine(ex);
            }
        }
    }
}
