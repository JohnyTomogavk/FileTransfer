using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FileTransfer.Converters;

class VisibilityInvertor : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var visibilityValue = (Visibility)value;


        return visibilityValue == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return DependencyProperty.UnsetValue;
    }
}