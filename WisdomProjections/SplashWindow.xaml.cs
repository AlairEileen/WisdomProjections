﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Newtonsoft.Json;
using WisdomProjections.Data_Executor;
using WisdomProjections.Views;

namespace WisdomProjections
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public static string IpCameraJsonPath = MaterialInputWindow.ResourcesRootPath + @"IpCamera.json";
        private IpCameraViewer ipCameraViewer;
        private IpCamera ipCameraInfo;
        public SplashWindow()
        {
            InitializeComponent();
            Closing += SplashWindow_Closing;
        }

        private void SplashWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ipCameraViewer?.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var ic = IpCamera.GetIpCameraInFile();
            if (ic != null)
            {
                TextBoxIp.Text = ic.Ip;
                TextBoxPort.Text = ic.Port;
                TextBoxUserName.Text = ic.UserName;
                TextBoxPassword.Text = ic.Password;
            }
            //StartInit();//执行默认摄像头启动
        }



        private void CheckBoxUsbCamera_OnChecked(object sender, RoutedEventArgs e)
        {
            StartInit();
        }

        private void StartInit()
        {
            //new Thread(() =>
            //{
            //Thread.Sleep(2000);
            bool isLoaded = true;
            SensorDataExecutor.SensorDe.InitSensorDE(s =>
            {
                MessageBox.Show("摄像头传感器" + s.ToString());
                isLoaded = false;
            });
            //Application.Current.Dispatcher.Invoke(() =>
            //{

            if (isLoaded)
                new MainWindow().Show();
            this.Close();
            //});

            //}).Start();
        }

        private void BorderOk_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ApplicationInfoContext.IpCameraInfo = ipCameraInfo;
            new MainWindow().Show();
            Close();
        }



        private void BorderTest_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ipCameraViewer?.Stop();

            ipCameraInfo = new IpCamera { Port = TextBoxPort.Text, Ip = TextBoxIp.Text, UserName = TextBoxUserName.Text, Password = TextBoxPassword.Text };

            ipCameraInfo.SaveIpCameraInFile();
            ipCameraViewer = new IpCameraViewer(ipCameraInfo, ImageViewer);
        }


    }


    public class IpCameraViewer
    {
        private bool isStartCamera;
        private WriteableBitmap writeableBitmap;
        private readonly Thread cameraThread;

        public IpCameraViewer(IpCamera cameraInfo, Image imageViewer)
        {
            cameraThread = new Thread(() =>
            {

                var capture =
                    new VideoCapture(
                        $"http://{cameraInfo.UserName}:{cameraInfo.Password}@{cameraInfo.Ip}:{cameraInfo.Port}");

                //capture.Start();
                if (capture.IsOpened)
                {

                    Mat mat = new Mat();
                    capture.Read(mat);
                    if (!mat.IsEmpty)
                    {
                        var img = mat.ToImage<Bgra, byte>();
                        Application.Current?.Dispatcher?.BeginInvoke(new Action(() =>
                        {
                            writeableBitmap =
                                new WriteableBitmap(img.Width, img.Height, 96, 96, PixelFormats.Bgra32, null);

                            imageViewer.Source = writeableBitmap;
                            writeableBitmap.WritePixels(new Int32Rect(0, 0, img.Width, img.Height), img.Bytes,
                                img.Width * 4, 0);
                        }));
                        isStartCamera = true;
                        //new Thread(() =>
                        //{
                        while (isStartCamera)
                        {
                            capture.Read(mat);
                            if (!mat.IsEmpty)
                            {
                                var img2 = mat.ToImage<Bgra, byte>();

                                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                                {
                                    writeableBitmap.WritePixels(new Int32Rect(0, 0, img2.Width, img2.Height),
                                        img2.Bytes,
                                        img2.Width * 4, 0);
                                }));
                            }

                            Thread.Sleep(15);
                        }

                    }
                    //}).Start();
                }
            });
            cameraThread.Start();
        }

        public void Stop()
        {
            isStartCamera = false;
            cameraThread.Abort();
        }
    }



    public class IpCamera
    {
        public string Ip { get; set; }
        public string Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public static IpCamera GetIpCameraInFile()
        {
            try
            {
                if (File.Exists(SplashWindow.IpCameraJsonPath))
                {
                    string json = File.ReadAllText(SplashWindow.IpCameraJsonPath, Encoding.UTF8);
                    return JsonConvert.DeserializeObject<IpCamera>(json);
                }
            }
            catch (Exception)
            {

                return null;
            }

            return null;
        }
        public void SaveIpCameraInFile()
        {
            try
            {
                string json = JsonConvert.SerializeObject(this);
                File.WriteAllText(SplashWindow.IpCameraJsonPath, json, Encoding.UTF8);
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
