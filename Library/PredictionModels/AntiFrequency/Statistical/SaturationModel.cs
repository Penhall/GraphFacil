// Library/PredictionModels/AntiFrequency/Statistical/SaturationModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.PredictionModels.AntiFrequency.Base;
using LotoLibrary.Enums;
using LotoLibrary.Utilities.AntiFrequency;

namespace LotoLibrary.PredictionModels.AntiFrequency.Statistical
{
    /// <summary>
    /// Modelo de Saturação - Detecta reversões usando indicadores técnicos
    /// Baseado em RSI e Bandas de Bollinger adaptados para loterias
    /// Identifica quando dezenas "quentes" estão saturadas e prestes a reverter
    /// </summary>
    public class SaturationModel : AntiFrequencyModelBase, IConfigurableModel, IExplainableModel
    {
        #region Propriedades Públicas

        public override string ModelName => "Saturação (RSI + Bollinger)";
        public override string ModelType => "AntiFrequency-Saturation";
        public override string ModelDescription => 
            "Detecta saturação de dezenas usando RSI e Bandas de Bollinger. " +
            "Identifica reversões quando dezenas quentes atingem níveis de sobrecompra.";

        public override ModelComplexity Complexity => ModelComplexity.High;
        public override AntiFrequencyStrategy Strategy => AntiFrequencyStrategy.TechnicalIndicators;

        #endregion

        #region Campos Privados

        private Dictionary<int, SaturationProfile> _saturationProfiles;
        private Dictionary<int, List<double>> _frequencyHistory;
        private Dictionary<int, double> _currentRSI;
        private Dictionary<int, BollingerBands> _bollingerBands;
        private DateTime _lastCalculation;

        #endregion

        #region Construtor

        public SaturationModel()
        {
            InitializeDefaultParameters();
            InitializeDataStructures();
        }

        #endregion

        #region Configuração de Parâmetros

        protected override void InitializeDefaultParameters()
        {
            DefaultParameters = new Dictionary<string, object>
            {
                // Parâmetros RSI
                ["PeriodoRSI"] = 14,                    // Período para cálculo do RSI
                ["RSILimiteSupercor"] = 70.0,          // Limite superior RSI (sobrecompra)
                ["RSILimiteSubvendido"] = 30.0,        // Limite inferior RSI (sobrevenda)
                ["RSIForcaMinima"] = 50.0,             // RSI mínimo para considerar saturação

                // Parâmetros Bandas de Bollinger
                ["PeriodoBollinger"] = 20,             // Período para média móvel
                ["MultiplicadorBollinger"] = 2.0,     // Multiplicador do desvio padrão
                ["LimiteBandaSuperior"] = 0.8,         // Limite para banda superior
                ["LimiteBandaInferior"] = 0.2,         // Limite para banda inferior

                // Parâmetros de Saturação
                ["FatorSaturacao"] = 1.5,             // Amplificação do sinal de saturação
                ["ThresholdMomentum"] = 0.15,          // Threshold para momentum reverso
                ["PesoRSI"] = 0.5,                     // Peso do RSI no score final
                ["PesoBollinger"] = 0.3,               // Peso das Bandas no score final
                ["PesoMomentum"] = 0.2,                // Peso do Momentum no score final

                // Filtros Avançados
                ["FiltroVolatilidade"] = true,        // Aplicar filtro de volatilidade
                ["FiltroTendencia"] = true,            // Aplicar filtro de tendência
                ["FiltroRegime"] = true,               // Aplicar filtro de regime
                ["FatoreCorreacao"] = 0.9              // Fator de correção geral
            };

            base.InitializeDefaultParameters();
        }

        #endregion

        #region Inicialização

        protected override async Task<bool> DoInitializeAsync()
        {
            try
            {
                ChangeStatus(ModelStatus.Initializing, "Iniciando análise de saturação...");

                // Carregar dados históricos
                await LoadHistoricalData();

                // Calcular indicadores técnicos
                await CalculateTechnicalIndicators();

                // Gerar perfis de saturação
                await GenerateSaturationProfiles();

                // Validar configuração
                var isValid = await ValidateConfiguration();

                if (isValid)
                {
                    ChangeStatus(ModelStatus.Ready, $"Modelo inicializado. {_saturationProfiles.Count} perfis criados.");
                    return true;
                }
                else
                {
                    ChangeStatus(ModelStatus.Error, "Falha na validação da configuração.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                ChangeStatus(ModelStatus.Error, $"Erro na inicialização: {ex.Message}");
                return false;
            }
        }

        private void InitializeDataStructures()
        {
            _saturationProfiles = new Dictionary<int, SaturationProfile>();
            _frequencyHistory = new Dictionary<int, List<double>>();
            _currentRSI = new Dictionary<int, double>();
            _bollingerBands = new Dictionary<int, BollingerBands>();
            _lastCalculation = DateTime.MinValue;
        }

        private async Task LoadHistoricalData()
        {
            const int janelaDados = 200; // Últimos 200 sorteios para análise
            
            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var frequencias = new List<double>();
                
                // Simular carregamento de frequências históricas
                for (int i = 0; i < janelaDados; i++)
                {
                    // Aqui você conectaria com seus dados reais
                    // Por agora, simular uma distribuição realística
                    var frequencia = SimulateRealisticFrequency(dezena, i);
                    frequencias.Add(frequencia);
                }
                
                _frequencyHistory[dezena] = frequencias;
            }

            await Task.Delay(10); // Simular operação assíncrona
        }

        private double SimulateRealisticFrequency(int dezena, int posicao)
        {
            // Simular frequência realística baseada em padrões da Lotofácil
            var random = new Random(dezena * 1000 + posicao);
            var baseFreq = 0.6; // Frequência base (~60%)
            var variation = random.NextDouble() * 0.4 - 0.2; // Variação ±20%
            return Math.Max(0.1, Math.Min(0.9, baseFreq + variation));
        }

        #endregion

        #region Cálculo de Indicadores Técnicos

        private async Task CalculateTechnicalIndicators()
        {
            var periodoRSI = (int)GetParameter("PeriodoRSI");
            var periodoBollinger = (int)GetParameter("PeriodoBollinger");
            var multiplicadorBollinger = (double)GetParameter("MultiplicadorBollinger");

            foreach (var dezena in Enumerable.Range(1, 25))
            {
                var frequencias = _frequencyHistory[dezena];
                
                // Calcular RSI
                var rsi = CalculateRSI(frequencias, periodoRSI);
                _currentRSI[dezena] = rsi;

                // Calcular Bandas de Bollinger
                var bollinger = CalculateBollingerBands(frequencias, periodoBollinger, multiplicadorBollinger);
                _bollingerBands[dezena] = bollinger;
            }

            await Task.Delay(5); // Simular processamento
        }

        private double CalculateRSI(List<double> prices, int period)
        {
            if (prices.Count < period + 1) return 50.0; // RSI neutro

            var gains = new List<double>();
            var losses = new List<double>();

            // Calcular ganhos e perdas
            for (int i = 1; i < prices.Count; i++)
            {
                var change = prices[i] - prices[i - 1];
                gains.Add(change > 0 ? change : 0);
                losses.Add(change < 0 ? Math.Abs(change) : 0);
            }

            // Calcular médias
            var avgGain = gains.TakeLast(period).Average();
            var avgLoss = losses.TakeLast(period).Average();

            if (avgLoss == 0) return 100.0; // Evitar divisão por zero

            var rs = avgGain / avgLoss;
            var rsi = 100.0 - (100.0 / (1 + rs));

            return Math.Max(0, Math.Min(100, rsi));
        }

        private BollingerBands CalculateBollingerBands(List<double> prices, int period, double multiplier)
        {
            if (prices.Count < period)
            {
                var avg = prices.Average();
                return new BollingerBands
                {
                    Upper = avg * 1.1,
                    Middle = avg,
                    Lower = avg * 0.9,
                    Position = 0.5
                };
            }

            var recentPrices = prices.TakeLast(period).ToList();
            var sma = recentPrices.Average();
            var variance = recentPrices.Select(p => Math.Pow(p - sma, 2)).Average();
            var stdDev = Math.Sqrt(variance);

            var upper = sma + (stdDev * multiplier);
            var lower = sma - (stdDev * multiplier);
            var current = prices.Last();

            // Calcular posição percentual dentro das bandas
            var position = upper > lower ? (current - lower) / (upper - lower) : 0.5;
            position = Math.Max(0, Math.Min(1, position));

            return new BollingerBands
            {
                Upper = upper,
                Middle = sma,
                Lower = lower,
                Position = position
            };
        }

        #endregion

        #region Geração de Perfis de Saturação

        private async Task GenerateSaturationProfiles()
        {
            foreach (var dezena in Enumerable.Range(1, 25))
            {
                var profile = await CreateSaturationProfile(dezena);
                _saturationProfiles[dezena] = profile;
            }
        }

        private async Task<SaturationProfile> CreateSaturationProfile(int dezena)
        {
            var rsi = _currentRSI[dezena];
            var bollinger = _bollingerBands[dezena];
            var frequencias = _frequencyHistory[dezena];

            // Calcular saturação baseada no RSI
            var saturationRSI = CalculateSaturationFromRSI(rsi);

            // Calcular saturação baseada nas Bandas de Bollinger
            var saturationBollinger = CalculateSaturationFromBollinger(bollinger);

            // Calcular momentum de saturação
            var momentum = CalculateSaturationMomentum(frequencias);

            // Combinar sinais
            var pesoRSI = (double)GetParameter("PesoRSI");
            var pesoBollinger = (double)GetParameter("PesoBollinger");
            var pesoMomentum = (double)GetParameter("PesoMomentum");

            var saturationScore = (saturationRSI * pesoRSI) + 
                                (saturationBollinger * pesoBollinger) + 
                                (momentum * pesoMomentum);

            // Aplicar filtros
            saturationScore = ApplyFilters(dezena, saturationScore, rsi, bollinger);

            var profile = new SaturationProfile
            {
                Dezena = dezena,
                RSI = rsi,
                SaturationRSI = saturationRSI,
                BollingerPosition = bollinger.Position,
                SaturationBollinger = saturationBollinger,
                Momentum = momentum,
                SaturationScore = saturationScore,
                IsOverbought = rsi > (double)GetParameter("RSILimiteSupercor"),
                IsOversold = rsi < (double)GetParameter("RSILimiteSubvendido"),
                IsAtUpperBand = bollinger.Position > (double)GetParameter("LimiteBandaSuperior"),
                IsAtLowerBand = bollinger.Position < (double)GetParameter("LimiteBandaInferior"),
                ReversalSignal = saturationScore > 0.7 ? "FORTE" : 
                               saturationScore > 0.5 ? "MODERADO" : "FRACO"
            };

            await Task.Delay(1); // Simular processamento
            return profile;
        }

        private double CalculateSaturationFromRSI(double rsi)
        {
            var limiteSuper = (double)GetParameter("RSILimiteSupercor");
            var limiteSub = (double)GetParameter("RSILimiteSubvendido");

            if (rsi >= limiteSuper)
            {
                // Sobrecompra - sinal de saturação alta
                return Math.Min(1.0, (rsi - limiteSuper) / (100 - limiteSuper));
            }
            else if (rsi <= limiteSub)
            {
                // Sobrevenda - sinal de saturação baixa (reversa)
                return Math.Min(1.0, (limiteSub - rsi) / limiteSub);
            }
            else
            {
                // Zona neutra
                return 0.0;
            }
        }

        private double CalculateSaturationFromBollinger(BollingerBands bollinger)
        {
            var limiteSuperior = (double)GetParameter("LimiteBandaSuperior");
            var limiteInferior = (double)GetParameter("LimiteBandaInferior");

            if (bollinger.Position >= limiteSuperior)
            {
                // Próximo à banda superior - saturação
                return (bollinger.Position - limiteSuperior) / (1.0 - limiteSuperior);
            }
            else if (bollinger.Position <= limiteInferior)
            {
                // Próximo à banda inferior - saturação reversa
                return (limiteInferior - bollinger.Position) / limiteInferior;
            }
            else
            {
                return 0.0;
            }
        }

        private double CalculateSaturationMomentum(List<double> frequencias)
        {
            if (frequencias.Count < 10) return 0.0;

            // Calcular momentum das últimas 5 vs 5 anteriores
            var recent = frequencias.TakeLast(5).Average();
            var previous = frequencias.Skip(frequencias.Count - 10).Take(5).Average();
            
            var momentum = previous > 0 ? (recent - previous) / previous : 0.0;
            var threshold = (double)GetParameter("ThresholdMomentum");

            // Normalizar momentum
            return Math.Max(-1.0, Math.Min(1.0, momentum / threshold));
        }

        private double ApplyFilters(int dezena, double baseScore, double rsi, BollingerBands bollinger)
        {
            var score = baseScore;
            var fatorCorrecao = (double)GetParameter("FatoreCorreacao");

            // Filtro de Volatilidade
            if ((bool)GetParameter("FiltroVolatilidade"))
            {
                var volatilidade = CalculateVolatility(_frequencyHistory[dezena]);
                if (volatilidade < 0.05) // Baixa volatilidade
                {
                    score *= 0.8; // Reduzir confiança
                }
            }

            // Filtro de Tendência
            if ((bool)GetParameter("FiltroTendencia"))
            {
                var tendencia = CalculateTrend(_frequencyHistory[dezena]);
                if (Math.Abs(tendencia) < 0.02) // Tendência muito fraca
                {
                    score *= 0.9;
                }
            }

            // Filtro de Regime
            if ((bool)GetParameter("FiltroRegime"))
            {
                if (rsi > 40 && rsi < 60 && bollinger.Position > 0.3 && bollinger.Position < 0.7)
                {
                    // Regime lateral - reduzir confiança
                    score *= 0.85;
                }
            }

            return score * fatorCorrecao;
        }

        private double CalculateVolatility(List<double> values)
        {
            if (values.Count < 2) return 0.0;
            
            var mean = values.Average();
            var variance = values.Select(v => Math.Pow(v - mean, 2)).Average();
            return Math.Sqrt(variance);
        }

        private double CalculateTrend(List<double> values)
        {
            if (values.Count < 10) return 0.0;

            var recent = values.TakeLast(5).Average();
            var older = values.Take(5).Average();
            
            return older > 0 ? (recent - older) / older : 0.0;
        }

        #endregion

        #region Predição Principal

        protected override async Task<List<int>> DoPredict(int concurso)
        {
            try
            {
                ChangeStatus(ModelStatus.Predicting, "Analisando saturação das dezenas...");

                // Atualizar indicadores se necessário
                await UpdateIndicatorsIfNeeded();

                // Calcular scores de saturação
                var saturationScores = CalculateSaturationScores();

                // Selecionar dezenas com maior saturação (mais prováveis de reverter)
                var selectedNumbers = SelectBestNumbers(saturationScores);

                // Aplicar balanceamento final
                selectedNumbers = ApplyFinalBalancing(selectedNumbers);

                ChangeStatus(ModelStatus.Ready, $"Predição concluída. {selectedNumbers.Count} dezenas selecionadas.");
                
                return selectedNumbers;
            }
            catch (Exception ex)
            {
                ChangeStatus(ModelStatus.Error, $"Erro na predição: {ex.Message}");
                return new List<int>();
            }
        }

        private async Task UpdateIndicatorsIfNeeded()
        {
            var horasDesdeUltimaAtualizacao = (DateTime.Now - _lastCalculation).TotalHours;
            
            if (horasDesdeUltimaAtualizacao > 1.0) // Atualizar a cada hora
            {
                await CalculateTechnicalIndicators();
                await GenerateSaturationProfiles();
                _lastCalculation = DateTime.Now;
            }
        }

        private Dictionary<int, double> CalculateSaturationScores()
        {
            var scores = new Dictionary<int, double>();
            var fatorSaturacao = (double)GetParameter("FatorSaturacao");

            foreach (var profile in _saturationProfiles.Values)
            {
                var score = profile.SaturationScore * fatorSaturacao;
                
                // Bonificar sinais fortes de reversão
                if (profile.IsOverbought && profile.IsAtUpperBand)
                {
                    score *= 1.3; // Amplificar sinal de saturação
                }
                else if (profile.IsOversold && profile.IsAtLowerBand)
                {
                    score *= 1.2; // Amplificar sinal de subsaturação
                }

                scores[profile.Dezena] = Math.Max(0, Math.Min(1, score));
            }

            return scores;
        }

        private List<int> SelectBestNumbers(Dictionary<int, double> scores)
        {
            // Ordenar por score de saturação (maior = mais provável reversão)
            var sortedNumbers = scores
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key) // Desempate por número
                .Select(kvp => kvp.Key)
                .ToList();

            // Selecionar top 15 com ajustes
            var selected = new List<int>();
            var targetCount = 15;

            // Primeiro, pegar os com saturação mais alta
            foreach (var number in sortedNumbers.Take(20)) // Considerar top 20
            {
                if (selected.Count >= targetCount) break;
                
                var profile = _saturationProfiles[number];
                
                // Filtros de qualidade
                if (profile.SaturationScore > 0.3 && // Score mínimo
                    profile.RSI != 50.0) // Evitar RSI neutro
                {
                    selected.Add(number);
                }
            }

            // Completar se necessário
            foreach (var number in sortedNumbers)
            {
                if (selected.Count >= targetCount) break;
                if (!selected.Contains(number))
                {
                    selected.Add(number);
                }
            }

            return selected.Take(targetCount).OrderBy(n => n).ToList();
        }

        #endregion

        #region Validação e Explicação

        protected override async Task<bool> ValidateConfiguration()
        {
            try
            {
                // Validar parâmetros RSI
                var periodoRSI = (int)GetParameter("PeriodoRSI");
                if (periodoRSI < 5 || periodoRSI > 30)
                {
                    AddValidationError("PeriodoRSI deve estar entre 5 e 30");
                    return false;
                }

                // Validar parâmetros Bollinger
                var periodoBollinger = (int)GetParameter("PeriodoBollinger");
                if (periodoBollinger < 10 || periodoBollinger > 50)
                {
                    AddValidationError("PeriodoBollinger deve estar entre 10 e 50");
                    return false;
                }

                // Validar dados históricos
                if (_frequencyHistory.Count != 25)
                {
                    AddValidationError("Dados históricos incompletos");
                    return false;
                }

                // Validar perfis de saturação
                if (_saturationProfiles.Count != 25)
                {
                    AddValidationError("Perfis de saturação incompletos");
                    return false;
                }

                await Task.Delay(5);
                return true;
            }
            catch (Exception ex)
            {
                AddValidationError($"Erro na validação: {ex.Message}");
                return false;
            }
        }

        public string ExplainPrediction(List<int> predictedNumbers)
        {
            if (!predictedNumbers?.Any() == true || _saturationProfiles == null)
                return "Explicação não disponível - dados insuficientes.";

            var explanation = new List<string>
            {
                "=== ANÁLISE DE SATURAÇÃO (RSI + BOLLINGER) ===\n",
                "Este modelo identifica dezenas saturadas prestes a reverter tendência.\n"
            };

            // Análise geral
            var avgRSI = _saturationProfiles.Values.Average(p => p.RSI);
            var overboughtCount = _saturationProfiles.Values.Count(p => p.IsOverbought);
            var oversoldCount = _saturationProfiles.Values.Count(p => p.IsOversold);

            explanation.Add($"📊 INDICADORES GERAIS:");
            explanation.Add($"   • RSI Médio: {avgRSI:F1}");
            explanation.Add($"   • Dezenas Sobrecompradas: {overboughtCount}");
            explanation.Add($"   • Dezenas Sobrevendidas: {oversoldCount}\n");

            // Top 5 dezenas selecionadas
            explanation.Add("🎯 TOP 5 DEZENAS SELECIONADAS:");
            foreach (var number in predictedNumbers.Take(5))
            {
                if (_saturationProfiles.TryGetValue(number, out var profile))
                {
                    explanation.Add($"   • {number:D2}: RSI {profile.RSI:F1} | " +
                                  $"Bollinger {profile.BollingerPosition:F2} | " +
                                  $"Saturação {profile.SaturationScore:F2} | " +
                                  $"Sinal: {profile.ReversalSignal}");
                }
            }

            explanation.Add($"\n💡 ESTRATÉGIA: Priorizar dezenas com alta saturação " +
                          $"(RSI > 70 ou posição Bollinger > 0.8) indicando reversão iminente.");

            return string.Join("\n", explanation);
        }

        #endregion

        #region Classes de Apoio

        private class SaturationProfile
        {
            public int Dezena { get; set; }
            public double RSI { get; set; }
            public double SaturationRSI { get; set; }
            public double BollingerPosition { get; set; }
            public double SaturationBollinger { get; set; }
            public double Momentum { get; set; }
            public double SaturationScore { get; set; }
            public bool IsOverbought { get; set; }
            public bool IsOversold { get; set; }
            public bool IsAtUpperBand { get; set; }
            public bool IsAtLowerBand { get; set; }
            public string ReversalSignal { get; set; }
        }

        private class BollingerBands
        {
            public double Upper { get; set; }
            public double Middle { get; set; }
            public double Lower { get; set; }
            public double Position { get; set; } // 0-1, posição atual dentro das bandas
        }

        #endregion
    }
}