using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.HashCalc
{
    public partial class HashCalcForm : Form
    {
        private HashCalc m_hashCalc;
        public HashCalcForm()
        {
            InitializeComponent();
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = Properties.Resources.HASH32x32;
            hashPicBox.AllowDrop = true;
            m_hashCalc = new HashCalc();
            EnableAllHashCheckBoxAndTextBox();
            hashTextBox.Text = m_hashCalc.help;
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

                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                    foreach (string fileLoc in filePaths)
                    {
                        // Code to read the contents of the text file
                        if (File.Exists(fileLoc))
                        {
                            /*                            using (TextReader tr = new StreamReader(fileLoc))
                                                        {
                                                            MessageBox.Show(tr.ReadToEnd());
                                                        }*/
                            this.TopMost = false;
                            MessageBox.Show(fileLoc);
                        }

                    }
                }
            } else
            {
                this.Cursor = System.Windows.Forms.Cursors.No;
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
                        m_hashCalc.file1Path = fileLoc;
                        hashfile1TextBox.Text = fileLoc;
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

        private void file2Panel_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] filePaths = (string[])(e.Data.GetData(DataFormats.FileDrop));
                foreach (string fileLoc in filePaths)
                {
                    if (File.Exists(fileLoc))
                    {
                        m_hashCalc.file2Path = fileLoc;
                        file2TextBox.Text = fileLoc;
                    }

                }
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
                    m_hashCalc.file1Path = hashfile1TextBox.Text;
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
                m_hashCalc.file1Path = path;
                hashfile1TextBox.Text = path;
            }
        }

        private void file2BrowseButton_Click(object sender, EventArgs e)
        {
            var path = HashOpenFileDialog(file2TextBox.Text);
            if (!string.IsNullOrEmpty(path))
            {
                m_hashCalc.file2Path = path;
                file2TextBox.Text = path;
            }
        }

        private void file2TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(file2TextBox.Text))
            {
                if (File.Exists(file2TextBox.Text))
                {
                    m_hashCalc.file1Path = file2TextBox.Text;
                }
            }
        }

        private string CopyOrSaveInfoFile1()
        {
            return CopyOrSaveInfo(m_hashCalc.file1Path,
                                  MD5_1TextBox.Text,
                                  CRC32_1TextBox.Text,
                                  CRC64_1TextBox.Text,
                                  SHA1_1TextBox.Text,
                                  SHA256_1TextBox.Text);
        }
        private string CopyOrSaveInfoFile2()
        {
            return CopyOrSaveInfo(m_hashCalc.file2Path,
                                  MD5_2TextBox.Text,
                                  CRC32_2TextBox.Text,
                                  CRC64_2TextBox.Text,
                                  SHA1_2TextBox.Text,
                                  SHA256_2TextBox.Text);            
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

        private bool file1Calculating = false;
        private bool file2Calculating = false;
        private CancellationTokenSource file1Cancel;
        private CancellationTokenSource file2Cancel;

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

            mD5_2checkBox.Checked = true;
            MD5_2TextBox.Enabled = true;

            cRC64_2checkBox.Checked = true;
            CRC64_2TextBox.Enabled = true;

            sha1_2checkBox.Checked = true;
            SHA1_2TextBox.Enabled = true;

            cRC32_2checkBox.Checked = true;
            CRC32_2TextBox.Enabled = true;

            sha256_2checkBox.Checked = true;
            SHA256_2TextBox.Enabled = true;

            MD5_1TextBox.TabStop = false;
            CRC64_1TextBox.TabStop = false;
            SHA1_1TextBox.TabStop = false;
            CRC32_1TextBox.TabStop = false;
            SHA256_1TextBox.TabStop = false;
            MD5_2TextBox.TabStop = false;
            CRC64_2TextBox.TabStop = false;
            SHA1_2TextBox.TabStop = false;
            CRC32_2TextBox.TabStop = false;
            SHA256_2TextBox.TabStop = false;
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

        private void file2CopyButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(CopyOrSaveInfoFile2());
        }

        private void file1SaveButton_Click(object sender, EventArgs e)
        {
            save2File(m_hashCalc.file1Path, CopyOrSaveInfoFile1());
        }

        private void file2SaveButton_Click(object sender, EventArgs e)
        {
            save2File(m_hashCalc.file2Path, CopyOrSaveInfoFile2());
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

        private void mD5_2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(mD5_2checkBox, MD5_2TextBox);
        }

        private void cRC64_2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(cRC64_2checkBox, CRC64_2TextBox);
        }

        private void sha1_2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(sha1_2checkBox, SHA1_2TextBox);
        }

        private void cRC32_2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(cRC32_2checkBox, CRC32_2TextBox);
        }

        private void sha256_2checkBox_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxAffectTextBox(sha256_2checkBox, SHA256_2TextBox);
        }
        
        public void Calculate(string file, CancellationToken token, bool crc32, bool crc64,
            bool md5, bool sha1, bool sha256)
        {
            if (crc32)  m_hashCalc.CRC32(file, token);
            if (crc64)  m_hashCalc.CRC64(file, token);
            if (md5)    m_hashCalc.MD5(file, token);
            if (sha1)   m_hashCalc.SHA1(file, token);
            if (sha256) m_hashCalc.SHA256(file, token);
        }



        private void file1CalcButton_Click(object sender, EventArgs e)
        {
            file1Cancel = new CancellationTokenSource();
            file1CalcButton.Enabled = false;
            Calculate(m_hashCalc.file1Path, file1Cancel.Token, cRC32_1checkBox.Checked, cRC64_1checkBox.Checked,
                mD5_1checkBox.Checked, sha1_1checkBox.Checked, sha256_1checkBox.Checked);
            file1CalcButton.Enabled = true;
            // file2Cancel.Token;
        }
        private void file2CalcButton_Click(object sender, EventArgs e)
        {
            file2Cancel = new CancellationTokenSource();
            file2CalcButton.Enabled = false;
            Calculate(m_hashCalc.file2Path, file2Cancel.Token, cRC32_2checkBox.Checked, cRC64_2checkBox.Checked,
                mD5_2checkBox.Checked, sha1_2checkBox.Checked, sha256_2checkBox.Checked);
            file2CalcButton.Enabled = true;
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
            m_hashCalc.file1Path = "";
            if (file1Calculating)
            {
                if (file1Cancel != null)
                {
                    file1Cancel.Cancel();
                }
            }
        }

        private void file2ClearButton_Click(object sender, EventArgs e)
        {
            MD5_2TextBox.Text = "";
            CRC32_2TextBox.Text = "";
            CRC64_2TextBox.Text = "";
            SHA1_2TextBox.Text = "";
            SHA256_2TextBox.Text = "";
            file2TextBox.Text = "";
            m_hashCalc.file2Path = "";
            if (file2Calculating)
            {
                if (file2Cancel != null)
                {
                    file2Cancel.Cancel();
                }
            }
        }
    }
}
