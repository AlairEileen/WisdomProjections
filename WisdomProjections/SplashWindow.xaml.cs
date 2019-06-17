using System;
using System.Collections.Generic;
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
using WisdomProjections.Data_Executor;

namespace WisdomProjections
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public SplashWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(() =>
            {

                Thread.Sleep(2000);
                bool isLoaded = true;
                SensorDataExecutor.SensorDE.InitSensorDE(s =>
                {
                    MessageBox.Show("摄像头传感器" + s.ToString());
                    isLoaded = false;
                });
                App.Current.Dispatcher.Invoke(() =>
                {

                    if (isLoaded)
                        new MainWindow().Show();
                    this.Close();
                });

            })).Start();
        }
    }
}
