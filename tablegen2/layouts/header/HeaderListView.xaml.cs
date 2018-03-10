using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using tablegen2.logic;

namespace tablegen2.layouts
{
    /// <summary>
    /// HeaderListView.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderListView : UserControl
    {
        private List<HeaderListItem> items_ = new List<HeaderListItem>();
        private HeaderListItem selectedItem_ = null;

        public event Action<HeaderListItem> SelectedChangedEvent;
        public event Action<HeaderListItem> AddCopyEvent;
        public event Action<HeaderListItem> EditEvent;
        public event Action<HeaderListItem> DeleteEvent;

        public HeaderListView()
        {
            InitializeComponent();
            _refreshDisplayTip();
            wp.SizeChanged += (_s, _e) => _refreshListViewTip();
        }

        public IEnumerable<HeaderListItem> Items
        {
            get { return items_; }
        }

        public HeaderListItem SelectedItem
        {
            get { return selectedItem_; }
            set { _selectItem(value); }
        }

        public IEnumerable<TableExcelHeader> HeaderItems
        {
            get
            {
                return items_.Select(a => new TableExcelHeader()
                {
                    FieldName = a.FieldName,
                    FieldType = a.FieldType,
                    FieldDesc = a.FieldDesc,
                });
            }
        }

        public bool isFieldNameExist(string fieldName)
        {
            return items_.Find(a => a.FieldName == fieldName) != null;
        }

        public void refreshDisplay(List<TableExcelHeader> headers)
        {
            _selectItem(null);
            items_.Clear();
            wp.Children.Clear();
            foreach (var header in headers)
            {
                var item = _createItem(header);
                var idx = items_.Count;
                items_.Insert(idx, item);
                wp.Children.Insert(idx, item);
            }
            _refreshDisplayTip();
        }

        public void addItem(TableExcelHeader header, bool isSelected)
        {
            var item = _createItem(header);
            var idx = items_.Count;
            items_.Insert(idx, item);
            wp.Children.Insert(idx, item);
            _refreshDisplayTip();

            if (isSelected)
                SelectedItem = item;
        }

        public void removeItem(HeaderListItem item)
        {
            if (item == selectedItem_)
                _selectItem(null);
            items_.Remove(item);
            wp.Children.Remove(item);
            _refreshDisplayTip();
        }

        public void moveUp(HeaderListItem item)
        {
            int idx = items_.IndexOf(item);
            if (idx < 3)
                return;

            var tmpItem = items_[idx - 1];
            items_[idx - 1] = item;
            items_[idx] = tmpItem;
            wp.Children.RemoveAt(idx - 1);
            wp.Children.Insert(idx, tmpItem);
            sv.MakesureChildVisible(item);
        }

        public void moveDown(HeaderListItem item)
        {
            int idx = items_.IndexOf(item);
            if (idx < 2 || idx == items_.Count - 1)
                return;

            var tmpItem = items_[idx + 1];
            items_[idx + 1] = item;
            items_[idx] = tmpItem;
            wp.Children.RemoveAt(idx + 1);
            wp.Children.Insert(idx, tmpItem);
            sv.MakesureChildVisible(item);
        }

        #region 事件
        private void sv_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectItem(null);
        }
        private void sv_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _selectItem(null);
            _flipMenuWithBackground();
        }
        private void sv_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _refreshListViewTip();
        }
        #endregion

        #region 辅助函数
        private void _refreshDisplayTip()
        {
            if (items_.Count > 0)
            {
                txtEmpty.Visibility = Visibility.Hidden;
                sv.Visibility = Visibility.Visible;
            }
            else
            {
                txtEmpty.Visibility = Visibility.Visible;
                sv.Visibility = Visibility.Hidden;
            }
        }
        private void _refreshListViewTip()
        {
            if (sv.ExtentHeight > sv.ActualHeight + 30)
                top.MaxHeight = 100;
            else
                top.MaxHeight = 0;
        }
        private void _selectItem(HeaderListItem item)
        {
            if (selectedItem_ != item)
            {
                if (selectedItem_ != null)
                    selectedItem_.IsSelected = false;
                selectedItem_ = item;
                if (selectedItem_ != null)
                    selectedItem_.IsSelected = true;
                if (SelectedChangedEvent != null)
                    SelectedChangedEvent.Invoke(selectedItem_);
            }
            if (selectedItem_ != null)
                sv.MakesureChildVisible(selectedItem_);
        }
        private HeaderListItem _createItem(TableExcelHeader header)
        {
            var item = new HeaderListItem();
            item.FieldName = header.FieldName;
            item.FieldType = header.FieldType;
            item.FieldDesc = header.FieldDesc;
            var binding = new Binding("ActualWidth") { Source = wp };
            item.SetBinding(FrameworkElement.WidthProperty, binding);
            item.MouseLeftButtonDown += (_s, _e) =>
            {
                _e.Handled = true;
                _selectItem(_s as HeaderListItem);
            };
            item.MouseRightButtonDown += (_s, _e) =>
            {
                _e.Handled = true;
                _selectItem(_s as HeaderListItem);
                _flipMenuWithItem(_s as HeaderListItem);
            };
            item.MouseDoubleClick += (_s, _e) =>
            {
                if (_e.ChangedButton == MouseButton.Left)
                {
                    _e.Handled = true;
                    _flipEditItemPanel(_s as HeaderListItem);
                }
            };
            return item;
        }
        private void _flipMenuWithItem(HeaderListItem item)
        {
            var menu = new ContextMenu();

            var miEdit = new MenuItem();
            miEdit.Header = "编辑(_E)";
            miEdit.IsEnabled = item.Editable;
            miEdit.FontWeight = FontWeights.Bold;
            miEdit.Click += (_s, _e) => _flipEditItemPanel(item);
            menu.Items.Add(miEdit);

            var miAddCopy = new MenuItem();
            miAddCopy.IsEnabled = item.Editable;
            miAddCopy.Header = "复制添加(_A)";
            miAddCopy.Click += (_s, _e) =>
            {
                if (AddCopyEvent != null)
                    AddCopyEvent.Invoke(item);
            };
            menu.Items.Add(miAddCopy);

            var miDelete = new MenuItem();
            miDelete.IsEnabled = item.Editable;
            miDelete.Header = "删除(_D)";
            miDelete.Click += (_s, _e) =>
            {
                if (DeleteEvent != null)
                    DeleteEvent.Invoke(item);
            };
            menu.Items.Add(miDelete);

            menu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
            menu.IsOpen = true;
        }
        private void _flipMenuWithBackground()
        {

        }
        private void _flipEditItemPanel(HeaderListItem item)
        {
            if (item.Editable && EditEvent != null)
                EditEvent.Invoke(item);
        }
        #endregion
    }
}
