using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;
using System.IO;
using System.Text.RegularExpressions;
using System;
using System.Net;
using System.Drawing.Imaging;

public class Wallpaper
{
    

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public enum PicturePosition
    {
        Fill,
        Fit,
        Span,
        Stretch,
        Tile,
        Center
    }
    
    private static int GetHanNumFromString(string str)
    {
        int count = 0;
        Regex regex = new Regex(@"^[\u4E00-\u9FA5]{0,}$");

        for (int i = 0; i < str.Length; i++)
        {
            if (regex.IsMatch(str[i].ToString()))
            {
                count++;
            }
        }

        return count;
    }
    public static void AddWaterMark(string sourceFile, string destFile, string waterMark, bool deleteSrc=false)
    {
        
        using (var bitmap = Bitmap.FromFile(sourceFile)){
            // Font font = new Font("Microsoft YaHei", 20, FontStyle.Regular, GraphicsUnit.Pixel);
            int fontSize = 20;
            Font font = new Font("Microsoft YaHei", fontSize, FontStyle.Regular);

            //Color color = FromArgb(255, 255, 0, 0);
            // bitmap.Width - font_len - 50
            int font_len = waterMark.Length + GetHanNumFromString(waterMark);
            /*System.Console.WriteLine("waterMark:" + waterMark);
            System.Console.WriteLine("waterMark.Length:" + waterMark.Length);
            System.Console.WriteLine("font_len:" + font_len);
            System.Console.WriteLine("GetHanNumFromString:" + GetHanNumFromString(waterMark));*/

            int wid = bitmap.Width - font_len * fontSize / 2 + 80;
            int hei = bitmap.Height - fontSize - 100;
            Point atpoint = new Point(wid, hei);
            Console.WriteLine("[" + wid + "," + hei + "]");
            Console.WriteLine("[" + bitmap.Width + "," + bitmap.Height + "]");
            SolidBrush brush = new SolidBrush(Color.White);
            Graphics graphics = Graphics.FromImage(bitmap);
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;
            graphics.DrawString(waterMark, font, brush, atpoint, stringFormat);
            graphics.Dispose();
            MemoryStream m = new MemoryStream();
            bitmap.Save(m, System.Drawing.Imaging.ImageFormat.Jpeg);
            byte[] convertedToBytes = m.ToArray();
            File.WriteAllBytes(destFile, convertedToBytes); 
        }
        if (deleteSrc)
        {
            File.Delete(sourceFile);
        }
    }

    public static void SetWallPaper(string wallpaper=null, PicturePosition style = PicturePosition.Fill)
    {
        if (wallpaper == null)
        {
            throw new ArgumentNullException(nameof(wallpaper));
        }
        using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", true))
        {
            if (style == PicturePosition.Fill)
            {
                key.SetValue(@"WallpaperStyle", "10");
                key.SetValue(@"TileWallpaper", "0");
            }
            if (style == PicturePosition.Fit)
            {
                key.SetValue(@"WallpaperStyle", "6");
                key.SetValue(@"TileWallpaper", "0");
            }
            if (style == PicturePosition.Span) // Windows 8 or newer only!
            {
                key.SetValue(@"WallpaperStyle", "22");
                key.SetValue(@"TileWallpaper", "0");
            }
            if (style == PicturePosition.Stretch)
            {
                key.SetValue(@"WallpaperStyle", "2");
                key.SetValue(@"TileWallpaper", "0");
            }
            if (style == PicturePosition.Tile)
            {
                key.SetValue(@"WallpaperStyle", "0");
                key.SetValue(@"TileWallpaper", "1");
            }
            if (style == PicturePosition.Center)
            {
                key.SetValue(@"WallpaperStyle", "0");
                key.SetValue(@"TileWallpaper", "0");
            }
        }
        // Console.WriteLine($"Setted {new FileInfo(wallpaper).Name} as Wallpaper.");
        const int SPI_SETDESKWALLPAPER = 20;
        const int SPIF_UPDATEINIFILE = 0x01;
        const int SPIF_SENDWININICHANGE = 0x02;
        SystemParametersInfo(SPI_SETDESKWALLPAPER,
                0,
                wallpaper,
                SPIF_UPDATEINIFILE | SPIF_SENDWININICHANGE);
    }
    public static void DownLoadFromUrl(string dir, string name, string url)
    {
        string file = Path.Combine(dir, name);
        if (File.Exists(file))
        {
            Console.WriteLine($"File already exists: {file}");
            return;
        }
        using (WebClient webClient = new WebClient())
        {
            byte[] data = webClient.DownloadData(url);

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (var img = Image.FromStream(mem))
                {
                    // ImageFormat.Png
                    img.Save(file, ImageFormat.Jpeg);
                    Console.WriteLine($"File download succeed: {file}");
                }
            }

        }
    }
}