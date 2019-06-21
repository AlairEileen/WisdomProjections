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
        public string Id { get; set; }
        public string Tag1 { get; set; }
        public string Tag2 { get; set; }
        public string Resource { get; set; }
        public bool IsVideo { get; set; }
        public EffectsListViewItem(string id,string tag1,string tag2,string title, string content, string resource)
        {
            InitializeComponent();
            Resource = resource;
            Tag1 = tag1;
            Tag2 = tag2;
            Id = id;

            this.lTag1.Content = Tag1;
            this.lTag2.Content = Tag2;
            this.lTitle.Content = title;
            this.tbContent.Text = content;
            var ex = System.IO.Path.GetExtension(resource);


            for (int i = 0; i <MaterialInputWindow. FileExtension.Length; i++)
            {
                if (MaterialInputWindow.FileExtension[i].Equals(ex) && i >=MaterialInputWindow. VStart)
                {
                    IsVideo = true;
                }
            }

            if (IsVideo)
            {
                iIcon.Visibility = Visibility.Hidden;
                meIcon.Visibility = Visibility.Visible;
                meIcon.Source = new Uri(resource);
            }
            else iIcon.Source = (ImageSource)new BitmapImage(new Uri(resource));

          
        }
       
    }
}
