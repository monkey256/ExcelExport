using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace tablegen2.layouts
{
    /// <summary>
    /// PopupWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PopupWindow : Window
    {
        public bool IsEscapeClose { get; set; }
        public event Func<bool> PreClosingEvent;
        public UserControl Panel { get; internal set; }

        public PopupWindow(UserControl uc)
        {
            InitializeComponent();
            gridContainer.Children.Add(uc);
            IsEscapeClose = true;
            Panel = uc;
            
            this.MaxWidth = SystemParameters.WorkArea.Width;
            this.MaxHeight = SystemParameters.WorkArea.Height;
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.SystemIdle,
               new Action(() =>
               {
                   this.SizeToContent = SizeToContent.Manual;
                   this.MaxWidth = System.Double.PositiveInfinity;
                   this.MaxHeight = System.Double.PositiveInfinity;
               }));
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsEscapeClose && e.Key == Key.Escape)
            {
                e.Handled = true;
                this.Close();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (PreClosingEvent != null && !PreClosingEvent.Invoke())
                e.Cancel = true;
            else
                base.OnClosing(e);
        }
    }
}
