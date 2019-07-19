using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WisdomProjections.Models
{
    public class RectPathModel
    {
        public double Width { get; set; }
        public double Height { get; set; }
        public Point Location { get; set; }
        public Point[] Points { get; set; }
    }
}
