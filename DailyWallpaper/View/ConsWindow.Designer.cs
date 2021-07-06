
namespace DailyWallpaper.View
{
    partial class ConsWindow
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
            this.textBoxCons = new System.Windows.Forms.TextBox();
            this.clearButton = new System.Windows.Forms.Button();
            this.saveToFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxCons
            // 
            this.textBoxCons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.textBoxCons.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCons.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxCons.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.textBoxCons.Location = new System.Drawing.Point(0, 0);
            this.textBoxCons.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBoxCons.Multiline = true;
            this.textBoxCons.Name = "textBoxCons";
            this.textBoxCons.ReadOnly = true;
            this.textBoxCons.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxCons.Size = new System.Drawing.Size(1013, 451);
            this.textBoxCons.TabIndex = 0;
            // 
            // clearButton
            // 
            this.clearButton.AutoSize = true;
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.clearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.clearButton.Location = new System.Drawing.Point(666, 433);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(83, 41);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = false;

            // 
            // saveToFileButton
            // 
            this.saveToFileButton.AutoSize = true;
            this.saveToFileButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.saveToFileButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.saveToFileButton.Location = new System.Drawing.Point(783, 433);
            this.saveToFileButton.Name = "saveToFileButton";
            this.saveToFileButton.Size = new System.Drawing.Size(133, 41);
            this.saveToFileButton.TabIndex = 3;
            this.saveToFileButton.Text = "Save to file";
            this.saveToFileButton.UseVisualStyleBackColor = false;

            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(13F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(1013, 481);
            this.Controls.Add(this.saveToFileButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.textBoxCons);
            this.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ConsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ConsWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxCons;
        public System.Windows.Forms.Button clearButton;
        public System.Windows.Forms.Button saveToFileButton;
    }
}