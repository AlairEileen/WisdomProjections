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
        public RectangleView(double width, double height)
        {
            InitializeComponent();
            this.Height = height;
            this.Width = width;
        }
        private PointLocationType currentPointType;
        private bool mouseIsDown;
        #region 各个点的鼠标进入事件
        private void BLT_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.LT; this.Cursor = Cursors.SizeNWSE; }

        }

        private void BLB_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.LB; this.Cursor = Cursors.SizeNESW; }

        }

        private void BLC_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.LC; this.Cursor = Cursors.SizeWE; }

        }

        private void BCT_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.CT; this.Cursor = Cursors.SizeNS; }

        }

        private void BCB_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.CB; this.Cursor = Cursors.SizeNS; }
        }

        private void BRT_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.RT; this.Cursor = Cursors.SizeNESW; }

        }

        private void BRC_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.RC; this.Cursor = Cursors.SizeWE; }

        }

        private void BRB_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!mouseIsDown) { currentPointType = PointLocationType.RB; this.Cursor = Cursors.SizeNWSE; }
        }
        #endregion

        #region 父容器鼠标事件

        internal bool OnContainerMouseMove(object sender, MouseEventArgs e)
        {
            bool isEx=true;
            if (mouseIsDown)
            {
                switch (currentPointType)
                {
                    case PointLocationType.LT:
                        ChangeLT(e);
                        break;
                    case PointLocationType.LC:
                        ChangeLC(e);
                        break;
                    case PointLocationType.LB:
                        ChangeLB(e);
                        break;
                    case PointLocationType.CT:
                        break;
                    case PointLocationType.CB:
                        break;
                    case PointLocationType.RT:
                        break;
                    case PointLocationType.RC:
                        break;
                    case PointLocationType.RB:
                        break;
                    default:
                        isEx = false;
                        break;
                }

            }
            else isEx = false;
            return isEx;
        }

        private void ChangeLB(MouseEventArgs e)
        {
            var p = e.GetPosition(gSelect);
            var cy = p.Y - ltdM.Y;
            var cx = p.X - ltdM.X;
            if (this.Height < cy || this.Width < cx)
            {
                return;
            }
            this.Height += cy;
            this.Width -= cx;
            this.SetValue(Canvas.LeftProperty, Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx);

        }

        private void ChangeLC(MouseEventArgs e)
        {
            var p = e.GetPosition(gSelect);
            var cx = p.X - ltdM.X;
            if ( this.Width < cx)
            {
                return;
            }
            this.Width -= cx;
            this.SetValue(Canvas.LeftProperty, Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx);
        }

        Point ltdM;
        private void ChangeLT(MouseEventArgs e)
        {
            var p = e.GetPosition(gSelect);
            var cy = p.Y - ltdM.Y;
            var cx = p.X - ltdM.X;
            if (this.Height < cy || this.Width < cx)
            {
                return;
            }
            this.Height -= cy;
            this.Width -= cx;
            this.SetValue(Canvas.LeftProperty, Convert.ToDouble(this.GetValue(Canvas.LeftProperty)) + cx);
            this.SetValue(Canvas.TopProperty, Convert.ToDouble(this.GetValue(Canvas.TopProperty)) + cy);
        }

        internal void OnContainerMouseDown(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = true;
            ltdM = e.GetPosition(gSelect);
        }

        internal void OnContainerMouseUp(object sender, MouseButtonEventArgs e)
        {
            mouseIsDown = false;
            currentPointType = PointLocationType.NO;
        }

        #endregion

    }

    public enum PointLocationType
    {
        LT, LC, LB, CT, CB, RT, RC, RB,NO
    }
}
