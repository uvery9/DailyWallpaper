using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DailyWallpaper.Tools
{
    [Serializable]
    public class Gemini
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
        public enum GeminiCompareMode
        {
            NameAndSize,
            ExtAndSize,
            HASH
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

        public struct GeminiCEFStruct
        {
            private bool @checked;
            public string name;
            public string fullPath;
            public string lastMtime;
            public Color color;

            public bool Checked { get => @checked; set => @checked = value; }
        }
        public class GeminiFileStruct
        {
            private bool @checked;
            public int index;
            public long size;
            public string sizeStr;
            public string name;
            public string extName;
            public string fullPath;
            public string dir;
            public string sha1;
            public string md5;
            public string hash;
            public string lastMtime;
            public string crtTime;
            public Color color;

            public bool Checked { get => @checked; set => @checked = value; }

            public bool EqualsMD5(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                        fullPath != @struct.fullPath &&
                        size == @struct.size &&
                        md5 == @struct.md5;
            }

            public bool EqualsHash(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                        fullPath != @struct.fullPath &&
                        size == @struct.size &&
                        hash == @struct.hash;
            }

            public bool EqualsSHA1(object obj)
            {
                return obj is GeminiFileStruct @struct &&
                        fullPath != @struct.fullPath &&
                        size == @struct.size &&
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
                tmp += "\r\ndir:         "  + dir ?? "";
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
                return obj is GeminiFileStruct @struct &&
                       fullPath == @struct.fullPath;
            }

            public override int GetHashCode()
            {
                return -1906184077 + EqualityComparer<string>.Default.GetHashCode(fullPath);
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
        public static GeminiFileStruct FillGeminiFileStruct(string fullPath)
        {
            var tmp = new GeminiFileStruct
            {
                index = 0,
                fullPath = fullPath,
                Checked = false,

                name = null,
                extName = null,
                size = 0,
                sha1 = null,
                md5 = null,
                hash = null,
            };
            try
            {
                if (!File.Exists(fullPath))
                {
                    return tmp;
                }
                tmp.name = Path.GetFileName(fullPath);
                tmp.dir = Path.GetDirectoryName(fullPath);
                tmp.size = new FileInfo(fullPath).Length;
                tmp.sizeStr = Len2Str(tmp.size);
                tmp.lastMtime = new FileInfo(fullPath).LastWriteTime.ToString("yyyy/M/d H:mm");
                tmp.crtTime = new FileInfo(fullPath).CreationTime.ToString("yyyy/M/d H:mm");
                tmp.extName = Path.GetExtension(fullPath);
            }
            catch
            {
                throw;
            }
            return tmp;
        }


        public static async Task<List<GeminiFileStruct>> ComparerTwoList(
            List<GeminiFileStruct> li1, List<GeminiFileStruct> li2,
            GeminiCompareMode mode,
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
                if (progress != null)
                {
                    cnt++;
                    if (cnt % 100 == 0)
                    {
                        progress.Report((long)100 * li2.Count);
                    }
                }
                foreach(var l2 in li2)
                {
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    
                    if (mode == GeminiCompareMode.ExtAndSize)
                    {
                        if (l1.EqualExtSize(l2))
                        {
                            tmp.Add(l1);
                            tmp.Add(l2);
                        }
                    }
                    else if (mode == GeminiCompareMode.NameAndSize) // Fastest.
                    {
                        if (l1.EqualNameSize(l2))
                        {
                            tmp.Add(l1);
                            tmp.Add(l2);
                        }
                    }
                    else if (mode == GeminiCompareMode.HASH)
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
                    await ComputeHashAsync(SHA1.Create(), one.fullPath, token, "SHA1", getRes);
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
                    await ComputeHashAsync(MD5.Create(), one.fullPath, token, "MD5", getRes);
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
        private static string GetTimeStringMsOrS(TimeSpan t)
        {
            string hashCostTime;
            if (t.TotalSeconds > 1)
            {
                hashCostTime = t.TotalSeconds.ToString() + "s";
            }
            else
            {
                hashCostTime = t.TotalMilliseconds.ToString() + "ms";
            }
            return hashCostTime;
        }
        // encoding = Encoding.UTF8
        public static string GetHash(HashAlgorithm hashAlgorithm = null, byte[] data = null,
            string input = null, Encoding encoding = default)
        {
            // Convert the input string to a byte array and compute the hash.
            if (data == null && !string.IsNullOrEmpty(input))
            {
                data = hashAlgorithm.ComputeHash(encoding.GetBytes(input));
            }

            var sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        
        public static async Task ComputeHashAsync(HashAlgorithm hashAlgorithm, string path,
            CancellationToken cancelToken = default, string who = null,
            Action<bool, string, string, string> action = null,
            IProgress<double> progress = null, int bufferSize = 1024 * 1024 * 10)
        {
            try
            {
                using (var stream = new FileInfo(path).OpenRead())
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    stream.Position = 0;
                    byte[] readAheadBuffer, buffer;
                    int readAheadBytesRead, bytesRead;
                    long totalBytesRead = 0;
                    var size = stream.Length;
                    readAheadBuffer = new byte[bufferSize];
                    readAheadBytesRead = await stream.ReadAsync(readAheadBuffer, 0,
                        readAheadBuffer.Length, cancelToken);
                    totalBytesRead += readAheadBytesRead;
                    do
                    {
                        bytesRead = readAheadBytesRead;
                        buffer = readAheadBuffer;
                        readAheadBuffer = new byte[bufferSize];
                        readAheadBytesRead = await stream.ReadAsync(readAheadBuffer, 0,
                            readAheadBuffer.Length, cancelToken);
                        totalBytesRead += readAheadBytesRead;

                        if (readAheadBytesRead == 0)
                            hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
                        else
                            hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                        if (progress != null)
                        {
                            progress.Report((double)totalBytesRead / size * 100);
                        }

                        if (cancelToken.IsCancellationRequested)
                            cancelToken.ThrowIfCancellationRequested();
                    } while (readAheadBytesRead != 0);
                    timer.Stop();
                    var hashCostTime = GetTimeStringMsOrS(timer.Elapsed);
                    if (action != null)
                    {
                        action(true, $"{who}", GetHash(data: hashAlgorithm.Hash), hashCostTime);
                    }

                }
            }
            catch (OperationCanceledException e)
            {
                if (action != null)
                {
                    action(false, $"Info {who}", null, e.Message);
                }
            }
            catch (Exception e)
            {
                if (action != null)
                {
                    action(false, $"ERROR {who}", null, e.Message);
                }
            }
        }

        /// <summary>
        /// Writes the given object instance to an XML file.
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [XmlIgnore] attribute.</para>
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>

        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static void SaveOperationHistory<T>(string pathName, List<T> list)
        {
            if (list == null)
            {
                return;
            }
            // Path.
            var executingLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var dir = Path.Combine(executingLocation, "Gemini.History");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            /*if (!IsList(list))
            {
                return;
            }*/

            if (list.Count < 1)
            {
                return;
            }
            pathName = Path.Combine(dir, pathName);
            WriteToXmlFile(pathName, list);
        }

        public static void WriteToXmlFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                writer = new StreamWriter(filePath, append);
                serializer.Serialize(writer, objectToWrite);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an XML file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the XML file.</returns>
        public static T ReadFromXmlFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                var serializer = new XmlSerializer(typeof(T));
                reader = new StreamReader(filePath);
                return (T)serializer.Deserialize(reader);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Writes the given object instance to a Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// <para>Only Public properties and variables will be written to the file. These can be any type though, even other classes.</para>
        /// <para>If there are public properties/variables that you do not want written to the file, decorate them with the [JsonIgnore] attribute.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToJsonFile<T>(string filePath, T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                if (writer != null)
                    writer.Close();
            }
        }

        /// <summary>
        /// Reads an object instance from an Json file.
        /// <para>Object type must have a parameterless constructor.</para>
        /// </summary>
        /// <typeparam name="T">The type of object to read from the file.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the Json file.</returns>
        public static T ReadFromJsonFile<T>(string filePath) where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }

        /// <summary>
        /// Writes the given object instance to a binary file.
        /// <para>Object type (and all child types) must be decorated with the [Serializable] attribute.</para>
        /// <para>To prevent a variable from being serialized, decorate it with the [NonSerialized] attribute; cannot be applied to properties.</para>
        /// </summary>
        /// <typeparam name="T">The type of object being written to the XML file.</typeparam>
        /// <param name="filePath">The file path to write the object instance to.</param>
        /// <param name="objectToWrite">The object instance to write to the XML file.</param>
        /// <param name="append">If false the file will be overwritten if it already exists. If true the contents will be appended to the file.</param>
        public static void WriteToBinaryFile<T>(string filePath, T objectToWrite, bool append = false)
        {
            using (Stream stream = File.Open(filePath, append ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, objectToWrite);
            }
        }

        /// <summary>
        /// Reads an object instance from a binary file.
        /// </summary>
        /// <typeparam name="T">The type of object to read from the XML.</typeparam>
        /// <param name="filePath">The file path to read the object instance from.</param>
        /// <returns>Returns a new instance of the object read from the binary file.</returns>
        public static T ReadFromBinaryFile<T>(string filePath)
        {
            using (Stream stream = File.Open(filePath, FileMode.Open))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                return (T)binaryFormatter.Deserialize(stream);
            }
        }

    }
}
