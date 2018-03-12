using System.Windows;
using System.Windows.Controls;

namespace tablegen2.layouts
{
    /// <summary>
    /// FrameSettingMore.xaml 的交互逻辑
    /// </summary>
    public partial class FrameSettingMore : UserControl
    {
        public FrameSettingMore()
        {
            InitializeComponent();
            if (AppData.Config != null)
            {
                txtSheetField.Text = AppData.Config.SheetNameForField;
                txtSheetData.Text = AppData.Config.SheetNameForData;
            }
        }

        private void btnSave_Clicked(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtSheetField.Text))
            {
                this.InfBox("请输入字段工作簿名称！");
                txtSheetField.Focus();
                return;
            }

            if (string.IsNullOrEmpty(txtSheetData.Text))
            {
                this.InfBox("请输入数据工作簿名称！");
                txtSheetField.Focus();
                return;
            }

            AppData.Config.SheetNameForField = txtSheetField.Text;
            AppData.Config.SheetNameForData = txtSheetData.Text;
            AppData.saveConfig();
            Window.GetWindow(this).DialogResult = true;
        }
    }
}
