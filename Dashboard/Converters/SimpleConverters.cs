// D:\PROJETOS\GraphFacil\Dashboard\Converters\SimpleConverters.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Dashboard.Converters;

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
            if (probability >= 0.8) return new SolidColorBrush(Color.FromRgb(163, 190, 140)); // Verde
            if (probability >= 0.6) return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Amarelo
            if (probability >= 0.4) return new SolidColorBrush(Color.FromRgb(208, 135, 112)); // Laranja
            return new SolidColorBrush(Color.FromRgb(191, 97, 106)); // Vermelho
        }
        return new SolidColorBrush(Color.FromRgb(136, 192, 208)); // Azul padrão
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
                ? new SolidColorBrush(Color.FromRgb(163, 190, 140)) // Verde (#A3BE8C)
                : new SolidColorBrush(Color.FromRgb(191, 97, 106));  // Vermelho (#BF616A)
        }

        return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Amarelo padrão (#EBCB8B)
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
        if (value is bool boolValue)
        {
            return boolValue ? "✅ Sim" : "❌ Não";
        }
        return "⚠️ Indefinido";
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
        return (value is bool b && b) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value is Visibility visibility && visibility == Visibility.Visible;
    }
}

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

/// <summary>
/// Converter para força em cor
/// </summary>
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

/// <summary>
/// Converter para status de validação em cor
/// </summary>
public class ValidationStatusToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string status)
        {
            if (status.Contains("sucesso") || status.Contains("✅"))
                return new SolidColorBrush(Color.FromRgb(163, 190, 140));

            if (status.Contains("erro") || status.Contains("❌"))
                return new SolidColorBrush(Color.FromRgb(191, 97, 106));

            if (status.Contains("aviso") || status.Contains("⚠️"))
                return new SolidColorBrush(Color.FromRgb(235, 203, 139));

            if (status.Contains("executando") || status.Contains("⏳"))
                return new SolidColorBrush(Color.FromRgb(136, 192, 208));

            return new SolidColorBrush(Color.FromRgb(236, 239, 244));
        }
        return new SolidColorBrush(Color.FromRgb(236, 239, 244));
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
        if (value is double performance)
        {
            if (performance >= 0.70) return new SolidColorBrush(Color.FromRgb(163, 190, 140)); // Verde excelente
            if (performance >= 0.65) return new SolidColorBrush(Color.FromRgb(235, 203, 139)); // Amarelo bom
            if (performance >= 0.60) return new SolidColorBrush(Color.FromRgb(208, 135, 112)); // Laranja médio
            return new SolidColorBrush(Color.FromRgb(191, 97, 106)); // Vermelho ruim
        }
        return new SolidColorBrush(Color.FromRgb(136, 192, 208)); // Azul indefinido
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
        if (value is int count)
        {
            if (count == 15) return "✅ Padrão Lotofácil";
            if (count < 15) return $"⚠️ Insuficiente ({count}/15)";
            if (count > 15) return $"❌ Excesso ({count}/15)";
            return "❓ Indefinido";
        }
        return "❓ Indefinido";
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
        if (value is double confidence)
        {
            var percentage = confidence * 100;
            var level = percentage switch
            {
                >= 90 => "🎯 Muito Alta",
                >= 80 => "✅ Alta",
                >= 70 => "⚡ Boa",
                >= 60 => "⚠️ Média",
                >= 50 => "🔄 Baixa",
                _ => "❌ Muito Baixa"
            };

            return $"{level} ({percentage:F1}%)";
        }
        return "❓ Indefinida";
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
        if (value is bool isInitialized)
        {
            return isInitialized ? "🚀" : "⏳";
        }

        if (value is string status)
        {
            var lowerStatus = status.ToLower();
            if (lowerStatus.Contains("inicializado")) return "🚀";
            if (lowerStatus.Contains("inicializando")) return "⏳";
            if (lowerStatus.Contains("erro")) return "❌";
            if (lowerStatus.Contains("aguardando")) return "⏸️";
            return "❓";
        }

        return "❓";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}