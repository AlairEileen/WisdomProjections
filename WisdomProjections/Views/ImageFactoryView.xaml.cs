using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WisdomProjections.Views
{
    /// <summary>
    /// Interaction logic for ImageFactoryView.xaml
    /// </summary>
    public partial class ImageFactoryView : Grid
    {
        public PaintTypeSelect[] PaintTypeSelects { get; set; }
        public List<RectangleView> RectangleViews { get; } = new List<RectangleView>();
        public ImageFactoryView()
        {
            InitializeComponent();
        }
        public ImageSource GetImageSource()
        {

            return img.Source;
        }
        public void SetImageSource(ImageSource imageSource)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                img.Source = imageSource;
            });
        }

        #region 图片缩放
        /// <summary>
        /// 鼠标缩放与拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            var point = e.GetPosition(img);
            //Console.WriteLine("position:" + point.X + "," + point.Y);
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var delta = e.Delta * 0.001;
            DowheelZoom(group, point, delta);
        }
        /// <summary>
        /// 自定义缩放
        /// </summary>
        /// <param name="delta">缩放大小</param>
        public void ChangeImageSize(int delta)
        {

            Point point1 = new Point(IMG.ActualWidth / 2, ImgContent.ActualHeight / 2);
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var d = delta * 0.001;
            DowheelZoom(group, point1, d);
        }
        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            var ch = canvas.ActualHeight;
            var cw = canvas.ActualWidth;
            Console.WriteLine($"前:{cw},{ch}");

            var pointToContent = group.Inverse.Transform(point);
            var transform = group.Children[0] as ScaleTransform;
            if (transform.ScaleX + delta < 0.1) return;
            transform.ScaleX += delta;
            transform.ScaleY += delta;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);

            var nh = canvas.ActualHeight;
            var nw = canvas.ActualWidth;
            Console.WriteLine($"后:{nw},{nh}");
            //changeOtherSize(ch,cw,nh,nw);

        }

        private void changeOtherSize(double ch, double cw, double nh, double nw)
        {

            foreach (var item in RectangleViews)
            {
                item.SetValue(Canvas.LeftProperty, Convert.ToDouble(item.GetValue(Canvas.LeftProperty)) * nw / cw);
                item.SetValue(Canvas.TopProperty, Convert.ToDouble(item.GetValue(Canvas.TopProperty)) * nh / ch);
            }
        }

        /// <summary>
        /// 鼠标拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
          
            switch (GetPaintTypeSelect().PaintType)
            {
                case PaintType.None:
                    break;
                case PaintType.Move:
                    var isEx0 = false;
                    foreach (var item in RectangleViews)
                    {
                        if (!isEx0)
                        {
                            if (item.CurrentPointType != PointLocationType.NO)
                                isEx0 = item.OnContainerMouseMove(sender, e);

                        }
                        else item.OnContainerMouseMove(sender, e);
                    }
                    if (!isEx0)
                    {
                        var img = sender as ContentControl;
                        if (img == null)
                        {
                            return;
                        }
                        if (mouseDown)
                        {
                            Domousemove(img, e);
                        }
                    }


                    break;
                case PaintType.Rectangle:
                    var position = e.GetPosition(canvas);
                    if (mouseDown)
                    {
                        if (position.X >= mouseCanvasXY.X && position.Y >= mouseCanvasXY.Y)
                        {
                            RectangleViews[RectangleViews.Count - 1].Height = position.Y - mouseCanvasXY.Y;
                            RectangleViews[RectangleViews.Count - 1].Width = position.X - mouseCanvasXY.X;
                        }
                    }
                    break;

                default:
                    break;
            }

        }


        /// <summary>
        /// 拖动实现
        /// </summary>
        /// <param name="img"></param>
        /// <param name="e"></param>
        private void Domousemove(ContentControl img, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
            {
                return;
            }
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var transform = group.Children[1] as TranslateTransform;
            var position = e.GetPosition(img);
            transform.X -= mouseXY.X - position.X;
            transform.Y -= mouseXY.Y - position.Y;
            mouseXY = position;
        }

        private bool mouseDown;
        private Point mouseXY;
        private Point mouseCanvasXY;
        /// <summary>
        /// 鼠标左键按下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.CaptureMouse();
            mouseDown = true;
            mouseXY = e.GetPosition(img);


            switch (GetPaintTypeSelect().PaintType)
            {
                case PaintType.None:
                    break;
                case PaintType.Move:
                    break;
                case PaintType.Rectangle:
                    mouseCanvasXY = e.GetPosition(canvas);
                    RectangleViews.Add(new RectangleView(this,0, 0));
                    RectangleViews[RectangleViews.Count - 1].SetValue(Canvas.LeftProperty, mouseCanvasXY.X);
                    RectangleViews[RectangleViews.Count - 1].SetValue(Canvas.TopProperty, mouseCanvasXY.Y);
                    canvas.Children.Add(RectangleViews[RectangleViews.Count - 1]);
                    break;
                default:
                    break;
            }
           

            foreach (var item in RectangleViews)
            {
                
                item.OnContainerMouseDown(sender, e);
            }
        }
        /// <summary>
        /// 鼠标左键松开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;

            foreach (var item in RectangleViews)
            {
                item.OnContainerMouseUp(sender, e);
            }
        }

        #endregion



        public PaintTypeSelect GetPaintTypeSelect()
        {
            return PaintTypeSelects.Where(x => x.IsSelected).FirstOrDefault();
        }
    }
    public class PaintTypeSelect
    {
        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set
            {
                isSelected = value;
                if (TypeView != null)
                {
                    TypeView.Opacity = isSelected ? 0.5 : 1;
                }

            }
        }

        public void ChangeCursor(Window window)
        {
            if (isSelected)
            {
                switch (PaintType)
                {
                    case PaintType.None:
                        window.Cursor = Cursors.Arrow;
                        break;
                    case PaintType.Move:
                        window.Cursor = Cursors.Hand;
                        break;
                    case PaintType.Rectangle:
                        window.Cursor = Cursors.Cross;
                        break;
                    case PaintType.Cricle:
                        window.Cursor = Cursors.Cross;
                        break;
                    case PaintType.Pen:
                        window.Cursor = Cursors.Pen;
                        break;
                    default:
                        break;
                }
            }
        }
        public PaintType PaintType { get; set; }
        public Image TypeView { get; set; }
    }


    public enum PaintType
    {
        None, Move, Rectangle, Cricle, Pen
    }
}
