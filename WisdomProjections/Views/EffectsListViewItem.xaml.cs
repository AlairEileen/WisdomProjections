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
    /// Interaction logic for EffectsListViewItem.xaml
    /// </summary>
    public partial class EffectsListViewItem : ListViewItem
    {
        public EffectsListViewItem(string title, string content,ImageSource imageSource)
        {
            InitializeComponent();
            this.lTitle.Content = title;
            this.tbContent.Text = content;
            if (imageSource!=null)
            {
                this.iIcon.Source = imageSource;
            }
        }
       
    }
}
