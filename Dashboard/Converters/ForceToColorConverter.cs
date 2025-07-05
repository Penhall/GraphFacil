using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dashboard.Converters;

public class ForceToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        double force = (double)value;
        return new SolidColorBrush(Color.FromRgb(
            (byte)(255 * (1 - force)),  // Menos vermelho quando mais forte
            (byte)(255 * force),         // Mais verde quando mais forte
            100));
    }


    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}