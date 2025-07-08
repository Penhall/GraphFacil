// D:\PROJETOS\GraphFacil\Dashboard\Converters\SimpleConverters.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Dashboard.Converters
{
    /// <summary>
    /// Converte string para Visibility (Visible se não vazio)
    /// </summary>
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return string.IsNullOrWhiteSpace(stringValue) ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte probabilidade para cor da barra
    /// </summary>
    public class ProbabilityToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double probability)
            {
                if (probability >= 0.7) return new SolidColorBrush(Colors.LimeGreen);
                if (probability >= 0.5) return new SolidColorBrush(Colors.Orange);
                if (probability >= 0.3) return new SolidColorBrush(Colors.Gold);
                return new SolidColorBrush(Colors.LightCoral);
            }
            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte tipo de metrônomo para ícone simples
    /// </summary>
    public class MetronomoTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                var tipo = value.ToString();
                return tipo switch
                {
                    "Regular" => "ClockOutline",
                    "Alternado" => "SwapHorizontal", 
                    "CicloLongo" => "Refresh",
                    "Tendencial" => "TrendingUp",
                    "MultiModal" => "Equalizer",
                    "Irregular" => "Help",
                    _ => "QuestionMark"
                };
            }
            return "QuestionMark";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte booleano invertido (para IsEnabled)
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return !boolValue;
            }
            return false;
        }
    }
}