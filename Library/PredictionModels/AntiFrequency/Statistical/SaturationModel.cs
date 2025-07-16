// E:\PROJETOS\GraphFacil\Library\PredictionModels\AntiFrequency\Statistical\SaturationModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Models;
using LotoLibrary.Models.Validation;
using LotoLibrary.Models.Base;
using LotoLibrary.Enums;

namespace LotoLibrary.PredictionModels.AntiFrequency.Statistical
{
    /// <summary>
    /// Modelo de Saturação - baseado na teoria de que números "saturados" 
    /// (que apareceram muito) tendem a não aparecer nos próximos sorteios
    /// </summary>
    public class SaturationModel : PredictionModelBase
    {
        #region Fields
        private Dictionary<int, SaturationInfo> _saturationData;
        private int _saturationWindow;
        private double _saturationThreshold;
        private int _cooldownPeriod;
        #endregion

        #region Properties
        public Dictionary<int, SaturationInfo> SaturationData => new Dictionary<int, SaturationInfo>(_saturationData);
        public int SaturationWindow => _saturationWindow;
        public double SaturationThreshold => _saturationThreshold;
        public int CooldownPeriod => _cooldownPeriod;
        #endregion

        #region Constructor
        public SaturationModel() : base("Saturation Model", ModelType.Saturation)
        {
            _saturationData = new Dictionary<int, SaturationInfo>();
            _saturationWindow = 20; // Analisar últimos 20 concursos para saturação
            _saturationThreshold = 0.6; // 60% de aparições na janela = saturado
            _cooldownPeriod = 5; // Período de "resfriamento" após saturação
            
            // Inicializar dados de saturação
            for (int i = 1; i <= 25; i++)
            {
                _saturationData[i] = new SaturationInfo
                {
                    Number = i,
                    AppearanceCount = 0,
                    LastAppearance = 0,
                    SaturationLevel = 0.0,
                    IsSaturated = false,
                    CooldownRemaining = 0
                };
            }
        }
        #endregion

        #region Abstract Methods Implementation

        protected override async Task InitializeModelSpecificAsync(Lances historicalData)
        {
            LogInfo("Inicializando modelo de Saturação...");
            
            // Calcular dados iniciais de saturação
            CalculateSaturationData(historicalData);
            
            // Métricas iniciais
            AddMetric("TotalConcursos", historicalData.Count);
            AddMetric("JanelaSaturacao", _saturationWindow);
            AddMetric("LimiarSaturacao", _saturationThreshold);
            AddMetric("PeriodoResfriamento", _cooldownPeriod);
            AddMetric("NumerosSaturados", GetSaturatedNumbers().Count);
            
            LogInfo($"Modelo inicializado. {GetSaturatedNumbers().Count} números saturados");
            
            await Task.CompletedTask;
        }

        protected override async Task<bool> TrainModelSpecificAsync(Lances historicalData)
        {
            LogInfo("Treinando modelo de Saturação...");
            
            try
            {
                // Analisar padrões de saturação históricos
                AnalyzeSaturationPatterns(historicalData);
                
                // Calcular efetividade da estratégia
                var effectiveness = CalculateStrategyEffectiveness(historicalData);
                
                // Otimizar parâmetros baseado na efetividade
                OptimizeParameters(effectiveness);
                
                // Atualizar dados de saturação
                CalculateSaturationData(historicalData);
                
                // Métricas de treinamento
                AddMetric("EfetividadeEstrategia", effectiveness);
                AddMetric("NumerosSaturados", GetSaturatedNumbers().Count);
                AddMetric("NumerosEmResfriamento", GetCoolingNumbers().Count);
                
                LogInfo($"Treinamento concluído. Efetividade: {effectiveness:P2}");
                
                await Task.CompletedTask;
                return true;
            }
            catch (Exception ex)
            {
                LogError($"Erro durante treinamento: {ex.Message}");
                return false;
            }
        }

        protected override async Task<List<int>> PredictModelSpecificAsync(int concurso)
        {
            LogInfo($"Gerando predição para concurso {concurso}...");
            
            try
            {
                var prediction = new List<int>();
                
                // Atualizar cooldown
                UpdateCooldownStatus();
                
                // Estratégia 1: Evitar números saturados
                var nonSaturatedNumbers = GetNonSaturatedNumbers();
                
                // Estratégia 2: Priorizar números que saíram do cooldown
                var recentlyCooled = GetRecentlyCooledNumbers();
                
                // Estratégia 3: Números com baixa saturação
                var lowSaturationNumbers = GetLowSaturationNumbers();
                
                // Combinar estratégias
                // Primeiro: números que saíram do cooldown
                foreach (var number in recentlyCooled.Take(5))
                {
                    if (!prediction.Contains(number))
                    {
                        prediction.Add(number);
                    }
                }
                
                // Segundo: números não saturados
                foreach (var number in nonSaturatedNumbers)
                {
                    if (!prediction.Contains(number) && prediction.Count < 12)
                    {
                        prediction.Add(number);
                    }
                }
                
                // Terceiro: números com baixa saturação
                foreach (var number in lowSaturationNumbers)
                {
                    if (!prediction.Contains(number) && prediction.Count < 15)
                    {
                        prediction.Add(number);
                    }
                }
                
                // Garantir que temos exatamente 15 números
                var finalPrediction = ValidatePrediction(prediction);
                
                LogInfo($"Predição gerada: {string.Join(", ", finalPrediction)}");
                LogInfo($"Números saturados evitados: {string.Join(", ", GetSaturatedNumbers())}");
                
                await Task.CompletedTask;
                return finalPrediction;
            }
            catch (Exception ex)
            {
                LogError($"Erro durante predição: {ex.Message}");
                return GenerateRandomPrediction();
            }
        }

        protected override string GetModelDescription()
        {
            return "Modelo baseado na teoria de saturação, que identifica números " +
                   "que apareceram com muita frequência recentemente e evita selecioná-los, " +
                   "priorizando números que estão em 'resfriamento' ou com baixa saturação.";
        }

        #endregion

        #region Private Methods

        private void CalculateSaturationData(Lances historicalData)
        {
            var recentData = historicalData
                .OrderByDescending(l => l.Id)
                .Take(_saturationWindow)
                .ToList();
            
            // Resetar contadores
            foreach (var info in _saturationData.Values)
            {
                info.AppearanceCount = 0;
                info.LastAppearance = 0;
            }
            
            // Contar aparições na janela de saturação
            for (int i = 0; i < recentData.Count; i++)
            {
                var lance = recentData[i];
                foreach (var number in lance.Lista)
                {
                    if (number >= 1 && number <= 25)
                    {
                        _saturationData[number].AppearanceCount++;
                        if (_saturationData[number].LastAppearance == 0)
                        {
                            _saturationData[number].LastAppearance = i + 1;
                        }
                    }
                }
            }
            
            // Calcular nível de saturação
            foreach (var info in _saturationData.Values)
            {
                info.SaturationLevel = (double)info.AppearanceCount / _saturationWindow;
                info.IsSaturated = info.SaturationLevel >= _saturationThreshold;
                
                // Iniciar cooldown se saturado
                if (info.IsSaturated && info.CooldownRemaining == 0)
                {
                    info.CooldownRemaining = _cooldownPeriod;
                }
            }
        }

        private void AnalyzeSaturationPatterns(Lances historicalData)
        {
            // Analisar padrões históricos de saturação
            var patterns = new Dictionary<int, List<bool>>();
            
            for (int i = 1; i <= 25; i++)
            {
                patterns[i] = new List<bool>();
            }
            
            // Simular saturação histórica
            var windowSize = _saturationWindow;
            for (int i = windowSize; i < historicalData.Count; i++)
            {
                var windowData = historicalData.Skip(i - windowSize).Take(windowSize).ToList();
                var nextDraw = historicalData.Skip(i).FirstOrDefault();
                
                if (nextDraw != null)
                {
                    foreach (var number in Enumerable.Range(1, 25))
                    {
                        var appearances = windowData.Count(l => l.Lista.Contains(number));
                        var saturationLevel = (double)appearances / windowSize;
                        var wasSaturated = saturationLevel >= _saturationThreshold;
                        var appearedNext = nextDraw.Lista.Contains(number);
                        
                        patterns[number].Add(wasSaturated && !appearedNext);
                    }
                }
            }
            
            // Calcular efetividade por número
            foreach (var number in patterns.Keys)
            {
                if (patterns[number].Count > 0)
                {
                    var effectiveness = patterns[number].Count(x => x) / (double)patterns[number].Count;
                    AddMetric($"Efetividade_{number:D2}", effectiveness);
                }
            }
        }

        private double CalculateStrategyEffectiveness(Lances historicalData)
        {
            if (historicalData.Count < _saturationWindow + 10)
            {
                return 0.5; // Dados insuficientes
            }
            
            int correctPredictions = 0;
            int totalPredictions = 0;
            
            // Testar estratégia nos últimos concursos
            for (int i = _saturationWindow; i < Math.Min(historicalData.Count, _saturationWindow + 50); i++)
            {
                var testData = historicalData.Take(i).ToList();
                var actualDraw = historicalData.Skip(i).FirstOrDefault();
                
                if (actualDraw != null)
                {
                    // Simular predição
                    var tempModel = new SaturationModel();
                    tempModel.CalculateSaturationData(testData);
                    
                    var saturatedNumbers = tempModel.GetSaturatedNumbers();
                    var nonSaturatedNumbers = tempModel.GetNonSaturatedNumbers();
                    
                    // Verificar se números saturados realmente não apareceram
                    var saturatedThatAppeared = saturatedNumbers.Count(n => actualDraw.Lista.Contains(n));
                    var nonSaturatedThatAppeared = nonSaturatedNumbers.Count(n => actualDraw.Lista.Contains(n));
                    
                    if (saturatedThatAppeared < nonSaturatedThatAppeared)
                    {
                        correctPredictions++;
                    }
                    
                    totalPredictions++;
                }
            }
            
            return totalPredictions > 0 ? (double)correctPredictions / totalPredictions : 0.5;
        }

        private void OptimizeParameters(double currentEffectiveness)
        {
            // Otimizar parâmetros baseado na efetividade
            if (currentEffectiveness < 0.4)
            {
                // Reduzir limiar de saturação
                _saturationThreshold = Math.Max(0.4, _saturationThreshold - 0.1);
                LogInfo($"Limiar de saturação reduzido para {_saturationThreshold:P1}");
            }
            else if (currentEffectiveness > 0.7)
            {
                // Aumentar limiar de saturação
                _saturationThreshold = Math.Min(0.8, _saturationThreshold + 0.05);
                LogInfo($"Limiar de saturação aumentado para {_saturationThreshold:P1}");
            }
        }

        private void UpdateCooldownStatus()
        {
            foreach (var info in _saturationData.Values)
            {
                if (info.CooldownRemaining > 0)
                {
                    info.CooldownRemaining--;
                }
            }
        }

        private List<int> GetSaturatedNumbers()
        {
            return _saturationData.Values
                .Where(info => info.IsSaturated)
                .Select(info => info.Number)
                .ToList();
        }

        private List<int> GetNonSaturatedNumbers()
        {
            return _saturationData.Values
                .Where(info => !info.IsSaturated)
                .OrderBy(info => info.SaturationLevel)
                .Select(info => info.Number)
                .ToList();
        }

        private List<int> GetRecentlyCooledNumbers()
        {
            return _saturationData.Values
                .Where(info => info.CooldownRemaining == 0 && info.SaturationLevel > 0.3)
                .OrderByDescending(info => info.SaturationLevel)
                .Select(info => info.Number)
                .ToList();
        }

        private List<int> GetLowSaturationNumbers()
        {
            return _saturationData.Values
                .Where(info => info.SaturationLevel < 0.3)
                .OrderBy(info => info.SaturationLevel)
                .Select(info => info.Number)
                .ToList();
        }

        private List<int> GetCoolingNumbers()
        {
            return _saturationData.Values
                .Where(info => info.CooldownRemaining > 0)
                .Select(info => info.Number)
                .ToList();
        }

        #endregion

        #region Override Methods

        protected override void UpdateConfidence()
        {
            // Confiança baseada na distribuição da saturação
            var saturationLevels = _saturationData.Values.Select(info => info.SaturationLevel).ToList();
            var variance = CalculateVariance(saturationLevels);
            var saturatedCount = GetSaturatedNumbers().Count;
            
            // Maior variância e números saturados moderados = maior confiança
            var varianceConfidence = Math.Min(0.9, variance * 2);
            var saturationConfidence = saturatedCount > 0 && saturatedCount < 20 ? 0.8 : 0.5;
            
            _confidence = (varianceConfidence + saturationConfidence) / 2;
            
            AddMetric("Confianca", _confidence);
            AddMetric("VarianciaSaturacao", variance);
            
            LogInfo($"Confiança atualizada para {_confidence:P2}");
        }

        private double CalculateVariance(IEnumerable<double> values)
        {
            var average = values.Average();
            return values.Sum(v => Math.Pow(v - average, 2)) / values.Count();
        }

        public override bool IsModelType(string modelType)
        {
            return modelType.Equals("Saturation", StringComparison.OrdinalIgnoreCase) ||
                   modelType.Equals("SaturationModel", StringComparison.OrdinalIgnoreCase) ||
                   base.IsModelType(modelType);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Obtém relatório de saturação
        /// </summary>
        public string GetSaturationReport()
        {
            var report = "RELATÓRIO DE SATURAÇÃO\n";
            report += "======================\n\n";
            
            var saturatedNumbers = GetSaturatedNumbers();
            var coolingNumbers = GetCoolingNumbers();
            
            report += $"NÚMEROS SATURADOS ({saturatedNumbers.Count}):\n";
            foreach (var number in saturatedNumbers)
            {
                var info = _saturationData[number];
                report += $"{number:D2}: {info.SaturationLevel:P1} (aparições: {info.AppearanceCount})\n";
            }
            
            report += $"\nNÚMEROS EM RESFRIAMENTO ({coolingNumbers.Count}):\n";
            foreach (var number in coolingNumbers)
            {
                var info = _saturationData[number];
                report += $"{number:D2}: {info.CooldownRemaining} concursos restantes\n";
            }
            
            return report;
        }

        /// <summary>
        /// Configura parâmetros do modelo
        /// </summary>
        public void ConfigureParameters(int saturationWindow, double saturationThreshold, int cooldownPeriod)
        {
            _saturationWindow = Math.Max(5, Math.Min(50, saturationWindow));
            _saturationThreshold = Math.Max(0.2, Math.Min(0.9, saturationThreshold));
            _cooldownPeriod = Math.Max(1, Math.Min(15, cooldownPeriod));
            
            LogInfo($"Parâmetros configurados: Janela={_saturationWindow}, Limiar={_saturationThreshold:P1}, Cooldown={_cooldownPeriod}");
        }

        #endregion

        #region Dispose

        public override void Dispose()
        {
            _saturationData?.Clear();
            base.Dispose();
        }

        #endregion
    }

    #region Supporting Classes

    /// <summary>
    /// Informações de saturação de um número
    /// </summary>
    public class SaturationInfo
    {
        public int Number { get; set; }
        public int AppearanceCount { get; set; }
        public int LastAppearance { get; set; }
        public double SaturationLevel { get; set; }
        public bool IsSaturated { get; set; }
        public int CooldownRemaining { get; set; }
    }

    #endregion
}