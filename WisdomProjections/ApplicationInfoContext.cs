
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WisdomProjections
{
    public class ApplicationInfoContext:App
    {
        public static bool IsIpCamera => IpCameraInfo != null;
        public static IpCamera IpCameraInfo { get; set; }
       
    }
}
