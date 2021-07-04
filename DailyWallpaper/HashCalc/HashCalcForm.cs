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
        private static Mutex mut = new Mutex();
        private ConfigIni m_ini = ConfigIni.GetInstance();
        private string textNeedToHash = null;
        public HashCalcForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.HASH32x32;
            hashPicBox.AllowDrop = true;
            m_hashCalc = new HashCalc();
            m_hashCalc.hashProgressBar = fileProgressBar;
            EnableDefaultHashCheckBoxAndTextBox();
            _console = new TextBoxCons(new ConsWriter(hashTextBox));
            _console.WriteLine(m_hashCalc.help);
            _console.WriteLine($"CurrentThread ID: {Thread.CurrentThread.ManagedThreadId}");
            
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
                                  SHA256TextBox.Text);
        }

        private string CopyOrSaveInfo(string name, string md5, string crc32, string crc64, string sha1, string sha256)
        {
            string failed = $"connect to {ProjectInfo.author} with {ProjectInfo.email}.";
            try
            {
                string size;
                if (File.Exists(name))
                {
                    var sizeNum = new FileInfo(name).Length / 1024;
                    size = sizeNum.ToString() + " KB\r\n";
                    if (sizeNum > 1024)
                    {
                        sizeNum /= 1024;
                        size = sizeNum.ToString() + " MB\r\n";
                    }
                }
                else
                {
                    return failed;
                }
                string ret = "File:   "   + name + "\r\n";
                ret += "Size:   "   + size;
                ret += "CRC32:  "  + (string.IsNullOrEmpty(crc32) ? "" : crc32) + "\r\n";
                ret += "CRC64:  "  + (string.IsNullOrEmpty(crc64) ? "" : crc64) + "\r\n";
                ret += "MD5:    " + (string.IsNullOrEmpty(md5) ? "" : md5) + "\r\n";
                ret += "SHA1:   "   + (string.IsNullOrEmpty(sha1) ? "" : sha1) + "\r\n";
                ret += "SHA256: " + (string.IsNullOrEmpty(sha256) ? "" : sha256) + "\r\n";
                ret += "    Paste from Hash Calculator in DailyWallpaper.\r\n";
                return ret;
            }
            catch (Exception e)
            {
                _console.WriteLine(e.Message);
                return failed;
            }
        }

        private CancellationTokenSource fileCancel;
        private void EnableDefaultHashCheckBoxAndTextBox()
        {
            // use default text to adjust the interface.
            hashfileTextBox.Text = "";
            MD5checkBox.Checked = true;
            MD5TextBox.Enabled = true;
            MD5TextBox.Text = "";

            CRC64checkBox.Checked = true;
            CRC64TextBox.Enabled = true;
            CRC64TextBox.Text = "";

            SHA1checkBox.Checked = true;
            SHA1TextBox.Enabled = true;
            SHA1TextBox.Text = "";

            CRC32checkBox.Checked = true;
            CRC32TextBox.Enabled = true;
            CRC32TextBox.Text = "";

            SHA256checkBox.Checked = false;
            SHA256TextBox.Enabled = false;
            SHA256TextBox.Text = "";

            SHA512checkBox.Checked = false;
            SHA512TextBox.Enabled = false;
            SHA512TextBox.Text = "";

            MD5TextBox.TabStop = false;
            CRC64TextBox.TabStop = false;
            SHA1TextBox.TabStop = false;
            CRC32TextBox.TabStop = false;
            SHA256TextBox.TabStop = false;
            SHA512TextBox.TabStop = false;
            automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Checked = true;
            enableConsoleStringHashGeneratorToolStripMenuItem.Checked = false;
            hashTextBox.ReadOnly = true;
            alwaysOnTopToolStripMenuItem.Checked = false;
            if (m_ini.EqualsIgnoreCase("HashCalcAlwaysOnTop", "true"))
            {
                alwaysOnTopToolStripMenuItem.Checked = true;
            }
            hashTextBox.AllowDrop = false;
            if (m_ini.EqualsIgnoreCase("hashTextBoxAllowDrop", "true"))
            {
                hashTextBox.AllowDrop = true;
                hashTextBox.ReadOnly = false;
            }


        }
        private void save2File(string file, string str)
        {
            var fi = Path.GetFileName(file);
            if (string.IsNullOrEmpty(file))
            {
                fi = "invalid";
            }
            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                saveFileDialog.Filter = "Txt files (*.txt)|*.txt";
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.FileName = fi + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    using (var stream = saveFileDialog.OpenFile())
                    {
                        var dataAsBytes = System.Text.Encoding.Default.GetBytes(str);
                        stream.Write(dataAsBytes, 0, dataAsBytes.Length);
                    }
                }
            }

        }
        private void fileCopyButton_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetText(CopyOrSaveInfoFile());
                _console.WriteLine("\r\nCopied to Clipboard");
            }
            catch (Exception ex)
            {
                _console.WriteLine(ex.Message);
            }
        }

        private void fileSaveButton_Click(object sender, EventArgs e)
        {
            save2File(m_hashCalc.filePath, CopyOrSaveInfoFile());
        }

        private void CheckBoxAffectTextBox(CheckBox cb, TextBox tb)
        {
            if (cb.Checked)
            {
                tb.Enabled = true;
            }
            else
            {
                tb.Enabled = false;
            }
        }
        private void MD5checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(MD5checkBox, MD5TextBox);
        }

        private void CRC64checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(CRC64checkBox, CRC64TextBox);
        }

        private void SHA1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(SHA1checkBox, SHA1TextBox);
        }

        private void CRC32checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(CRC32checkBox, CRC32TextBox);
        }

        private void SHA256checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(SHA256checkBox, SHA256TextBox);
        }
        private void SHA512checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(SHA512checkBox, SHA512TextBox);
        }

        private void Calculate(HashCalc hashCalc)
        {
            fileCancel = new CancellationTokenSource();
            var token = fileCancel.Token;
            HashAlgorithmCalc(MD5TextBox, MD5checkBox, hashCalc.CalcMD5, token);
            HashAlgorithmCalc(SHA256TextBox, SHA256checkBox, hashCalc.CalcSHA256, token);
            HashAlgorithmCalc(SHA1TextBox, SHA1checkBox, hashCalc.CalcSHA1, token);
            HashAlgorithmCalc(CRC32TextBox, CRC32checkBox, hashCalc.CalcCRC32, token);
            HashAlgorithmCalc(CRC64TextBox, CRC64checkBox, hashCalc.CalcCRC64, token);
            HashAlgorithmCalc(SHA512TextBox, SHA512checkBox, hashCalc.CalcSHA512, token);
        }



        private void HashAlgorithmCalc(TextBox tx, CheckBox cb, CalcMethod Calcmethod, CancellationToken token)
        {
            void TellResultAsync(bool res, string who, string result, string costTimeOrReson)
            {
                mut.WaitOne();
                if (res)
                {
                    tx.Text = result;
                    _console.WriteLine("" + who + result + "\r\n        cost time: " + costTimeOrReson);
                }
                else
                {
                    _console.WriteLine("\r\n>>> " + who + costTimeOrReson);
                    fileProgressBar.Value = 0;
                }
                mut.ReleaseMutex();
            }
            if (cb.Checked)
            {
                // m_hashCalc.CalcMD5
                Calcmethod(m_hashCalc.filePath, TellResultAsync, token);
            }
        }
        private void HashAlgorithmCalc(HashAlgorithm ha, TextBox tx, CheckBox cb)
        {
            if (cb.Checked) {
                tx.Text = m_hashCalc.ComputeHashOfString(ha, textNeedToHash);
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
                TopMost = false;
                m_ini.UpdateIniItem("HashCalcAlwaysOnTop", "false");

            }
            else
            {
                it.Checked = true;
                TopMost = true;
                m_ini.UpdateIniItem("HashCalcAlwaysOnTop", "true");
            }
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
            // fileProgressBar.Value = 0;
        }

        /// <summary>
        /// https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader?view=net-5.0
        /// </summary>
        private void hashTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
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
                });
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

        private void enableConsoleStringHashGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var it = enableConsoleStringHashGeneratorToolStripMenuItem;
            if (it.Checked)
            {
                it.Checked = false;
                hashTextBox.ReadOnly = true;
                hashTextBox.AllowDrop = false;
                m_ini.UpdateIniItem("hashTextBoxAllowDrop", "false");
            }
            else
            {
                it.Checked = true;
                hashTextBox.ReadOnly = false;
                hashTextBox.AllowDrop = true;
                m_ini.UpdateIniItem("hashTextBoxAllowDrop", "true");
            }
        }

        private void HashCalcForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            hashTextBox.AllowDrop = false;
            hashPicBox.AllowDrop = false;
            filePanel.AllowDrop = false;
            stopButton.PerformClick();
            // mut.Dispose();
            // Thread.Sleep(500);
        }

        private void hashStringCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (hashStringCheckBox.Checked)
            {
                hashfileTextBox.Text = "       Calculate the hash value of the text in the console.      ";
            }
            else
            {
                // _console.WriteLine("Calculate the hash value of file.");
                hashfileTextBox.Text = "";
            }
        }
    }
}
