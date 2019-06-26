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
using System.Windows.Shapes;

namespace WisdomProjections.Views
{
    /// <summary>
    /// Interaction logic for OutEffectsWindow.xaml
    /// </summary>
    public partial class OutEffectsWindow : Window
    {

        private WinLocation winLocation;
        public OutEffectsWindow(WinLocation winLocation)
        {
            this.winLocation = winLocation;
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Left = winLocation.Left;
            this.Top = winLocation.Top;
            this.WindowStyle = WindowStyle.None;
            this.WindowState = WindowState.Maximized;

        }

        internal void Refresh(List<OutEffectsView> outEffectsView)
        {
            this.Background = new SolidColorBrush(Colors.Black);
            if (gContainer.Children!=null)
            {
                gContainer.Children.Clear();
            }
            outEffectsView.ForEach(x=>gContainer.Children.Add(x));
        }
    }
    public class WinLocation
    {
        public WinLocation(double left, double top, double width, double height)
        {
            Left = left;
            Width = width;
            Height = height;
            Top = top;
        }

        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
    }

    public class ScreenWindow
    {
        public OutEffectsWindow Window { get; set; }
        public System.Windows.Forms.Screen Screen { get; set; }
    }
}
