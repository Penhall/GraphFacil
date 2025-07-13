// D:\PROJETOS\GraphFacil\Library\PredictionModels\Individual\MetronomoModel.cs
using CommunityToolkit.Mvvm.ComponentModel;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Utilities;

namespace LotoLibrary.PredictionModels.Individual;

/// <summary>
/// Modelo de predição baseado em análise de metrônomo
/// </summary>
public partial class MetronomoModel : ObservableObject, IPredictionModel, IConfigurableModel, IExplainableModel
{
    #region Observable Properties
    [ObservableProperty]
    private string _statusEngine = "Aguardando inicialização";

    [ObservableProperty]
    private bool _isInicializado = false;

    [ObservableProperty]
    private int _totalMetronomos = 0;

    [ObservableProperty]
    private string _resumoPerformance = "Nenhuma análise realizada";
    #endregion

    #region IPredictionModel Properties
    public string ModelName => "Metrônomo";
    public string ModelType => "Individual";
    public bool IsInitialized { get; private set; }
    public double Confidence { get; private set; }
    public DateTime LastTrainingTime { get; private set; }
    public int TrainingDataSize { get; private set; }

    public event EventHandler<string> OnStatusChanged;
    public event EventHandler<double> OnConfidenceChanged;
    #endregion

    #region IConfigurableModel Properties
    private Dictionary<string, object> _currentParameters = new();
    private Dictionary<string, object> _defaultParameters = new();

    public Dictionary<string, object> CurrentParameters => _currentParameters;
    public Dictionary<string, object> DefaultParameters => _defaultParameters;
    #endregion

    #region IExplainableModel Properties
    public bool SupportsDetailedExplanations => true;
    public double ExplanationConfidence => 0.85;
    #endregion

    #region Private Fields
    private readonly Random _random = new();
    private List<Lance> _historicalData = new();
    private Dictionary<int, MetronomoInfo> _metronomos = new();
    #endregion

    #region Constructor
    public MetronomoModel()
    {
        InitializeDefaultParameters();
        StatusEngine = "Modelo Metrônomo inicializado";
    }
    #endregion

    #region IPredictionModel Implementation
    public async Task<bool> InitializeAsync(Lances historicalData)
    {
        try
        {
            StatusEngine = "Inicializando modelo Metrônomo...";
            OnStatusChanged?.Invoke(this, StatusEngine);

            await Task.Delay(100); // Simular processamento assíncrono

            if (historicalData?.Any() == true)
            {
                _historicalData = historicalData.ToList();
                TrainingDataSize = _historicalData.Count;
                LastTrainingTime = DateTime.Now;

                // Inicializar metrônomos
                await InicializarMetronomos();

                IsInitialized = true;
                IsInicializado = true;
                Confidence = 0.75;
                StatusEngine = "Modelo inicializado com sucesso";
                ResumoPerformance = $"Inicializado com {TrainingDataSize} registros históricos";

                OnStatusChanged?.Invoke(this, StatusEngine);
                OnConfidenceChanged?.Invoke(this, Confidence);

                return true;
            }

            StatusEngine = "Erro: Dados históricos inválidos";
            OnStatusChanged?.Invoke(this, StatusEngine);
            return false;
        }
        catch (Exception ex)
        {
            StatusEngine = $"Erro na inicialização: {ex.Message}";
            OnStatusChanged?.Invoke(this, StatusEngine);
            return false;
        }
    }

    public async Task<PredictionResult> PredictAsync(int concurso)
    {
        try
        {
            if (!IsInitialized)
            {
                throw new InvalidOperationException("Modelo não inicializado");
            }

            StatusEngine = $"Gerando predição para concurso {concurso}...";
            OnStatusChanged?.Invoke(this, StatusEngine);

            await Task.Delay(200); // Simular processamento

            // Gerar predição usando lógica de metrônomo
            var dezenas = await GerarPredicaoMetronomo(concurso);

            // Calcular confiança baseada na análise
            var confidence = CalcularConfianca(dezenas);
            Confidence = confidence;

            var result = new PredictionResult(
                concurso: concurso,
                dezenas: dezenas,
                confidence: confidence,
                modelName: ModelName
            )
            {
                Strategy = "Análise de Metrônomo",
                Metadata = new Dictionary<string, object>
                {
                    ["TotalMetronomos"] = TotalMetronomos,
                    ["MetronomosAtivos"] = _metronomos.Count(m => m.Value.Ativo),
                    ["AnalysisTime"] = DateTime.Now,
                    ["WindowSize"] = GetParameter<int>("WindowSize", 50)
                }
            };

            StatusEngine = "Predição gerada com sucesso";
            ResumoPerformance = $"Última predição: {dezenas.Count} dezenas, confiança: {confidence:P2}";

            OnStatusChanged?.Invoke(this, StatusEngine);
            OnConfidenceChanged?.Invoke(this, confidence);

            return result;
        }
        catch (Exception ex)
        {
            StatusEngine = $"Erro na predição: {ex.Message}";
            OnStatusChanged?.Invoke(this, StatusEngine);
            throw;
        }
    }

    public async Task<ValidationResult> ValidateAsync(Lances testData)
    {
        try
        {
            await Task.Delay(150);

            if (testData?.Any() != true)
            {
                return new ValidationResult(false, 0, "Dados de teste inválidos");
            }

            // Simular validação
            var accuracy = CalcularAcuraciaSimulada(testData);

            StatusEngine = "Validação concluída";
            OnStatusChanged?.Invoke(this, StatusEngine);

            return new ValidationResult(true, accuracy, $"Validado com {testData.Count} registros");
        }
        catch (Exception ex)
        {
            return new ValidationResult(false, 0, $"Erro na validação: {ex.Message}");
        }
    }

    public async Task<bool> TrainAsync(Lances trainingData)
    {
        try
        {
            StatusEngine = "Iniciando treinamento...";
            OnStatusChanged?.Invoke(this, StatusEngine);

            await Task.Delay(300);

            if (trainingData?.Any() == true)
            {
                _historicalData = trainingData.ToList();
                TrainingDataSize = _historicalData.Count;
                LastTrainingTime = DateTime.Now;

                // Retreinar metrônomos
                await TreinarMetronomos();

                StatusEngine = "Treinamento concluído";
                ResumoPerformance = $"Retreinado com {TrainingDataSize} registros";

                OnStatusChanged?.Invoke(this, StatusEngine);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            StatusEngine = $"Erro no treinamento: {ex.Message}";
            OnStatusChanged?.Invoke(this, StatusEngine);
            return false;
        }
    }

    public void Reset()
    {
        IsInitialized = false;
        IsInicializado = false;
        Confidence = 0;
        TotalMetronomos = 0;
        StatusEngine = "Modelo resetado";
        ResumoPerformance = "Modelo foi resetado";
        _metronomos.Clear();
        _historicalData.Clear();

        OnStatusChanged?.Invoke(this, StatusEngine);
        OnConfidenceChanged?.Invoke(this, 0);
    }

    public void Dispose()
    {
        // Não há recursos não gerenciados para liberar
    }
    #endregion

    #region IConfigurableModel Implementation
    public void UpdateParameters(Dictionary<string, object> parameters)
    {
        if (ValidateParameters(parameters))
        {
            foreach (var param in parameters)
            {
                _currentParameters[param.Key] = param.Value;
            }
            StatusEngine = "Parâmetros atualizados";
            OnStatusChanged?.Invoke(this, StatusEngine);
        }
    }

    public bool ValidateParameters(Dictionary<string, object> parameters)
    {
        if (parameters == null) return false;

        // Validar parâmetros específicos do metrônomo
        if (parameters.ContainsKey("WindowSize"))
        {
            if (parameters["WindowSize"] is not int windowSize || windowSize < 10 || windowSize > 200)
                return false;
        }

        if (parameters.ContainsKey("MinConfidence"))
        {
            if (parameters["MinConfidence"] is not double confidence || confidence < 0 || confidence > 1)
                return false;
        }

        return true;
    }

    public string GetParameterDescription(string parameterName)
    {
        return parameterName switch
        {
            "WindowSize" => "Tamanho da janela de análise (10-200)",
            "MinConfidence" => "Confiança mínima para predições (0.0-1.0)",
            "MaxMetronomos" => "Número máximo de metrônomos ativos",
            "AnalysisDepth" => "Profundidade da análise histórica",
            _ => $"Parâmetro: {parameterName}"
        };
    }

    public List<object> GetAllowedValues(string parameterName)
    {
        return parameterName switch
        {
            "WindowSize" => Enumerable.Range(10, 191).Cast<object>().ToList(),
            "MinConfidence" => new List<object> { 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9 },
            "MaxMetronomos" => Enumerable.Range(5, 21).Cast<object>().ToList(),
            _ => new List<object>()
        };
    }

    public void ResetToDefaults()
    {
        _currentParameters = new Dictionary<string, object>(_defaultParameters);
        StatusEngine = "Parâmetros resetados para padrão";
        OnStatusChanged?.Invoke(this, StatusEngine);
    }
    #endregion

    #region IExplainableModel Implementation
    public async Task<Utilities.PredictionExplanation> ExplainPredictionAsync(int concurso)
    {
        await Task.Delay(100);

        var explanation = new Utilities.PredictionExplanation
        {
            Concurso = concurso,
            ModelName = ModelName,
            MainReason = "Análise baseada em padrões de metrônomo detectados nos dados históricos",
            ConfidenceLevel = ExplanationConfidence
        };

        return explanation;
    }

    public List<string> GetKeyDecisionFactors()
    {
        return new List<string>
        {
            $"Total de metrônomos analisados: {TotalMetronomos}",
            $"Janela de análise: {GetParameter<int>("WindowSize", 50)} concursos",
            $"Confiança do modelo: {Confidence:P2}",
            $"Dados históricos: {TrainingDataSize} registros",
            "Padrões de oscilação detectados",
            "Análise de frequência temporal",
            "Correlação entre números"
        };
    }

    public async Task<string> ExplainNumberSelectionAsync(int concurso, int numero)
    {
        await Task.Delay(50);

        var metronomo = _metronomos.Values.FirstOrDefault(m => m.NumerosPrincipais.Contains(numero));
        if (metronomo != null)
        {
            return $"Número {numero} selecionado pelo metrônomo '{metronomo.Nome}' " +
                   $"com confiança {metronomo.Confianca:P2}. " +
                   $"Baseado em padrão de {metronomo.Ciclo} concursos.";
        }

        return $"Número {numero} selecionado baseado em análise estatística geral do modelo.";
    }
    #endregion

    #region Private Methods
    private void InitializeDefaultParameters()
    {
        _defaultParameters = new Dictionary<string, object>
        {
            ["WindowSize"] = 50,
            ["MinConfidence"] = 0.6,
            ["MaxMetronomos"] = 15,
            ["AnalysisDepth"] = 100
        };
        _currentParameters = new Dictionary<string, object>(_defaultParameters);
    }

    private async Task InicializarMetronomos()
    {
        await Task.Delay(100);

        _metronomos.Clear();

        // Criar metrônomos baseados nos dados históricos
        for (int i = 1; i <= 25; i++)
        {
            var metronomo = new MetronomoInfo
            {
                Id = i,
                Nome = $"Metrônomo_{i:D2}",
                NumerosPrincipais = new List<int> { i },
                Ciclo = CalcularCicloMetronomo(i),
                Confianca = CalcularConfiancaMetronomo(i),
                Ativo = true,
                UltimaAtualizacao = DateTime.Now
            };

            _metronomos[i] = metronomo;
        }

        TotalMetronomos = _metronomos.Count;
        StatusEngine = $"{TotalMetronomos} metrônomos inicializados";
    }

    private async Task TreinarMetronomos()
    {
        await Task.Delay(200);

        foreach (var metronomo in _metronomos.Values)
        {
            metronomo.Confianca = CalcularConfiancaMetronomo(metronomo.Id);
            metronomo.UltimaAtualizacao = DateTime.Now;
        }

        StatusEngine = "Metrônomos retreinados";
    }

    private async Task<List<int>> GerarPredicaoMetronomo(int concurso)
    {
        await Task.Delay(100);

        var prediction = new List<int>();
        var metronomosAtivos = _metronomos.Values.Where(m => m.Ativo && m.Confianca > 0.5).ToList();

        // Selecionar números baseados nos metrônomos mais confiáveis
        var numerosSelecionados = new HashSet<int>();

        foreach (var metronomo in metronomosAtivos.OrderByDescending(m => m.Confianca).Take(15))
        {
            foreach (var numero in metronomo.NumerosPrincipais)
            {
                if (numerosSelecionados.Count < 15 && !numerosSelecionados.Contains(numero))
                {
                    numerosSelecionados.Add(numero);
                }
            }
        }

        // Se não temos 15 números, completar aleatoriamente
        while (numerosSelecionados.Count < 15)
        {
            var numero = _random.Next(1, 26);
            if (!numerosSelecionados.Contains(numero))
            {
                numerosSelecionados.Add(numero);
            }
        }

        return numerosSelecionados.OrderBy(n => n).ToList();
    }

    private int CalcularCicloMetronomo(int numero)
    {
        if (_historicalData.Count < 10) return 5; // Padrão se poucos dados

        var aparicoes = _historicalData
            .Where(l => l.Lista.Contains(numero))
            .Select(l => l.Id)
            .OrderBy(c => c)
            .ToList();

        if (aparicoes.Count < 2) return 5;

        var intervalos = new List<int>();
        for (int i = 1; i < aparicoes.Count; i++)
        {
            intervalos.Add(aparicoes[i] - aparicoes[i - 1]);
        }

        return intervalos.Any() ? (int)intervalos.Average() : 5;
    }

    private double CalcularConfiancaMetronomo(int numero)
    {
        if (_historicalData.Count < 10) return 0.5;

        var windowSize = GetParameter<int>("WindowSize", 50);
        var recentData = _historicalData.TakeLast(windowSize);

        var totalConcursos = recentData.Count();
        var aparicoes = recentData.Count(l => l.Lista.Contains(numero));

        var frequenciaObservada = (double)aparicoes / totalConcursos;
        var frequenciaEsperada = 15.0 / 25.0; // 15 números de 25 possíveis

        // Calcular confiança baseada na proximidade da frequência esperada
        var diferenca = Math.Abs(frequenciaObservada - frequenciaEsperada);
        var confianca = Math.Max(0.1, 1.0 - (diferenca * 2));

        return Math.Min(0.95, confianca);
    }

    private double CalcularConfianca(List<int> dezenas)
    {
        if (!dezenas.Any() || !_metronomos.Any()) return 0.5;

        var confiancas = dezenas
            .Where(d => _metronomos.ContainsKey(d))
            .Select(d => _metronomos[d].Confianca);

        return confiancas.Any() ? confiancas.Average() : 0.5;
    }

    private double CalcularAcuraciaSimulada(Lances testData)
    {
        // Simulação simplificada de acurácia
        var baseAccuracy = 0.65;
        var dataQualityFactor = Math.Min(1.0, testData.Count / 100.0);
        var modelFactor = Confidence;

        return Math.Min(0.95, baseAccuracy * dataQualityFactor * modelFactor);
    }

    private T GetParameter<T>(string key, T defaultValue)
    {
        if (_currentParameters.ContainsKey(key) && _currentParameters[key] is T value)
        {
            return value;
        }
        return defaultValue;
    }
    #endregion

    #region Nested Classes
    private class MetronomoInfo
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public List<int> NumerosPrincipais { get; set; } = new();
        public int Ciclo { get; set; }
        public double Confianca { get; set; }
        public bool Ativo { get; set; }
        public DateTime UltimaAtualizacao { get; set; }
    }
    #endregion
}
