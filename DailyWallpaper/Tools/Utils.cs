using System.Windows.Forms;
using System.Drawing;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;
using System.Drawing.Imaging;

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
                            using (Graphics g = Graphics.FromImage(target))
                            {
                                g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
                                                cropRect,
                                                GraphicsUnit.Pixel);
                            }
                            //target.Save("DAILYWALLPAPER.SCANQRCODE." + i + ".jpg");
                            BitmapLuminanceSource source = new BitmapLuminanceSource(target);
                            BinaryBitmap bitmap = new BinaryBitmap(new HybridBinarizer(source));
                            QRCodeReader reader = new QRCodeReader();
                            Result result = reader.decode(bitmap);
                            if (result != null)
                            {
                                return result.Text;
                            }
                        }
                        for (int i = 0; i < 1; i++)
                        {
                            Rectangle cropRect = new Rectangle(0, 0, fullImage.Width, fullImage.Height);
                            Bitmap target = new Bitmap(screen.Bounds.Width, screen.Bounds.Height);
                            using (Graphics g = Graphics.FromImage(target))
                            {
                                g.DrawImage(fullImage, new Rectangle(0, 0, target.Width, target.Height),
                                                cropRect,
                                                GraphicsUnit.Pixel);
                            }
                            var gray = MakeGrayscale3(target);
                            //gray.Save("DAILYWALLPAPER.SCANQRCODE.GRAY.jpg");
                            BitmapLuminanceSource source = new BitmapLuminanceSource(gray);
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

        public static Image ConvertToGrayScale(Image srce)
        {
            Bitmap bmp = new Bitmap(srce.Width, srce.Height);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                var matrix = new float[][] {
                    new float[] { 0.299f, 0.299f, 0.299f, 0, 0 },
                    new float[] { 0.587f, 0.587f, 0.587f, 0, 0 },
                    new float[] { 0.114f, 0.114f, 0.114f, 0, 0 },
                    new float[] { 0,      0,      0,      1, 0 },
                    new float[] { 0,      0,      0,      0, 1 }
                };
                var ia = new ImageAttributes();
                ia.SetColorMatrix(new ColorMatrix(matrix));
                var rc = new Rectangle(0, 0, srce.Width, srce.Height);
                gr.DrawImage(srce, rc, 0, 0, srce.Width, srce.Height, GraphicsUnit.Pixel, ia);
                return bmp;
            }
        }

        public static Bitmap MakeGrayscale3(Bitmap original)
        {
            //create a blank bitmap the same size as original
            Bitmap newBitmap = new Bitmap(original.Width, original.Height);

            //get a graphics object from the new image
            using (Graphics g = Graphics.FromImage(newBitmap))
            {

                //create the grayscale ColorMatrix
                ColorMatrix colorMatrix = new ColorMatrix(
                   new float[][]
                   {
             new float[] {.3f, .3f, .3f, 0, 0},
             new float[] {.59f, .59f, .59f, 0, 0},
             new float[] {.11f, .11f, .11f, 0, 0},
             new float[] {0, 0, 0, 1, 0},
             new float[] {0, 0, 0, 0, 1}
                   });

                //create some image attributes
                using (ImageAttributes attributes = new ImageAttributes())
                {

                    //set the color matrix attribute
                    attributes.SetColorMatrix(colorMatrix);

                    //draw the original image on the new image
                    //using the grayscale color matrix
                    g.DrawImage(original, new Rectangle(0, 0, original.Width, original.Height),
                                0, 0, original.Width, original.Height, GraphicsUnit.Pixel, attributes);
                }
            }
            return newBitmap;
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


