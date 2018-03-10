using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace tablegen2.layouts
{
    /// <summary>
    /// WaitingControl.xaml 的交互逻辑
    /// </summary>
    public partial class WaitingControl : UserControl
    {
        private bool is_busy_;
        public int MinElapse { get; set; }

        public WaitingControl()
        {
            is_busy_ = false;
            MinElapse = 500;
            InitializeComponent();
        }

        public void BeginAction(Action func, Action callback)
        {
            if (is_busy_)
            {
                throw new Exception("Waiting Control is busy now, can't start new action!");
            }

            var sb = this.Resources["sbStory"] as Storyboard;
            sb.Begin();

            this.Visibility = Visibility.Visible;
            this.Focus();

            is_busy_ = true;
            Action f = () =>
            {
                int tick = Environment.TickCount;

                func();

                while (Environment.TickCount - tick < MinElapse)
                    Thread.Sleep(1);

                this.Dispatcher.Invoke(
                    DispatcherPriority.Background, 
                    new Action(() =>
                    {
                        this.Visibility = Visibility.Hidden;
                        sb.Stop();
                        if (callback != null)
                            callback.Invoke();
                        is_busy_ = false;
                    }));
            };

            var th = new Thread(new ThreadStart(f));
            th.Start();
        }

        public bool IsBusy
        {
            get { return is_busy_; }
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
