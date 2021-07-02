using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper.Helpers
{
    class CallConsole
    {
        public static string Md5SumByProcess(string file, string exexPath = "md5sum.exe")
        {
            var p = new Process();
            p.StartInfo.FileName = exexPath;
            p.StartInfo.Arguments = file;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit();
            string output = p.StandardOutput.ReadToEnd();
            return output.Split(' ')[0].Substring(1).ToUpper();
        }
    }
}
