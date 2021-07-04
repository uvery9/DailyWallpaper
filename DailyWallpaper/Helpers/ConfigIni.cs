using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace DailyWallpaper
{    public class ConfigIni
    {
        public static ConfigIni _instance;
        private string iniPath;
        private string logIniPath;
        public string exeName;

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string Section, string Key, string Value, string FilePath);

        [DllImport("kernel32", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string Section, string Key, string Default, StringBuilder RetVal, int Size, string FilePath);

        private ConfigIni(string congfigOfLog="Config", string IniPath = null, string exeName=null)
        {
            this.exeName = exeName ?? Helpers.ProjectInfo.exeName;
            iniPath = new FileInfo(IniPath ?? "config.ini").FullName;
            logIniPath = new FileInfo(IniPath ?? "configlog.ini").FullName;
            if (!File.Exists(iniPath))
            {
                CreateDefIni();
            }
            if (!File.Exists(logIniPath))
            {
                CreateDefIni();
            }
        }

        // 

        public bool EqualsIgnoreCase(string Key, string compareString,string Section = null)
        {
            if (string.IsNullOrEmpty(compareString) || string.IsNullOrEmpty(Key))
            {
                return false;
            }
            if (compareString.ToLower().Equals(Read(Key, Section).ToLower()))
            {
                return true;
            }
            return false;
        }

        public string Read(string Key, string Section = null)
        {
            string configFile;
            if (string.IsNullOrEmpty(Section))
            {
                Section = exeName;
            }
            if (Section.ToLower().Equals("log"))
            {
                configFile = logIniPath;
            }
            else
            {
                configFile = iniPath;
            }
            var RetVal = new StringBuilder(255);
            GetPrivateProfileString(Section, Key, "", RetVal, 255, configFile);
            return RetVal.ToString();
        }

        /// <summary>
        /// MUST USE UpdateIniItem
        /// </summary>
        private void Write(string Key, string Value, string Section = null)
        {
            if (string.IsNullOrEmpty(Section))
            {
                Section = exeName;
            }
            string configFile;
            if (Section.ToLower().Equals("log"))
            {
                configFile = logIniPath;
            } else
            {
                configFile = iniPath;
            }

            WritePrivateProfileString(Section, Key, Value, configFile);
        }

        public void DeleteKey(string Key, string Section = null)
        {
            Write(Key, null, Section ?? exeName);
        }

        public void DeleteSection(string Section = null)
        {
            Write(null, null, Section ?? exeName);
        }

        public bool KeyExists(string Key, string Section = null)
        {
            return Read(Key, Section).Length > 0;
        }

        public void CreateDefIni()
        {

            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            Write("RunAtStartUp", "no", exeName);
            Write("Timer", "3", exeName);
            Write("UseShortcutKeys", "yes", exeName);
            
            Write("downLoadSavePath", Path.Combine(myPictures, exeName), "Online");
            Write("bing", "yes", "Online");
            Write("bingWMK", "yes", "Online");
            Write("alwaysDLBingWallpaper", "no", "Online");
            Write("Spotlight", "yes", "Online");
            Write("SpotlightPath", "AUTO", "Online");

            
            Write("localPath", "no", "Local");
            Write("localPathSetting", myPictures, "Local");
            Write("localPathScan", "AUTO", "Local");
            
            Write("localPathMtime", "NULL", "LOG");
            Write("TimerSetWallpaper", "false", "LOG");
        }
        
        private void PrintDict(Dictionary<string, string> dict)
        {
            foreach (string key in dict.Keys)
            {
                Console.WriteLine($"{key}: {dict[key]}");
            }
        }
        
        public void UpdateIniItem(string key=null,string value=null, string section=null)
        {
            if (string.IsNullOrEmpty(section))
            {
                section = exeName;
            }
            if (!String.IsNullOrEmpty(key) && !String.IsNullOrEmpty(value))
            {
                Write(key, value, section);               
                Console.WriteLine($"update \"{key}\" -> \"{value}\"");
            }                
        }
        
        public void RunAtStartup()
        {
            // ConfigIni iniFile
            string runAtStartUp = Read("RunAtStartUp").ToLower();
            if (runAtStartUp.Equals("yes") || runAtStartUp.Equals("once"))
            {
                AutoStartupHelper.CreateAutorunShortcut();
                if (runAtStartUp.Equals("once"))
                {
                    Write("RunAtStartUp", "yes/no");
                }
            }
            else if (runAtStartUp.Equals("no"))
            {
                if (!AutoStartupHelper.IsAutorun())
                {
                    return;
                }
                Console.WriteLine("You don't want this program run at Windows startup.");
                AutoStartupHelper.RemoveAutorunShortcut();
            }
        }
        public static ConfigIni GetInstance()
        {
            return _instance ?? (_instance = new ConfigIni());
        }
    }
}
