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
            this.updateButton = new System.Windows.Forms.Button();
            this.filterExample = new System.Windows.Forms.TextBox();
            this.btnSelectTargetFolder2 = new System.Windows.Forms.Button();
            this.modeCheckBox = new System.Windows.Forms.CheckBox();
            this.regexCheckBox = new System.Windows.Forms.CheckBox();
            this.labelTargetFolder1 = new System.Windows.Forms.Label();
            this.labelTargetFolder2 = new System.Windows.Forms.Label();
            this.folderFilterTextBox = new System.Windows.Forms.TextBox();
            this.fldFilterText = new System.Windows.Forms.TextBox();
            this.targetFolder2TextBox = new System.Windows.Forms.TextBox();
            this.btnSelectTargetFolder1 = new System.Windows.Forms.Button();
            this.targetFolder1TextBox = new System.Windows.Forms.TextBox();
            this.btnAnalyze = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.deleteOrRecycleBin = new System.Windows.Forms.CheckBox();
            this.resultListView = new System.Windows.Forms.ListView();
            this.CBcolumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.modifiedTimeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.extNameColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.sizeColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.dirColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.hashColumnHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.openDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyFullPathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cleanUpButton = new System.Windows.Forms.Button();
            this.geminiProgressBar = new System.Windows.Forms.ProgressBar();
            this.summaryTextBox = new System.Windows.Forms.TextBox();
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
            this.tbConsole = new System.Windows.Forms.TextBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.fileOptionsmenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOnTopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveLogToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveResultToFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reverseElectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cleanNonExistentItemsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.usageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listViewContextMenuStrip.SuspendLayout();
            this.panel2.SuspendLayout();
            this.fileModeGroupBox.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel5.SuspendLayout();
            this.fileOptionsmenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(821, 104);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(77, 28);
            this.updateButton.TabIndex = 11;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // filterExample
            // 
            this.filterExample.BackColor = System.Drawing.SystemColors.Control;
            this.filterExample.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.filterExample.Location = new System.Drawing.Point(569, 109);
            this.filterExample.Name = "filterExample";
            this.filterExample.ReadOnly = true;
            this.filterExample.Size = new System.Drawing.Size(246, 21);
            this.filterExample.TabIndex = 8;
            this.filterExample.Text = "SSSSSS";
            // 
            // btnSelectTargetFolder2
            // 
            this.btnSelectTargetFolder2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTargetFolder2.Location = new System.Drawing.Point(821, 51);
            this.btnSelectTargetFolder2.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelectTargetFolder2.Name = "btnSelectTargetFolder2";
            this.btnSelectTargetFolder2.Size = new System.Drawing.Size(77, 35);
            this.btnSelectTargetFolder2.TabIndex = 5;
            this.btnSelectTargetFolder2.Text = "Browse";
            this.btnSelectTargetFolder2.UseVisualStyleBackColor = true;
            this.btnSelectTargetFolder2.Click += new System.EventHandler(this.btnSelectTargetFolder2_Click);
            // 
            // modeCheckBox
            // 
            this.modeCheckBox.AutoSize = true;
            this.modeCheckBox.Location = new System.Drawing.Point(497, 108);
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
            this.regexCheckBox.Location = new System.Drawing.Point(439, 108);
            this.regexCheckBox.Name = "regexCheckBox";
            this.regexCheckBox.Size = new System.Drawing.Size(52, 22);
            this.regexCheckBox.TabIndex = 9;
            this.regexCheckBox.Text = "RE";
            this.regexCheckBox.UseVisualStyleBackColor = true;
            // 
            // labelTargetFolder1
            // 
            this.labelTargetFolder1.AutoSize = true;
            this.labelTargetFolder1.Location = new System.Drawing.Point(11, 13);
            this.labelTargetFolder1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTargetFolder1.Name = "labelTargetFolder1";
            this.labelTargetFolder1.Size = new System.Drawing.Size(89, 18);
            this.labelTargetFolder1.TabIndex = 1;
            this.labelTargetFolder1.Text = "Folder 1:";
            // 
            // labelTargetFolder2
            // 
            this.labelTargetFolder2.AutoSize = true;
            this.labelTargetFolder2.Location = new System.Drawing.Point(11, 59);
            this.labelTargetFolder2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTargetFolder2.Name = "labelTargetFolder2";
            this.labelTargetFolder2.Size = new System.Drawing.Size(89, 18);
            this.labelTargetFolder2.TabIndex = 1;
            this.labelTargetFolder2.Text = "Folder 2:";
            // 
            // folderFilterTextBox
            // 
            this.folderFilterTextBox.Location = new System.Drawing.Point(125, 106);
            this.folderFilterTextBox.Name = "folderFilterTextBox";
            this.folderFilterTextBox.Size = new System.Drawing.Size(294, 28);
            this.folderFilterTextBox.TabIndex = 7;
            // 
            // fldFilterText
            // 
            this.fldFilterText.BackColor = System.Drawing.SystemColors.Control;
            this.fldFilterText.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.fldFilterText.Location = new System.Drawing.Point(14, 109);
            this.fldFilterText.Name = "fldFilterText";
            this.fldFilterText.ReadOnly = true;
            this.fldFilterText.Size = new System.Drawing.Size(117, 21);
            this.fldFilterText.TabIndex = 6;
            this.fldFilterText.Text = "Path Filter:";
            // 
            // targetFolder2TextBox
            // 
            this.targetFolder2TextBox.AllowDrop = true;
            this.targetFolder2TextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.targetFolder2TextBox.Location = new System.Drawing.Point(108, 56);
            this.targetFolder2TextBox.Margin = new System.Windows.Forms.Padding(4);
            this.targetFolder2TextBox.Name = "targetFolder2TextBox";
            this.targetFolder2TextBox.Size = new System.Drawing.Size(695, 28);
            this.targetFolder2TextBox.TabIndex = 3;
            this.targetFolder2TextBox.TabStop = false;
            this.targetFolder2TextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.targetFolder2_DragDrop);
            this.targetFolder2TextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.targetFolder1_2_DragEnter);
            // 
            // btnSelectTargetFolder1
            // 
            this.btnSelectTargetFolder1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectTargetFolder1.Location = new System.Drawing.Point(821, 5);
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
            this.targetFolder1TextBox.Location = new System.Drawing.Point(108, 10);
            this.targetFolder1TextBox.Margin = new System.Windows.Forms.Padding(4);
            this.targetFolder1TextBox.Name = "targetFolder1TextBox";
            this.targetFolder1TextBox.Size = new System.Drawing.Size(695, 28);
            this.targetFolder1TextBox.TabIndex = 3;
            this.targetFolder1TextBox.TabStop = false;
            this.targetFolder1TextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.targetFolder1_DragDrop);
            this.targetFolder1TextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.targetFolder1_2_DragEnter);
            // 
            // btnAnalyze
            // 
            this.btnAnalyze.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAnalyze.Location = new System.Drawing.Point(4, 106);
            this.btnAnalyze.Margin = new System.Windows.Forms.Padding(4);
            this.btnAnalyze.Name = "btnAnalyze";
            this.btnAnalyze.Size = new System.Drawing.Size(93, 43);
            this.btnAnalyze.TabIndex = 5;
            this.btnAnalyze.Text = "Analyze";
            this.btnAnalyze.UseVisualStyleBackColor = true;
            this.btnAnalyze.Click += new System.EventHandler(this.btnAnalyze_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.Location = new System.Drawing.Point(252, 107);
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
            this.btnClear.Location = new System.Drawing.Point(4, 48);
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
            this.btnStop.Location = new System.Drawing.Point(139, 111);
            this.btnStop.Margin = new System.Windows.Forms.Padding(4);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(61, 32);
            this.btnStop.TabIndex = 4;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // deleteOrRecycleBin
            // 
            this.deleteOrRecycleBin.AutoSize = true;
            this.deleteOrRecycleBin.Location = new System.Drawing.Point(223, 118);
            this.deleteOrRecycleBin.Name = "deleteOrRecycleBin";
            this.deleteOrRecycleBin.Size = new System.Drawing.Size(22, 21);
            this.deleteOrRecycleBin.TabIndex = 6;
            this.deleteOrRecycleBin.UseVisualStyleBackColor = true;
            this.deleteOrRecycleBin.Click += new System.EventHandler(this.deleteOrRecycleBin_Click);
            // 
            // resultListView
            // 
            this.resultListView.AllowDrop = true;
            this.resultListView.CheckBoxes = true;
            this.resultListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.CBcolumnHeader,
            this.nameColumnHeader,
            this.modifiedTimeColumnHeader,
            this.extNameColumnHeader,
            this.sizeColumnHeader,
            this.dirColumnHeader,
            this.hashColumnHeader});
            this.resultListView.ContextMenuStrip = this.listViewContextMenuStrip;
            this.resultListView.FullRowSelect = true;
            this.resultListView.HideSelection = false;
            this.resultListView.Location = new System.Drawing.Point(0, 3);
            this.resultListView.Name = "resultListView";
            this.resultListView.Size = new System.Drawing.Size(1285, 533);
            this.resultListView.TabIndex = 2;
            this.resultListView.TabStop = false;
            this.resultListView.UseCompatibleStateImageBehavior = false;
            this.resultListView.View = System.Windows.Forms.View.Details;
            this.resultListView.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.resultListView_ColumnClick);
            this.resultListView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.resultListView_ItemCheck);
            this.resultListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.targetFolder2_DragDrop);
            this.resultListView.DragEnter += new System.Windows.Forms.DragEventHandler(this.targetFolder1_2_DragEnter);
            this.resultListView.MouseClick += new System.Windows.Forms.MouseEventHandler(this.resultListView_MouseClick);
            this.resultListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.resultListView_MouseDoubleClick);
            // 
            // CBcolumnHeader
            // 
            this.CBcolumnHeader.Text = "CB";
            this.CBcolumnHeader.Width = 28;
            // 
            // nameColumnHeader
            // 
            this.nameColumnHeader.Text = "Name";
            this.nameColumnHeader.Width = 339;
            // 
            // modifiedTimeColumnHeader
            // 
            this.modifiedTimeColumnHeader.Text = "Modified Time";
            this.modifiedTimeColumnHeader.Width = 156;
            // 
            // extNameColumnHeader
            // 
            this.extNameColumnHeader.Text = "EXT";
            this.extNameColumnHeader.Width = 66;
            // 
            // sizeColumnHeader
            // 
            this.sizeColumnHeader.Text = "Size";
            this.sizeColumnHeader.Width = 63;
            // 
            // dirColumnHeader
            // 
            this.dirColumnHeader.Text = "Directory";
            this.dirColumnHeader.Width = 359;
            // 
            // hashColumnHeader
            // 
            this.hashColumnHeader.Text = "HASH";
            this.hashColumnHeader.Width = 282;
            // 
            // listViewContextMenuStrip
            // 
            this.listViewContextMenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.listViewContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openDirectoryToolStripMenuItem,
            this.copyFullPathToolStripMenuItem});
            this.listViewContextMenuStrip.Name = "listViewContextMenuStrip";
            this.listViewContextMenuStrip.Size = new System.Drawing.Size(241, 97);
            // 
            // openDirectoryToolStripMenuItem
            // 
            this.openDirectoryToolStripMenuItem.Name = "openDirectoryToolStripMenuItem";
            this.openDirectoryToolStripMenuItem.Size = new System.Drawing.Size(240, 30);
            this.openDirectoryToolStripMenuItem.Text = "Open Directory";
            this.openDirectoryToolStripMenuItem.Click += new System.EventHandler(this.openDirectoryToolStripMenuItem_Click);
            // 
            // copyFullPathToolStripMenuItem
            // 
            this.copyFullPathToolStripMenuItem.Name = "copyFullPathToolStripMenuItem";
            this.copyFullPathToolStripMenuItem.Size = new System.Drawing.Size(240, 30);
            this.copyFullPathToolStripMenuItem.Text = "Copy FullPath";
            this.copyFullPathToolStripMenuItem.Click += new System.EventHandler(this.copyFullPathToolStripMenuItem_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnAnalyze);
            this.panel2.Controls.Add(this.cleanUpButton);
            this.panel2.Controls.Add(this.btnClear);
            this.panel2.Controls.Add(this.deleteOrRecycleBin);
            this.panel2.Controls.Add(this.btnDelete);
            this.panel2.Controls.Add(this.btnStop);
            this.panel2.Controls.Add(this.geminiProgressBar);
            this.panel2.Controls.Add(this.summaryTextBox);
            this.panel2.Location = new System.Drawing.Point(922, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(363, 168);
            this.panel2.TabIndex = 10;
            // 
            // cleanUpButton
            // 
            this.cleanUpButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanUpButton.Location = new System.Drawing.Point(269, 48);
            this.cleanUpButton.Margin = new System.Windows.Forms.Padding(4);
            this.cleanUpButton.Name = "cleanUpButton";
            this.cleanUpButton.Size = new System.Drawing.Size(91, 32);
            this.cleanUpButton.TabIndex = 4;
            this.cleanUpButton.Text = "Clean-UP";
            this.cleanUpButton.UseVisualStyleBackColor = true;
            this.cleanUpButton.Click += new System.EventHandler(this.cleanUpButton_Click);
            // 
            // geminiProgressBar
            // 
            this.geminiProgressBar.Location = new System.Drawing.Point(5, 13);
            this.geminiProgressBar.Name = "geminiProgressBar";
            this.geminiProgressBar.Size = new System.Drawing.Size(356, 28);
            this.geminiProgressBar.TabIndex = 10;
            this.geminiProgressBar.Click += new System.EventHandler(this.geminiProgressBar_Click);
            // 
            // summaryTextBox
            // 
            this.summaryTextBox.Location = new System.Drawing.Point(5, 13);
            this.summaryTextBox.Name = "summaryTextBox";
            this.summaryTextBox.ReadOnly = true;
            this.summaryTextBox.Size = new System.Drawing.Size(350, 28);
            this.summaryTextBox.TabIndex = 9;
            this.summaryTextBox.Text = "Summay: Found 1,500 duplicate files.";
            this.summaryTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // fileModeGroupBox
            // 
            this.fileModeGroupBox.Controls.Add(this.fileExtNameCheckBox);
            this.fileModeGroupBox.Controls.Add(this.fileNameCheckBox);
            this.fileModeGroupBox.Controls.Add(this.panel1);
            this.fileModeGroupBox.Location = new System.Drawing.Point(916, 6);
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
            this.fileSizeCheckBox.Checked = true;
            this.fileSizeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fileSizeCheckBox.Enabled = false;
            this.fileSizeCheckBox.Location = new System.Drawing.Point(3, 7);
            this.fileSizeCheckBox.Name = "fileSizeCheckBox";
            this.fileSizeCheckBox.Size = new System.Drawing.Size(223, 22);
            this.fileSizeCheckBox.TabIndex = 0;
            this.fileSizeCheckBox.Text = "Same File Size (Base)";
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
            // tbConsole
            // 
            this.tbConsole.AllowDrop = true;
            this.tbConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbConsole.Location = new System.Drawing.Point(4, 4);
            this.tbConsole.Margin = new System.Windows.Forms.Padding(4);
            this.tbConsole.Multiline = true;
            this.tbConsole.Name = "tbConsole";
            this.tbConsole.ReadOnly = true;
            this.tbConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbConsole.Size = new System.Drawing.Size(911, 170);
            this.tbConsole.TabIndex = 0;
            this.tbConsole.WordWrap = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.tbConsole);
            this.panel3.Controls.Add(this.panel2);
            this.panel3.Location = new System.Drawing.Point(12, 735);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1286, 178);
            this.panel3.TabIndex = 15;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.fileModeGroupBox);
            this.panel4.Controls.Add(this.panel6);
            this.panel4.Location = new System.Drawing.Point(11, 35);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1287, 155);
            this.panel4.TabIndex = 16;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.folderFilterTextBox);
            this.panel6.Controls.Add(this.updateButton);
            this.panel6.Controls.Add(this.labelTargetFolder1);
            this.panel6.Controls.Add(this.filterExample);
            this.panel6.Controls.Add(this.targetFolder1TextBox);
            this.panel6.Controls.Add(this.btnSelectTargetFolder2);
            this.panel6.Controls.Add(this.btnSelectTargetFolder1);
            this.panel6.Controls.Add(this.modeCheckBox);
            this.panel6.Controls.Add(this.targetFolder2TextBox);
            this.panel6.Controls.Add(this.regexCheckBox);
            this.panel6.Controls.Add(this.fldFilterText);
            this.panel6.Controls.Add(this.labelTargetFolder2);
            this.panel6.Location = new System.Drawing.Point(3, 3);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(916, 157);
            this.panel6.TabIndex = 3;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.resultListView);
            this.panel5.Location = new System.Drawing.Point(12, 193);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1286, 539);
            this.panel5.TabIndex = 17;
            // 
            // fileOptionsmenuStrip
            // 
            this.fileOptionsmenuStrip.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.fileOptionsmenuStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.fileOptionsmenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.selectToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.fileOptionsmenuStrip.Location = new System.Drawing.Point(0, 0);
            this.fileOptionsmenuStrip.Name = "fileOptionsmenuStrip";
            this.fileOptionsmenuStrip.Size = new System.Drawing.Size(1310, 32);
            this.fileOptionsmenuStrip.TabIndex = 18;
            this.fileOptionsmenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alwaysOnTopToolStripMenuItem,
            this.saveLogToFileToolStripMenuItem,
            this.saveResultToFileToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(56, 28);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // alwaysOnTopToolStripMenuItem
            // 
            this.alwaysOnTopToolStripMenuItem.Name = "alwaysOnTopToolStripMenuItem";
            this.alwaysOnTopToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.alwaysOnTopToolStripMenuItem.Text = "Always On Top";
            this.alwaysOnTopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOnTopMenu_Click);
            // 
            // saveLogToFileToolStripMenuItem
            // 
            this.saveLogToFileToolStripMenuItem.Name = "saveLogToFileToolStripMenuItem";
            this.saveLogToFileToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.saveLogToFileToolStripMenuItem.Text = "Save log to file";
            this.saveLogToFileToolStripMenuItem.Click += new System.EventHandler(this.saveLogToFileToolStripMenuItem_Click);
            // 
            // saveResultToFileToolStripMenuItem
            // 
            this.saveResultToFileToolStripMenuItem.Name = "saveResultToFileToolStripMenuItem";
            this.saveResultToFileToolStripMenuItem.Size = new System.Drawing.Size(256, 34);
            this.saveResultToFileToolStripMenuItem.Text = "Save result to file";
            this.saveResultToFileToolStripMenuItem.Click += new System.EventHandler(this.saveResultToFileToolStripMenuItem_Click);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.selectAllToolStripMenuItem,
            this.unselectAllToolStripMenuItem,
            this.reverseElectionToolStripMenuItem,
            this.cleanNonExistentItemsToolStripMenuItem});
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(77, 28);
            this.selectToolStripMenuItem.Text = "Select";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(323, 34);
            this.undoToolStripMenuItem.Text = "Undo";
            this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(323, 34);
            this.redoToolStripMenuItem.Text = "Redo";
            this.redoToolStripMenuItem.Click += new System.EventHandler(this.redoToolStripMenuItem_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(323, 34);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.selectAllToolStripMenuItem_Click);
            // 
            // unselectAllToolStripMenuItem
            // 
            this.unselectAllToolStripMenuItem.Name = "unselectAllToolStripMenuItem";
            this.unselectAllToolStripMenuItem.Size = new System.Drawing.Size(323, 34);
            this.unselectAllToolStripMenuItem.Text = "Unselect All";
            this.unselectAllToolStripMenuItem.Click += new System.EventHandler(this.unselectAllToolStripMenuItem_Click);
            // 
            // reverseElectionToolStripMenuItem
            // 
            this.reverseElectionToolStripMenuItem.Name = "reverseElectionToolStripMenuItem";
            this.reverseElectionToolStripMenuItem.Size = new System.Drawing.Size(323, 34);
            this.reverseElectionToolStripMenuItem.Text = "Reverse Election";
            this.reverseElectionToolStripMenuItem.Click += new System.EventHandler(this.reverseElectionToolStripMenuItem_Click);
            // 
            // cleanNonExistentItemsToolStripMenuItem
            // 
            this.cleanNonExistentItemsToolStripMenuItem.Name = "cleanNonExistentItemsToolStripMenuItem";
            this.cleanNonExistentItemsToolStripMenuItem.Size = new System.Drawing.Size(323, 34);
            this.cleanNonExistentItemsToolStripMenuItem.Text = "Clean non-existent items";
            this.cleanNonExistentItemsToolStripMenuItem.Click += new System.EventHandler(this.cleanUpButton_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.usageToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(67, 28);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // usageToolStripMenuItem
            // 
            this.usageToolStripMenuItem.Name = "usageToolStripMenuItem";
            this.usageToolStripMenuItem.Size = new System.Drawing.Size(163, 34);
            this.usageToolStripMenuItem.Text = "Usage";
            this.usageToolStripMenuItem.Click += new System.EventHandler(this.usageToolStripMenuItem_Click);
            // 
            // GeminiForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1310, 915);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.fileOptionsmenuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.fileOptionsmenuStrip;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(754, 536);
            this.Name = "GeminiForm";
            this.Text = "Gemini: find duplicate files and delete.";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GeminiForm_FormClosing);
            this.listViewContextMenuStrip.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.fileModeGroupBox.ResumeLayout(false);
            this.fileModeGroupBox.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.fileOptionsmenuStrip.ResumeLayout(false);
            this.fileOptionsmenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox targetFolder1TextBox;
        private System.Windows.Forms.Label labelTargetFolder1;
        private System.Windows.Forms.Button btnSelectTargetFolder1;
        private System.Windows.Forms.Button btnAnalyze;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.CheckBox deleteOrRecycleBin;
        private System.Windows.Forms.TextBox fldFilterText;
        private System.Windows.Forms.TextBox folderFilterTextBox;
        private System.Windows.Forms.TextBox filterExample;
        private System.Windows.Forms.CheckBox regexCheckBox;
        private System.Windows.Forms.CheckBox modeCheckBox;
        private System.Windows.Forms.Button btnSelectTargetFolder2;
        private System.Windows.Forms.TextBox targetFolder2TextBox;
        private System.Windows.Forms.Label labelTargetFolder2;
        private System.Windows.Forms.ListView resultListView;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox summaryTextBox;
        private System.Windows.Forms.GroupBox fileModeGroupBox;
        private System.Windows.Forms.CheckBox fileNameCheckBox;
        private System.Windows.Forms.CheckBox fileSizeCheckBox;
        private System.Windows.Forms.CheckBox fileSHA1CheckBox;
        private System.Windows.Forms.CheckBox fileMD5CheckBox;
        private System.Windows.Forms.CheckBox fileExtNameCheckBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox ignoreFileCheckBox;
        private System.Windows.Forms.ComboBox ignoreFileSizecomboBox;
        private System.Windows.Forms.TextBox ignoreFileSizeTextBox;
        private System.Windows.Forms.ColumnHeader nameColumnHeader;
        private System.Windows.Forms.ColumnHeader modifiedTimeColumnHeader;
        private System.Windows.Forms.ColumnHeader sizeColumnHeader;
        private System.Windows.Forms.ColumnHeader extNameColumnHeader;
        public System.Windows.Forms.TextBox tbConsole;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ProgressBar geminiProgressBar;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.MenuStrip fileOptionsmenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unselectAllToolStripMenuItem;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.ToolStripMenuItem alwaysOnTopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveLogToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveResultToFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem usageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cleanNonExistentItemsToolStripMenuItem;
        private System.Windows.Forms.Button cleanUpButton;
        private System.Windows.Forms.ColumnHeader dirColumnHeader;
        private System.Windows.Forms.ColumnHeader hashColumnHeader;
        private System.Windows.Forms.ToolStripMenuItem reverseElectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redoToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip listViewContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyFullPathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDirectoryToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader CBcolumnHeader;
    }
}