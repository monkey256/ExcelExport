using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace tablegen2
{
    internal static class Log
    {
        public static void Msg(string msg, params object[] args)
        {
            var mw = App.Current.MainWindow as MainWindow;
            if (mw != null)
                mw.addMessage(string.Format(msg, args), Color.FromArgb(0xFF, 0, 200, 0));
        }

        public static void Wrn(string msg, params object[] args)
        {
            var mw = App.Current.MainWindow as MainWindow;
            if (mw != null)
                mw.addMessage(string.Format(msg, args), Colors.Yellow);
        }

        public static void Err(string msg, params object[] args)
        {
            var mw = App.Current.MainWindow as MainWindow;
            if (mw != null)
                mw.addMessage(string.Format(msg, args), Color.FromArgb(0xFF, 255, 50, 50));
        }
    }
}
