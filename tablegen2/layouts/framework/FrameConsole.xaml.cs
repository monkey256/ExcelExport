using System;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace tablegen2.layouts
{
    /// <summary>
    /// FrameConsole.xaml 的交互逻辑
    /// </summary>
    public partial class FrameConsole : UserControl
    {
        public FrameConsole()
        {
            InitializeComponent();
        }

        public void addMessage(string msg, Color color)
        {
            Run run = new Run()
            {
                Text = string.Format("{0}    {1}\n", DateTime.Now.ToString("HH:mm:ss"), msg),
                Foreground = new SolidColorBrush(color),
            };
            phMessage.Inlines.Add(run);
            rtxt.ScrollToEnd();
        }

        public void clearMessage()
        {
            phMessage.Inlines.Clear();
        }
    }
}
