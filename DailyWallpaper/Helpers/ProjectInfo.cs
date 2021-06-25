using System.Globalization;
using System.Windows.Forms;

namespace DailyWallpaper.Helpers
{
    class ProjectInfo
    {
        public static string author = "Jared DC";
        public static string email = "jared.dcx@gmail.com";
        public static string exeName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
        public static string logFile = new System.IO.FileInfo(exeName + ".log.txt").FullName;
        private static string officalWebSiteGlobal = "https://github.com/JaredDC/DailyWallpaperUI";
        private static string officalWebSiteCHN = "https://gitee.com/imtvip/DailyWallpaperUI";
        private static string newIssueCHN = "https://gitee.com/imtvip/DailyWallpaperUI/issues/new";
        private static string newIssueGlobal = "https://github.com/JaredDC/DailyWallpaperUI/issues/new";
        private static string donationUrlGlobal   = "https://github.com/JaredDC/DailyWallpaperUI/tree/master/Donate";
        private static string donationUrlCHN = "https://gitee.com/imtvip/DailyWallpaperUI/tree/master/Donate";

        public static string DonationUrl { 
            get {
                if (CultureInfo.CurrentUICulture.Equals(CultureInfo.GetCultureInfo("zh-CN"))) {
                    return donationUrlCHN;
                } else
                {
                    return donationUrlGlobal;
                }
            } 
        }
        
        public static string OfficalWebSite { get {
                if (CultureInfo.CurrentUICulture.Equals(CultureInfo.GetCultureInfo("zh-CN")))
                {
                    return officalWebSiteCHN;
                }
                else
                {
                    return officalWebSiteGlobal;
                }
            } 
        }
        public static string NewIssue
        {
            get
            {
                if (CultureInfo.CurrentUICulture.Equals(CultureInfo.GetCultureInfo("zh-CN")))
                {
                    return newIssueCHN;
                }
                else
                {
                    return newIssueGlobal;
                }
            }
        }
    }
}
