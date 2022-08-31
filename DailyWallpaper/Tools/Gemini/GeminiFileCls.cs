using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DailyWallpaper.Tools.Gemini
{
    public interface ICloneable<T>
    {
        T Clone();
    }
    public enum LoadFileStep
    {
        NO_LOAD,
        STEP_1_ALL_FILES,
        STEP_2_FILES_TO_STRUCT,
        STEP_3_FAST_COMPARE,
        STEP_4_COMPARE_HASH,
        DEFAULT,
        ERROR
    }

    public enum MODE
    {
        MD5,
        SHA1,
        NAME,
        EXT
    }

    public class GeminiFileCls : ICloneable<GeminiFileCls>
    {

        public int index;
        private bool @checked;
        public string name;
        public string fullPath;
        public string sizeStr;

        public string hash;
        public string crtTime;
        public string lastMtime;
        public MODE mode;
        public LoadFileStep step;
        public bool ignoreLittleFile;
        public long ignoreLittleFileSize;
        public Color color;
        /*public string sha1;
        public string md5;*/

        public bool bigFile = false;
        public string dir;
        public string extName;
        public long size;

        public void SetModeStepIgnoreFile(MODE m, LoadFileStep s, bool ignore = false, long size = 0)
        {
            mode = m;
            step = s;
            ignoreLittleFile = ignore;
            ignoreLittleFileSize = size;
        }

        // 序列化必须
        public GeminiFileCls() { }
        public GeminiFileCls(string outfullPath)
        {

            fullPath = outfullPath;
            index = 0;

            Checked = false;

            name = "";
            extName = "";
            size = 0;
            /*sha1 = "";
            md5 = "";*/
            hash = "";

            try
            {
                if (!File.Exists(fullPath))
                {
                    return;
                }
                name = Path.GetFileName(fullPath);
                dir = Path.GetDirectoryName(fullPath);
                size = new FileInfo(fullPath).Length;
                sizeStr = FileUtils.Len2Str(size);
                lastMtime = new FileInfo(fullPath).LastWriteTime.ToString("yyyy/M/d H:mm");
                crtTime = new FileInfo(fullPath).CreationTime.ToString("yyyy/M/d H:mm");
                extName = Path.GetExtension(fullPath);
            }
            catch
            {
                throw;
            }
        }
        public bool Checked { get => @checked; set => @checked = value; }

        // 不管错误,错误算球
        public static void updateNewPathToList(List<GeminiFileCls> list, string oldPath, string newPath)
        {
            try
            {
                if (File.Exists(newPath))
                {
                    findAndUpdate(list, it => it.fullPath == oldPath, ret =>
                    {
                        ret.fullPath = newPath;
                        ret.name = Path.GetFileName(newPath);
                    });
                }
            }
            catch (Exception) { }
        }


        public static void findAndUpdate<T>(List<T> list, Func<T, bool> predicate, Action<T> update)
        {
            if (list != null)
            {
                var listItem = list.FirstOrDefault(predicate);
                if (listItem != null && !listItem.Equals(default(GeminiFileCls)))
                {
                    update(listItem);
                }
            }
        }


        /*public bool EqualsMD5(object obj)
        {
            return obj is GeminiFileCls @struct &&
                    fullPath != @struct.fullPath &&
                    size == @struct.size &&
                    md5 == @struct.md5;
        }*/

        public bool EqualsHash(object obj)
        {
            return obj is GeminiFileCls @struct &&
                    fullPath != @struct.fullPath &&
                    size == @struct.size &&
                    hash == @struct.hash;
        }

        /*public bool EqualsSHA1(object obj)
        {
            return obj is GeminiFileCls @struct &&
                    fullPath != @struct.fullPath &&
                    size == @struct.size &&
                    sha1 == @struct.sha1;
        }*/

        public bool EqualSize(object obj)
        {
            return obj is GeminiFileCls @struct &&
                   fullPath != @struct.fullPath &&
                   size == @struct.size;
        }

        public bool EqualExtSize(object obj)
        {
            return obj is GeminiFileCls @struct &&
                   fullPath != @struct.fullPath &&
                   extName == @struct.extName &&
                   size == @struct.size;
        }

        // fastest
        public bool EqualNameSize(object obj)
        {
            return obj is GeminiFileCls @struct &&
                   fullPath != @struct.fullPath &&
                   name == @struct.name &&
                   size == @struct.size;
        }

        public override string ToString()
        {
            string tmp = "" +
                   "fullPath:         " + fullPath ?? "";
            tmp += "\r\nname:         " + name ?? "";
            tmp += "\r\ndir:         " + dir ?? "";
            tmp += "\r\nextName:      " + extName ?? "";
            tmp += "\r\nsize:         " + size.ToString();
            tmp += "\r\nsizeStr:      " + sizeStr;
            //tmp += "\r\nmd5:          " + md5 ?? "";
            //tmp += "\r\nsha1:         " + sha1 ?? "";
            tmp += "\r\nhash:         " + hash ?? "";
            tmp += "\r\ncreateTime:   " + crtTime.ToString() ?? "";
            tmp += "\r\nlastMtime:    " + lastMtime.ToString() ?? "";
            tmp += "\r\nwillDelete:   " + Checked.ToString() ?? "";
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
            return obj is GeminiFileCls @struct &&
                   fullPath == @struct.fullPath;
        }

        public override int GetHashCode()
        {
            return -1906184077 + EqualityComparer<string>.Default.GetHashCode(fullPath);
        }

        public GeminiFileCls Clone()
        {
            var other = (GeminiFileCls)MemberwiseClone();
            // https://docs.microsoft.com/en-us/dotnet/api/system.object.memberwiseclone?view=net-5.0
            // string may be copied in MemberwiseClone
            other.sizeStr = String.Copy(sizeStr);
            other.name = String.Copy(name);
            other.extName = String.Copy(extName);
            other.fullPath = String.Copy(fullPath);
            other.dir = String.Copy(dir);
            /*other.sha1 = String.Copy(sha1);
            other.md5 = String.Copy(md5);*/
            other.hash = String.Copy(hash);
            other.lastMtime = String.Copy(lastMtime);
            other.crtTime = String.Copy(crtTime);
            // if you have class in the member, you have to use n.classmember = new Classmember();
            return other;
        }

    }
}
