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
        private HashCalc hashCalc;
        private TextBoxCons _console;
        public HashCalcForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.HASH32x32;
            hashPicBox.AllowDrop = true;
            hashCalc = new HashCalc();
            hashCalc.hashProgressBar = fileProgressBar;
            EnableAllHashCheckBoxAndTextBox();
            _console = new TextBoxCons(new ConsWriter(hashTextBox));
            _console.WriteLine(hashCalc.help);

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
                        hashCalc.filePath = fileLoc;
                        hashfileTextBox.Text = fileLoc;
                        fileCalcButton.PerformClick(); // Pretend to be clicked.
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
                    hashCalc.filePath = hashfileTextBox.Text;
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
                hashCalc.filePath = path;
                hashfileTextBox.Text = path;
            }
        }

        private string CopyOrSaveInfoFile1()
        {
            return CopyOrSaveInfo(hashCalc.filePath,
                                  MD5TextBox.Text,
                                  CRC32TextBox.Text,
                                  CRC64TextBox.Text,
                                  SHA1TextBox.Text,
                                  SHA256TextBox.Text);
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
            Clipboard.SetText(CopyOrSaveInfoFile1());
        }

        private void fileSaveButton_Click(object sender, EventArgs e)
        {
            save2File(hashCalc.filePath, CopyOrSaveInfoFile1());
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

        private void fileCalcButton_Click(object sender, EventArgs e)
        {
            fileCancel = new CancellationTokenSource();
            fileCalcButton.Enabled = false;
            var dict = Calculate(hashCalc, fileCancel.Token, CRC32checkBox.Checked, CRC64checkBox.Checked,
                MD5checkBox.Checked, SHA1checkBox.Checked, SHA256checkBox.Checked);

            if (CRC32checkBox.Checked)
            {
                var s = dict["CRC32"];
                CRC32TextBox.Text = s ?? "";

            }
            
            if (CRC64checkBox.Checked)
            {
                var s = dict["CRC64"];
                CRC64TextBox.Text = s ?? "";
            }

            if (MD5checkBox.Checked)
            {
                var s = dict["MD5"];
                MD5TextBox.Text = s ?? "";
            }

            if (SHA1checkBox.Checked)
            {
                var s = dict["SHA1"];
                SHA1TextBox.Text = s ?? "";
            }
            
            if (SHA256checkBox.Checked)
            {
                var s = dict["SHA256"];
                SHA256TextBox.Text = s ?? "";
            }
            fileCalcButton.Enabled = true;
            // file2Cancel.Token;
        }

        private void fileClearButton_Click(object sender, EventArgs e)
        {
            MD5TextBox.Text = "";
            CRC32TextBox.Text = "";
            CRC64TextBox.Text = "";
            SHA1TextBox.Text = "";
            SHA256TextBox.Text = "";
            hashfileTextBox.Text = "";
            hashCalc.filePath = "";

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

        private void button2_Click(object sender, EventArgs e)
        {
            MD5TextBox.Text = "";
            CRC32TextBox.Text = "";
            CRC64TextBox.Text = "";
            SHA1TextBox.Text = "";
            SHA256TextBox.Text = "";
            hashfileTextBox.Text = "";
            hashCalc.filePath = "";

            if (fileCancel != null)
            {
                fileCancel.Cancel();
            }
        }
    }
}
