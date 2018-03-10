using System;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace tablegen2
{
    internal static class FrameworkExtension
    {
        public static Vector GetOffsetFromRelativeParent(this FrameworkElement e, FrameworkElement p)
        {
            p.UpdateLayout();

            var r = new Vector(0, 0);
            var t = e;
            while (t != p)
            {
                if (t == null)
                    throw new Exception("GetOffsetFromRelativeParent 异常！");

                var v = VisualTreeHelper.GetOffset(t);
                r += v;
                t = VisualTreeHelper.GetParent(t) as FrameworkElement;
            }

            return r;
        }

        public static void MakesureChildVisible(this ScrollViewer sv, FrameworkElement child)
        {
            if (child == null)
                return;

            var reserve = 20;

            var p = child.GetOffsetFromRelativeParent(sv);
            p.X += sv.HorizontalOffset;
            p.Y += sv.VerticalOffset;

            if (sv.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                var x1 = p.X - reserve;
                var x2 = p.X + child.ActualWidth + reserve;
                var offsetx = sv.HorizontalOffset;
                var width = sv.ActualWidth;
                if (x1 < offsetx)
                    offsetx = x1;
                else if (offsetx + width < x2)
                    offsetx = x2 - width;
                sv.ScrollToHorizontalOffset(offsetx);
            }

            if (sv.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled)
            {
                var y1 = p.Y - reserve;
                var y2 = p.Y + child.ActualHeight + reserve;
                var offsety = sv.VerticalOffset;
                var height = sv.ActualHeight;
                if (y1 < offsety)
                    offsety = y1;
                else if (offsety + height < y2)
                    offsety = y2 - height;
                sv.ScrollToVerticalOffset(offsety);
            }
        }

        public static void SelectComboBoxItemByTag(this ComboBox cb, string tagString)
        {
            cb.SelectedItem = cb.Items.Cast<ComboBoxItem>().FirstOrDefault(a => (a.Tag as string) == tagString);
        }
    }
}
