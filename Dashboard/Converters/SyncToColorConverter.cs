using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dashboard.Converters;

public class SyncToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value is bool b && b) ? Brushes.Green : Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}