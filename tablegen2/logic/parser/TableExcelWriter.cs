using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace tablegen2.logic
{
    public static class TableExcelWriter
    {
        public static void genExcel(TableExcelData data, string filePath)
        {
            Util.MakesureFolderExist(Path.GetDirectoryName(filePath));

            var ext = Path.GetExtension(filePath).ToLower();
            if (ext != ".xls" && ext != ".xlsx")
                throw new Exception(string.Format("无法识别的文件扩展名 {0}", ext));

            var workbook = ext == ".xls" ? (IWorkbook)new HSSFWorkbook() : (IWorkbook)new XSSFWorkbook();
            var sheet1 = workbook.CreateSheet(AppData.Config.SheetNameForField);
            var sheet2 = workbook.CreateSheet(AppData.Config.SheetNameForData);

            //创建新字体
            var font = workbook.CreateFont();
            font.IsBold = true;

            //创建新样式
            var style = workbook.CreateCellStyle();
            style.SetFont(font);
            style.FillPattern = FillPattern.SolidForeground;
            style.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.Grey25Percent.Index;

            //默认样式
            for (short i = 0; i < workbook.NumCellStyles; i++)
            {
                workbook.GetCellStyleAt(i).VerticalAlignment = VerticalAlignment.Center;
            }

            _writeToDefSheet(sheet1, data.Headers);
            _writeToDataSheet(sheet2, data);

            var tmppath = Path.Combine(Path.GetDirectoryName(filePath), 
                string.Format("{0}.tmp{1}", Path.GetFileNameWithoutExtension(filePath), ext));
            using (var fs = File.Open(tmppath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            {
                workbook.Write(fs);
            }
            var content = File.ReadAllBytes(tmppath);
            File.Delete(tmppath);
            File.WriteAllBytes(filePath, content);
        }

        private static void _writeToSheet(ISheet sheet, IEnumerable<string> lst)
        {
            IRow row;
            if (sheet.GetRow(0) == null)
                row = sheet.CreateRow(0);
            else
                row = sheet.CreateRow(sheet.LastRowNum + 1);

            int idx = 0;
            foreach (var s in lst)
            {
                var cell = row.CreateCell(idx);
                double val;
                if (double.TryParse(s, out val))
                    cell.SetCellValue(val);
                else
                    cell.SetCellValue(s);
                int curwidth = sheet.GetColumnWidth(idx);
                sheet.SetColumnWidth(idx, Math.Max(curwidth, _calcSheetWidth(s)));

                if (row.RowNum == 0)
                {
                    cell.CellStyle = sheet.Workbook.GetCellStyleAt((short)(sheet.Workbook.NumCellStyles - 1));
                }

                row.Height = 24 * 15;

                idx++;
            }
        }

        private static int _calcSheetWidth(string str)
        {
            var r = 0;
            if (str.Length > 50)
                str = str.Substring(0, 50);
            foreach (var c in str)
            {
                if ((int)c < 128)
                    r += 256;
                else
                    r += 512;
            }
            return Math.Max(2048, r + 1024);
        }

        private static void _writeToDefSheet(ISheet sheet, List<TableExcelHeader> headers)
        {
            _writeToSheet(sheet, new string[]
            {
                "字段名称",
                "字段类型",
                "字段说明",
            });

            foreach (var hdr in headers)
            {
                _writeToSheet(sheet, new string[]
                {
                    hdr.FieldName,
                    hdr.FieldType,
                    hdr.FieldDesc,
                });
            }
        }

        private static void _writeToDataSheet(ISheet sheet, TableExcelData data)
        {
            var lst = data.Headers.Select(a => a.FieldName).ToList();
            _writeToSheet(sheet, lst);

            foreach (var row in data.Rows)
            {
                _writeToSheet(sheet, row.StrList);
            }
        }
    }
}
