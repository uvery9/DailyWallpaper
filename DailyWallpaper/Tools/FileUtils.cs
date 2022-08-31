using System;
using System.IO;

namespace DailyWallpaper.Tools
{
    class FileUtils
    {
        // 复制错误, 取消返回null/错误返回异常信息,成功返回新地址
        public static string RenameFileInUI(string fullPath)
        {
            try
            {
                var dir = Path.GetDirectoryName(fullPath);
                var name = Path.GetFileNameWithoutExtension(fullPath);
                var ext = Path.GetExtension(fullPath);
                string input = Microsoft.VisualBasic.Interaction.InputBox("新的文件名", "重命名", name);
                if (!String.IsNullOrEmpty(input) && input != name)
                {
                    var newFile = Path.Combine(dir, input + ext);
                    File.Move(fullPath, newFile);
                    // FileSystem.RenameFile(fullPath, newFile);
                    return newFile;
                }
                return null;
            }
            catch (Exception e)
            {
                return e.Message;
            }
            
        }

        public static string Len2Str(long len)
        {
            string str;
            if (len > 1024 * 1024 * 1024)
            {
                str = (len / 1024 / 1024 / 1024) + "GB";
            }
            else if (len > 1024 * 1024)
            {
                str = (len / 1024 / 1024) + "MB";
            }
            else if (len > 1024)
            {
                str = (len / 1024) + "KB";
            }
            else
            {
                str = (len) + "B";
            }
            return str;
        }

    }
}
