using Emgu.CV;
using Emgu.CV.Structure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using WisdomProjections.Data_Executor;
using WisdomProjections.Models;
using WisdomProjections.Views;
using WisdomProjections.Views.Sys;
using YDPeopleSensor.Net;

namespace WisdomProjections
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {


        #region Window事件
        public MainWindow()
        {
            InitializeComponent();
            SensorDataExecutor.SensorDE.InitContent(imgContainer);
            Closing += SensorDataExecutor.SensorDE.Close;
        }
        /// <summary>
        /// 窗口加载完
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Button btnGrdSplitter = gsSplitterr.Template.FindName("btnExpend", gsSplitterr) as Button;
            Button btnGrdSplitter2 = gsSplitterr2.Template.FindName("btnExpend", gsSplitterr2) as Button;
            if (btnGrdSplitter != null)
                btnGrdSplitter.Click += new RoutedEventHandler(BtnGrdSplitter_Click);
            if (btnGrdSplitter2 != null)
                btnGrdSplitter2.Click += new RoutedEventHandler(BtnGrdSplitter2_Click);
            InitData();
            deviceTimer = new Timer(o => { if (App.Current != null && App.Current.Dispatcher != null) App.Current.Dispatcher.Invoke(InitDevices); }, null, 500, 500);
        }



        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            imgContainer.PaintTypeSelects = new PaintTypeSelect[] {
                new PaintTypeSelect { PaintType = PaintType.None, IsSelected = true },
                new PaintTypeSelect { PaintType = PaintType.Cricle, IsSelected = false ,TypeView=imgCircle},
                new PaintTypeSelect { PaintType = PaintType.Move, IsSelected = false ,TypeView=imgSlect},
                new PaintTypeSelect { PaintType = PaintType.Rectangle, IsSelected = false,TypeView=imgSquare },
                new PaintTypeSelect { PaintType = PaintType.Pen, IsSelected = false ,TypeView=imgPen},
            };

            InitModelItems();

            InitEffects();
        }



        /// <summary>
        /// 窗口关闭中
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
        #endregion

        //private IOutputArray contours;

        #region 导航按钮点击事件
        /// <summary>
        /// 返回点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgBack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 下一个点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgNext_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 拖动点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgSlect_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPaintType(PaintType.Move);
        }
        /// <summary>
        /// 矩形点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgSquare_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPaintType(PaintType.Rectangle);
        }
        private void SwitchPaintType(PaintType rectangle)
        {
            foreach (var item in imgContainer.PaintTypeSelects)
            {
                if (item.PaintType == rectangle && item.IsSelected)
                {
                    item.IsSelected = false;

                    var ic = imgContainer.PaintTypeSelects.Where(x => x.PaintType == PaintType.None).FirstOrDefault();
                    ic.IsSelected = true;
                    ic.ChangeCursor(this);
                    break;
                }
                else
                {
                    item.IsSelected = item.PaintType == rectangle;
                    item.ChangeCursor(this);
                }
            }

        }
        /// <summary>
        /// 圆点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgCircle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 笔点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgPen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        /// <summary>
        /// 缩小点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgShrink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgContainer.ChangeImageSize(-60);
        }
        /// <summary>
        /// 放大点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgMagnify_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            imgContainer.ChangeImageSize(60);
        }
        private void ChangeCursor()
        {

        }
        #endregion


        #region 模型列表操作
        /// <summary>
        /// 模型列表
        /// </summary>
        public List<ModelItem> ModelItems { get; set; } = new List<ModelItem>();
        private int rectNameIndex;
        private void InitModelItems()
        {
            lvModel.ItemsSource = ModelItems;
            lvModel.SelectionChanged += LvModel_SelectionChanged;


            imgContainer.RectangleAdd += d =>
            {
                var name = "矩形" + ++rectNameIndex;
                d.ToolTip = name;
                ModelItems.Add(new ModelItem { Name = name, View = d });
                //modelTextList.Add(new TextListViewItem(ModelItems[ModelItems.Count - 1].Name, t => (s, e) =>
                //    {
                //        foreach (var item in modelTextList)
                //        {
                //            item.IsChecked = false;
                //        }
                //        t.IsChecked = true;

                //    }));
                lvModel.Items.Refresh();
            };
            imgContainer.RectangleDel += d =>
            {
                for (int i = 0; i < ModelItems.Count; i++)
                {
                    if (d.Equals(ModelItems[i].View))
                    {
                        ModelItems.RemoveAt(i);
                    }
                }
                lvModel.Items.Refresh();
            };

        }
        private void LvModel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = lvModel.SelectedValue as ModelItem;
            if (item == null) return;
            imgContainer.RectangleViews.ForEach(x => x.Selected = false);
            item.View.Selected = true;
            //Keyboard.Focus(this);
            Keyboard.Focus(item.View);
            Console.WriteLine("LvModel_SelectionChanged");
        }
        private void MiDelModelItem_Click(object sender, RoutedEventArgs e)
        {
            var item = lvModel.SelectedValue as ModelItem;
            imgContainer.DelRectangle(item.View);
        }



        private void lvModel_MouseMove(object sender, MouseEventArgs e)
        {
            ListView listview = sender as ListView;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                System.Collections.IList list = listview.SelectedItems as System.Collections.IList;
                DataObject data = new DataObject(typeof(System.Collections.IList), list);
                if (list.Count > 0)
                {
                    DragDrop.DoDragDrop(listview, data, DragDropEffects.Move);
                }
            }
        }

        private void lvModel_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(System.Collections.IList)))
            {
                System.Collections.IList peopleList = e.Data.GetData(typeof(System.Collections.IList)) as System.Collections.IList;
                //index為放置時鼠標下元素項的索引
                int index = GetCurrentIndex(new GetPositionDelegate(e.GetPosition));
                if (index > -1)
                {
                    ModelItem Logmess = peopleList[0] as ModelItem;
                    //拖動元素集合的第一個元素索引
                    int OldFirstIndex = ModelItems.IndexOf(Logmess);
                    //下邊那個循環要求數據源必須為ObservableCollection<T>類型，T為對象
                    for (int i = 0; i < peopleList.Count; i++)
                    {
                        ModelItems.RemoveAt(OldFirstIndex);
                        ModelItems.Insert(index, Logmess);
                        imgContainer.RectangleViews.RemoveAt(OldFirstIndex);
                        imgContainer.RectangleViews.Insert(index, Logmess.View);

                    }
                    imgContainer.RefreshRectangleZIndex();
                    lvModel.SelectedItems.Clear();
                    lvModel.Items.Refresh();
                }
            }
        }

        private int GetCurrentIndex(GetPositionDelegate getPosition)
        {
            int index = -1;
            for (int i = 0; i < lvModel.Items.Count; ++i)
            {
                ListViewItem item = GetListViewItem(i);
                if (item != null && this.IsMouseOverTarget(item, getPosition))
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private bool IsMouseOverTarget(Visual target, GetPositionDelegate getPosition)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(target);
            System.Windows.Point mousePos = getPosition((IInputElement)target);
            return bounds.Contains(mousePos);
        }

        delegate System.Windows.Point GetPositionDelegate(IInputElement element);

        ListViewItem GetListViewItem(int index)
        {
            if (lvModel.ItemContainerGenerator.Status != GeneratorStatus.ContainersGenerated)
                return null;
            return lvModel.ItemContainerGenerator.ContainerFromIndex(index) as ListViewItem;
        }




        #endregion



        #region 设备列表检测
        Timer deviceTimer;
        private OutEffectsView outEffectsView = new OutEffectsView();
        private List<System.Windows.Forms.Screen> otherScreens = new List<System.Windows.Forms.Screen>();
        private List<ScreenWindow> screenWindows = new List<ScreenWindow>();
        private System.Windows.Forms.Screen primaryScreen;
        private List<TextListViewItem> deviceNameList = new List<TextListViewItem>();
        private Dictionary<string, Window> deviceWindow = new Dictionary<string, Window>();
        /// <summary>
        /// 初始化显示设备
        /// </summary>
        private void InitDevices()
        {
            deviceNameList.Clear();
            otherScreens.Clear();
            //otherScreens = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).ToList();
            var ss = System.Windows.Forms.Screen.AllScreens.ToList();
            ss.ForEach(x => { if (x.Primary) primaryScreen = x; else otherScreens.Add(x); });
            //otherScreens = System.Windows.Forms.Screen.AllScreens.ToList();
            for (int i = 0; i < otherScreens.Count; i++)
            {

                var si = otherScreens[i];
                deviceNameList.Add(new TextListViewItem(si.DeviceName, t => (s, e) =>
                {

                    if (t.lText.Content.Equals(si.DeviceName))
                    {
                        var w = screenWindows.Find(x => x.Screen.Equals(si));
                        if (w == null)
                        {

                            var workingArea = si.WorkingArea;
                            //otw.Top = (primaryScreen.WorkingArea.Height - workingArea.Height) / 2;
                            //Console.WriteLine($"Dpi:{ScreenTool.GetDpiFromVisual(this).X},{ScreenTool.GetDpiFromVisual(this).Y}");
                            var otw = new OutEffectsWindow(new WinLocation(ScreenTool.GetRealSize(this, workingArea.Left), 0, workingArea.Width, workingArea.Height));
                            w = new ScreenWindow { Screen = si, Window = otw };
                            screenWindows.Add(w);
                            otw.Show();
                        }
                        var oevList = new List<OutEffectsView>();
                        foreach (UIElement item in imgContainer.canvas.Children)
                        {
                            var rv = item as RectangleView;
                            if (rv == null) continue;
                            var outEffectsView = new OutEffectsView();
                            outEffectsView.gContainer.Width = rv.gData.ActualWidth;
                            outEffectsView.gContainer.Height = rv.gData.ActualHeight;
                            outEffectsView.img.Source = rv.img.Source;
                            //outEffectsView.img.Width = rv.img.ActualWidth;
                            //outEffectsView.img.Height = rv.img.Height;
                            outEffectsView.IsVideo = rv.IsVideo;
                            outEffectsView.video.Source = rv.video.Source;
                            //outEffectsView.video.Width = rv.video.ActualWidth;
                            //outEffectsView.video.Height = rv.video.Height;
                            outEffectsView.SetValue(Canvas.LeftProperty, rv.GetValue(Canvas.LeftProperty));
                            outEffectsView.SetValue(Canvas.TopProperty, rv.GetValue(Canvas.TopProperty));
                            oevList.Add(outEffectsView);
                        }
                        //outEffectsView.canvas

                        w.Window.Refresh(oevList);
                        //if (otw.IsLoaded)
                        //    otw.WindowState = WindowState.Maximized;
                    }
                }));
            }
            lvDevice.ItemsSource = deviceNameList;

        }
        private void MiCheckModelItem_Click(object sender, RoutedEventArgs e)
        {
            //var tlvi = lvDevice.SelectedValue as TextListViewItem;
            var index = lvDevice.SelectedIndex;
            if (screenWindows.Count > index)
                screenWindows[index].Window.Background = new SolidColorBrush(Colors.Red);
        }
        #endregion



        #region 左右两侧折叠与缩放
        /// <summary>
        /// Grid窗口属性
        /// </summary>
        GridLength m_WidthCache1, m_WidthCache2;
        int grid1Index = 0, grid2Index = 4;

        public void BtnGrdSplitter_Click(object sender, RoutedEventArgs e)
        {
            GridSplitterClick(ref m_WidthCache1, grid1Index);
        }
        private void BtnGrdSplitter2_Click(object sender, RoutedEventArgs e)
        {
            GridSplitterClick(ref m_WidthCache2, grid2Index);
        }
        /// <summary>
        /// Grid分割器点击
        /// </summary>
        /// <param name="w"></param>
        /// <param name="grid1Index"></param>
        private void GridSplitterClick(ref GridLength w, int grid1Index)
        {
            GridLength temp = grdWorkbench.ColumnDefinitions[grid1Index].Width;
            GridLength def = new GridLength(0);
            if (temp.Equals(def))
            {
                //恢复
                grdWorkbench.ColumnDefinitions[grid1Index].Width = w;
            }
            else
            {
                //折叠
                w = grdWorkbench.ColumnDefinitions[grid1Index].Width;
                grdWorkbench.ColumnDefinitions[grid1Index].Width = def;
            }
        }
        #endregion

        #region 特效部分操作
        /// <summary>
        /// 特效数据
        /// </summary>
        private MaterialJsonModel materialJsonModel;
        /// <summary>
        /// 初始化特效
        /// </summary>
        private void InitEffects()
        {

            if (File.Exists(MaterialInputWindow.ResourcesMaterialPath))
            {

                try
                {
                    var json = File.ReadAllText(MaterialInputWindow.ResourcesMaterialPath);
                    if (json.Trim().Length > 0)
                    {
                        materialJsonModel = JsonConvert.DeserializeObject<MaterialJsonModel>(json);
                        if (materialJsonModel != null && materialJsonModel.TagModels != null && materialJsonModel.MaterialModels != null)
                        {
                            currentMaterialModels = materialJsonModel.MaterialModels;
                            InitAllEffectsItem(materialJsonModel.MaterialModels);
                            var list1 = new List<string>();
                            materialJsonModel.TagModels.ForEach(x => list1.Add(x.Name));
                            cbEffectsType1.ItemsSource = list1;
                            cbEffectsType1.SelectedIndex = 0;
                            InitEffectsType(0);
                            cbEffectsType2.SelectedIndex = 0;
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

            }
        }
        /// <summary>
        /// 初始化所有特效
        /// </summary>
        /// <param name="materials"></param>
        private void InitAllEffectsItem(List<MaterialModel> materials)
        {
            var a3 = new List<EffectsListViewItem>();
            materials.ForEach(x =>
            {
                a3.Add(new EffectsListViewItem(this, x.Id, x.Tag1.Name, x.Tag2.Name, x.Title, x.Content, x.ResourceFileName));
            });
            lvEffects.ItemsSource = a3;
        }
        /// <summary>
        /// 初始化特效标签
        /// </summary>
        /// <param name="v"></param>
        private void InitEffectsType(int v)
        {

            var list2 = new List<string>();
            materialJsonModel.TagModels[v == -1 ? 0 : v].Tag2.ForEach(x => list2.Add(x.Name));
            cbEffectsType2.ItemsSource = list2;
        }
        /// <summary>
        /// 当前筛选后的特效数据
        /// </summary>
        private List<MaterialModel> currentMaterialModels;

        /// <summary>
        /// 清除搜索空内容点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ICleanSearchText_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            tbSearch.Text = "";
        }
        /// <summary>
        /// 导入特效选项点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = MaterialInputWindow.OpenFilter;
            openFileDialog.Title = "请选择需要导入的视频或者图片";
            openFileDialog.Multiselect = false;

            try
            {
                if ((bool)openFileDialog.ShowDialog())
                {
                    var miw =
                    new MaterialInputWindow(openFileDialog.FileName);
                    miw.Closed += MaterialInput_Closed;
                    miw.ShowDialog();

                }
            }
            catch (Exception)
            {

            }
        }
        /// <summary>
        /// 导入特效窗口关闭后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaterialInput_Closed(object sender, EventArgs e)
        {
            InitEffects();
        }
        /// <summary>
        /// 特效标签1选项更变后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbEffectsType1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitEffectsType(cbEffectsType1.SelectedIndex);
            CbEffectsTypeChanged();
            if (cbEffectsType2.SelectedIndex != 0)
            {
                cbEffectsType2.SelectedIndex = 0;
            }
        }
        /// <summary>
        /// 特效标签2选项更变后
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbEffectsType2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CbEffectsTypeChanged();

        }
        /// <summary>
        /// 类型下拉框改变后，重置特效内容
        /// </summary>
        private void CbEffectsTypeChanged()
        {
            try
            {
                var t1 = cbEffectsType1.SelectedValue.ToString();
                var t2 = cbEffectsType2.SelectedValue.ToString();
                var index1 = cbEffectsType1.SelectedIndex;
                var index2 = cbEffectsType2.SelectedIndex;
                if (materialJsonModel != null && materialJsonModel.MaterialModels != null)
                {
                    if (index1 == 0)
                    {
                        currentMaterialModels = materialJsonModel.MaterialModels;
                        InitAllEffectsItem(materialJsonModel.MaterialModels);
                        cbEffectsType1.SelectedIndex = 0;
                        cbEffectsType2.SelectedIndex = 0;
                        if (tbSearch.Text.Trim().Length != 0) TbSearch_TextChanged(null, null);
                    }
                    else
                    {
                        var findList = materialJsonModel.MaterialModels.FindAll(x => index2 == 0 ? x.Tag1.Name.Equals(t1) : x.Tag1.Name.Equals(t1) && x.Tag2.Name.Equals(t2)).ToList();
                        currentMaterialModels = findList;
                        InitAllEffectsItem(findList);
                        if (index2 == 0)
                        {
                            cbEffectsType2.SelectedIndex = 0;
                        }
                        if (tbSearch.Text.Trim().Length != 0) TbSearch_TextChanged(null, null);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {


        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var moveType = MoveType.Left;
            if (Keyboard.Modifiers == ModifierKeys.Alt)
            {
                var rotateType = RotateType.Clockwise;
                if (e.KeyStates == Keyboard.GetKeyStates(Key.Right))
                {
                    imgContainer?.RectangleViews?.Find(x => x.Selected)?.RotateWithKey(1, rotateType);
                }
                else if (e.KeyStates == Keyboard.GetKeyStates(Key.Left))
                {
                    rotateType = RotateType.Anticlockwise;
                    imgContainer?.RectangleViews?.Find(x => x.Selected)?.RotateWithKey(1, rotateType);
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.Up:
                        moveType = MoveType.Top;
                        break;
                    case Key.Right:
                        moveType = MoveType.Right;
                        break;
                    case Key.Down:
                        moveType = MoveType.Bottom;
                        break;
                    default:
                        break;
                }
                imgContainer?.RectangleViews?.Find(x => x.Selected)?.MoveWithKey(1, moveType);
            }
        }

        private void Window_PreviewKeyDown_1(object sender, KeyEventArgs e)
        {
            Window_PreviewKeyDown(sender, e);
            e.Handled = true;
        }

        /// <summary>
        /// 搜索框文字改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var sText = tbSearch.Text;
            var list = currentMaterialModels.FindAll(x => x.Title.Contains(sText));
            InitAllEffectsItem(list);

        }
        #endregion


        ///create by alair
        ///
        public Bitmap FloodFill(Bitmap src, System.Drawing.Point location, System.Drawing.Color fillColor, int threshould)
        {
            try
            {
                Bitmap srcbmp = src;
                Bitmap dstbmp = new Bitmap(src.Width, src.Height);
                int w = srcbmp.Width;
                int h = srcbmp.Height;
                Stack<System.Drawing.Point> fillPoints = new Stack<System.Drawing.Point>(w * h);
                System.Drawing.Imaging.BitmapData bmpData = srcbmp.LockBits(new System.Drawing.Rectangle(0, 0, srcbmp.Width, srcbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Imaging.BitmapData dstbmpData = dstbmp.LockBits(new System.Drawing.Rectangle(0, 0, dstbmp.Width, dstbmp.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                IntPtr ptr = bmpData.Scan0;
                int stride = bmpData.Stride;
                int bytes = bmpData.Stride * srcbmp.Height;
                byte[] grayValues = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(ptr, grayValues, 0, bytes);
                System.Drawing.Color backColor = System.Drawing.Color.FromArgb(grayValues[location.X * 3 + 2 + location.Y * stride], grayValues[location.X * 3 + 1 + location.Y * stride], grayValues[location.X * 3 + location.Y * stride]);

                IntPtr dstptr = dstbmpData.Scan0;
                byte[] temp = new byte[bytes];
                System.Runtime.InteropServices.Marshal.Copy(dstptr, temp, 0, bytes);

                int gray = (int)((backColor.R + backColor.G + backColor.B) / 3);
                if (location.X < 0 || location.X >= w || location.Y < 0 || location.Y >= h) return null;
                fillPoints.Push(new System.Drawing.Point(location.X, location.Y));
                int[,] mask = new int[w, h];

                while (fillPoints.Count > 0)
                {

                    System.Drawing.Point p = fillPoints.Pop();
                    mask[p.X, p.Y] = 1;
                    temp[3 * p.X + p.Y * stride] = (byte)fillColor.B;
                    temp[3 * p.X + 1 + p.Y * stride] = (byte)fillColor.G;
                    temp[3 * p.X + 2 + p.Y * stride] = (byte)fillColor.R;
                    if (p.X > 0 && (Math.Abs(gray - (int)((grayValues[3 * (p.X - 1) + p.Y * stride] + grayValues[3 * (p.X - 1) + 1 + p.Y * stride] + grayValues[3 * (p.X - 1) + 2 + p.Y * stride]) / 3)) < threshould) && (mask[p.X - 1, p.Y] != 1))
                    {
                        temp[3 * (p.X - 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X - 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X - 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X - 1, p.Y));
                        mask[p.X - 1, p.Y] = 1;
                    }
                    if (p.X < w - 1 && (Math.Abs(gray - (int)((grayValues[3 * (p.X + 1) + p.Y * stride] + grayValues[3 * (p.X + 1) + 1 + p.Y * stride] + grayValues[3 * (p.X + 1) + 2 + p.Y * stride]) / 3)) < threshould) && (mask[p.X + 1, p.Y] != 1))
                    {
                        temp[3 * (p.X + 1) + p.Y * stride] = (byte)fillColor.B;
                        temp[3 * (p.X + 1) + 1 + p.Y * stride] = (byte)fillColor.G;
                        temp[3 * (p.X + 1) + 2 + p.Y * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X + 1, p.Y));
                        mask[p.X + 1, p.Y] = 1;
                    }
                    if (p.Y > 0 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y - 1) * stride] + grayValues[3 * p.X + 1 + (p.Y - 1) * stride] + grayValues[3 * p.X + 2 + (p.Y - 1) * stride]) / 3)) < threshould) && (mask[p.X, p.Y - 1] != 1))
                    {
                        temp[3 * p.X + (p.Y - 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y - 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y - 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X, p.Y - 1));
                        mask[p.X, p.Y - 1] = 1;
                    }
                    if (p.Y < h - 1 && (Math.Abs(gray - (int)((grayValues[3 * p.X + (p.Y + 1) * stride] + grayValues[3 * p.X + 1 + (p.Y + 1) * stride] + grayValues[3 * p.X + 2 + (p.Y + 1) * stride]) / 3)) < threshould) && (mask[p.X, p.Y + 1] != 1))
                    {
                        temp[3 * p.X + (p.Y + 1) * stride] = (byte)fillColor.B;
                        temp[3 * p.X + 1 + (p.Y + 1) * stride] = (byte)fillColor.G;
                        temp[3 * p.X + 2 + (p.Y + 1) * stride] = (byte)fillColor.R;
                        fillPoints.Push(new System.Drawing.Point(p.X, p.Y + 1));
                        mask[p.X, p.Y + 1] = 1;
                    }
                }
                fillPoints.Clear();

                System.Runtime.InteropServices.Marshal.Copy(temp, 0, dstptr, bytes);
                srcbmp.UnlockBits(bmpData);
                dstbmp.UnlockBits(dstbmpData);

                return dstbmp;
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
                return null;
            }
        }


    }


}
