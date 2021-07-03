using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security.Cryptography;
using System.IO;

namespace DailyWallpaper.HashCalc
{
    class HashCalc
    {
        public string help;
        public string file1Path;
        public string file2Path;

        public HashCalc()
        {
            help = "You can drag file to picture/panel 1/panel2";
        }
        public void GenerateHash_Click(string text)
        {

        }
        public string MD5(string path, CancellationToken token)
        {
            return null;
        }
        public string CRC64(string path, CancellationToken token)
        {
            return null;
        }
        public string SHA1(string path, CancellationToken token)
        {
            return null;
        }
        public string SHA256(string path, CancellationToken token)
        {
            return null;
        }
        public string CRC32(string path, CancellationToken token)
        {
            return null;
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
                        cancellationToken: s.Token,
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
        public static class HashAlgorithmExtensions
        {
            // hashAlgorithm = SHA1.Create()
            public static async Task<byte[]> ComputeHashAsync(
                HashAlgorithm hashAlgorithm, Stream stream,
                CancellationToken cancellationToken = default(CancellationToken),
                IProgress<long> progress = null,
                int bufferSize = 1024 * 1024)
            {
                byte[] readAheadBuffer, buffer, hash;
                int readAheadBytesRead, bytesRead;
                long size, totalBytesRead = 0;
                size = stream.Length;
                readAheadBuffer = new byte[bufferSize];
                readAheadBytesRead = await stream.ReadAsync(readAheadBuffer, 0,
                   readAheadBuffer.Length, cancellationToken);
                totalBytesRead += readAheadBytesRead;
                do
                {
                    bytesRead = readAheadBytesRead;
                    buffer = readAheadBuffer;
                    readAheadBuffer = new byte[bufferSize];
                    readAheadBytesRead = await stream.ReadAsync(readAheadBuffer, 0,
                        readAheadBuffer.Length, cancellationToken);
                    totalBytesRead += readAheadBytesRead;

                    if (readAheadBytesRead == 0)
                        hashAlgorithm.TransformFinalBlock(buffer, 0, bytesRead);
                    else
                        hashAlgorithm.TransformBlock(buffer, 0, bytesRead, buffer, 0);
                    if (progress != null)
                        progress.Report(totalBytesRead);
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();
                } while (readAheadBytesRead != 0);
                return hash = hashAlgorithm.Hash;
            }
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
