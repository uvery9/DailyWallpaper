using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaper.Tools
{
    public partial class CommonCMDForm : Form
    {
        public CommonCMDForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon32x32;
            
            UpdateCMDText();
            var oldFont = cmdTextBox.Font;
            cmdTextBox.Font = new Font(oldFont.FontFamily, oldFont.Size + 1);
        }

        private void AppendText(int index, string cmd, string usage = "")
        {
            cmdTextBox.AppendText(" "+ index.ToString() + "." + usage + ":" + Environment.NewLine + "   ");
            cmdTextBox.AppendText(cmd + Environment.NewLine);
        }

        private void UpdateCMDText()
        {
            var cmdFile = Path.Combine(Helpers.ProjectInfo.executingLocation, "CommonCommands-UTF8.txt");
            if (File.Exists(cmdFile))
            {
                var lines = File.ReadAllLines(cmdFile);
                lines.ToList().ForEach(line => cmdTextBox.AppendText(line + Environment.NewLine));
                return;
            }
            AppendText(1, "ipconfig | find \"IPv4\"", "Query the local IPv4 address");
            AppendText(2, "mysql -u root -p", "Log in to mysql");
            AppendText(3, "use first_db;", "Use the database");
            AppendText(4, "tasklist | find /I \"DailyWall\"", "Find the process");
            AppendText(5, "taskkill /f /im DailyWall*   /t", "Kill the process");
            AppendText(6, "ping -n 4 127.0.0.1 > nul", "delay in .bat");
            AppendText(7, "Slmgr –dli", "Check if Your Windows License is Retail, OEM, or Volume");
            AppendText(8, "\"C:\\Program Files\\Git\\usr\\bin\\du.exe\" -sh $FOLDER", "Find total size of a directory");
            AppendText(9, "\"C:\\Program Files\\Git\\usr\\bin\\du.exe\" -sh *", "Find total size of each item in a directory");
            AppendText(10, "\"C:\\Program Files\\Git\\usr\\bin\\grep.exe\" -i -I -r 'SOMEWORD' .\r\n   grep -i -I -r 'SOMEWORD' . ", "grep find STRING recursively");
            cmdTextBox.AppendText(Environment.NewLine + Environment.NewLine  + " ### Record Common Commands to the file by yourself: " + 
                Environment.NewLine + "    " + cmdFile);
            File.WriteAllText(cmdFile, cmdTextBox.Text, Encoding.UTF8);
        }
    }
}
