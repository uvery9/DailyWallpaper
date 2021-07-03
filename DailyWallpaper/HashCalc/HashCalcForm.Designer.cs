
namespace DailyWallpaper.HashCalc
{
    partial class HashCalcForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hashPicBox = new System.Windows.Forms.PictureBox();
            this.hashTextBox = new System.Windows.Forms.TextBox();
            this.file1Panel = new System.Windows.Forms.Panel();
            this.file1CopyButton = new System.Windows.Forms.Button();
            this.file1SaveButton = new System.Windows.Forms.Button();
            this.file1BrowseButton = new System.Windows.Forms.Button();
            this.file1CalcButton = new System.Windows.Forms.Button();
            this.file1ClearButton = new System.Windows.Forms.Button();
            this.file1 = new System.Windows.Forms.TextBox();
            this.cRC64_1checkBox = new System.Windows.Forms.CheckBox();
            this.file1ProgressBar = new System.Windows.Forms.ProgressBar();
            this.cRC32_1checkBox = new System.Windows.Forms.CheckBox();
            this.mD5_1checkBox = new System.Windows.Forms.CheckBox();
            this.sha256_1checkBox = new System.Windows.Forms.CheckBox();
            this.sha1_1checkBox = new System.Windows.Forms.CheckBox();
            this.hashfile1TextBox = new System.Windows.Forms.TextBox();
            this.SHA256_1TextBox = new System.Windows.Forms.TextBox();
            this.SHA1_1TextBox = new System.Windows.Forms.TextBox();
            this.CRC64_1TextBox = new System.Windows.Forms.TextBox();
            this.CRC32_1TextBox = new System.Windows.Forms.TextBox();
            this.MD5_1TextBox = new System.Windows.Forms.TextBox();
            this.headMenuStrip = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.rememberWindowPositionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useUppercaseLettersInHashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.calculateHashOfTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.hashPicBox)).BeginInit();
            this.file1Panel.SuspendLayout();
            this.headMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // hashPicBox
            // 
            this.hashPicBox.Image = global::DailyWallpaper.Properties.Resources.draganddrop_long;
            this.hashPicBox.Location = new System.Drawing.Point(14, 35);
            this.hashPicBox.Name = "hashPicBox";
            this.hashPicBox.Size = new System.Drawing.Size(828, 245);
            this.hashPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.hashPicBox.TabIndex = 1;
            this.hashPicBox.TabStop = false;
            this.hashPicBox.Click += new System.EventHandler(this.file1BrowseButton_Click);
            this.hashPicBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragDrop);
            this.hashPicBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragEnter);
            // 
            // hashTextBox
            // 
            this.hashTextBox.Location = new System.Drawing.Point(14, 544);
            this.hashTextBox.Multiline = true;
            this.hashTextBox.Name = "hashTextBox";
            this.hashTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.hashTextBox.Size = new System.Drawing.Size(830, 216);
            this.hashTextBox.TabIndex = 5;
            this.hashTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hashTextBox_KeyDown);
            // 
            // file1Panel
            // 
            this.file1Panel.AllowDrop = true;
            this.file1Panel.Controls.Add(this.file1CopyButton);
            this.file1Panel.Controls.Add(this.file1SaveButton);
            this.file1Panel.Controls.Add(this.file1CalcButton);
            this.file1Panel.Controls.Add(this.file1ClearButton);
            this.file1Panel.Controls.Add(this.cRC64_1checkBox);
            this.file1Panel.Controls.Add(this.file1ProgressBar);
            this.file1Panel.Controls.Add(this.cRC32_1checkBox);
            this.file1Panel.Controls.Add(this.mD5_1checkBox);
            this.file1Panel.Controls.Add(this.sha256_1checkBox);
            this.file1Panel.Controls.Add(this.sha1_1checkBox);
            this.file1Panel.Controls.Add(this.SHA256_1TextBox);
            this.file1Panel.Controls.Add(this.SHA1_1TextBox);
            this.file1Panel.Controls.Add(this.CRC64_1TextBox);
            this.file1Panel.Controls.Add(this.CRC32_1TextBox);
            this.file1Panel.Controls.Add(this.MD5_1TextBox);
            this.file1Panel.Location = new System.Drawing.Point(12, 332);
            this.file1Panel.Name = "file1Panel";
            this.file1Panel.Size = new System.Drawing.Size(830, 206);
            this.file1Panel.TabIndex = 4;
            this.file1Panel.DragDrop += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragDrop);
            this.file1Panel.DragEnter += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragEnter);
            // 
            // file1CopyButton
            // 
            this.file1CopyButton.Location = new System.Drawing.Point(727, 24);
            this.file1CopyButton.Name = "file1CopyButton";
            this.file1CopyButton.Size = new System.Drawing.Size(75, 30);
            this.file1CopyButton.TabIndex = 7;
            this.file1CopyButton.Text = "Copy";
            this.file1CopyButton.UseVisualStyleBackColor = true;
            this.file1CopyButton.Click += new System.EventHandler(this.file1CopyButton_Click);
            // 
            // file1SaveButton
            // 
            this.file1SaveButton.Location = new System.Drawing.Point(727, 83);
            this.file1SaveButton.Name = "file1SaveButton";
            this.file1SaveButton.Size = new System.Drawing.Size(75, 30);
            this.file1SaveButton.TabIndex = 7;
            this.file1SaveButton.Text = "Save";
            this.file1SaveButton.UseVisualStyleBackColor = true;
            this.file1SaveButton.Click += new System.EventHandler(this.file1SaveButton_Click);
            // 
            // file1BrowseButton
            // 
            this.file1BrowseButton.Location = new System.Drawing.Point(752, 294);
            this.file1BrowseButton.Name = "file1BrowseButton";
            this.file1BrowseButton.Size = new System.Drawing.Size(92, 32);
            this.file1BrowseButton.TabIndex = 6;
            this.file1BrowseButton.Text = "Browse";
            this.file1BrowseButton.UseVisualStyleBackColor = true;
            this.file1BrowseButton.Click += new System.EventHandler(this.file1BrowseButton_Click);
            // 
            // file1CalcButton
            // 
            this.file1CalcButton.Location = new System.Drawing.Point(712, 141);
            this.file1CalcButton.Name = "file1CalcButton";
            this.file1CalcButton.Size = new System.Drawing.Size(98, 44);
            this.file1CalcButton.TabIndex = 5;
            this.file1CalcButton.Text = "Calculate";
            this.file1CalcButton.UseVisualStyleBackColor = true;
            this.file1CalcButton.Click += new System.EventHandler(this.file1CalcButton_Click);
            // 
            // file1ClearButton
            // 
            this.file1ClearButton.Location = new System.Drawing.Point(609, 150);
            this.file1ClearButton.Name = "file1ClearButton";
            this.file1ClearButton.Size = new System.Drawing.Size(64, 25);
            this.file1ClearButton.TabIndex = 5;
            this.file1ClearButton.Text = "Clear";
            this.file1ClearButton.UseVisualStyleBackColor = true;
            this.file1ClearButton.Click += new System.EventHandler(this.file1ClearButton_Click);
            // 
            // file1
            // 
            this.file1.BackColor = System.Drawing.SystemColors.Control;
            this.file1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.file1.Location = new System.Drawing.Point(14, 301);
            this.file1.Name = "file1";
            this.file1.Size = new System.Drawing.Size(67, 21);
            this.file1.TabIndex = 4;
            this.file1.Text = "File:";
            // 
            // cRC64_1checkBox
            // 
            this.cRC64_1checkBox.AutoSize = true;
            this.cRC64_1checkBox.Location = new System.Drawing.Point(458, 24);
            this.cRC64_1checkBox.Name = "cRC64_1checkBox";
            this.cRC64_1checkBox.Size = new System.Drawing.Size(79, 22);
            this.cRC64_1checkBox.TabIndex = 1;
            this.cRC64_1checkBox.Text = "CRC64";
            this.cRC64_1checkBox.UseVisualStyleBackColor = true;
            this.cRC64_1checkBox.CheckedChanged += new System.EventHandler(this.cRC64_1checkBox_CheckedChanged);
            // 
            // file1ProgressBar
            // 
            this.file1ProgressBar.Location = new System.Drawing.Point(16, 150);
            this.file1ProgressBar.Name = "file1ProgressBar";
            this.file1ProgressBar.Size = new System.Drawing.Size(570, 25);
            this.file1ProgressBar.TabIndex = 3;
            // 
            // cRC32_1checkBox
            // 
            this.cRC32_1checkBox.AutoSize = true;
            this.cRC32_1checkBox.Location = new System.Drawing.Point(524, 66);
            this.cRC32_1checkBox.Name = "cRC32_1checkBox";
            this.cRC32_1checkBox.Size = new System.Drawing.Size(79, 22);
            this.cRC32_1checkBox.TabIndex = 1;
            this.cRC32_1checkBox.Text = "CRC32";
            this.cRC32_1checkBox.UseVisualStyleBackColor = true;
            this.cRC32_1checkBox.CheckedChanged += new System.EventHandler(this.cRC32_1checkBox_CheckedChanged);
            // 
            // mD5_1checkBox
            // 
            this.mD5_1checkBox.AutoSize = true;
            this.mD5_1checkBox.Location = new System.Drawing.Point(11, 22);
            this.mD5_1checkBox.Name = "mD5_1checkBox";
            this.mD5_1checkBox.Size = new System.Drawing.Size(61, 22);
            this.mD5_1checkBox.TabIndex = 1;
            this.mD5_1checkBox.Text = "MD5";
            this.mD5_1checkBox.UseVisualStyleBackColor = true;
            this.mD5_1checkBox.CheckedChanged += new System.EventHandler(this.mD5_1checkBox_CheckedChanged);
            // 
            // sha256_1checkBox
            // 
            this.sha256_1checkBox.AutoSize = true;
            this.sha256_1checkBox.Location = new System.Drawing.Point(11, 109);
            this.sha256_1checkBox.Name = "sha256_1checkBox";
            this.sha256_1checkBox.Size = new System.Drawing.Size(88, 22);
            this.sha256_1checkBox.TabIndex = 1;
            this.sha256_1checkBox.Text = "SHA256";
            this.sha256_1checkBox.UseVisualStyleBackColor = true;
            this.sha256_1checkBox.CheckedChanged += new System.EventHandler(this.sha256_1checkBox_CheckedChanged);
            // 
            // sha1_1checkBox
            // 
            this.sha1_1checkBox.AutoSize = true;
            this.sha1_1checkBox.Location = new System.Drawing.Point(11, 66);
            this.sha1_1checkBox.Name = "sha1_1checkBox";
            this.sha1_1checkBox.Size = new System.Drawing.Size(70, 22);
            this.sha1_1checkBox.TabIndex = 1;
            this.sha1_1checkBox.Text = "SHA1";
            this.sha1_1checkBox.UseVisualStyleBackColor = true;
            this.sha1_1checkBox.CheckedChanged += new System.EventHandler(this.sha1_1checkBox_CheckedChanged);
            // 
            // hashfile1TextBox
            // 
            this.hashfile1TextBox.Location = new System.Drawing.Point(63, 298);
            this.hashfile1TextBox.Name = "hashfile1TextBox";
            this.hashfile1TextBox.Size = new System.Drawing.Size(683, 28);
            this.hashfile1TextBox.TabIndex = 0;
            this.hashfile1TextBox.TabStop = false;
            this.hashfile1TextBox.Text = "D:\\jared\\Videos\\movies\\The.Good.Doctor\\The.Good.Doctor.S04E02.1080p.WEB.H264-STRO" +
    "NTiUM.chs.eng.mp4\r\n";
            this.hashfile1TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hashfile1TextBox_KeyDown);
            // 
            // SHA256_1TextBox
            // 
            this.SHA256_1TextBox.BackColor = System.Drawing.SystemColors.Window;
            this.SHA256_1TextBox.Location = new System.Drawing.Point(104, 107);
            this.SHA256_1TextBox.Name = "SHA256_1TextBox";
            this.SHA256_1TextBox.ReadOnly = true;
            this.SHA256_1TextBox.Size = new System.Drawing.Size(608, 28);
            this.SHA256_1TextBox.TabIndex = 0;
            this.SHA256_1TextBox.Text = "86C507C836317BA18E3588155ED5F6E6CA6C1A6DE780C6873C87479A346D0CEF";
            this.SHA256_1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SHA1_1TextBox
            // 
            this.SHA1_1TextBox.BackColor = System.Drawing.SystemColors.Window;
            this.SHA1_1TextBox.Location = new System.Drawing.Point(104, 64);
            this.SHA1_1TextBox.Name = "SHA1_1TextBox";
            this.SHA1_1TextBox.ReadOnly = true;
            this.SHA1_1TextBox.Size = new System.Drawing.Size(408, 28);
            this.SHA1_1TextBox.TabIndex = 0;
            this.SHA1_1TextBox.Text = "FEF9B281F46996777B94C2F8D6031091990C5142";
            this.SHA1_1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CRC64_1TextBox
            // 
            this.CRC64_1TextBox.BackColor = System.Drawing.SystemColors.Window;
            this.CRC64_1TextBox.Location = new System.Drawing.Point(538, 22);
            this.CRC64_1TextBox.Name = "CRC64_1TextBox";
            this.CRC64_1TextBox.ReadOnly = true;
            this.CRC64_1TextBox.Size = new System.Drawing.Size(174, 28);
            this.CRC64_1TextBox.TabIndex = 0;
            this.CRC64_1TextBox.Text = "2075A693782B44F0";
            this.CRC64_1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CRC32_1TextBox
            // 
            this.CRC32_1TextBox.BackColor = System.Drawing.SystemColors.Window;
            this.CRC32_1TextBox.Location = new System.Drawing.Point(609, 64);
            this.CRC32_1TextBox.Name = "CRC32_1TextBox";
            this.CRC32_1TextBox.ReadOnly = true;
            this.CRC32_1TextBox.Size = new System.Drawing.Size(103, 28);
            this.CRC32_1TextBox.TabIndex = 0;
            this.CRC32_1TextBox.Text = "EEB3C00D";
            this.CRC32_1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MD5_1TextBox
            // 
            this.MD5_1TextBox.BackColor = System.Drawing.SystemColors.Window;
            this.MD5_1TextBox.Location = new System.Drawing.Point(104, 20);
            this.MD5_1TextBox.Name = "MD5_1TextBox";
            this.MD5_1TextBox.ReadOnly = true;
            this.MD5_1TextBox.Size = new System.Drawing.Size(327, 28);
            this.MD5_1TextBox.TabIndex = 0;
            this.MD5_1TextBox.Text = "22D59D8E90DE3F0E0BE5BF5A0128E53F";
            this.MD5_1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // headMenuStrip
            // 
            this.headMenuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.headMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.headMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.optionsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.headMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.headMenuStrip.Name = "headMenuStrip";
            this.headMenuStrip.Size = new System.Drawing.Size(854, 32);
            this.headMenuStrip.TabIndex = 6;
            this.headMenuStrip.Text = "menuStrip1";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alwaysOnTopToolStripMenuItem,
            this.toolStripMenuItem1,
            this.rememberWindowPositionToolStripMenuItem,
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem,
            this.useUppercaseLettersInHashToolStripMenuItem,
            this.calculateHashOfTextToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(95, 28);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // alwaysOnTopToolStripMenuItem
            // 
            this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
            this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.alwaysOnTopToolStripMenuItem.Text = "Always on Top";
            this.alwaysOnTopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOnTopToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(529, 6);
            // 
            // rememberWindowPositionToolStripMenuItem
            // 
            this.rememberWindowPositionToolStripMenuItem.Name = "rememberWindowPositionToolStripMenuItem";
            this.rememberWindowPositionToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.rememberWindowPositionToolStripMenuItem.Text = "Remember window position";
            // 
            // automaticallyCalculateHashAfterDragAndDropToolStripMenuItem
            // 
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Name = "automaticallyCalculateHashAfterDragAndDropToolStripMenuItem";
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Text = "Automatically calculate hash after drag and drop";
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Click += new System.EventHandler(this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem_Click);
            // 
            // useUppercaseLettersInHashToolStripMenuItem
            // 
            this.useUppercaseLettersInHashToolStripMenuItem.Name = "useUppercaseLettersInHashToolStripMenuItem";
            this.useUppercaseLettersInHashToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.useUppercaseLettersInHashToolStripMenuItem.Text = "Use upper-case letters in hash";
            // 
            // calculateHashOfTextToolStripMenuItem
            // 
            this.calculateHashOfTextToolStripMenuItem.Name = "calculateHashOfTextToolStripMenuItem";
            this.calculateHashOfTextToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.calculateHashOfTextToolStripMenuItem.Text = "Calculate hash of TextBox";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.donateToolStripMenuItem,
            this.usageToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(67, 28);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // donateToolStripMenuItem
            // 
            this.donateToolStripMenuItem.Name = "donateToolStripMenuItem";
            this.donateToolStripMenuItem.Size = new System.Drawing.Size(173, 34);
            this.donateToolStripMenuItem.Text = "Donate";
            // 
            // usageToolStripMenuItem
            // 
            this.usageToolStripMenuItem.Name = "usageToolStripMenuItem";
            this.usageToolStripMenuItem.Size = new System.Drawing.Size(173, 34);
            this.usageToolStripMenuItem.Text = "Usage";
            // 
            // HashCalcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 768);
            this.Controls.Add(this.hashfile1TextBox);
            this.Controls.Add(this.hashTextBox);
            this.Controls.Add(this.hashPicBox);
            this.Controls.Add(this.file1BrowseButton);
            this.Controls.Add(this.file1Panel);
            this.Controls.Add(this.file1);
            this.Controls.Add(this.headMenuStrip);
            this.MainMenuStrip = this.headMenuStrip;
            this.Name = "HashCalcForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hash Calculator";
            ((System.ComponentModel.ISupportInitialize)(this.hashPicBox)).EndInit();
            this.file1Panel.ResumeLayout(false);
            this.file1Panel.PerformLayout();
            this.headMenuStrip.ResumeLayout(false);
            this.headMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox hashPicBox;
        private System.Windows.Forms.TextBox hashTextBox;
        private System.Windows.Forms.Panel file1Panel;
        private System.Windows.Forms.Button file1CopyButton;
        private System.Windows.Forms.Button file1SaveButton;
        private System.Windows.Forms.Button file1BrowseButton;
        private System.Windows.Forms.Button file1CalcButton;
        private System.Windows.Forms.Button file1ClearButton;
        private System.Windows.Forms.TextBox file1;
        private System.Windows.Forms.CheckBox cRC64_1checkBox;
        private System.Windows.Forms.ProgressBar file1ProgressBar;
        private System.Windows.Forms.CheckBox cRC32_1checkBox;
        private System.Windows.Forms.CheckBox mD5_1checkBox;
        private System.Windows.Forms.CheckBox sha256_1checkBox;
        private System.Windows.Forms.CheckBox sha1_1checkBox;
        private System.Windows.Forms.TextBox hashfile1TextBox;
        private System.Windows.Forms.TextBox SHA256_1TextBox;
        private System.Windows.Forms.TextBox SHA1_1TextBox;
        private System.Windows.Forms.TextBox CRC64_1TextBox;
        private System.Windows.Forms.TextBox CRC32_1TextBox;
        private System.Windows.Forms.TextBox MD5_1TextBox;
        private System.Windows.Forms.MenuStrip headMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem rememberWindowPositionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem automaticallyCalculateHashAfterDragAndDropToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useUppercaseLettersInHashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem calculateHashOfTextToolStripMenuItem;
    }
}