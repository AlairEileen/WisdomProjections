using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Image = System.Drawing.Image;

namespace WisdomProjections.Views
{
    public class BaseBlob : ICloneable
    {
        /// <summary>
        /// 模型数据
        /// </summary>
        public UIElement Data { get; set; }

        /// <summary>
        /// 视频资源
        /// </summary>
        public Uri VideoSource { get; set; }
        /// <summary>
        /// 图片资源
        /// </summary>
        public ImageSource PictureSource { get; set; }


        public void RePlay()
        {
            if (IsVideo && mediaElement != null)
            {
                mediaElement.Stop();
                mediaElement.Play();
            }
        }


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

        private MediaElement mediaElement = null;
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
            //添加素材代码
            var vb = new VisualBrush();
            if (IsVideo)
            {
                mediaElement = new MediaElement
                {
                    Source = meIconSource,
                    LoadedBehavior = MediaState.Play,
                    Stretch = Stretch.UniformToFill
                };
                mediaElement.LoadedBehavior = MediaState.Manual;
                mediaElement.Loaded += Me_Loaded;
                mediaElement.MediaEnded += Me_MediaEnded;
                vb.Visual = mediaElement;
            }
            else
            {
                var img = new System.Windows.Controls.Image { Source = iIconSource, Stretch = Stretch.UniformToFill };
                vb.Visual = img;
            }
            VideoSource = meIconSource;
            PictureSource = iIconSource;
            if (Data is RectangleView rv)
            {
                rv.bContent.Background = vb;
            }
            else if (Data is Path p)
            {
                //添加素材代码
                p.Fill = vb;
                p.Stroke = null;
            }
        }

        internal void Me_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (sender is MediaElement v)
            {
                v.Stop();
                v.Play();
            }
        }

        internal void Me_Loaded(object sender, RoutedEventArgs e)
        {
            var v = sender as MediaElement;
            v?.Play();
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



        public object Clone()
        {
            UIElement uie = null;
            var bb = new BaseBlob();

            var vb = new VisualBrush();
            if (IsVideo)
            {
                var me = new MediaElement { Source = VideoSource, LoadedBehavior = MediaState.Manual };
                me.Loaded += bb.Me_Loaded;
                me.MediaEnded += bb.Me_MediaEnded;
                vb.Visual = me;
            }
            else
            {
                vb.Visual = new System.Windows.Controls.Image { Source = PictureSource };
            }
            if (Data is RectangleView rv)
            {
                var rtf = new RotateTransform
                {
                    CenterX = rv.gRT.CenterX,
                    CenterY = rv.gRT.CenterY,
                    Angle = rv.gRT.Angle
                };

                uie = new System.Windows.Shapes.Rectangle
                {
                    Width = rv.bContent.ActualWidth,
                    Height = rv.bContent.ActualHeight,
                    RenderTransform = rtf,
                    Fill = vb,
                    RadiusX = rv.bContent.CornerRadius.BottomLeft,
                    RadiusY = rv.bContent.CornerRadius.BottomLeft
                };
                uie.SetValue(Canvas.LeftProperty, (double)Data.GetValue(Canvas.LeftProperty) + rv.bContent.Margin.Left);
                uie.SetValue(Canvas.TopProperty, (double)Data.GetValue(Canvas.TopProperty) + rv.bContent.Margin.Top);
            }
            else if (Data is Path p)
            {
                uie = new Path
                {
                    Data = p.Data,
                    Fill = vb
                };
                uie?.SetValue(Canvas.LeftProperty, Data.GetValue(Canvas.LeftProperty));
                uie?.SetValue(Canvas.TopProperty, Data.GetValue(Canvas.TopProperty));
            }


            bb.Data = uie;
            return bb;
        }
    }
}
