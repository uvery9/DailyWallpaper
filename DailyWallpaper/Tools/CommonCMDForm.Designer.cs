﻿
namespace DailyWallpaper.Tools
{
    partial class CommonCMDForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.cmdTextBox = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.cmdTextBox);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(888, 537);
            this.panel1.TabIndex = 0;
            // 
            // cmdTextBox
            // 
            this.cmdTextBox.Location = new System.Drawing.Point(3, 0);
            this.cmdTextBox.Multiline = true;
            this.cmdTextBox.Name = "cmdTextBox";
            this.cmdTextBox.ReadOnly = true;
            this.cmdTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.cmdTextBox.Size = new System.Drawing.Size(885, 534);
            this.cmdTextBox.TabIndex = 0;
            this.cmdTextBox.TabStop = false;
            // 
            // CommonCMDForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(912, 561);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.Name = "CommonCMDForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CommonCMD";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox cmdTextBox;
    }
}