using DailyWallpaper.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class DailyWallpaperCons
    {
        static async Task MainSTD(string[] args)
        {
            var exeName = ProjectInfo.exeName;
            await DailyWallpaper(exeName);
        }
        
        public static async Task<bool> ShowDialog(bool useTextWriter = false, TextWriter textWriter = null)
        {
            var exeName = ProjectInfo.exeName;
            var logFile = ProjectInfo.logFile;
            bool res = false;
            if (useTextWriter)
            {
                Console.SetOut(textWriter);
                Console.SetError(textWriter);
                Console.WriteLine($"------  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}  ------");
                try
                {
                    res = await DailyWallpaper(exeName);
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
                Console.WriteLine($"------  {DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}  ------");
                try
                {
                    res = await DailyWallpaper(exeName);
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
            var standardError = new StreamWriter(Console.OpenStandardError());
            standardError.AutoFlush = true;
            Console.SetError(standardError);

            // redirect stdout to default.
            var standardOutput = new StreamWriter(Console.OpenStandardOutput());
            standardOutput.AutoFlush = true;
            Console.SetOut(standardOutput);
        }

        /*TODO*/
        // generate single file excutable
        private async static Task<bool> DailyWallpaper(string exeName)
        {
            // = new ConfigIni(exeName: exeName);
            var iniFile = ConfigIni.GetInstance();
            // iniFile.RunAtStartup();  Console Mode shouldn't do it.

            
            if (iniFile.Read("bing", "Online").ToLower().Equals("yes"))
            {
                if (iniFile.Read("alwaysDLBingWallpaper", "Online").ToLower().Equals("yes"))
                {
                    await new OnlineImage(iniFile).Bing();
                }
            }
            var choiceDict = new Dictionary<string, string>();
            choiceDict.Add("bing", iniFile.Read("bing", "Online"));
            choiceDict.Add("Spotlight", iniFile.Read("Spotlight", "Online"));
            choiceDict.Add("localPath", iniFile.Read("localPath", "Local"));

            var choiceList = new List<string>();
            foreach (string key in choiceDict.Keys)
            {
                if (choiceDict[key].ToLower().Equals("yes"))
                {
                    choiceList.Add(key);
                }
            }
            var random = new Random();
            int index = random.Next(choiceList.Count);
            string choice = choiceList[index];
            Console.WriteLine($"-> The choice is: {choice}");
            string wallpaper = null;
            switch (choice)
            {
                case "bing":
                    wallpaper = await new OnlineImage(iniFile).Bing(print: false);
                    break;

                case "Spotlight":
                    
                    wallpaper = new OnlineImage(iniFile).Spotlight();
                    break;

                case "localPath":
                    Console.WriteLine("Scan localPath, StartTime:" + DateTime.Now.ToString());
                    var localImage = new LocalImage(iniFile, 
                        iniFile.Read("localPathSetting", "Local"));
                    wallpaper = localImage.RandomSelectOne();
                    Console.WriteLine("Scan localPath, EndTime:" + DateTime.Now.ToString());
                    break;
            }
            if (!File.Exists(wallpaper))
            {
                return false;
            }
            // Fill is the best.
            Wallpaper.SetWallPaper(wallpaper, Wallpaper.PicturePosition.Fill);
            iniFile.UpdateIniItem("WALLPAPER", wallpaper, "LOG");
            iniFile.UpdateIniItem("wallpaperWithTime", wallpaper + "    " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "LOG");
            return true;
        }
    }
}