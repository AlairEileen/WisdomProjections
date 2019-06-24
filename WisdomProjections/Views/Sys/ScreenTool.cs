using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace WisdomProjections.Views.Sys
{
    public class ScreenTool
    {
        public static Dpi GetDpiFromVisual(Visual visual)
        {
            var source = PresentationSource.FromVisual(visual);

            var dpiX = 96.0;
            var dpiY = 96.0;

            if (source?.CompositionTarget != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            return new Dpi(dpiX, dpiY);
        }

        public static double GetRealSize(Visual visual, double size)
        {
            return size * 96 / GetDpiFromVisual(visual).X;
        }
    }
    public struct Dpi
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Dpi(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
