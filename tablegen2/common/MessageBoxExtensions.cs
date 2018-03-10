using System.Windows;

namespace tablegen2
{
    public static class MessageBoxExtensions
    {
        public static void InfBox(this FrameworkElement owner, string str, params object[] args)
        {
            MessageBox.Show(
                Window.GetWindow(owner),
                string.Format(str, args),
                "提示",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        public static void WrnBox(this FrameworkElement owner, string str, params object[] args)
        {
            MessageBox.Show(
                Window.GetWindow(owner),
                string.Format(str, args),
                "警告",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        public static void ErrBox(this FrameworkElement owner, string str, params object[] args)
        {
            MessageBox.Show(
                Window.GetWindow(owner),
                string.Format(str, args),
                "错误",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static bool YesNoBox(this FrameworkElement owner, string str, params object[] args)
        {
            return MessageBox.Show(
                Window.GetWindow(owner),
                string.Format(str, args),
                "请选择",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes;
        }

        public static bool? YesNoCancelBox(this FrameworkElement owner, string str, params object[] args)
        {
            var mbr = MessageBox.Show(
                Window.GetWindow(owner),
                string.Format(str, args),
                "请选择",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
                return true;
            if (mbr == MessageBoxResult.No)
                return false;
            return null;
        }
    }
}
