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
using WisdomProjections.Views.Sys;

namespace WisdomProjections.Views
{
    /// <summary>
    /// Interaction logic for TextListViewItem.xaml
    /// </summary>
    public partial class TextListViewItem : ListViewItem
    {
        private bool isChecked;
        public TextListViewItem(string text, Func<TextListViewItem, MouseButtonEventHandler> itemClick)
        {
            InitializeComponent();
            lText.Content = text;
            
            lText.MouseLeftButtonDown += itemClick(this);
        }
       
        public bool IsChecked
        {
            get => isChecked; set
            {
                isChecked = value;
                //lText.Foreground = new SolidColorBrush(isChecked ? ColorTool.GetMediaColorFromDrawingColor(System.Drawing.Color.FromName("#FF493F61")) : Colors.White);
            }
        }
    }
}
