using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace WisdomProjections.Views
{
    /// <summary>
    /// Interaction logic for MaterialInputWindow.xaml
    /// </summary>
    public partial class MaterialInputWindow : Window
    {
        private static string[] fileExtension = { ".jpg", ".png", ".jpeg", ".bmp", ".gif", ".mp4", ".avi" };
        private int vStart = 5;
        public static string OpenFilter
        {
            get
            {
                var str = "媒体文件(*.jpg,*.png,*.jpeg,*.bmp,*.gif,*.mp4,*.avi)|*.jpg;*.png;*.jpeg;*.bmp;*.gif;*.mp4;*.avi";
                var str0 = "媒体文件(";
                var str1 = ")|";
                foreach (var x in fileExtension)
                {
                    str0 += "*" + x + ","; str1 += "*" + x + ";";
                }
                str0.Substring(0, str0.Length - 1);
                str1.Substring(0, str1.Length - 1);
                str = str0 + str1;
                return str;
            }
        }
        private string fileName;

        public MaterialInputWindow(string fileName)
        {
            InitializeComponent();
            this.fileName = fileName;
            InitData();
        }

        private void InitData()
        {
            var f = fileName.Substring(0, fileName.LastIndexOf("."));
            f = f.Substring(f.LastIndexOf("\\") + 1);
            tbTitle.Text = f.Length > 10 ? f.Substring(0, 10) : f;
            var ex = System.IO.Path.GetExtension(fileName);
            bool isVideo = false;
            for (int i = 0; i < fileExtension.Length; i++)
            {
                if (fileExtension[i].Equals(ex) && i >= vStart)
                {
                    isVideo = true;
                }
            }

            if (isVideo)
            {
                iIcon.Visibility = Visibility.Hidden;
                meIcon.Visibility = Visibility.Visible;
                meIcon.Source = new Uri(fileName);
            }
            else iIcon.Source = (ImageSource)new BitmapImage(new Uri(fileName));
        }
        string title = "", content = "", tag1 = "", tag2 = "";

        private void CbTag2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbTag2.Text.Length > 5) cbTag2.Text = tag2;
                else tag2 = cbTag2.Text;
            }
            catch (Exception)
            {
            }

        }

        private void CbTag1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cbTag1.Text.Length > 5) cbTag1.Text = tag1;
                else tag1 = cbTag1.Text;
            }
            catch (Exception)
            {
            }
        }

        private void TbTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbTitle.Text.Length > 10) tbTitle.Text = title;
                else title = tbTitle.Text;
            }
            catch (Exception)
            {
            }
        }

        private void TbContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (tbContent.Text.Length > 40) tbContent.Text = content;
                else content = tbContent.Text;
            }
            catch (Exception)
            {
            }

        }


    }
}
