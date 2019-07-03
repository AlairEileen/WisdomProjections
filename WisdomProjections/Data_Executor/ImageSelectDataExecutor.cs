using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using WisdomProjections.Views.Sys;
using Point = OpenCvSharp.Point;
using Rect = System.Windows.Rect;

namespace WisdomProjections.Data_Executor
{
   public class ImageSelectDataExecutor
    {

        //private bool FloodFillTOcanny(Bitmap image,int floodValue,Action<Bitmap> refreshImage,DrawingVisual drawingVisual )
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

        //    using (var dc=drawingVisual.RenderOpen())
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

        //public void Draw2(Bitmap imageBitmap, DrawingVisual drawingVisual)
        //{
        //    Image<Gray, Byte> currentImage = new Image<Gray, byte>(imageBitmap);
        //    //Image<Gray, Byte> currentImage = new Image<Gray, byte>(@"C:\Data\Documents\Dev\WeChat Image_20190703112224.jpg");
        //    Image<Gray, Byte> res = new Image<Gray, byte>(currentImage.Width, currentImage.Height, new Gray(0));
        //    Mat src = new Image<Bgr, byte>(imageBitmap).Mat;
        //    VectorOfVectorOfPoint vvp = new VectorOfVectorOfPoint();
        //    Image<Rgba, Byte> disp = new Image<Rgba, byte>(currentImage.Width, currentImage.Height,new Rgba(0,0,0,0));
        //    Image<Rgba, Byte> edges = new Image<Rgba, byte>(currentImage.Width, currentImage.Height, new Rgba(0, 0, 0, 0));
        //    Mat b1 = new Mat();

        //    CvInvoke.Canny(currentImage, edges, 50, 100);
        //    CvInvoke.FindContours(edges, vvp, b1, RetrType.External, ChainApproxMethod.ChainApproxNone);

        //    Mat mask = src.ToImage<Bgr, byte>().CopyBlank().Mat;
        //    //CvInvoke.DrawContours(disp, vvp, -1, new MCvScalar(255,255, 255,100));


        //    for (int i = 0; i < vvp.Size; i++)
        //    {
        //        CvInvoke.DrawContours(disp, vvp, i, new MCvScalar(255, 255, 255, 100), 1);
        //    }

            
        //    //for (int i = 0; i < vvp.Size; i++)
        //    //{
        //    //    for (int j = 0; j < vvp[i].Size; j++)
        //    //    {
        //    //        res.Data[vvp[i][j].Y, vvp[i][j].X, 0] = 255;
        //    //    }
        //    //}


        //    using (var dc = drawingVisual.RenderOpen())
        //    {
        //      dc.DrawImage(ImageTool.BitmapToImageSource(disp.Bitmap),new Rect(0,0, disp.Width, disp.Height) );
        //      //dc.DrawImage(ImageTool.BitmapToImageSource(disp.Bitmap),new Rect(0,0, disp.ROI.Width, disp.ROI.Height) );
        //    }

        //}
        public void Draw2(Bitmap imageBitmap, DrawingVisual drawingVisual)
        {
            Mat mat1 = imageBitmap.ToMat();
            
            //Mat mat1 = new Mat(@"C:\Data\Documents\Dev\WeChat Image_20190703112224.jpg", ImreadModes.AnyColor);
            Mat mat2 = new Mat();
            Mat mat3 = new Mat();
            Cv2.CvtColor(mat1, mat3, ColorConversionCodes.BGR2GRAY, 0);
            Cv2.Threshold(mat3, mat2, 200, 255, ThresholdTypes.Binary);
            Cv2.Canny(mat2,mat3,50,100);
            

            Point[][] contours;
            HierarchyIndex[] hierarchy;

            Cv2.FindContours(mat2, out contours, out hierarchy, RetrievalModes.External, ContourApproximationModes.ApproxNone);

            for (int i = 0; i < hierarchy.Length; i++)
            {
                Cv2.DrawContours(mat1, contours, i, Scalar.Red, 1, LineTypes.Link8, hierarchy, 4, new Point(10, 10));
            }
            using (var dc = drawingVisual.RenderOpen())
            {
                
                dc.DrawImage(mat1.ToBitmapSource(), new Rect(0, 0, mat2.Width, mat2.Height));
            }



            

        }
    }
}
