// D:\PROJETOS\GraphFacil\Dashboard\ViewModels\MainWindowViewModel.cs - Versão corrigida
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Models;
using LotoLibrary.Engines;
using LotoLibrary.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

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

        // Manter osciladores se ainda necessário (pode ser movido depois)
        private readonly OscillatorEngine _oscillatorEngine;
        #endregion

        #region Specialized ViewModels - COMPOSIÇÃO
        /// <summary>
        /// ViewModel para TODOS os modelos de predição
        /// </summary>
        public PredictionModelsViewModel PredictionModels { get; }

        /// <summary>
        /// ViewModel para TODAS as validações
        /// </summary>
        public ValidationViewModel Validation { get; }

        /// <summary>
        /// ViewModel para TODAS as comparações
        /// </summary>
        public ComparisonViewModel Comparison { get; }

        /// <summary>
        /// ViewModel para TODAS as configurações
        /// </summary>
        public ConfigurationViewModel Configuration { get; }
        #endregion

        #region Observable Properties - APENAS O ESSENCIAL
        [ObservableProperty]
        private string _textoConcurso = "3000";

        [ObservableProperty]
        private string _applicationStatus = "Inicializando...";

        [ObservableProperty]
        private bool _isSystemReady;

        // Propriedades de osciladores (compatibilidade - podem ser removidas depois)
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

            // Criar ViewModels especializados via factory
            PredictionModels = _viewModelFactory.CreatePredictionModelsViewModel();
            Validation = _viewModelFactory.CreateValidationViewModel();
            Comparison = _viewModelFactory.CreateComparisonViewModel();
            Configuration = _viewModelFactory.CreateConfigurationViewModel();

            // Manter osciladores por compatibilidade (opcional)
            _oscillatorEngine = new OscillatorEngine(historico);
            InitializeOscillators();

            // Inicializar sistema
            _ = InitializeSystemAsync();
        }
        #endregion

        #region System Initialization
        private async Task InitializeSystemAsync()
        {
            await ExecuteWithLoadingAsync(async () =>
            {
                SetStatus("Inicializando sistema...");

                // Inicializar ViewModels especializados
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
            // Manter funcionalidade de osciladores por compatibilidade
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
                Infra.SalvaSaidaW(resultado, Infra.NomeSaida("ListaEstudo1", alvo));
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
                Infra.SalvaSaidaW(resultado, Infra.NomeSaida("ListaEstudo2", alvo));
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
                Infra.SalvaSaidaW(resultado, Infra.NomeSaida("ListaEstudo3", alvo));
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
            // Implementar logging apropriado
            Console.WriteLine($"ERRO: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}");
        }
        #endregion
    }
}
