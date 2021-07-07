using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyWallpaper
{
    class Gemini
    {

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

        public Gemini()
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

        public struct GeminiFileStruct
        {
            public bool available;
            public bool willDelete;
            public bool selected;
            public long size;
            public string name;
            public string extName;
            public string fullPath;
            public string sha1;
            public string md5;
            public DateTime lastMtime;
            public DateTime crtTime;

            public bool EqualsHash(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       size == @struct.size &&
                       name == @struct.name &&
                       extName == @struct.extName &&
                       sha1 == @struct.sha1 &&
                       md5 == @struct.md5;
            }
            public bool EqualsMD5(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       size == @struct.size &&
                       extName == @struct.extName &&
                       md5 == @struct.md5;
            }

            public async Task<bool> EqualsSHA1(object obj, CancellationToken token)
            {
                if (sha1 == null)
                {
                    var tmp = this;
                    void getRes(bool res, string who, string sha1, string costTimeOrMsg)
                    {
                        if (res)
                        {
                            tmp.sha1 = sha1;
                        }
                    }
                    await HashCalc.HashCalculator.ComputeHashAsync(SHA1.Create(), tmp.fullPath, token, "SHA1", getRes);
                    sha1 = tmp.sha1;
                }
                if (((GeminiFileStruct)obj).sha1 == null)
                {
                    var tmp = (GeminiFileStruct)obj;
                    void getRes(bool res, string who, string sha1, string costTimeOrMsg)
                    {
                        if (res)
                        {
                            tmp.sha1 = sha1;
                        }
                    }
                    await HashCalc.HashCalculator.ComputeHashAsync(SHA1.Create(), tmp.fullPath, token, "SHA1", getRes);
                    obj = tmp;
                }
                return obj is GeminiFileStruct @struct &&
                       size == @struct.size &&
                       extName == @struct.extName &&
                       sha1 == @struct.sha1;
            }

            public override bool Equals(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       size == @struct.size &&
                       name == @struct.name &&
                       extName == @struct.extName;
            }

            public override string ToString()
            {
                string tmp = "" +
                       "fullPath:         " + fullPath ?? "";
                tmp += "\r\nname:         " + name ?? "";
                tmp += "\r\nextName:      " + extName ?? "";
                tmp += "\r\nsize:         " + size.ToString() ?? "";
                tmp += "\r\nmd5:          " + md5 ?? "";
                tmp += "\r\nsha1:         " + sha1 ?? "";
                tmp += "\r\nCreateTime:   " + crtTime.ToString() ?? "";
                tmp += "\r\nlastMtime:    " + lastMtime.ToString() ?? "";
                tmp += "\r\navailable:    " + available.ToString() ?? "";
                tmp += "\r\nwillDelete:   " + willDelete.ToString() ?? "";
                return tmp;
            }
            public string ToStringSimple()
            {
                string tmp = "" +
                       "fullPath:         " + fullPath ?? "";
                tmp += "\r\nname:         " + name ?? "";
                tmp += "\r\nsize:         " + size.ToString() ?? "";
                return tmp;
            }

            public override int GetHashCode()
            {
                int hashCode = -1158680255;
                hashCode = hashCode * -1521134295 + size.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(extName);
                return hashCode;
            }
        }

        public static GeminiFileStruct FillGeminiFileStruct(string fullPath)
        {
            var tmp = new GeminiFileStruct
            {
                fullPath = fullPath,
                willDelete = false,
                available = false,
                selected = false,

                name = null,
                extName = null,
                size = 0,
                sha1 = null,
                md5 = null,
            };
            try
            {
                if (!File.Exists(fullPath))
                {
                    return tmp;
                }
                tmp.name = Path.GetFileName(fullPath);
                tmp.size = new FileInfo(fullPath).Length;
                tmp.lastMtime = new FileInfo(fullPath).LastWriteTime;
                tmp.crtTime = new FileInfo(fullPath).CreationTime;
                tmp.extName = Path.GetExtension(fullPath);
                tmp.available = true;
            }
            catch
            {
                tmp.available = false;
                throw;
            }
            finally
            {

            }
            return tmp;
        }

        public static List<GeminiFileStruct> ComparerTwoList(List<GeminiFileStruct> li1, List<GeminiFileStruct> li2)
        {
            var tmp = new List<GeminiFileStruct>();
            foreach (var l1 in li1)
            {
                foreach(var l2 in li2)
                {
                    if (l1.Equals(l2))
                    {
                        tmp.Add(l1);
                        tmp.Add(l2);
                    }
                }
            }
            return tmp;
        }

        private static void UpdateHash(GeminiFileStruct g, string md5 = null, string sha1 = null)
        {
            g.md5 = md5 ?? "";
            g.sha1 = sha1 ?? "";
        }

        public static async Task<List<GeminiFileStruct>> ForceGetHashGeminiFileStructList(
            List<GeminiFileStruct> li, CancellationToken token, bool calcSHA1 = false, bool calcMD5 = false)
        {
            var ret = new List<GeminiFileStruct>();
            foreach(var one in li)
            {
                var tmp = one;
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                if (calcSHA1)
                {
                    void getRes(bool res, string who, string sha1, string costTimeOrMsg)
                    {
                        if (res)
                        {
                            tmp.sha1 = sha1;
                        }
                    }
                    await HashCalc.HashCalculator.ComputeHashAsync(SHA1.Create(), one.fullPath, token, "SHA1", getRes);
                }
                if (calcMD5)
                {
                    void getRes(bool res, string who, string md5, string costTimeOrMsg)
                    {
                        if (res)
                        {
                            tmp.md5 = md5;
                        }
                    }
                    await HashCalc.HashCalculator.ComputeHashAsync(MD5.Create(), one.fullPath, token, "MD5", getRes);
                }
                ret.Add(tmp);
            }
            return ret;
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
