using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using tablegen2.logic;

namespace tablegen2.layouts
{
    /// <summary>
    /// FrameSetting.xaml 的交互逻辑
    /// </summary>
    public partial class FrameSetting : UserControl
    {
        public event Action ExcelDirChanged;
        public event Action ExportDirChanged;
        public event Action ExportFormatChanged;
        public event Action MoreSettingEvent;

        public FrameSetting()
        {
            InitializeComponent();
            if (AppData.Config != null)
            {
                txtExcelDir.Text = AppData.Config.ExcelDir;
                txtExportDir.Text = AppData.Config.ExportDir;
                cbExportFormat.SelectComboBoxItemByTag(AppData.Config.ExportFormat.ToString());
            }
        }

        public void browseExcelDirectory()
        {
            Util.performClick(btnBrowseExcelDir);
        }

        private void exportFormat_Selected(object sender, RoutedEventArgs e)
        {
            var item = sender as ComboBoxItem;
            var fmt = (TableExportFormat)Enum.Parse(typeof(TableExportFormat), item.Tag as string);
            if (fmt != TableExportFormat.Unknown)
            {
                AppData.Config.ExportFormat = fmt;
                AppData.saveConfig();
                if (ExportFormatChanged != null)
                    ExportFormatChanged.Invoke();
            }
        }

        private void btnBrowseExcelDir_Clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择Excel目录";
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    txtExcelDir.Text = dialog.SelectedPath;
                    AppData.Config.ExcelDir = dialog.SelectedPath;
                    AppData.saveConfig();
                    if (ExcelDirChanged != null)
                        ExcelDirChanged.Invoke();
                }
            }
        }

        private void btnBrowseExportDir_Clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = "请选择数据导出目录";
            dialog.ShowNewFolderButton = true;
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!string.IsNullOrEmpty(dialog.SelectedPath))
                {
                    txtExportDir.Text = dialog.SelectedPath;
                    AppData.Config.ExportDir = dialog.SelectedPath;
                    AppData.saveConfig();
                    if (ExportDirChanged != null)
                        ExportDirChanged.Invoke();
                }
            }
        }

        private void btnOpenExcelDir_Clicked(object sender, RoutedEventArgs e)
        {
            var dir = AppData.Config.ExcelDir;
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                this.InfBox("请先使用‘浏览’功能选择合法的配置目录");
                return;
            }
            Util.OpenDir(dir);
        }

        private void btnOpenExportDir_Clicked(object sender, RoutedEventArgs e)
        {
            var dir = AppData.Config.ExportDir;
            if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
            {
                this.InfBox("请先使用‘浏览’功能选择合法的配置目录");
                return;
            }
            Util.OpenDir(dir);
        }

        private void btnMoreSetting_Clicked(object sender, RoutedEventArgs e)
        {
            if (MoreSettingEvent != null)
                MoreSettingEvent.Invoke();
        }
    }
}
