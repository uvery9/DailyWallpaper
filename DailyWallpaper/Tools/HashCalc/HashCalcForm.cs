using DailyWallpaper.Helpers;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.HashCalc
{
    public partial class HashCalcForm : Form
    {
        private HashCalc m_hashCalc;
        private TextBoxCons _console;
        delegate void CalcMethod(string path, Action<bool, string, string, string> action, CancellationToken token);
        private Mutex mut = new Mutex();
        private ConfigIni m_ini = ConfigIni.GetInstance();
        private string textNeedToHash = null;
        public bool selfFromClosing = true;
        public HashCalcForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.HASH32x32;
            hashPicBox.AllowDrop = true;
            hashPicBox.Image = Properties.Resources.draganddrop;
            m_hashCalc = new HashCalc
            {
                hashProgressBar = fileProgressBar
            };
            
            _console = new TextBoxCons(new ConsWriter(hashTextBox));
            _console.WriteLine(m_hashCalc.help);
            // _console.WriteLine($"CurrentThread ID: {Thread.CurrentThread.ManagedThreadId}");
            if (selfFromClosing) 
                FormClosing += new FormClosingEventHandler(this.HashCalcForm_FormClosing);
            EnableDefaultHashCheckBoxAndTextBox();
        }

        /// <summary>
        /// SAMPLE USELESS
        /// </summary>
        private void HashCalcForm_DragDrop(object sender, DragEventArgs e)
        {
            int x = this.PointToClient(new Point(e.X, e.Y)).X;
            int y = this.PointToClient(new Point(e.X, e.Y)).Y;
            if (x >= hashPicBox.Location.X && x <= hashPicBox.Location.X + hashPicBox.Width
                && y >= hashPicBox.Location.Y && y <= hashPicBox.Location.Y + hashPicBox.Height)
            {
                // DO SOMETHING.
            } 
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (!IsHandleCreated)
            {
                this.Close();
            }
        }

        private void HashCalcForm_DragOver(object sender, DragEventArgs e)
        {
            int x = this.PointToClient(new Point(e.X, e.Y)).X;
            int y = this.PointToClient(new Point(e.X, e.Y)).Y;
            if (!(x >= hashPicBox.Location.X && x <= hashPicBox.Location.X + hashPicBox.Width
                && y >= hashPicBox.Location.Y && y <= hashPicBox.Location.Y + hashPicBox.Height))
            {
                this.Cursor = System.Windows.Forms.Cursors.No;
            }
        }


        private void hashPicBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string fileLoc in filePaths)
                {
                    if (File.Exists(fileLoc))
                    {
                        m_hashCalc.filePath = fileLoc;
                        hashfileTextBox.Text = fileLoc;
                        hashStringCheckBox.Checked = false;
                        if (automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Checked)
                        {
                            hashCalcButton.PerformClick(); // Pretend to be clicked.
                        }
                    }

                }
            }
        }

        private void hashPicBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void hashfileTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(hashfileTextBox.Text))
            {
                if (File.Exists(hashfileTextBox.Text)){
                    m_hashCalc.filePath = hashfileTextBox.Text;
                }
            }
        }
        private string HashOpenFileDialog(string initString)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {

                try
                {
                    dialog.InitialDirectory = new DirectoryInfo(initString).Name;
                }
                catch
                {
                    dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                }
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
            }
            return null;
        }
        private void fileBrowseButton_Click(object sender, EventArgs e)
        {
            var path = HashOpenFileDialog(hashfileTextBox.Text);
            if (!string.IsNullOrEmpty(path))
            {
                m_hashCalc.filePath = path;
                hashfileTextBox.Text = path;
                if (File.Exists(path)) 
                { 
                    hashCalcButton.PerformClick(); // Pretend to be clicked.
                }
            }
        }

        private string CopyOrSaveInfoFile()
        {
            return CopyOrSaveInfo(m_hashCalc.filePath,
                                  MD5TextBox.Text,
                                  CRC32TextBox.Text,
                                  CRC64TextBox.Text,
                                  SHA1TextBox.Text,
                                  SHA256TextBox.Text,
                                  SHA512TextBox.Text);
        }

        private void NotEmptyThenAppend(ref string all, string name, string input)
        {
            if (!string.IsNullOrEmpty(input))
                all += name + input + "\r\n";
        }
        
        private string CopyOrSaveInfo(string path, string md5, string crc32, string crc64, string sha1, string sha256, string sha512)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return null;
            }
            try
            {
                
                string size;
                
                var sizeNum = new FileInfo(path).Length;
                
                if (sizeNum < 1024)
                {
                    size = sizeNum.ToString() + " Bytes";
                }
                else if(sizeNum > 1024 * 1024)
                {
                    size = (sizeNum / (1024 * 1024)).ToString() + " MB";
                }
                else
                {
                    size = (sizeNum / (1024)).ToString() + " KB";

                }
                string ret = "";
                NotEmptyThenAppend(ref ret, "File:\r\n  ", path);
                NotEmptyThenAppend(ref ret, "Size:          ", size);
                NotEmptyThenAppend(ref ret, "CRC32:         ", crc32);
                NotEmptyThenAppend(ref ret, "CRC64:         ", crc64);
                NotEmptyThenAppend(ref ret, "MD5:           ", md5);
                NotEmptyThenAppend(ref ret, "SHA1:          ", sha1);
                NotEmptyThenAppend(ref ret, "SHA256:        ", sha256);
                NotEmptyThenAppend(ref ret, "SHA512:\r\n", sha512);
                NotEmptyThenAppend(ref ret, "LastWriteTime: ", new FileInfo(path).LastWriteTime.ToString());
                ret += "    Paste from Hash Calculator in DailyWallpaper.\r\n";
                
                return ret;
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                return null;
            }
        }

        private CancellationTokenSource fileCancel;
        private void EnableDefaultHashCheckBoxAndTextBox()
        {
            // use default text to adjust the interface.
            hashfileTextBox.Text = "";
           
            if (m_ini.EqualsIgnoreCase("AllowConsoleTextBoxDrop", "true", "HashCalc"))
            {
                hashTextBox.AllowDrop = true;
                hashTextBox.ReadOnly = false;
                hashStringCheckBox.Checked = true;
                consoleTextBoxAllowDropToolStripMenuItem.Checked = true;
            }
            else
            {
                hashTextBox.AllowDrop = false;
                hashTextBox.ReadOnly = true;
                hashStringCheckBox.Checked = false;
                consoleTextBoxAllowDropToolStripMenuItem.Checked = false;
            }

            OptionInit("AlwaysOnTop", alwaysOnTopToolStripMenuItem, false);
            OptionInit("AutoCalcAfterDrop", automaticallyCalculateHashAfterDragAndDropToolStripMenuItem, true);
            OptionInit("lowercase", useUppercaseLettersInHashToolStripMenuItem, false);

            HashCheckBoxTextBoxInitDef();
            HashCheckBoxTextBoxInit("MD5", MD5checkBox, MD5TextBox);
            HashCheckBoxTextBoxInit("CRC64", CRC64checkBox, CRC64TextBox);
            HashCheckBoxTextBoxInit("SHA1", SHA1checkBox, SHA1TextBox);
            HashCheckBoxTextBoxInit("CRC32", CRC32checkBox, CRC32TextBox);
            HashCheckBoxTextBoxInit("SHA256", SHA256checkBox, SHA256TextBox);
            HashCheckBoxTextBoxInit("SHA512", SHA512checkBox, SHA512TextBox);
        }
        
        private void HashCheckBoxTextBoxInitDef()
        {
            CRC32checkBox.Checked = true;
            MD5checkBox.Checked = true;
            SHA1checkBox.Checked = true;
            SHA256checkBox.Checked = true;

            CRC32TextBox.Enabled = true;
            MD5TextBox.Enabled = true;
            SHA1TextBox.Enabled = true;
            SHA256TextBox.Enabled = true;

        }
        private void HashCheckBoxTextBoxInit(string hashKeyInIni, CheckBox cb, TextBox tb)
        {
            if (m_ini.EqualsIgnoreCase(hashKeyInIni, "true", "HashCalc"))
            {
                cb.Checked = true;
                tb.Enabled = true;
            }

            if (m_ini.EqualsIgnoreCase(hashKeyInIni, "false", "HashCalc"))
            {
                cb.Checked = false;
                tb.Enabled = false;
            }
            tb.Text = "";
            tb.TabStop = false;
        }

        // default value: init, if not, set !default
        private void OptionInit(string optionName, ToolStripMenuItem it, bool init)
        {
            it.Checked = init;
            if (m_ini.EqualsIgnoreCase(optionName, "true", "HashCalc"))
            {
                it.Checked = true;
            }
            if (m_ini.EqualsIgnoreCase(optionName, "false", "HashCalc"))
            {
                it.Checked = false;
            }
        }

        private void save2File(string file, string str)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file))
            {
                return;
            }
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory =
                    Path.GetDirectoryName(file);
                    // Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog.Filter = "Txt files (*.txt)|*.txt";
                saveFileDialog.RestoreDirectory = true;
                var fi = Path.GetFileName(file);
                // DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
                saveFileDialog.FileName = fi + ".hash.txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var stream = saveFileDialog.OpenFile())
                        {
                            var dataAsBytes = Encoding.Default.GetBytes(str);
                            stream.Write(dataAsBytes, 0, dataAsBytes.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        _console.WriteLine("Save hash file error: " + ex.Message);
                    }
                }
            }

        }
        private void fileCopyButton_Click(object sender, EventArgs e)
        {
            try
            {
                var s = CopyOrSaveInfoFile();
                if (!string.IsNullOrEmpty(s))
                {
                    Clipboard.SetText(s);
                    _console.WriteLine("\r\nCopied to Clipboard");
                    _console.WriteLine($"\r\n{s}");
                }
                else
                {
                    _console.WriteLine("Nothing to be copied.");
                }
                
            }
            catch (Exception ex)
            {
                _console.WriteLine(ex.Message);
            }
        }

        private void fileSaveButton_Click(object sender, EventArgs e)
        {
            var s = CopyOrSaveInfoFile();
            if (!string.IsNullOrEmpty(s))
            {
                save2File(m_hashCalc.filePath, s);
            }
            else
            {
                _console.WriteLine("Nothing to be saved.");
            }
            
        }

        private void CheckBoxAffectTextBox(CheckBox cb, TextBox tb, string keyInIni)
        {
            if (cb.Checked)
            {
                tb.Enabled = true;
            }
            else
            {
                tb.Enabled = false;
            }
            m_ini.UpdateIniItem(keyInIni, tb.Enabled.ToString(), "HashCalc");
        }
        private void MD5checkBox_Click(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(MD5checkBox, MD5TextBox, "MD5");
        }

        private void CRC64checkBox_Click(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(CRC64checkBox, CRC64TextBox, "CRC64");
        }

        private void SHA1checkBox_Click(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(SHA1checkBox, SHA1TextBox, "SHA1");
        }

        private void CRC32checkBox_Click(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(CRC32checkBox, CRC32TextBox, "CRC32");
        }

        private void SHA256checkBox_Click(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(SHA256checkBox, SHA256TextBox, "SHA256");
        }
        private void SHA512checkBox_Click(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(SHA512checkBox, SHA512TextBox, "SHA512");
        }

        private void Calculate(HashCalc hashCalc)
        {
            fileCancel = new CancellationTokenSource();
            var token = fileCancel.Token;
            
            hashCalc.succeedCnt = 0;
            hashCalc.hashProgressBar.Value = 0;
            hashCalc.percent = 0.0;

            hashCalc.hashCalcCnt = 0;
            if (MD5checkBox.Checked)
                hashCalc.hashCalcCnt += 1;
            if (SHA1checkBox.Checked)
                hashCalc.hashCalcCnt += 1;
            if (SHA256checkBox.Checked)
                hashCalc.hashCalcCnt += 1;
            if (SHA512checkBox.Checked)
                hashCalc.hashCalcCnt += 1;

            HashAlgorithmCalc(CRC32TextBox, CRC32checkBox, hashCalc.CalcCRC32, token, totalProgress: false);
            HashAlgorithmCalc(CRC64TextBox, CRC64checkBox, hashCalc.CalcCRC64, token, totalProgress: false);
            HashAlgorithmCalc(MD5TextBox, MD5checkBox, hashCalc.CalcMD5, token);
            HashAlgorithmCalc(SHA1TextBox, SHA1checkBox, hashCalc.CalcSHA1, token);
            HashAlgorithmCalc(SHA256TextBox, SHA256checkBox, hashCalc.CalcSHA256, token);
            HashAlgorithmCalc(SHA512TextBox, SHA512checkBox, hashCalc.CalcSHA512, token);
        }

        private void HashAlgorithmCalc(TextBox tx, CheckBox cb, CalcMethod Calcmethod, CancellationToken token, bool totalProgress = true)
        {
            void TellResultAsync(bool res, string who, string result, string costTimeOrReson)
            {
                mut.WaitOne();
                if (res)
                {
                    if (useUppercaseLettersInHashToolStripMenuItem.Checked)
                    {
                        result = result.ToLower();
                    }
                    tx.Text = result;
                    _console.WriteLine("" + who + result + "\r\n        cost time: " + costTimeOrReson);
                    if (totalProgress) 
                        m_hashCalc.succeedCnt += 1;
                    if (m_hashCalc.succeedCnt == m_hashCalc.hashCalcCnt)
                    {
                        m_hashCalc.hashProgressBar.Value = 100;
                    }
                }
                else
                {
                    _console.WriteLine("\r\n>>> " + who + costTimeOrReson);
                    fileProgressBar.Value = 0;
                }
                mut.ReleaseMutex();
            }
            tx.Text = "";
            if (cb.Checked)
            {
                // hashCalc.CalcMD5
                Calcmethod(m_hashCalc.filePath, TellResultAsync, token);
            }
        }
        private void HashAlgorithmCalc(HashAlgorithm ha, TextBox tx, CheckBox cb)
        {
            if (cb.Checked) {
                string result = m_hashCalc.ComputeHashOfString(ha, textNeedToHash);
                if (useUppercaseLettersInHashToolStripMenuItem.Checked)
                {
                    result = result.ToLower();
                }
                tx.Text = result;
            }
        }
        private void Calculate(string input)
        {
            hashTextBox.Text = $"\r\nCalculating string in UTF-8: \r\n===============\r\n\"{input}\"\r\n===============\r\n";
            HashAlgorithmCalc(MD5.Create(), MD5TextBox, MD5checkBox);
            HashAlgorithmCalc(SHA256.Create(), SHA256TextBox, SHA256checkBox);
            HashAlgorithmCalc(SHA1.Create(), SHA1TextBox, SHA1checkBox);
            HashAlgorithmCalc(SHA512.Create(), SHA512TextBox, SHA512checkBox);
        }

        private void hashCalcButton_Click(object sender, EventArgs e)
        {
            if (!hashStringCheckBox.Checked) { 
                if (string.IsNullOrEmpty(hashfileTextBox.Text) || !File.Exists(hashfileTextBox.Text))
                {
                    _console.WriteLine("Please check the File textbox...");
                    return;
                }
                var sizeNum = new FileInfo(hashfileTextBox.Text).Length / 1024;
                var size = sizeNum.ToString() + " KB";
                if (sizeNum > 1024)
                {
                    sizeNum /= 1024;
                    size = sizeNum.ToString() + " MB";
                }
                var info = "\r\nFile:   " + hashfileTextBox.Text + "\r\n";
                info    += "Size:   " + size;
                _console.WriteLine(info);
                Calculate(m_hashCalc);
            }
            else
            {
                if (string.IsNullOrEmpty(textNeedToHash))
                {
                    if (string.IsNullOrEmpty(hashTextBox.Text))
                    {
                        _console.WriteLine("you need to type something. Clear me and do it.");
                        return;
                    }
                    textNeedToHash = hashTextBox.Text;
                }
                
                Calculate(textNeedToHash);
            }
        }

      /* private void fileHashbackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
       * {
       *     string filePath = e.Argument.ToString();
       *     byte[] buffer;
       *     int bytesRead;
       *     long size;
       *     long totalBytesRead = 0;
       *     using (var file = File.OpenRead(filePath))
       *     {
       *         size = file.Length;
       *         using (var hasher = MD5.Create()) {
       *             do
       *             {
       *                 buffer = new byte[1024 * 1024 * 5];
       *                 bytesRead = file.Read(buffer, 0, buffer.Length);
       *                 totalBytesRead += bytesRead;
       *                 hasher.TransformBlock(buffer, 0, bytesRead, null, 0);
       *                 fileHashbackGroundWorker.ReportProgress((int)((double)totalBytesRead / size * 100));
       *
       *
       *             } while (bytesRead != 0);
       *             hasher.TransformFinalBlock(buffer, 0, 0);
       *             e.Result = MakeHashString(hasher.Hash);
       *         }
       *     }
       * }
        */

        /*        private void fileHashbackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
         *   {
         *       fileProgressBar.Value = e.ProgressPercentage;
         *   }       private void fileHashbackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
         *   {
         *       // MessageBox.Show(e.Result.ToString());
         *       hashTextBox.Text = e.Result.ToString();
         *       fileProgressBar.Value = 0;
         * }
         */

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = alwaysOnTopToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
            }
            else
            {
                it.Checked = true;
            }
            TopMost = it.Checked;
            m_ini.UpdateIniItem("AlwaysOnTop", it.Checked.ToString(), "HashCalc");
        }

        private void automaticallyCalculateHashAfterDragAndDropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = automaticallyCalculateHashAfterDragAndDropToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
            }
            else
            {
                it.Checked = true;
            }
            m_ini.UpdateIniItem("AutoCalcAfterDrop", it.Checked.ToString(), "HashCalc");
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
           
            hashTextBox.Text = "";
            MD5TextBox.Text = "";
            CRC32TextBox.Text = "";
            CRC64TextBox.Text = "";
            SHA1TextBox.Text = "";
            SHA256TextBox.Text = "";
            SHA512TextBox.Text = "";
            fileProgressBar.Value = 0;

            if (!hashStringCheckBox.Checked)
                hashfileTextBox.Text = "";
            m_hashCalc.filePath = "";
            textNeedToHash = null;
            if (fileCancel != null)
            {
                fileCancel.Cancel();
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (fileCancel != null)
            {
                fileCancel.Cancel();
            }
            _console.WriteLine("Stop...");
            fileProgressBar.Value = 0;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader?view=net-5.0
        /// </summary>
        private void hashTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                m_hashCalc.tasks.Add(
                Task.Run(()=>
                {
                    string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                    if (File.Exists(filePaths[0]))
                    {
                        var file = filePaths[0];
                        if (new FileInfo(file).Length < 1024 * 1024)
                        {
                            hashStringCheckBox.Checked = true;
                            textNeedToHash = File.ReadAllText(filePaths[0]);
/*                            if ((textNeedToHash.Length) > 5000)
                            {
                                _console.WriteLine("Try to use file mode.");
                                return;
                            }*/
                            mut.WaitOne();
                            hashTextBox.Text = textNeedToHash;                           
                            mut.ReleaseMutex();
                            if (automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Checked)
                            {
                                hashCalcButton.PerformClick(); // Pretend to be clicked.
                            }
                        }
                        else
                        {
                            hashStringCheckBox.Checked = false;
                            m_hashCalc.filePath = file;
                            hashfileTextBox.Text = file;
                            if (automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Checked)
                            {
                                hashCalcButton.PerformClick(); // Pretend to be clicked.
                            }
                        }
                        //Task.Run(async() => await DealWithHashTextBoxDragDrop(filePaths[0]));
                    }
                }));
            }
        }

        private void usageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _console.WriteLine(m_hashCalc.help);
        }

        private void donateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(ProjectInfo.DonationUrl);
        }

        private void consoleTextBoxAllowDropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = consoleTextBoxAllowDropToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
            }
            else
            {
                it.Checked = true;
            }
            hashStringCheckBox.Checked = it.Checked;
            if (hashStringCheckBox.Checked)
            {
                hashfileTextBox.Text = "       Calculate the hash value of the text in the console.      ";
            }
            else
            {
                hashfileTextBox.Text = "";
            }
            hashTextBox.ReadOnly = !hashStringCheckBox.Checked;
            hashTextBox.AllowDrop = hashStringCheckBox.Checked;
            m_ini.UpdateIniItem("AllowConsoleTextBoxDrop",
                hashStringCheckBox.Checked.ToString(), "HashCalc");
        }

        private void HashCalcForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            hashTextBox.AllowDrop = false;
            hashPicBox.AllowDrop = false;
            filePanel.AllowDrop = false;
            stopButton.PerformClick();
            stopButton.PerformClick();
            stopButton.PerformClick();
            // MessageBox.Show("want to close");
            Hide();
            // MessageBox.Show("hiding..");
            Task.Run(()=>
            {   // MessageBox.Show("Start WaitAll.");
                Task.WaitAll(m_hashCalc.tasks.ToArray());
                // MessageBox.Show("finished.");
                e.Cancel = false;
            }
            );
            // MessageBox.Show("closing...");
            // MessageBox.Show($"{hashTextBox.Text}"); // should error.
            // this.Close();
            // mut.Dispose();
            // Thread.Sleep(500);
        }

        private void useUppercaseLettersInHashToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = useUppercaseLettersInHashToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
            }
            else
            {
                it.Checked = true;
            }
            m_ini.UpdateIniItem("lowercase", it.Checked.ToString(), "HashCalc");
        }

        private void hashStringCheckBox_Click(object sender, EventArgs e)
        {
            consoleTextBoxAllowDropToolStripMenuItem.PerformClick();
        }
    }
}
