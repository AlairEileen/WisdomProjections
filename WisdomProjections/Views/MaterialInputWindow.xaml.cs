using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WisdomProjections.Models;

namespace WisdomProjections.Views
{
    /// <summary>
    /// Interaction logic for MaterialInputWindow.xaml
    /// </summary>
    public partial class MaterialInputWindow : Window
    {


        private static string _rootPath;

        public static string ResourcesRootPath =>
            _rootPath ?? (_rootPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) +
                                    @"\Wisdom\");

        public static string ResourcesFilePath = ResourcesRootPath + @"Resources\";
        public static string ResourcesMaterialPath = ResourcesRootPath + @"Material.json";
        public static string[] FileExtension = { ".jpg", ".png", ".jpeg", ".bmp", ".gif", ".mp4", ".avi" };
        public const int VStart = 5;
        public static string OpenFilter
        {
            get
            {
                var str = "媒体文件(*.jpg,*.png,*.jpeg,*.bmp,*.gif,*.mp4,*.avi)|*.jpg;*.png;*.jpeg;*.bmp;*.gif;*.mp4;*.avi";
                var str0 = "媒体文件(";
                var str1 = ")|";
                foreach (var x in FileExtension)
                {
                    str0 += "*" + x + ","; str1 += "*" + x + ";";
                }
                //str0= str0.Substring(0, str0.Length - 1);
                str = str0 + str1;
                return str;
            }
        }
        private readonly string fileName;
        private MaterialJsonModel materialJsonModel;

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
            for (int i = 0; i < FileExtension.Length; i++)
            {
                if (FileExtension[i].Equals(ex) && i >= VStart)
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


            if (File.Exists(ResourcesMaterialPath))
            {

                try
                {
                    var json = File.ReadAllText(ResourcesMaterialPath);
                    if (json.Trim().Length > 0)
                    {
                        materialJsonModel = JsonConvert.DeserializeObject<MaterialJsonModel>(json);
                        if (materialJsonModel != null && materialJsonModel.TagModels != null)
                        {
                            var tags = new List<string>();
                            materialJsonModel.TagModels.ForEach(x => tags.Add(x.Name));
                            cbTag1.ItemsSource = tags;
                            cbTag2.ItemsSource = tags;
                        }
                    }
                }
                catch (Exception)
                {
                }
            }
        }

        private void LOK_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //var tag1Text= (cbTag1.FindName("cbtb") as TextBox).Text;
            var tag2Text = (cbTag2.Template.FindName("cbtb", cbTag2) as TextBox).Text;
            var tag1Text = (cbTag1.Template.FindName("cbtb", cbTag1) as TextBox).Text;
            var titleText = tbTitle.Text.ToString();
            var contentText = tbContent.Text.ToString();
            if (!(titleText.Trim().Length == 0 || tag1Text.Trim().Length == 0 || tag2Text.ToString().Trim().Length == 0 || tag1Text.Trim().Equals(tag2Text)))
            {
                if (!Directory.Exists(ResourcesFilePath)) Directory.CreateDirectory(ResourcesFilePath);
                var json = "";
                MaterialJsonModel materialJsonModel = null;
                if (File.Exists(ResourcesMaterialPath))
                {
                    json = File.ReadAllText(ResourcesMaterialPath);
                }
                try
                {
                    if (json.Trim().Length != 0)
                    {
                        materialJsonModel = JsonConvert.DeserializeObject<MaterialJsonModel>(json);
                    }
                }
                catch (Exception)
                {
                    File.Move(ResourcesMaterialPath, ResourcesMaterialPath + ".bak");
                }

                TagModel tag1 = null, tag2 = null;
                if (materialJsonModel == null)
                {
                    materialJsonModel = new MaterialJsonModel();
                }
                if (materialJsonModel.MaterialModels != null)
                {
                    if (materialJsonModel.MaterialModels.Find(x => x.Title.Equals(tbTitle.Text)) != null)
                    {
                        MessageBox.Show($"该标题:{tbTitle.Text} 已存在!");
                        return;
                    }
                }
                else materialJsonModel.MaterialModels = new List<MaterialModel>();
                if (materialJsonModel.TagModels != null)
                {
                    tag1 = materialJsonModel.TagModels.Find(x => x.Name.Equals(tag1Text));
                    if (tag1 != null) tag2 = tag1.Tag2.Find(x => x.Name.Equals(tag2Text));

                }
                else materialJsonModel.TagModels = new List<TagModel> { new TagModel { Id = Guid.NewGuid().ToString("N"), Name = "全部", Tag2 = new List<TagModel> { new TagModel { Id = Guid.NewGuid().ToString("N"), Name = "全部" } } } };
                if (tag1 == null)
                {
                    tag1 = new TagModel { Id = Guid.NewGuid().ToString("N"), Name = tag1Text };
                    materialJsonModel.TagModels.Add(tag1);
                }
                if (tag2 == null)
                {
                    tag2 = new TagModel { Id = Guid.NewGuid().ToString("N"), Name = tag2Text };
                    if (tag1.Tag2 == null) tag1.Tag2 = new List<TagModel> { new TagModel { Id = Guid.NewGuid().ToString("N"), Name = "全部" } };
                    tag1.Tag2.Add(tag2);
                }

                var realName = System.IO.Path.GetFileNameWithoutExtension(fileName);
                var mmId = Guid.NewGuid().ToString("N");
                var resouceFileName = mmId + System.IO.Path.GetExtension(fileName);
                var resourceName = ResourcesFilePath + resouceFileName;
                File.Copy(fileName, resourceName);


                var materialModel = new MaterialModel { Content = tbContent.Text, Id = mmId, Tag1 = tag1, Tag2 = tag2, ResourceFileRealName = realName, ResourceFileName = resouceFileName, Title = tbTitle.Text };
                materialJsonModel.MaterialModels.Add(materialModel);
                var wJson = JsonConvert.SerializeObject(materialJsonModel);
                File.WriteAllText(ResourcesMaterialPath, wJson);

                if (MessageBoxResult.OK == MessageBox.Show("添加成功！")) this.Close();

            }
            else MessageBox.Show("标题、标签为必填项!");
        }

        private bool CheckValue() => !(tbTitle.Text.Trim().Length == 0 || cbTag1.Text.Trim().Length == 0 || cbTag2.Text.ToString().Trim().Length == 0);


        private void LCancel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void CbTag1_TextInput(object sender, TextCompositionEventArgs e)
        {
            var a = sender;
            var b = e;
        }

        private void CbTag1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitEffectsType(cbTag1.SelectedIndex);
            if (cbTag1.SelectedIndex != 0)
            {
                cbTag2.SelectedIndex = 0;
            }
        }
        private void InitEffectsType(int v)
        {

            var list2 = new List<string>();
            materialJsonModel.TagModels[v == -1 ? 0 : v].Tag2.ForEach(x => list2.Add(x.Name));
            cbTag2.ItemsSource = list2;
        }
        private void CbTag2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
