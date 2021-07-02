using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DailyWallpaper.Helpers;

namespace DailyWallpaper.View
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
            this.aboutWindowWebsite.Text = Helpers.ProjectInfo.OfficalWebSite;
            this.Icon = Properties.Resources.icon32x32;
        }

        private void aboutWindowWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Helpers.ProjectInfo.OfficalWebSite);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void GPLtextBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
