
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashCalcForm));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.hashPicBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.hashPicBox)).BeginInit();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(101, 289);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(331, 28);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(101, 406);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(331, 28);
            this.textBox2.TabIndex = 0;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(101, 479);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(331, 28);
            this.textBox3.TabIndex = 0;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(101, 562);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(331, 28);
            this.textBox4.TabIndex = 0;
            // 
            // hashPicBox
            // 
            this.hashPicBox.Image = ((System.Drawing.Image)(resources.GetObject("hashPicBox.Image")));
            this.hashPicBox.Location = new System.Drawing.Point(140, 22);
            this.hashPicBox.Name = "hashPicBox";
            this.hashPicBox.Size = new System.Drawing.Size(250, 250);
            this.hashPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.hashPicBox.TabIndex = 1;
            this.hashPicBox.TabStop = false;
            this.hashPicBox.Click += new System.EventHandler(this.hashPicBox_Click);
            this.hashPicBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragDrop);
            this.hashPicBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.hashPicBox_DragEnter);
            // 
            // HashCalcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(517, 633);
            this.Controls.Add(this.hashPicBox);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Name = "HashCalcForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hash Calculator";
            this.TopMost = true;
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.HashCalcForm_DragDrop);
            this.DragEnter += new System.Windows.Forms.DragEventHandler(this.HashCalcForm_DragEnter);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.HashCalcForm_DragOver);
            ((System.ComponentModel.ISupportInitialize)(this.hashPicBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.PictureBox hashPicBox;
    }
}