using DailyWallpaper;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace DailyWallpaper
{
	public class LocalImage
	{
		private List<string> files = null;
		private string path;
		private int invalidCnt = 0;
		private ConfigIni ini;
		private string localPathScan;
		// private readonly string timeFormat = "yyyy-MM-dd HH:mm:ss";
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
		
		public LocalImage(ConfigIni ini, string path)
		{
			if (!Directory.Exists(path))
            {
				throw new DirectoryNotFoundException($"The folder MUST exists: {path}");
            }
			this.path = path;
			this.ini = ini;
			this.localPathScan = ini.GetCfgFromIni()["localPathScan"];
			if (this.localPathScan.ToLower().Equals("auto")) {
				this.update = LocalImage.Update.AUTO;
			} else if (this.localPathScan.ToLower().Equals("no"))
            {
				this.update = LocalImage.Update.NO;
			}
			this.txtFile = Path.Combine(this.path, "_img_list.txt");
			this.old_files = null;
	}

		private bool ShoulditUpdate()
		{
			if (!new FileInfo(this.txtFile).Exists)
			{
				this.update = Update.FORCE;
				// update config.ini modified time.
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
				var lastLocalPathSetting = ini.GetCfgFromIni()["lastLocalPathSetting"];
				if (this.path != lastLocalPathSetting)
				{
					Console.WriteLine($"lastLocalPathSetting: {lastLocalPathSetting}");
					return true;
				} else {
					var iniMtime = DateTime.Parse(ini.GetCfgFromIni()["localPathMtime"]);
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
						Bitmap img = new Bitmap(file);
						// new FileInfo(file).
						if (img.Width > 1900 && (img.Width + 0.0 / img.Height > 1.4))
						{
							files.Add(file);
							if (print) { Console.WriteLine(file + ": " + length + "KB"); }
						}
						img.Dispose();
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
					ini.UpdateIniItem("localPathMtime", new FileInfo(this.path).LastWriteTime.ToString(), "Local");
					// update config.ini modified time.
				}
			}
			catch (ArgumentNullException e)
			{
				Console.WriteLine("Exception caught: {0}", e);
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
				Console.WriteLine("Exception caught: {0}", e);
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
			if (ini.GetCfgFromIni()["want2Copy"].ToLower().Equals("yes"))
			{
				var copyFolder = ini.GetCfgFromIni()["copyFolder"];
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
		
		public string RandomSelectOneImgToWallpaper()
		{
			ScanLocalPath();
			// CopyTo();
			var random = new Random();
			int index = random.Next(this.files.Count);
			return this.files[index];
		}
	}
}