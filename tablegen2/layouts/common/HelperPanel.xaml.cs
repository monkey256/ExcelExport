using System.Windows;
using System.Windows.Controls;

namespace tablegen2.layouts
{
    /// <summary>
    /// HelperPanel.xaml 的交互逻辑
    /// </summary>
    public partial class HelperPanel : UserControl
    {
        public static readonly string HelpString =
@"1.Excel格式说明：

  * 目前字段类型支持
        string（字符串）
        int（整型）
        double（小数）

  * 每张表须包含两个字段名称固定且值不能重复的字段
        Id（全局唯一数字索引）
        KeyName（全局唯一字符串索引）

  * 每张表须包含两个工作簿
        字段工作簿（用于声明字段名称、字段类型、字段描述）
        数据工作簿（用于填充整张表的行数据）

2.工具操作说明：

    * 用户配置栏
        配置目录----Excel表所在的目录
        输出目录----生成数据文件的存放目录
        输出格式----选择生成数据文件的格式

    * 左侧列表栏
        可使用右上角的‘+’按钮通过可视化界面来新建Excel表格文件
        也可以在空白地方右键使用[新建Excel表...]命令来新建表格
        可使用右键菜单中的[美化Excel表]命令来规范化Excel文件
        可使用右键菜单中的[生成数据]命令来单独生成某个数据表文件
        可使用右键菜单中的[编辑Excel...]命令通过可视化界面来操作数据内容

    * 右侧输出栏
        在执行命令时，会将执行过程中的日志信息打印输出到该窗口中

3.通过命令行启动说明：

    * 使用工具查看并编辑指定Excel文件
        tablegen2.exe excelFullPath.xls(.xlsx)

    * 使用工具查看并编辑指定.exdat（加密数据格式）文件
        tablegen2.exe exdatFullPath.exdat

    * 使用工具将Excel文件或所在的整个目录导出到指定格式的数据文件
        tablegen2.exe -i excelFullPath|excelDir -o outputDir -t xml|json|lua|dat
";
        public HelperPanel()
        {
            InitializeComponent();
            setString(HelpString);
        }

        public void setString(string str)
        {
            txt.Text = str;
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).DialogResult = true;
        }
    }
}
