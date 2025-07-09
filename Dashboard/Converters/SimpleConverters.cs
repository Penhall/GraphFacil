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
            try
            {
                if (value is string stringValue)
                {
                    return string.IsNullOrWhiteSpace(stringValue) ? Visibility.Collapsed : Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
            }
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
            try
            {
                if (value is double probability)
                {
                    if (probability >= 0.8) return new SolidColorBrush(Color.FromRgb(163, 190, 140)); // Verde
                    if (probability >= 0.6) return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Amarelo
                    if (probability >= 0.4) return new SolidColorBrush(Color.FromRgb(208, 135, 112)); // Laranja
                    return new SolidColorBrush(Color.FromRgb(191, 97, 106)); // Vermelho
                }
                return new SolidColorBrush(Color.FromRgb(136, 192, 208)); // Azul padrão
            }
            catch
            {
                return new SolidColorBrush(Color.FromRgb(136, 192, 208)); // Fallback seguro
            }
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
            try
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
            catch
            {
                return "QuestionMark";
            }
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
            try
            {
                if (value is bool boolValue)
                {
                    return !boolValue;
                }
                return true;
            }
            catch
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool boolValue)
                {
                    return !boolValue;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Converter para transformar bool em cores para status
    /// </summary>
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool boolValue)
                {
                    return boolValue
                        ? new SolidColorBrush(Color.FromRgb(163, 190, 140)) // Verde (#A3BE8C)
                        : new SolidColorBrush(Color.FromRgb(191, 97, 106));  // Vermelho (#BF616A)
                }
                return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Amarelo padrão (#EBCB8B)
            }
            catch
            {
                return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Fallback seguro
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para transformar bool em texto Sim/Não
    /// </summary>
    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool boolValue)
                {
                    return boolValue ? "✅ Sim" : "❌ Não";
                }
                return "⚠️ Indefinido";
            }
            catch
            {
                return "⚠️ Erro";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converte bool para Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return value is Visibility visibility && visibility == Visibility.Visible;
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Converter para sincronização para cores
    /// </summary>
    public class SyncToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool isSync)
                {
                    return isSync ? Brushes.Green : Brushes.LightGray;
                }
                return Brushes.LightGray;
            }
            catch
            {
                return Brushes.LightGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para força em cor - VERSÃO CORRIGIDA
    /// </summary>
    public class ForceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value == null)
                    return new SolidColorBrush(Colors.Gray);

                double force = 0.5; // Valor padrão

                if (value is double doubleValue)
                {
                    force = doubleValue;
                }
                else if (value is float floatValue)
                {
                    force = floatValue;
                }
                else if (value is int intValue)
                {
                    force = intValue / 100.0; // Assumindo que int representa percentual
                }
                else
                {
                    // Tentar converter string ou outros tipos
                    if (double.TryParse(value.ToString(), out var parsedValue))
                    {
                        force = parsedValue;
                    }
                }

                // Garantir que force está no range 0-1
                force = Math.Max(0, Math.Min(1, force));

                return new SolidColorBrush(Color.FromRgb(
                    (byte)(255 * (1 - force)),  // Menos vermelho quando mais forte
                    (byte)(255 * force),         // Mais verde quando mais forte
                    100));
            }
            catch
            {
                return new SolidColorBrush(Colors.Gray);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para status de validação em cor
    /// </summary>
    public class ValidationStatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is string status && !string.IsNullOrEmpty(status))
                {
                    var lowerStatus = status.ToLower();

                    if (lowerStatus.Contains("sucesso") || lowerStatus.Contains("✅"))
                        return new SolidColorBrush(Color.FromRgb(163, 190, 140));

                    if (lowerStatus.Contains("erro") || lowerStatus.Contains("❌"))
                        return new SolidColorBrush(Color.FromRgb(191, 97, 106));

                    if (lowerStatus.Contains("aviso") || lowerStatus.Contains("⚠️"))
                        return new SolidColorBrush(Color.FromRgb(235, 203, 139));

                    if (lowerStatus.Contains("executando") || lowerStatus.Contains("⏳"))
                        return new SolidColorBrush(Color.FromRgb(136, 192, 208));
                }
                return new SolidColorBrush(Color.FromRgb(236, 239, 244));
            }
            catch
            {
                return new SolidColorBrush(Color.FromRgb(236, 239, 244));
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para performance em cor
    /// </summary>
    public class PerformanceToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is double performance)
                {
                    if (performance >= 0.70) return new SolidColorBrush(Color.FromRgb(163, 190, 140)); // Verde excelente
                    if (performance >= 0.65) return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Amarelo bom
                    if (performance >= 0.60) return new SolidColorBrush(Color.FromRgb(208, 135, 112)); // Laranja médio
                    return new SolidColorBrush(Color.FromRgb(191, 97, 106)); // Vermelho ruim
                }
                return new SolidColorBrush(Color.FromRgb(136, 192, 208)); // Azul indefinido
            }
            catch
            {
                return new SolidColorBrush(Color.FromRgb(136, 192, 208)); // Fallback
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para número de dezenas em texto descritivo
    /// </summary>
    public class DezenasCountToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is int count)
                {
                    if (count == 15) return "✅ Padrão Lotofácil";
                    if (count < 15) return $"⚠️ Insuficiente ({count}/15)";
                    if (count > 15) return $"❌ Excesso ({count}/15)";
                }
                return "❓ Indefinido";
            }
            catch
            {
                return "❓ Erro";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para tempo de processamento em texto amigável
    /// </summary>
    public class ProcessingTimeToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is TimeSpan timeSpan)
                {
                    if (timeSpan.TotalSeconds < 1)
                        return $"{timeSpan.TotalMilliseconds:F0}ms";
                    if (timeSpan.TotalMinutes < 1)
                        return $"{timeSpan.TotalSeconds:F1}s";
                    return $"{timeSpan.TotalMinutes:F1}min";
                }
                return "0ms";
            }
            catch
            {
                return "0ms";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para nível de confiança em cor e texto
    /// </summary>
    public class ConfidenceToDisplayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is double confidence)
                {
                    var percentage = confidence * 100;

                    var level = "❓ Indefinida";
                    if (percentage >= 90) level = "🎯 Muito Alta";
                    else if (percentage >= 80) level = "✅ Alta";
                    else if (percentage >= 70) level = "⚡ Boa";
                    else if (percentage >= 60) level = "⚠️ Média";
                    else if (percentage >= 50) level = "🔄 Baixa";
                    else level = "❌ Muito Baixa";

                    return $"{level} ({percentage:F1}%)";
                }
                return "❓ Indefinida";
            }
            catch
            {
                return "❓ Erro";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para status do PredictionEngine em ícone
    /// </summary>
    public class EngineStatusToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is bool isInitialized)
                {
                    return isInitialized ? "🚀" : "⏳";
                }

                if (value is string status && !string.IsNullOrEmpty(status))
                {
                    var lowerStatus = status.ToLower();
                    if (lowerStatus.Contains("inicializado")) return "🚀";
                    if (lowerStatus.Contains("inicializando")) return "⏳";
                    if (lowerStatus.Contains("erro")) return "❌";
                    if (lowerStatus.Contains("aguardando")) return "⏸️";
                }

                return "❓";
            }
            catch
            {
                return "❓";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter simples para números em formato D2
    /// </summary>
    public class NumberToD2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is int number)
                {
                    return number.ToString("D2");
                }
                return value?.ToString() ?? "00";
            }
            catch
            {
                return "00";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converter para formato de percentual
    /// </summary>
    public class PercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (value is double doubleValue)
                {
                    return $"{doubleValue:P1}";
                }
                if (value is float floatValue)
                {
                    return $"{floatValue:P1}";
                }
                return "0,0%";
            }
            catch
            {
                return "0,0%";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
