using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
using WisdomProjections.Views;
using WisdomProjections.Views.Sys;
using YDPeopleSensor.Net;

namespace WisdomProjections
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SensorDataExecutor.SensorDE.InitContent(imgContainer);
            Closing += SensorDataExecutor.SensorDE.Close;
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Button btnGrdSplitter = gsSplitterr.Template.FindName("btnExpend", gsSplitterr) as Button;
            Button btnGrdSplitter2 = gsSplitterr2.Template.FindName("btnExpend", gsSplitterr2) as Button;
            if (btnGrdSplitter != null)
                btnGrdSplitter.Click += new RoutedEventHandler(BtnGrdSplitter_Click);
            if (btnGrdSplitter2 != null)
                btnGrdSplitter2.Click += new RoutedEventHandler(BtnGrdSplitter2_Click);

            InitData();

        }

        private void InitData()
        {
            imgContainer.PaintTypeSelects = new PaintTypeSelect[] {
                new PaintTypeSelect { PaintType = PaintType.None, IsSelected = true },
            new PaintTypeSelect { PaintType = PaintType.Cricle, IsSelected = false ,TypeView=imgCircle},
            new PaintTypeSelect { PaintType = PaintType.Move, IsSelected = false ,TypeView=imgSlect},
            new PaintTypeSelect { PaintType = PaintType.Rectangle, IsSelected = false,TypeView=imgSquare },
            new PaintTypeSelect { PaintType = PaintType.Pen, IsSelected = false ,TypeView=imgPen},
        };


            cbEffectsType1.ItemsSource = new string[] { "143", "飞机打瞌睡了.", "扣税的", "房贷首付" };
            cbEffectsType2.ItemsSource = new string[] { "发士大夫", "反倒是.", "fsffdsf", "f234ferfewrdsads" };
            var a = new List<TextListViewItem>();
            var a2 = new List<TextListViewItem>();
            var a3 = new List<EffectsListViewItem>();
            for (int i = 0; i < 30; i++)
            {
                a.Add(new TextListViewItem("模型" + i));
                a2.Add(new TextListViewItem("设备" + i));
                a3.Add(new EffectsListViewItem("特效" + i, "fjkldsjffsdk分离技术的路口附近空军副司令雕刻技法路口附近上空的房间里上空的飞机弗兰克点击上方空间分厘卡电视机开了房间裂缝的快速减肥离开房间扣税的分厘卡电视机发", null));
            }
            lvModel.ItemsSource = a;
            lvDevice.ItemsSource = a2;

            lvEffects.ItemsSource = a3;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }


        //private IOutputArray contours;

        #region 导航按钮点击事件
        /// <summary>
        /// 返回点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 下一个点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 拖动点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgSlect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPaintType(PaintType.Move);

        }
        /// <summary>
        /// 矩形点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgSquare_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPaintType(PaintType.Rectangle);
        }

        private void SwitchPaintType(PaintType rectangle)
        {
            foreach (var item in imgContainer.PaintTypeSelects)
            {
                if (item.PaintType == rectangle && item.IsSelected)
                {
                    item.IsSelected = false;
                    imgContainer.PaintTypeSelects.Where(x => x.PaintType == PaintType.None).FirstOrDefault().IsSelected = true;
                    item.ChangeCursor(this);
                    break;
                }
                else
                {
                    item.IsSelected = item.PaintType == rectangle;
                    item.ChangeCursor(this);
                }
            }

        }

        /// <summary>
        /// 圆点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgCircle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 笔点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgPen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 缩小点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgShrink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgContainer.ChangeImageSize(-60);
        }
        /// <summary>
        /// 放大点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgMagnify_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgContainer.ChangeImageSize(60);
        }
        private void ChangeCursor()
        {

        }
        #endregion


        #region 左右两侧折叠与缩放
        GridLength m_WidthCache1, m_WidthCache2;
        int grid1Index = 0, grid2Index = 4;
        public void BtnGrdSplitter_Click(object sender, RoutedEventArgs e)
        {
            GridSplitterClick(ref m_WidthCache1, grid1Index);
        }
        private void BtnGrdSplitter2_Click(object sender, RoutedEventArgs e)
        {
            GridSplitterClick(ref m_WidthCache2, grid2Index);
        }

        private void ICleanSearchText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbSearch.Text = "";
        }



        private void GridSplitterClick(ref GridLength w, int grid1Index)
        {
            GridLength temp = grdWorkbench.ColumnDefinitions[grid1Index].Width;
            GridLength def = new GridLength(0);
            if (temp.Equals(def))
            {
                //恢复
                grdWorkbench.ColumnDefinitions[grid1Index].Width = w;
            }
            else
            {
                //折叠
                w = grdWorkbench.ColumnDefinitions[grid1Index].Width;
                grdWorkbench.ColumnDefinitions[grid1Index].Width = def;
            }
        }
        #endregion



        ///create by alair
        ///
        public Bitmap FloodFill(Bitmap src, System.Drawing.Point location, System.Drawing.Color fillColor, int threshould)
        {
            try
            {
                Bitmap srcbmp = src;
                Bitmap dstbmp = new Bitmap(src.Width, src.Height);
                int w = srcbmp.Width;
                int h = srcbmp.Height;
                Stack<System.Drawing.Point> fillPoints = new Stack<System.Drawing.Point>(w * h);
                System.Drawing.Imaging.BitmapData bmpData = srcbmp.LockBits(new System.Drawing.Rectangle(0, 0, srcbmp.Width, srcbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData dstbmpData = dstbmp.LockBits(new System.Drawing.Rectangle(0, 0, dstbmp.Width, dstbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytes = bmpData.Stride * srcbmp.Height;
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
                System.Drawing.Color backColor = System.Drawing.Color.FromArgb(grayValues[location.X * 3 + 2 + location.Y * stride], grayValues[location.X * 3 + 1 + location.Y * stride], grayValues[location.X * 3 + location.Y * stride]);

                IntPtr dstptr = dstbmpData.Scan0;
                byte[] temp = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dstptr, temp, 0, bytes);

                int gray = (int)((backColor.R + backColor.G + backColor.B) / 3);
                if (location.X < 0 || location.X >= w || location.Y < 0 || location.Y >= h) return null;
                fillPoints.Push(new System.Drawing.Point(location.X, location.Y));
                int[,] mask = new int[w, h];

                while (fillPoints.Count > 0)
                {

                    System.Drawing.Point p = fillPoints.Pop();
                    mask[p.X, p.Y] = 1;
                    temp[3 * p.X + p.Y * stride] = (byte)fillColor.B;
                    temp[3 * p.X + 1 + p.Y * stride] = (byte)fillColor.G;
                    temp[3 * p.X + 2 + p.Y * stride] = (byte)fillColor.R;
                    if (p.X > 0 && (Math.Abs(gray - (int)((grayValues[3 * (p.X - 1) + p.Y * stride] + grayValues[3 * (p.X - 1) + 1 + p.Y * stride] + grayValues[3 * (p.X - 1) + 2 + p.Y * stride]) / 3)) < threshould) && (mask[p.X - 1, p.Y] != 1))
                    {
                        temp[3 * (p.X - 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X - 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X - 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X - 1, p.Y));
                        mask[p.X - 1, p.Y] = 1;
                    }
                    if (p.X < w - 1 && (Math.Abs(gray - (int)((grayValues[3 * (p.X + 1) + p.Y * stride] + grayValues[3 * (p.X + 1) + 1 + p.Y * stride] + grayValues[3 * (p.X + 1) + 2 + p.Y * stride]) / 3)) < threshould) && (mask[p.X + 1, p.Y] != 1))
                    {
                        temp[3 * (p.X + 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X + 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X + 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X + 1, p.Y));
                        mask[p.X + 1, p.Y] = 1;
                    }
                    if (p.Y > 0 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y - 1) * stride] + grayValues[3 * p.X + 1 + (p.Y - 1) * stride] + grayValues[3 * p.X + 2 + (p.Y - 1) * stride]) / 3)) < threshould) && (mask[p.X, p.Y - 1] != 1))
                    {
                        temp[3 * p.X + (p.Y - 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y - 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y - 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X, p.Y - 1));
                        mask[p.X, p.Y - 1] = 1;
                    }
                    if (p.Y < h - 1 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y + 1) * stride] + grayValues[3 * p.X + 1 + (p.Y + 1) * stride] + grayValues[3 * p.X + 2 + (p.Y + 1) * stride]) / 3)) < threshould) && (mask[p.X, p.Y + 1] != 1))
                    {
                        temp[3 * p.X + (p.Y + 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y + 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y + 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X, p.Y + 1));
                        mask[p.X, p.Y + 1] = 1;
                    }
                }
                fillPoints.Clear();

                System.Runtime.InteropServices.Marshal.Copy(temp, 0, dstptr, bytes);
                srcbmp.UnlockBits(bmpData);
                dstbmp.UnlockBits(dstbmpData);

                return dstbmp;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return null;
            }
        }


    }


}
