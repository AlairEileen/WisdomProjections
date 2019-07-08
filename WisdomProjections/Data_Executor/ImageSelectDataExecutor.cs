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


        public void Draw2(double with, double height,  System.Drawing.Bitmap imageBitmap,  Image imagePreview, Canvas canvas)
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
                CvInvoke.Canny(currentImage, edges, 20, 100);
                //CvInvoke.Watershed(currentImage,edges);
                //CvInvoke.GrabCut(currentImage,edges);
                CvInvoke.FindContours(edges, contours, hierarchyMat, RetrType.External, ChainApproxMethod.ChainApproxSimple);
                //Rectangle rectMax = Rectangle.Empty;

                double area0 = 0;
                double area01 = 0;
                int index = 0;
                for (int i = 0; i < contours.Size; i++)
                {
                    //var area = CvInvoke.ArcLength(contours[i],false);
                    //var area2 = CvInvoke.ContourArea(contours[i]);
                    //if (area0 < area&&area01<area2)
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



        #endregion



        //public void Draw4(double with, double height, Bitmap imageBitmap, System.Windows.Controls.Image imagePreview, System.Windows.Controls. Canvas canvas)
        //{
        //    //var aii = AForge.Imaging.Image.Clone(imageBitmap);

        //    var filtersSequence = new AForge.Imaging.Filters.FiltersSequence (AForge.Imaging.Filters.Grayscale.CommonAlgorithms.BT709);
        //    var fib = filtersSequence.Apply(imageBitmap);
        //    var aift=new AForge.Imaging.Filters.Threshold();
        //     aift.ApplyInPlace(fib);




        //    var aibcb = new AForge.Imaging.BlobCounter(fib)
        //    {
        //        FilterBlobs = true, MinWidth = 10, MinHeight = 10, ObjectsOrder = AForge.Imaging.ObjectsOrder.Area
        //    };
        //    //aibcb.ProcessImage(imageBitmap);
        //    AForge.Imaging.Blob[] blobs = aibcb.GetObjects(imageBitmap,false);
        //    //List<Bitmap> bitmaps = new List<Bitmap>();
        //    try
        //    {
        //        for (int i = 0; i < blobs.Length; i++)
        //        {
        //            CvInvoke.Imshow("blob"+i, new Image<Bgra, byte>(blobs[i].Image.ToManagedImage()));

        //        }
        //    }
        //    catch (AccessViolationException)
        //    {

        //    }


        //}





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



