using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WisdomProjections.Views.Sys
{
   public class ImageTool
    {
        public static Bitmap ImageSourceToBitmap(ImageSource imageSource)
        {
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            BmpBitmapEncoder encoder = new BmpBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create((BitmapSource)imageSource));
            encoder.Save(ms);

            Bitmap bp = new Bitmap(ms);
            ms.Close();
            return bp;
        }

        public static ImageSource BitmapToImageSource(Bitmap bitmap)
        {
            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return bs;
        }

        

        }
    }
