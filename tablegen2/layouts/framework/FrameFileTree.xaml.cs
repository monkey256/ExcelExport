using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using tablegen2.logic;

namespace tablegen2.layouts
{
    /// <summary>
    /// FrameFileTree.xaml 的交互逻辑
    /// </summary>
    public partial class FrameFileTree : UserControl
    {
        public event Action OpenExcelRequest;

        public FrameFileTree()
        {
            InitializeComponent();
            if (AppData.Config != null)
                refreshExcelPath(AppData.Config.ExcelDir);
            lv.CreateExcelEvent += () => Util.performClick(btnAddExcel);
        }

        public IEnumerable<string> AllExcels
        {
            get { return lv.AllExcels; }
        }

        public void refreshExcelPath(string excelPath)
        {
            if (string.IsNullOrEmpty(excelPath) || !System.IO.Directory.Exists(excelPath))
            {
                txtPath.Text = string.Empty;
                lv.Visibility = Visibility.Hidden;
                layer.Visibility = Visibility.Visible;
            }
            else
            {
                txtPath.Text = excelPath;
                lv.Visibility = Visibility.Visible;
                layer.Visibility = Visibility.Hidden;
                lv.refreshByDir(excelPath);
            }
        }

        private void btnOpenExcel_Clicked(object sender, RoutedEventArgs e)
        {
            if (OpenExcelRequest != null)
                OpenExcelRequest.Invoke();
        }

        private void btnAddExcel_Clicked(object sender, RoutedEventArgs e)
        {
            var panel = new HeaderPage();
            panel.IsNewFileMode = true;
            panel.ExcelDir = AppData.Config.ExcelDir;
            panel.refreshDislpay(new List<TableExcelHeader>()
            {
                new TableExcelHeader()
                {
                    FieldName = "Id",
                    FieldType = "int",
                    FieldDesc = "唯一Id索引",
                },
                new TableExcelHeader()
                {
                    FieldName = "KeyName",
                    FieldType = "string",
                    FieldDesc = "唯一Key索引",
                },
            });

            var pw = new PopupWindow(panel);
            pw.ResizeMode = ResizeMode.CanResize;
            pw.Owner = Window.GetWindow(this);
            pw.Title = string.Format("新建Excel表格");
            pw.MinWidth = 600;
            pw.MinHeight = 400;
            if (pw.ShowDialog() == true)
            {
                try
                {
                    TableExcelWriter.genExcel(new TableExcelData(panel.Headers, new List<TableExcelRow>()), panel.ExcelFullPath);
                    refreshExcelPath(AppData.Config.ExcelDir);
                    lv.selectItemByFullPath(panel.ExcelFullPath);
                }
                catch (System.Exception ex)
                {
                    this.ErrBox(ex.Message);
                }
            }
        }
    }
}
