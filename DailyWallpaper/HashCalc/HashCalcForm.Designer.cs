
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
            this.filePanel = new System.Windows.Forms.Panel();
            this.CRC64TextBox = new System.Windows.Forms.TextBox();
            this.hashStringCheckBox = new System.Windows.Forms.CheckBox();
            this.fileCopyButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.fileSaveButton = new System.Windows.Forms.Button();
            this.hashCalcButton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.CRC64checkBox = new System.Windows.Forms.CheckBox();
            this.fileProgressBar = new System.Windows.Forms.ProgressBar();
            this.CRC32checkBox = new System.Windows.Forms.CheckBox();
            this.MD5checkBox = new System.Windows.Forms.CheckBox();
            this.SHA512checkBox = new System.Windows.Forms.CheckBox();
            this.SHA256checkBox = new System.Windows.Forms.CheckBox();
            this.SHA1checkBox = new System.Windows.Forms.CheckBox();
            this.SHA512TextBox = new System.Windows.Forms.TextBox();
            this.SHA256TextBox = new System.Windows.Forms.TextBox();
            this.SHA1TextBox = new System.Windows.Forms.TextBox();
            this.CRC32TextBox = new System.Windows.Forms.TextBox();
            this.MD5TextBox = new System.Windows.Forms.TextBox();
            this.fileBrowseButton = new System.Windows.Forms.Button();
            this.file = new System.Windows.Forms.TextBox();
            this.hashfileTextBox = new System.Windows.Forms.TextBox();
            this.headMenuStrip = new System.Windows.Forms.MenuStrip();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.consoleTextBoxAllowDropToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.useUppercaseLettersInHashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.donateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.hashPicBox)).BeginInit();
            this.filePanel.SuspendLayout();
            this.headMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // hashPicBox
            // 
            this.hashPicBox.Location = new System.Drawing.Point(41, 50);
            this.hashPicBox.Name = "hashPicBox";
            this.hashPicBox.Size = new System.Drawing.Size(775, 233);
            this.hashPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.hashPicBox.TabIndex = 1;
            this.hashPicBox.TabStop = false;
            this.hashPicBox.Click += new System.EventHandler(this.fileBrowseButton_Click);
            this.hashPicBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragDrop);
            this.hashPicBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragEnter);
            // 
            // hashTextBox
            // 
            this.hashTextBox.AllowDrop = true;
            this.hashTextBox.Location = new System.Drawing.Point(14, 572);
            this.hashTextBox.Multiline = true;
            this.hashTextBox.Name = "hashTextBox";
            this.hashTextBox.ReadOnly = true;
            this.hashTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.hashTextBox.Size = new System.Drawing.Size(828, 211);
            this.hashTextBox.TabIndex = 5;
            this.hashTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.hashTextBox_DragDrop);
            this.hashTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragEnter);
            // 
            // filePanel
            // 
            this.filePanel.AllowDrop = true;
            this.filePanel.Controls.Add(this.CRC64TextBox);
            this.filePanel.Controls.Add(this.hashStringCheckBox);
            this.filePanel.Controls.Add(this.fileCopyButton);
            this.filePanel.Controls.Add(this.clearButton);
            this.filePanel.Controls.Add(this.stopButton);
            this.filePanel.Controls.Add(this.fileSaveButton);
            this.filePanel.Controls.Add(this.hashCalcButton);
            this.filePanel.Controls.Add(this.button1);
            this.filePanel.Controls.Add(this.CRC64checkBox);
            this.filePanel.Controls.Add(this.fileProgressBar);
            this.filePanel.Controls.Add(this.CRC32checkBox);
            this.filePanel.Controls.Add(this.MD5checkBox);
            this.filePanel.Controls.Add(this.SHA512checkBox);
            this.filePanel.Controls.Add(this.SHA256checkBox);
            this.filePanel.Controls.Add(this.SHA1checkBox);
            this.filePanel.Controls.Add(this.SHA512TextBox);
            this.filePanel.Controls.Add(this.SHA256TextBox);
            this.filePanel.Controls.Add(this.SHA1TextBox);
            this.filePanel.Controls.Add(this.CRC32TextBox);
            this.filePanel.Controls.Add(this.MD5TextBox);
            this.filePanel.Location = new System.Drawing.Point(14, 337);
            this.filePanel.Name = "filePanel";
            this.filePanel.Size = new System.Drawing.Size(828, 229);
            this.filePanel.TabIndex = 4;
            this.filePanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragDrop);
            this.filePanel.DragEnter += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragEnter);
            // 
            // CRC64TextBox
            // 
            this.CRC64TextBox.BackColor = System.Drawing.SystemColors.Control;
            this.CRC64TextBox.Location = new System.Drawing.Point(538, 20);
            this.CRC64TextBox.Name = "CRC64TextBox";
            this.CRC64TextBox.ReadOnly = true;
            this.CRC64TextBox.Size = new System.Drawing.Size(174, 28);
            this.CRC64TextBox.TabIndex = 0;
            this.CRC64TextBox.Text = "2075A693782B44F0";
            this.CRC64TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // hashStringCheckBox
            // 
            this.hashStringCheckBox.AutoSize = true;
            this.hashStringCheckBox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.hashStringCheckBox.Location = new System.Drawing.Point(733, 143);
            this.hashStringCheckBox.Name = "hashStringCheckBox";
            this.hashStringCheckBox.Size = new System.Drawing.Size(88, 22);
            this.hashStringCheckBox.TabIndex = 8;
            this.hashStringCheckBox.Text = "String";
            this.hashStringCheckBox.UseVisualStyleBackColor = true;
            this.hashStringCheckBox.Click += new System.EventHandler(this.hashStringCheckBox_Click);
            // 
            // fileCopyButton
            // 
            this.fileCopyButton.Location = new System.Drawing.Point(750, 3);
            this.fileCopyButton.Name = "fileCopyButton";
            this.fileCopyButton.Size = new System.Drawing.Size(75, 30);
            this.fileCopyButton.TabIndex = 7;
            this.fileCopyButton.Text = "Copy";
            this.fileCopyButton.UseVisualStyleBackColor = true;
            this.fileCopyButton.Click += new System.EventHandler(this.fileCopyButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(11, 191);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(70, 35);
            this.clearButton.TabIndex = 7;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(750, 101);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(75, 30);
            this.stopButton.TabIndex = 7;
            this.stopButton.Text = "Stop";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // fileSaveButton
            // 
            this.fileSaveButton.Location = new System.Drawing.Point(750, 53);
            this.fileSaveButton.Name = "fileSaveButton";
            this.fileSaveButton.Size = new System.Drawing.Size(75, 30);
            this.fileSaveButton.TabIndex = 7;
            this.fileSaveButton.Text = "Save";
            this.fileSaveButton.UseVisualStyleBackColor = true;
            this.fileSaveButton.Click += new System.EventHandler(this.fileSaveButton_Click);
            // 
            // hashCalcButton
            // 
            this.hashCalcButton.Location = new System.Drawing.Point(718, 177);
            this.hashCalcButton.Name = "hashCalcButton";
            this.hashCalcButton.Size = new System.Drawing.Size(110, 49);
            this.hashCalcButton.TabIndex = 5;
            this.hashCalcButton.Text = "Calculate";
            this.hashCalcButton.UseVisualStyleBackColor = true;
            this.hashCalcButton.Click += new System.EventHandler(this.hashCalcButton_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(-726, 181);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(64, 25);
            this.button1.TabIndex = 5;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // CRC64checkBox
            // 
            this.CRC64checkBox.AutoSize = true;
            this.CRC64checkBox.Location = new System.Drawing.Point(437, 22);
            this.CRC64checkBox.Name = "CRC64checkBox";
            this.CRC64checkBox.Size = new System.Drawing.Size(106, 22);
            this.CRC64checkBox.TabIndex = 1;
            this.CRC64checkBox.Text = "CRC64ISO";
            this.CRC64checkBox.UseVisualStyleBackColor = true;
            this.CRC64checkBox.Click += new System.EventHandler(this.CRC64checkBox_Click);
            // 
            // fileProgressBar
            // 
            this.fileProgressBar.Location = new System.Drawing.Point(104, 194);
            this.fileProgressBar.Name = "fileProgressBar";
            this.fileProgressBar.Size = new System.Drawing.Size(608, 32);
            this.fileProgressBar.TabIndex = 3;
            // 
            // CRC32checkBox
            // 
            this.CRC32checkBox.AutoSize = true;
            this.CRC32checkBox.Location = new System.Drawing.Point(524, 66);
            this.CRC32checkBox.Name = "CRC32checkBox";
            this.CRC32checkBox.Size = new System.Drawing.Size(79, 22);
            this.CRC32checkBox.TabIndex = 1;
            this.CRC32checkBox.Text = "CRC32";
            this.CRC32checkBox.UseVisualStyleBackColor = true;
            this.CRC32checkBox.Click += new System.EventHandler(this.CRC32checkBox_Click);
            // 
            // MD5checkBox
            // 
            this.MD5checkBox.AutoSize = true;
            this.MD5checkBox.Location = new System.Drawing.Point(11, 22);
            this.MD5checkBox.Name = "MD5checkBox";
            this.MD5checkBox.Size = new System.Drawing.Size(61, 22);
            this.MD5checkBox.TabIndex = 1;
            this.MD5checkBox.Text = "MD5";
            this.MD5checkBox.UseVisualStyleBackColor = true;
            this.MD5checkBox.Click += new System.EventHandler(this.MD5checkBox_Click);
            // 
            // SHA512checkBox
            // 
            this.SHA512checkBox.AutoSize = true;
            this.SHA512checkBox.Location = new System.Drawing.Point(11, 150);
            this.SHA512checkBox.Name = "SHA512checkBox";
            this.SHA512checkBox.Size = new System.Drawing.Size(88, 22);
            this.SHA512checkBox.TabIndex = 1;
            this.SHA512checkBox.Text = "SHA512";
            this.SHA512checkBox.UseVisualStyleBackColor = true;
            this.SHA512checkBox.Click += new System.EventHandler(this.SHA512checkBox_Click);
            // 
            // SHA256checkBox
            // 
            this.SHA256checkBox.AutoSize = true;
            this.SHA256checkBox.Location = new System.Drawing.Point(11, 109);
            this.SHA256checkBox.Name = "SHA256checkBox";
            this.SHA256checkBox.Size = new System.Drawing.Size(88, 22);
            this.SHA256checkBox.TabIndex = 1;
            this.SHA256checkBox.Text = "SHA256";
            this.SHA256checkBox.UseVisualStyleBackColor = true;
            this.SHA256checkBox.Click += new System.EventHandler(this.SHA256checkBox_Click);
            // 
            // SHA1checkBox
            // 
            this.SHA1checkBox.AutoSize = true;
            this.SHA1checkBox.Location = new System.Drawing.Point(11, 66);
            this.SHA1checkBox.Name = "SHA1checkBox";
            this.SHA1checkBox.Size = new System.Drawing.Size(70, 22);
            this.SHA1checkBox.TabIndex = 1;
            this.SHA1checkBox.Text = "SHA1";
            this.SHA1checkBox.UseVisualStyleBackColor = true;
            this.SHA1checkBox.Click += new System.EventHandler(this.SHA1checkBox_Click);
            // 
            // SHA512TextBox
            // 
            this.SHA512TextBox.BackColor = System.Drawing.SystemColors.Control;
            this.SHA512TextBox.Location = new System.Drawing.Point(104, 141);
            this.SHA512TextBox.Multiline = true;
            this.SHA512TextBox.Name = "SHA512TextBox";
            this.SHA512TextBox.ReadOnly = true;
            this.SHA512TextBox.Size = new System.Drawing.Size(608, 44);
            this.SHA512TextBox.TabIndex = 0;
            this.SHA512TextBox.TabStop = false;
            this.SHA512TextBox.Text = "30163935C002FC4E1200906C3D30A9C4956B4AF9F6DCAEF1EB4B1FCB8FBA69E7A7ACDC491EA5B1F28" +
    "64EA8C01B01580EF09DEFC3B11B3F183CB21D236F7F1A6B";
            this.SHA512TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SHA256TextBox
            // 
            this.SHA256TextBox.BackColor = System.Drawing.SystemColors.Control;
            this.SHA256TextBox.Location = new System.Drawing.Point(104, 107);
            this.SHA256TextBox.Name = "SHA256TextBox";
            this.SHA256TextBox.ReadOnly = true;
            this.SHA256TextBox.Size = new System.Drawing.Size(608, 28);
            this.SHA256TextBox.TabIndex = 0;
            this.SHA256TextBox.TabStop = false;
            this.SHA256TextBox.Text = "86C507C836317BA18E3588155ED5F6E6CA6C1A6DE780C6873C87479A346D0CEF";
            this.SHA256TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // SHA1TextBox
            // 
            this.SHA1TextBox.BackColor = System.Drawing.SystemColors.Control;
            this.SHA1TextBox.Location = new System.Drawing.Point(104, 64);
            this.SHA1TextBox.Name = "SHA1TextBox";
            this.SHA1TextBox.ReadOnly = true;
            this.SHA1TextBox.Size = new System.Drawing.Size(408, 28);
            this.SHA1TextBox.TabIndex = 0;
            this.SHA1TextBox.Text = "FEF9B281F46996777B94C2F8D6031091990C5142";
            this.SHA1TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CRC32TextBox
            // 
            this.CRC32TextBox.BackColor = System.Drawing.SystemColors.Control;
            this.CRC32TextBox.Location = new System.Drawing.Point(609, 64);
            this.CRC32TextBox.Name = "CRC32TextBox";
            this.CRC32TextBox.ReadOnly = true;
            this.CRC32TextBox.Size = new System.Drawing.Size(103, 28);
            this.CRC32TextBox.TabIndex = 0;
            this.CRC32TextBox.Text = "EEB3C00D";
            this.CRC32TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MD5TextBox
            // 
            this.MD5TextBox.BackColor = System.Drawing.SystemColors.Control;
            this.MD5TextBox.Location = new System.Drawing.Point(104, 20);
            this.MD5TextBox.Name = "MD5TextBox";
            this.MD5TextBox.ReadOnly = true;
            this.MD5TextBox.Size = new System.Drawing.Size(327, 28);
            this.MD5TextBox.TabIndex = 0;
            this.MD5TextBox.Text = "22D59D8E90DE3F0E0BE5BF5A0128E53F";
            this.MD5TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // fileBrowseButton
            // 
            this.fileBrowseButton.Location = new System.Drawing.Point(747, 294);
            this.fileBrowseButton.Name = "fileBrowseButton";
            this.fileBrowseButton.Size = new System.Drawing.Size(92, 32);
            this.fileBrowseButton.TabIndex = 6;
            this.fileBrowseButton.Text = "Browse";
            this.fileBrowseButton.UseVisualStyleBackColor = true;
            this.fileBrowseButton.Click += new System.EventHandler(this.fileBrowseButton_Click);
            // 
            // file
            // 
            this.file.BackColor = System.Drawing.SystemColors.Control;
            this.file.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.file.Location = new System.Drawing.Point(14, 301);
            this.file.Name = "file";
            this.file.Size = new System.Drawing.Size(67, 21);
            this.file.TabIndex = 4;
            this.file.Text = "File:";
            // 
            // hashfileTextBox
            // 
            this.hashfileTextBox.AllowDrop = true;
            this.hashfileTextBox.Location = new System.Drawing.Point(62, 296);
            this.hashfileTextBox.Name = "hashfileTextBox";
            this.hashfileTextBox.Size = new System.Drawing.Size(683, 30);
            this.hashfileTextBox.TabIndex = 0;
            this.hashfileTextBox.TabStop = false;
            this.hashfileTextBox.Text = "D:\\jared\\Videos\\movies\\The.Good.Doctor\\The.Good.Doctor.S04E02.1080p.WEB.H264-STRO" +
    "NTiUM.chs.eng.mp4\r\n";
            this.hashfileTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragDrop);
            this.hashfileTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragEnter);
            this.hashfileTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.hashfileTextBox_KeyDown);
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
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem,
            this.consoleTextBoxAllowDropToolStripMenuItem,
            this.useUppercaseLettersInHashToolStripMenuItem});
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
            // automaticallyCalculateHashAfterDragAndDropToolStripMenuItem
            // 
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Name = "automaticallyCalculateHashAfterDragAndDropToolStripMenuItem";
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Text = "Automatically calculate hash after drag and drop";
            this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem.Click += new System.EventHandler(this.automaticallyCalculateHashAfterDragAndDropToolStripMenuItem_Click);
            // 
            // consoleTextBoxAllowDropToolStripMenuItem
            // 
            this.consoleTextBoxAllowDropToolStripMenuItem.Name = "consoleTextBoxAllowDropToolStripMenuItem";
            this.consoleTextBoxAllowDropToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.consoleTextBoxAllowDropToolStripMenuItem.Text = "Allow ConsoleTextBox Drop";
            this.consoleTextBoxAllowDropToolStripMenuItem.Click += new System.EventHandler(this.consoleTextBoxAllowDropToolStripMenuItem_Click);
            // 
            // useUppercaseLettersInHashToolStripMenuItem
            // 
            this.useUppercaseLettersInHashToolStripMenuItem.Name = "useUppercaseLettersInHashToolStripMenuItem";
            this.useUppercaseLettersInHashToolStripMenuItem.Size = new System.Drawing.Size(532, 34);
            this.useUppercaseLettersInHashToolStripMenuItem.Text = "Use lower-case in hash result";
            this.useUppercaseLettersInHashToolStripMenuItem.Click += new System.EventHandler(this.useUppercaseLettersInHashToolStripMenuItem_Click);
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
            this.donateToolStripMenuItem.Click += new System.EventHandler(this.donateToolStripMenuItem_Click);
            // 
            // usageToolStripMenuItem
            // 
            this.usageToolStripMenuItem.Name = "usageToolStripMenuItem";
            this.usageToolStripMenuItem.Size = new System.Drawing.Size(173, 34);
            this.usageToolStripMenuItem.Text = "Usage";
            this.usageToolStripMenuItem.Click += new System.EventHandler(this.usageToolStripMenuItem_Click);
            // 
            // HashCalcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 795);
            this.Controls.Add(this.hashfileTextBox);
            this.Controls.Add(this.hashTextBox);
            this.Controls.Add(this.hashPicBox);
            this.Controls.Add(this.fileBrowseButton);
            this.Controls.Add(this.filePanel);
            this.Controls.Add(this.file);
            this.Controls.Add(this.headMenuStrip);
            this.MainMenuStrip = this.headMenuStrip;
            this.Name = "HashCalcForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hash Calculator";
            ((System.ComponentModel.ISupportInitialize)(this.hashPicBox)).EndInit();
            this.filePanel.ResumeLayout(false);
            this.filePanel.PerformLayout();
            this.headMenuStrip.ResumeLayout(false);
            this.headMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox hashPicBox;
        private System.Windows.Forms.TextBox hashTextBox;
        private System.Windows.Forms.Panel filePanel;
        private System.Windows.Forms.Button fileCopyButton;
        private System.Windows.Forms.Button fileSaveButton;
        private System.Windows.Forms.Button fileBrowseButton;
        private System.Windows.Forms.Button hashCalcButton;
        private System.Windows.Forms.TextBox file;
        private System.Windows.Forms.CheckBox CRC64checkBox;
        private System.Windows.Forms.ProgressBar fileProgressBar;
        private System.Windows.Forms.CheckBox CRC32checkBox;
        private System.Windows.Forms.CheckBox MD5checkBox;
        private System.Windows.Forms.CheckBox SHA256checkBox;
        private System.Windows.Forms.CheckBox SHA1checkBox;
        private System.Windows.Forms.TextBox hashfileTextBox;
        private System.Windows.Forms.TextBox SHA256TextBox;
        private System.Windows.Forms.TextBox SHA1TextBox;
        private System.Windows.Forms.TextBox CRC64TextBox;
        private System.Windows.Forms.TextBox CRC32TextBox;
        private System.Windows.Forms.TextBox MD5TextBox;
        private System.Windows.Forms.MenuStrip headMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem automaticallyCalculateHashAfterDragAndDropToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem useUppercaseLettersInHashToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem donateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usageToolStripMenuItem;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox SHA512checkBox;
        private System.Windows.Forms.TextBox SHA512TextBox;
        private System.Windows.Forms.ToolStripMenuItem consoleTextBoxAllowDropToolStripMenuItem;
        private System.Windows.Forms.CheckBox hashStringCheckBox;
    }
}