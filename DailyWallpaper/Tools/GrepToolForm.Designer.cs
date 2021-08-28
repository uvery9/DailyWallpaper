
namespace DailyWallpaper.Tools
{
    partial class GrepToolForm
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
            this.grepLocationTextBox = new System.Windows.Forms.TextBox();
            this.grepLocateButton = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.targetFolderTextBox = new System.Windows.Forms.TextBox();
            this.targetFolderTag = new System.Windows.Forms.TextBox();
            this.targetFolderBrowserButton = new System.Windows.Forms.Button();
            this.argsTextBox = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.runButton = new System.Windows.Forms.Button();
            this.consRichTextBox = new System.Windows.Forms.RichTextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.stringTextBox = new System.Windows.Forms.TextBox();
            this.stringTag = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // grepLocationTextBox
            // 
            this.grepLocationTextBox.Location = new System.Drawing.Point(91, 23);
            this.grepLocationTextBox.Name = "grepLocationTextBox";
            this.grepLocationTextBox.Size = new System.Drawing.Size(600, 28);
            this.grepLocationTextBox.TabIndex = 1;
            this.grepLocationTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grepLocationTextBox_KeyDown);
            // 
            // grepLocateButton
            // 
            this.grepLocateButton.Location = new System.Drawing.Point(705, 19);
            this.grepLocateButton.Name = "grepLocateButton";
            this.grepLocateButton.Size = new System.Drawing.Size(80, 28);
            this.grepLocateButton.TabIndex = 1;
            this.grepLocateButton.Text = "Locate";
            this.grepLocateButton.UseVisualStyleBackColor = true;
            this.grepLocateButton.Click += new System.EventHandler(this.grepLocateButton_Click);
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(24, 26);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(56, 21);
            this.textBox2.TabIndex = 0;
            this.textBox2.TabStop = false;
            this.textBox2.Text = "grep:";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // targetFolderTextBox
            // 
            this.targetFolderTextBox.Location = new System.Drawing.Point(91, 70);
            this.targetFolderTextBox.Name = "targetFolderTextBox";
            this.targetFolderTextBox.Size = new System.Drawing.Size(600, 28);
            this.targetFolderTextBox.TabIndex = 2;
            // 
            // targetFolderTag
            // 
            this.targetFolderTag.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.targetFolderTag.Location = new System.Drawing.Point(11, 75);
            this.targetFolderTag.Name = "targetFolderTag";
            this.targetFolderTag.ReadOnly = true;
            this.targetFolderTag.Size = new System.Drawing.Size(69, 21);
            this.targetFolderTag.TabIndex = 0;
            this.targetFolderTag.TabStop = false;
            this.targetFolderTag.Text = "target:";
            this.targetFolderTag.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // targetFolderBrowserButton
            // 
            this.targetFolderBrowserButton.Location = new System.Drawing.Point(705, 71);
            this.targetFolderBrowserButton.Name = "targetFolderBrowserButton";
            this.targetFolderBrowserButton.Size = new System.Drawing.Size(80, 27);
            this.targetFolderBrowserButton.TabIndex = 3;
            this.targetFolderBrowserButton.Text = "Browser";
            this.targetFolderBrowserButton.UseVisualStyleBackColor = true;
            this.targetFolderBrowserButton.Click += new System.EventHandler(this.targetFolderBrowserButton_Click);
            // 
            // argsTextBox
            // 
            this.argsTextBox.Location = new System.Drawing.Point(91, 122);
            this.argsTextBox.Name = "argsTextBox";
            this.argsTextBox.Size = new System.Drawing.Size(157, 28);
            this.argsTextBox.TabIndex = 4;
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Location = new System.Drawing.Point(11, 125);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(69, 21);
            this.textBox5.TabIndex = 0;
            this.textBox5.TabStop = false;
            this.textBox5.Text = "args:";
            this.textBox5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // runButton
            // 
            this.runButton.Location = new System.Drawing.Point(705, 116);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(80, 36);
            this.runButton.TabIndex = 0;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // consRichTextBox
            // 
            this.consRichTextBox.Location = new System.Drawing.Point(12, 169);
            this.consRichTextBox.Name = "consRichTextBox";
            this.consRichTextBox.Size = new System.Drawing.Size(776, 269);
            this.consRichTextBox.TabIndex = 2;
            this.consRichTextBox.Text = "";
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(624, 121);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(67, 27);
            this.clearButton.TabIndex = 5;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // stringTextBox
            // 
            this.stringTextBox.Location = new System.Drawing.Point(345, 122);
            this.stringTextBox.Name = "stringTextBox";
            this.stringTextBox.Size = new System.Drawing.Size(263, 28);
            this.stringTextBox.TabIndex = 4;
            // 
            // stringTag
            // 
            this.stringTag.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.stringTag.Location = new System.Drawing.Point(270, 125);
            this.stringTag.Name = "stringTag";
            this.stringTag.ReadOnly = true;
            this.stringTag.Size = new System.Drawing.Size(69, 21);
            this.stringTag.TabIndex = 0;
            this.stringTag.TabStop = false;
            this.stringTag.Text = "string:";
            this.stringTag.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // GrepToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.runButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.targetFolderBrowserButton);
            this.Controls.Add(this.grepLocateButton);
            this.Controls.Add(this.stringTag);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.targetFolderTag);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.stringTextBox);
            this.Controls.Add(this.argsTextBox);
            this.Controls.Add(this.targetFolderTextBox);
            this.Controls.Add(this.grepLocationTextBox);
            this.Controls.Add(this.consRichTextBox);
            this.Name = "GrepToolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Grep Tool";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox grepLocationTextBox;
        private System.Windows.Forms.Button grepLocateButton;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox targetFolderTextBox;
        private System.Windows.Forms.TextBox targetFolderTag;
        private System.Windows.Forms.Button targetFolderBrowserButton;
        private System.Windows.Forms.TextBox argsTextBox;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.RichTextBox consRichTextBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.TextBox stringTextBox;
        private System.Windows.Forms.TextBox stringTag;
    }
}