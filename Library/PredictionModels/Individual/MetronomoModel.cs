// D:\PROJETOS\GraphFacil\Library\PredictionModels\Individual\MetronomoModel.cs - Adaptação do MetronomoEngine
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.PredictionModels.Individual
{
    /// <summary>
    /// Modelo de predição baseado em metrônomos individuais
    /// Adaptação do MetronomoEngine original para a nova arquitetura
    /// </summary>
    public partial class MetronomoModel : PredictionModelBase, IConfigurableModel, IExplainableModel
    {
        #region Fields
        private Dictionary<int, MetronomoIndividual> _metronomos = new();
        private List<Lance> _dadosTreino;
        private List<Lance> _dadosValidacao;
        private readonly ValidationMetricsService _validationService;
        private Dictionary<string, object> _parameters;
        #endregion

        #region Properties
        public override string ModelName => "Metrônomo Individual";
        public override string ModelType => "Temporal-Statistical";

        public Dictionary<string, object> Parameters
        {
            get => _parameters ??= GetDefaultParameters();
            set => _parameters = value;
        }

        // Propriedades específicas do modelo (mantendo compatibilidade)
        public Dictionary<int, MetronomoIndividual> Metronomos => _metronomos;
        public int ConcursoAlvo { get; private set; }
        public int TamanhoValidacao { get; set; } = 100;
        #endregion

        #region Constructor
        public MetronomoModel()
        {
            _validationService = new ValidationMetricsService();
            _parameters = GetDefaultParameters();
        }

        public MetronomoModel(Lances historico) : this()
        {
            // Compatibilidade com construtor original
            if (historico?.Any() == true)
            {
                var task = InitializeAsync(historico);
                task.Wait(); // Sincronizar para manter compatibilidade
            }
        }
        #endregion

        #region IPredictionModel Implementation
        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                UpdateStatus("Inicializando metrônomos...");

                if (!historicalData.Any())
                {
                    UpdateStatus("Erro: Sem dados históricos");
                    return false;
                }

                // Configurar dados de treino/validação
                (_dadosTreino, _dadosValidacao) = SplitDataPersonalizado(historicalData);

                // Determinar concurso alvo
                ConcursoAlvo = (historicalData.LastOrDefault()?.Id ?? 3000) + 1;

                UpdateStatus($"Dados divididos: {_dadosTreino.Count} treino, {_dadosValidacao.Count} validação");

                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro na inicialização: {ex.Message}");
                return false;
            }
        }

        protected override async Task<bool> DoTrainAsync(Lances trainingData)
        {
            try
            {
                UpdateStatus("Treinando metrônomos individuais...");

                await Task.Run(() =>
                {
                    _metronomos.Clear();

                    // Criar metrônomo para cada dezena (1-25)
                    for (int dezena = 1; dezena <= 25; dezena++)
                    {
                        var historicoAparicoes = ExtrairHistoricoAparicoes(dezena, _dadosTreino);
                        var metronomo = new MetronomoIndividual(dezena, historicoAparicoes);

                        _metronomos[dezena] = metronomo;

                        UpdateStatus($"Analisando dezena {dezena}/25...");
                    }

                    // Atualizar estado atual
                    AtualizarEstadoAtual();
                });

                UpdateStatus($"✅ {_metronomos.Count} metrônomos treinados");
                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro no treinamento: {ex.Message}");
                return false;
            }
        }

        protected override async Task<PredictionResult> DoPredictAsync(int targetConcurso)
        {
            try
            {
                if (!_metronomos.Any())
                {
                    throw new InvalidOperationException("Modelo não treinado");
                }

                UpdateStatus($"Gerando predição para concurso {targetConcurso}...");

                var result = await Task.Run(() => GerarPalpiteOtimizado());

                // Calcular probabilidades individuais
                var probabilidades = new Dictionary<int, double>();
                foreach (var metronomo in _metronomos.Values)
                {
                    probabilidades[metronomo.Numero] = metronomo.ProbabilidadeAtual;
                }

                // Ordenar e selecionar top 15
                var top15 = probabilidades
                    .OrderByDescending(kv => kv.Value)
                    .Take(15)
                    .Select(kv => kv.Key)
                    .OrderBy(x => x)
                    .ToList();

                // Calcular confiança geral
                var confiancaGeral = probabilidades.Values.Take(15).Average();

                var predictionResult = new PredictionResult
                {
                    PredictedNumbers = top15,
                    NumberProbabilities = probabilidades,
                    OverallConfidence = confiancaGeral,
                    TargetConcurso = targetConcurso,
                    Metadata = new Dictionary<string, object>
                    {
                        {"TotalMetronomos", _metronomos.Count},
                        {"MetodoSelecao", "Top15PorProbabilidade"},
                        {"ConcursoBase", _dadosTreino.LastOrDefault()?.Id ?? 0}
                    }
                };

                UpdateConfidence(confiancaGeral);
                return predictionResult;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro na predição: {ex.Message}");
                throw;
            }
        }

        protected override async Task<ValidationResult> DoValidateAsync(Lances validationData)
        {
            try
            {
                UpdateStatus("Validando modelo com dados de teste...");

                var predictions = new List<Lance>();
                var actuals = validationData.ToList();

                // Gerar predições para cada concurso de validação
                foreach (var concurso in actuals.Take(Math.Min(50, actuals.Count))) // Limitar para performance
                {
                    var prediction = await PredictAsync(concurso.Id);
                    predictions.Add(new Lance(concurso.Id, prediction.PredictedNumbers));
                }

                var validationResult = CreateValidationResult(predictions, actuals.Take(predictions.Count).ToList());

                // Métricas detalhadas
                validationResult.DetailedMetrics = new Dictionary<string, double>
                {
                    {"AverageConfidence", predictions.Count > 0 ? _confidence : 0},
                    {"ModelComplexity", _metronomos.Count},
                    {"TrainingSize", _dadosTreino.Count}
                };

                return validationResult;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro na validação: {ex.Message}");
                throw;
            }
        }

        protected override void DoReset()
        {
            _metronomos.Clear();
            _dadosTreino = null;
            _dadosValidacao = null;
            ConcursoAlvo = 0;
        }
        #endregion

        #region IConfigurableModel Implementation
        public void UpdateParameters(Dictionary<string, object> newParameters)
        {
            if (newParameters == null) return;

            foreach (var param in newParameters)
            {
                if (_parameters.ContainsKey(param.Key))
                {
                    _parameters[param.Key] = param.Value;
                }
            }

            // Aplicar parâmetros específicos
            if (_parameters.ContainsKey("TamanhoValidacao"))
            {
                TamanhoValidacao = Convert.ToInt32(_parameters["TamanhoValidacao"]);
            }
        }

        public Dictionary<string, object> GetDefaultParameters()
        {
            return new Dictionary<string, object>
            {
                {"TamanhoValidacao", 100},
                {"ThresholdConfianca", 0.5},
                {"NumeroDezenasSelecao", 15},
                {"PesoTemporalDecay", 0.95},
                {"UsarNormalizacao", true}
            };
        }

        public bool ValidateParameters(Dictionary<string, object> parameters)
        {
            if (parameters == null) return false;

            try
            {
                // Validações específicas
                if (parameters.ContainsKey("TamanhoValidacao"))
                {
                    var tamanho = Convert.ToInt32(parameters["TamanhoValidacao"]);
                    if (tamanho <= 0 || tamanho > 1000) return false;
                }

                if (parameters.ContainsKey("NumeroDezenasSelecao"))
                {
                    var numero = Convert.ToInt32(parameters["NumeroDezenasSelecao"]);
                    if (numero < 5 || numero > 25) return false;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region IExplainableModel Implementation
        public ModelExplanation ExplainPrediction(PredictionResult prediction)
        {
            var explanation = new ModelExplanation
            {
                Reasoning = "Predição baseada em análise de intervalos históricos de cada dezena usando metrônomos individuais.",
                ConfidenceLevel = prediction.OverallConfidence
            };

            // Fatores de decisão
            explanation.Factors.Add(new DecisionFactor
            {
                Name = "Análise Temporal",
                Weight = 0.6,
                Description = "Intervalos entre aparições de cada dezena",
                Value = "Padrões de periodicidade detectados"
            });

            explanation.Factors.Add(new DecisionFactor
            {
                Name = "Probabilidade Individual",
                Weight = 0.4,
                Description = "Probabilidade calculada para cada dezena",
                Value = $"Média: {prediction.NumberProbabilities.Values.Average():P2}"
            });

            // Razões por dezena (top 5)
            var top5 = prediction.NumberProbabilities
                .OrderByDescending(kv => kv.Value)
                .Take(5)
                .ToList();

            foreach (var item in top5)
            {
                if (_metronomos.TryGetValue(item.Key, out var metronomo))
                {
                    explanation.NumberReasons[item.Key] =
                        $"Probabilidade: {item.Value:P2}, Intervalo atual: {metronomo.IntervalAtual}, " +
                        $"Tipo: {metronomo.TipoMetronomo}";
                }
            }

            return explanation;
        }

        public List<FeatureImportance> GetFeatureImportances()
        {
            return new List<FeatureImportance>
            {
                new FeatureImportance { FeatureName = "Intervalos Históricos", Importance = 0.4, Type = Models.Prediction.PredictionModels.Temporal },
                new FeatureImportance { FeatureName = "Tendência Atual", Importance = 0.3, Type = Models.Prediction.PredictionModels.Pattern },
                new FeatureImportance { FeatureName = "Tipo de Metrônomo", Importance = 0.2, Type = Models.Prediction.PredictionModels.Statistical },
                new FeatureImportance { FeatureName = "Posição no Ciclo", Importance = 0.1, Type = Models.Prediction.PredictionModels.Temporal }
            };
        }

        public string GetModelDescription()
        {
            return "Modelo baseado em metrônomos individuais que analisa padrões temporais de cada dezena, " +
                   "identificando tipos de comportamento (crescente, decrescente, estável) e calculando " +
                   "probabilidades baseadas em intervalos históricos entre aparições.";
        }
        #endregion

        #region Private Methods (mantendo lógica original)
        private List<int> ExtrairHistoricoAparicoes(int dezena, List<Lance> dados)
        {
            if (dados == null || !dados.Any())
                return new List<int>();

            return dados
                .Where(lance => lance.Lista.Contains(dezena))
                .Select(lance => lance.Id)
                .OrderBy(num => num)
                .ToList();
        }

        private (List<Lance> treino, List<Lance> validacao) SplitDataPersonalizado(Lances historico)
        {
            var lista = historico.ToList();

            // Dividir dados
            var treino = lista.SkipLast(TamanhoValidacao).ToList();
            var validacao = lista.TakeLast(TamanhoValidacao).ToList();

            return (treino, validacao);
        }

        private void AtualizarEstadoAtual()
        {
            foreach (var metronomo in _metronomos.Values)
            {
                metronomo.AtualizarEstadoAtual(ConcursoAlvo);

            }
        }

        private List<int> GerarPalpiteOtimizado()
        {
            if (!_metronomos.Any())
                return new List<int>();

            try
            {
                // Obter top 15 metrônomos por probabilidade
                var top15Metronomos = _metronomos.Values
                    .OrderByDescending(m => m.ProbabilidadeAtual)
                    .Take(15)
                    .ToList();

                var palpite = top15Metronomos
                    .Select(m => m.Numero)
                    .OrderBy(x => x)
                    .ToList();

                UpdateStatus($"Palpite gerado: [{string.Join(", ", palpite.Select(d => d.ToString("D2")))}]");

                return palpite;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro ao gerar palpite: {ex.Message}");
                return new List<int>();
            }
        }
        #endregion

        #region Compatibility Methods (para manter interface original)
        [Obsolete("Use InitializeAsync instead")]
        public async Task<bool> InicializarMetronomosAsync()
        {
            return await TrainAsync(_historicalData);
        }

        [Obsolete("Use PredictAsync instead")]
        public List<int> GerarPalpiteComDebug()
        {
            var task = PredictAsync(ConcursoAlvo);
            task.Wait();
            return task.Result?.PredictedNumbers ?? new List<int>();
        }

        public MetronomoIndividual ObterMetronomo(int dezena)
        {
            return _metronomos.TryGetValue(dezena, out var metronomo) ? metronomo : null;
        }

        public List<MetronomoIndividual> ObterTop15Metronomos()
        {
            return _metronomos.Values
                .OrderByDescending(m => m.ProbabilidadeAtual)
                .Take(15)
                .ToList();
        }
        #endregion
    }
}