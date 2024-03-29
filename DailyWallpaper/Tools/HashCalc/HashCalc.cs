﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Cryptography;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace DailyWallpaper.HashCalc
{
    class HashCalculator
    {
        public string help;
        public string filePath;
        public ProgressBar hashProgressBar;

        private Progress<double> totalProgessDouble;
        private Progress<int> progessBar;
        public List<string> hashList;
        public List<Task> tasks;
        private static Mutex m_mut;
        public int hashCalcCnt;
        public double percent;
        public int succeedCnt;

        public HashCalculator()
        {
            help = "You can drag FILE to picture/hash panel, \r\n  drag text file to console if \"Allow ConsoleTextBox Drop\" is Enabled).";
            // totalHashProgess = new ProgressImpl();
            void ProgressActionD(double i) // percent in file.
            {
                // readTotal += i;
                // FIX ERROR: System.InvalidOperationException
                if (hashProgressBar.IsHandleCreated)
                {
                    hashProgressBar.Invoke(new Action(() =>
                    {
                        m_mut.WaitOne();
                        percent += (i / hashCalcCnt * 100);
                        var percentInt = (int)percent;
                        if (percentInt > 99.9)
                            percentInt = 100;
                        hashProgressBar.Value = percentInt;
                        m_mut.ReleaseMutex();
                    }));
                }
            }
            totalProgessDouble = new Progress<double>(ProgressActionD);


            void ProgressAction(int i) // percent in file.
            {
                // readTotal += i;
                // FIX ERROR: System.InvalidOperationException
                if (hashProgressBar.IsHandleCreated)
                {
                    hashProgressBar.Invoke(new Action(() =>
                    {
                        m_mut.WaitOne();
                        hashProgressBar.Value = i;
                        m_mut.ReleaseMutex();
                    }));
                }
            }
            progessBar = new Progress<int>(ProgressAction);
            tasks = new List<Task>();
            hashList = new List<string>();
            m_mut = new Mutex();
        }

        /* new ProgressImpl(i => hashProgressBar.Invoke(new Action(() =>
         {
             hashProgressBar.Value = i;
         }));*/
        /* FUCK THE LAMBA.
         * new Progress<int>(i =>
         * {
         *     hashProgressBar.Invoke(new Action(() =>
         *     {
         *         hashProgressBar.Value = i;
         *     }));
         * });*/

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
        public void CalcCRC32(string path, Action<bool, string, string, string> action, CancellationToken token)
        {
            tasks.Add(Task.Run(() => {
                var who = "CRC32:  ";
                try
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    var crc32 = new CRC32(token);
                    var hash = String.Empty;
                    using (var stream = new FileInfo(path).OpenRead())
                    {
                        stream.Position = 0;
                        foreach (byte b in crc32.ComputeHash(stream))
                        {
                            hash += b.ToString("X2");
                        }
                    }
                    // Console.WriteLine("CRC-32 is {0}", hash);
                    ((IProgress<int>)progessBar).Report(100);
                    timer.Stop();
                    string hashCostTime = GetTimeStringMsOrS(timer.Elapsed);
                    action(true, $"{who}", hash, hashCostTime);
                }
                catch (OperationCanceledException e)
                {
                    action(false, $"Info {who}", null, e.Message);
                }
                catch (Exception e)
                {
                    action(false, $"ERROR {who}", null, e.Message);
                    MessageBox.Show(e.ToString());
                }
            }));
        }
        /// <summary>
        /// FOR 7-ZIP
        /// CRC-32 - same as ZIP, Gzip, xz.
        ///CRC-64 - same as xz utils(wikipedia writes that it's ECMA-182).
        ///So you can check any details in source code of zip/gzip/7z/xz.
        ///And you can check exact digest values with sofware 7-Zip / xz utils / gzip / info-zip.
        /// </summary>
        public void CalcCRC64(string path, Action<bool, string, string, string> action, CancellationToken token)
        {
            tasks.Add(Task.Run(() => {
                var who = "CRC64-ISO3309:   ";
                try
                {
                    var timer = new Stopwatch();
                    timer.Start();
                    /* 
                    * The ISO polynomial, defined in ISO 3309 and used in HDLC.
                    * ISO = 0xD800000000000000
                    */
                    var crc64 = new CRC64ISO(token);
                    var hash = String.Empty;
                    using (var fs = new FileInfo(path).OpenRead()) {
                        foreach (byte b in crc64.ComputeHash(fs))
                        {
                            hash += b.ToString("X2");
                        }
                    }
                    ((IProgress<int>)progessBar).Report(100);
                    timer.Stop();
                    var hashCostTime = GetTimeStringMsOrS(timer.Elapsed);
                    action(true, $"{who}", hash, hashCostTime);
                }
                catch (OperationCanceledException e)
                {
                    action(false, $"Info {who}", null, e.Message);
                }
                catch (Exception e)
                {
                    action(false, $"ERROR {who}", null, e.Message);
                }
            }));
        }
        public void CalcMD5(string path, Action<bool, string, string, string> action, CancellationToken token)
        {
            tasks.Add(Task.Run(() => ComputeHashAsync(
                MD5.Create(), path, token, "MD5:    ", action, totalProgessDouble)));
        }
        public void CalcSHA1(string path, Action<bool, string, string, string> action, CancellationToken token)
        {
            tasks.Add(Task.Run(async () => await ComputeHashAsync(
                SHA1.Create(), path, token, "SHA1:   ", action, totalProgessDouble)));
        }
        public void CalcSHA256(string path, Action<bool, string, string, string> action, CancellationToken token)
        {
            tasks.Add(Task.Run(async () => await ComputeHashAsync(
                SHA256.Create(), path, token, "SHA256: ", action, totalProgessDouble)));
        }
        
        public void CalcSHA512(string path, Action<bool, string, string, string> action, CancellationToken token)
        {
            tasks.Add(Task.Run(async () => await ComputeHashAsync(
                SHA512.Create(), path, token, "SHA512: ", action, totalProgessDouble)));
        }
        public string ComputeHashOfString(HashAlgorithm hashAlgorithm, string input)
        {
            /*            // Byte array representation of input string
            var sourceBytes = Encoding.UTF8.GetBytes(input);

            // Generate hash value(Byte Array) for input data
            var hashBytes = hashAlgorithm.ComputeHash(sourceBytes);

            // Convert hash byte array to string
            var hash = BitConverter.ToString(hashBytes).Replace("-", string.Empty);

            // Output the MD5 hash*/
            return GetHash(hashAlgorithm: hashAlgorithm, input: input, encoding: Encoding.UTF8);
        }
        /*

         byte[] bytes;
        using (var hash = MD5.Create())
        {
            using (var fs = new FileStream(f, FileMode.Open))
            {
                bytes = await hash.ComputeHashAsync(fs,
                    progress: new Progress<long>(i =>
                    {
                        progressBar1.Invoke(new Action(() =>
                        {
                            progressBar1.Value = i;
                        }));
                    }));
                MessageBox.Show(BitConverter.ToString(bytes).Replace("-", string.Empty));
            }
        }
         */
        /*try
        {
            var s = new CancellationTokenSource();
            s.CancelAfter(1000);
            byte[] bytes;
            using (var hash = MD5.Create())
            {
                using (var fs = new FileStream(f, FileMode.Open))
                {
                    bytes = await hash.ComputeHashAsync(fs,
                        cancelToken: s.Token,
                        progress: new Progress<long>(i =>
                        {
                            progressBar1.Invoke(new Action(() =>
                            {
                                progressBar1.Value = i;
                            }));
                        }));

                    MessageBox.Show(BitConverter.ToString(bytes).Replace("-", string.Empty));
                }
            }
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Operation canceled.");
        }*/
        /*
         var f = Path.Combine(Application.StartupPath, "temp.log");
        File.Delete(f);
        using (var fs = new FileStream(f, FileMode.Create))
        {
            fs.Seek(1L * 1024 * 1024 * 1024, SeekOrigin.Begin);
            fs.WriteByte(0);
            fs.Close();
        }
         */

        /// <summary>
        /// https://stackoverflow.com/questions/53965380/report-hash-progress
        /// http://www.alexandre-gomes.com/?p=144
        /// Extension Methods (C# Programming Guide)
        /// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods
        /// </summary>

        /// <summary>
        /// hashAlgorithm = SHA1.Create(), progress: progressbar
        /// </summary>
        /// <param name="hashAlgorithm"></param>
        /// <param name="stream"></param>
        /// <param name="cancelToken"></param>
        /// <param name="progress"></param>
        /// <param name="bufferSize"></param>
        /// <returns></returns>
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
                            progress.Report((double)readAheadBytesRead / size);
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


    }

    internal class User32TopWindow
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 TOPMOST_FLAGS = SWP_NOMOVE | SWP_NOSIZE;
        /// <summary>
        /// SetWindowPos(this.Handle, HWND_TOPMOST, 0, 0, 0, 0, TOPMOST_FLAGS);
        /// </summary>
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        /*
         private void button1_Click(object sender, EventArgs e)
            {
                if (on)
                {
                    button1.Text = "yes on top";
                    IntPtr HwndTopmost = new IntPtr(-1);
                    SetWindowPos(this.Handle, HwndTopmost, 0, 0, 0, 0, TopmostFlags);
                    on = false;
                }
                else
                {
                    button1.Text = "not on top";
                    IntPtr HwndTopmost = new IntPtr(-2);
                    SetWindowPos(this.Handle, HwndTopmost, 0, 0, 0, 0, TopmostFlags);
                    on = true;
                }
            }
         */
    }
}
