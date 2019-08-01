using System.Collections.Generic;

namespace WisdomProjections.Extensions
{
    public static class PointExtension
    {
        public static System.Windows.Point ToWindowsPoint(this System.Drawing.Point point)
        {
            return new System.Windows.Point(point.X, point.Y);
        }
        public static System.Drawing.Point ToDrawingPoint(this System.Windows.Point point)
        {
            return new System.Drawing.Point((int)point.X, (int)point.Y);
        }

        public static List<System.Windows.Point> ToWindowsPointList(this List<System.Drawing.Point> points)
        {
            var wp = new List<System.Windows.Point>();
            points.ForEach(x => wp.Add(x.ToWindowsPoint()));
            return wp;
        }
    }
}