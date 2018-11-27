using System;
using System.Collections.Generic;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using NPOI.XSSF.UserModel;

namespace tablegen2.logic
{
    public static class TableExcelReader
    {
        public static TableExcelData loadFromExcel(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception(string.Format("{0} 文件不存在！", filePath));

            var ext = Path.GetExtension(filePath).ToLower();
            if (ext != ".xls" && ext != ".xlsx")
                throw new Exception(string.Format("无法识别的文件扩展名 {0}", ext));

            var headers = new List<TableExcelHeader>();
            var rows = new List<TableExcelRow>();
            
            var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var workbook = ext == ".xls" ? (IWorkbook)new HSSFWorkbook(fs) : (IWorkbook)new XSSFWorkbook(fs);
            fs.Close();

            _readDataFromWorkbook(workbook, headers, rows);

            return new TableExcelData(headers, rows);
        }

        private static void _readDataFromWorkbook(IWorkbook wb, List<TableExcelHeader> headers, List<TableExcelRow> rows)
        {
            var defSheetName = AppData.Config.SheetNameForField;
            var dataSheetName = AppData.Config.SheetNameForData;

            var sheet1 = wb.GetSheet(defSheetName);
            if (sheet1 == null)
                throw new Exception(string.Format("'{0}'工作簿不存在", defSheetName));

            var sheet2 = wb.GetSheet(dataSheetName);
            if (sheet2 == null)
                throw new Exception(string.Format("'{0}'工作簿不存在", dataSheetName));

            //加载字段
            _readHeadersFromDefSheet(sheet1, headers);

            var h1 = headers.Find(a => a.FieldName == "Id");
            if (h1 == null)
                throw new Exception(string.Format("'{0}'工作簿中不存在Id字段！", defSheetName));

            var h2 = headers.Find(a => a.FieldName == "KeyName");
            if (h2 == null)
                throw new Exception(string.Format("'{0}'工作簿中不存在KeyName字段！", defSheetName));

            //加载数据
            var headers2 = _readHeadersFromDataSheet(sheet2);
            var headerIndexes = new int[headers.Count];
            _checkFieldsSame(headers, headers2, headerIndexes);

            foreach (var ds in _readDataFromDataSheet(sheet2, headers2.Count))
            {
                var rowData = new List<string>();
                for (int i = 0; i < headers.Count; i++)
                {
                    var idx = headerIndexes[i];
                    rowData.Add(ds[idx]);
                }
                rows.Add(new TableExcelRow() { StrList = rowData });
            }
        }

        private static string _convertCellToString(ICell cell)
        {
            string r = string.Empty;
            if (cell != null)
            {
                switch (cell.CellType)
                {
                    case CellType.Boolean:
                        r = cell.BooleanCellValue.ToString();
                        break;
                    case CellType.Numeric:
                        r = cell.NumericCellValue.ToString();
                        break;
                    case CellType.Formula:
                        r = cell.CellFormula;
                        break;
                    default:
                        r = cell.StringCellValue;
                        break;
                }
            }
            return r;
        }

        private static void _readHeadersFromDefSheet(ISheet sheet, List<TableExcelHeader> headers)
        {
            for (int row = 1; row <= sheet.LastRowNum; row++)
            {
                var rd = sheet.GetRow(row);
                if (rd == null)
                    continue;

                var str1 = _convertCellToString(rd.GetCell(0));
                var str2 = _convertCellToString(rd.GetCell(1));
                var str3 = _convertCellToString(rd.GetCell(2));

                if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2) && string.IsNullOrEmpty(str3))
                    continue;

                if (!string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2))
                {
                    headers.Add(new TableExcelHeader()
                    {
                        FieldName = str1,
                        FieldType = str2,
                        FieldDesc = str3,
                    });
                    continue;
                }

                throw new Exception(string.Format(
                    "'{0}'工作簿中第{1}行数据异常，有缺失！", AppData.Config.SheetNameForField, row + 1));
            }
        }
        
        private static List<string> _readHeadersFromDataSheet(ISheet sheet)
        {
            var r = new List<string>();
            var rd = sheet.GetRow(0);
            for (int i = 0; i < rd.LastCellNum; i++)
            {
                var cell = rd.GetCell(i);
                r.Add(cell != null ? cell.StringCellValue : string.Empty);
            }
            for (int i = r.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(r[i]))
                    r.RemoveAt(i);
                else
                    break;
            }
            int idx = r.IndexOf(string.Empty);
            if (idx >= 0)
                throw new Exception(string.Format(
                    "'{0}'工作簿中第1行第{1}列字段名称非法", AppData.Config.SheetNameForData, idx + 1));
            return r;
        }

        private static void _checkFieldsSame(List<TableExcelHeader> headers1, List<string> headers2, int[] indexes)
        {
            for (int i = 0; i < headers1.Count; i++)
            {
                var hd = headers1[i];
                var idx = headers2.IndexOf(hd.FieldName);
                if (idx < 0)
                    throw new Exception(string.Format("'{0}'工作簿中不存在字段'{1}'所对应的列", AppData.Config.SheetNameForData, hd.FieldName));
                indexes[i] = idx;
            }
            if (headers1.Count < headers2.Count)
            {
                foreach (var s in headers2)
                {
                    if (headers1.Find(a => a.FieldName == s) == null)
                        throw new Exception(string.Format("'{0}'工作簿中的包含多余的数据列'{1}'", AppData.Config.SheetNameForData, s));
                }
            }
        }

        private static IEnumerable<List<string>> _readDataFromDataSheet(ISheet sheet, int columns)
        {
            for (int i = 1; i <= sheet.LastRowNum; i++)
            {
                var rd = sheet.GetRow(i);
                if (rd == null)
                    continue;

                var ds = new List<string>();
                bool is_all_empty = true;
                for (int c = 0; c < columns; c++)
                {
                    var val = _convertCellToString(rd.GetCell(c));
                    if (!string.IsNullOrEmpty(val))
                        is_all_empty = false;
                    ds.Add(val);
                }

                if (!is_all_empty)
                    yield return ds;
            }
        }
    }
}
