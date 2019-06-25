using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WisdomProjections.Views.Sys
{
   public class ColorTool
    {
        public static System.Windows.Media.Color GetMediaColorFromDrawingColor(System.Drawing.Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
