using System.Drawing;

namespace DailyWallpaper.Tools.Gemini
{
    public class GeminiCEFCls
    {
        private bool @checked;
        public string name;
        public string fullPath;
        public string lastMtime;
        public Color color;

        public bool Checked { get => @checked; set => @checked = value; }
    }
}
