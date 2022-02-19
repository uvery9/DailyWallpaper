using System.Windows.Forms;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace DailyWallpaper
{
    class Utils
    {
        // 扫描屏幕上的二维码
        private static int MAX_TRY = 3;
        public static string ScanScreen()
        {
            try
            {
                foreach (Screen screen in Screen.AllScreens)
                {
                    using (Bitmap fullImage = new Bitmap(screen.Bounds.Width,
                                                    screen.Bounds.Height))
                    {
                        using (Graphics g = Graphics.FromImage(fullImage))
                        {
                            g.CopyFromScreen(screen.Bounds.X,
                                             screen.Bounds.Y,
                                             0, 0,
                                             fullImage.Size,
                                             CopyPixelOperation.SourceCopy);
                        }
                        for (int i = 0; i < MAX_TRY; i++)
                        {
                            int marginLeft = (int)((double)fullImage.Width * i / 2.5 / MAX_TRY);
                            int marginTop = (int)((double)fullImage.Height * i / 2.5 / MAX_TRY);
                            Rectangle cropRect = new Rectangle(marginLeft, marginTop, fullImage.Width - marginLeft * 2, fullImage.Height - marginTop * 2);
                            Bitmap target = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
                            double imageScale = (double)screen.Bounds.Width / (double)cropRect.Width;
                            using (Graphics g = Graphics.FromImage(target))
                            {
                                g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
                                                cropRect,
                                                GraphicsUnit.Pixel);
                            }
                            // target.Save("DAILYWALLPAPER.SCANQRCODE." + i + ".jpg");
                            BitmapLuminanceSource source = new BitmapLuminanceSource(target);
                            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                            QRCodeReader reader = new QRCodeReader();
                            Result result = reader.decode(bitmap);
                            if (result != null)
                            {
                                return result.Text;
                            }
                        }
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        public static Image GenerateQRCodeImage(string strContent)
        {
            try
            {
                QrCodeEncodingOptions options = new QrCodeEncodingOptions
                {
                    CharacterSet = "UTF-8",
                    DisableECI = true, // Extended Channel Interpretation (ECI) 主要用于特殊的字符集。并不是所有的扫描器都支持这种编码。
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M, // ErrorCorrection level
                    Width = 500,
                    Height = 500,
                    Margin = 1
                };
                BarcodeWriter writer = new BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = options
                };
                return writer.Write(strContent);
            }
            catch
            {
                return null;
            }
        }

    }
}


