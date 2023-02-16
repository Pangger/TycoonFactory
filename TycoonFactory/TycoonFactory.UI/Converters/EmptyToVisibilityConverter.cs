using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace TycoonFactory.UI.Converters;

public class EmptyToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string str && !string.IsNullOrEmpty(str))
            return Visibility.Visible;
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Visibility visibility && visibility == Visibility.Visible)
            return true;
        return false;
    }
}