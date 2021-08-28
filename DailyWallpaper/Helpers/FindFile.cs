using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DailyWallpaper.Helpers
{
    class FindFile
    {
        // ignore case

        public static async Task<List<string>> FindAsync(string path, string searchPattern, 
            bool ignoreCase = false, CancellationToken token = default)
        {
            if (!Directory.Exists(path))
                return null;
            var pathPattern = "*";
            if (searchPattern.Contains("\\"))
            {
                var res = searchPattern.Split('\\');
                searchPattern = res.Last();
                pathPattern = res[0];
            }
            Debug.WriteLine($"> searchPattern = {searchPattern}");
            Debug.WriteLine($"> pathPattern = {pathPattern}");
            return await Task.Run(() => ScanDirsRecursively(path, searchPattern, pathPattern,
                    token, ignoreCase)
                );
        }

        public static async Task<List<string>> FindIgnoreCaseAsync(string path, string searchPattern, CancellationToken token = default)
        {
            return await FindAsync(path, searchPattern, ignoreCase: true, token);
        }


        // [DebuggerStepThrough]
        private static void FindFileInCurrentFolder(string path, string searchPattern, List<string> resList, bool ignoreCase = false)
        {
            try
            {
                if (string.IsNullOrEmpty(path))
                    return;
                // Debug.WriteLine("-->" + path);
                // EnumerateFiles ignoreCase
                foreach (var fi in Directory.EnumerateFiles(path, "*" + searchPattern + "*", SearchOption.TopDirectoryOnly))
                {
                    // Debug.WriteLine("-->" + fi);
                    if (ignoreCase)
                    {
                        // new FileInfo().Name
                        if (fi.ToLower().Contains(searchPattern.ToLower()))
                        {
                            resList.Add(fi);
                            Debug.WriteLine("ic-->" + fi);
                            return;
                        }
                            
                    }
                    else
                    {
                        if (fi.Contains(searchPattern))
                        {
                            resList.Add(fi);
                            // Debug.WriteLine("-->" + fi);
                            return;
                        }
                            
                    }
                }
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException)
            {
                // Debug.WriteLine("!IOException>" + path);
            }
        }


        private static List<string> ScanDirsRecursively(string dir, string searchPattern, string pathPattern, 
            CancellationToken token, bool ignoreCase = false, List<string> resList = null)
        {
            if (String.IsNullOrEmpty(dir))
            {
                throw new ArgumentException("Starting directory is a null reference or an empty string: dir");
            }
            if (resList == null)
                resList = new List<string>();
            try
            {
                // var enumDirs = Directory.EnumerateDirectories(dir, "*bin*", SearchOption.AllDirectories);
                // Debug.WriteLine("--->CNT=" + enumDirs.Count());
                foreach (var d in Directory.EnumerateDirectories(dir)) // , "*", SearchOption.TopDirectoryOnly
                {
                    // FUCK THE $RECYCLE.BIN
                    if (d.ToLower().Contains("$RECYCLE.BIN".ToLower()))
                        continue;
                    
                    if (token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }
                    ScanDirsRecursively(d, searchPattern, pathPattern, token, ignoreCase, resList);
                }
                if (token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                }
                if (dir.Contains(pathPattern) || (ignoreCase && dir.ToLower().Contains(pathPattern.ToLower())))
                {
                    FindFileInCurrentFolder(dir, searchPattern, resList, ignoreCase);
                }                
            }
            catch (UnauthorizedAccessException) { }
            catch (IOException) { }
            catch (OperationCanceledException)
            {
                // Debug.WriteLine("> User Canceled.");
            }
            return resList;
        }



    }
}
