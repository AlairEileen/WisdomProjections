using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace WisdomProjections.Views
{
    /// <summary>
    /// Interaction logic for OutEffectsView.xaml
    /// </summary>
    public partial class OutEffectsView : Grid
    {
        public OutEffectsView()
        {
            InitializeComponent();
        }
        private bool isVideo;
        public bool IsVideo
        {
            get => isVideo; set
            {
                isVideo = value;
                img.Visibility = isVideo ? Visibility.Hidden : Visibility.Visible;
                video.Visibility = isVideo ? Visibility.Visible : Visibility.Hidden;
            }
        }

        private void Video_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (video.Source!=null)
            {
                video.Stop();
                video.Play();
            }
        }

        private void Video_Loaded(object sender, RoutedEventArgs e)
        {
            if (video.Source != null)
            {
                video.Play();
            }
        }
    }
}
