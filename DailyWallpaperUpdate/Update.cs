using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DailyWallpaperUpdate
{
    public partial class Update : Form
    {
        public Update()
        {
            InitializeComponent(); 
            var li = ParseXml();
            if (li.Count == 3)
            {
                targetTextBox.Text = li[0];
                zipFileTextBox.Text = li[1];
                unzipPathTextBox.Text = li[2];
            }
        }

        private List<string> ParseXml()
        {
            var xmlFile = Path.Combine(Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location), Assembly.GetExecutingAssembly().GetName().Name + ".exe.xml");
            updateConsTextBox.AppendText(xmlFile + Environment.NewLine);
            if (!File.Exists(xmlFile))
                return new List<string>() { "DailyWallpaper.exe", "DailyWallpaper.Protable-latest.zip", "Some Path"};

            return new List<string>();
        }
    }
}
