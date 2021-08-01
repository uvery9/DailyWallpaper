
namespace DailyWallpaper.Tools.ShutdownTimer
{
    partial class Shutdown
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
            this.cancelButton = new System.Windows.Forms.Button();
            this.shutdownButton = new System.Windows.Forms.Button();
            this.timerTextBox = new System.Windows.Forms.TextBox();
            this.consTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.timeLeftTextBox = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.timerComboBox = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(308, 16);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 33);
            this.cancelButton.TabIndex = 0;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // shutdownButton
            // 
            this.shutdownButton.Location = new System.Drawing.Point(199, 16);
            this.shutdownButton.Name = "shutdownButton";
            this.shutdownButton.Size = new System.Drawing.Size(101, 33);
            this.shutdownButton.TabIndex = 0;
            this.shutdownButton.Text = "Shutdown";
            this.shutdownButton.UseVisualStyleBackColor = true;
            this.shutdownButton.Click += new System.EventHandler(this.shutdownButton_Click);
            // 
            // timerTextBox
            // 
            this.timerTextBox.Location = new System.Drawing.Point(12, 20);
            this.timerTextBox.Name = "timerTextBox";
            this.timerTextBox.Size = new System.Drawing.Size(81, 28);
            this.timerTextBox.TabIndex = 1;
            this.timerTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.timerTextBox_KeyDown);
            this.timerTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.timerTextBox_KeyPress);
            // 
            // consTextBox
            // 
            this.consTextBox.Location = new System.Drawing.Point(12, 60);
            this.consTextBox.Multiline = true;
            this.consTextBox.Name = "consTextBox";
            this.consTextBox.ReadOnly = true;
            this.consTextBox.Size = new System.Drawing.Size(440, 121);
            this.consTextBox.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBox2);
            this.panel1.Controls.Add(this.timeLeftTextBox);
            this.panel1.Controls.Add(this.clearButton);
            this.panel1.Controls.Add(this.timerComboBox);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.shutdownButton);
            this.panel1.Controls.Add(this.timerTextBox);
            this.panel1.Controls.Add(this.consTextBox);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(468, 225);
            this.panel1.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(199, 190);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(90, 21);
            this.textBox2.TabIndex = 10;
            this.textBox2.Text = "Time left:";
            // 
            // timeLeftTextBox
            // 
            this.timeLeftTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.timeLeftTextBox.Location = new System.Drawing.Point(295, 190);
            this.timeLeftTextBox.Name = "timeLeftTextBox";
            this.timeLeftTextBox.ReadOnly = true;
            this.timeLeftTextBox.Size = new System.Drawing.Size(157, 21);
            this.timeLeftTextBox.TabIndex = 9;
            this.timeLeftTextBox.Text = "00:00:00 (17:52)";
            this.timeLeftTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // clearButton
            // 
            this.clearButton.Location = new System.Drawing.Point(388, 20);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(57, 25);
            this.clearButton.TabIndex = 8;
            this.clearButton.Text = "cls";
            this.clearButton.UseVisualStyleBackColor = true;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // timerComboBox
            // 
            this.timerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.timerComboBox.FormattingEnabled = true;
            this.timerComboBox.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.timerComboBox.Items.AddRange(new object[] {
            "mins",
            "hours"});
            this.timerComboBox.Location = new System.Drawing.Point(108, 20);
            this.timerComboBox.MaxDropDownItems = 3;
            this.timerComboBox.Name = "timerComboBox";
            this.timerComboBox.Size = new System.Drawing.Size(80, 26);
            this.timerComboBox.TabIndex = 7;
            this.timerComboBox.TabStop = false;
            this.timerComboBox.SelectedIndexChanged += new System.EventHandler(this.timerComboBox_SelectedIndexChanged);
            // 
            // Shutdown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(487, 245);
            this.Controls.Add(this.panel1);
            this.Name = "Shutdown";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shutdown";
            this.VisibleChanged += new System.EventHandler(this.Shutdown_VisibleChanged);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button shutdownButton;
        private System.Windows.Forms.TextBox timerTextBox;
        private System.Windows.Forms.TextBox consTextBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox timerComboBox;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.TextBox timeLeftTextBox;
        private System.Windows.Forms.TextBox textBox2;
    }
}