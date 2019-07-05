using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using WisdomProjections.Views.Sys;
using Color = System.Drawing.Color;
using Mat = OpenCvSharp.Mat;
using Pen = System.Drawing.Pen;
using Point = OpenCvSharp.Point;
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


        public void Draw2(double with, double height, Bitmap imageBitmap, System.Windows.Controls.Image imagePreview)
        {
            using (imageBitmap)
            {
                //ImageSource imageS;
                currentImage = new Image<Gray, byte>(imageBitmap);


                var t = new Thread(new ThreadStart(() =>
                {
                    var str = "";
                    for (int i = 0; i < currentImage.Rows; i++)
                    {
                        for (int j = 0; j < currentImage.Cols; j++)
                        {
                            str += $"{i},{j},{currentImage[i, j].Dimension},{currentImage[i, j].Intensity}";
                        }
                        str += "\r\n";
                    }
                    File.WriteAllText(@"C:\Data\Desktop\currentImage.txt", str);
                }));
                t.IsBackground = true;
                t.Start();

                //currentImage = new Image<Gray, byte>();

                contours = new VectorOfVectorOfPoint();
                imageFound = new Image<Bgra, byte>(currentImage.Width, currentImage.Height, new Bgra(255, 255, 255, 0));

                edges = new Image<Gray, byte>(currentImage.Width, currentImage.Height);
                var edges2 = new Image<Gray, byte>(currentImage.Width, currentImage.Height);

                //var im = new Image<Bgra, byte>(imageBitmap);
                //im.Data[im.Cols/

                hierarchyMat = new Emgu.CV.Mat();

                CvInvoke.CvtColor(currentImage, edges, ColorConversion.BayerBg2Gray);
                CvInvoke.Threshold(edges, edges2, 80, 100, ThresholdType.Binary);
                //CvInvoke.Canny(currentImage, edges, 420, 500);
                CvInvoke.FindContours(edges2, contours, hierarchyMat, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                Rectangle rectMax = Rectangle.Empty;

                double len = 0;
                int index = 0;
                for (int i = 0; i < contours.Size; i++)
                {
                    //var rect = CvInvoke.BoundingRectangle(contours[i]);
                    //var l = rect.Width * rect.Height;
                    //if (len < l)
                    //{


                    //    len = l;
                    //    index = i;
                    //}


                    CvInvoke.DrawContours(imageFound, contours, i, new MCvScalar(255, 0, 0, 255), 1);

                    //VectorOfPoint contour = contours[i];
                    //Rectangle rect = CvInvoke.BoundingRectangle(contour);
                    //if (rectMax==Rectangle.Empty)
                    //{
                    //    rectMax = rect;
                    //}
                    //else if (rectMax.Height*rectMax.Width<rect.Width*rect.Height)
                    //{
                    //    rectMax = rect;
                    //}
                }

                //CvInvoke.DrawContours(imageFound, contours, index, new MCvScalar(255, 0, 0, 255), 1);
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



        #endregion


        public void Draw3(double with, double height, Bitmap imageBitmap, System.Windows.Controls.Image imagePreview)
        {

            Mat mat1 = imageBitmap.ToMat();

            //Mat mat1 = new Mat(@"C:\Data\Documents\Dev\WeChat Image_20190703112224.jpg", ImreadModes.AnyColor);
            Mat mat2 = new Mat();
            Mat mat3 = new Mat();
            Cv2.CvtColor(mat1, mat3, ColorConversionCodes.BGR2GRAY, 0);
            Cv2.Threshold(mat3, mat2, 50, 100, ThresholdTypes.Binary);
            Cv2.Canny(mat2, mat3, 30, 60);

            Mat mat = mat1.EmptyClone();
            //for (int i = 0; i < mat.Rows; ++i)
            //{
            //    for (int j = 0; j < mat.Cols; ++j)
            //    {
            //        Vec4b rgba = mat.At<Vec4b>(i, j);
            //        rgba[0] = 0;
            //        rgba[1] = 0;
            //        rgba[2] = 0;
            //        rgba[3] = 0;
            //    }
            //}


            Point[][] contours;
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(mat3, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

            for (int i = 0; i < hierarchy.Length; i++)
            {
                Cv2.DrawContours(mat, contours, i, Scalar.Red, 1, LineTypes.Link8, hierarchy, 4, new Point());
            }
            Mat matNew = mat.Resize(new OpenCvSharp.Size(with, height));

            //App.Current.Dispatcher.BeginInvoke(new Action(() =>
            //{
            //    imagePreview.Source = mat.ToBitmapSource();
            //}), DispatcherPriority.Render);

            if (wBitmap == null)
            {
                wBitmap = new WriteableBitmap(matNew.Width, matNew.Height, 96, 96, PixelFormats.Bgra32, null);
                imagePreview.Source = wBitmap;
            }

            var w = matNew.Width * 4;


            wBitmap.WritePixels(new Int32Rect(0, 0, matNew.Width, matNew.Height), mat.Data, w, w);

        }
    }
}



