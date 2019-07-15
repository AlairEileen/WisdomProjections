using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string Title { get; set; }
        public int ZIndex { get; set; }
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
        /// <summary>
        /// 原点
        /// </summary>
        Point pointOrigin;
        ///// <summary>
        ///// 矩形原点
        ///// </summary>
        //Point pointO;
        /// <summary>
        /// 被动点
        /// </summary>
        Point pointPassiveM;
        /// <summary>
        /// 动点
        /// </summary>
        Point pointM;
        /// <summary>
        /// 移动用到的点
        /// </summary>
        Point pointMove;
        /// <summary>
        /// 旧的移动用到的点
        /// </summary>
        Point pointMoveOld;
        /// <summary>
        /// 移动前的点
        /// </summary>
        Point pointOldM;
        /// <summary>
        /// image工厂画布
        /// </summary>
        private ImageFactoryView ifv;

        private Point pointOriginInside;
        private Point pointOriginInsideNew;

        internal bool OnContainerMouseMove(object sender, MouseEventArgs e)
        {
            return DoMove(sender, e);
        }



        /// <summary>
        /// 根据余弦定理求两个线段夹角
        /// </summary>
        /// <param name="o">端点</param>
        /// <param name="s">start点</param>
        /// <param name="e">end点</param>
        /// <returns></returns>
        double Angle(Point o, Point s, Point e) 
        {
            double cosfi = 0, fi = 0, norm = 0;
            double dsx = s.X - o.X;
            double dsy = s.Y - o.Y;
            double dex = e.X - o.X;
            double dey = e.Y - o.Y;

            cosfi = dsx * dex + dsy * dey;
            norm = (dsx * dsx + dsy * dsy) * (dex * dex + dey * dey);
            cosfi /= Math.Sqrt(norm);

            if (cosfi >= 1.0) return 0;
            if (cosfi <= -1.0) return Math.PI;
            fi = Math.Acos(cosfi);

            if (180 * fi / Math.PI < 180)
            {
                return 180 * fi / Math.PI;
            }
            else
            {
                return 360 - 180 * fi / Math.PI;
            }
        }


        /// <summary>
        /// 计算两点距离
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns></returns>
        public static double GetDistance(Point startPoint, Point endPoint)
        {
            var x = Math.Abs(endPoint.X - startPoint.X);
            var y = Math.Abs(endPoint.Y - startPoint.Y);
            return Math.Sqrt(x * x + y * y);

        }
        /// <summary>
        /// 计算两点距离
        /// </summary>
        /// <param name="startPoint">起点</param>
        /// <param name="endPoint">终点</param>
        /// <returns></returns>
        public static double GetDistancePoint(Point startPoint, Point endPoint)
        {
            var x = Math.Abs(endPoint.X - startPoint.X);
            var y = Math.Abs(endPoint.Y - startPoint.Y);
            return Math.Sqrt(x * x + y * y);

        }

        private bool DoMove(object sender, MouseEventArgs e)
        {
            bool isEx = true;
            if (mouseIsDown)
            {
               
                pointM = e.GetPosition(ifv.canvas);

                pointMove = e.GetPosition(this);

                var cy = (pointM.Y - pointOrigin.Y);
                var cx = (pointM.X - pointOrigin.X);

                var mX=(pointMove.X- pointMoveOld.X);
                var mY=(pointMove.Y- pointMoveOld.Y);


                var h = this.Height;
                var w = this.Width;

                double nX = 0, nY = 0;
              



                var l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty));
                var t = Convert.ToDouble(this.GetValue(Canvas.TopProperty));

                var oc = Angle(pointOrigin, pointPassiveM, pointM);
                var pl = GetDistance(pointM, pointOrigin);

                var ol = (pl * Math.Sin(Math.PI / (180 / oc))) / Math.Sin(Math.PI / 2);
                var ml = Math.Sqrt(pl * pl - ol * ol);

                var resizeType = ResizeType.None;

                switch (CurrentPointType)
                {
                    case PointLocationType.LT:
                        e.MouseDevice.SetCursor(Cursors.SizeNWSE);
                        //h = this.Height - cy;
                        //w = this.Width - cx;
                        //l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx;
                        //t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + cy;
                        resizeType = ResizeType.All;
                        break;
                    case PointLocationType.LC:
                        e.MouseDevice.SetCursor(Cursors.SizeWE);
                        //w = this.Width - cx;
                        //l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx;
                        resizeType = ResizeType.With;
                        break;
                    case PointLocationType.LB:
                        e.MouseDevice.SetCursor(Cursors.SizeNESW);
                        //w = this.Width - cx;
                        //h = this.Height + cy;
                        //l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx;
                        //pointOrigin = pointM;
                        resizeType = ResizeType.All;
                        break;
                    case PointLocationType.CT:
                        e.MouseDevice.SetCursor(Cursors.SizeNS);
                        //h = this.Height - cy;
                        //t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + cy;

                        resizeType = ResizeType.Height;
                        break;
                    case PointLocationType.CB:
                        e.MouseDevice.SetCursor(Cursors.SizeNS);
                        //h = this.Height + cy;
                        //pointOrigin = pointM;
                        resizeType = ResizeType.Height;

                        break;
                    case PointLocationType.RT:
                        e.MouseDevice.SetCursor(Cursors.SizeNESW);
                        //w = ml;
                        //h = ol;
                        //this.Height = h + 30;
                        //this.Width = w + 30;
                        //pointOriginInsideNew = CurrentDptb.TranslatePoint(new Point(), this);
                        //nX = -pointOriginInsideNew.X + pointOriginInside.X;
                        //nY = -pointOriginInsideNew.Y + pointOriginInside.Y;
                        //pointOriginInside = pointOriginInsideNew;
                        //pointOrigin = pointM;
                        resizeType = ResizeType.All;
                        break;
                    case PointLocationType.RC:
                        e.MouseDevice.SetCursor(Cursors.SizeWE);
                        //w = this.Width + cx;
                        //pointOrigin = pointM;
                        resizeType = ResizeType.With;
                        break;
                    case PointLocationType.RB:
                        e.MouseDevice.SetCursor(Cursors.SizeNWSE);
                        //w = ml;
                        //h = ol;
                        //this.Height = h + 30;
                        //this.Width = w + 30;
                        //pointOriginInsideNew = CurrentDptb.TranslatePoint(new Point(), this);
                        //nX = -pointOriginInsideNew.X + pointOriginInside.X;
                        //nY = -pointOriginInsideNew.Y + pointOriginInside.Y;
                        //pointOriginInside = pointOriginInsideNew;
                        resizeType = ResizeType.All;

                        break;
                    case PointLocationType.CC:
                        e.MouseDevice.SetCursor(Cursors.SizeAll);
                        l += mX;
                        t += mY;
                        //pointMoveOld = pointMove;
                        break;
                    default:
                        isEx = false;
                        break;
                }


                switch (resizeType)
                {
                    case ResizeType.All:
                        w = ml;
                        h = ol;
                        this.Height = h + 50;
                        this.Width = w + 50;
                        pointOriginInsideNew = CurrentDptb.TranslatePoint(new Point(), this);
                        nX = -pointOriginInsideNew.X + pointOriginInside.X;
                        nY = -pointOriginInsideNew.Y + pointOriginInside.Y;
                        pointOriginInside = pointOriginInsideNew;
                        break;
                    case ResizeType.Height:
                        //w = ml;
                        h = ol;
                        this.Height = h + 100;
                        //this.Width = w + 30;
                        pointOriginInsideNew = CurrentDptb.TranslatePoint(new Point(), this);
                        nX = -pointOriginInsideNew.X + pointOriginInside.X;
                        nY = -pointOriginInsideNew.Y + pointOriginInside.Y;
                        pointOriginInside = pointOriginInsideNew;
                        break;
                    case ResizeType.With:
                        w = ml;
                        //h = ol;
                        //this.Height = h + 30;
                        this.Width = w + 100;
                        pointOriginInsideNew = CurrentDptb.TranslatePoint(new Point(), this);
                        nX = -pointOriginInsideNew.X + pointOriginInside.X;
                        nY = -pointOriginInsideNew.Y + pointOriginInside.Y;
                        pointOriginInside = pointOriginInsideNew;
                        break;
                    case ResizeType.None:
                        break;
                }


                //if (CurrentPointType==PointLocationType.CC)
                //{
                //    e.MouseDevice.SetCursor(Cursors.SizeAll);
                //    l += mX;
                //    t += mY;
                //}
                //else
                //{
                //    w = ml;
                //    h = ol;
                //    this.Height = h + 30;
                //    this.Width = w + 30;
                //    pointOriginInsideNew = CurrentDptb.TranslatePoint(new Point(), this);
                //    nX = -pointOriginInsideNew.X + pointOriginInside.X;
                //    nY = -pointOriginInsideNew.Y + pointOriginInside.Y;
                //    pointOriginInside = pointOriginInsideNew;
                //}
                if (h > 0 && w > 0)
                {

                    //    var pd = bPLB.TranslatePoint(new Point(),ifv.canvas);
                    //this.Height = h;
                    //this.Width = w;
                    this.SetValue(Canvas.LeftProperty, l + nX);
                    this.SetValue(Canvas.TopProperty, t + nY);
                    //this.SetValue(Canvas.LeftProperty, l - pd.X + pointOrigin.X);
                    //this.SetValue(Canvas.TopProperty, t -pd.Y + pointOrigin.Y);
                }

            }
            else isEx = false;
            return isEx;
        }

        private Dictionary<PointLocationType, Border> dptb;
        private Dictionary<PointLocationType, Border> dptbM;

        public Border CurrentDptb =>
            (dptb ?? (dptb = new Dictionary<PointLocationType, Border>
            {
                {PointLocationType.RT, bPLB},
                {PointLocationType.RB, bPLT},
                {PointLocationType.RC, bPLC},
                {PointLocationType.LT, bPRB},
                {PointLocationType.LC, bPRC},
                {PointLocationType.LB, bPRT},
                {PointLocationType.CT, bPCB},
                {PointLocationType.CB, bPCT},
                {PointLocationType.NO, bPCT},
                {PointLocationType.CC, bPCT},
            }))[CurrentPointType];
        public Border CurrentDptbM =>
            (dptbM ?? (dptbM = new Dictionary<PointLocationType, Border>
            {
                {PointLocationType.RT, bPRB},
                {PointLocationType.RB, bPRT},
                {PointLocationType.RC, bPRB},
                {PointLocationType.LT, bPLB},
                {PointLocationType.LC, bPLB},
                {PointLocationType.LB, bPLT},
                {PointLocationType.CT, bPLT},
                {PointLocationType.CB, bPLB},
                {PointLocationType.NO, bPLB},
                {PointLocationType.CC, bPLB},
            }))[CurrentPointType];




        Point Pointdinate(Point a, Point b, Point c)//a点为直角点，求第4个点的坐标.
        {
            Point po = new Point();
            po.X = b.X + c.X - a.X;
            po.Y = b.Y + c.Y - a.Y;
            return po;
        }

        bool Judge(Point a, Point b, Point c)//判断向量数量积是否为0.
        {
            if (((b.X - a.X) * (c.X - a.X) + (b.Y - a.Y) * (c.Y - a.Y)) != 0)
                return true;
            return false;
        }



        void Seeking(Point[] a)
        {
            if (Judge(a[0], a[1], a[2]))//判断直角点.
                a[3] = Pointdinate(a[0], a[1], a[2]);
            else if (Judge(a[1], a[0], a[2]))
                a[3] = Pointdinate(a[1], a[0], a[2]);
            else a[3] = Pointdinate(a[2], a[0], a[1]);
            //a[3]即为第4个点.
        }








        private Point CalcPoint(Point op, Point pointM, Point pointPassiveM)
        {
            var g = new Point();
            if ((op.X - pointM.X) * (pointM.X - pointPassiveM.X) + (op.Y - pointM.Y) * (pointM.Y - pointPassiveM.Y) == 0)
            {
                g = GetPoint(op.X, op.Y, pointM.X, pointM.Y, pointPassiveM.X, pointPassiveM.Y);
            }
            if ((op.X - pointPassiveM.X) * (pointM.X - pointPassiveM.X) + (op.Y - pointPassiveM.Y) * (pointM.Y - pointPassiveM.Y) == 0)
            {
                g = GetPoint(op.X, op.Y, pointPassiveM.X, pointPassiveM.Y, pointM.X, pointM.Y);
            }
            if ((op.X - pointPassiveM.X) * (pointM.X - op.X) + (op.Y - pointPassiveM.Y) * (pointM.Y - op.Y) == 0)
            {
                g = GetPoint(pointPassiveM.X, pointPassiveM.Y, op.X, op.Y, pointM.X, pointM.Y);
            }
            return g;
        }

        private Point GetPoint(double x1, double y1, double x2, double y2, double x3, double y3)
        {
            Point g = new Point();
            g.X = x1 + x3 - x2; g.Y = y1 + y3 - y2;
            return g;
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
                //case PointLocationType.LB:
                //    pointOrigin = e.GetPosition(bRT);
                //    pointPassiveM = bRB.TranslatePoint(new Point(), bRT);
                //    break;
                //case PointLocationType.RT:
                //    pointOrigin = bPLB.TranslatePoint(new Point(), ifv.canvas);
                //    pointOriginInside = bPLB.TranslatePoint(new Point(), this);
                //    pointPassiveM = bPRB.TranslatePoint(new Point(), ifv.canvas);
                //    pointOldM = e.GetPosition(ifv.canvas);
                //    //pointO = bPLT.TranslatePoint(new Point(), ifv.canvas);
                //    break;
                //case PointLocationType.RB:
                //    pointOrigin = bPLT.TranslatePoint(new Point(), ifv.canvas);
                //    pointOriginInside = bPLT.TranslatePoint(new Point(), this);
                //    pointPassiveM = bPRT.TranslatePoint(new Point(), ifv.canvas);
                //    pointOldM = e.GetPosition(ifv.canvas);
                //    //pointO = bPLT.TranslatePoint(new Point(), ifv.canvas);
                //    break;
                //case PointLocationType.LT:
                //    pointOrigin = e.GetPosition(bRB);
                //    pointPassiveM = bLB.TranslatePoint(new Point(), bRB);
                //    break;
                //case PointLocationType.LC:
                //    pointOrigin = e.GetPosition(bRB);
                //    pointPassiveM = bLB.TranslatePoint(new Point(), bRB);
                //    break;
                case PointLocationType.CC:
                    pointMoveOld = e.GetPosition( this);
                    break;
            }
            pointOrigin = CurrentDptb.TranslatePoint(new Point(), ifv.canvas);
            pointOriginInside = CurrentDptb.TranslatePoint(new Point(), this);
            pointPassiveM = CurrentDptbM.TranslatePoint(new Point(), ifv.canvas);
            pointOldM = e.GetPosition(ifv.canvas);


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

        public void MoveWithKey(int size, MoveType moveType)
        {
            var l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + (moveType == MoveType.Left ? -size : moveType == MoveType.Right ? size : 0);
            var t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + (moveType == MoveType.Top ? -size : moveType == MoveType.Bottom ? size : 0);
            this.SetValue(Canvas.LeftProperty, l);
            this.SetValue(Canvas.TopProperty, t);
        }
        public void RotateWithKey(int size, RotateType rotateType)
        {
            //RotateTransform rotateTransform = new RotateTransform(rotateType == RotateType.Clockwise ? size : -size);   //其中180是旋转180度
            //double cx = this.ActualWidth / 2;
            //double cy = this.ActualHeight / 2;
            //rotateTransform.CenterX = cx <= 0 ? 0 : cx;
            //rotateTransform.CenterY = cy <= 0 ? 0 : cy;
            //bCC.GetValue(Canvas.LeftProperty);
            //gTG.Children.Add(rotateTransform);
            gRT.Angle += rotateType == RotateType.Clockwise ? size : -size;
        }
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


    public enum ResizeType
    {
        With,Height,All,None
    }

    public enum MoveType
    {
        /// <summary>
        /// 左
        /// </summary>
        Left,
        /// <summary>
        /// 上
        /// </summary>
        Top,
        /// <summary>
        /// 右
        /// </summary>
        Right,
        /// <summary>
        /// 下
        /// </summary>
        Bottom
    }

    public enum RotateType
    {
        /// <summary>
        /// 顺时针
        /// </summary>
        Clockwise,
        /// <summary>
        /// 逆时针
        /// </summary>
        Anticlockwise
    }
}
