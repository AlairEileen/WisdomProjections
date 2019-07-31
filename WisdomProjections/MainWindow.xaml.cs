using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using WisdomProjections.Data_Executor;
using WisdomProjections.Models;
using WisdomProjections.Views;
using WisdomProjections.Views.Sys;

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
            if (ApplicationInfoContext.IsIpCamera)
            {
                IpCameraDataExecutor.IpCameraDe.InitContent(imgContainer);
                Closing += IpCameraDataExecutor.IpCameraDe.Close;
            }
            else
            {
                SensorDataExecutor.SensorDe.InitContent(imgContainer);
                Closing += SensorDataExecutor.SensorDe.Close;
            }
            Closing += MainWindow_Closing;
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
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
                btnGrdSplitter.Click += BtnGrdSplitter_Click;
            if (btnGrdSplitter2 != null)
                btnGrdSplitter2.Click += BtnGrdSplitter2_Click;
            InitData();
            InitDevices();
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
            InitDisplay();
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
                    var ic = imgContainer.PaintTypeSelects.FirstOrDefault(x => x.PaintType == PaintType.None);
                    if (ic != null)
                    {
                        ic.IsSelected = true;
                        ic.ChangeCursor(this);
                    }
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
            SwitchPaintType(PaintType.Cricle);
        }
        /// <summary>
        /// 笔点击
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ImgPen_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SwitchPaintType(PaintType.Pen);
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
            Keyboard.Focus(item.View.Data);
            //Console.WriteLine("LvModel_SelectionChanged");
        }
        private void MiDelModelItem_Click(object sender, RoutedEventArgs e)
        {
            var item = lvModel.SelectedValue as ModelItem;
            imgContainer.DelRectangle(item?.View);
        }



        private void lvModel_MouseMove(object sender, MouseEventArgs e)
        {
            ListView listView = sender as ListView;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (listView != null)
                {
                    System.Collections.IList list = listView.SelectedItems;
                    DataObject data = new DataObject(typeof(System.Collections.IList), list);
                    if (list.Count > 0)
                    {
                        DragDrop.DoDragDrop(listView, data, DragDropEffects.Move);
                    }
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
                    if (peopleList != null)
                    {
                        ModelItem modelItem = peopleList[0] as ModelItem;
                        if (modelItem == null) return;
                        //拖動元素集合的第一個元素索引
                        int oldFirstIndex = ModelItems.IndexOf(modelItem);
                        //下邊那個循環要求數據源必須為ObservableCollection<T>類型，T為對象
                        for (int i = 0; i < peopleList.Count; i++)
                        {
                            ModelItems.RemoveAt(oldFirstIndex);
                            ModelItems.Insert(index, modelItem);
                            imgContainer.RectangleViews.RemoveAt(oldFirstIndex);
                            imgContainer.RectangleViews.Insert(index, modelItem.View);

                        }
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







        #region 左右两侧折叠与缩放
        /// <summary>
        /// Grid窗口属性
        /// </summary>
        GridLength mWidthCache1, mWidthCache2;
        int grid1Index = 0, grid2Index = 4;

        public void BtnGrdSplitter_Click(object sender, RoutedEventArgs e)
        {
            GridSplitterClick(ref mWidthCache1, grid1Index);
        }
        private void BtnGrdSplitter2_Click(object sender, RoutedEventArgs e)
        {
            GridSplitterClick(ref mWidthCache2, grid2Index);
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
                if (materialJsonModel?.MaterialModels != null)
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
                // ignored
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
                if (e.KeyStates == Keyboard.GetKeyStates(Key.Right))
                {
                    imgContainer?.RectangleViews?.Find(x => x.Selected)?.RotateWithKey(1);
                }
                else if (e.KeyStates == Keyboard.GetKeyStates(Key.Left))
                {
                    imgContainer?.RectangleViews?.Find(x => x.Selected)?.RotateWithKey(-1);
                }
                else if (e.KeyStates == Keyboard.GetKeyStates(Key.P))
                {
                    imgContainer?.TakePhotos();
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


        #region 设备模块
        Timer deviceTimer;
        private int deviceFoundIndex;
        /// <summary>
        /// 初始化显示设备
        /// </summary>
        private void InitDevices()
        {
            //otherScreens = System.Windows.Forms.Screen.AllScreens.Where(s => !s.Primary).ToList();
            deviceTimer = new Timer(o =>
            {
                Application.Current?.Dispatcher?.Invoke(() =>
              {
                  var ss = System.Windows.Forms.Screen.AllScreens.ToList();
                  ss.ForEach(x =>
                  {
                      if (!x.Primary && deviceModels.Find(y => y.Screen.Equals(x)) == null)
                      {
                          deviceModels.Add(new DeviceModel { Screen = x, Name = $"设备{++deviceFoundIndex}" });
                      }
                  });
                  deviceModels?.ForEach(y =>
                  {
                      if (ss.Find(x => x.Equals(y.Screen)) == null)
                      {
                          y.Window?.Close();
                          deviceModels.Remove(y);
                      }
                  });
                  lvDevice.Items.Refresh();
              });
            }, null, 50, 500);
            lvDevice.ItemsSource = deviceModels;
        }
        private readonly List<DeviceModel> deviceModels = new List<DeviceModel>();
        private void LvDevice_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshWindow();

        }

        public void RefreshWindow()
        {

            var dm = lvDevice.SelectedItem as DeviceModel;
            if (dm == null) return;
            dm.Window = dm.Window ?? CreateScreenWindowItem(dm.Screen);
            //var oevList = new List<OutEffectsView>();
            //foreach (UIElement item in imgContainer.canvas.Children)
            //{
            //    var rv = item as RectangleView;
            //    if (rv == null) continue;
            //    var outEffectsView = new OutEffectsView
            //    {
            //        Width = rv.bContent.ActualWidth,
            //        Height = rv.bContent.ActualHeight,
            //        img = {Source = rv.img.Source},
            //        IsVideo = rv.IsVideo,
            //        video = {Source = rv.video.Source}
            //    };
            //    //outEffectsView.img.Width = rv.img.ActualWidth;
            //    //outEffectsView.img.Height = rv.img.Height;
            //    //outEffectsView.video.Width = rv.video.ActualWidth;
            //    //outEffectsView.video.Height = rv.video.Height;
            //    var p = rv.bContent.TranslatePoint(new System.Windows.Point(), imgContainer.canvas);
            //    outEffectsView.SetValue(Canvas.LeftProperty, p.X);
            //    outEffectsView.SetValue(Canvas.TopProperty, p.Y);
            //    oevList.Add(outEffectsView);
            //}

            var newRvList = new List<BaseBlob>();

            imgContainer.RectangleViews.ForEach(x=>newRvList.Add(x.Clone() as BaseBlob));

            //outEffectsView.canvas
            //dm.Window.Refresh(oevList, imgContainer.BSDSize, imgContainer.bSelectedDisplay.ActualWidth, imgContainer.bSelectedDisplay.ActualHeight);
            dm.Window.Refresh(newRvList, imgContainer.bSelectedDisplay.ActualWidth, imgContainer.bSelectedDisplay.ActualHeight);
        }

        private OutEffectsWindow CreateScreenWindowItem(System.Windows.Forms.Screen screen)
        {
            var workingArea = screen.WorkingArea;
            //otw.Top = (primaryScreen.WorkingArea.Height - workingArea.Height) / 2;
            //Console.WriteLine($"Dpi:{ScreenTool.GetDpiFromVisual(this).X},{ScreenTool.GetDpiFromVisual(this).Y}");
            var otw = new OutEffectsWindow(new WinLocation(ScreenTool.GetRealSize(this, workingArea.Left), 0, workingArea.Width, workingArea.Height));
            otw.Show();
            return otw;
        }
        #endregion



        #region 调校模块

        private int[][] dpScale = { new[] { 16, 10 }, new[] { 16, 9 }, new[] { 4, 3 } };
        private string[] launchModeList = { "正投", "背投", "地装", "吊装" };
        private SolidColorBrush selectedDisplayColor = new SolidColorBrush(Colors.Red);
        protected SolidColorBrush SelectedDisplayColor
        {
            get => selectedDisplayColor;
            private set
            {
                selectedDisplayColor = value;
                RefreshDMWindow();
            }
        }

        /// <summary>
        /// 初始化投影机分辨率
        /// </summary>
        private void InitDisplay()
        {
            var list = new List<string>();
            foreach (var item in dpScale)
            {
                list.Add($"{item[0]} : {item[1]}");
            }
            cbDisplay.ItemsSource = list;
            cbDisplay.SelectedIndex = 0;
            cbLaunchMode.ItemsSource = launchModeList;
            cbLaunchMode.SelectedIndex = 0;
        }
        /// <summary>
        /// 投影仪分辨率改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbDisplay_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var index = cbDisplay.SelectedIndex;
            //imgContainer.BSDSize = (double)dpScale[index][0] / dpScale[index][1];
            //Console.WriteLine($"sd= w:{imgContainer.bSelectedDisplay.Width},h:{imgContainer.bSelectedDisplay.Height};gsd= w:{imgContainer.gSD.ActualWidth},h:{imgContainer.gSD.ActualHeight}");
            //imgContainer.bSelectedDisplay.Height = imgContainer.bSelectedDisplay.Width / dpScale[index][0] * dpScale[index][1];
        }
        private void CbDebug_Checked(object sender, RoutedEventArgs e)
        {
            if (lvDevice.SelectedItem == null)
            {
                MessageBox.Show("请选择要调校的设备!");
                cbDebug.IsChecked = false;
            }
            else
            {
                RefreshDMWindow();
            }
        }

        private void RefreshDMWindow()
        {
            var dm = lvDevice.SelectedItem as DeviceModel;
            if (dm == null) return;
            if (dm.Window == null)
            {
                dm.Window = CreateScreenWindowItem(dm.Screen);
            }
            dm.Window.Background = SelectedDisplayColor;
        }

        private void CbDC1_OnChecked(object sender, RoutedEventArgs e)
        {
            var border = (Border)VisualTreeHelper.GetParent(sender as RadioButton);
            SelectedDisplayColor = (SolidColorBrush)border.Background.Clone();
        }
        #endregion



      

        private void SliderPosition_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SliderPositionX != null && SliderPositionY != null && SliderPositionWidth != null && SliderPositionHeight != null)
            {
                var x = SliderPositionX.Value;
                var y = SliderPositionY.Value;
                var w = SliderPositionWidth.Value;
                var h = SliderPositionHeight.Value;
                imgContainer.RefreshPosition(x, y, w, h);
            }

        }
    }


}
