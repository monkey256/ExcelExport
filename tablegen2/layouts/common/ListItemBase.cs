using System.Windows;
using System.Windows.Controls;

namespace tablegen2.layouts
{
    public class ListItemBase : UserControl
    {
        //条目是否选中
        public static readonly DependencyProperty IsSelectedProperty;
        static ListItemBase()
        {
            IsSelectedProperty = DependencyProperty.Register(
                "IsSelected",
                typeof(bool),
                typeof(ListItemBase),
                new FrameworkPropertyMetadata(false, OnIsSelectedPropertyChanged),
                null);
        }

        //选中状态发生变化
        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //(d as TableListItemBase).OnSelectedChanged((bool)e.NewValue);
        }

        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public virtual void RefreshByTableInterface()
        {

        }

        //public abstract void OnSelectedChanged(bool val);
    }
}
