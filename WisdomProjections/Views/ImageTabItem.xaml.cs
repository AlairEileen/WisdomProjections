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
    /// ImageTabItem.xaml 的交互逻辑
    /// </summary>
    public partial class ImageTabItem : TabItem
    {
        private TabControl tabControl;

        public ImageTabItem(string header,bool canClose, TabControl tabControl)
        {
            InitializeComponent();
            this.tabControl = tabControl;
            if (header.Length > 8)
            {
                var ex = header.Substring(header.LastIndexOf("."));
                var c = 8 - ex.Length - 2;
                header = "**" + header.Substring(header.LastIndexOf(".") - c, c) + ex;
            }
            this.tHeader.Content = header;
            if (!canClose)
            {
                this.tClose.Visibility = Visibility.Collapsed;
            }
            tabControl.Items.Add(this);

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
        private void img_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var img = sender as ContentControl;
            if (img == null)
            {
                return;
            }
            var point = e.GetPosition(img);
            var group = IMG.FindResource("Imageview") as TransformGroup;
            var delta = e.Delta * 0.001;
            DowheelZoom(group, point, delta);
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
        private void TClose_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            tabControl.Items.Remove(this);
        }

        private void Img_MouseMove(object sender, MouseEventArgs e)
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
