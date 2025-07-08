// D:\PROJETOS\GraphFacil\Dashboard\App.xaml.cs
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// Fase 1 - Inicialização e configuração global da aplicação
    /// </summary>
    public partial class App : Application
    {
        #region Constants

        private const string APP_NAME = "GraphFacil - Sistema Lotofácil";
        private const string VERSION = "2.0 - Fase 1";
        private const string LOG_FOLDER = "Logs";

        #endregion

        #region Application Lifecycle

        /// <summary>
        /// Executa na inicialização da aplicação
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                // Configurações básicas da aplicação
                SetupApplicationDefaults();

                // Inicializa sistema de logging
                InitializeLogging();

                // Configura tratamento global de exceções
                SetupGlobalExceptionHandling();

                // Log da inicialização
                LogInfo("Aplicação iniciada com sucesso");
                LogInfo($"Versão: {VERSION}");
                LogInfo($"Argumentos: {string.Join(" ", e.Args)}");

                // Processa argumentos da linha de comando se houver
                ProcessCommandLineArguments(e.Args);

                base.OnStartup(e);
            }
            catch (Exception ex)
            {
                HandleStartupException(ex);
            }
        }

        /// <summary>
        /// Executa quando a aplicação está sendo fechada
        /// </summary>
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                LogInfo("Aplicação sendo fechada");

                // Cleanup de recursos se necessário
                CleanupResources();

                LogInfo("Aplicação fechada com sucesso");
            }
            catch (Exception ex)
            {
                LogError("Erro durante fechamento da aplicação", ex);
            }

            base.OnExit(e);
        }

        #endregion

        #region Configuration Methods

        /// <summary>
        /// Configura padrões da aplicação
        /// </summary>
        private void SetupApplicationDefaults()
        {
            // Configura cultura da aplicação (importante para formatação de números)
            System.Threading.Thread.CurrentThread.CurrentCulture =
                System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR");

            System.Threading.Thread.CurrentThread.CurrentUICulture =
                System.Globalization.CultureInfo.CreateSpecificCulture("pt-BR");

            // Configura modo de renderização para melhor performance
            System.Windows.Media.RenderOptions.ProcessRenderMode =
                System.Windows.Interop.RenderMode.SoftwareOnly;

            // Configura shutdown mode
            ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        /// <summary>
        /// Inicializa sistema de logging básico
        /// </summary>
        private void InitializeLogging()
        {
            try
            {
                // Cria pasta de logs se não existir
                if (!Directory.Exists(LOG_FOLDER))
                {
                    Directory.CreateDirectory(LOG_FOLDER);
                }

                // Limita número de arquivos de log antigos
                CleanupOldLogFiles();
            }
            catch (Exception ex)
            {
                // Se não conseguir criar logs, continua sem eles
                MessageBox.Show($"Aviso: Não foi possível inicializar sistema de logs.\n{ex.Message}",
                    "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// Configura tratamento global de exceções
        /// </summary>
        private void SetupGlobalExceptionHandling()
        {
            // Exceções em threads da UI
            DispatcherUnhandledException += App_DispatcherUnhandledException;

            // Exceções em outras threads
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Exceções em tasks
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        }

        #endregion

        #region Exception Handlers

        /// <summary>
        /// Manipula exceções não tratadas na UI thread
        /// </summary>
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                LogError("Exceção não tratada na UI thread", e.Exception);

                var result = MessageBox.Show(
                    $"Ocorreu um erro inesperado:\n\n{e.Exception.Message}\n\n" +
                    $"Deseja continuar executando a aplicação?",
                    "Erro Inesperado",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Error);

                if (result == MessageBoxResult.Yes)
                {
                    e.Handled = true; // Continua executando
                }
                else
                {
                    Shutdown(1); // Fecha a aplicação
                }
            }
            catch
            {
                // Se não conseguir nem mostrar o erro, força o fechamento
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Manipula exceções não tratadas em outras threads
        /// </summary>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                if (e.ExceptionObject is Exception ex)
                {
                    LogError("Exceção não tratada em thread secundária", ex);
                }

                if (e.IsTerminating)
                {
                    LogError("Aplicação sendo terminada devido a exceção fatal", null);
                }
            }
            catch
            {
                // Ignora erros no tratamento de exceções
            }
        }

        /// <summary>
        /// Manipula exceções não observadas em tasks
        /// </summary>
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            try
            {
                LogError("Exceção não observada em Task", e.Exception);
                e.SetObserved(); // Marca como observada para evitar crash
            }
            catch
            {
                // Ignora erros no tratamento de exceções
            }
        }

        /// <summary>
        /// Manipula exceções durante inicialização
        /// </summary>
        private void HandleStartupException(Exception ex)
        {
            try
            {
                var message = $"Erro crítico durante inicialização da aplicação:\n\n{ex.Message}\n\n" +
                             $"A aplicação será fechada.";

                MessageBox.Show(message, "Erro de Inicialização",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Environment.Exit(1);
            }
        }

        #endregion

        #region Command Handlers

        /// <summary>
        /// Manipula comando global de saída
        /// </summary>
        private void ExitApplication_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var result = MessageBox.Show(
                    "Deseja realmente fechar a aplicação?",
                    "Confirmar Saída",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    Shutdown();
                }
            }
            catch (Exception ex)
            {
                LogError("Erro ao executar comando de saída", ex);
            }
        }

        /// <summary>
        /// Manipula comando para mostrar "Sobre"
        /// </summary>
        private void ShowAbout_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var aboutMessage = $"{APP_NAME}\n" +
                                  $"Versão: {VERSION}\n" +
                                  $"Desenvolvido para análise e predição da Lotofácil\n\n" +
                                  $"🚀 Fase 1: Arquitetura Refatorada\n" +
                                  $"📊 PredictionEngine Implementado\n" +
                                  $"🎯 Sistema de Validação Automatizado\n\n" +
                                  $"© 2024 - Todos os direitos reservados";

                MessageBox.Show(aboutMessage, "Sobre o GraphFacil",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                LogError("Erro ao mostrar informações sobre", ex);
            }
        }

        /// <summary>
        /// Manipula comando para mostrar ajuda
        /// </summary>
        private void ShowHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var helpMessage = $"📖 AJUDA - {APP_NAME}\n\n" +
                                 $"🎯 AÇÕES PRINCIPAIS:\n" +
                                 $"• PRIMEIRO-SEXTO: Funcionalidades originais\n" +
                                 $"• PALPITE NOVO: Usa novo PredictionEngine\n" +
                                 $"• DIAGNÓSTICO: Analisa problemas do sistema\n" +
                                 $"• VALIDAR F1: Testa implementação da Fase 1\n" +
                                 $"• COMPARAR: Compara performance antigo vs novo\n\n" +
                                 $"⌨️ TECLAS DE ATALHO:\n" +
                                 $"• F1: Executar Diagnósticos\n" +
                                 $"• F2: Validar Fase 1\n" +
                                 $"• F5: Gerar Palpite Novo\n" +
                                 $"• ESC: Fechar Aplicação\n\n" +
                                 $"📊 INDICADORES:\n" +
                                 $"• Verde (✅): Funcionamento normal\n" +
                                 $"• Amarelo (⚠️): Atenção necessária\n" +
                                 $"• Vermelho (❌): Erro ou problema\n" +
                                 $"• Azul (⏳): Processando/Carregando";

                MessageBox.Show(helpMessage, "Ajuda - GraphFacil",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                LogError("Erro ao mostrar ajuda", ex);
            }
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Processa argumentos da linha de comando
        /// </summary>
        private void ProcessCommandLineArguments(string[] args)
        {
            try
            {
                foreach (var arg in args)
                {
                    switch (arg.ToLower())
                    {
                        case "--debug":
                        case "-d":
                            LogInfo("Modo debug ativado via linha de comando");
                            break;

                        case "--help":
                        case "-h":
                            ShowCommandLineHelp();
                            Shutdown();
                            return;

                        case "--version":
                        case "-v":
                            MessageBox.Show($"{APP_NAME}\nVersão: {VERSION}",
                                "Versão", MessageBoxButton.OK, MessageBoxImage.Information);
                            Shutdown();
                            return;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Erro ao processar argumentos da linha de comando", ex);
            }
        }

        /// <summary>
        /// Mostra ajuda da linha de comando
        /// </summary>
        private void ShowCommandLineHelp()
        {
            var helpText = $"{APP_NAME}\n\n" +
                          $"Argumentos disponíveis:\n" +
                          $"  --debug, -d    Ativa modo debug\n" +
                          $"  --help, -h     Mostra esta ajuda\n" +
                          $"  --version, -v  Mostra versão da aplicação";

            MessageBox.Show(helpText, "Ajuda - Linha de Comando",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// Cleanup de recursos da aplicação
        /// </summary>
        private void CleanupResources()
        {
            try
            {
                // Aqui podem ser adicionadas operações de limpeza específicas
                // Por exemplo: salvar configurações, fechar conexões, etc.

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                LogError("Erro durante cleanup de recursos", ex);
            }
        }

        /// <summary>
        /// Remove arquivos de log antigos para economizar espaço
        /// </summary>
        private void CleanupOldLogFiles()
        {
            try
            {
                var logFiles = Directory.GetFiles(LOG_FOLDER, "*.log");
                var cutoffDate = DateTime.Now.AddDays(-7); // Mantém logs de 7 dias

                foreach (var file in logFiles)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < cutoffDate)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // Ignora erros de limpeza de logs
            }
        }

        #endregion

        #region Logging Methods

        /// <summary>
        /// Log de informação
        /// </summary>
        private void LogInfo(string message)
        {
            WriteLog("INFO", message, null);
        }

        /// <summary>
        /// Log de erro
        /// </summary>
        private void LogError(string message, Exception ex)
        {
            WriteLog("ERROR", message, ex);
        }

        /// <summary>
        /// Escreve entrada no log
        /// </summary>
        private void WriteLog(string level, string message, Exception ex)
        {
            try
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var logEntry = $"[{timestamp}] [{level}] {message}";

                if (ex != null)
                {
                    logEntry += $"\nException: {ex}";
                }

                var logFile = Path.Combine(LOG_FOLDER, $"app_{DateTime.Now:yyyyMMdd}.log");
                File.AppendAllText(logFile, logEntry + Environment.NewLine);

                // Também escreve no console para debug
                Console.WriteLine(logEntry);
            }
            catch
            {
                // Se não conseguir escrever no log, apenas ignora
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Método público para outras classes logarem informações
        /// </summary>
        public static void LogApplicationInfo(string message)
        {
            try
            {
                (Current as App)?.LogInfo(message);
            }
            catch
            {
                // Ignora erros de log
            }
        }

        /// <summary>
        /// Método público para outras classes logarem erros
        /// </summary>
        public static void LogApplicationError(string message, Exception ex = null)
        {
            try
            {
                (Current as App)?.LogError(message, ex);
            }
            catch
            {
                // Ignora erros de log
            }
        }

        #endregion
    }
}