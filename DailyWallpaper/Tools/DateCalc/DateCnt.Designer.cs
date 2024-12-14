
namespace DailyWallpaper
{
    partial class DateCnt
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
            this.dateTimePickerOri = new System.Windows.Forms.DateTimePicker();
            this.yearTextBox = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.dayTextBox = new System.Windows.Forms.TextBox();
            this.monthTextBox = new System.Windows.Forms.TextBox();
            this.rightDateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.textBox14 = new System.Windows.Forms.TextBox();
            this.addSubComboBox = new System.Windows.Forms.ComboBox();
            this.addSubResTextBox = new System.Windows.Forms.TextBox();
            this.timeDiffTextBox = new System.Windows.Forms.TextBox();
            this.updateButton = new System.Windows.Forms.Button();
            this.addSubCheckBox = new System.Windows.Forms.CheckBox();
            this.timeDiffCheckBox = new System.Windows.Forms.CheckBox();
            this.hmDiffTextBox = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.targetHourTextBox = new System.Windows.Forms.TextBox();
            this.targetMinsTextBox = new System.Windows.Forms.TextBox();
            this.nowHMTextBox = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // dateTimePickerOri
            // 
            this.dateTimePickerOri.Location = new System.Drawing.Point(22, 51);
            this.dateTimePickerOri.Name = "dateTimePickerOri";
            this.dateTimePickerOri.Size = new System.Drawing.Size(200, 28);
            this.dateTimePickerOri.TabIndex = 0;
            this.dateTimePickerOri.TabStop = false;
            // 
            // yearTextBox
            // 
            this.yearTextBox.Location = new System.Drawing.Point(178, 112);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(44, 28);
            this.yearTextBox.TabIndex = 1;
            this.yearTextBox.Text = "0";
            this.yearTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.yearTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DateCntKeyDown);
            this.yearTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ymdTextBox_KeyPress);
            // 
            // textBox4
            // 
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox4.Location = new System.Drawing.Point(228, 115);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(27, 21);
            this.textBox4.TabIndex = 4;
            this.textBox4.TabStop = false;
            this.textBox4.Text = "yr";
            // 
            // textBox7
            // 
            this.textBox7.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox7.Location = new System.Drawing.Point(320, 115);
            this.textBox7.Name = "textBox7";
            this.textBox7.ReadOnly = true;
            this.textBox7.Size = new System.Drawing.Size(27, 21);
            this.textBox7.TabIndex = 7;
            this.textBox7.TabStop = false;
            this.textBox7.Text = "mo";
            // 
            // textBox8
            // 
            this.textBox8.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox8.Location = new System.Drawing.Point(403, 115);
            this.textBox8.Name = "textBox8";
            this.textBox8.ReadOnly = true;
            this.textBox8.Size = new System.Drawing.Size(27, 21);
            this.textBox8.TabIndex = 8;
            this.textBox8.TabStop = false;
            this.textBox8.Text = "day";
            // 
            // dayTextBox
            // 
            this.dayTextBox.Location = new System.Drawing.Point(353, 112);
            this.dayTextBox.Name = "dayTextBox";
            this.dayTextBox.Size = new System.Drawing.Size(44, 28);
            this.dayTextBox.TabIndex = 9;
            this.dayTextBox.Text = "0";
            this.dayTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.dayTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DateCntKeyDown);
            this.dayTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ymdTextBox_KeyPress);
            // 
            // monthTextBox
            // 
            this.monthTextBox.Location = new System.Drawing.Point(261, 112);
            this.monthTextBox.Name = "monthTextBox";
            this.monthTextBox.Size = new System.Drawing.Size(44, 28);
            this.monthTextBox.TabIndex = 10;
            this.monthTextBox.Text = "0";
            this.monthTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.monthTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DateCntKeyDown);
            this.monthTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ymdTextBox_KeyPress);
            // 
            // rightDateTimePicker
            // 
            this.rightDateTimePicker.Location = new System.Drawing.Point(63, 181);
            this.rightDateTimePicker.Name = "rightDateTimePicker";
            this.rightDateTimePicker.Size = new System.Drawing.Size(200, 28);
            this.rightDateTimePicker.TabIndex = 11;
            this.rightDateTimePicker.ValueChanged += new System.EventHandler(this.rightDateTimePicker_ValueChanged);
            // 
            // textBox14
            // 
            this.textBox14.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox14.Location = new System.Drawing.Point(294, 185);
            this.textBox14.Name = "textBox14";
            this.textBox14.ReadOnly = true;
            this.textBox14.Size = new System.Drawing.Size(60, 21);
            this.textBox14.TabIndex = 4;
            this.textBox14.TabStop = false;
            this.textBox14.Text = "diff";
            this.textBox14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // addSubComboBox
            // 
            this.addSubComboBox.FormattingEnabled = true;
            this.addSubComboBox.Items.AddRange(new object[] {
            "(+) Add",
            "(-) Sub"});
            this.addSubComboBox.Location = new System.Drawing.Point(63, 112);
            this.addSubComboBox.Name = "addSubComboBox";
            this.addSubComboBox.Size = new System.Drawing.Size(94, 26);
            this.addSubComboBox.TabIndex = 18;
            this.addSubComboBox.TabStop = false;
            this.addSubComboBox.Text = "(+)Add";
            // 
            // addSubResTextBox
            // 
            this.addSubResTextBox.Location = new System.Drawing.Point(444, 112);
            this.addSubResTextBox.Name = "addSubResTextBox";
            this.addSubResTextBox.ReadOnly = true;
            this.addSubResTextBox.Size = new System.Drawing.Size(214, 28);
            this.addSubResTextBox.TabIndex = 19;
            // 
            // timeDiffTextBox
            // 
            this.timeDiffTextBox.Location = new System.Drawing.Point(360, 185);
            this.timeDiffTextBox.Name = "timeDiffTextBox";
            this.timeDiffTextBox.ReadOnly = true;
            this.timeDiffTextBox.Size = new System.Drawing.Size(298, 28);
            this.timeDiffTextBox.TabIndex = 20;
            this.timeDiffTextBox.Text = "1 yr 2 mo 20 day(s), 445 days";
            this.timeDiffTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(282, 225);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(91, 34);
            this.updateButton.TabIndex = 0;
            this.updateButton.Text = "Update";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // addSubCheckBox
            // 
            this.addSubCheckBox.AutoSize = true;
            this.addSubCheckBox.Location = new System.Drawing.Point(22, 113);
            this.addSubCheckBox.Name = "addSubCheckBox";
            this.addSubCheckBox.Size = new System.Drawing.Size(22, 21);
            this.addSubCheckBox.TabIndex = 22;
            this.addSubCheckBox.UseVisualStyleBackColor = true;
            // 
            // timeDiffCheckBox
            // 
            this.timeDiffCheckBox.AutoSize = true;
            this.timeDiffCheckBox.Location = new System.Drawing.Point(22, 188);
            this.timeDiffCheckBox.Name = "timeDiffCheckBox";
            this.timeDiffCheckBox.Size = new System.Drawing.Size(22, 21);
            this.timeDiffCheckBox.TabIndex = 22;
            this.timeDiffCheckBox.UseVisualStyleBackColor = true;
            // 
            // hmDiffTextBox
            // 
            this.hmDiffTextBox.Location = new System.Drawing.Point(526, 51);
            this.hmDiffTextBox.Name = "hmDiffTextBox";
            this.hmDiffTextBox.ReadOnly = true;
            this.hmDiffTextBox.Size = new System.Drawing.Size(100, 28);
            this.hmDiffTextBox.TabIndex = 23;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(436, 54);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(19, 21);
            this.textBox2.TabIndex = 23;
            this.textBox2.Text = ":";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // targetHourTextBox
            // 
            this.targetHourTextBox.Location = new System.Drawing.Point(403, 51);
            this.targetHourTextBox.Name = "targetHourTextBox";
            this.targetHourTextBox.Size = new System.Drawing.Size(32, 28);
            this.targetHourTextBox.TabIndex = 23;
            this.targetHourTextBox.Text = "12";
            this.targetHourTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.targetHourTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ymdTextBox_KeyPress);
            // 
            // targetMinsTextBox
            // 
            this.targetMinsTextBox.Location = new System.Drawing.Point(461, 51);
            this.targetMinsTextBox.Name = "targetMinsTextBox";
            this.targetMinsTextBox.Size = new System.Drawing.Size(32, 28);
            this.targetMinsTextBox.TabIndex = 23;
            this.targetMinsTextBox.Text = "20";
            this.targetMinsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.targetMinsTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ymdTextBox_KeyPress);
            // 
            // nowHMTextBox
            // 
            this.nowHMTextBox.Location = new System.Drawing.Point(294, 51);
            this.nowHMTextBox.Name = "nowHMTextBox";
            this.nowHMTextBox.ReadOnly = true;
            this.nowHMTextBox.Size = new System.Drawing.Size(60, 28);
            this.nowHMTextBox.TabIndex = 23;
            this.nowHMTextBox.Text = "09:45";
            this.nowHMTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Location = new System.Drawing.Point(369, 54);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(19, 21);
            this.textBox1.TabIndex = 23;
            this.textBox1.Text = "to";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox3.Location = new System.Drawing.Point(499, 54);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(19, 21);
            this.textBox3.TabIndex = 23;
            this.textBox3.Text = "=";
            this.textBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DateCnt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(670, 271);
            this.Controls.Add(this.targetMinsTextBox);
            this.Controls.Add(this.targetHourTextBox);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.nowHMTextBox);
            this.Controls.Add(this.hmDiffTextBox);
            this.Controls.Add(this.timeDiffCheckBox);
            this.Controls.Add(this.addSubCheckBox);
            this.Controls.Add(this.updateButton);
            this.Controls.Add(this.timeDiffTextBox);
            this.Controls.Add(this.addSubResTextBox);
            this.Controls.Add(this.addSubComboBox);
            this.Controls.Add(this.rightDateTimePicker);
            this.Controls.Add(this.monthTextBox);
            this.Controls.Add(this.dayTextBox);
            this.Controls.Add(this.textBox8);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox14);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.yearTextBox);
            this.Controls.Add(this.dateTimePickerOri);
            this.Name = "DateCnt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Date Calculator";
            this.VisibleChanged += new System.EventHandler(this.DateCnt_VisibleChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePickerOri;
        private System.Windows.Forms.TextBox yearTextBox;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox dayTextBox;
        private System.Windows.Forms.TextBox monthTextBox;
        private System.Windows.Forms.DateTimePicker rightDateTimePicker;
        private System.Windows.Forms.TextBox textBox14;
        private System.Windows.Forms.ComboBox addSubComboBox;
        private System.Windows.Forms.TextBox addSubResTextBox;
        private System.Windows.Forms.TextBox timeDiffTextBox;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.CheckBox addSubCheckBox;
        private System.Windows.Forms.CheckBox timeDiffCheckBox;
        private System.Windows.Forms.TextBox hmDiffTextBox;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox targetHourTextBox;
        private System.Windows.Forms.TextBox targetMinsTextBox;
        private System.Windows.Forms.TextBox nowHMTextBox;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox3;
    }
}