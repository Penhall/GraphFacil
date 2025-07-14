using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Engines;
using LotoLibrary.Models;
using LotoLibrary.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ServiceInfra = LotoLibrary.Utilities.Infra;


namespace Dashboard.ViewModels
{
    /// <summary>
    /// ViewModel principal REFATORADO - Apenas orquestra os ViewModels especializados
    /// Responsabilidade única: coordenar e expor os ViewModels especializados
    /// MUITO MAIS SIMPLES que a versão anterior!
    /// </summary>
    public partial class MainWindowViewModel : ViewModelBase
    {
        #region Private Fields
        private readonly Lances _historicalData;
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
        #endregion

        #region Constructor
        public MainWindowViewModel(Lances historico)
        {
            _historicalData = historico ?? throw new ArgumentNullException(nameof(historico));
            _viewModelFactory = new ViewModelFactory(historico);
            _infraService = new ServiceInfra();

            // Criar ViewModels especializados via factory
            PredictionModels = _viewModelFactory.CreatePredictionModelsViewModel();
            Validation = _viewModelFactory.CreateValidationViewModel();
            Comparison = _viewModelFactory.CreateComparisonViewModel();
            Configuration = _viewModelFactory.CreateConfigurationViewModel();

            _oscillatorEngine = new OscillatorEngine(historico);
            InitializeOscillators();

            _ = InitializeSystemAsync();
        }
        #endregion

        #region System Initialization
        private async Task InitializeSystemAsync()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                SetStatus("Inicializando sistema...");

                var initTasks = new[]
                {
                    PredictionModels.InitializeAsync(),
                    Validation.InitializeAsync(),
                    Comparison.InitializeAsync(),
                    Configuration.InitializeAsync()
                };

                await Task.WhenAll(initTasks);

                IsSystemReady = true;
                ApplicationStatus = "✅ Sistema pronto";
                SetStatus("✅ Todos os sistemas inicializados");

            }, "Inicializando sistema completo...");
        }

        private void InitializeOscillators()
        {
            try
            {
                DezenasOscilantes = new ObservableCollection<DezenaOscilante>(_oscillatorEngine.InicializarOsciladores());
                MostrarOsciladores = false;
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetStatus($"Erro ao inicializar osciladores: {ex.Message}");
            }
        }
        #endregion

        #region Legacy Commands - MANTIDOS PARA COMPATIBILIDADE
        [RelayCommand]
        private async Task Primeiro()
        {
            try
            {
                int alvo = Convert.ToInt32(TextoConcurso);
                var resultado = Estudos.Estudo1(alvo);
                ServiceInfra.SalvaSaidaW(resultado, ServiceInfra.NomeSaida("ListaEstudo1", alvo));
                SetStatus($"Estudo 1 concluído para concurso {alvo}");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetStatus($"Erro no Estudo 1: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Segundo()
        {
            try
            {
                int alvo = Convert.ToInt32(TextoConcurso);
                var resultado = Estudos.Estudo2(alvo);
                ServiceInfra.SalvaSaidaW(resultado, ServiceInfra.NomeSaida("ListaEstudo2", alvo));
                SetStatus($"Estudo 2 concluído para concurso {alvo}");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetStatus($"Erro no Estudo 2: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task Terceiro()
        {
            try
            {
                int alvo = Convert.ToInt32(TextoConcurso);
                var resultado = Estudos.Estudo3(alvo);
                ServiceInfra.SalvaSaidaW(resultado, ServiceInfra.NomeSaida("ListaEstudo3", alvo));
                SetStatus($"Estudo 3 concluído para concurso {alvo}");
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetStatus($"Erro no Estudo 3: {ex.Message}");
            }
        }

        [RelayCommand]
        private void ToggleOsciladores()
        {
            MostrarOsciladores = !MostrarOsciladores;
            SetStatus($"Osciladores {(MostrarOsciladores ? "exibidos" : "ocultados")}");
        }
        #endregion

        #region Overrides
        public override async Task Cleanup()
        {
            if (PredictionModels != null) await PredictionModels.Cleanup();
            if (Validation != null) await Validation.Cleanup();
            if (Comparison != null) await Comparison.Cleanup();
            if (Configuration != null) await Configuration.Cleanup();

            DezenasOscilantes?.Clear();

            await base.Cleanup();
        }
        #endregion

        #region Private Helper Methods
        private void LogError(Exception ex)
        {
            Console.WriteLine($"ERRO: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}");
        }
        #endregion
    }
}
