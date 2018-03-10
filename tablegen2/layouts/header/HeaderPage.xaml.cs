using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using tablegen2.logic;

namespace tablegen2.layouts
{
    /// <summary>
    /// HeaderPage.xaml 的交互逻辑
    /// </summary>
    public partial class HeaderPage : UserControl
    {
        #region 依赖属性
        public static readonly DependencyProperty EditableProperty;
        public static readonly DependencyProperty IsNewFileModeProperty;
        static HeaderPage()
        {
            EditableProperty = DependencyProperty.Register(
                "Editable",
                typeof(bool),
                typeof(HeaderPage),
                new FrameworkPropertyMetadata(false),
                null);
            IsNewFileModeProperty = DependencyProperty.Register(
                "IsNewFileMode",
                typeof(bool),
                typeof(HeaderPage),
                new FrameworkPropertyMetadata(false),
                null);
        }
        public bool Editable
        {
            get { return (bool)GetValue(EditableProperty); }
            set { SetValue(EditableProperty, value); }
        }
        public bool IsNewFileMode
        {
            get { return (bool)GetValue(IsNewFileModeProperty); }
            set { SetValue(IsNewFileModeProperty, value); }
        }
        #endregion

        public List<TableExcelHeader> Headers { get; internal set; }
        public string ExcelDir { get; set; }
        public string ExcelFullPath { get; internal set; }

        public HeaderPage()
        {
            InitializeComponent();
            lv.SelectedChangedEvent += (item) => Editable = item != null && item.Editable;
            lv.AddCopyEvent += (item) => itemAddCopyImpl(item);
            lv.EditEvent += (item) => itemEditImpl(item);
            lv.DeleteEvent += (item) => itemDeleteImpl(item);
        }

        public void refreshDislpay(List<TableExcelHeader> headers)
        {
            lv.refreshDisplay(headers);
        }

        private void btnAddNew_Clicked(object sender, RoutedEventArgs e)
        {
            var panel = new HeaderFieldEdit();
            panel.IsFieldNameReadonly = false;

            var pw = new PopupWindow(panel);
            pw.Owner = Window.GetWindow(this);
            pw.Title = string.Format("添加字段");
            if (pw.ShowDialog() == true)
            {
                if (lv.isFieldNameExist(panel.FieldName))
                {
                    this.InfBox("该字段已存在！");
                    return;
                }

                lv.addItem(new TableExcelHeader()
                {
                    FieldName = panel.FieldName,
                    FieldType = panel.FieldType,
                    FieldDesc = panel.FieldDesc,
                }, true);
            }
        }

        private void btnAddCopy_Clicked(object sender, RoutedEventArgs e)
        {
            itemAddCopyImpl(lv.SelectedItem);
        }

        private void btnEdit_Clicked(object sender, RoutedEventArgs e)
        {
            itemEditImpl(lv.SelectedItem);
        }

        private void btnDelete_Clicked(object sender, RoutedEventArgs e)
        {
            itemDeleteImpl(lv.SelectedItem);
        }

        private void btnMoveUp_Clicked(object sender, RoutedEventArgs e)
        {
            lv.moveUp(lv.SelectedItem);
        }

        private void btnMoveDown_Clicked(object sender, RoutedEventArgs e)
        {
            lv.moveDown(lv.SelectedItem);
        }

        private void btnSave_Clicked(object sender, RoutedEventArgs e)
        {
            if (IsNewFileMode)
            {
                if (!Directory.Exists(ExcelDir))
                {
                    this.InfBox("系统错误，Excel目录不存在！");
                    return;
                }

                var fileName = txtFileName.Text;
                if (string.IsNullOrEmpty(fileName))
                {
                    this.InfBox("请填写文件名称！");
                    txtFileName.Focus();
                    return;
                }

                var ext = Path.GetExtension(fileName).ToLower();
                if (ext != ".xls" && ext != ".xlsx")
                    fileName += ".xlsx";

                var fullPath = Path.Combine(ExcelDir, fileName);
                if (File.Exists(fullPath))
                {
                    this.InfBox("{0}\n该文件已存在，请重新填写！", fullPath);
                    txtFileName.Focus();
                    txtFileName.SelectAll();
                    return;
                }

                ExcelFullPath = fullPath;
            }

            Headers = lv.HeaderItems.ToList();
            Window.GetWindow(this).DialogResult = true;
        }

        private void itemAddCopyImpl(HeaderListItem item)
        {
            var panel = new HeaderFieldEdit();
            panel.IsFieldNameReadonly = false;
            panel.FieldName = item.FieldName;
            panel.FieldType = item.FieldType;
            panel.FieldDesc = item.FieldDesc;

            var pw = new PopupWindow(panel);
            pw.Owner = Window.GetWindow(this);
            pw.Title = string.Format("复制添加字段");
            if (pw.ShowDialog() == true)
            {
                if (lv.isFieldNameExist(panel.FieldName))
                {
                    this.InfBox("该字段已存在！");
                    return;
                }

                lv.addItem(new TableExcelHeader()
                {
                    FieldName = panel.FieldName,
                    FieldType = panel.FieldType,
                    FieldDesc = panel.FieldDesc,
                }, true);
            }
        }
        private void itemEditImpl(HeaderListItem item)
        {
            var panel = new HeaderFieldEdit();
            panel.IsFieldNameReadonly = true;
            panel.FieldName = item.FieldName;
            panel.FieldType = item.FieldType;
            panel.FieldDesc = item.FieldDesc;

            var pw = new PopupWindow(panel);
            pw.Owner = Window.GetWindow(this);
            pw.Title = string.Format("复制添加字段");
            if (pw.ShowDialog() == true)
            {
                item.FieldType = panel.FieldType;
                item.FieldDesc = panel.FieldDesc;
            }
        }
        private void itemDeleteImpl(HeaderListItem item)
        {
            if (this.YesNoBox("确定要删除该条目吗？"))
            {
                lv.removeItem(item);
            }
        }
    }
}
