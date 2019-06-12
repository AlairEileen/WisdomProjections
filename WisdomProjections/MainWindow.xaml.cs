using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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

            this.palette = new int[0x1000];

            for (uint d = 0; d < 0x190; d++)
            {
                unchecked
                {
                    this.palette[d] = (int)0xFF000000;
                }
            }

            for (uint d = 0x190; d < 0x400; d++)
            {
                uint r = 0xFF - 0xFF * (d - 0x190) / (0x400 - 0x190);
                uint g = 0;
                uint b = 0xFF;

                this.palette[d] = (int)(b | (g << 8) | (r << 16) | 0xFF000000);
            }

            for (uint d = 0x400; d < 0x600; d++)
            {
                uint r = 0;
                uint g = 0xFF * (d - 0x400) / (0x600 - 0x400);
                uint b = 0xFF;

                this.palette[d] = (int)(b | (g << 8) | (r << 16) | 0xFF000000);
            }

            for (uint d = 0x600; d < 0x800; d++)
            {
                uint r = 0;
                uint g = 0xFF;
                uint b = 0xFF - 0xFF * (d - 0x600) / (0x800 - 0x600);

                this.palette[d] = (int)(b | (g << 8) | (r << 16) | 0xFF000000);
            }

            for (uint d = 0x800; d < 0xC00; d++)
            {
                uint r = 0xFF * (d - 0x800) / (0xC00 - 0x800);
                uint g = 0xFF;
                uint b = 0;

                this.palette[d] = (int)(b | (g << 8) | (r << 16) | 0xFF000000);
            }

            for (uint d = 0xC00; d < 0x1000; d++)
            {
                uint r = 0xFF;
                uint g = 0xFF - 0xFF * (d - 0xC00) / (0x1000 - 0xC00);
                uint b = 0;

                this.palette[d] = (int)(b | (g << 8) | (r << 16) | 0xFF000000);
            }
        }




        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < MaxPersons * MaxJoints; i++)
            {
                var joint = new System.Windows.Shapes.Ellipse();
                joint.Fill = System.Windows.Media.Brushes.Red;
                joint.Width = 16.0;
                joint.Height = 16.0;
                joint.Visibility = System.Windows.Visibility.Collapsed;

                this.canvas.Children.Add(joint);
            }

            for (var i = 0; i < MaxPersons * MaxBones; i++)
            {
                var bone = new Line();
                bone.Stroke = System.Windows.Media.Brushes.Blue;
                bone.StrokeThickness = 6.0;
                bone.Visibility = System.Windows.Visibility.Collapsed;

                this.canvas.Children.Add(bone);
            }

            this.sensor = new Sensor();

            if (ErrorCode.Success != this.sensor.Initialize(ColorResolution.VGA, DepthResolution.VGA, true, 0))
            {
                MessageBox.Show("Failed to initialize sensor", "PeopleSensorAppWpf");
                this.Close();
            }

            if (ErrorCode.Success != this.sensor.Start())
            {
                MessageBox.Show("Failed to start sensor", "PeopleSensorAppWpf");
                this.Close();
            }

            this.sensor.ColorFrameReady += this.OnColorFrameReady;
            this.sensor.DepthFrameAndPublishDataReady += this.OnDepthFrameAndPublishDataReady;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.sensor.ColorFrameReady -= this.OnColorFrameReady;
            this.sensor.DepthFrameAndPublishDataReady -= this.OnDepthFrameAndPublishDataReady;

            this.sensor.Stop();
            this.sensor.Uninitialize();
            this.sensor.Dispose();
        }

        private void OnColorFrameReady(Sensor sensor, ColorFrame frame, ErrorCode error)
        {
            if (ErrorCode.Success != error)
            {
                return;
            }

            if (null == this.cframe)
            {
                this.cframe = new ColorFrame();
            }

            lock (this.cframe)
            {
                frame.CopyTo(this.cframe);
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (this.cframe)
                {
                    if (null == this.cbitmap || this.cbitmap.PixelWidth != this.cframe.Width || this.cbitmap.PixelHeight != this.cframe.Height)
                    {
                        this.cbitmap = new WriteableBitmap(this.cframe.Width, this.cframe.Height, 96, 96, PixelFormats.Bgra32, null);
                        this.cimage.Source = this.cbitmap;
                    }

                    this.cbitmap.WritePixels(new Int32Rect(0, 0, this.cframe.Width, this.cframe.Height), this.cframe.Pixels, this.cframe.Width * 4, 0);
                }
            }), DispatcherPriority.Render);
        }

        private void OnDepthFrameAndPublishDataReady(Sensor sensor, DepthFrame depthFrame, PublishData publishData, ErrorCode error)
        {




            if (ErrorCode.Success != error)
            {
                return;
            }
            if (null == this.dpixels || this.dpixels.Length != depthFrame.Pixels.Length)
            {
                this.dpixels = new int[depthFrame.Pixels.Length];
            }


            ///create by alair end
            //TestPointCloud(sensor,depthFrame,);

            ///create by alair end

            lock (this.dpixels)
            {
                this.dw = depthFrame.Width;
                this.dh = depthFrame.Height;
                //Console.Write("深度返回的点");
                for (var i = 0; i < depthFrame.Pixels.Length; i++)
                {
                    var d = depthFrame.Pixels[i];

                    if (d >= 0x1000)
                    {
                        unchecked
                        {
                            this.dpixels[i] = (int)0xFF000000;
                        }
                    }
                    else
                    {
                        this.dpixels[i] = this.palette[d];
                    }
                    //Console.Write(d+",");
                }
                //Console.WriteLine();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    lock (this.dpixels)
                    {
                        if (null == this.dbitmap || this.dbitmap.PixelHeight != this.dw || this.dbitmap.PixelHeight != this.dh)
                        {
                            this.dbitmap = new WriteableBitmap(this.dw, this.dh, 96, 96, PixelFormats.Bgra32, null);
                            this.dimage.Source = this.dbitmap;
                        }

                        this.dbitmap.WritePixels(new Int32Rect(0, 0, this.dw, this.dh), this.dpixels, this.dw * 4, 0);
                        //this.dbitmap.WritePixels(new Int32Rect(0, 0, this.dw, this.dh), this.dpixels, this.dbitmap.BackBufferStride, 0);
                    }
                }), DispatcherPriority.Render);
            }

            if (null == this.pubData)
            {
                this.pubData = new PublishData();
            }

            lock (this.pubData)
            {
                publishData.CopyTo(this.pubData);
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (this.pubData)
                {
                    this.DrawSkeletons(this.pubData);
                }
            }), DispatcherPriority.Render);
        }

        private void TestPointCloud(Sensor sensor)
        {
           //sensor.ConvertToPointCloud()
        }

        private void DrawSkeletons(PublishData pub)
        {
            int jIndex = 0;
            int bIndex = MaxPersons * MaxJoints;
            float scaleX = (float)this.canvas.ActualWidth / pub.UserMask.Width;
            float scaleY = (float)this.canvas.ActualHeight / pub.UserMask.Height;

            for (var i = 0; i < pub.Skeletons.Length; i++)
            {
                this.DrawSkeleton(pub.Skeletons[i], ref jIndex, ref bIndex, scaleX, scaleY);
            }

            for (var i = jIndex; i < MaxPersons * MaxJoints; i++)
            {
                this.canvas.Children[i].Visibility = System.Windows.Visibility.Collapsed;
            }

            for (var i = bIndex; i < this.canvas.Children.Count; i++)
            {
                this.canvas.Children[i].Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        private void DrawSkeleton(Skeleton skeleton, ref int jointIndex, ref int boneIndex, float scaleX, float scaleY)
        {
            foreach (var joint in skeleton.Joints)
            {
                this.DrawJoint(joint.Position, ref jointIndex, scaleX, scaleY);
            }

            this.DrawBone(skeleton.Joints[0].Position, skeleton.Joints[1].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[1].Position, skeleton.Joints[2].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[1].Position, skeleton.Joints[3].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[2].Position, skeleton.Joints[4].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[3].Position, skeleton.Joints[5].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[4].Position, skeleton.Joints[6].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[5].Position, skeleton.Joints[7].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[2].Position, skeleton.Joints[11].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[3].Position, skeleton.Joints[10].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[10].Position, skeleton.Joints[12].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[11].Position, skeleton.Joints[13].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[12].Position, skeleton.Joints[14].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[13].Position, skeleton.Joints[15].Position, ref boneIndex, scaleX, scaleY);
            this.DrawBone(skeleton.Joints[10].Position, skeleton.Joints[11].Position, ref boneIndex, scaleX, scaleY);
        }

        private void DrawJoint(Float4 position, ref int index, float scaleX, float scaleY)
        {
            if (float.IsNaN(position.X) || float.IsNaN(position.Y) || float.IsNaN(position.Z))
            {
                return;
            }

            if (0 == position.Z)
            {
                return;
            }

            var p = this.Project(position, scaleX, scaleY);
            var e = (System.Windows.Shapes.Ellipse)this.canvas.Children[index++];

            Canvas.SetLeft(e, p.X - e.Width * 0.5);
            Canvas.SetTop(e, p.Y - e.Width * 0.5);
            e.Visibility = System.Windows.Visibility.Visible;
        }

        private void DrawBone(Float4 p0, Float4 p1, ref int index, float scaleX, float scaleY)
        {
            if (float.IsNaN(p0.X) || float.IsNaN(p0.Y) || float.IsNaN(p0.Z) ||
                float.IsNaN(p1.X) || float.IsNaN(p1.Y) || float.IsNaN(p1.Z))
            {
                return;
            }

            if (0.0f == p0.Z || 0.0f == p1.Z)
            {
                // TODO: investigate why z = 0
                return;
            }

            var pt0 = this.Project(p0, scaleX, scaleY);
            var pt1 = this.Project(p1, scaleX, scaleY);

            var l = (Line)this.canvas.Children[index++];
            l.X1 = pt0.X;
            l.Y1 = pt0.Y;
            l.X2 = pt1.X;
            l.Y2 = pt1.Y;
            l.Visibility = System.Windows.Visibility.Visible;
        }

        private System.Windows.Point Project(Float4 position, float scaleX, float scaleY)
        {
            Float2 point2D = new Float2();
            this.sensor.DepthSpacePointToScreen(position, ref point2D);

            var p =new System.Windows.Point();
            p.X = point2D.X * scaleX;
            p.Y = point2D.Y * scaleY;

            return p;
        }

        private Sensor sensor;

        private ColorFrame cframe;
        private WriteableBitmap cbitmap;

        private int dw, dh;
        private int[] dpixels;
        private int[] palette;
        private WriteableBitmap dbitmap;

        private PublishData pubData;
        private IOutputArray contours;
        private const int MaxPersons = 6;
        private const int MaxJoints = 17;
        private const int MaxBones = 14;

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

        //private bool FloodFillTOcanny()
        //{


        //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //    BmpBitmapEncoder encoder = new BmpBitmapEncoder();
        //    encoder.Frames.Add(BitmapFrame.Create((BitmapSource)this.cimage.Source));
        //    encoder.Save(ms);

        //    Bitmap bp = new Bitmap(ms);

        //    Image<Bgr, Byte> srcimg = new Image<Bgr, Byte>(bp);
        //    //转成灰度图
        //    Image<Gray, Byte> grayimg = srcimg.Convert<Gray, Byte>();
        //    // CvInvoke.BitwiseNot(grayimg, grayimg);
        //    //Canny 边缘检测
        //    Image<Gray, Byte> cannyGrayimg = grayimg.Canny((int)numericUD_FloodFill.Value, (int)numericUD_FloodFill.Value);
        //    Gray bkGrayWhite = new Gray(255);
        //    Emgu.CV.IOutputArray hierarchy = new Image<Gray, Byte>(srcimg.Width, srcimg.Height, bkGrayWhite);
        //    Image<Rgb, Byte> imgresult = new Image<Rgb, Byte>(srcimg.Width, srcimg.Height, new Rgb(255, 255, 255));

        //    CvInvoke.FindContours(cannyGrayimg,
        //                            contours,
        //                            (Emgu.CV.IOutputArray)hierarchy,
        //                            Emgu.CV.CvEnum.RetrType.Ccomp,
        //                            Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone//保存为
        //                             );
        //    GraphicsPath myGraphicsPath = new GraphicsPath();
        //    Region myRegion = new Region();
        //    myGraphicsPath.Reset();
        //    contours.GetOutputArray().GetSize();
        //    int areaMax = 0, idx = 0;
        //    for (int ii = 0; ii < contours.Size; ii++)
        //    {
        //        int area = (int)CvInvoke.ContourArea(contours[ii]);
        //        if (area > areaMax) { areaMax = area; idx = ii; }
        //        if (area < 1) continue;
        //        CvInvoke.DrawContours(imgresult, contours.GetInputArray(), ii, new MCvScalar(0, 0, 0), 1, Emgu.CV.CvEnum.LineType.EightConnected, (Emgu.CV.IInputArray)null, 2147483647);
        //        imageBox1.Image = imgresult;
        //        try
        //        {
        //            myGraphicsPath.AddPolygon(contours[ii].ToArray());
        //        }
        //        catch
        //        {
        //            //MessageBox.Show(e.Message);
        //        }
        //    }

        //    myRegion.MakeEmpty();
        //    myRegion.Union(myGraphicsPath);

        //    pictureBox1.Refresh();
        //    System.Drawing.Pen pen = new System.Drawing.Pen(System.Drawing.Color.Red, 1);
        //    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        //    Graphics gs = pictureBox1.CreateGraphics();
        //    gs.DrawPath(pen, myGraphicsPath);
        //    if (myRegion.IsVisible(lastPoint))
        //    {
        //        //gs.DrawPolygon(pen, respts);
        //    }
        //    else
        //    {
        //        //gs.DrawRectangle(pen, new Rectangle(0,0,pictureBox1.Image.Width, pictureBox1.Image.Height));
        //    }
        //    gs.Dispose();
        //    return true;
        //}


        /// 
        ///end

    }
}
