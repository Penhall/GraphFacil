using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Converters;

/// <summary>
/// Converter para cores baseadas em valores numéricos
/// </summary>
public class NumberToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double doubleValue)
        {
            if (doubleValue > 0.7)
                return System.Windows.Media.Brushes.Red; // Quente
            else if (doubleValue < 0.3)
                return System.Windows.Media.Brushes.Blue; // Frio
            else
                return System.Windows.Media.Brushes.Orange; // Normal
        }
        return System.Windows.Media.Brushes.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
