using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;
using DailyWallpaper.Helpers;

namespace DailyWallpaper
{
    public partial class CleanEmptyFoldersForm : Form
    {
        private CleanEmptyFolders _cef;
        public CleanEmptyFoldersForm()
        {
            InitializeComponent();
            Icon = Properties.Resources.icon32x32;
            _cef = new CleanEmptyFolders();
        }

        private void btnSelectOutFolder_Click(object sender, EventArgs e)
        {
            /*  Note that you need to install the Microsoft.WindowsAPICodePack.Shell package 
                through NuGet before you can use this CommonOpenFileDialog
                
                VS->Tools->NuGet Package manager->Program Package Manager Terminal->
                Type: Install-Package Microsoft.WindowsAPICodePack-Shell    Enter 
                using Microsoft.WindowsAPICodePack.Dialogs;
            */

            using (var dialog = new CommonOpenFileDialog())
            {
                var localPathSetting = "";// _ini.GetCfgFromIni()["localPathSetting"];
                // Path.GetDirectoryName(Application.ExecutablePath)
                var deskTopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (!localPathSetting.ToLower().Equals("null") && Directory.Exists(localPathSetting))
                {
                    dialog.InitialDirectory = localPathSetting;
                }
                else
                {
                    dialog.InitialDirectory = deskTopPath;
                }
                dialog.IsFolderPicker = true;
                dialog.EnsurePathExists = true;
                dialog.Multiselect = false;
                dialog.Title = TranslationHelper.Get("Icon_CleanEmptyFolders");


                // maybe add some log
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok && !string.IsNullOrEmpty(dialog.FileName))
                {

                    tbTargetFolder.Text = dialog.FileName;
                    // _ini.UpdateIniItem("localPathSetting", pathSelected, "Local");
                }
            }
        }

        private void btnExtract_Click(object sender, EventArgs e)
        {

            // Invoke(new Action(Program.RunClean));
        }

        private void btnClear_Click(object sender, EventArgs e)
        {

            // Invoke(new Action(Program.RunClear));
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {

            //Invoke(new Action(Program.RunPrint));
        }        
        private void btnStop_Click(object sender, EventArgs e)
        {

            //Invoke(new Action(Program.InvokeStop));
        }


        private void tbTargetFolder_TextChanged(object sender, EventArgs e)
        {
            _cef.targetFolderPath = tbTargetFolder.Text;

            //var newPath = "path change to:" + tbTargetFolder.Text + "\r\n";
            //this.tbConsole.Text += newPath;
        }

    }
}
