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
           "1.\"Select\":                  Select/Type+ENTER\r\n" +
           "2.\"Print\" :                  Show all the empty folders recursively\r\n" +
           "3.\"Clear\" :                  Clear screen.\r\n" +
           "4.\"STOP\"  :                  Stop \r\n" +
           "5.\"RecycleBin/Delete\" :      literally.\r\n" +
           "6.\"Protection filter:\" :     use filter to select/not select the folder, \r\n "+
           "                                 use checkbox, trigger general/regex\r\n" +
           "7.\"filter mode\" :            Type on the Target Folder Textbox then trigger \r\n  " +
            "                                1)regex find, 2)regex protect , 3)general find, 4)general protect \r\n" +
           "8.\"Save list/log to File\":   literally.\r\n" +
            "******************************************** USAGE ********************************************\r\n";
        public ConfigIni ini;
        public List<string> controlledFolder1st;
        public List<string> controlledFolderAll;
        public CleanEmptyFolders()
        {
            ini = ConfigIni.GetInstance();
            controlledFolderAll = new List<string>();
            controlledFolderAll.Add(@"C:\Intel");
            controlledFolderAll.Add(@"C:\Temp");
            controlledFolderAll.Add(@"C:\Windows");
            controlledFolderAll.Add(@"And their all subfolders, such as C:\Windows\all\be\controlled");

            controlledFolder1st = new List<string>();
            controlledFolder1st.Add(@"C:");
            controlledFolder1st.Add(@"C:\Program Files");
            controlledFolder1st.Add(@"C:\Program Files (x86)");
            controlledFolder1st.Add(@"C:\ProgramData");
            controlledFolder1st.Add(@"C:\Users");
            controlledFolder1st.Add(@"And their fist subfolder, such as C:\Users\SOMEONE");
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
