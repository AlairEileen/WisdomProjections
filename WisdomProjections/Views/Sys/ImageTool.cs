using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WisdomProjections.Views.Sys
{
    public class ImageTool
    {
        public static void ImageSourceToBitmap(ImageSource imageSource, out Bitmap bitmap)
        {

            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
              
                    var bf = BitmapFrame.Create((BitmapSource)imageSource);
                    encoder.Frames.Add(bf);
                    encoder.Save(ms);
                bitmap = new Bitmap(ms);
            }
        }

        public static void BitmapToImageSource(Bitmap bitmap, out ImageSource imageSource)
        {

            imageSource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
        }



    }
}
