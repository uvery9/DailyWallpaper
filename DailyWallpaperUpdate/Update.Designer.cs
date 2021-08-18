
namespace DailyWallpaperUpdate
{
    partial class Update
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.targetTextBox = new System.Windows.Forms.TextBox();
            this.zipFileTextBox = new System.Windows.Forms.TextBox();
            this.unzipPathTextBox = new System.Windows.Forms.TextBox();
            this.updateConsTextBox = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // targetTextBox
            // 
            this.targetTextBox.Location = new System.Drawing.Point(140, 16);
            this.targetTextBox.Name = "targetTextBox";
            this.targetTextBox.Size = new System.Drawing.Size(195, 28);
            this.targetTextBox.TabIndex = 1;
            // 
            // zipFileTextBox
            // 
            this.zipFileTextBox.Location = new System.Drawing.Point(140, 74);
            this.zipFileTextBox.Name = "zipFileTextBox";
            this.zipFileTextBox.Size = new System.Drawing.Size(522, 28);
            this.zipFileTextBox.TabIndex = 2;
            // 
            // unzipPathTextBox
            // 
            this.unzipPathTextBox.Location = new System.Drawing.Point(140, 131);
            this.unzipPathTextBox.Name = "unzipPathTextBox";
            this.unzipPathTextBox.Size = new System.Drawing.Size(522, 28);
            this.unzipPathTextBox.TabIndex = 4;
            // 
            // updateConsTextBox
            // 
            this.updateConsTextBox.Location = new System.Drawing.Point(12, 182);
            this.updateConsTextBox.Multiline = true;
            this.updateConsTextBox.Name = "updateConsTextBox";
            this.updateConsTextBox.ReadOnly = true;
            this.updateConsTextBox.Size = new System.Drawing.Size(776, 199);
            this.updateConsTextBox.TabIndex = 0;
            this.updateConsTextBox.TabStop = false;
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox5.Location = new System.Drawing.Point(24, 19);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(100, 21);
            this.textBox5.TabIndex = 0;
            this.textBox5.TabStop = false;
            this.textBox5.Text = "Target:";
            // 
            // textBox6
            // 
            this.textBox6.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox6.Location = new System.Drawing.Point(24, 77);
            this.textBox6.Name = "textBox6";
            this.textBox6.ReadOnly = true;
            this.textBox6.Size = new System.Drawing.Size(100, 21);
            this.textBox6.TabIndex = 0;
            this.textBox6.TabStop = false;
            this.textBox6.Text = "Zip File:";
            // 
            // textBox7
            // 
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox7.Location = new System.Drawing.Point(24, 134);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(100, 21);
            this.textBox7.TabIndex = 0;
            this.textBox7.TabStop = false;
            this.textBox7.Text = "Unzip Path:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(696, 10);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "Update";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(696, 73);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 27);
            this.button2.TabIndex = 3;
            this.button2.Text = "Browser";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(696, 130);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(92, 27);
            this.button3.TabIndex = 5;
            this.button3.Text = "Browser";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 402);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.updateConsTextBox);
            this.Controls.Add(this.unzipPathTextBox);
            this.Controls.Add(this.zipFileTextBox);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.targetTextBox);
            this.Name = "Update";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox targetTextBox;
        private System.Windows.Forms.TextBox zipFileTextBox;
        private System.Windows.Forms.TextBox unzipPathTextBox;
        private System.Windows.Forms.TextBox updateConsTextBox;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
    }
}

