using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace DailyWallpaper.Tools.ShutdownTimer
{
    public partial class Shutdown : Form
    {
        private TextBoxCons _console;
        private bool shutdownTimeSet = false;
        private DateTime shutdownTime;
        private System.Timers.Timer _updateTimer;
        public Shutdown()
        {
            InitializeComponent();
            timerComboBox.SelectedIndex = 0; // mins
            timerTextBox.Text = "30";        
            _console = new TextBoxCons(new ConsWriter(consTextBox));
            timeLeftTextBox.Text = "00:00:00";
            UpdateTimeLeftCallerTimerInit();
        }

        private void timerTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // SetMinimumFileLimit();
                shutdownButton.PerformClick();
            }
        }

        private void timerTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // !char.IsControl(e.KeyChar) allow Enter.
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) // && (e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void shutdownButton_Click(object sender, EventArgs e)
        {
            if (int.TryParse(timerTextBox.Text, out int ret)) {
                var raw_ret = ret;
                string unit;
                if (timerComboBox.SelectedIndex == 0)
                {
                    ret *= 60; // mins
                    unit = "分钟";
                }
                else
                {
                    ret *= 3600; // hours
                    unit = "小时";
                }
                var cmd = string.Format(" -s -t {0}", ret);
                shutdownTime = DateTime.Now.AddSeconds(ret);
                _console.WriteLine($"> {raw_ret} {unit}后关机, 关机时间: {shutdownTime:H:mm}");
                _console.WriteLine("> CMD: shutdown.exe " + cmd);
                shutdownTimeSet = true;
                _updateTimer.Enabled = true;
                System.Diagnostics.Process.Start("shutdown.exe", cmd);
            }
            else
            {
                timerTextBox.Text = "输入有误";
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("shutdown.exe", " -a");
            //_console.WriteLine
            //    ("> " + DateTime.Now.ToString() + " 任务已取消");
            _console.WriteLine
                ("> 关机任务已取消");
            shutdownTimeSet = false;
            timeLeftTextBox.Text = "暂未设置";
        }

        private void clearButton_Click(object sender, EventArgs e)
        {
            consTextBox.Clear();
        }

        private void UpdateTimeLeftCallerTimerInit()
        {
            _updateTimer = new System.Timers.Timer
            {
                Interval = 1000, // update by seconds
                AutoReset = true,
                Enabled = false
            };
            // _timer.
            _updateTimer.Elapsed += UpdateShutdownTimeLeft_Elapsed;
            _updateTimer.Start();
        }

        private void UpdateShutdownTimeLeft_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (shutdownTimeSet && Visible)
            {
                var timeLeft = (int)(shutdownTime - DateTime.Now).TotalSeconds;
                var hours = timeLeft / 3600;
                timeLeft -= hours * 3600;
                var mins = timeLeft / 60;
                timeLeft -= mins * 60;
                var secs = timeLeft;
                var timeLeftStr = $"{hours:D2}:{mins:D2}:{secs:D2}";
                timeLeftTextBox.Text = timeLeftStr;
            }
            //if (!Visible)
            //{
            //    // MessageBox.Show("Not Visible");
            //    _console.WriteLine($"{DateTime.Now}: No Visible");
            //}
        }

        private void timerComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            shutdownButton.PerformClick();
        }

        private void Shutdown_VisibleChanged(object sender, EventArgs e)
        {
            if (shutdownTimeSet && Visible)
                _updateTimer.Enabled = true;
            else
                _updateTimer.Enabled = false;
        }
    }
}
