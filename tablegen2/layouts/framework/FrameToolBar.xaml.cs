using System;
using System.Windows;
using System.Windows.Controls;

namespace tablegen2.layouts
{
    /// <summary>
    /// FrameToolBar.xaml 的交互逻辑
    /// </summary>
    public partial class FrameToolBar : UserControl
    {
        public event Action OpenExcelEvent;

        public FrameToolBar()
        {
            InitializeComponent();
        }

        private void btnOpenExcel_Clicked(object sender, RoutedEventArgs e)
        {
            if (OpenExcelEvent != null)
                OpenExcelEvent.Invoke();
        }
    }
}
