using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Threading;
using WisdomProjections.Data_Executor;
using WisdomProjections.Views.Sys;

namespace WisdomProjections.Views
{
    /// <summary>
    /// Interaction logic for ImageSelectView.xaml
    /// </summary>
    public partial class ImageSelectView : Grid
    {
        //private DrawingVisual drawingVisual;
        private ImageSelectDataExecutor imageSelectData;
        public ImageSelectView()
        {
            InitializeComponent();
            //if (drawingVisual == null)
            //{
            //    drawingVisual = new DrawingVisual();
            //    this.AddVisualChild(drawingVisual);
            //}

            if (imageSelectData == null)
            {
                imageSelectData = new ImageSelectDataExecutor();
            }

        }

        public void Draw(System.Windows.Controls.Image img)
        {
            //Bitmap bitmap = null;
            //App.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    ImageTool.ImageSourceToBitmap(img.Source, out  bitmap);
            //}),  bitmap);
            //imageSelectData.Draw3(img.ActualWidth, img.ActualHeight, bitmap, ImagePreview);
            App.Current?.Dispatcher?.BeginInvoke(new Action(() =>
            {
                ImageTool.ImageSourceToBitmap(img.Source, out var bitmap);
                imageSelectData.Draw2(img.ActualWidth, img.ActualHeight, bitmap, ImagePreview,canvas);
            }), DispatcherPriority.Render);

        }
        ////必须重载这两个方法，不然是画不出来的
        //// 重载自己的VisualTree的孩子的个数，由于只有一个DrawingVisual，返回1
        //protected override int VisualChildrenCount
        //{
        //    get { return 1; }
        //}

        //// 重载当WPF框架向自己要孩子的时候，返回返回DrawingVisual
        //protected override Visual GetVisualChild(int index)
        //{
        //    if (index == 0)
        //        return drawingVisual;

        //    throw new IndexOutOfRangeException();
        //}
    }
}
