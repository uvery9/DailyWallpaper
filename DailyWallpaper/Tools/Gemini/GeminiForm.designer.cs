namespace DailyWallpaper
{
    partial class GeminiForm
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
            this.components = new System.ComponentModel.Container();
            this.groupBoxTarget = new System.Windows.Forms.GroupBox();
            this.btnSelectTargetFolder2 = new System.Windows.Forms.Button();
            this.labelTargetFolder1 = new System.Windows.Forms.Label();
            this.labelTargetFolder2 = new System.Windows.Forms.Label();
            this.targetFolder2TextBox = new System.Windows.Forms.TextBox();
            this.btnSelectTargetFolder1 = new System.Windows.Forms.Button();
            this.targetFolder1TextBox = new System.Windows.Forms.TextBox();
            this.modeCheckBox = new System.Windows.Forms.CheckBox();
            this.regexCheckBox = new System.Windows.Forms.CheckBox();
            this.filterExample = new System.Windows.Forms.TextBox();
            this.folderFilterTextBox = new System.Windows.Forms.TextBox();
            this.fldFilterText = new System.Windows.Forms.TextBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.tbConsole = new System.Windows.Forms.TextBox();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.deleteOrRecycleBin = new System.Windows.Forms.CheckBox();
            this.saveList2File = new System.Windows.Forms.Button();
            this.listOrLog = new System.Windows.Forms.CheckBox();
            this.resultListView = new System.Windows.Forms.ListView();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.summaryTextBox = new System.Windows.Forms.TextBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.resultGroupBox = new System.Windows.Forms.GroupBox();
            this.fileModeGroupBox = new System.Windows.Forms.GroupBox();
            this.fileExtNameCheckBox = new System.Windows.Forms.CheckBox();
            this.fileNameCheckBox = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ignoreFileSizeTextBox = new System.Windows.Forms.TextBox();
            this.ignoreFileSizecomboBox = new System.Windows.Forms.ComboBox();
            this.ignoreFileCheckBox = new System.Windows.Forms.CheckBox();
            this.fileSHA1CheckBox = new System.Windows.Forms.CheckBox();
            this.fileSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.fileMD5CheckBox = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBoxTarget.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.resultGroupBox.SuspendLayout();
            this.fileModeGroupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxTarget
            // 
            this.groupBoxTarget.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxTarget.Controls.Add(this.btnSelectTargetFolder2);
            this.groupBoxTarget.Controls.Add(this.labelTargetFolder1);
            this.groupBoxTarget.Controls.Add(this.labelTargetFolder2);
            this.groupBoxTarget.Controls.Add(this.targetFolder2TextBox);
            this.groupBoxTarget.Controls.Add(this.btnSelectTargetFolder1);
            this.groupBoxTarget.Controls.Add(this.targetFolder1TextBox);
            this.groupBoxTarget.Location = new System.Drawing.Point(13, 13);
            this.groupBoxTarget.Margin = new System.Windows.Forms.Padding(4);
            this.groupBoxTarget.Name = "groupBoxTarget";
            this.groupBoxTarget.Padding = new System.Windows.Forms.Padding(4);
            this.groupBoxTarget.Size = new System.Drawing.Size(967, 100);
            this.groupBoxTarget.TabIndex = 1;
            this.groupBoxTarget.TabStop = false;
            this.groupBoxTarget.Text = "Paths (Allow Drop)";
            // 
            // btnSelectTargetFolder2
            // 
            this.btnSelectTargetFolder2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTargetFolder2.Location = new System.Drawing.Point(885, 54);
            this.btnSelectTargetFolder2.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectTargetFolder2.Name = "btnSelectTargetFolder2";
            this.btnSelectTargetFolder2.Size = new System.Drawing.Size(77, 35);
            this.btnSelectTargetFolder2.TabIndex = 5;
            this.btnSelectTargetFolder2.Text = "Browse";
            this.btnSelectTargetFolder2.UseVisualStyleBackColor = true;
            this.btnSelectTargetFolder2.Click += new System.EventHandler(this.btnSelectTargetFolder2_Click);
            // 
            // labelTargetFolder1
            // 
            this.labelTargetFolder1.AutoSize = true;
            this.labelTargetFolder1.Location = new System.Drawing.Point(0, 25);
            this.labelTargetFolder1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTargetFolder1.Name = "labelTargetFolder1";
            this.labelTargetFolder1.Size = new System.Drawing.Size(152, 18);
            this.labelTargetFolder1.TabIndex = 1;
            this.labelTargetFolder1.Text = "Target Folder 1:";
            // 
            // labelTargetFolder2
            // 
            this.labelTargetFolder2.AutoSize = true;
            this.labelTargetFolder2.Location = new System.Drawing.Point(0, 62);
            this.labelTargetFolder2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTargetFolder2.Name = "labelTargetFolder2";
            this.labelTargetFolder2.Size = new System.Drawing.Size(152, 18);
            this.labelTargetFolder2.TabIndex = 1;
            this.labelTargetFolder2.Text = "Target Folder 2:";
            // 
            // targetFolder2TextBox
            // 
            this.targetFolder2TextBox.AllowDrop = true;
            this.targetFolder2TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetFolder2TextBox.Location = new System.Drawing.Point(157, 59);
            this.targetFolder2TextBox.Margin = new System.Windows.Forms.Padding(4);
            this.targetFolder2TextBox.Name = "targetFolder2TextBox";
            this.targetFolder2TextBox.Size = new System.Drawing.Size(720, 28);
            this.targetFolder2TextBox.TabIndex = 3;
            this.targetFolder2TextBox.TabStop = false;
            this.targetFolder2TextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.targetFolder2_DragDrop);
            this.targetFolder2TextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.targetFolder1_2_DragEnter);
            // 
            // btnSelectTargetFolder1
            // 
            this.btnSelectTargetFolder1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTargetFolder1.Location = new System.Drawing.Point(885, 17);
            this.btnSelectTargetFolder1.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectTargetFolder1.Name = "btnSelectTargetFolder1";
            this.btnSelectTargetFolder1.Size = new System.Drawing.Size(77, 35);
            this.btnSelectTargetFolder1.TabIndex = 5;
            this.btnSelectTargetFolder1.Text = "Browse";
            this.btnSelectTargetFolder1.UseVisualStyleBackColor = true;
            this.btnSelectTargetFolder1.Click += new System.EventHandler(this.btnSelectTargetFolder1_Click);
            // 
            // targetFolder1TextBox
            // 
            this.targetFolder1TextBox.AllowDrop = true;
            this.targetFolder1TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetFolder1TextBox.Location = new System.Drawing.Point(157, 22);
            this.targetFolder1TextBox.Margin = new System.Windows.Forms.Padding(4);
            this.targetFolder1TextBox.Name = "targetFolder1TextBox";
            this.targetFolder1TextBox.Size = new System.Drawing.Size(722, 28);
            this.targetFolder1TextBox.TabIndex = 3;
            this.targetFolder1TextBox.TabStop = false;
            this.targetFolder1TextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.targetFolder1_DragDrop);
            this.targetFolder1TextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.targetFolder1_2_DragEnter);
            // 
            // modeCheckBox
            // 
            this.modeCheckBox.AutoSize = true;
            this.modeCheckBox.Location = new System.Drawing.Point(203, 7);
            this.modeCheckBox.Name = "modeCheckBox";
            this.modeCheckBox.Size = new System.Drawing.Size(70, 22);
            this.modeCheckBox.TabIndex = 10;
            this.modeCheckBox.Text = "Find";
            this.modeCheckBox.UseVisualStyleBackColor = true;
            this.modeCheckBox.CheckedChanged += new System.EventHandler(this.modeCheckBox_CheckedChanged);
            // 
            // regexCheckBox
            // 
            this.regexCheckBox.AutoSize = true;
            this.regexCheckBox.Location = new System.Drawing.Point(135, 7);
            this.regexCheckBox.Name = "regexCheckBox";
            this.regexCheckBox.Size = new System.Drawing.Size(52, 22);
            this.regexCheckBox.TabIndex = 9;
            this.regexCheckBox.Text = "RE";
            this.regexCheckBox.UseVisualStyleBackColor = true;
            // 
            // filterExample
            // 
            this.filterExample.BackColor = System.Drawing.SystemColors.Control;
            this.filterExample.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filterExample.Location = new System.Drawing.Point(39, 69);
            this.filterExample.Name = "filterExample";
            this.filterExample.ReadOnly = true;
            this.filterExample.Size = new System.Drawing.Size(234, 21);
            this.filterExample.TabIndex = 8;
            this.filterExample.Text = "SSSSSS";
            this.filterExample.TextChanged += new System.EventHandler(this.filterExample_TextChanged);
            // 
            // folderFilterTextBox
            // 
            this.folderFilterTextBox.Location = new System.Drawing.Point(8, 35);
            this.folderFilterTextBox.Name = "folderFilterTextBox";
            this.folderFilterTextBox.Size = new System.Drawing.Size(265, 28);
            this.folderFilterTextBox.TabIndex = 7;
            this.folderFilterTextBox.TextChanged += new System.EventHandler(this.folderFilterTextBox_TextChanged);
            // 
            // fldFilterText
            // 
            this.fldFilterText.BackColor = System.Drawing.SystemColors.Control;
            this.fldFilterText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fldFilterText.Location = new System.Drawing.Point(3, 8);
            this.fldFilterText.Name = "fldFilterText";
            this.fldFilterText.ReadOnly = true;
            this.fldFilterText.Size = new System.Drawing.Size(177, 21);
            this.fldFilterText.TabIndex = 6;
            this.fldFilterText.Text = "Folder Filter:";
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnalyze.Location = new System.Drawing.Point(5, 108);
            this.btnAnalyze.Margin = new System.Windows.Forms.Padding(4);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(93, 54);
            this.btnAnalyze.TabIndex = 5;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // tbConsole
            // 
            this.tbConsole.AllowDrop = true;
            this.tbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConsole.Location = new System.Drawing.Point(0, 546);
            this.tbConsole.Margin = new System.Windows.Forms.Padding(4);
            this.tbConsole.Multiline = true;
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.ReadOnly = true;
            this.tbConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbConsole.Size = new System.Drawing.Size(882, 242);
            this.tbConsole.TabIndex = 0;
            this.tbConsole.WordWrap = false;
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(250, 115);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(4);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(109, 40);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "RecycleBin";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(4, 6);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(61, 32);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnStop
            // 
            this.btnStop.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStop.Location = new System.Drawing.Point(131, 120);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(57, 32);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // deleteOrRecycleBin
            // 
            this.deleteOrRecycleBin.AutoSize = true;
            this.deleteOrRecycleBin.Location = new System.Drawing.Point(221, 127);
            this.deleteOrRecycleBin.Name = "deleteOrRecycleBin";
            this.deleteOrRecycleBin.Size = new System.Drawing.Size(22, 21);
            this.deleteOrRecycleBin.TabIndex = 6;
            this.deleteOrRecycleBin.UseVisualStyleBackColor = true;
            this.deleteOrRecycleBin.CheckedChanged += new System.EventHandler(this.deleteOrRecycleBin_CheckedChanged);
            // 
            // saveList2File
            // 
            this.saveList2File.Location = new System.Drawing.Point(194, 6);
            this.saveList2File.Name = "saveList2File";
            this.saveList2File.Size = new System.Drawing.Size(169, 33);
            this.saveList2File.TabIndex = 7;
            this.saveList2File.Text = "Save list to File";
            this.saveList2File.UseVisualStyleBackColor = true;
            this.saveList2File.Click += new System.EventHandler(this.saveList2File_Click);
            // 
            // listOrLog
            // 
            this.listOrLog.AutoSize = true;
            this.listOrLog.Location = new System.Drawing.Point(166, 13);
            this.listOrLog.Name = "listOrLog";
            this.listOrLog.Size = new System.Drawing.Size(22, 21);
            this.listOrLog.TabIndex = 8;
            this.listOrLog.UseVisualStyleBackColor = true;
            this.listOrLog.CheckedChanged += new System.EventHandler(this.listOrLog_CheckedChanged);
            // 
            // resultListView
            // 
            this.resultListView.HideSelection = false;
            this.resultListView.Location = new System.Drawing.Point(0, 27);
            this.resultListView.Name = "resultListView";
            this.resultListView.Size = new System.Drawing.Size(880, 586);
            this.resultListView.TabIndex = 2;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.summaryTextBox);
            this.panel2.Controls.Add(this.listOrLog);
            this.panel2.Controls.Add(this.btnAnalyze);
            this.panel2.Controls.Add(this.saveList2File);
            this.panel2.Controls.Add(this.btnClear);
            this.panel2.Controls.Add(this.deleteOrRecycleBin);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.btnStop);
            this.panel2.Location = new System.Drawing.Point(898, 740);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(363, 168);
            this.panel2.TabIndex = 10;
            // 
            // summaryTextBox
            // 
            this.summaryTextBox.Location = new System.Drawing.Point(4, 45);
            this.summaryTextBox.Multiline = true;
            this.summaryTextBox.Name = "summaryTextBox";
            this.summaryTextBox.ReadOnly = true;
            this.summaryTextBox.Size = new System.Drawing.Size(355, 56);
            this.summaryTextBox.TabIndex = 9;
            this.summaryTextBox.Text = "Summay: Found 1,500 duplicate files.";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.filterExample);
            this.panel4.Controls.Add(this.regexCheckBox);
            this.panel4.Controls.Add(this.modeCheckBox);
            this.panel4.Controls.Add(this.fldFilterText);
            this.panel4.Controls.Add(this.folderFilterTextBox);
            this.panel4.Location = new System.Drawing.Point(984, 15);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(277, 98);
            this.panel4.TabIndex = 12;
            // 
            // resultGroupBox
            // 
            this.resultGroupBox.Controls.Add(this.tbConsole);
            this.resultGroupBox.Controls.Add(this.resultListView);
            this.resultGroupBox.Location = new System.Drawing.Point(8, 120);
            this.resultGroupBox.Name = "resultGroupBox";
            this.resultGroupBox.Size = new System.Drawing.Size(882, 788);
            this.resultGroupBox.TabIndex = 13;
            this.resultGroupBox.TabStop = false;
            this.resultGroupBox.Text = "Result";
            // 
            // fileModeGroupBox
            // 
            this.fileModeGroupBox.Controls.Add(this.fileExtNameCheckBox);
            this.fileModeGroupBox.Controls.Add(this.fileNameCheckBox);
            this.fileModeGroupBox.Controls.Add(this.panel1);
            this.fileModeGroupBox.Location = new System.Drawing.Point(894, 129);
            this.fileModeGroupBox.Name = "fileModeGroupBox";
            this.fileModeGroupBox.Size = new System.Drawing.Size(367, 151);
            this.fileModeGroupBox.TabIndex = 14;
            this.fileModeGroupBox.TabStop = false;
            this.fileModeGroupBox.Text = "File Mode";
            // 
            // fileExtNameCheckBox
            // 
            this.fileExtNameCheckBox.AutoSize = true;
            this.fileExtNameCheckBox.Location = new System.Drawing.Point(162, 27);
            this.fileExtNameCheckBox.Name = "fileExtNameCheckBox";
            this.fileExtNameCheckBox.Size = new System.Drawing.Size(205, 22);
            this.fileExtNameCheckBox.TabIndex = 0;
            this.fileExtNameCheckBox.Text = "Same File extension";
            this.fileExtNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileNameCheckBox
            // 
            this.fileNameCheckBox.AutoSize = true;
            this.fileNameCheckBox.Location = new System.Drawing.Point(6, 27);
            this.fileNameCheckBox.Name = "fileNameCheckBox";
            this.fileNameCheckBox.Size = new System.Drawing.Size(160, 22);
            this.fileNameCheckBox.TabIndex = 0;
            this.fileNameCheckBox.Text = "Same File Name";
            this.fileNameCheckBox.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.ignoreFileSizeTextBox);
            this.panel1.Controls.Add(this.ignoreFileSizecomboBox);
            this.panel1.Controls.Add(this.ignoreFileCheckBox);
            this.panel1.Controls.Add(this.fileSHA1CheckBox);
            this.panel1.Controls.Add(this.fileSizeCheckBox);
            this.panel1.Controls.Add(this.fileMD5CheckBox);
            this.panel1.Location = new System.Drawing.Point(6, 55);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(361, 90);
            this.panel1.TabIndex = 3;
            // 
            // ignoreFileSizeTextBox
            // 
            this.ignoreFileSizeTextBox.Location = new System.Drawing.Point(232, 60);
            this.ignoreFileSizeTextBox.Name = "ignoreFileSizeTextBox";
            this.ignoreFileSizeTextBox.Size = new System.Drawing.Size(56, 28);
            this.ignoreFileSizeTextBox.TabIndex = 2;
            this.ignoreFileSizeTextBox.Text = "1024";
            this.ignoreFileSizeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ignoreFileSizecomboBox
            // 
            this.ignoreFileSizecomboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ignoreFileSizecomboBox.FormattingEnabled = true;
            this.ignoreFileSizecomboBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ignoreFileSizecomboBox.Items.AddRange(new object[] {
            "B",
            "KB",
            "MB",
            "GB"});
            this.ignoreFileSizecomboBox.Location = new System.Drawing.Point(294, 61);
            this.ignoreFileSizecomboBox.MaxDropDownItems = 3;
            this.ignoreFileSizecomboBox.Name = "ignoreFileSizecomboBox";
            this.ignoreFileSizecomboBox.Size = new System.Drawing.Size(61, 26);
            this.ignoreFileSizecomboBox.TabIndex = 1;
            this.ignoreFileSizecomboBox.TabStop = false;
            this.ignoreFileSizecomboBox.SelectedIndexChanged += new System.EventHandler(this.ignoreFileSizecomboBox_SelectedIndexChanged);
            // 
            // ignoreFileCheckBox
            // 
            this.ignoreFileCheckBox.AutoSize = true;
            this.ignoreFileCheckBox.Location = new System.Drawing.Point(3, 62);
            this.ignoreFileCheckBox.Name = "ignoreFileCheckBox";
            this.ignoreFileCheckBox.Size = new System.Drawing.Size(232, 22);
            this.ignoreFileCheckBox.TabIndex = 0;
            this.ignoreFileCheckBox.Text = "Ignore files less than";
            this.ignoreFileCheckBox.UseVisualStyleBackColor = true;
            this.ignoreFileCheckBox.Click += new System.EventHandler(this.ignoreFileCheckBox_Click);
            // 
            // fileSHA1CheckBox
            // 
            this.fileSHA1CheckBox.AutoSize = true;
            this.fileSHA1CheckBox.Location = new System.Drawing.Point(173, 35);
            this.fileSHA1CheckBox.Name = "fileSHA1CheckBox";
            this.fileSHA1CheckBox.Size = new System.Drawing.Size(115, 22);
            this.fileSHA1CheckBox.TabIndex = 0;
            this.fileSHA1CheckBox.Text = "Same SHA1";
            this.fileSHA1CheckBox.UseVisualStyleBackColor = true;
            // 
            // fileSizeCheckBox
            // 
            this.fileSizeCheckBox.AutoSize = true;
            this.fileSizeCheckBox.Location = new System.Drawing.Point(3, 7);
            this.fileSizeCheckBox.Name = "fileSizeCheckBox";
            this.fileSizeCheckBox.Size = new System.Drawing.Size(223, 22);
            this.fileSizeCheckBox.TabIndex = 0;
            this.fileSizeCheckBox.Text = "Same File Size (Fast)";
            this.fileSizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // fileMD5CheckBox
            // 
            this.fileMD5CheckBox.AutoSize = true;
            this.fileMD5CheckBox.Location = new System.Drawing.Point(37, 35);
            this.fileMD5CheckBox.Name = "fileMD5CheckBox";
            this.fileMD5CheckBox.Size = new System.Drawing.Size(106, 22);
            this.fileMD5CheckBox.TabIndex = 0;
            this.fileMD5CheckBox.Text = "Same MD5";
            this.fileMD5CheckBox.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Location = new System.Drawing.Point(898, 280);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer1.Size = new System.Drawing.Size(363, 453);
            this.splitContainer1.SplitterDistance = 224;
            this.splitContainer1.TabIndex = 3;
            // 
            // GeminiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1270, 915);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.fileModeGroupBox);
            this.Controls.Add(this.resultGroupBox);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.groupBoxTarget);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(754, 536);
            this.Name = "GeminiForm";
            this.Text = "Gemini: find duplicate files and delete.";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GeminiForm_FormClosing);
            this.groupBoxTarget.ResumeLayout(false);
            this.groupBoxTarget.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.resultGroupBox.ResumeLayout(false);
            this.resultGroupBox.PerformLayout();
            this.fileModeGroupBox.ResumeLayout(false);
            this.fileModeGroupBox.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBoxTarget;
        private System.Windows.Forms.TextBox targetFolder1TextBox;
        private System.Windows.Forms.Label labelTargetFolder1;
        private System.Windows.Forms.Button btnSelectTargetFolder1;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.TextBox tbConsole;
        private System.Windows.Forms.CheckBox deleteOrRecycleBin;
        private System.Windows.Forms.Button saveList2File;
        private System.Windows.Forms.CheckBox listOrLog;
        private System.Windows.Forms.TextBox fldFilterText;
        private System.Windows.Forms.TextBox folderFilterTextBox;
        private System.Windows.Forms.TextBox filterExample;
        private System.Windows.Forms.CheckBox regexCheckBox;
        private System.Windows.Forms.CheckBox modeCheckBox;
        private System.Windows.Forms.Button btnSelectTargetFolder2;
        private System.Windows.Forms.TextBox targetFolder2TextBox;
        private System.Windows.Forms.Label labelTargetFolder2;
        private System.Windows.Forms.ListView resultListView;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.TextBox summaryTextBox;
        private System.Windows.Forms.GroupBox resultGroupBox;
        private System.Windows.Forms.GroupBox fileModeGroupBox;
        private System.Windows.Forms.CheckBox fileNameCheckBox;
        private System.Windows.Forms.CheckBox fileSizeCheckBox;
        private System.Windows.Forms.CheckBox fileSHA1CheckBox;
        private System.Windows.Forms.CheckBox fileMD5CheckBox;
        private System.Windows.Forms.CheckBox fileExtNameCheckBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.CheckBox ignoreFileCheckBox;
        private System.Windows.Forms.ComboBox ignoreFileSizecomboBox;
        private System.Windows.Forms.TextBox ignoreFileSizeTextBox;
    }
}