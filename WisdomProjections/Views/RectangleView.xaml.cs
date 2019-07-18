using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

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

        private List<ElementWithPointType> elementWithPointTypes;
        private List<ElementWithPointType> ElementWithPointTypes =>
            elementWithPointTypes ?? (elementWithPointTypes = new List<ElementWithPointType>()
            {
                new ElementWithPointType
                    {Element = bLT, Cursor = Cursors.SizeNWSE, PointLocationType = PointLocationType.LT,RelativeBorder = bPRB,AdjacentBorder = bPLB,ResizeType = ResizeType.All},
                new ElementWithPointType
                    {Element = bLB, Cursor = Cursors.SizeNESW, PointLocationType = PointLocationType.LB,RelativeBorder = bPRT,AdjacentBorder = bPLT,ResizeType = ResizeType.All},
                new ElementWithPointType
                    {Element = bLC, Cursor = Cursors.SizeWE, PointLocationType = PointLocationType.LC,RelativeBorder = bPRB,AdjacentBorder = bPLB,ResizeType = ResizeType.With},
                new ElementWithPointType
                    {Element = bCT, Cursor = Cursors.SizeNS, PointLocationType = PointLocationType.CT,RelativeBorder = bPRB,AdjacentBorder = bPRT,ResizeType = ResizeType.Height},
                new ElementWithPointType
                    {Element = bCB, Cursor = Cursors.SizeNS, PointLocationType = PointLocationType.CB,RelativeBorder = bPRT,AdjacentBorder = bPRB,ResizeType = ResizeType.Height},
                new ElementWithPointType
                    {Element = bRT, Cursor = Cursors.SizeNESW, PointLocationType = PointLocationType.RT,RelativeBorder = bPLB,AdjacentBorder = bPRB,ResizeType = ResizeType.All},
                new ElementWithPointType
                    {Element = bRC, Cursor = Cursors.SizeWE, PointLocationType = PointLocationType.RC,RelativeBorder = bPLB,AdjacentBorder = bPRB,ResizeType = ResizeType.With},
                new ElementWithPointType
                    {Element = bRB, Cursor = Cursors.SizeNWSE, PointLocationType = PointLocationType.RB,RelativeBorder = bPLT,AdjacentBorder = bPRT,ResizeType = ResizeType.All},
                new ElementWithPointType
                    {Element = pRotate, Cursor = Cursors.Hand, PointLocationType = PointLocationType.Rotate,RelativeBorder = bPCT,AdjacentBorder = bPLB,ResizeType = ResizeType.None},
                new ElementWithPointType
                    {Element = bContent, Cursor = Cursors.SizeAll, PointLocationType = PointLocationType.CC,RelativeBorder = bPCT,AdjacentBorder = bPLB,ResizeType = ResizeType.None},
                new ElementWithPointType
                    {Element = bContent, Cursor = Cursors.SizeAll, PointLocationType = PointLocationType.NO,RelativeBorder = bPCT,AdjacentBorder = bPLB,ResizeType = ResizeType.None},
            });

        private void UIElement_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown)
            {
                var ept = ElementWithPointTypes.FirstOrDefault(x => x.Element.Equals(sender as UIElement));
                ifv?.RectangleViews?.ForEach(x => x.CurrentPointType = PointLocationType.NO);
                if (ept != null)
                {
                    CurrentPointType = ept.PointLocationType;
                    this.Cursor = ept.Cursor;
                }
            }
        }

        #endregion

        #region 父容器鼠标事件
        /// <summary>
        /// 原点
        /// </summary>
        Point pointOrigin;
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
        /// image工厂画布
        /// </summary>
        private readonly ImageFactoryView ifv;

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

        private bool DoMove(object sender, MouseEventArgs e)
        {
            bool isEx = true;
            if (mouseIsDown)
            {
                isEx = CurrentOperate.PointLocationType != PointLocationType.NO;
                pointM = e.GetPosition(ifv.canvas);
                pointMove = e.GetPosition(this);
                var mX = (pointMove.X - pointMoveOld.X);
                var mY = (pointMove.Y - pointMoveOld.Y);
                var h = this.Height;
                var w = this.Width;
                double nX = 0, nY = 0;
                var l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty));
                var t = Convert.ToDouble(this.GetValue(Canvas.TopProperty));
                var oc = Angle(pointOrigin, pointPassiveM, pointM);
                var pl = GetDistance(pointM, pointOrigin);
                var ol = (pl * Math.Sin(Math.PI / (180 / oc))) / Math.Sin(Math.PI / 2);
                var ml = Math.Sqrt(pl * pl - ol * ol);
                e.MouseDevice.SetCursor(CurrentOperate.Cursor);
                switch (CurrentPointType)
                {
                    case PointLocationType.CC:
                        l += mX;
                        t += mY;
                        break;
                    case PointLocationType.Rotate:
                        Point currentLocation = Mouse.GetPosition(this);
                        Point knobCenter = new Point(this.ActualHeight / 2, this.ActualWidth / 2);
                        double radians = Math.Atan((currentLocation.Y - knobCenter.Y) /
                                                   (currentLocation.X - knobCenter.X));
                        gRT.Angle = radians * 180 / Math.PI + 90;
                        if (currentLocation.X - knobCenter.X < 0)
                        {
                            gRT.Angle += 180;
                        }
                        break;
                }
                switch (CurrentOperate.ResizeType)
                {
                    case ResizeType.All:
                        w = ml;
                        h = ol;
                        this.Height = h + 50;
                        this.Width = w + 50;
                        break;
                    case ResizeType.Height:
                        h = ml;
                        this.Height = h + 50;
                        break;
                    case ResizeType.With:
                        w = ml;
                        this.Width = w + 50;
                        break;
                    case ResizeType.None:
                        break;
                }

                if (CurrentOperate.ResizeType != ResizeType.None)
                {
                    pointOriginInsideNew = CurrentOperate.RelativeBorder.TranslatePoint(new Point(), this);
                    nX = -pointOriginInsideNew.X + pointOriginInside.X;
                    nY = -pointOriginInsideNew.Y + pointOriginInside.Y;
                    pointOriginInside = pointOriginInsideNew;
                }

                if (h > 0 && w > 0)
                {
                    this.SetValue(Canvas.LeftProperty, l + nX);
                    this.SetValue(Canvas.TopProperty, t + nY);
                }

            }
            else isEx = false;
            return isEx;
        }



        private ElementWithPointType CurrentOperate =>
            ElementWithPointTypes.FirstOrDefault(x => x.PointLocationType == CurrentPointType);




        internal void OnContainerMouseDown(object sender, MouseButtonEventArgs e)
        {
            StartMove(e);
        }

        private void StartMove(MouseButtonEventArgs e)
        {
            mouseIsDown = true;
            if (CurrentPointType == PointLocationType.CC)
                pointMoveOld = e.GetPosition(this);
            pointOrigin = CurrentOperate.RelativeBorder.TranslatePoint(new Point(), ifv.canvas);
            pointOriginInside = CurrentOperate.RelativeBorder.TranslatePoint(new Point(), this);
            pointPassiveM = CurrentOperate.AdjacentBorder.TranslatePoint(new Point(), ifv.canvas);


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

        public void MoveWithKey(int size, MoveType moveType)
        {
            var l = Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + (moveType == MoveType.Left ? -size : moveType == MoveType.Right ? size : 0);
            var t = Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + (moveType == MoveType.Top ? -size : moveType == MoveType.Bottom ? size : 0);
            this.SetValue(Canvas.LeftProperty, l);
            this.SetValue(Canvas.TopProperty, t);
        }
        public void RotateWithKey(int size)
        {
            gRT.Angle += size;
        }
        #endregion

        private void BContent_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var pt = ifv.PaintTypeSelects.FirstOrDefault(x => x.PaintType == PaintType.None && x.IsSelected);
            if (pt != null)
            {
                ifv.RectangleViews.ForEach(x => x.Selected = false);
                Selected = true;
            }
        }


    }

    public enum PointLocationType
    {
        LT, LC, LB, CT, CB, RT, RC, RB, NO, CC, Rotate
    }


    public enum ResizeType
    {
        With, Height, All, None
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

    class ElementWithPointType
    {
        public UIElement Element { get; set; }
        public Cursor Cursor { get; set; }
        public PointLocationType PointLocationType { get; set; }
        /// <summary>
        /// 相对的点
        /// </summary>
        public Border RelativeBorder { get; set; }
        /// <summary>
        /// 相邻的点
        /// </summary>
        public Border AdjacentBorder { get; set; }

        public ResizeType ResizeType { get; set; }


    }
}
