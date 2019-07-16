using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using WisdomProjections.Views.Sys;
using Color = System.Drawing.Color;
using Pen = System.Drawing.Pen;
using Rect = System.Windows.Rect;
using VectorOfPoint = Emgu.CV.Util.VectorOfPoint;

namespace WisdomProjections.Data_Executor
{
    public class ImageSelectDataExecutor
    {

        //private bool FloodFillTOcanny(Bitmap image,int floodValue,Action<Bitmap> refreshImage,DrawingVisual imagePreview )
        //{
        //    Image<Bgr, Byte> srcimg = new Image<Bgr, Byte>((Bitmap)image);
        //    //转成灰度图
        //    Image<Gray, Byte> grayimg = srcimg.Convert<Gray, Byte>();
        //    // CvInvoke.BitwiseNot(grayimg, grayimg);
        //    //Canny 边缘检测
        //    Image<Gray, Byte> cannyGrayimg = grayimg.Canny((int)floodValue, (int)floodValue);
        //    Gray bkGrayWhite = new Gray(255);
        //    Emgu.CV.IOutputArray hierarchy = new Image<Gray, Byte>(srcimg.Width, srcimg.Height, bkGrayWhite);
        //    Image<Rgb, Byte> imgresult = new Image<Rgb, Byte>(srcimg.Width, srcimg.Height, new Rgb(255, 255, 255));
        //    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
        //    CvInvoke.FindContours(cannyGrayimg,
        //                            contours,
        //                            (Emgu.CV.IOutputArray)hierarchy,
        //                            Emgu.CV.CvEnum.RetrType.Ccomp,
        //                            Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone//保存为
        //                             );
        //    GraphicsPath myGraphicsPath = new GraphicsPath();
        //    Region myRegion = new Region();
        //    myGraphicsPath.Reset();

        //    int areaMax = 0, idx = 0;
        //    for (int contourIdx = 0; contourIdx < contours.Size; contourIdx++)
        //    {
        //        int area = (int)CvInvoke.ContourArea(contours[contourIdx]);
        //        if (area > areaMax) { areaMax = area; idx = contourIdx; }
        //        if (area < 1) continue;
        //        CvInvoke.DrawContours(imgresult, contours, contourIdx, new MCvScalar(0, 0, 0), 1, Emgu.CV.CvEnum.LineType.EightConnected, (Emgu.CV.IInputArray)null, 2147483647);
        //        try
        //        {
        //            myGraphicsPath.AddPolygon(contours[contourIdx].ToArray());
        //        }
        //        catch
        //        {
        //            //MessageBox.Show(e.Message);
        //        }
        //    }

        //    myRegion.MakeEmpty();
        //    myRegion.Union(myGraphicsPath);

        //    Pen pen = new Pen(Color.Red, 1) { DashStyle = DashStyle.Dot };

        //    using (var dc=imagePreview.RenderOpen())
        //    {
        //        dc.DrawRectangle();
        //    }

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


        #region Draw2 实时监测图像边缘
        public void StopDraw2()
        {
            currentImage?.Dispose();
            contours?.Dispose();
            imageFound?.Dispose();
            edges?.Dispose();
            hierarchyMat?.Dispose();
            imageFlipHorizontal?.Dispose();

        }

        private Image<Gray, byte> currentImage;
        private VectorOfVectorOfPoint contours;
        private Image<Bgra, byte> imageFound;
        private Image<Gray, byte> edges;
        private Emgu.CV.Mat hierarchyMat;
        private Image<Bgra, byte> imageFlipHorizontal;
        private WriteableBitmap wBitmap;


        public void Draw2(double with, double height, System.Drawing.Bitmap imageBitmap, Image imagePreview, Canvas canvas)
        {
            using (imageBitmap)
            {

                //ImageSource imageS;
                currentImage = new Image<Gray, byte>(imageBitmap);
                //currentImage = new Image<Gray, byte>(@"C:\Data\Documents\Dev\WeChat Image_20190708104044.jpg");

                //var cc =currentImage[currentImage.Rows / 2, currentImage.Cols / 2];
                //var cc2 =currentImage[(currentImage.Rows / 2)+10, (currentImage.Cols / 2)+10];

                //var result = currentImage.InRange(cc, cc);

                //CvInvoke.Imshow("color-gray",result);

                //var t = new Thread(new ThreadStart(() =>
                //{
                //    var str = "";
                //    for (int i = 0; i < currentImage.Rows; i++)
                //    {
                //        for (int j = 0; j < currentImage.Cols; j++)
                //        {
                //            str += $"{i},{j},{currentImage[i, j].Dimension},{currentImage[i, j].Intensity}";
                //        }
                //        str += "\r\n";
                //    }
                //    File.WriteAllText(@"C:\Data\Desktop\currentImage.txt", str);
                //}));
                //t.IsBackground = true;
                //t.Start();

                //currentImage = new Image<Gray, byte>();

                contours = new VectorOfVectorOfPoint();
                imageFound = new Image<Bgra, byte>(currentImage.Width, currentImage.Height, new Bgra(255, 255, 255, 0));

                edges = new Image<Gray, byte>(currentImage.Width, currentImage.Height);
                //var edges2 = new Image<Gray, byte>(currentImage.Width, currentImage.Height);

                //var im = new Image<Bgra, byte>(imageBitmap);
                //im.Data[im.Cols/

                hierarchyMat = new Emgu.CV.Mat();

                //CvInvoke.CvtColor(currentImage, edges, ColorConversion.BayerBg2Gray);
                //CvInvoke.Threshold(edges, edges2, 80, 100, ThresholdType.Binary);
                CvInvoke.Canny(currentImage, edges, 20, 60);
                //CvInvoke.Watershed(currentImage,edges);
                //CvInvoke.GrabCut(currentImage,edges);

                CvInvoke.FindContours(edges,contours,hierarchyMat,RetrType.External,ChainApproxMethod.ChainApproxSimple);
                //int[,] hierachy = CvInvoke.FindContourTree(edges, contours, ChainApproxMethod.ChainApproxSimple);
                //Rectangle rectMax = Rectangle.Empty;


                //double area0 = 0;
                //double area01 = 0;
                //int index = 0;
                for (int i = 0; i < contours.Size; i++)
                {
                    //var area = CvInvoke.ArcLength(contours[i], false);
                    //var area2 = CvInvoke.ContourArea(contours[i]);
                    //if (area0 < area && area01 < area2)
                    //{
                    //    area0 = area;
                    //    area01 = area2;
                    //    index = i;
                    //}
                    //var img = new Image<Bgra, byte>(currentImage.Width, currentImage.Height, new Bgra(255, 255, 255, 0));
                    //CvInvoke.DrawContours(img, contours, i, new MCvScalar(255, 0, 0, 255), 1);
                    //CvInvoke.Imshow("img" + i, img);


                    CvInvoke.DrawContours(imageFound, contours, i, new MCvScalar(255, 0, 0, 255), 1);
                }

                //CvInvoke.DrawContours(imageFound, contours, index, new MCvScalar(255, 0, 0, 255), 1);

                //CvInvoke.Imshow("手机拍摄照片",imageFound);
                var scaleNum = with / imageFound.Width;

                //using (var ro=drawingVisual.RenderOpen())
                //{
                //    ro.DrawRectangle(new SolidColorBrush(Colors.Transparent), new System.Windows.Media.Pen(new SolidColorBrush(Colors.Red), 1), new Rect(rectMax.X*scaleNum,rectMax.Y * scaleNum, rectMax.Width * scaleNum, rectMax.Height * scaleNum) );
                //}


                imageFlipHorizontal = imageFound.Resize(scaleNum, Inter.Area).Flip(FlipType.Horizontal);
                //bitmapFound = imageFlipHorizontal.ToBitmap();


                //CvInvoke.ConvexHull()
                if (wBitmap == null)
                {
                    wBitmap = new WriteableBitmap(imageFlipHorizontal.Width, imageFlipHorizontal.Height, 96, 96, PixelFormats.Bgra32, null);
                    imagePreview.Source = wBitmap;
                }
                wBitmap.WritePixels(new Int32Rect(0, 0, imageFlipHorizontal.Width, imageFlipHorizontal.Height), imageFlipHorizontal.Bytes, imageFlipHorizontal.Width * 4, 0);



                //GC.Collect();
                //GC.WaitForPendingFinalizers();
                //GC.Collect();

            }
        }

        public void Draw1(double with, double height, System.Drawing.Bitmap imageBitmap,
            System.Windows.Controls.Image imagePreview, System.Windows.Controls.Canvas canvas)
        {
            int smallarea=250;
            //Mat frame = new Image<Bgr, byte>(@"C:\Data\Documents\Dev\WeChat Image_20190703112224.jpg").Mat;
            Mat frame = new Image<Bgr, byte>(imageBitmap).Mat;

            Mat grayimg = new Mat(), thresholdimg = new Mat(), canny = new Mat();
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            double gnthresh = 40;
            int idx, idy = 0;
            String text1 = null, text2 = null, text3 = null;

           var box = new System.Drawing.Rectangle();
            CvInvoke.CvtColor(frame, grayimg, ColorConversion.Bgr2Gray);
            CvInvoke.Blur(grayimg, grayimg, new System.Drawing.Size(3, 3), new System.Drawing.Point(-1, -1));
            CvInvoke.Threshold(grayimg, thresholdimg, gnthresh, 255, ThresholdType.Binary);
            //CvInvoke.Dilate(thresholdimg, thresholdimg, null, new Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);//膨胀扩张
            CvInvoke.Erode(thresholdimg, thresholdimg, null, new System.Drawing.Point(-1, -1), 1, BorderType.Constant, CvInvoke.MorphologyDefaultBorderValue);//腐蚀，先膨胀后腐蚀是闭运算，消除小黑洞
            CvInvoke.Canny(thresholdimg, canny, 100, 50);//最大幅值100，最小幅值50，大于100，则保留；小于50，排除；两者之间且连着大于100的点才保留
            //Mat mask1 = new Mat(thresholdimg.Size.Height,thresholdimg.Size.Width,DepthType.Cv8U,1);
            //mask1.SetTo(new MCvScalar(255));
            //canny.CopyTo(mask1);
            int[,] hierachy = CvInvoke.FindContourTree(canny, contours, ChainApproxMethod.ChainApproxSimple);//最后一个参数表示一个矩形只要4个点的参数   
            //int indexshow = 0;
            for (int i = 0; i < contours.Size; i++)
            {
                var img = new Image<Bgra, byte>(frame.Width, frame.Height, new Bgra(255, 255, 255, 0));
                CvInvoke.DrawContours(img, contours, i, new MCvScalar(255, 0, 0, 255), 1);
                CvInvoke.Imshow("img" + i, img);
            }

            if (contours.Size == 0) return;
            for (; idy >= 0 && text1 == null; idy = hierachy[idy, 0])
                for (idx = idy; idx >= 0; idx = hierachy[idx, 2])//hierachy[idx, 0]表示后一个轮廓索引编号    [idx,2]表示内部轮廓
                {
                    using (VectorOfPoint c = contours[idx])
                    using (VectorOfPoint approx = new VectorOfPoint())
                    {
                        
                        CvInvoke.ApproxPolyDP(c, approx, CvInvoke.ArcLength(c, true) * 0.001, true);//逼近多边形，ApproxPolyDP（，，精度为周长的0.02倍，要封闭的）
                        double area = CvInvoke.ContourArea(approx);
                        /*Point[] pts = approx.ToArray();
                                for (int j = 0; j < pts.Length; j++)
                                {
                                    Point p1 = new Point(pts[j].X, pts[j].Y);
                                    Point p2;
                                    if (j == pts.Length - 1)
                                        p2 = new Point(pts[0].X, pts[0].Y);
                                    else
                                        p2 = new Point(pts[j + 1].X, pts[j + 1].Y);
                                    CvInvoke.Line(frame, p1, p2, new MCvScalar(255, 255, 0), 3);
                                }*/
                        if ( area > smallarea)
                        {
                            box = CvInvoke.BoundingRectangle(c);//返回外部矩形边界                                 
                            Mat candidate = new Mat();
                            using (Mat tmp = new Mat(frame, box))  //roi
                            {
                                CvInvoke.CvtColor(tmp, candidate, ColorConversion.Bgr2Gray);
                            }
                            using (Mat mask = new Mat(candidate.Size.Height, candidate.Width, DepthType.Cv8U, 1))  //set the value of pixels not in the contour region to zero  设置像素值不在轮廓区域为零
                            {
                                mask.SetTo(new MCvScalar(255));//白色图像
                                CvInvoke.DrawContours(mask, contours, idx, new MCvScalar(0), -1, LineType.EightConnected, null, int.MaxValue, new System.Drawing.Point(-box.X, -box.Y));//黑色轮廓
                                //CvInvoke.Imshow($"x:{indexshow++}", mask);
                                CvInvoke.Rectangle(frame, box, new MCvScalar(0, 0, 255), 2);
                                text1 = Convert.ToString((2 * box.X + box.Width) / 2);//横向坐标
                                text2 = Convert.ToString(box.Y);//纵向坐标 
                                text3 = Convert.ToString(area);
                            }

                            break;
                        }
                    }

                }
            if (text1 != null && text2 != null)
            {
                CvInvoke.PutText(frame, text1, new System.Drawing.Point(0, 40), FontFace.HersheyTriplex, 1, new MCvScalar(255, 255, 255), 2);
                CvInvoke.PutText(frame, text2, new  System.Drawing.Point(160, 40), FontFace.HersheyTriplex, 1, new MCvScalar(255, 255, 255), 2);
                CvInvoke.PutText(frame, text3, new  System.Drawing.Point(320, 40), FontFace.HersheyTriplex, 1, new MCvScalar(255, 255, 255), 2);
            }
            //CvInvoke.Imshow("frame",new Image<Bgra,byte>(box));
        }





        #endregion








        //public void Draw3(double with, double height, Bitmap imageBitmap, System.Windows.Controls.Image imagePreview, Canvas canvas)
        //{

        //    Mat mat1 = imageBitmap.ToMat();

        //    //Mat mat1 = new Mat(@"C:\Data\Documents\Dev\WeChat Image_20190703112224.jpg", ImreadModes.AnyColor);
        //    Mat mat2 = new Mat();
        //    Mat mat3 = new Mat();
        //    Cv2.CvtColor(mat1, mat3, ColorConversionCodes.BGR2GRAY, 0);
        //    Cv2.Threshold(mat3, mat2, 50, 100, ThresholdTypes.Binary);
        //    Cv2.Canny(mat2, mat3, 30, 60);

        //    Mat mat = mat1.EmptyClone();
        //    //for (int i = 0; i < mat.Rows; ++i)
        //    //{
        //    //    for (int j = 0; j < mat.Cols; ++j)
        //    //    {
        //    //        Vec4b rgba = mat.At<Vec4b>(i, j);
        //    //        rgba[0] = 0;
        //    //        rgba[1] = 0;
        //    //        rgba[2] = 0;
        //    //        rgba[3] = 0;
        //    //    }
        //    //}


        //    Point[][] contours;
        //    HierarchyIndex[] hierarchy;
        //    Cv2.FindContours(mat3, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

        //    for (int i = 0; i < hierarchy.Length; i++)
        //    {
        //        Cv2.DrawContours(mat, contours, i, Scalar.Red, 1, LineTypes.Link8, hierarchy, 4, new Point());
        //    }
        //    Mat matNew = mat.Resize(new OpenCvSharp.Size(with, height));

        //    //App.Current.Dispatcher.BeginInvoke(new Action(() =>
        //    //{
        //    //    imagePreview.Source = mat.ToBitmapSource();
        //    //}), DispatcherPriority.Render);

        //    if (wBitmap == null)
        //    {
        //        wBitmap = new WriteableBitmap(matNew.Width, matNew.Height, 96, 96, PixelFormats.Bgra32, null);
        //        imagePreview.Source = wBitmap;
        //    }

        //    var w = matNew.Width * 4;


        //    wBitmap.WritePixels(new Int32Rect(0, 0, matNew.Width, matNew.Height), mat.Data, w, w);

        //}
    }
}



