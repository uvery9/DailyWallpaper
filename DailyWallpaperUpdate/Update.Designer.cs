
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
            this.updateButton = new System.Windows.Forms.Button();
            this.zipFileButton = new System.Windows.Forms.Button();
            this.unzipPathButton = new System.Windows.Forms.Button();
            this.readOnlyCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // targetTextBox
            // 
            this.targetTextBox.Location = new System.Drawing.Point(140, 16);
            this.targetTextBox.Name = "targetTextBox";
            this.targetTextBox.Size = new System.Drawing.Size(229, 28);
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
            this.updateConsTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
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
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(696, 10);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(91, 36);
            this.updateButton.TabIndex = 0;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // zipFileButton
            // 
            this.zipFileButton.Location = new System.Drawing.Point(696, 73);
            this.zipFileButton.Name = "zipFileButton";
            this.zipFileButton.Size = new System.Drawing.Size(91, 27);
            this.zipFileButton.TabIndex = 3;
            this.zipFileButton.Text = "Browser";
            this.zipFileButton.UseVisualStyleBackColor = true;
            // 
            // unzipPathButton
            // 
            this.unzipPathButton.Location = new System.Drawing.Point(696, 130);
            this.unzipPathButton.Name = "unzipPathButton";
            this.unzipPathButton.Size = new System.Drawing.Size(92, 27);
            this.unzipPathButton.TabIndex = 5;
            this.unzipPathButton.Text = "Browser";
            this.unzipPathButton.UseVisualStyleBackColor = true;
            // 
            // readOnlyCheckBox
            // 
            this.readOnlyCheckBox.AutoSize = true;
            this.readOnlyCheckBox.Location = new System.Drawing.Point(393, 18);
            this.readOnlyCheckBox.Name = "readOnlyCheckBox";
            this.readOnlyCheckBox.Size = new System.Drawing.Size(115, 22);
            this.readOnlyCheckBox.TabIndex = 6;
            this.readOnlyCheckBox.Text = "Read Only";
            this.readOnlyCheckBox.UseVisualStyleBackColor = true;
            this.readOnlyCheckBox.Click += new System.EventHandler(this.readOnlyCheckBox_Click);
            // 
            // Update
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 402);
            this.Controls.Add(this.readOnlyCheckBox);
            this.Controls.Add(this.unzipPathButton);
            this.Controls.Add(this.zipFileButton);
            this.Controls.Add(this.updateButton);
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
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button zipFileButton;
        private System.Windows.Forms.Button unzipPathButton;
        private System.Windows.Forms.CheckBox readOnlyCheckBox;
    }
}

