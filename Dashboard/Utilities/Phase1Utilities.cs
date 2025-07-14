using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Dashboard.Utilities
{
    /// <summary>
    /// Utilitários específicos para a Fase 1 da implementação
    /// </summary>
    public static class Phase1Utilities
    {
        #region Constants

        private const string PHASE1_CONFIG_FILE = "phase1_config.json";
        private const string VALIDATION_RESULTS_FOLDER = "ValidationResults";
        private const string DIAGNOSTIC_RESULTS_FOLDER = "DiagnosticResults";

        #endregion

        #region UI Utilities

        /// <summary>
        /// Executa uma ação na thread da UI de forma segura
        /// </summary>
        public static void InvokeOnUI(Action action)
        {
            if (Application.Current?.Dispatcher?.CheckAccess() == true)
            {
                action();
            }
            else
            {
                Application.Current?.Dispatcher?.Invoke(action);
            }
        }

        /// <summary>
        /// Executa uma ação na thread da UI de forma assíncrona
        /// </summary>
        public static Task InvokeOnUIAsync(Action action)
        {
            if (Application.Current?.Dispatcher?.CheckAccess() == true)
            {
                action();
                return Task.CompletedTask;
            }
            else
            {
                return Application.Current?.Dispatcher?.InvokeAsync(action).Task ?? Task.CompletedTask;
            }
        }

        /// <summary>
        /// Formata números de previsão para exibição
        /// </summary>
        public static string FormatPredictionNumbers(int[] numbers)
        {
            if (numbers == null || numbers.Length == 0)
                return string.Empty;

            return string.Join(", ", numbers.OrderBy(n => n));
        }

        /// <summary>
        /// Mostra uma mensagem de notificação não intrusiva
        /// </summary>
        public static void ShowNotification(string title, string message, NotificationType type = NotificationType.Info)
        {
            InvokeOnUI(() =>
            {
                try
                {
                    var icon = type switch
                    {
                        NotificationType.Success => MessageBoxImage.Information,
                        NotificationType.Warning => MessageBoxImage.Warning,
                        NotificationType.Error => MessageBoxImage.Error,
                        _ => MessageBoxImage.Information
                    };

                    MessageBox.Show(message, title, MessageBoxButton.OK, icon);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao mostrar notificação: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// Encontra um elemento visual pai de um tipo específico
        /// </summary>
        public static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            if (parent == null)
                return null;

            if (parent is T parentT)
                return parentT;

            return FindVisualParent<T>(parent);
        }

        /// <summary>
        /// Encontra um elemento visual filho de um tipo específico
        /// </summary>
        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T childT)
                    return childT;

                var childOfChild = FindVisualChild<T>(child);
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        #endregion

        #region File and Directory Utilities

        /// <summary>
        /// Cria diretórios necessários para a Fase 1
        /// </summary>
        public static void EnsureDirectoriesExist()
        {
            try
            {
                var directories = new[]
                {
                    VALIDATION_RESULTS_FOLDER,
                    DIAGNOSTIC_RESULTS_FOLDER,
                    "Logs",
                    "Reports",
                    "Exports"
                };

                foreach (var dir in directories)
                {
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar diretórios: {ex.Message}");
            }
        }

        /// <summary>
        /// Salva dados de validação em arquivo JSON
        /// </summary>
        public static async Task<bool> SaveValidationResultsAsync(object data, string fileName = null)
        {
            try
            {
                EnsureDirectoriesExist();

                fileName ??= $"validation_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(VALIDATION_RESULTS_FOLDER, fileName);

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar resultados de validação: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Salva dados de diagnóstico em arquivo JSON
        /// </summary>
        public static async Task<bool> SaveDiagnosticResultsAsync(object data, string fileName = null)
        {
            try
            {
                EnsureDirectoriesExist();

                fileName ??= $"diagnostic_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                var filePath = Path.Combine(DIAGNOSTIC_RESULTS_FOLDER, fileName);

                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(filePath, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar resultados de diagnóstico: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Abre pasta de resultados no Explorer
        /// </summary>
        public static void OpenResultsFolder(string subfolder = null)
        {
            try
            {
                var folderPath = subfolder ?? Directory.GetCurrentDirectory();

                if (Directory.Exists(folderPath))
                {
                    Process.Start("explorer.exe", folderPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao abrir pasta: {ex.Message}");
            }
        }

        #endregion

        #region Configuration Utilities

        /// <summary>
        /// Carrega configurações da Fase 1
        /// </summary>
        public static Phase1Config LoadConfiguration()
        {
            try
            {
                if (File.Exists(PHASE1_CONFIG_FILE))
                {
                    var json = File.ReadAllText(PHASE1_CONFIG_FILE);
                    return JsonSerializer.Deserialize<Phase1Config>(json) ?? new Phase1Config();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao carregar configuração: {ex.Message}");
            }

            return new Phase1Config();
        }

        /// <summary>
        /// Salva configurações da Fase 1
        /// </summary>
        public static async Task<bool> SaveConfigurationAsync(Phase1Config config)
        {
            try
            {
                var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await File.WriteAllTextAsync(PHASE1_CONFIG_FILE, json);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao salvar configuração: {ex.Message}");
                return false;
            }
        }

        #endregion

        #region Performance Utilities

        /// <summary>
        /// Mede o tempo de execução de uma ação
        /// </summary>
        public static TimeSpan MeasureExecutionTime(Action action)
        {
            var stopwatch = Stopwatch.StartNew();
            action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Mede o tempo de execução de uma ação assíncrona
        /// </summary>
        public static async Task<TimeSpan> MeasureExecutionTimeAsync(Func<Task> action)
        {
            var stopwatch = Stopwatch.StartNew();
            await action();
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        /// <summary>
        /// Obtém informações de sistema para diagnóstico
        /// </summary>
        public static SystemInfo GetSystemInfo()
        {
            try
            {
                return new SystemInfo
                {
                    ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
                    FrameworkVersion = Environment.Version.ToString(),
                    OperatingSystem = Environment.OSVersion.ToString(),
                    MachineName = Environment.MachineName,
                    UserName = Environment.UserName,
                    ProcessorCount = Environment.ProcessorCount,
                    WorkingSet = Environment.WorkingSet,
                    TotalMemory = GC.GetTotalMemory(false),
                    CurrentTimestamp = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter informações do sistema: {ex.Message}");
                return new SystemInfo { ErrorMessage = ex.Message };
            }
        }

        #endregion

        #region Validation Utilities

        /// <summary>
        /// Valida se uma lista de dezenas está no formato correto da Lotofácil
        /// </summary>
        public static bool ValidateLotofacilNumbers(IEnumerable<int> numbers)
        {
            try
            {
                var numbersList = numbers?.ToList();

                if (numbersList == null || numbersList.Count != 15)
                    return false;

                // Verifica se todas as dezenas estão entre 1 e 25
                if (numbersList.Any(n => n < 1 || n > 25))
                    return false;

                // Verifica se não há duplicatas
                if (numbersList.Distinct().Count() != 15)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Analisa a distribuição de dezenas baixas (1-9) vs altas (10-25)
        /// </summary>
        public static DistributionAnalysis AnalyzeNumberDistribution(IEnumerable<int> numbers)
        {
            try
            {
                var numbersList = numbers?.ToList() ?? new List<int>();

                var lowNumbers = numbersList.Where(n => n >= 1 && n <= 9).Count();
                var highNumbers = numbersList.Where(n => n >= 10 && n <= 25).Count();

                return new DistributionAnalysis
                {
                    TotalNumbers = numbersList.Count,
                    LowNumbers = lowNumbers,
                    HighNumbers = highNumbers,
                    LowPercentage = numbersList.Count > 0 ? (double)lowNumbers / numbersList.Count : 0,
                    HighPercentage = numbersList.Count > 0 ? (double)highNumbers / numbersList.Count : 0,
                    IsBalanced = lowNumbers >= 3 && lowNumbers <= 7 // Considerado balanceado entre 3-7 dezenas baixas
                };
            }
            catch (Exception ex)
            {
                return new DistributionAnalysis { ErrorMessage = ex.Message };
            }
        }

        #endregion

        #region String Utilities

        /// <summary>
        /// Formata lista de dezenas para exibição
        /// </summary>
        public static string FormatNumbersForDisplay(IEnumerable<int> numbers)
        {
            try
            {
                return string.Join(", ", numbers.OrderBy(n => n).Select(n => n.ToString("D2")));
            }
            catch
            {
                return "Erro na formatação";
            }
        }

        /// <summary>
        /// Trunca texto se for muito longo
        /// </summary>
        public static string TruncateText(string text, int maxLength, string suffix = "...")
        {
            if (string.IsNullOrEmpty(text) || text.Length <= maxLength)
                return text ?? string.Empty;

            return text.Substring(0, maxLength - suffix.Length) + suffix;
        }

        #endregion
    }

    #region Support Classes

    /// <summary>
    /// Configurações da Fase 1
    /// </summary>
    public class Phase1Config
    {
        public bool EnableDiagnostics { get; set; } = true;
        public bool EnableLogging { get; set; } = true;
        public bool AutoSaveResults { get; set; } = true;
        public int MaxLogFiles { get; set; } = 10;
        public int ValidationTimeout { get; set; } = 30; // segundos
        public string PreferredLanguage { get; set; } = "pt-BR";
        public Dictionary<string, object> CustomSettings { get; set; } = new();
    }

    /// <summary>
    /// Informações do sistema
    /// </summary>
    public class SystemInfo
    {
        public string ApplicationVersion { get; set; } = string.Empty;
        public string FrameworkVersion { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string MachineName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int ProcessorCount { get; set; }
        public long WorkingSet { get; set; }
        public long TotalMemory { get; set; }
        public DateTime CurrentTimestamp { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Análise de distribuição de números
    /// </summary>
    public class DistributionAnalysis
    {
        public int TotalNumbers { get; set; }
        public int LowNumbers { get; set; }
        public int HighNumbers { get; set; }
        public double LowPercentage { get; set; }
        public double HighPercentage { get; set; }
        public bool IsBalanced { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Tipos de notificação
    /// </summary>
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error
    }

    #endregion

    #region Extension Methods

    /// <summary>
    /// Extensões para facilitar o trabalho com collections
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Verifica se uma coleção é nula ou vazia
        /// </summary>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return source == null || !source.Any();
        }

        /// <summary>
        /// Executa uma ação para cada item da coleção
        /// </summary>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// Divide uma coleção em batches de tamanho específico
        /// </summary>
        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
        {
            var batch = new List<T>();
            foreach (var item in source)
            {
                batch.Add(item);
                if (batch.Count == batchSize)
                {
                    yield return batch;
                    batch = new List<T>();
                }
            }

            if (batch.Any())
                yield return batch;
        }
    }

    /// <summary>
    /// Extensões para trabalhar com strings
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Verifica se uma string é nula, vazia ou contém apenas espaços
        /// </summary>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        /// <summary>
        /// Capitaliza a primeira letra de uma string
        /// </summary>
        public static string Capitalize(this string value)
        {
            if (value.IsNullOrWhiteSpace())
                return value;

            return char.ToUpper(value[0]) + value.Substring(1).ToLower();
        }
    }

    #endregion
}
