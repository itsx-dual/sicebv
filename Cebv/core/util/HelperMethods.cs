using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace Cebv.core.util;

public class HelperMethods
{
    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if (depObj != null)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if (child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach (T childofChild in FindVisualChildren<T>(child))
                {
                    yield return childofChild;
                }
            }
        }
    }
}