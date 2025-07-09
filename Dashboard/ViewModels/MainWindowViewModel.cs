// Dashboard/ViewModels/MainWindowViewModel.cs - REFATORADO: Orquestrador principal LIMPO
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Services;
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

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
                DezenasOscilantes = new ObservableCollection<DezenaOscilante>();
            }
        }
        #endregion

        #region Orchestration Commands - DELEGAM para ViewModels especializados
        /// <summary>
        /// Comando principal para gerar palpite - DELEGA para PredictionModels
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanExecuteMainOperations))]
        private async Task GerarPalpitePrincipal()
        {
            try
            {
                // Atualizar concurso alvo nos ViewModels especializados
                PredictionModels.TargetConcurso = TextoConcurso;

                // Delegar para o ViewModel especializado
                await PredictionModels.GeneratePrediction();

                UpdateApplicationStatus();
            }
            catch (Exception ex)
            {
                SetStatus($"Erro na geração de palpite: {ex.Message}", true);
                UINotificationService.Instance.ShowError(ex.Message, "Erro");
            }
        }

        /// <summary>
        /// Comando principal para validação - DELEGA para Validation
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanExecuteMainOperations))]
        private async Task ExecutarValidacaoPrincipal()
        {
            try
            {
                await Validation.RunFullValidation();
                UpdateApplicationStatus();
            }
            catch (Exception ex)
            {
                SetStatus($"Erro na validação: {ex.Message}", true);
                UINotificationService.Instance.ShowError(ex.Message, "Erro");
            }
        }

        /// <summary>
        /// Comando principal para comparação - DELEGA para Comparison
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanExecuteMainOperations))]
        private async Task ExecutarComparacaoPrincipal()
        {
            try
            {
                // Configurar modelos para comparação automaticamente
                Comparison.SelectedModelsForComparison.Clear();
                foreach (var modelType in Comparison.AvailableModelsForComparison)
                {
                    Comparison.SelectedModelsForComparison.Add(modelType);
                    if (Comparison.SelectedModelsForComparison.Count >= 3) break; // Limitar para não sobrecarregar
                }

                Comparison.TargetConcursoForComparison = TextoConcurso;
                await Comparison.CompareSelectedModels();

                UpdateApplicationStatus();
            }
            catch (Exception ex)
            {
                SetStatus($"Erro na comparação: {ex.Message}", true);
                UINotificationService.Instance.ShowError(ex.Message, "Erro");
            }
        }
        #endregion

        #region Legacy Commands - MANTIDOS para compatibilidade
        [RelayCommand]
        private void Primeiro()
        {
            ExecuteLegacyStudy(1, "Estudo1");
        }

        [RelayCommand]
        private void Segundo()
        {
            ExecuteLegacyStudy(2, "Estudo2");
        }

        [RelayCommand]
        private void Terceiro()
        {
            ExecuteLegacyStudy(3, "Estudo3");
        }

        [RelayCommand]
        private void Quarto()
        {
            ExecuteLegacyStudy(4, "Estudo4");
        }

        [RelayCommand]
        private void Quinto()
        {
            ExecuteLegacyStudy(5, "Estudo5");
        }

        [RelayCommand]
        private void Sexto()
        {
            ExecuteLegacyStudy(6, "Estudo6");
        }

        [RelayCommand]
        private void Setimo()
        {
            ExecuteLegacyStudy(7, "Estudo7");
        }

        [RelayCommand]
        private void Oitavo()
        {
            ExecuteLegacyStudy(8, "Estudo8");
        }

        [RelayCommand]
        private void Nono()
        {
            ExecuteLegacyStudy(9, "Estudo9");
        }

        [RelayCommand]
        private void Dez()
        {
            ExecuteLegacyStudy(10, "Estudo10");
        }
        #endregion

        #region Oscillator Commands - MANTIDOS para compatibilidade
        [RelayCommand]
        private void IniciarSincronizacao()
        {
            try
            {
                MostrarOsciladores = true;
                SetStatus("Sincronização de osciladores iniciada");

                // Lógica de sincronização se necessário
                // _oscillatorEngine.IniciarSincronizacao();

            }
            catch (Exception ex)
            {
                SetStatus($"Erro na sincronização: {ex.Message}", true);
                LogError(ex);
            }
        }

        [RelayCommand]
        private void PararSincronizacao()
        {
            try
            {
                MostrarOsciladores = false;
                SetStatus("Sincronização parada");
            }
            catch (Exception ex)
            {
                SetStatus($"Erro ao parar sincronização: {ex.Message}", true);
                LogError(ex);
            }
        }
        #endregion

        #region Can Execute Methods
        private bool CanExecuteMainOperations()
        {
            return CanExecute() && IsSystemReady;
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Executa estudos legacy mantendo compatibilidade
        /// </summary>
        private void ExecuteLegacyStudy(int studyNumber, string studyName)
        {
            try
            {
                SetStatus($"Executando {studyName}...");

                int alvo = Convert.ToInt32(TextoConcurso);

                // Usar reflection para chamar método correto do Estudos
                var estudosType = typeof(Estudos);
                var metodo = estudosType.GetMethod($"Estudo{studyNumber}");

                if (metodo != null)
                {
                    var resultado = (Lances)metodo.Invoke(null, new object[] { alvo });
                    var nomeArquivo = Infra.NomeSaida($"Lista{studyName}", alvo);

                    Infra.SalvaSaidaW(resultado, nomeArquivo);

                    SetStatus($"✅ {studyName} executado: {nomeArquivo}");
                    UINotificationService.Instance.ShowSuccess($"{studyName} salvo em: {nomeArquivo}");
                }
                else
                {
                    SetStatus($"❌ Método {studyName} não encontrado", true);
                }
            }
            catch (Exception ex)
            {
                SetStatus($"❌ Erro no {studyName}: {ex.Message}", true);
                UINotificationService.Instance.ShowError(ex.Message, $"Erro - {studyName}");
                LogError(ex);
            }
        }

        /// <summary>
        /// Atualiza status da aplicação baseado nos ViewModels especializados
        /// </summary>
        private void UpdateApplicationStatus()
        {
            try
            {
                var errors = 0;
                var warnings = 0;

                // Verificar status dos ViewModels especializados
                if (PredictionModels.HasError) errors++;
                if (Validation.HasError) errors++;
                if (Comparison.HasError) errors++;
                if (Configuration.HasError) errors++;

                // Determinar status geral
                if (errors > 0)
                {
                    ApplicationStatus = $"⚠️ {errors} componente(s) com erro";
                }
                else if (!PredictionModels.IsPredictionEngineReady)
                {
                    ApplicationStatus = "🔄 Inicializando sistema de predição...";
                }
                else
                {
                    ApplicationStatus = "✅ Todos os sistemas funcionando";
                }
            }
            catch (Exception ex)
            {
                ApplicationStatus = "❌ Erro na atualização de status";
                LogError(ex);
            }
        }

        /// <summary>
        /// Termina programa (compatibilidade)
        /// </summary>
        private void TerminarPrograma()
        {
            try
            {
                // Cleanup dos ViewModels especializados
                PredictionModels?.Cleanup();
                Validation?.Cleanup();
                Comparison?.Cleanup();
                Configuration?.Cleanup();

                // Fechar aplicação
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                LogError(ex);
                Application.Current.Shutdown();
            }
        }
        #endregion

        #region Property Change Handlers
        /// <summary>
        /// Quando concurso muda, atualiza ViewModels especializados
        /// </summary>
        partial void OnTextoConcursoChanged(string value)
        {
            try
            {
                // Validar se é número válido
                if (int.TryParse(value, out var concurso) && concurso > 0)
                {
                    // Propagar para ViewModels especializados
                    PredictionModels.TargetConcurso = value;
                    Comparison.TargetConcursoForComparison = value;

                    SetStatus($"Concurso alvo: {concurso}");
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }
        #endregion

        #region Cleanup
        public override void Cleanup()
        {
            try
            {
                base.Cleanup();

                // Cleanup dos ViewModels especializados
                PredictionModels?.Cleanup();
                Validation?.Cleanup();
                Comparison?.Cleanup();
                Configuration?.Cleanup();

                // Cleanup da factory
                _viewModelFactory?.ClearCache();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }
        #endregion
    }
}

