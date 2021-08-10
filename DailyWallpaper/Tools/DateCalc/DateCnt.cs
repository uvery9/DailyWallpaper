using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper
{
    public partial class DateCnt : Form
    {
        public DateCnt()
        {
            InitializeComponent();
            Icon = Properties.Resources.Dc32x32;
            addSubComboBox.SelectedIndex = 0;
            addSubCheckBox.Checked = true;
            timeDiffCheckBox.Checked = true;
            addSubCheckBox.Click += (_, e) => { 
                if (!addSubCheckBox.Checked)
                {
                    addSubResTextBox.Text = "";
                }
                else
                {
                    updateButton.PerformClick();
                }
            };
            timeDiffCheckBox.Click += (_, e) => {
                if (!timeDiffCheckBox.Checked)
                {
                    timeDiffTextBox.Text = "";
                }
                else
                {
                    updateButton.PerformClick();
                }
                    
            };
            addSubComboBox.SelectedIndexChanged += (_, e) => updateButton.PerformClick();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (timeDiffCheckBox.Checked)
            {
                var daysDiff = (int)(rightDateTimePicker.Value - dateTimePickerOri.Value).TotalDays;
                var positiveOrNegative =
                    daysDiff < 0
                    ? "- " : "";
                var days = Math.Abs(daysDiff);
                string y = "";
                string m = "";
                var totalDays = "";
                if (days > 30)
                {
                    totalDays = $", {days} days";
                }
                if (days > 365)
                {
                    y = "" + (days / 365) + " yr ";
                    days -= days / 365 * 365;
                }
                if (days > 30)
                {
                    m = "" + (days / 30) + " mo ";
                    days -= days / 30 * 30;
                }
                string d = $"{days} day(s)";
                timeDiffTextBox.Text = $" {positiveOrNegative + y + m + d}{totalDays}";
            }
            if (addSubCheckBox.Checked)
            {
                var dest = dateTimePickerOri.Value;
                if (addSubComboBox.SelectedIndex == 0)
                {
                    if (int.TryParse(yearTextBox.Text, out int y))
                        dest = dest.AddYears(y);
                    if (int.TryParse(monthTextBox.Text, out int m))
                        dest = dest.AddMonths(m);
                    if (int.TryParse(dayTextBox.Text, out int d))
                        dest = dest.AddDays(d);
                }
                else
                {
                    if (int.TryParse(yearTextBox.Text, out int y))
                        dest = dest.AddYears(0 - y);
                    if (int.TryParse(monthTextBox.Text, out int m))
                        dest = dest.AddMonths(0 - m);
                    if (int.TryParse(dayTextBox.Text, out int d))
                        dest = dest.AddDays(0 - d);
                }
                addSubResTextBox.Text = $"{dest:yyyy/MM/dd ddd}";
                // addSubResTextBox.Text = $"{dateTimePickerOri.Value:yyyy 年 MM 月 dd 日}";
                
            }          
        }

        private void DateCntKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                updateButton.PerformClick();
            }
        }

        private void ymdTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // !char.IsControl(e.KeyChar) allow Enter.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) // && (e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void rightDateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            updateButton.PerformClick();
        }
    }
}
