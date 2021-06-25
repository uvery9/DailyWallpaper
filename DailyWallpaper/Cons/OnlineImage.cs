using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class OnlineImage
    {
        private ConfigIni ini;
        private string path;

        public OnlineImage(ConfigIni ini, string path = null)
        {
            this.ini = ini;
            this.path = path;
            if (String.IsNullOrEmpty(path))
            {
                if (this.ini.Read("downLoadSavePath", "Online").ToLower().Equals("null")) 
                { 
                    var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    myPictures = Path.Combine(myPictures, ini.exeName);
                    Console.WriteLine($"-> The downloaded picture will be saved to: {myPictures}");
                    ini.UpdateIniItem("downLoadSavePath", myPictures, "Online");
                    this.path = myPictures;
                } else
                {
                    this.path = this.ini.Read("downLoadSavePath", "Online");
                }

            }
            if (!Directory.Exists(this.path))
            {
                Directory.CreateDirectory(this.path);
            }  
        }

        /*
         * Input: url
         * Return: img-url, img_name
         */


        public async Task<string> BingChina(bool print=true)
        {
            var bingImg = await new BingImageProvider().GetImage(check:true);
            // remove illegal characters
            // var file_name = string.Concat(filename.Split(Path.GetInvalidFileNameChars()));
            // replace illegal characters with _
            var file_name = string.Join("_", bingImg.Copyright.Split(Path.GetInvalidFileNameChars()));
            string wallpaper = Path.Combine(path, file_name + ".jpg");
            var wallpaperWMK = Path.Combine(path, file_name + "-WMK.jpg");
            if (print)
            {
                Console.WriteLine($"Downloading BingChina IMG: {bingImg.Copyright}");
                Console.WriteLine($"Know more: {bingImg.CopyrightLink}");
            }
            if (!File.Exists(wallpaperWMK)) {
                // Don't download the picture again and again.
                var img = await new BingImageProvider().GetImage(check:false);
                img.Img.Save(wallpaper, System.Drawing.Imaging.ImageFormat.Jpeg);
                Wallpaper.AddWaterMark(wallpaper, wallpaperWMK, bingImg.Copyright, deleteSrc: true);
            }            
            return wallpaperWMK;
        }

        public static void PrintAllSystemEnvironmentInfo()
    {
            foreach (System.Collections.DictionaryEntry e in System.Environment.GetEnvironmentVariables())
            {
                Console.WriteLine(e.Key + ":" + e.Value);
            }
        }
        private string GetDailySpotlightDir()
        {
            if (!ini.GetCfgFromIni()["SpotlightPath"].ToLower().Equals("auto"))
            {
                var dir = ini.GetCfgFromIni()["SpotlightPath"];
                if (!Directory.Exists(dir)) {
                    throw new DirectoryNotFoundException($"SpotlightPath invalid: {dir}");
                }
                return dir;
            }
            var exception = new Exception("ERROR: it should be ONE SpotlightPath, tell me what to do next.\r\n" +
                    "The sample path is: " +
                    @"C:\Users\jared\AppData\Local\Packages\Microsoft.Windows.ContentDeliveryManager_cw5n1h2txyewy\LocalState\Assets" + Environment.NewLine+
                    "\r\nFIRST OF ALL: \r\n    Turn on Spotlight feature in Windows 10 by \r\n" +
                    "  Google/Baidu \"How to enable Windows Spotlight?\" \r\n\r\n" +
                    "You can specify the PATH in the config.ini by:\r\n" +
                    @"  SpotlightPath=C:\your_path" + "\r\n\r\n" + 
                    "Or turn off Spotlight feature by:\r\n" +
                    "  Spotlight=no\r\n"
                    ); 
            string LOCALAPPDATA = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var pkg = Path.Combine(LOCALAPPDATA, "Packages");
            var contentDeliveryManager = Directory.GetDirectories(pkg, "*ContentDeliveryManager*", SearchOption.AllDirectories);
            if (!contentDeliveryManager.Length.Equals(1))
            {
                // throw exception;
                ini.UpdateIniItem("Spotlight", "no", "Online");
                return null;
            } 
            var assets = Directory.GetDirectories(contentDeliveryManager[0], "Assets", SearchOption.AllDirectories);
            if (!assets.Length.Equals(1))
            {
                // throw exception;
                ini.UpdateIniItem("Spotlight", "no", "Online");
                return null;
            }
            return assets[0];
        }
        public string DailySpotlight()
        {
            string spotlightPath = GetDailySpotlightDir();
            if (string.IsNullOrEmpty(spotlightPath)){
                return null;
            }
            var jpegFiles = new List<FileInfo>();
            var wallpaperDict = new Dictionary<string, string>();
            foreach (string file in Directory.GetFiles(spotlightPath, "*", SearchOption.AllDirectories))
            {
                /*try { 
                    System.Drawing.Image imgInput = System.Drawing.Image.FromFile(file);
                    System.Drawing.Graphics gInput = System.Drawing.Graphics.FromImage(imgInput);
                    System.Drawing.Imaging.ImageFormat thisFormat = imgInput.RawFormat;
                    Console.WriteLine("It is image");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("It is not image");
                }*/

                long length = new FileInfo(file).Length / 1024;
                if (length > 100)
                {
                   /* Bitmap img = new Bitmap(file);
                    Console.WriteLine(img.RawFormat.ToString());
                    img.Dispose();*/

                    Image img = Image.FromFile(file);
                    if (System.Drawing.Imaging.ImageFormat.Jpeg.Equals(img.RawFormat))
                    {
                        if (img.Width > 1900 && (img.Width + 0.0 / img.Height > 1.4))
                        {
                            var jpegfi = new FileInfo(file);
                            jpegFiles.Add(jpegfi);
                            var dest = Path.Combine(path, jpegfi.CreationTime.ToString("yyyy-MMdd_HH-mm-ss") + ".jpeg");
                            wallpaperDict.Add(jpegfi.Name, dest);
                            if (!File.Exists(dest))
                            {
                                jpegfi.CopyTo(dest);
                                Console.WriteLine($"copy to: {dest}");
                            }
                            // 
                        }
                        else
                        {        
                            var jpegPhone = new FileInfo(file);
                            var dest = Path.Combine(path, jpegPhone.CreationTime.ToString("yyyy-MMdd_HH-mm-ss") + "-Phone.jpeg");
                            if (!File.Exists(dest))
                            {
                                jpegPhone.CopyTo(dest);
                                Console.WriteLine($"copy to: {dest}");
                            }
                        }
                    } else if (System.Drawing.Imaging.ImageFormat.Png.Equals(img.RawFormat))
                    {
                        Console.WriteLine("PNG: abandon");
                        
                    }                   
                    img.Dispose();
                }             
            }
            List <FileInfo> jpegFilesOrdered     = jpegFiles.OrderByDescending(x => x.CreationTime).ToList();
            return wallpaperDict[jpegFilesOrdered[0].Name];
        }      
    }
}
