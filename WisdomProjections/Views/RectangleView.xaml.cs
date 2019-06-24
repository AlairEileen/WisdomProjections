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
    /// Interaction logic for RectangleView.xaml
    /// </summary>
    public partial class RectangleView : Grid
    {
        private bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                bContent.BorderBrush = new SolidColorBrush(selected ? Colors.Red : Colors.Blue);
            }
        }
        private bool isVideo;
        public bool IsVideo
        {
            get => isVideo; set
            {
                isVideo = value;
                img.Visibility = isVideo ? Visibility.Hidden : Visibility.Visible;
                video.Visibility = isVideo ? Visibility.Visible : Visibility.Hidden;
            }
        }
        public RectangleView(ImageFactoryView ifv, double width, double height)
        {
            InitializeComponent();
            this.Height = height;
            this.Width = width;
            this.ifv = ifv;
        }
        public PointLocationType CurrentPointType { get; set; }

        private bool mouseIsDown;
        #region 各个点的鼠标进入事件
        private void BLT_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.LT; this.Cursor = Cursors.SizeNWSE; }

        }
        private void BLB_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.LB; this.Cursor = Cursors.SizeNESW; }

        }
        private void BLC_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.LC; this.Cursor = Cursors.SizeWE; }

        }
        private void BCT_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.CT; this.Cursor = Cursors.SizeNS; }

        }
        private void BCB_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.CB; this.Cursor = Cursors.SizeNS; }
        }
        private void BRT_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.RT; this.Cursor = Cursors.SizeNESW; }

        }
        private void BRC_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.RC; this.Cursor = Cursors.SizeWE; }

        }
        private void BRB_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.RB; this.Cursor = Cursors.SizeNWSE; }
        }

        #endregion

        #region 父容器鼠标事件
        Point ltdM;
        private ImageFactoryView ifv;

        internal bool OnContainerMouseMove(object sender, MouseEventArgs e)
        {
            return DoMove(sender, e);

        }
        private bool DoMove(object sender, MouseEventArgs e)
        {
            bool isEx = true;
            if (mouseIsDown)
            {
                var p = e.GetPosition(CurrentPointType == PointLocationType.LB ? bRT : CurrentPointType == PointLocationType.RT ? bLB : bLT);
                var cy = p.Y - ltdM.Y;
                var cx = p.X - ltdM.X;
                var h = this.Height;
                var w = this.Width;
                var l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty));
                var t = Convert.ToDouble(this.GetValue(Canvas.TopProperty));
                switch (CurrentPointType)
                {
                    case PointLocationType.LT:
                        e.MouseDevice.SetCursor(Cursors.SizeNWSE);
                        h = this.Height - cy;
                        w = this.Width - cx;
                        l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx;
                        t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + cy;
                        break;
                    case PointLocationType.LC:
                        e.MouseDevice.SetCursor(Cursors.SizeWE);
                        w = this.Width - cx;
                        l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx;
                        break;
                    case PointLocationType.LB:
                        e.MouseDevice.SetCursor(Cursors.SizeNESW);
                        w = this.Width - cx;
                        h = this.Height + cy;
                        l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx;
                        ltdM = p;
                        break;
                    case PointLocationType.CT:
                        e.MouseDevice.SetCursor(Cursors.SizeNS);
                        h = this.Height - cy;
                        t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + cy;
                        break;
                    case PointLocationType.CB:
                        e.MouseDevice.SetCursor(Cursors.SizeNS);
                        h = this.Height + cy;
                        ltdM = p;
                        break;
                    case PointLocationType.RT:
                        e.MouseDevice.SetCursor(Cursors.SizeNESW);
                        w = this.Width + cx;
                        h = this.Height - cy;
                        t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + cy;
                        ltdM = p;
                        break;
                    case PointLocationType.RC:
                        e.MouseDevice.SetCursor(Cursors.SizeWE);
                        w = this.Width + cx;
                        ltdM = p;
                        break;
                    case PointLocationType.RB:
                        e.MouseDevice.SetCursor(Cursors.SizeNWSE);
                        h = this.Height + cy;
                        w = this.Width + cx;
                        ltdM = p;
                        break;
                    case PointLocationType.CC:
                        e.MouseDevice.SetCursor(Cursors.SizeAll);
                        l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx;
                        t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + cy;
                        break;
                    default:
                        isEx = false;
                        break;
                }
                if (h > 0 && w > 0)
                {
                    this.Height = h;
                    this.Width = w;
                    this.SetValue(Canvas.LeftProperty, l);
                    this.SetValue(Canvas.TopProperty, t);
                }

            }
            else isEx = false;
            return isEx;
        }
        internal void OnContainerMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartMove(e);

        }
        private void StartMove(MouseButtonEventArgs e)
        {
            mouseIsDown = true;
            switch (CurrentPointType)
            {
                case PointLocationType.LB:
                    ltdM = e.GetPosition(bRT);
                    break;
                case PointLocationType.RT:
                    ltdM = e.GetPosition(bLB);
                    break;
                default:
                    ltdM = e.GetPosition(bLT);
                    break;
            }

        }
        internal void OnContainerMouseUp(object sender, MouseButtonEventArgs e)
        {
            EndMove();
        }
        private void EndMove()
        {
            mouseIsDown = false;
            CurrentPointType = PointLocationType.NO;
        }
        #endregion


        #region 挪动控件
        private void BContent_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { ifv.RectangleViews.ForEach(x => x.CurrentPointType = PointLocationType.NO); CurrentPointType = PointLocationType.CC; this.Cursor = Cursors.SizeAll; }
        }
        //private void BContent_MouseMove(object sender, MouseEventArgs e)
        //{
        //    DoMove(sender, e);
        //}

        //private void BContent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    StartMove(e);
        //}

        //private void BContent_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    EndMove();
        //}
        #endregion

        private void BContent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pt = ifv.PaintTypeSelects.Where(x => x.PaintType == PaintType.None && x.IsSelected).FirstOrDefault();
            if (pt != null)
            {
                ifv.RectangleViews.ForEach(x => x.Selected = false);
                Selected = true;
            }
        }
    }

    public enum PointLocationType
    {
        LT, LC, LB, CT, CB, RT, RC, RB, NO, CC
    }


}
