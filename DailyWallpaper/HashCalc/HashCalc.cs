﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Threading;

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