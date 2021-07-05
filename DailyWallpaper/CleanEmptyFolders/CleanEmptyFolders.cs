using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class CleanEmptyFolders
    {
        public string targetFolderPath = null;
        public string helpString =
            "******************************************** USAGE ********************************************\r\n" +
           "1.\"Select\":                    Select / (TYPE + ENTER), PS: When you type, ENTER is NEEDED\r\n" +
           "2.\"Print\" :                    Show all the empty folders recursively\r\n" +
           "3.\"Clear\" :                    Clear screen.\r\n" +
           "4.\"STOP\"  :                    Stop Scanning\r\n" +
           "5.\"RecycleBin/Delete\" :        literally.\r\n" +                                   
           "6.\"Save list/log to File\":     literally.\r\n" +
           "7.\"Folder filter CHECKBOX:\":   use CHECKBOX select general/regex\r\n" +
           "8.\"Folder filter TEXTBOX\":\r\n" +
            "  8.1 Command mode:             TYPE then ENTER \r\n" +
            "                                        1) help  2) list controlled  3) re 4) find\r\n" +
            "  8.2 Word    mode:             JUST TYPE [ENTER: Yes but not needed]\r\n\r\n" +
            "SUM:   1) use RE CHECKBOX choose general/regex  2) use folder filter TEXTBOX type \" mode \"choose find/protect \r\n" +
            "                 Then you get GEN_FIND, GEN_PROTECT, REGEX_FIND, REGX_PROTECT   \r\n" +
            "******************************************** USAGE ********************************************\r\n";
        public ConfigIni ini;
        public List<string> controlledFolder1st;
        public List<string> controlledFolderAll;
        public CleanEmptyFolders()
        {
            ini = ConfigIni.GetInstance();
            controlledFolderAll = new List<string>
            {
                @"C:\Intel",
                @"C:\Temp",
                @"C:\Windows",
                @"And their all subfolders, such as C:\Windows\all\be\controlled"
            };

            controlledFolder1st = new List<string>
            {
                @"C:",
                @"C:\Program Files",
                @"C:\Program Files (x86)",
                @"C:\ProgramData",
                @"C:\Users",
                @"And their fist subfolder, such as C:\Users\SOMEONE"
            };
        }
        
        public List<string> GetAllControlledFolders()
        {
            var temp = controlledFolderAll;
            temp.AddRange(controlledFolder1st);
            return temp;
        }
        
        public bool IsControlledFolder(string path)
        {
            if (string.IsNullOrEmpty(path)) {
                return true;
            }
            // List<string> lowerCase = controlledFolder.Select(x => x.ToLower()).ToList();
            var controlledFolder1stLower = controlledFolder1st.ConvertAll(item => item.ToLower());
            var controlledFolderAllLower = controlledFolderAll.ConvertAll(item => item.ToLower());

            controlledFolder1stLower.AddRange(controlledFolderAllLower);

            while (path.EndsWith("\\"))
            {
                path = path.Substring(0, path.Length - 1);
            }
            // Completely matched.
            if (controlledFolder1stLower.Contains(path.ToLower()))
            {
                return true;
            }

            // first subfolder matched.
            var parent = Directory.GetParent(path);
            if (parent != null) // null when it's d:/e:/f:, only ban c:\
            {
                if (controlledFolder1stLower.Contains(parent.FullName.ToLower()))
                {
                    return true;
                }
            }

            // all subfolder matched.
            foreach (var it in controlledFolderAllLower)
            {
                if (path.ToLower().StartsWith(it))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
