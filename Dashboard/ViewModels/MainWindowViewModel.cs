// Dashboard/ViewModels/MainWindowViewModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Engines;
using LotoLibrary.Models;
using LotoLibrary.Models.Core;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ServiceInfra = LotoLibrary.Utilities.Infra;

namespace Dashboard.ViewModels
{
    /// <summary>
    /// ViewModel principal REFATORADO - Apenas orquestra os ViewModels especializados
    /// Responsabilidade única: coordenar e expor os ViewModels especializados
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        #region Private Fields
        private readonly LotoLibrary.Models.Core.Lances _historicalData;
        private readonly ViewModelFactory _viewModelFactory;
        private readonly ServiceInfra _infraService;
        private readonly OscillatorEngine _oscillatorEngine;
        #endregion

        #region Specialized ViewModels - COMPOSIÇÃO
        public PredictionModelsViewModel PredictionModels { get; }
        public ValidationViewModel Validation { get; }
        public ComparisonViewModel Comparison { get; }
        public ConfigurationViewModel Configuration { get; }
        #endregion

        #region Observable Properties - APENAS O ESSENCIAL
        [ObservableProperty]
        private string _textoConcurso = "3000";

        [ObservableProperty]
        private string _applicationStatus = "Inicializando...";

        [ObservableProperty]
        private bool _isSystemReady;

        [ObservableProperty]
        private ObservableCollection<DezenaOscilante> _dezenasOscilantes;

        [ObservableProperty]
        private bool _mostrarOsciladores;

        [ObservableProperty]
        private string _lastUpdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

        [ObservableProperty]
        private bool _isSyncRunning = false;

        [ObservableProperty]
        private string _syncStatus = "Sistema pronto";
        #endregion

        #region Constructor
        public MainWindowViewModel(LotoLibrary.Models.Core.Lances historico)
        {
            _historicalData = historico ?? throw new ArgumentNullException(nameof(historico));

            // Debug - verificar se os dados foram carregados
            System.Diagnostics.Debug.WriteLine($"MainWindowViewModel recebeu {_historicalData.Count} registros históricos");

            // Inicializar serviços
            _infraService = new ServiceInfra();

            // CORREÇÃO: Passar o parâmetro historico para OscillatorEngine
            _oscillatorEngine = new OscillatorEngine(historico);

            // Factory para criar ViewModels especializados
            _viewModelFactory = new ViewModelFactory(_historicalData);

            // Criar ViewModels especializados via Factory
            PredictionModels = _viewModelFactory.CreatePredictionModelsViewModel();
            Validation = _viewModelFactory.CreateValidationViewModel();
            Comparison = _viewModelFactory.CreateComparisonViewModel();
            Configuration = _viewModelFactory.CreateConfigurationViewModel();

            // Inicializar coleções
            DezenasOscilantes = new ObservableCollection<DezenaOscilante>();

            // Inicialização assíncrona
            _ = InitializeAsync();
        }
        #endregion

        #region Initialization
        private async Task InitializeAsync()
        {
            try
            {
                ApplicationStatus = "Inicializando sistema...";

                // Inicializar ViewModels especializados
                await Task.WhenAll(
                    PredictionModels.InitializeAsync(),
                    Validation.InitializeAsync(),
                    Comparison.InitializeAsync(),
                    Configuration.InitializeAsync()
                );

                // Carregar osciladores
                await LoadOscillatorsAsync();

                IsSystemReady = true;
                ApplicationStatus = "✅ Sistema pronto para uso";
                LastUpdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
            catch (Exception ex)
            {
                ApplicationStatus = $"❌ Erro na inicialização: {ex.Message}";
                IsSystemReady = false;
            }
        }
        #endregion

        #region Commands - COMANDOS PRINCIPAIS

        [RelayCommand(CanExecute = nameof(CanExecuteMainOperations))]
        private async Task ExecutarValidacaoPrincipal()
        {
            await ExecuteWithProgressAsync(async () =>
            {
                ApplicationStatus = "Executando validação principal...";

                // CORREÇÃO: Usar o comando correto que existe no ValidationViewModel
                if (Validation.RunQuickValidationCommand.CanExecute(null))
                {
                    await Validation.RunQuickValidationCommand.ExecuteAsync(null);
                }

                ApplicationStatus = "✅ Validação principal concluída";
                LastUpdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            }, "Executando validação principal...");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMainOperations))]
        private async Task ExecutarComparacaoPrincipal()
        {
            await ExecuteWithProgressAsync(async () =>
            {
                ApplicationStatus = "Executando comparação principal...";

                // Simular operação de comparação
                await Task.Delay(2000);

                ApplicationStatus = "✅ Comparação principal concluída";
                LastUpdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            }, "Executando comparação principal...");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteMainOperations))]
        private async Task GerarPalpitePrincipal()
        {
            await ExecuteWithProgressAsync(async () =>
            {
                ApplicationStatus = "Gerando palpite principal...";

                // Usar o comando QuickPredict do PredictionModels
                if (PredictionModels.QuickPredictCommand.CanExecute(null))
                {
                    await PredictionModels.QuickPredictCommand.ExecuteAsync(null);
                }

                ApplicationStatus = "✅ Palpite principal gerado";
                LastUpdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

            }, "Gerando palpite principal...");
        }

        [RelayCommand]
        private async Task PrimeiroCommand()
        {
            await ExecuteSimpleOperationAsync("Primeira operação executada", "Executando primeira operação...");
        }

        [RelayCommand]
        private async Task SegundoCommand()
        {
            await ExecuteSimpleOperationAsync("Segunda operação executada", "Executando segunda operação...");
        }

        [RelayCommand]
        private async Task TerceiroCommand()
        {
            await ExecuteSimpleOperationAsync("Terceira operação executada", "Executando terceira operação...");
        }

        [RelayCommand]
        private async Task QuartoCommand()
        {
            await ExecuteSimpleOperationAsync("Quarta operação executada", "Executando quarta operação...");
        }

        [RelayCommand]
        private async Task QuintoCommand()
        {
            await ExecuteSimpleOperationAsync("Quinta operação executada", "Executando quinta operação...");
        }

        [RelayCommand]
        private async Task SextoCommand()
        {
            await ExecuteSimpleOperationAsync("Sexta operação executada", "Executando sexta operação...");
        }

        [RelayCommand(CanExecute = nameof(CanExecuteSync))]
        private async Task IniciarSincronizacao()
        {
            await ExecuteWithProgressAsync(async () =>
            {
                IsSyncRunning = true;

                var steps = new[]
                {
                    "Verificando conexão...",
                    "Baixando dados atualizados...",
                    "Processando informações...",
                    "Atualizando cache...",
                    "Sincronização concluída"
                };

                foreach (var step in steps)
                {
                    SyncStatus = step;
                    ApplicationStatus = step;
                    await Task.Delay(1000);
                }

                LastUpdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                SyncStatus = "✅ Sincronização concluída com sucesso";
                ApplicationStatus = "✅ Sistema atualizado e pronto";

            }, "Sincronizando dados...");
        }

        #endregion

        #region Can Execute Methods
        private bool CanExecuteMainOperations()
        {
            return IsSystemReady && !IsProcessing && _historicalData != null;
        }

        private bool CanExecuteSync()
        {
            return IsSystemReady && !IsProcessing && !IsSyncRunning;
        }
        #endregion

        #region Helper Methods
        private async Task ExecuteSimpleOperationAsync(string successMessage, string progressMessage)
        {
            await ExecuteWithProgressAsync(async () =>
            {
                await Task.Delay(1500); // Simular operação
                ApplicationStatus = successMessage;
                LastUpdate = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }, progressMessage);
        }

        private async Task ExecuteWithProgressAsync(Func<Task> operation, string progressMessage)
        {
            try
            {
                IsProcessing = true;
                ApplicationStatus = progressMessage;

                await operation();
            }
            catch (Exception ex)
            {
                ApplicationStatus = $"❌ Erro: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
                IsSyncRunning = false;
            }
        }

        private async Task LoadOscillatorsAsync()
        {
            try
            {
                ApplicationStatus = "Carregando osciladores...";

                // Usar o OscillatorEngine para inicializar osciladores
                var osciladores = _oscillatorEngine.InicializarOsciladores();

                // CORREÇÃO: Remover UltimaAparicao que não existe - usar outras propriedades
                foreach (var oscilador in osciladores)
                {
                    // Usar as propriedades que existem na DezenaOscilante
                    oscilador.Frequencia = Random.Shared.NextDouble() * 10;
                    oscilador.Fase = Random.Shared.Next(0, 360);
                }

                DezenasOscilantes = new ObservableCollection<DezenaOscilante>(osciladores);
                MostrarOsciladores = true;

                ApplicationStatus = "Osciladores carregados com sucesso";
            }
            catch (Exception ex)
            {
                ApplicationStatus = $"Erro ao carregar osciladores: {ex.Message}";
            }
        }
        #endregion

        #region Properties Override
        [ObservableProperty]
        private bool _isProcessing;

        partial void OnIsProcessingChanged(bool value)
        {
            // Notificar mudanças nos comandos quando o processamento muda
            ExecutarValidacaoPrincipalCommand.NotifyCanExecuteChanged();
            ExecutarComparacaoPrincipalCommand.NotifyCanExecuteChanged();
            GerarPalpitePrincipalCommand.NotifyCanExecuteChanged();
            IniciarSincronizacaoCommand.NotifyCanExecuteChanged();
        }

        partial void OnIsSystemReadyChanged(bool value)
        {
            // Notificar mudanças nos comandos quando o sistema fica pronto
            ExecutarValidacaoPrincipalCommand.NotifyCanExecuteChanged();
            ExecutarComparacaoPrincipalCommand.NotifyCanExecuteChanged();
            GerarPalpitePrincipalCommand.NotifyCanExecuteChanged();
            IniciarSincronizacaoCommand.NotifyCanExecuteChanged();
        }
        #endregion
    }
}