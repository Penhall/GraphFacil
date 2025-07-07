// Dashboard/Converters/ValueConverters.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Dashboard.Converters
{
    /// <summary>
    /// Converte booleano para Visibility
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is Visibility visibility && visibility == Visibility.Visible;
        }
    }

    /// <summary>
    /// Converte booleano invertido
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
    /// Converte booleano para Verde/Vermelho
    /// </summary>
    public class BoolToGreenRedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue 
                    ? new SolidColorBrush(Colors.LimeGreen) 
                    : new SolidColorBrush(Colors.LightGray);
            }
            return new SolidColorBrush(Colors.LightGray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte booleano para Sim/Não
    /// </summary>
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "SIM" : "NÃO";
            }
            return "NÃO";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string stringValue)
            {
                return stringValue.ToUpper() == "SIM";
            }
            return false;
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
    /// Converte tipo de metrônomo para ícone
    /// </summary>
    public class MetronomoTypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is LotoLibrary.Services.TipoMetronomo tipo)
            {
                return tipo switch
                {
                    LotoLibrary.Services.TipoMetronomo.Regular => "ClockOutline",
                    LotoLibrary.Services.TipoMetronomo.Alternado => "SwapHorizontal",
                    LotoLibrary.Services.TipoMetronomo.CicloLongo => "Refresh",
                    LotoLibrary.Services.TipoMetronomo.Tendencial => "TrendingUp",
                    LotoLibrary.Services.TipoMetronomo.MultiModal => "Equalizer",
                    LotoLibrary.Services.TipoMetronomo.Irregular => "Help",
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
}