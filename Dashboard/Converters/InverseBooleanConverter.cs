using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Converters;

/// <summary>
/// Converter que inverte um valor booleano
/// </summary>
public class InverseBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true; // Valor padrão se não for booleano
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return false; // Valor padrão se não for booleano
    }
}
