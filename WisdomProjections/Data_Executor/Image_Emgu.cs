using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace WisdomProjections.Data_Executor
{
    public class Image_Emgu
    {
        // 得到投影仪区域，以及轮廓
        public static VectorOfPointF GetScreen(Bitmap imgPath, double x, double y, double w, double h)
        {
            // 使用GrabCut算法分割图像的前景和背景
            Image<Bgr, Byte> img = new Image<Bgr, byte>(imgPath);
            //Image<Bgr, Byte> img2 = new Image<Bgr, byte>(imgPath);
            //CvInvoke.Flip(img2,img,FlipType.Vertical);
            Matrix<Double> bgdModel_mask = new Matrix<double>(1, 13 * 5);
            Matrix<Double> fgdModel_mask = new Matrix<double>(1, 13 * 5);
            Image<Gray, byte> mask_mask = new Image<Gray, byte>(img.Size);
            // 特别说明：Rectangle对象的参数是需要设置的，即设置前景图像的大概位置
            // Rectangle rect = new Rectangle(new Point(100, 230), new Size(2100, 1500));
            // Rectangle rect = new Rectangle(new Point(50, 130), new Size(530, 330));
            //  Rectangle rect = new Rectangle(new Point(25, 55), new Size(580, 370));
            Rectangle rect = new Rectangle(new Point((int)x, (int)y), new Size((int)w, (int)h));
            CvInvoke.GrabCut(img, mask_mask, rect, bgdModel_mask, fgdModel_mask, 1, GrabcutInitType.InitWithRect);

            // 得到前景图的Mask
            using (ScalarArray ia_mask = new ScalarArray(3))
                CvInvoke.Compare(mask_mask, ia_mask, mask_mask, CmpType.Equal);

            // 查找前景图的Mask的所有轮廓
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(mask_mask, contours, hierarchy, RetrType.External,
                ChainApproxMethod.ChainApproxSimple);

            // 根据面积查找轮廓最大的索引
            int maxAreaIndex = 0;
            double areaContour = CvInvoke.ContourArea(contours[0]);
            for (int i = 1; i < contours.Size; i++)
            {
                if (CvInvoke.ContourArea(contours[i]) > areaContour)
                {
                    areaContour = CvInvoke.ContourArea(contours[i]);
                    maxAreaIndex = i;
                }
            }

            // 将轮廓显示在图形上面
            Image<Bgr, byte> imageContour = new Image<Bgr, byte>(img.Width, img.Height);
            CvInvoke.DrawContours(imageContour, contours, maxAreaIndex, new MCvScalar(255, 0, 255), 1);

            // 返回轮廓位置坐标，轮廓边缘坐标
            VectorOfPoint vectorOfPoint = contours[maxAreaIndex];

            // 通过矩形检测返回投影仪坐标位置
            RotatedRect rotatedRect = CvInvoke.MinAreaRect(vectorOfPoint);

            PointF topLeft = new PointF(); //定义上左点对象
            PointF topRight = new PointF(); //定义上右点对象
            PointF bottomLeft = new PointF(); //定义下左点对象
            PointF bottomRight = new PointF(); //定义下右点对象
            if (rotatedRect.Angle >= 0)
            {
                topLeft = new PointF(rotatedRect.Center.X - rotatedRect.Size.Width / 2,
                    rotatedRect.Center.Y - rotatedRect.Size.Height / 2);
                topRight = new PointF(rotatedRect.Center.X + rotatedRect.Size.Width / 2,
                    rotatedRect.Center.Y - rotatedRect.Size.Height / 2);
                bottomLeft = new PointF(rotatedRect.Center.X - rotatedRect.Size.Width / 2,
                    rotatedRect.Center.Y + rotatedRect.Size.Height / 2);
                bottomRight = new PointF(rotatedRect.Center.X + rotatedRect.Size.Width / 2,
                    rotatedRect.Center.Y + rotatedRect.Size.Height / 2);
            }
            else
            {
                topLeft = new PointF(rotatedRect.Center.X - rotatedRect.Size.Height / 2,
                    rotatedRect.Center.Y - rotatedRect.Size.Width / 2);
                topRight = new PointF(rotatedRect.Center.X + rotatedRect.Size.Height / 2,
                    rotatedRect.Center.Y - rotatedRect.Size.Width / 2);
                bottomLeft = new PointF(rotatedRect.Center.X - rotatedRect.Size.Height / 2,
                    rotatedRect.Center.Y + rotatedRect.Size.Width / 2);
                bottomRight = new PointF(rotatedRect.Center.X + rotatedRect.Size.Height / 2,
                    rotatedRect.Center.Y + rotatedRect.Size.Width / 2);
            }

            VectorOfPointF coordinateOfRect = new VectorOfPointF(new[] { topLeft, topRight, bottomLeft, bottomRight });

            return coordinateOfRect;
        }
    }
}
