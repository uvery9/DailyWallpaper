
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxCons
            // 
            this.textBoxCons.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.textBoxCons.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCons.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBoxCons.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.textBoxCons.Location = new System.Drawing.Point(3, 0);
            this.textBoxCons.Multiline = true;
            this.textBoxCons.Name = "textBoxCons";
            this.textBoxCons.ReadOnly = true;
            this.textBoxCons.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxCons.Size = new System.Drawing.Size(1011, 645);
            this.textBoxCons.TabIndex = 0;
            // 
            // clearButton
            // 
            this.clearButton.AutoSize = true;
            this.clearButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.clearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.clearButton.Location = new System.Drawing.Point(757, 609);
            this.clearButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(63, 28);
            this.clearButton.TabIndex = 1;
            this.clearButton.Text = "Clear";
            this.clearButton.UseVisualStyleBackColor = false;
            // 
            // saveToFileButton
            // 
            this.saveToFileButton.AutoSize = true;
            this.saveToFileButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(12)))), ((int)(((byte)(12)))));
            this.saveToFileButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.saveToFileButton.Location = new System.Drawing.Point(847, 609);
            this.saveToFileButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.saveToFileButton.Name = "saveToFileButton";
            this.saveToFileButton.Size = new System.Drawing.Size(126, 28);
            this.saveToFileButton.TabIndex = 3;
            this.saveToFileButton.Text = "Save to file";
            this.saveToFileButton.UseVisualStyleBackColor = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.clearButton);
            this.panel1.Controls.Add(this.saveToFileButton);
            this.panel1.Controls.Add(this.textBoxCons);
            this.panel1.Location = new System.Drawing.Point(1, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1014, 645);
            this.panel1.TabIndex = 4;
            // 
            // ConsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlText;
            this.ClientSize = new System.Drawing.Size(1016, 650);
            this.Controls.Add(this.panel1);
            this.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "ConsWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ConsWindow";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.TextBox textBoxCons;
        public System.Windows.Forms.Button clearButton;
        public System.Windows.Forms.Button saveToFileButton;
        private System.Windows.Forms.Panel panel1;
    }
}