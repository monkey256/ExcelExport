using System;
using System.IO;
using System.Text;
using System.Windows;
using tablegen2.logic;
using tablegen2.layouts;
using System.Collections.Generic;
using System.Linq;

namespace tablegen2
{
    internal enum CommandType
    {
        Unknown = 0,
        Help,
        OpenDatFile,
        OpenExcelFile,
        ExportFiles,
    }

    internal static class CommandHelper
    {
        //-h|-help|/h|/help
        //fullPath.exdat
        //fullPath.xls|.xlsx
        //-i excelFullPath|excelDir -o outputDir -t xml|json|lua|dat

        public static CommandType Command { get; internal set; }

        public static string OpenDatFullPath { get; internal set; }

        public static string OpenExcelFullPath { get; internal set; }

        public static string ExportInputPath { get; internal set; }
        public static string ExportOutputDir { get; internal set; }
        public static string ExportType { get; internal set; }

        static CommandHelper()
        {
            _analyzeCommandParameter();
        }

        public static void MsgBox(string str, params object[] args)
        {
            System.Windows.MessageBox.Show(
                string.Format(str, args),
                "提示",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);
        }

        private static void _analyzeCommandParameter()
        {
            Command = CommandType.Unknown;

            do
            {
                string[] arr = Environment.GetCommandLineArgs();
                if (arr.Length == 0)
                    break;

                string[] cpy = arr;
                string s = arr[0];
                if (s.Contains("vshost") || s.ToLower() == Util.ModulePath.ToLower())
                {
                    cpy = new string[arr.Length - 1];
                    Array.Copy(arr, 1, cpy, 0, arr.Length - 1);
                }

                #region help
                if (cpy.Length == 1)
                {
                    var c = cpy[0];
                    if (c == "-h" || c == "-help" || c == "/h" || c == "/help")
                    {
                        Command = CommandType.Help;
                        break;
                    }
                }
                #endregion

                #region open dat
                if (cpy.Length == 1 && File.Exists(cpy[0]))
                {
                    if (Path.GetExtension(cpy[0]).ToLower() == ".exdat")
                    {
                        Command = CommandType.OpenDatFile;
                        OpenDatFullPath = cpy[0];
                        break;
                    }
                }
                #endregion

                #region open excel
                if (cpy.Length == 1 && File.Exists(cpy[0]))
                {
                    var ext = Path.GetExtension(cpy[0]).ToLower();
                    if (ext == ".xls" || ext == ".xlsx")
                    {
                        Command = CommandType.OpenExcelFile;
                        OpenExcelFullPath = cpy[0];
                        break;
                    }
                }
                #endregion

                #region convert excel
                if (cpy.Length == 6)
                {
                    var ps = new Dictionary<string, string>();
                    ps[cpy[0]] = cpy[1];
                    ps[cpy[2]] = cpy[3];
                    ps[cpy[4]] = cpy[5];
                    var ks = ps.Keys.ToList();
                    ks.Sort();
                    if (string.Concat(ks) == "-i-o-t")
                    {
                        Command = CommandType.ExportFiles;
                        ExportInputPath = ps["-i"];
                        ExportOutputDir = ps["-o"];
                        ExportType = ps["-t"];
                        break;
                    }
                }
                #endregion
            } while (false);
        }

        public static void processHelp()
        {
            var sb = new StringBuilder();
            sb.AppendLine("使用说明");
            sb.AppendLine("1.datPath");
            sb.AppendLine("  功能说明：打开并查看.exdat文件");
            sb.AppendLine("2.excelPath");
            sb.AppendLine("  功能说明：打开并查看.xls或.xlsx文件");
            sb.AppendLine("3.-i excelFullPath|excelDir -o outputDir -t xml|json|lua|dat");
            sb.AppendLine("  功能说明：将Excel表或整个目录导出指定格式的数据文件");
            MsgBox(sb.ToString());
        }

        public static void processOpenDatFile()
        {
            try
            {
                var data = TableExcelImportDat.importFile(OpenDatFullPath);
                openEditUI(data, OpenDatFullPath);
            }
            catch (System.Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        public static void processOpenExcelFile()
        {
            try
            {
                var data = TableExcelReader.loadFromExcel(OpenExcelFullPath);
                openEditUI(data, OpenExcelFullPath);
            }
            catch (System.Exception ex)
            {
                MsgBox(ex.Message);
            }
        }

        private static void openEditUI(TableExcelData data, string fullPath)
        {
            var panel = new FrameExcelView();
            panel.refreshUIByTableExcelData(data);
            panel.setFilePath(fullPath);

            var pw = new PopupWindow(panel);
            pw.ResizeMode = ResizeMode.CanResize;
            pw.Title = string.Format("查看配置表 -- {0}", fullPath);
            pw.MinWidth = 600;
            pw.MinHeight = 400;
            pw.ShowInTaskbar = true;
            pw.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            App app = new App();
            app.InitializeComponent();
            app.Run((Window)pw);
        }

        public static void processExportFiles()
        {
            var lst = new List<string>();
            if (Directory.Exists(ExportInputPath))
            {
                lst = Directory.GetFiles(ExportInputPath, "*.xls", SearchOption.AllDirectories).ToList();
                lst.Union(Directory.GetFiles(ExportInputPath, "*.xlsx", SearchOption.AllDirectories));
            }
            else if (File.Exists(ExportInputPath))
            {
                lst.Add(ExportInputPath);
            }
            else
            {
                MsgBox("文件或目录不存在！ {0}", ExportInputPath);
                return;
            }

            Util.MakesureFolderExist(ExportOutputDir);
            if (!Directory.Exists(ExportOutputDir))
            {
                MsgBox("无法创建目录！ {0}", ExportOutputDir);
                return;
            }

            var fmt = TableExportFormat.Unknown;
            switch (ExportType.ToLower())
            {
                case "xml":
                    fmt = TableExportFormat.Xml;
                    break;
                case "json":
                    fmt = TableExportFormat.Json;
                    break;
                case "lua":
                    fmt = TableExportFormat.Lua;
                    break;
                case "dat":
                    fmt = TableExportFormat.Dat;
                    break;
            }
            if (fmt == TableExportFormat.Unknown)
            {
                MsgBox("无法识别的导出格式！ {0}", ExportType);
                return;
            }

            foreach (var filePath in lst)
            {
                try
                {
                    exportFile(filePath, ExportOutputDir, fmt);
                }
                catch (System.Exception ex)
                {
                    MsgBox("转换文件'{0}'时出错：\n{1}", filePath, ex.Message);
                    return;
                }
            }
        }

        private static void exportFile(string excelPath, string outputDir, TableExportFormat fmt)
        {
            var data = TableExcelReader.loadFromExcel(excelPath);
            switch (fmt)
            {
                case TableExportFormat.Dat:
                    {
                        var exportPath = Path.Combine(outputDir, string.Format("{0}.exdat", Path.GetFileNameWithoutExtension(excelPath)));
                        TableExcelExportDat.exportExcelFile(data, exportPath);
                    }
                    break;
                case TableExportFormat.Json:
                    {
                        var exportPath = Path.Combine(outputDir, string.Format("{0}.json", Path.GetFileNameWithoutExtension(excelPath)));
                        TableExcelExportJson.exportExcelFile(data, exportPath);
                    }
                    break;
                case TableExportFormat.Xml:
                    {
                        var exportPath = Path.Combine(outputDir, string.Format("{0}.xml", Path.GetFileNameWithoutExtension(excelPath)));
                        TableExcelExportXml.exportExcelFile(data, exportPath);
                    }
                    break;
                case TableExportFormat.Lua:
                    {
                        var exportPath = Path.Combine(outputDir, string.Format("{0}.lua", Path.GetFileNameWithoutExtension(excelPath)));
                        TableExcelExportLua.exportExcelFile(data, exportPath);
                    }
                    break;
            }
        }
    }
}
