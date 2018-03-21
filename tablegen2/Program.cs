using System;
using System.Windows;

namespace tablegen2
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            ApplicationEvents.Application_Startup();
            if (CommandHelper.Command == CommandType.Help)
            {
                CommandHelper.processHelp();
            }
            else if (CommandHelper.Command == CommandType.OpenDatFile)
            {
                CommandHelper.processOpenDatFile();
            }
            else if (CommandHelper.Command == CommandType.OpenExcelFile)
            {
                CommandHelper.processOpenExcelFile();
            }
            else if (CommandHelper.Command == CommandType.ExportFiles)
            {
                CommandHelper.processExportFiles();
            }
            else
            {
                App app = new App();
                app.InitializeComponent();
                app.Run((Window)new MainWindow());
            }
            ApplicationEvents.Application_Exit();
        }
    }
}
