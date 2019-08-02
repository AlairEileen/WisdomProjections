using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace WisdomProjections.Data_Executor
{
    public class Image_Emgu
    {
        #region 获取投影仪轮廓
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
        #endregion


        #region 获取物体轮廓

        // 存储去除冗余内接矩形后的矩形集合
        static List<Rectangle> contourResult = new List<Rectangle>();

        // 存储合并交叉矩形后的矩形集合
        static List<Rectangle> contourFinal = new List<Rectangle>();

        // 存储交叉矩形合并后的矩形
        static HashSet<Rectangle> rectSet = new HashSet<Rectangle>();

        // 去除冗余内接矩形
        static void GetRectNoRepeat(List<Rectangle> rectList, Rectangle rect)
        {
            int flag = 0; //判断一个矩形是否是矩形集合的内接矩形，0表示否，1表示是
            for (int i = 0; i < rectList.Count; i++)
            {
                // 如果该矩形是矩形集合的内接矩形，那么设置flag为1
                if (rect.X > rectList[i].X && rect.Y > rectList[i].Y &&
                    rect.X + rect.Width < rectList[i].X + rectList[i].Width
                    && rect.Y + rect.Height < rectList[i].Y + rectList[i].Height)
                {
                    flag = 1;
                }
            }

            // 如果该矩形不是矩形集合的内接矩形，那么将该矩形添加到contourResult中
            if (flag == 0)
            {
                contourResult.Add(rect);
            }
        }

        //返回最小值
        static int minValue(int x, int y)
        {
            if (x <= y)
            {
                return x;
            }
            else
            {
                return y;
            }
        }

        //返回最大值
        static int maxValue(int x, int y)
        {
            if (x >= y)
            {
                return x;
            }
            else
            {
                return y;
            }
        }

        // 将交叉矩形合并为大的矩形
        static void MergeRect(List<Rectangle> rectList, Rectangle rect)
        {
            int flag = 0; //0表示没有交集，1表示有交集
            for (int i = 0; i < rectList.Count; i++)
            {
                if (rectList[i].IntersectsWith(rect) && rectList[i] != rect)
                {
                    flag = 1;
                    rectSet.Add(new Rectangle(
                        new Point(minValue(rectList[i].X, rect.X), minValue(rectList[i].Y, rect.Y)),
                        new Size(
                            maxValue(rectList[i].X + rectList[i].Width, rect.X + rect.Width) -
                            minValue(rectList[i].X, rect.X),
                            maxValue(rectList[i].Y + rectList[i].Height, rect.Y + rect.Height) -
                            minValue(rectList[i].Y, rect.Y))));
                }
            }

            // 如果没有交集，那么存储该矩形
            if (flag == 0)
            {
                contourFinal.Add(rect);
            }
        }

        // 拟合点集得到轮廓最外边缘的坐标点
        static List<Point> FitPointContour(List<Point> listOfPoint, Rectangle rect)
        {
            List<Point> leftPoint = new List<Point>();
            List<Point> rightPoint = new List<Point>();
            List<Point> allPoint = new List<Point>();

            // 将点集转换为字典的形式
            Dictionary<int, List<int>> pointDict = new Dictionary<int, List<int>>();
            for (int i = 0; i < listOfPoint.Count; i++)
            {
                if (pointDict.ContainsKey(listOfPoint[i].Y))
                {
                    pointDict[listOfPoint[i].Y] = pointDict[listOfPoint[i].Y].Append(listOfPoint[i].X).ToList();
                }
                else
                {
                    pointDict.Add(listOfPoint[i].Y, new List<int>(listOfPoint[i].X));
                }
            }

            // 根据字典的Key值从小到大进行排序
            Dictionary<int, List<int>> pointDictDesc =
                pointDict.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);

            // 得到左边和右边的点集合
            foreach (var dict in pointDictDesc)
            {
                if (dict.Value.Count >= 2) // 对应的X列表至少应该包含2个元素
                {
                    if ((dict.Value.Max() - dict.Value.Min()) > 10)
                    {
                        leftPoint.Add(new Point(dict.Value.Min(), dict.Key));
                        rightPoint.Add(new Point(dict.Value.Max(), dict.Key));
                    }
                }
            }

            // 按照顺时针方向合并左边和右边的点集合
            for (int i = 0; i < rightPoint.Count; i++)
            {
                allPoint.Add(rightPoint[i]);
            }

            for (int i = leftPoint.Count - 1; i > 0; i--)
            {
                allPoint.Add(leftPoint[i]);
            }

            return allPoint;
        }

        //根据输入参数调整物体的轮廓
        public static Mat GetContourView(Bitmap imgPath, double threshold1, double threshold2)
        {
            // 根据路径读取图像[RGB]
            Mat imgOriginal = new Image<Bgr, byte>(imgPath).Mat;
            //将RGB图像转换为灰度图像
            Mat grayImg = new Mat();
            CvInvoke.CvtColor(imgOriginal, grayImg, ColorConversion.Rgb2Gray);
            //使用Canny算子做边缘检测
            Mat cannyImg = new Mat();
            CvInvoke.Canny(grayImg, cannyImg, threshold1, threshold2, apertureSize: 3);
            //使用高斯滤波进行噪音去除
            CvInvoke.GaussianBlur(cannyImg, cannyImg, new Size(3, 3), 0);
            return cannyImg;
        }

        // 根据鼠标位置，得到物体轮廓边缘
        public static Tuple<List<Point>, Rectangle> GetBlobView(Mat cannyImg, Point inputPoint, int number)
        {
            // 轮廓检测
            var contoursImg = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(cannyImg, contoursImg, null, RetrType.Tree, ChainApproxMethod.ChainApproxSimple);

            // 将轮廓面积及索引存储在字典中
            Dictionary<string, double> areaIndex = new Dictionary<string, double>();
            for (int i = 0; i < contoursImg.Size; i++)
            {
                areaIndex.Add(Convert.ToString(i), CvInvoke.ContourArea(contoursImg[i]));
            }
            // 按照轮廓面积值的索引从大到小进行排序
            List<int> listIndex = new List<int>();
            Dictionary<string, double> areaIndexDesc =
                areaIndex.OrderByDescending(s => s.Value).ToDictionary(o => o.Key, p => p.Value);
            foreach (KeyValuePair<string, double> kvp in areaIndexDesc)
            {
                listIndex.Add(Convert.ToInt16(kvp.Key));
            }

            //自定义取TopK最大轮廓数目
            List<int> listIndexTopK = listIndex.Take(number).ToList();

            // 勾画轮廓
            Image<Bgr, byte> imageContour = new Image<Bgr, byte>(cannyImg.Width, cannyImg.Height);
            for (int i = 0; i < listIndexTopK.Count; i++)
            {
                CvInvoke.DrawContours(imageContour, contoursImg, listIndexTopK[i], new MCvScalar(255, 0, 0), 2);
            }

            // 画出轮廓的最小外接矩形
            List<Rectangle> contourOfRect = new List<Rectangle>();
            for (int i = 0; i < listIndexTopK.Count; i++)
            {
                RotatedRect rotatedRect = CvInvoke.MinAreaRect(contoursImg[listIndexTopK[i]]);
                contourOfRect.Add(rotatedRect.MinAreaRect());
            }
            // 调用去除冗余外接矩形函数
            for (int i = 0; i < contourOfRect.Count; i++)
            {
                GetRectNoRepeat(contourOfRect, contourOfRect[i]);
            }

            // 调用合并交叉矩形函数
            for (int i = 0; i < contourResult.Count; i++)
            {
                MergeRect(contourResult, contourResult[i]);
            }

            // 将合并后的矩形添加到矩形集合中
            foreach (var i in rectSet)
            {
                contourFinal.Add(i);
            }

            // 调试点2：将外接矩形显示在图像上，调整自定义TopK最大轮廓数目
            // ****************************************************
            for (int i = 0; i < contourFinal.Count; i++)
            {
                CvInvoke.Rectangle(imageContour, contourFinal[i], new MCvScalar(255, 255, 255), 3);
            }
            //****************************************************

            // 找出最终的矩形质心及对应矩阵坐标[字典类型]
            Dictionary<Point, Rectangle> dictOfPoint = new Dictionary<Point, Rectangle>();
            foreach (var rect in contourFinal)
            {
                // 如果字典dictOfPoint已经包含该点，那么将该点右下移动1个像素
                if (dictOfPoint.ContainsKey(new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2)))
                {
                    dictOfPoint.Add(new Point(rect.X + rect.Width / 2 + 1, rect.Y + rect.Height / 2 + 1), rect);
                }
                else
                {
                    dictOfPoint.Add(new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2), rect);
                }
            }

            // 通过输入坐标(X,Y)找出对应图形轮廓坐标
            // 如果输入的点在检测的矩形中，那么得到相应矩形坐标
            // 如果输入的点不在检测的矩形中，那么找到最近的矩形质心，从而得到相应矩形坐标
            List<Rectangle> inputRect = new List<Rectangle>();
            Dictionary<double, Point> resultRect = new Dictionary<double, Point>();
            foreach (var rect in contourFinal)
            {
                // 判断输入点是否在最终的矩形集合中
                if (rect.Contains(inputPoint))
                {
                    inputRect.Add(rect);
                }
            }

            // 输入的点在检测的矩形中
            int flag = 0;
            if (inputRect.Count != 0)
            {
                // 调试点3：显示输入的坐标所对应的矩形[输入的点在检测的矩形中]
                // ****************************************************
                //CvInvoke.Rectangle(imageContour, inputRect[0], new MCvScalar(255, 255, 255), 3);
                // ****************************************************
            }
            else //输入的点不在检测的矩形中
            {
                flag = 1;
                foreach (var key in dictOfPoint)
                {
                    resultRect.Add(Math.Sqrt(Math.Pow(key.Key.X - inputPoint.X, 2) + Math.Pow(key.Key.Y - inputPoint.Y, 2)), key.Key);
                }

                // 调试点4：显示输入的坐标所对应的矩形[输入的点不在检测的矩形中]
                // ****************************************************
                //CvInvoke.Rectangle(imageContour, dictOfPoint[resultRect[resultRect.Keys.Min()]], new MCvScalar(255, 255, 255), 3);
                // ****************************************************
            }

            // vectorOfPoint表示根据输入坐标确定的物体轮廓的坐标点集合
            // Bgr color = new Bgr(255, 255, 255); //白色的点 
            Bgr blue = new Bgr(255, 0, 0); //蓝色的点
            List<Point> listOfPoint = new List<Point>();
            if (flag == 0)
            {
                for (int i = 0; i < imageContour.Rows; i++)
                {
                    for (int j = 0; j < imageContour.Cols; j++)
                    {
                        if (inputRect[0].Contains(new Point(j, i)) && imageContour[i, j].Equals(blue))
                        {
                            //imageContour[i, j] = color;
                            listOfPoint.Add(new Point(j, i));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < imageContour.Rows; i++)
                {
                    for (int j = 0; j < imageContour.Cols; j++)
                    {
                        if (dictOfPoint[resultRect[resultRect.Keys.Min()]].Contains(new Point(j, i)) &&
                            imageContour[i, j].Equals(blue))
                        {
                            //imageContour[i, j] = color;
                            listOfPoint.Add(new Point(j, i));
                        }
                    }
                }
            }

            // 拟合点集得到轮廓
            List<Point> finalPoint = new List<Point>();
            if (flag == 0)
            {
                finalPoint = FitPointContour(listOfPoint, inputRect[0]);
            }
            else
            {
                finalPoint = FitPointContour(listOfPoint, dictOfPoint[resultRect[resultRect.Keys.Min()]]);
            }

            // 记录相邻点对，为画直线做准备[调试程序用，根据该结果进行参数调整]
            for (int i = 0; i < finalPoint.Count - 1; i++)
            {
                CvInvoke.Line(imageContour, finalPoint[i], finalPoint[i + 1], new MCvScalar(255, 255, 255), 1,
                    LineType.EightConnected, 0);
            }

            CvInvoke.Line(imageContour, finalPoint[0], finalPoint[finalPoint.Count - 1], new MCvScalar(255, 255, 255),
                1, LineType.EightConnected, 0);

            // 调试点5：将外轮廓点显示在图像上
            // ****************************************************
            //foreach (var point in finalPoint)
            //{
            //    imageContour[point.Y, point.X] = color;
            //}
            // ****************************************************

            // 调试点6：显示图形用于调试代码
            // ****************************************************
            CvInvoke.NamedWindow("Contour Coordinates", NamedWindowType.FreeRatio);
            CvInvoke.Circle(imageContour, new Point(inputPoint.X, inputPoint.Y), 20, new MCvScalar(255, 255, 255), 3,
                LineType.EightConnected, 0);
            //CvInvoke.Imshow("Contour Coordinates", imageContour);
            //CvInvoke.WaitKey(0);
            // ****************************************************

            if (flag == 0)
            {
                return new Tuple<List<Point>, Rectangle>(finalPoint, inputRect[0]);
            }
            else
            {
                return new Tuple<List<Point>, Rectangle>(finalPoint, dictOfPoint[resultRect[resultRect.Keys.Min()]]);
            }
        }



        #endregion



    }


}
