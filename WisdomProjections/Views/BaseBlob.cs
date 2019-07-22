using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WisdomProjections.Views
{
    public class BaseBlob
    {
        /// <summary>
        /// 模型数据
        /// </summary>
        public UIElement Data { get; set; }
        private bool selected;
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Selected
        {
            get
            {
                if (Data is RectangleView rv)
                {
                    return rv.Selected;
                }

                return selected;
            }
            set
            {
                selected = value;
                if (Data is RectangleView rv)
                {
                    rv.Selected = selected;
                }
            }
        }
        /// <summary>
        /// 特效是否为video
        /// </summary>
        public bool IsVideo { get; set; }
        /// <summary>
        /// 鼠标提示
        /// </summary>
        public string ToolTip
        {
            set
            {
                if (Data is RectangleView rv)
                {
                    rv.ToolTip = value;
                }
            }
        }
        /// <summary>
        /// 当前点的类型
        /// </summary>
        public PointLocationType CurrentPointType
        {
            get
            {
                if (Data is RectangleView rv)
                {
                    return rv.CurrentPointType;
                }
                return PointLocationType.NO;
            }
            set
            {
                if (Data is RectangleView rv)
                {
                    rv.CurrentPointType = value;
                }
            }
        }

        public double Height
        {
            get
            {
                if (Data is RectangleView rv)
                {
                    return rv.Height;
                }

                return 0;
            }
            set
            {
                if (Data is RectangleView rv)
                {
                    rv.Height = value;
                }
            }
        }

        public double Width
        {
            get
            {
                if (Data is RectangleView rv)
                {
                    return rv.Width;
                }

                return 0;
            }
            set
            {
                if (Data is RectangleView rv)
                {
                    rv.Width = value;
                }
            }
        }


        public void RotateWithKey(int size)
        {
            if (Data is RectangleView rv)
            {
                rv.RotateWithKey(size);
            }
        }


        internal void MoveWithKey(int size, MoveType moveType)
        {
            if (Data is RectangleView rv)
            {
                rv.MoveWithKey(size, moveType);
            }
        }

        public void SetMedia(Uri meIconSource, ImageSource iIconSource)
        {
            if (Data is RectangleView rv)
            {
                if (IsVideo)
                {
                    rv.video.Visibility = Visibility.Visible;
                    rv.img.Visibility = Visibility.Hidden;
                    rv.video.Source = meIconSource;
                }
                else
                {
                    rv.img.Visibility = Visibility.Visible;
                    rv.video.Visibility = Visibility.Hidden;
                    rv.img.Source = iIconSource;
                }
            }
        }

        public bool OnContainerMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (Data is RectangleView rv)
            {
                return rv.OnContainerMouseMove(sender, mouseEventArgs);
            }

            return false;
        }

        public void OnContainerMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (Data is RectangleView rv)
            {
                rv.OnContainerMouseDown(sender, mouseButtonEventArgs);
            }
        }

        public void OnContainerMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (Data is RectangleView rv)
            {
                rv.OnContainerMouseUp(sender, mouseButtonEventArgs);
            }
        }
    }
}
