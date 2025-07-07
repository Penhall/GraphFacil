using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Converters;

/// <summary>
/// Converter para sincronização para cores
/// </summary>
public class SyncToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool isSync)
        {
            return isSync ?
                System.Windows.Media.Brushes.Green :
                System.Windows.Media.Brushes.LightGray;
        }
        return System.Windows.Media.Brushes.LightGray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
