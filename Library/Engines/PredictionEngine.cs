// D:\PROJETOS\GraphFacil\Library\Engines\PredictionEngine.cs - Novo coordenador principal
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using LotoLibrary.PredictionModels.Individual;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.Engines
{
    /// <summary>
    /// Engine principal que coordena todos os modelos de predição
    /// Substitui o MetronomoEngine como coordenador central
    /// </summary>
    public partial class PredictionEngine : ObservableObject
    {
        #region Fields
        private readonly ConcurrentDictionary<string, IPredictionModel> _models = new();
        private readonly IModelFactory _modelFactory;
        private readonly IPerformanceAnalyzer _performanceAnalyzer;
        private Lances _historicalData;
        private IEnsembleModel _ensembleModel;
        private string _activeStrategy = "Single";
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private bool _isInitialized;

        [ObservableProperty]
        private string _statusEngine = "Aguardando inicialização...";

        [ObservableProperty]
        private int _totalModels;

        [ObservableProperty]
        private double _overallConfidence;

        [ObservableProperty]
        private List<int> _lastPrediction = new();

        [ObservableProperty]
        private string _performanceSummary = "";

        [ObservableProperty]
        private bool _isProcessing;
        #endregion

        #region Properties
        public IReadOnlyDictionary<string, IPredictionModel> Models => _models;
        public string ActiveStrategy => _activeStrategy;
        public bool HasEnsemble => _ensembleModel != null;
        #endregion

        #region Events
        public event EventHandler<PredictionResult> OnPredictionGenerated;
        public event EventHandler<PerformanceReport> OnPerformanceUpdated;
        public event EventHandler<string> OnStatusChanged;
        #endregion

        #region Constructor
        public PredictionEngine(IModelFactory modelFactory = null, IPerformanceAnalyzer performanceAnalyzer = null)
        {
            _modelFactory = modelFactory ?? new DefaultModelFactory();
            _performanceAnalyzer = performanceAnalyzer ?? new DefaultPerformanceAnalyzer();
            
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(StatusEngine))
                {
                    OnStatusChanged?.Invoke(this, StatusEngine);
                }
            };
        }
        #endregion

        #region Initialization
        [RelayCommand]
        public async Task<bool> InitializeAsync(Lances historicalData)
        {
            try
            {
                StatusEngine = "Inicializando PredictionEngine...";
                IsProcessing = true;

                if (historicalData == null || !historicalData.Any())
                {
                    StatusEngine = "❌ Erro: Dados históricos inválidos";
                    return false;
                }

                _historicalData = historicalData;

                // Inicializar modelo padrão (Metrônomo)
                await RegisterDefaultModels();

                IsInitialized = true;
                TotalModels = _models.Count;
                StatusEngine = $"✅ PredictionEngine inicializado com {TotalModels} modelo(s)";

                return true;
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na inicialização: {ex.Message}";
                return false;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task RegisterDefaultModels()
        {
            // Registrar modelo Metrônomo (compatibilidade com sistema atual)
            var metronomoModel = new MetronomoModel();
            await RegisterModelAsync("Metronomo", metronomoModel);
        }
        #endregion

        #region Model Management
        public async Task<bool> RegisterModelAsync(string name, IPredictionModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name) || model == null)
                    return false;

                StatusEngine = $"Registrando modelo: {name}...";

                // Inicializar modelo se necessário
                if (!model.IsInitialized && _historicalData != null)
                {
                    var initResult = await model.InitializeAsync(_historicalData);
                    if (!initResult)
                    {
                        StatusEngine = $"❌ Falha ao inicializar modelo: {name}";
                        return false;
                    }
                }

                // Treinar modelo
                if (_historicalData != null)
                {
                    var trainResult = await model.TrainAsync(_historicalData);
                    if (!trainResult)
                    {
                        StatusEngine = $"⚠️ Falha no treinamento do modelo: {name}";
                        // Continua mesmo com falha no treinamento
                    }
                }

                // Conectar eventos
                model.OnStatusChanged += (s, status) => 
                {
                    StatusEngine = $"[{name}] {status}";
                };

                model.OnConfidenceChanged += (s, confidence) =>
                {
                    UpdateOverallConfidence();
                };

                _models.TryAdd(name, model);
                TotalModels = _models.Count;

                StatusEngine = $"✅ Modelo {name} registrado com sucesso";
                return true;
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro ao registrar modelo {name}: {ex.Message}";
                return false;
            }
        }

        public bool UnregisterModel(string name)
        {
            if (_models.TryRemove(name, out var model))
            {
                model?.Reset();
                TotalModels = _models.Count;
                StatusEngine = $"Modelo {name} removido";
                return true;
            }
            return false;
        }

        public IPredictionModel GetModel(string name)
        {
            return _models.TryGetValue(name, out var model) ? model : null;
        }
        #endregion

        #region Prediction
        [RelayCommand]
        public async Task<PredictionResult> GeneratePredictionAsync(int targetConcurso)
        {
            try
            {
                if (!IsInitialized)
                {
                    throw new InvalidOperationException("PredictionEngine não inicializado");
                }

                StatusEngine = $"Gerando predição para concurso {targetConcurso}...";
                IsProcessing = true;

                PredictionResult result = null;

                switch (_activeStrategy)
                {
                    case "Single":
                        result = await GenerateSingleModelPrediction(targetConcurso);
                        break;
                    case "Ensemble":
                        result = await GenerateEnsemblePrediction(targetConcurso);
                        break;
                    default:
                        result = await GenerateSingleModelPrediction(targetConcurso);
                        break;
                }

                if (result != null)
                {
                    LastPrediction = result.PredictedNumbers;
                    OverallConfidence = result.OverallConfidence;
                    OnPredictionGenerated?.Invoke(this, result);
                    StatusEngine = $"✅ Predição gerada: {result.PredictedNumbers.Count} dezenas, Confiança: {result.OverallConfidence:P2}";
                }

                return result;
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na predição: {ex.Message}";
                throw;
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task<PredictionResult> GenerateSingleModelPrediction(int targetConcurso)
        {
            // Usar o primeiro modelo disponível (geralmente Metrônomo)
            var model = _models.Values.FirstOrDefault(m => m.IsInitialized);
            if (model == null)
            {
                throw new InvalidOperationException("Nenhum modelo inicializado disponível");
            }

            return await model.PredictAsync(targetConcurso);
        }

        private async Task<PredictionResult> GenerateEnsemblePrediction(int targetConcurso)
        {
            if (_ensembleModel == null)
            {
                throw new InvalidOperationException("Ensemble não configurado");
            }

            return await _ensembleModel.PredictAsync(targetConcurso);
        }
        #endregion

        #region Ensemble Management
        [RelayCommand]
        public async Task CreateEnsembleAsync(Dictionary<string, double> modelWeights = null)
        {
            try
            {
                StatusEngine = "Criando ensemble...";

                var availableModels = _models.Values.Where(m => m.IsInitialized).ToList();
                if (availableModels.Count < 2)
                {
                    StatusEngine = "❌ Mínimo 2 modelos necessários para ensemble";
                    return;
                }

                // Criar ensemble usando factory
                var modelTypes = availableModels.Select(m => ModelType.Metronomo).ToList(); // Temporário
                _ensembleModel = _modelFactory.CreateEnsemble(modelTypes, modelWeights);

                if (_ensembleModel != null)
                {
                    await _ensembleModel.InitializeAsync(_historicalData);
                    await _ensembleModel.TrainAsync(_historicalData);
                    
                    StatusEngine = $"✅ Ensemble criado com {availableModels.Count} modelos";
                }
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro ao criar ensemble: {ex.Message}";
            }
        }

        public void SetActiveStrategy(string strategy)
        {
            if (strategy == "Ensemble" && _ensembleModel == null)
            {
                StatusEngine = "⚠️ Ensemble não disponível, mantendo estratégia atual";
                return;
            }

            _activeStrategy = strategy;
            StatusEngine = $"Estratégia ativa: {strategy}";
        }

        public void UpdateEnsembleWeights(Dictionary<string, double> weights)
        {
            if (_ensembleModel != null)
            {
                _ensembleModel.UpdateWeights(weights);
                StatusEngine = "Pesos do ensemble atualizados";
            }
        }
        #endregion

        #region Performance Analysis
        [RelayCommand]
        public async Task AnalyzePerformanceAsync()
        {
            try
            {
                StatusEngine = "Analisando performance dos modelos...";
                IsProcessing = true;

                var reports = new List<PerformanceReport>();

                foreach (var model in _models.Values.Where(m => m.IsInitialized))
                {
                    try
                    {
                        var report = await _performanceAnalyzer.AnalyzeAsync(model, _historicalData);
                        reports.Add(report);
                        OnPerformanceUpdated?.Invoke(this, report);
                    }
                    catch (Exception ex)
                    {
                        StatusEngine = $"⚠️ Erro ao analisar {model.ModelName}: {ex.Message}";
                    }
                }

                // Gerar resumo
                if (reports.Any())
                {
                    var avgAccuracy = reports.Average(r => r.ValidationResults.Accuracy);
                    var bestModel = reports.OrderByDescending(r => r.ValidationResults.Accuracy).First();
                    
                    PerformanceSummary = $"Accuracy média: {avgAccuracy:P2} | Melhor: {bestModel.ModelName} ({bestModel.ValidationResults.Accuracy:P2})";
                }

                StatusEngine = $"✅ Análise concluída para {reports.Count} modelo(s)";
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na análise: {ex.Message}";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        public async Task CompareModelsAsync()
        {
            try
            {
                if (_models.Count < 2)
                {
                    StatusEngine = "⚠️ Mínimo 2 modelos necessários para comparação";
                    return;
                }

                StatusEngine = "Comparando modelos...";
                
                var modelsList = _models.Values.Where(m => m.IsInitialized).ToList();
                var comparison = await _performanceAnalyzer.CompareModelsAsync(modelsList, _historicalData);

                StatusEngine = $"✅ Comparação concluída | Melhor: {comparison.BestModelName}";
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro na comparação: {ex.Message}";
            }
        }
        #endregion

        #region Diagnostics
        [RelayCommand]
        public async Task RunDiagnosticsAsync()
        {
            try
            {
                StatusEngine = "Executando diagnósticos...";

                // Testar geração de múltiplos palpites
                var testPredictions = new List<List<int>>();
                for (int i = 0; i < 20; i++)
                {
                    var prediction = await GeneratePredictionAsync(3500 + i);
                    if (prediction?.PredictedNumbers?.Any() == true)
                    {
                        testPredictions.Add(prediction.PredictedNumbers);
                    }
                }

                // Analisar distribuição usando DiagnosticService
                if (testPredictions.Any())
                {
                    var report = DiagnosticService.AnalisarDistribuicaoDezenas(testPredictions);
                    var summary = DiagnosticService.GerarRelatorioTexto(report);
                    
                    StatusEngine = report.TemProblemaDistribuicao 
                        ? $"⚠️ Problemas detectados - Gravidade: {report.GravidadeProblema}"
                        : "✅ Distribuição normal detectada";

                    // Salvar relatório detalhado
                    System.IO.File.WriteAllText("DiagnosticReport.txt", summary);
                }
                else
                {
                    StatusEngine = "❌ Nenhuma predição válida gerada para diagnóstico";
                }
            }
            catch (Exception ex)
            {
                StatusEngine = $"❌ Erro no diagnóstico: {ex.Message}";
            }
        }
        #endregion

        #region Helpers
        private void UpdateOverallConfidence()
        {
            var activeModels = _models.Values.Where(m => m.IsInitialized).ToList();
            if (activeModels.Any())
            {
                OverallConfidence = activeModels.Average(m => m.Confidence);
            }
        }

        public string GetSystemStatus()
        {
            var status = $"=== STATUS DO PREDICTION ENGINE ===\n\n";
            status += $"Inicializado: {IsInitialized}\n";
            status += $"Total de Modelos: {TotalModels}\n";
            status += $"Estratégia Ativa: {ActiveStrategy}\n";
            status += $"Confiança Geral: {OverallConfidence:P2}\n";
            status += $"Tem Ensemble: {HasEnsemble}\n";
            status += $"Última Predição: [{string.Join(", ", LastPrediction.Select(d => d.ToString("D2")))}]\n\n";

            status += "MODELOS REGISTRADOS:\n";
            foreach (var model in _models)
            {
                status += $"  • {model.Key}: {(model.Value.IsInitialized ? "✅" : "❌")} ";
                status += $"Confiança: {model.Value.Confidence:P2}\n";
            }

            return status;
        }
        #endregion
    }

    // Implementações temporárias dos serviços (até serem criados)
    internal class DefaultModelFactory : IModelFactory
    {
        public IPredictionModel CreateModel(ModelType type, Dictionary<string, object> parameters = null)
        {
            return type switch
            {
                ModelType.Metronomo => new MetronomoModel(),
                _ => throw new NotSupportedException($"Tipo de modelo {type} não suportado ainda")
            };
        }

        public IEnsembleModel CreateEnsemble(List<ModelType> modelTypes, Dictionary<string, double> weights = null)
        {
            // Implementação temporária - será criada na Fase 3
            throw new NotImplementedException("EnsembleModel será implementado na Fase 3");
        }

        public List<ModelType> GetAvailableModelTypes()
        {
            return new List<ModelType> { ModelType.Metronomo };
        }

        public ModelInfo GetModelInfo(ModelType type)
        {
            return new ModelInfo
            {
                Type = type,
                Name = type.ToString(),
                Description = "Modelo temporário",
                DefaultParameters = new Dictionary<string, object>()
            };
        }
    }

    internal class DefaultPerformanceAnalyzer : IPerformanceAnalyzer
    {
        public async Task<PerformanceReport> AnalyzeAsync(IPredictionModel model, Lances testData)
        {
            // Implementação básica - será expandida
            var validation = await model.ValidateAsync(testData);
            
            return new PerformanceReport
            {
                ModelName = model.ModelName,
                ReportTime = DateTime.Now,
                ValidationResults = validation,
                Grade = validation.Accuracy > 0.7 ? PerformanceGrade.A : 
                       validation.Accuracy > 0.6 ? PerformanceGrade.B : PerformanceGrade.C
            };
        }

        public async Task<ComparisonReport> CompareModelsAsync(List<IPredictionModel> models, Lances testData)
        {
            var comparisons = new List<ModelComparison>();
            
            foreach (var model in models)
            {
                var report = await AnalyzeAsync(model, testData);
                comparisons.Add(new ModelComparison
                {
                    ModelName = model.ModelName,
                    Accuracy = report.ValidationResults.Accuracy,
                    Confidence = model.Confidence,
                    Grade = report.Grade
                });
            }

            return new ComparisonReport
            {
                ComparisonTime = DateTime.Now,
                ModelComparisons = comparisons,
                BestModelName = comparisons.OrderByDescending(c => c.Accuracy).First().ModelName
            };
        }

        public async Task<List<PerformanceMetric>> GetDetailedMetricsAsync(IPredictionModel model, Lances testData)
        {
            // Implementação básica
            return new List<PerformanceMetric>
            {
                new PerformanceMetric { Name = "Accuracy", Value = model.Confidence, Type = MetricType.Accuracy }
            };
        }
    }
}