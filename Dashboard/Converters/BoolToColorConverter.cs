using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Dashboard.Converters
{
    /// <summary>
    /// Converter para transformar bool em cores para status
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue
                    ? new SolidColorBrush(Color.FromRgb(163, 190, 140)) // Verde (#FFA3BE8C)
                    : new SolidColorBrush(Color.FromRgb(191, 97, 106));  // Vermelho (#FFBF616A)
            }

            return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Amarelo padrão (#FFEBCB8B)
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

