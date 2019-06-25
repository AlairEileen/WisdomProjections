using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WisdomProjections.Views;
using YDPeopleSensor.Net;

namespace WisdomProjections.Data_Executor
{
    public class SensorDataExecutor
    {
        private SensorDataExecutor()
        {

        }

        private bool isInitedContent;

        internal void InitContent(ImageFactoryView imgContainer)
        {
            this.imgContainer = imgContainer;
            InitDImageColor();

            for (var i = 0; i < MaxPersons * MaxJoints; i++)
            {
                var joint = new System.Windows.Shapes.Ellipse();
                joint.Fill = System.Windows.Media.Brushes.Red;
                joint.Width = 16.0;
                joint.Height = 16.0;
                joint.Visibility = System.Windows.Visibility.Collapsed;

                imgContainer.canvas.Children.Add(joint);
            }

            for (var i = 0; i < MaxPersons * MaxBones; i++)
            {
                var bone = new Line();
                bone.Stroke = System.Windows.Media.Brushes.Blue;
                bone.StrokeThickness = 6.0;
                bone.Visibility = System.Windows.Visibility.Collapsed;

                imgContainer.canvas.Children.Add(bone);
            }
            isInitedContent = true;
        }

        public enum InitStatus
        {
            初始化失败, 开启失败
        }
        public void InitSensorDE(Action<InitStatus> action)
        {
            this.sensor = new Sensor();

            if (ErrorCode.Success != this.sensor.Initialize(ColorResolution.VGA, DepthResolution.VGA, true, 0))
            {
                action(InitStatus.初始化失败);
                return;
            }
            this.sensor.SetDepthMappedToColor(true);

            if (ErrorCode.Success != this.sensor.Start())
            {
                action(InitStatus.开启失败);
                return;
            }

            this.sensor.ColorFrameReady += this.OnColorFrameReady;
            this.sensor.DepthFrameAndPublishDataReady += this.OnDepthFrameAndPublishDataReady;
        }




        public void Close(object sender, CancelEventArgs e)
        {
            this.sensor.ColorFrameReady -= this.OnColorFrameReady;
            this.sensor.DepthFrameAndPublishDataReady -= this.OnDepthFrameAndPublishDataReady;
            this.sensor.Stop();
            this.sensor.Uninitialize();
            this.sensor.Dispose();
        }
        private void InitDImageColor()
        {


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
        private void OnColorFrameReady(Sensor sensor, ColorFrame frame, ErrorCode error)
        {
            if (!isInitedContent)
            {
                return;
            }
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

            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (this.cframe)
                {
                    if (null == this.cbitmap || this.cbitmap.PixelWidth != this.cframe.Width || this.cbitmap.PixelHeight != this.cframe.Height)
                    {
                        this.cbitmap = new WriteableBitmap(this.cframe.Width, this.cframe.Height, 96, 96, PixelFormats.Bgra32, null);
                        imgContainer.img.Source =this.cbitmap;
                    }
                    this.cbitmap.WritePixels(new Int32Rect(0, 0, this.cframe.Width, this.cframe.Height), this.cframe.Pixels, this.cframe.Width * 4, 0);


                }
            }), DispatcherPriority.Render);
        }



        private void OnDepthFrameAndPublishDataReady(Sensor sensor, DepthFrame depthFrame, PublishData publishData, ErrorCode error)
        {

            if (!showDpImg)
            {
                return;
            }
            if (!isInitedContent)
            {
                return;
            }

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

                App.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    lock (this.dpixels)
                    {
                        if (null == this.dbitmap || this.dbitmap.PixelHeight != this.dw || this.dbitmap.PixelHeight != this.dh)
                        {
                            this.dbitmap = new WriteableBitmap(this.dw, this.dh, 96, 96, PixelFormats.Bgra32, null);
                            imgContainer.img.Source = this.dbitmap;
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

            App.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                lock (this.pubData)
                {
                    this.DrawSkeletons(this.pubData);
                }
            }), DispatcherPriority.Render);
        }



        private void DrawSkeletons(PublishData pub)
        {
            int jIndex = 0;
            int bIndex = MaxPersons * MaxJoints;
            float scaleX = (float)imgContainer.canvas.ActualWidth / pub.UserMask.Width;
            float scaleY = (float)imgContainer.canvas.ActualHeight / pub.UserMask.Height;

            for (var i = 0; i < pub.Skeletons.Length; i++)
            {
                this.DrawSkeleton(pub.Skeletons[i], ref jIndex, ref bIndex, scaleX, scaleY);
            }

            for (var i = jIndex; i < MaxPersons * MaxJoints; i++)
            {
                imgContainer.canvas.Children[i].Visibility = System.Windows.Visibility.Collapsed;
            }

            for (var i = bIndex; i < imgContainer.canvas.Children.Count; i++)
            {
                imgContainer.canvas.Children[i].Visibility = System.Windows.Visibility.Collapsed;
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
            var e = (System.Windows.Shapes.Ellipse)imgContainer.canvas.Children[index++];

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

            var l = (Line)imgContainer.canvas.Children[index++];
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

            var p = new System.Windows.Point();
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

        private bool showDpImg = false;

        private PublishData pubData;
        private ImageFactoryView imgContainer;
        private const int MaxPersons = 6;
        private const int MaxJoints = 17;
        private const int MaxBones = 14;
        private static SensorDataExecutor _sensorDE;

        public static SensorDataExecutor SensorDE
        {
            get
            {
                if (_sensorDE == null) _sensorDE = new SensorDataExecutor();

                return _sensorDE;
            }
        }
    }
}
