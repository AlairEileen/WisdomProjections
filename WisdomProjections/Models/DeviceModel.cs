using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WisdomProjections.Views;

namespace WisdomProjections.Models
{
    public class DeviceModel
    {
        /// <summary>
        /// 屏幕
        /// </summary>
        public System.Windows.Forms.Screen Screen { get; set; }
        /// <summary>
        /// 视图
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 窗口
        /// </summary>
        public OutEffectsWindow Window { get; set; }
    }
}
