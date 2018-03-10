using System;
using System.IO;
using System.Text;
using System.Windows;
using tablegen2.logic;
using tablegen2.layouts;

namespace tablegen2
{
    internal enum CommandType
    {
        Unknown = 0,
        Help,
        OpenDatFile,
        OpenExcelFile,
    }

    internal static class CommandHelper
    {
        //-h|-help|/h|/help
        //fullPath.exdat
        //fullPath.xls|.xlsx

        public static CommandType Command { get; internal set; }

        public static string OpenDatFullPath { get; internal set; }

        public static string OpenExcelFullPath { get; internal set; }

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
    }
}
