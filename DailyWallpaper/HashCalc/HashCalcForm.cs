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
        private HashCalc m_hashCalc1;
        private HashCalc m_hashCalc2;
        public HashCalcForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.HASH32x32;
            hashPicBox.AllowDrop = true;
            m_hashCalc1 = new HashCalc();
            m_hashCalc2 = new HashCalc();
            m_hashCalc1.hashProgressBar = file1ProgressBar;
            EnableAllHashCheckBoxAndTextBox();
            hashTextBox.Text = m_hashCalc1.help;
            //

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

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

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
                        m_hashCalc1.filePath = fileLoc;
                        hashfile1TextBox.Text = fileLoc;
                        file1CalcButton.PerformClick(); // Pretend to be clicked.
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

        private void hashfile1TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(hashfile1TextBox.Text))
            {
                if (File.Exists(hashfile1TextBox.Text)){
                    m_hashCalc1.filePath = hashfile1TextBox.Text;
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
        private void file1BrowseButton_Click(object sender, EventArgs e)
        {
            var path = HashOpenFileDialog(hashfile1TextBox.Text);
            if (!string.IsNullOrEmpty(path))
            {
                m_hashCalc1.filePath = path;
                hashfile1TextBox.Text = path;
            }
        }

        private string CopyOrSaveInfoFile1()
        {
            return CopyOrSaveInfo(m_hashCalc1.filePath,
                                  MD5_1TextBox.Text,
                                  CRC32_1TextBox.Text,
                                  CRC64_1TextBox.Text,
                                  SHA1_1TextBox.Text,
                                  SHA256_1TextBox.Text);
        }

        private string CopyOrSaveInfo(string name, string md5, string crc32, string crc64, string sha1, string sha256)
        {

            string ret = "";
            string size;

            try
            {
                size = (new FileInfo(name).Length / 1024).ToString();
            }
            catch
            {
                name = "NULL";
                size = "None";
            }
            ret += "File:   "   +  name + "\r\n";
            ret += "Size:   "   +  size + " KB\r\n";
           
            ret += "CRC32:  "  + (string.IsNullOrEmpty(crc32) ? "" : crc32) + "\r\n";
            ret += "CRC64:  "  + (string.IsNullOrEmpty(crc64) ? "" : crc64) + "\r\n";
            ret += "MD5:    " + (string.IsNullOrEmpty(md5) ? "" : md5) + "\r\n";
            ret += "SHA1:   "   + (string.IsNullOrEmpty(sha1) ? "" : sha1) + "\r\n";
            ret += "SHA256: " + (string.IsNullOrEmpty(sha256) ? "" : sha256) + "\r\n";
            ret += "    Paste from Hash Calculator in DailyWallpaper.\r\n";

            return ret;
        }

        private CancellationTokenSource file1Cancel;
        private void EnableAllHashCheckBoxAndTextBox()
        {
            mD5_1checkBox.Checked = true;
            MD5_1TextBox.Enabled = true;

            cRC64_1checkBox.Checked = true;
            CRC64_1TextBox.Enabled = true;

            sha1_1checkBox.Checked = true;
            SHA1_1TextBox.Enabled = true;

            cRC32_1checkBox.Checked = true;
            CRC32_1TextBox.Enabled = true;

            sha256_1checkBox.Checked = true;
            SHA256_1TextBox.Enabled = true;

            MD5_1TextBox.TabStop = false;
            CRC64_1TextBox.TabStop = false;
            SHA1_1TextBox.TabStop = false;
            CRC32_1TextBox.TabStop = false;
            SHA256_1TextBox.TabStop = false;
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
        private void file1CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(CopyOrSaveInfoFile1());
        }

        private void file1SaveButton_Click(object sender, EventArgs e)
        {
            save2File(m_hashCalc1.filePath, CopyOrSaveInfoFile1());
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
        private void mD5_1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(mD5_1checkBox, MD5_1TextBox);
        }

        private void cRC64_1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(cRC64_1checkBox, CRC64_1TextBox);
        }

        private void sha1_1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(sha1_1checkBox, SHA1_1TextBox);
        }

        private void cRC32_1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(cRC32_1checkBox, CRC32_1TextBox);
        }

        private void sha256_1checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(sha256_1checkBox, SHA256_1TextBox);
        }
       

        private Dictionary<string, string> Calculate(HashCalc hashcalc, CancellationToken token, bool crc32, bool crc64,
            bool md5, bool sha1, bool sha256)
        {
            var hashList = new List<string>();
            void TellResultAsync(string who, string result, string costTime)
            {
                hashTextBox.Text += "\r\n" + who + ": " + result + " " + costTime;
            }
            var dict = new Dictionary<string, string>();
            if (crc32) 
            {
                var res = hashcalc.CalcCRC32(hashcalc.filePath, token);
                dict.Add("CRC32", res);
            }
            if (crc64)
            {
                var res = hashcalc.CalcCRC64(hashcalc.filePath, token);
                dict.Add("CRC64", res);
            }
            if (md5)
            {
                hashcalc.CalcMD5(hashcalc.filePath, TellResultAsync, token);
                // dict.Add("MD5", res);
            }
            if (sha1)
            {
                //var res = hashcalc.CalcSHA1(hashcalc.filePath, token);
                // hashcalc.CalcSHA1(hashcalc.filePath, GetString, token);
                //dict.Add("SHA1", res);
            }

            if (sha256)
            {
                //var res = hashcalc.CalcSHA256(hashcalc.filePath, token);
               // dict.Add("SHA256", res);
            }
            return dict;
        }

        private void file1CalcButton_Click(object sender, EventArgs e)
        {
            file1Cancel = new CancellationTokenSource();
            file1CalcButton.Enabled = false;
            var dict = Calculate(m_hashCalc1, file1Cancel.Token, cRC32_1checkBox.Checked, cRC64_1checkBox.Checked,
                mD5_1checkBox.Checked, sha1_1checkBox.Checked, sha256_1checkBox.Checked);

            if (cRC32_1checkBox.Checked)
            {
                var s = dict["CRC32"];
                CRC32_1TextBox.Text = s ?? "";

            }
            
            if (cRC64_1checkBox.Checked)
            {
                var s = dict["CRC64"];
                CRC64_1TextBox.Text = s ?? "";
            }

            if (mD5_1checkBox.Checked)
            {
                var s = dict["MD5"];
                MD5_1TextBox.Text = s ?? "";
            }

            if (sha1_1checkBox.Checked)
            {
                var s = dict["SHA1"];
                SHA1_1TextBox.Text = s ?? "";
            }
            
            if (sha256_1checkBox.Checked)
            {
                var s = dict["SHA256"];
                SHA256_1TextBox.Text = s ?? "";
            }
            file1CalcButton.Enabled = true;
            // file2Cancel.Token;
        }

        private void file1ClearButton_Click(object sender, EventArgs e)
        {
            MD5_1TextBox.Text = "";
            CRC32_1TextBox.Text = "";
            CRC64_1TextBox.Text = "";
            SHA1_1TextBox.Text = "";
            SHA256_1TextBox.Text = "";
            hashfile1TextBox.Text = "";
            m_hashCalc1.filePath = "";

            if (file1Cancel != null)
            {
                file1Cancel.Cancel();
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
      /* private void file1HashbackGroundWorker_DoWork(object sender, DoWorkEventArgs e)
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
       *                 file1HashbackGroundWorker.ReportProgress((int)((double)totalBytesRead / size * 100));
       *
       *
       *             } while (bytesRead != 0);
       *             hasher.TransformFinalBlock(buffer, 0, 0);
       *             e.Result = MakeHashString(hasher.Hash);
       *         }
       *     }
       * }
        */

    /*        private void file1HashbackGroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
     *   {
     *       file1ProgressBar.Value = e.ProgressPercentage;
     *   }       private void file1HashbackGroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
     *   {
     *       // MessageBox.Show(e.Result.ToString());
     *       hashTextBox.Text = e.Result.ToString();
     *       file1ProgressBar.Value = 0;
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
    }
}
