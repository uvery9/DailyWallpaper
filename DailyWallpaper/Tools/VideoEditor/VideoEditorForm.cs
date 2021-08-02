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
            
            _console = new TextBoxCons(new ConsWriter(consRichTextBox));
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

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs oneLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            // Console.WriteLine(outLine.Data);
            // _console.WriteLine(outLine.Data); // why no response???
            // videoEditorConsTextBox.Text += oneLine.Data + Environment.NewLine;
            /*Debug.WriteLine(oneLine.Data);*/
        }
        private void ErrorHandler(object sendingProcess, DataReceivedEventArgs oneLine)
        {
            //* Do your stuff with the output (write to console/log/StringBuilder)
            // Console.WriteLine(outLine.Data);
            // _console.WriteLine(outLine.Data); // why no response???
            consRichTextBox.Text += oneLine.Data + Environment.NewLine;
            /*
            consRichTextBox.SelectionStart = 7;
            consRichTextBox.SelectionLength = 7;
            consRichTextBox.SelectionColor = Color.Red;*/
            //LogError(oneLine.Data);
            // Application.DoEvents();
            //Application.DoEvents();
            //LogWithColor(consRichTextBox, Color.Red, oneLine.Data);
            // LogWithColorDirect(consRichTextBox, Color.Red, oneLine.Data);

            /*Debug.WriteLine(oneLine.Data);*/
        }

        #region 日志记录、支持其他线程访问 
        public delegate void LogAppendDelegate(Color color, string text);
        /// <summary> 
        /// 追加显示文本 
        /// </summary> 
        /// <param name="color">文本颜色</param> 
        /// <param name="text">显示文本</param> 
        public void LogAppend(Color color, string text)
        {
            consRichTextBox.AppendText("\r\n");
            consRichTextBox.SelectionColor = color;
            consRichTextBox.AppendText(text);
        }
        /// <summary> 
        /// 显示错误日志 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogError(string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            consRichTextBox.Invoke(la, Color.Red, DateTime.Now.ToString("HH:mm:ss ") + text);
        }
        /// <summary> 
        /// 显示警告信息 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogWarning(string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            consRichTextBox.Invoke(la, Color.Violet, DateTime.Now.ToString("HH:mm:ss ") + text);
        }
        /// <summary> 
        /// 显示信息 
        /// </summary> 
        /// <param name="text"></param> 
        public void LogMessage(string text)
        {
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            consRichTextBox.Invoke(la, Color.Black, DateTime.Now.ToString("HH:mm:ss ") + text);
        }
        #endregion

        private void LogWithColor(RichTextBox rtb, Color color, string text)
        {
            if (text == null)
            {
                return;
            }

            if (InvokeRequired)
            {
                Invoke(new Action(() => LogWithColor(rtb, color, text)));
                return;
            }

            rtb.AppendText(Environment.NewLine);
            rtb.SelectionColor = color;
            rtb.AppendText(text);
        }

        private void RunCommand(Process p, string exePath, string args)
        {
            //* Create your Process
            // ffmpeg.exe -h
            p.StartInfo.FileName = exePath;
            p.StartInfo.Arguments = args;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            //* Set your output and error (asynchronous) handlers
            p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.ErrorDataReceived += new DataReceivedEventHandler(ErrorHandler);

            //* Start process and handlers
            p.Start();
            p.BeginOutputReadLine();
            p.BeginErrorReadLine();
            p.WaitForExit();
        }

        private async void startButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ffmpeg) || !File.Exists(ffmpeg))
            {
                return;
            }

            LogWithColor(consRichTextBox, Color.Black, "Start...");
            string args = "-h";

            await Task.Run(() =>
            {
                using (var p = new Process())
                {
                    p.StartInfo = new ProcessStartInfo(ffmpeg, args)
                    {
                        UseShellExecute = false,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    };

                    p.EnableRaisingEvents = true;

                    p.OutputDataReceived += (_, data) =>
                    {
                        LogWithColor(consRichTextBox, Color.Black, data.Data);
                    };

                    p.ErrorDataReceived += (_, data) =>
                    {
                        LogWithColor(consRichTextBox, Color.Red, data.Data);
                    };

                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                }
            });

            LogWithColor(consRichTextBox, Color.Black, "Finished.");
        }
        /*

         using System;
        using System.Text;
        using System.Windows.Forms;
        using System.Diagnostics;

        namespace WindowsFormsApplication1
        {
            public partial class Form1 : Form
            {
                public Form1()
                {
                    InitializeComponent();
                }

                private object syncGate = new object();
                private Process process;
                private StringBuilder output = new StringBuilder();
                private bool outputChanged;

                private void button1_Click(object sender, EventArgs e)
                {
                    lock (syncGate)
                    {
                        if (process != null) return;
                    }

                    output.Clear();
                    outputChanged = false;
                    richTextBox1.Text = "";

                    process = new Process();
                    process.StartInfo.FileName = @"c:/req/dist/ex/ex.exe";
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.OutputDataReceived += OnOutputDataReceived;
                    process.Exited += OnProcessExited;
                    process.Start();
                    process.BeginOutputReadLine();
                }

                private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
                {
                    lock (syncGate)
                    {
                        if (sender != process) return;
                        output.AppendLine(e.Data);
                        if (outputChanged) return;
                        outputChanged = true;
                        BeginInvoke(new Action(OnOutputChanged));
                    }
                }

                private void OnOutputChanged()
                {
                    lock (syncGate)
                    {
                        richTextBox1.Text = output.ToString();
                        outputChanged = false;
                    }
                }

                private void OnProcessExited(object sender, EventArgs e)
                {
                    lock (syncGate)
                    {
                        if (sender != process) return;
                        process.Dispose();
                        process = null;
                    }
                }
            }
        }






         */

        /*
         * https://stackoverflow.com/questions/4501511/c-sharp-realtime-console-output-redirection?noredirect=1&lq=1
         public class ConsoleInputReadEventArgs : EventArgs
        {
            public ConsoleInputReadEventArgs(string input)
            {
                this.Input = input;
            }

            public string Input { get; private set; }
        }

        public interface IConsoleAutomator
        {
            StreamWriter StandardInput { get; }

            event EventHandler<ConsoleInputReadEventArgs> StandardInputRead;
        }

        public abstract class ConsoleAutomatorBase : IConsoleAutomator
        {
            protected readonly StringBuilder inputAccumulator = new StringBuilder();

            protected readonly byte[] buffer = new byte[256];

            protected volatile bool stopAutomation;

            public StreamWriter StandardInput { get; protected set; }

            protected StreamReader StandardOutput { get; set; }

            protected StreamReader StandardError { get; set; }

            public event EventHandler<ConsoleInputReadEventArgs> StandardInputRead;

            protected void BeginReadAsync()
            {
                if (!this.stopAutomation) {
                    this.StandardOutput.BaseStream.BeginRead(this.buffer, 0, this.buffer.Length, this.ReadHappened, null);
                }
            }

            protected virtual void OnAutomationStopped()
            {
                this.stopAutomation = true;
                this.StandardOutput.DiscardBufferedData();
            }

            private void ReadHappened(IAsyncResult asyncResult)
            {
                var bytesRead = this.StandardOutput.BaseStream.EndRead(asyncResult);
                if (bytesRead == 0) {
                    this.OnAutomationStopped();
                    return;
                }

                var input = this.StandardOutput.CurrentEncoding.GetString(this.buffer, 0, bytesRead);
                this.inputAccumulator.Append(input);

                if (bytesRead < this.buffer.Length) {
                    this.OnInputRead(this.inputAccumulator.ToString());
                }

                this.BeginReadAsync();
            }

            private void OnInputRead(string input)
            {
                var handler = this.StandardInputRead;
                if (handler == null) {
                    return;
                }

                handler(this, new ConsoleInputReadEventArgs(input));
                this.inputAccumulator.Clear();
            }
        }

        public class ConsoleAutomator : ConsoleAutomatorBase, IConsoleAutomator
        {
            public ConsoleAutomator(StreamWriter standardInput, StreamReader standardOutput)
            {
                this.StandardInput = standardInput;
                this.StandardOutput = standardOutput;
            }

            public void StartAutomate()
            {
                this.stopAutomation = false;
                this.BeginReadAsync();
            }

            public void StopAutomation()
            {
                this.OnAutomationStopped();
            }
        }

        var processStartInfo = new ProcessStartInfo
    {
        FileName = "myprocess.exe",
        RedirectStandardInput = true,
        RedirectStandardOutput = true,
        UseShellExecute = false,
    };

var process = Process.Start(processStartInfo);
var automator = new ConsoleAutomator(process.StandardInput, process.StandardOutput);

// AutomatorStandardInputRead is your event handler
automator.StandardInputRead += AutomatorStandardInputRead;
automator.StartAutomate();

// do whatever you want while that process is running
process.WaitForExit();
automator.StandardInputRead -= AutomatorStandardInputRead;
process.Close();
         */
    }
}
