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
        public bool CanMouseMove { get; set; } = false;
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
        /// 鼠标缩放
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void img_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            var point = e.GetPosition(img);
            Console.WriteLine("position:"+point.X+","+point.Y);
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

            Point point1 = new Point(IMG.ActualWidth/2,ImgContent.ActualHeight/2);
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var d = delta * 0.001;
            DowheelZoom(group, point1, d);
        }
        private void DowheelZoom(TransformGroup group, Point point, double delta)
        {
            var pointToContent = group.Inverse.Transform(point);
            var transform = group.Children[0] as ScaleTransform;
            if (transform.ScaleX + delta < 0.1) return;
            transform.ScaleX += delta;
            transform.ScaleY += delta;
            var transform1 = group.Children[1] as TranslateTransform;
            transform1.X = -1 * ((pointToContent.X * transform.ScaleX) - point.X);
            transform1.Y = -1 * ((pointToContent.Y * transform.ScaleY) - point.Y);
        }
        #endregion


        private void Img_MouseMove(object sender, MouseEventArgs e)
        {
            if (!CanMouseMove)
            {
                return;
            }
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
        }

        private void Img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            img.ReleaseMouseCapture();
            mouseDown = false;

        }
    }
}
