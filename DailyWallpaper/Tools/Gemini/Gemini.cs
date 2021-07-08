using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyWallpaper.Tools
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
        public enum CompareMode
        {
            NameAndSize,
            ExtAndSize,
            SizeAndHash
        }

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
            public string sizeStr;
            public string name;
            public string extName;
            public string fullPath;
            public string sha1;
            public string md5;
            public string lastMtime;
            public string crtTime;

            public bool EqualsHash(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       fullPath != @struct.fullPath &&
                       size == @struct.size &&
                       name == @struct.name &&
                       extName == @struct.extName &&
                       sha1 == @struct.sha1 &&
                       md5 == @struct.md5;
            }

            public bool EqualsMD5(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                        fullPath != @struct.fullPath &&
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

            public bool EqualSize(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       fullPath != @struct.fullPath &&
                       size == @struct.size;
            }

            public bool EqualExtSize(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       fullPath != @struct.fullPath &&
                       extName == @struct.extName &&
                       size == @struct.size;
            }

            // fastest
            public bool EqualNameSize(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       fullPath != @struct.fullPath &&
                       name == @struct.name &&
                       size == @struct.size;
            }

            public override string ToString()
            {
                string tmp = "" +
                       "fullPath:         " + fullPath ?? "";
                tmp += "\r\nname:         " + name ?? "";
                tmp += "\r\nextName:      " + extName ?? "";
                tmp += "\r\nsize:         " + size.ToString();
                tmp += "\r\nsizeStr:      " + sizeStr;
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

            public override bool Equals(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                       fullPath == @struct.fullPath;
            }

            public override int GetHashCode()
            {
                return -1906184077 + EqualityComparer<string>.Default.GetHashCode(fullPath);
            }
        }
        private static string Len2Str(long len)
        {
            var str = "";
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
                tmp.sizeStr = Len2Str(tmp.size);
                tmp.lastMtime = new FileInfo(fullPath).LastWriteTime.ToString("yyyy/M/d H:mm");
                tmp.crtTime = new FileInfo(fullPath).CreationTime.ToString("yyyy/M/d H:mm");
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

        public static async Task<List<GeminiFileStruct>> ComparerTwoList(
            List<GeminiFileStruct> li1, List<GeminiFileStruct> li2,
            CompareMode mode,
            CancellationToken token = default, Action<bool,
            List<GeminiFileStruct>> action = default,
            IProgress<long> progress = null)
        {
            var tmp = new List<GeminiFileStruct>();

            if (li1.Count < 1 || li2.Count < 1)
            {
                action(false, tmp);
                return tmp;
            }
            
            await Task.Run(() => {
            long cnt = 0;
            foreach (var l1 in li1)
            {
                foreach(var l2 in li2)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    if (progress != null)
                    {
                        cnt++;
                        if (cnt >= 1000)
                        {
                            progress.Report(1000);
                            cnt = 0;
                        }
                    }
                    if (mode == CompareMode.ExtAndSize)
                    {
                        if (l1.EqualExtSize(l2))
                        {
                            tmp.Add(l1);
                            tmp.Add(l2);
                        }
                    }
                    else if (mode == CompareMode.NameAndSize) // Fastest.
                    {
                        if (l1.EqualNameSize(l2))
                        {
                            tmp.Add(l1);
                            tmp.Add(l2);
                        }
                    }
                    else if (mode == CompareMode.SizeAndHash)
                    {
                        if (l1.EqualSize(l2))
                        {
                            tmp.Add(l1);
                            tmp.Add(l2);
                        }
                    }

                }
            }
            });
            action(true, tmp);
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
