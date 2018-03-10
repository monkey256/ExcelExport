using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using tablegen2.logic;

namespace tablegen2.layouts
{
    /// <summary>
    /// FrameExcelView.xaml 的交互逻辑
    /// </summary>
    public partial class FrameExcelView : UserControl
    {
        private TableExcelData data_;
        private string filePath_;

        public FrameExcelView()
        {
            InitializeComponent();
        }

        public void setFilePath(string filePath)
        {
            filePath_ = filePath;
        }

        public void refreshUIByTableExcelData(TableExcelData data)
        {
            data_ = data;

            DataTable dt = new DataTable();

            //columns
            foreach (var hdr in data.Headers)
            {
                dt.Columns.Add(new DataColumn(hdr.FieldName, typeof(string)));
            }

            //rows
            foreach (var row in data.Rows)
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < data.Headers.Count; i++)
                {
                    var hdr = data.Headers[i];
                    dr[hdr.FieldName] = row.StrList[i];
                }
                dt.Rows.Add(dr);
            }

            dataGrid1.ItemsSource = dt.DefaultView;
        }

        public void updateDataFromDataGrid()
        {
            var dv = dataGrid1.ItemsSource as DataView;
            var dt = dv.Table;

            data_.Rows.Clear();
            foreach (DataRow row in dt.Rows)
            {
                var lst = new List<string>();
                for (int i = 0; i < data_.Headers.Count; i++)
                {
                    var s = row[i].ToString();
                    lst.Add(s);
                }
                data_.Rows.Add(new TableExcelRow() { StrList = lst });
            }
        }

        private void btnSave_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                dataGrid1.CommitEdit(DataGridEditingUnit.Row, true);

                updateDataFromDataGrid();

                var fp = filePath_.ToLower();
                if (fp.EndsWith(".xls") || fp.EndsWith(".xlsx"))
                {
                    TableExcelWriter.genExcel(data_, filePath_);
                }
                else if (fp.EndsWith(".exdat"))
                {
                    TableExcelExportDat.exportExcelFile(data_, filePath_);
                }
                else if (fp.EndsWith(".json"))
                {
                    TableExcelExportJson.exportExcelFile(data_, filePath_);
                }
                else if (fp.EndsWith(".xml"))
                {
                    TableExcelExportXml.exportExcelFile(data_, filePath_);
                }
                else if (fp.EndsWith(".lua"))
                {
                    TableExcelExportLua.exportExcelFile(data_, filePath_);
                }
                Window.GetWindow(this).Close();
            }
            catch (System.Exception ex)
            {
                this.ErrBox(ex.Message);
            }
        }

        private void btnSetting_Clicked(object sender, RoutedEventArgs e)
        {
            var panel = new HeaderPage();
            panel.IsNewFileMode = false;
            panel.refreshDislpay(data_.Headers);

            var pw = new PopupWindow(panel);
            pw.ResizeMode = ResizeMode.CanResize;
            pw.Owner = Window.GetWindow(this);
            pw.Title = string.Format("设置字段 -- {0}", filePath_);
            pw.MinWidth = 600;
            pw.MinHeight = 400;
            if (pw.ShowDialog() == true)
            {
                var headers = panel.Headers;

                var tmp = new TableExcelData(headers, new List<TableExcelRow>());
                var arr = headers.Select(a => data_.Headers.FindIndex(b => b.FieldName == a.FieldName)).ToArray();
                foreach (var row in data_.Rows)
                {
                    var trow = new TableExcelRow();
                    trow.StrList = arr.Select(idx => idx < 0 ? string.Empty : row.StrList[idx]).ToList();
                    tmp.Rows.Add(trow);
                }
                
                refreshUIByTableExcelData(tmp);
            }
        }
    }
}
