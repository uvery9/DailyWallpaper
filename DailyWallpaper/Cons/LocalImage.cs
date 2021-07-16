using DailyWallpaper;
using DailyWallpaper.Helpers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DailyWallpaper
{
	public class LocalImage
	{
		private List<string> files = null;
		private string path;
		private int invalidCnt = 0;
		private ConfigIni ini;
		private string txtFile;
		private List<string> old_files;
		public enum Update : int
		{
			AUTO,
			NO,
			FORCE,
			CleanInvalid,
			UNKNOWN
		}
		private Update update = Update.UNKNOWN;

		private static string GetHash(string input)
		{
			// Use input string to calculate MD5 hash
			using (MD5 md5 = MD5.Create())
			{
				byte[] inputBytes = System.Text.Encoding.Default.GetBytes(input);
				byte[] hashBytes = md5.ComputeHash(inputBytes);

				// Convert the byte array to hexadecimal string
				StringBuilder sb = new StringBuilder();
				for (int i = 0; i < hashBytes.Length; i++)
				{
					sb.Append(hashBytes[i].ToString("X2"));
				}
				return sb.ToString();
			}
		}

		private string GenerateListFileName(string path)
        {
			// name+md5+imgList.txt save the log file.
			var md5 = GetHash(path).Substring(0, 5);
			var name = new DirectoryInfo(path).Name;
			var imgListTxt = name + "-" + md5 + "-imgList.txt";
			var saveDir = Path.Combine(Helpers.ProjectInfo.executingLocation, "imglist_dir");
			if (!Directory.Exists(saveDir))
            {
				Directory.CreateDirectory(saveDir);
				Console.WriteLine($"Created saveDir: {saveDir}");
			}
			var imgListFile1st = Path.Combine(saveDir, imgListTxt);
			var imgListFile = imgListFile1st;
			var imgListFile2nd = Path.Combine(path, "_imgList.txt");
			if (File.Exists(imgListFile2nd))
			{
				imgListFile = imgListFile2nd;
			}
			if (!File.Exists(imgListFile1st) && !File.Exists(imgListFile2nd))
            {
				try
                {
					File.WriteAllText(imgListFile1st, "TEST");
					File.Delete(imgListFile1st);
					imgListFile = imgListFile1st;
				}
				catch (Exception e)
				{
					// may no permission???
					Console.Error.WriteLine(e.Message);
					Console.Error.WriteLine($"Maybe PATH IS TOO LOOG: {imgListFile1st}");
					Console.Error.WriteLine($"Maybe the program don't have permission.");
					Console.WriteLine($"Try to use: {imgListFile2nd}");
					try
					{
						File.WriteAllText(imgListFile2nd, "TEST");
						File.Delete(imgListFile2nd);
						imgListFile = imgListFile2nd;
					}
					catch (Exception err)
					{
						// may no permission???
						Console.Error.WriteLine(err.Message);
						var newLine = Environment.NewLine;
						throw new Exception(newLine + newLine+ newLine +
							$">>> Please contact me[{ProjectInfo.email}] with: " +
							$"{Path.Combine(ProjectInfo.executingLocation, ProjectInfo.exeName + ".log.txt")}:" +
							newLine + newLine + newLine
							);
					}

				}
			}			
			Console.WriteLine($"imgListFile: {imgListFile}");
			return imgListFile;
		}
		public LocalImage(ConfigIni ini, string path)
		{
			if (!Directory.Exists(path))
            {
				throw new DirectoryNotFoundException($"The folder MUST exists: {path}");
            }
            this.path = path;
			this.ini = ini;
			if (ini.EqualsIgnoreCase("localPathScan", "auto", "Local")) {
				update = LocalImage.Update.AUTO;
            }
            else if (ini.EqualsIgnoreCase("localPathScan", "no", "Local"))
            {
                update = LocalImage.Update.NO;
			}
			txtFile = GenerateListFileName(this.path);
			old_files = null;
	}

		private bool ShoulditUpdate()
		{
			if (!new FileInfo(this.txtFile).Exists)
			{
				this.update = Update.FORCE;
			}
			if (this.update == Update.FORCE || this.update == Update.CleanInvalid)
			{
				return true;
			} else if (this.update == Update.NO)
			{
				return false;
			}
			else // (this.update == Update.YES)
			{
				if (DateTime.TryParse(ini.Read("localPathMtime", "LOG"), out DateTime iniMtime))
				{
					var localPathMtime = new FileInfo(this.path).LastWriteTime;
					var timeDiff = Math.Abs((int)(localPathMtime - iniMtime).TotalSeconds);
					Console.WriteLine($"timeDiff: {timeDiff}s.");
					if (timeDiff > 20)
					{
						return true;
					}
					else
					{
						return false;
					}
				}
                else
                {
					return true;
				}
			}
		}

		public void ScanLocalPath(string path = null, bool print = true)
		{
			if (!ShoulditUpdate())
			{
				this.files = Txt2List(checkExist:false);
				return;
			}
			if (path == null)
			{
				path = this.path;
			}
			// && this.update != Update.CleanInvalid
			if (File.Exists(this.txtFile)) {
				if (File.Exists(txtFile) && this.old_files == null)
				{
					this.old_files = Txt2List(checkExist: true);
					if (this.update == Update.CleanInvalid)
					{
						ScanLocalPath();
						return;
					}
				}	
			}
			List<string> files = new List<string>();
			foreach (string file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
			{
				// Console.WriteLine($"file: {file}");
				if (this.old_files != null && this.old_files.Contains(file)){
					files.Add(file);
					continue;
				}
				long length = new FileInfo(file).Length / 1024;
				string file_low = file.ToLower();
				if (file_low.EndsWith(".jpg") || file_low.EndsWith(".jpeg") || file_low.EndsWith(".png"))
				{
					if (length > 100)
					{
						// 74s
						// using (var img = Image.FromFile(file))
						/*using (var img = new Bitmap(file)) { 
							if (img.Width > 1900 && (img.Width + 0.0 / img.Height > 1.4))
							{
								files.Add(file);
								if (print) { Console.WriteLine(file + ": " + length + "KB"); }
							}
						}*/

						// Cost 0.072833s
						using (Stream stream = File.OpenRead(file))
                        {
							// validateImageData = false will be super fast.
							using (var img = Image.FromStream(stream, false, false)) 
							{
								if (img.Width > 1900 && (img.Width + 0.0 / img.Height > 1.4))
								{
									files.Add(file);
									if (print) { Console.WriteLine(file + ": " + length + "KB"); }
								}
							}
						}

                    }
				}
			}
			if (files.Count < 1)
			{
				if (print) { Console.WriteLine("No suitable image files."); }
			}
			this.files = files;
			List2Txt(files, txtFile: txtFile);
		}
		public void List2Txt(List<string> filelist=null, string txtFile=null)
		{
			if (filelist == null)
            {
				filelist = this.files;
            }
			try
			{
				if (this.update != Update.NO)
				{
					File.WriteAllLines(txtFile, filelist);
					if (this.update == Update.FORCE)
					{
						Console.WriteLine("Created: {0}", txtFile);
						
					}
					else if (this.update == Update.CleanInvalid)
					{
						Console.WriteLine("Clean Invalid files[{1:D}]: {0}", txtFile, this.invalidCnt);
					}
					else
					{
						Console.WriteLine("Updated: {0}", txtFile);
					}
					ini.UpdateIniItem("localPathMtime", new FileInfo(this.path).LastWriteTime.ToString(), "LOG");
				}
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("Exception caught: {0}", e.Message);
			}
		}
		public List<string> Txt2List(bool checkExist=true)
		{
			try
			{
				List<string> readfiles = File.ReadLines(this.txtFile).ToList();
				if (!checkExist)
				{
					return readfiles;
				}
					List<string> effectivefiles = new List<string>();
				foreach (string file in readfiles)
				{
					if (File.Exists(file))
					{
						effectivefiles.Add(file);
					}
					else
					{
						Console.WriteLine("File does not exist: " + file);
					}

				}
				this.invalidCnt = readfiles.Count - effectivefiles.Count;
				if (this.invalidCnt != 0)
				{
					this.update = Update.CleanInvalid;
				}
				// this.files = effectivefiles;
				return effectivefiles;
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception caught: {0}", e.Message);
				return new List<string>();
			}
			
		}
		private void PrintList()
		{
			foreach (string file in this.files)
			{
				Console.WriteLine(file);
			}
		}
		
		// give up this feature
		private void CopyTo()
        {
			if (ini.EqualsIgnoreCase("want2Copy", "yes", "Local"))
			{
				var copyFolder = ini.Read("copyFolder", "Local");
				if (string.IsNullOrEmpty(copyFolder))
                {
					return;
                }
				string existListTxt = Path.Combine(copyFolder, "_existing_file_list.txt");
				bool firstCopy = true;
				if (File.Exists(existListTxt))
				{
					firstCopy = false;
				}
				var existList = new List<string>();
				if (!Directory.Exists(copyFolder))
				{
					Directory.CreateDirectory(copyFolder);
					Console.WriteLine($"Created copyFolder: {copyFolder}");
				}
				foreach (var fi in this.files)
				{
					var newFi = Path.Combine(copyFolder, new FileInfo(fi).Name);
					if (!File.Exists(newFi))
					{
						File.Copy(fi, newFi);
						// Console.WriteLine($"Copied to: {newFi}");
					}
					else
					{
						if (firstCopy)
						{
							existList.Add(fi);
							Console.WriteLine($"File exists: {newFi}");
						}
					}
				}
				if (existList.Count > 0)
				{
					File.WriteAllLines(existListTxt, existList);
				}
			}
		}
		
		public string RandomSelectOne()
		{
			ScanLocalPath();
			// CopyTo();
			var random = new Random();
			int index = random.Next(files.Count);
			if (files.Count < 1)
            {
				throw new ArgumentOutOfRangeException("There are no suitable pictures in the LocalPath: " + Environment.NewLine+
					"1)jpg/jpeg/png,  2)>100kb,  3)Width>1900,  4)Width/Height>1.4");
                
			}
			return this.files[index];
		}
	}
}