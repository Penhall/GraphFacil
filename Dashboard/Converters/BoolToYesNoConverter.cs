using System;
using System.Globalization;
using System.Windows.Data;

namespace Dashboard.Converters
{
    /// <summary>
    /// Converte um valor booleano para as strings "Sim" ou "Não".
    /// </summary>
    public class BoolToYesNoConverter : IValueConverter
    {
        /// <summary>
        /// Converte um booleano para "Sim" (true) ou "Não" (false).
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Sim" : "Não";
            }
            return "Não";
        }

        /// <summary>
        /// Converte "Sim" de volta para true, e qualquer outra coisa para false.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return string.Equals(stringValue, "Sim", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
