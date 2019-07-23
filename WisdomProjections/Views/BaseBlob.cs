using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
                else if (Data is Path p)
                {
                    p.Stroke = new SolidColorBrush(selected ? Colors.Red : Colors.Blue);
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
                else if (Data is Path p)
                {
                    p.ToolTip = value;
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
        /// <summary>
        /// 设置矩形高度
        /// </summary>
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
        /// <summary>
        /// 设置矩形宽度
        /// </summary>
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

        /// <summary>
        /// 矩形旋转
        /// </summary>
        /// <param name="size"></param>
        public void RotateWithKey(int size)
        {
            if (Data is RectangleView rv)
            {
                rv.RotateWithKey(size);
            }
        }

        /// <summary>
        /// 矩形移动
        /// </summary>
        /// <param name="size"></param>
        /// <param name="moveType"></param>
        internal void MoveWithKey(int size, MoveType moveType)
        {
            if (Data is RectangleView rv)
            {
                rv.MoveWithKey(size, moveType);
            }
        }
        /// <summary>
        /// 设置特效
        /// </summary>
        /// <param name="meIconSource"></param>
        /// <param name="iIconSource"></param>
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
            else if (Data is Path p)
            {
                //添加素材代码
                var vb = new VisualBrush();
                if (IsVideo)
                {
                    var me = new MediaElement
                    {
                        Source = meIconSource,
                        LoadedBehavior = MediaState.Play,
                        Stretch = Stretch.UniformToFill
                    };
                    vb.Visual = me;
                }
                else
                {
                    var img = new Image { Source = iIconSource, Stretch = Stretch.UniformToFill };
                    vb.Visual = img;
                }
                p.Fill = vb;
                p.Stroke = null;
            }
        }
        /// <summary>
        /// 父容器鼠标移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseEventArgs"></param>
        /// <returns></returns>
        public bool OnContainerMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (Data is RectangleView rv)
            {
                return rv.OnContainerMouseMove(sender, mouseEventArgs);
            }

            return false;
        }
        /// <summary>
        /// 父容器鼠标左键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseButtonEventArgs"></param>
        public void OnContainerMouseDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (Data is RectangleView rv)
            {
                rv.OnContainerMouseDown(sender, mouseButtonEventArgs);
            }
        }
        /// <summary>
        /// 父容器鼠标抬起
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="mouseButtonEventArgs"></param>
        public void OnContainerMouseUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (Data is RectangleView rv)
            {
                rv.OnContainerMouseUp(sender, mouseButtonEventArgs);
            }
        }
    }
}
