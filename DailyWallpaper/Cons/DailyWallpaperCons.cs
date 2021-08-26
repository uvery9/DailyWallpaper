using DailyWallpaper.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class DailyWallpaperCons
    {
        private static DailyWallpaperCons _instance;
        public static DailyWallpaperCons GetInstance()
        {
            return _instance ?? (_instance = new DailyWallpaperCons());
        }
        private DailyWallpaperCons()
        {

        }

        public bool ShowDialog(bool useTextWriter = false, TextWriter textWriter = null)
        {
            var logFile = ProjectInfo.logFile;
            bool res = false;
            if (useTextWriter)
            {
                Console.SetOut(textWriter);
                Console.SetError(textWriter);
                Console.WriteLine($"------  {DateTime.Now:yyyy-MM-dd HH:mm:ss}  ------");
                try
                {
                    res = DailyWallpaper();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    res = false;
                }
                Console.Out.Flush();
                Console.Error.Flush();
                ResetStdoutAndStderr();
                return res;
            }
            
            Console.WriteLine($"Set stdoutput and stderr to file: {logFile}");
            Console.WriteLine("Please be PATIENT, the result will not be lost.");
            using (var writer = new StreamWriter(logFile))
            {
                Console.SetOut(writer);
                Console.SetError(writer);
                Console.WriteLine($"------  {DateTime.Now:yyyy-MM-dd HH:mm:ss}  ------");
                try
                {
                    res = DailyWallpaper();
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.ToString());
                    res = false;
                }
                Console.Out.Flush();
                Console.Error.Flush();
                writer.Close();
            }
            ResetStdoutAndStderr();
            return res;
            // print the log file.
            //Console.OutputEncoding = Encoding.UTF8;
            //Console.WriteLine(File.ReadAllText(logFile));
        }

        /// <summary>
        /// FIRST writer = new StreamWriter(ProjectInfo.logFile)
        /// </summary>
        /// <param name="writer"></param>
        /// <returns></returns>
        private static void ResetStdoutAndStderr()
        {
            // redirect stderr to default.
            var standardError = new StreamWriter(Console.OpenStandardError())
            {
                AutoFlush = true
            };
            Console.SetError(standardError);

            // redirect stdout to default.
            var standardOutput = new StreamWriter(Console.OpenStandardOutput())
            {
                AutoFlush = true
            };
            Console.SetOut(standardOutput);
        }

        private static bool DailyWallpaper()
        {
            var iniFile = ConfigIni.GetInstance();
            // iniFile.RunAtStartup();  Console Mode shouldn't do it.

            if (iniFile.EqualsIgnoreCase("bing", "yes", "Online"))
            {
                if (iniFile.EqualsIgnoreCase("alwaysDLBingWallpaper", "yes", "Online"))
                {
                    new OnlineImage(iniFile).Bing();
                }
            }
            var choiceList = new List<string>();
            bool bingSkipToday = false;
            if (DateTime.TryParse(iniFile.Read("bingSkipToday", "Online"), out DateTime ret))
            {
                if (ret.DayOfYear.Equals(DateTime.Today.DayOfYear))
                {
                    bingSkipToday = true;
                }
            }
            if (iniFile.EqualsIgnoreCase("bing", "yes", "Online") && !bingSkipToday)
            {
                choiceList.Add("bing");
            }
            if (iniFile.EqualsIgnoreCase("Spotlight", "yes", "Online"))
            {
                choiceList.Add("Spotlight");
            }
            if(iniFile.EqualsIgnoreCase("localPath", "yes", "Local"))
            {
                choiceList.Add("localPath");
            }
            if (choiceList.Count < 1)
            {
                Console.WriteLine($"-> The choiceList.Count less than 1: {choiceList.Count}");
                return false;
            }
            Console.WriteLine($"-> The choiceList.Count is: {choiceList.Count}");
            string choice = choiceList[new Random().Next(choiceList.Count)];
            Console.WriteLine($"-> The choice is: {choice}");
            string wallpaper = null;
            switch (choice)
            {
                case "bing":
                        wallpaper = new OnlineImage(iniFile).Bing(print: false);
                    break;

                case "Spotlight":
                    wallpaper = new OnlineImage(iniFile).Spotlight();
                    var creationTime = new FileInfo(wallpaper).CreationTime;
                    var dateTimeFormat = "yyyy-MM-dd HH:mm";
                    if (DateTime.TryParseExact(iniFile.Read("SpotlightCreationTime", "Online"), 
                        dateTimeFormat, null, DateTimeStyles.None, out DateTime lastCreationTime)) {
                        if ((creationTime - lastCreationTime).TotalMinutes < 1)
                        {
                            Console.WriteLine("Spotlight doesn't update picture.");
                            if (!bingSkipToday)
                                wallpaper = new OnlineImage(iniFile).Bing(print: false);
                            else
                                wallpaper = null;
                            break;
                        }
                    }
                    iniFile.UpdateIniItem("SpotlightCreationTime",
                        $"{creationTime.ToString(dateTimeFormat)}", "Online");
                    break;

                case "localPath":
                        var start = DateTime.Now;
                        Console.WriteLine("Scan localPath, StartTime:" + start.ToString());
                        // very cost time. show add async function.
                        var localImage = new LocalImage(iniFile,
                            iniFile.Read("localPathSetting", "Local"));
                        wallpaper = localImage.RandomSelectOne();
                        var end = DateTime.Now;
                        Console.WriteLine("Scan localPath, EndTime:" + end.ToString());
                        Console.WriteLine($"Scan localPath, Cost {(end - start).TotalSeconds}s:");                        
                    break;
            }
            if (!File.Exists(wallpaper))
            {
                return false;
            }
            // Fill is the best.
            Wallpaper.SetWallPaper(wallpaper, Wallpaper.PicturePosition.Fill);
            iniFile.UpdateIniItem("WallpaperType", choice, "LOG");
            iniFile.UpdateIniItem("WALLPAPER", wallpaper, "LOG");
            iniFile.UpdateIniItem("WALLPAPERSetTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LOG");
            return true;
        }
    }
}