using System;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System.Linq;
using System.Windows.Data;
using tablegen2.logic;
using System.Collections.Generic;

namespace tablegen2.layouts
{
    /// <summary>
    /// TreeListView.xaml 的交互逻辑
    /// </summary>
    public partial class TreeListView : UserControl
    {
        private TreeListItem selectedItem_ = null;
        public event Action SelectedChangedEvent;
        public event Action CreateExcelEvent;

        public TreeListView()
        {
            InitializeComponent();
        }

        public TreeListItem SelectedItem
        {
            get { return selectedItem_; }
            set { _selectItem(value); }
        }

        public IEnumerable<string> AllExcels
        {
            get { return wp.Children.Cast<TreeListItem>().Select(a => a.FullPath); }
        }

        public void refreshByDir(string dir)
        {
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                _selectItem(null);
                wp.Children.Clear();
                spEmpty.Visibility = Visibility.Visible;
            }
            else
            {
                _selectItem(null);
                wp.Children.Clear();

                var files = Directory.GetFiles(dir, "*.xls", SearchOption.TopDirectoryOnly).ToList();
                files = files.Union(Directory.GetFiles(dir, "*.xlsx", SearchOption.TopDirectoryOnly)).ToList();
                files.Sort((a, b) => string.Compare(Path.GetFileNameWithoutExtension(a), Path.GetFileNameWithoutExtension(b), true));

                spEmpty.Visibility = files.Count == 0 ? Visibility.Visible : Visibility.Hidden;
                foreach (var fullPath in files)
                {
                    if (Path.GetFileName(fullPath).StartsWith("~$"))
                        continue;

                    var item = _createItem(fullPath);
                    wp.Children.Add(item);
                }
            }
        }

        public void selectItemByFullPath(string fullPath)
        {
            _selectItem(wp.Children.Cast<TreeListItem>().FirstOrDefault(a => string.Compare(a.FullPath, fullPath, true) == 0));
        }

        private void sv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectItem(null);
        }

        private void sv_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectItem(null);
            _flipMenuWithBackground();
        }

        private void btnCreateExcel_Clicked(object sender, RoutedEventArgs e)
        {
            if (CreateExcelEvent != null)
                CreateExcelEvent.Invoke();
        }

        #region 辅助函数
        private void _selectItem(TreeListItem item)
        {
            if (selectedItem_ != item)
            {
                if (selectedItem_ != null)
                    selectedItem_.IsSelected = false;
                selectedItem_ = item;
                if (selectedItem_ != null)
                    selectedItem_.IsSelected = true;
                if (SelectedChangedEvent != null)
                    SelectedChangedEvent.Invoke();
            }
            if (selectedItem_ != null)
                sv.MakesureChildVisible(selectedItem_);
        }
        private TreeListItem _createItem(string fullPath)
        {
            var item = new TreeListItem();
            item.setTreeItemFile(fullPath);
            var binding = new Binding("ActualWidth") { Source = wp };
            item.SetBinding(FrameworkElement.WidthProperty, binding);
            item.MouseLeftButtonDown += (_s, _e) =>
            {
                _e.Handled = true;
                _selectItem(_s as TreeListItem);
            };
            item.MouseRightButtonDown += (_s, _e) =>
            {
                _e.Handled = true;
                _selectItem(_s as TreeListItem);
                _flipMenuWithItem(_s as TreeListItem);
            };
            item.MouseDoubleClick += (_s, _e) =>
            {
                if (_e.ChangedButton == MouseButton.Left)
                {
                    _e.Handled = true;
                    _performListItem(_s as TreeListItem);
                }
            };
            return item;
        }
        private void _flipMenuWithItem(TreeListItem item)
        {
            var menu = new ContextMenu();

            var miOpen = new MenuItem();
            miOpen.Header = "打开Excel(_O)";
            miOpen.FontWeight = FontWeights.Bold;
            miOpen.Click += (_s, _e) => System.Diagnostics.Process.Start(item.FullPath);
            menu.Items.Add(miOpen);

            var miEdit = new MenuItem();
            miEdit.Header = "编辑Excel(_E)";
            miEdit.Click += (_s, _e) => AppData.MainWindow.editForExcel(item.FullPath);
            menu.Items.Add(miEdit);
            menu.Items.Add(new Separator());

            //导出菜单选项
            {
                string menuText = string.Empty;
                switch (AppData.Config.ExportFormat)
                {
                    case TableExportFormat.Dat:
                        menuText = "导出Dat数据";
                        break;
                    case TableExportFormat.Json:
                        menuText = "导出Json数据";
                        break;
                    case TableExportFormat.Xml:
                        menuText = "导出Xml数据";
                        break;
                    case TableExportFormat.Lua:
                        menuText = "导出Lua数据";
                        break;
                }
                if (!string.IsNullOrEmpty(menuText))
                {
                    var miExport = new MenuItem();
                    miExport.Header = menuText;
                    miExport.Click += (_s, _e) => AppData.MainWindow.genSingleFile(item.FullPath, AppData.Config.ExcelDir, AppData.Config.ExportFormat);
                    menu.Items.Add(miExport);
                }
            }

            //美化Excel格式
            var miRectify = new MenuItem();
            miRectify.Header = "美化Excel表";
            miRectify.Click += (_s, _e) => AppData.MainWindow.rectifyFileFormat(item.FullPath);
            menu.Items.Add(miRectify);
            menu.Items.Add(new Separator());

            var miExplorer = new MenuItem();
            miExplorer.Header = "打开所在文件夹";
            miExplorer.Click += (_s, _e) => Util.OpenDirFile(item.FullPath);
            menu.Items.Add(miExplorer);

            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            menu.IsOpen = true;
        }
        private void _performListItem(TreeListItem item)
        {
            System.Diagnostics.Process.Start(item.FullPath);
        }
        private void _flipMenuWithBackground()
        {
            var menu = new ContextMenu();

            var miRefresh = new MenuItem();
            miRefresh.Header = "刷新(_E)";
            miRefresh.Click += (_s, _e) => refreshByDir(AppData.Config.ExcelDir);
            menu.Items.Add(miRefresh);

            var miCreate = new MenuItem();
            miCreate.Header = "创建Excel表...";
            miCreate.Click += (_s, _e) =>
            {
                if (CreateExcelEvent != null)
                    CreateExcelEvent.Invoke();
            };
            menu.Items.Add(miCreate);
            menu.Items.Add(new Separator());

            var miExcelFormat = new MenuItem();
            miExcelFormat.Header = "一键美化Excel表";
            miExcelFormat.Click += (_s, _e) => AppData.MainWindow.rectifyAllFileFormat(AllExcels.ToList());
            menu.Items.Add(miExcelFormat);
            menu.Items.Add(new Separator());

            var miExplorer = new MenuItem();
            miExplorer.Header = "打开所在文件夹";
            miExplorer.Click += (_s, _e) => Util.OpenDir(AppData.Config.ExcelDir);
            menu.Items.Add(miExplorer);

            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            menu.IsOpen = true;
        }
        #endregion
    }
}
