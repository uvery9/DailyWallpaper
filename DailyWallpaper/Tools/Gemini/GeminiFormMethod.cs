using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.IO;
using System.Windows.Forms;
using DailyWallpaper.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using DailyWallpaper.Tools;
using System.Diagnostics;
using System.Collections;
using static DailyWallpaper.Tools.Gemini;
using System.Drawing;
using System.Security.Cryptography;
using System.Reflection;
using Button = System.Windows.Forms.Button;

namespace DailyWallpaper
{
    partial class GeminiForm
    {
        private delegate void EnableButtonDelegate(Button b, bool enable);

        private void EnableButton(Button b, bool enable)
        {
            if (b.InvokeRequired) //  && b.IsHandleCreated
            {
                var f = new EnableButtonDelegate(EnableButton);
                b.Invoke(f, new object[] { b, enable });
            }
            else
            {
                b.Enabled = enable;
            } 
        }

        private enum ListViewOP
        {
            ADD,
            CLEAR,
            UPDATE_CHECK,
            UPDATE_CHECK_INTHELOOP,
            UPDATE_CHECK_BY_INDEX,
            CEF_INTHELOOP,
            UPDATE_INDEX_AFTER_SORTED
        }


        private delegate void ListViewOperateLoopDelegate(System.Windows.Forms.ListView liv,
            ListViewOP op, List<GeminiFileStruct> gfl = null,
            Action<bool, List<GeminiFileStruct>> actionLoop = null);

        private void ListViewOperateLoop(System.Windows.Forms.ListView liv,
            ListViewOP op, List<GeminiFileStruct> gfl = null,
            Action<bool, List<GeminiFileStruct>> actionLoop = null)
        {
            if (liv.InvokeRequired)
            {
                var addDele = new ListViewOperateLoopDelegate(ListViewOperateLoop);
                liv.Invoke(addDele, new object[] { liv, op, gfl, actionLoop });
            }
            else
            {
                if (op == ListViewOP.UPDATE_CHECK_INTHELOOP)
                {
                    var tmpL = new List<GeminiFileStruct>();
                    foreach (var it in liv.Items)
                    {
                        var lvi = ((System.Windows.Forms.ListViewItem)it);
                        var fullPathLV = lvi.SubItems["fullPath"].Text;
                        foreach (var gf in gfl)
                        {
                            var tmp = gf;
                            if (gf.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                            {
                                tmp.Checked = lvi.Checked;
                                tmpL.Add(tmp);
                                break;
                            }
                        }
                    }
                    actionLoop(true, tmpL);

                }
                if (op == ListViewOP.UPDATE_CHECK_BY_INDEX)
                {
                    var tmpL = new List<GeminiFileStruct>();
                    foreach (var gf in gfl)
                    {
                        var tmp = gf;
                        var lvi = liv.Items[tmp.index];
                        var fullPathLV = lvi.SubItems["fullPath"].Text;
                        if (gf.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                        {
                            tmp.Checked = lvi.Checked;
                            tmpL.Add(tmp);
                        }
                        else
                        {
                            actionLoop(false, tmpL);
                        }
                    }
                    actionLoop(true, tmpL);

                }
                if (op == ListViewOP.UPDATE_INDEX_AFTER_SORTED)
                {
                    var tmpL = new List<GeminiFileStruct>();
                    int index = 0;
                    foreach (var it in liv.Items)
                    {
                        var lvi = ((System.Windows.Forms.ListViewItem)it);
                        /*_console.WriteLine(liv.Items.IndexOf(lvi));*/
                        // how to update index in SubItems
                        lvi.SubItems["index"].Text = index.ToString();
                        lvi.SubItems[0].Text = index.ToString();
                        var fullPathLV = lvi.SubItems["fullPath"].Text;
                        foreach (var gf in gfl)
                        {
                            var tmp = gf;
                            if (gf.fullPath.ToLower().Equals(fullPathLV.ToLower()))
                            {
                                tmp.index = index;
                                tmpL.Add(tmp);
                                break;
                            }
                        }
                        index++;
                    }
                    actionLoop(true, tmpL);
                }
            }

        }


        private delegate void ListViewOperateDelegate(System.Windows.Forms.ListView liv, ListViewOP op,
            System.Windows.Forms.ListViewItem item = null, bool ischeck = false);
        private void ListViewOperate(System.Windows.Forms.ListView liv, ListViewOP op,
            System.Windows.Forms.ListViewItem item = null, bool ischeck = false
           )
        {
            if (liv.InvokeRequired)
            {
                var addDele = new ListViewOperateDelegate(ListViewOperate);
                liv.Invoke(addDele, new object[] { liv, op, item, ischeck });
            }
            else
            {
                if (op == ListViewOP.ADD)
                {
                    liv.Items.Add(item);
                }
                if (op == ListViewOP.CLEAR)
                {
                    liv.Items.Clear();
                }
                if (op == ListViewOP.UPDATE_CHECK)
                {
                    item.Checked = ischeck;
                }
            }
        }
    }
}