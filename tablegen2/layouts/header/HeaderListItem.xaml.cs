using System.Windows;

namespace tablegen2.layouts
{
    /// <summary>
    /// HeaderListItem.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderListItem : ListItemBase
    {
        #region 依赖属性
        public static readonly DependencyProperty FieldNameProperty;
        public static readonly DependencyProperty FieldTypeProperty;
        public static readonly DependencyProperty FieldDescProperty;
        public static readonly DependencyProperty EditableProperty;
        static HeaderListItem()
        {
            FieldNameProperty = DependencyProperty.Register(
                "FieldName",
                typeof(string),
                typeof(HeaderListItem),
                new FrameworkPropertyMetadata(""),
                null);
            FieldTypeProperty = DependencyProperty.Register(
                "FieldType",
                typeof(string),
                typeof(HeaderListItem),
                new FrameworkPropertyMetadata(""),
                null);
            FieldDescProperty = DependencyProperty.Register(
                "FieldDesc",
                typeof(string),
                typeof(HeaderListItem),
                new FrameworkPropertyMetadata(""),
                null);
            EditableProperty = DependencyProperty.Register(
                "Editable",
                typeof(bool),
                typeof(HeaderListItem),
                new FrameworkPropertyMetadata(true),
                null);
        }
        public string FieldName
        {
            get { return (string)GetValue(FieldNameProperty); }
            set
            {
                if (value == "Id" || value == "KeyName")
                    Editable = false;
                SetValue(FieldNameProperty, value);
            }
        }
        public string FieldType
        {
            get { return (string)GetValue(FieldTypeProperty); }
            set { SetValue(FieldTypeProperty, value); }
        }
        public string FieldDesc
        {
            get { return (string)GetValue(FieldDescProperty); }
            set { SetValue(FieldDescProperty, value); }
        }
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }
        #endregion

        public HeaderListItem()
        {
            InitializeComponent();
        }
    }
}
