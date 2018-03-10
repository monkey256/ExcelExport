using System.Windows;
using System.Windows.Controls;

namespace tablegen2.layouts
{
    /// <summary>
    /// HeaderFieldEdit.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderFieldEdit : UserControl
    {
        #region 依赖属性
        public static readonly DependencyProperty FieldNameProperty;
        public static readonly DependencyProperty IsFieldNameReadonlyProperty;
        public static readonly DependencyProperty FieldDescProperty;
        static HeaderFieldEdit()
        {
            FieldNameProperty = DependencyProperty.Register(
                "FieldName",
                typeof(string),
                typeof(HeaderFieldEdit),
                new FrameworkPropertyMetadata(""),
                null);
            IsFieldNameReadonlyProperty = DependencyProperty.Register(
                "IsFieldNameReadonly",
                typeof(bool),
                typeof(HeaderFieldEdit),
                new FrameworkPropertyMetadata(false),
                null);
            FieldDescProperty = DependencyProperty.Register(
                "FieldDesc",
                typeof(string),
                typeof(HeaderFieldEdit),
                new FrameworkPropertyMetadata(""),
                null);
        }
        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set { SetValue(FieldNameProperty, value); }
        }
        public bool IsFieldNameReadonly
        {
            get { return (bool)GetValue(IsFieldNameReadonlyProperty); }
            set { SetValue(IsFieldNameReadonlyProperty, value); }
        }
        public string FieldDesc
        {
            get { return (string)GetValue(FieldDescProperty); }
            set { SetValue(FieldDescProperty, value); }
        }
        public string FieldType
        {
            get { return (cbFieldType.SelectedItem as ComboBoxItem).Tag as string; }
            set { cbFieldType.SelectComboBoxItemByTag(value); }
        }
        #endregion

        public HeaderFieldEdit()
        {
            InitializeComponent();
        }

        private void btnSave_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FieldName))
            {
                this.InfBox("请填写字段名称！");
                txtFieldName.Focus();
                return;
            }

            Window.GetWindow(this).DialogResult = true;
        }
    }
}
