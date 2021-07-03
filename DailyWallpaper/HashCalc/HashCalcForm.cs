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
        public HashCalcForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.HASH32x32;
            hashPicBox.AllowDrop = true;
            m_hashCalc = new HashCalc();
            m_hashCalc.hashProgressBar = fileProgressBar;
            EnableAllHashCheckBoxAndTextBox();
            _console = new TextBoxCons(new ConsWriter(hashTextBox));
            _console.WriteLine(m_hashCalc.help);

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
                        if (automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Checked)
                        {
                            fileCalcButton.PerformClick(); // Pretend to be clicked.
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

        private void hashTextBox_KeyDown(object sender, KeyEventArgs e)
        {

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
                    fileCalcButton.PerformClick(); // Pretend to be clicked.
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
        private void EnableAllHashCheckBoxAndTextBox()
        {
            MD5checkBox.Checked = true;
            MD5TextBox.Enabled = true;

            CRC64checkBox.Checked = true;
            CRC64TextBox.Enabled = true;

            SHA1checkBox.Checked = true;
            SHA1TextBox.Enabled = true;

            CRC32checkBox.Checked = true;
            CRC32TextBox.Enabled = true;

            SHA256checkBox.Checked = true;
            SHA256TextBox.Enabled = true;

            MD5TextBox.TabStop = false;
            CRC64TextBox.TabStop = false;
            SHA1TextBox.TabStop = false;
            CRC32TextBox.TabStop = false;
            SHA256TextBox.TabStop = false;
            automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Checked = true;
            
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
                _console.WriteLine("Copied to Clipboard");
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
       

        private void Calculate(HashCalc hashCalc)
        {
            fileCancel = new CancellationTokenSource();
            var token = fileCancel.Token;
            //fileCalcButton.Enabled = false;
            HashAlgorithmCalc(MD5TextBox, MD5checkBox, hashCalc.CalcMD5, token);
            HashAlgorithmCalc(SHA256TextBox, SHA256checkBox, hashCalc.CalcSHA256, token);
            HashAlgorithmCalc(SHA1TextBox, SHA1checkBox, hashCalc.CalcSHA1, token);
            HashAlgorithmCalc(CRC32TextBox, CRC32checkBox, hashCalc.CalcCRC32, token);
            HashAlgorithmCalc(CRC64TextBox, CRC64checkBox, hashCalc.CalcCRC64, token);
            //fileCalcButton.Enabled = true;
            //stopButton.Enabled = false;
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

        private void fileCalcButton_Click(object sender, EventArgs e)
        {
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

        private void fileClearButton_Click(object sender, EventArgs e)
        {
            MD5TextBox.Text = "";
            CRC32TextBox.Text = "";
            CRC64TextBox.Text = "";
            SHA1TextBox.Text = "";
            SHA256TextBox.Text = "";
            hashfileTextBox.Text = "";
            m_hashCalc.filePath = "";

            if (fileCancel != null)
            {
                fileCancel.Cancel();
            }

        }

        private string MakeHashString(byte[] hashBytes)
        {
            var hash = new StringBuilder(32);
            foreach (var b in hashBytes)
            {
                hash.Append(b.ToString("X2"));
            }
            return hash.ToString();
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
            }
            else
            {
                it.Checked = true;
                TopMost = true;
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
            MD5TextBox.Text = "";
            CRC32TextBox.Text = "";
            CRC64TextBox.Text = "";
            SHA1TextBox.Text = "";
            SHA256TextBox.Text = "";
            hashfileTextBox.Text = "";
            m_hashCalc.filePath = "";
            hashTextBox.Text = "";
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

        private void hashTextBox_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string fileLoc in filePaths)
                {
                    if (File.Exists(fileLoc))
                    {
                        string ext = Path.GetExtension(fileLoc);
                        _console.WriteLine($"file ext is {ext}");
                    }

                }
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
    }
}
