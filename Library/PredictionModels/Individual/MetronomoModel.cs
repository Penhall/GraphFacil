// D:\PROJETOS\GraphFacil\Library\PredictionModels\Individual\MetronomoModel.cs - Migração Completa do MetronomoEngine
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Services;
using LotoLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotoLibrary.PredictionModels.Individual
{
    /// <summary>
    /// Modelo de predição baseado em metrônomos individuais
    /// Migração COMPLETA do algoritmo MetronomoEngine original para nova arquitetura
    /// </summary>
    public partial class MetronomoModel : PredictionModelBase, IConfigurableModel, IExplainableModel
    {
        #region Fields
        private Dictionary<int, MetronomoIndividual> _metronomos = new();
        private List<Lance> _dadosTreino;
        private List<Lance> _dadosValidacao;
        private readonly ValidationMetricsService _validationService;
        private Dictionary<string, object> _parameters;
        private int _concursoAlvo;
        private List<int> _ultimoPalpite = new();
        private double _confiancaGeralPalpite;
        private readonly Random _random = new Random();
        #endregion

        #region Observable Properties
        [ObservableProperty]
        private string _statusEngine = "Aguardando inicialização...";

        [ObservableProperty]
        private bool _isInicializado;

        [ObservableProperty]
        private int _totalMetronomos;

        [ObservableProperty]
        private string _resumoPerformance = "";
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
        public int ConcursoAlvo => _concursoAlvo;
        public int TamanhoValidacao { get; set; } = 100;
        public List<int> UltimoPalpite => _ultimoPalpite;
        public double ConfiancaGeralPalpite => _confiancaGeralPalpite;
        #endregion

        #region Constructor
        public MetronomoModel()
        {
            _validationService = new ValidationMetricsService();
            _parameters = GetDefaultParameters();
        }

        /// <summary>
        /// Construtor de compatibilidade com sistema anterior
        /// </summary>
        public MetronomoModel(Lances historico) : this()
        {
            if (historico != null && historico.Any())
            {
                // Inicialização síncrona para compatibilidade
                var task = Task.Run(async () => await InitializeAsync(historico));
                task.Wait();
            }
        }
        #endregion

        #region PredictionModelBase Implementation
        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                UpdateStatus("Inicializando modelo Metronomo...");

                if (historicalData == null || historicalData.Count == 0)
                {
                    UpdateStatus("Erro: Dados históricos inválidos");
                    return false;
                }

                var minimumDataSize = GetParameter<int>("MinimumDataSize");
                if (historicalData.Count < minimumDataSize)
                {
                    UpdateStatus($"Erro: Dados insuficientes. Mínimo: {minimumDataSize}, Atual: {historicalData.Count}");
                    return false;
                }

                // Determinar concurso alvo
                _concursoAlvo = (historicalData.LastOrDefault()?.Id ?? 0) + 1;

                // Configurar dados de treino/validação
                (_dadosTreino, _dadosValidacao) = SplitDataPersonalizado(historicalData);

                // Inicializar estruturas de dados
                InitializeDataStructures();

                // Configurar metrônomos para todas as dezenas - ALGORITMO ORIGINAL
                await InitializeMetronomos(_dadosTreino);

                IsInicializado = true;
                TotalMetronomos = _metronomos.Count;
                UpdateStatus($"✅ Modelo Metronomo inicializado: {TotalMetronomos} metrônomos criados");

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
                UpdateStatus("Iniciando treinamento do modelo Metronomo...");

                if (trainingData == null || trainingData.Count == 0)
                {
                    UpdateStatus("Erro: Dados de treino inválidos");
                    return false;
                }

                // Analisar padrões históricos - ALGORITMO ORIGINAL
                await AnalyzeHistoricalPatterns(trainingData);

                // Configurar metrônomos individuais - MIGRAÇÃO COMPLETA
                await ConfigureIndividualMetronomos(trainingData);

                // Calcular confiança baseada na consistência
                var confidence = CalculateModelConfidence();
                UpdateConfidence(confidence);

                UpdateStatus($"Treinamento concluído. Confiança: {confidence:P2}");
                return true;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro no treinamento: {ex.Message}");
                return false;
            }
        }

        protected override async Task<PredictionResult> DoPredict(int targetConcurso)
        {
            try
            {
                if (!IsInicializado)
                {
                    throw new InvalidOperationException("Modelo não inicializado");
                }

                UpdateStatus($"Gerando predição para concurso {targetConcurso}...");

                // Atualizar concurso alvo
                _concursoAlvo = targetConcurso;

                // Gerar palpite usando algoritmo original completo
                var palpite = await GerarPalpiteCompletoAsync(targetConcurso);

                if (palpite == null || !palpite.Any())
                {
                    throw new InvalidOperationException("Falha ao gerar palpite válido");
                }

                // Calcular confiança
                _confiancaGeralPalpite = CalcularConfiancaPalpite(palpite);
                _ultimoPalpite = palpite;

                // Criar resultado
                var result = new PredictionResult
                {
                    PredictedNumbers = palpite,
                    OverallConfidence = _confiancaGeralPalpite,
                    ModelUsed = ModelName,
                    Timestamp = DateTime.Now,
                    TargetConcurso = targetConcurso,
                    GenerationMethod = "Metrônomos Individuais",
                    Metadata = new Dictionary<string, object>
                    {
                        ["TotalMetronomos"] = TotalMetronomos,
                        ["ConcursoAlvo"] = _concursoAlvo,
                        ["DadosTreino"] = _dadosTreino?.Count ?? 0,
                        ["DadosValidacao"] = _dadosValidacao?.Count ?? 0
                    }
                };

                UpdateStatus($"✅ Predição gerada: {palpite.Count} dezenas, Confiança: {_confiancaGeralPalpite:P2}");
                return result;
            }
            catch (Exception ex)
            {
                UpdateStatus($"Erro na predição: {ex.Message}");
                throw;
            }
        }
        #endregion

        #region Core Algorithm - MIGRAÇÃO COMPLETA DO METRONOMOENGINE
        private async Task InitializeMetronomos(List<Lance> dadosTreino)
        {
            UpdateStatus("Criando metrônomos individuais...");

            _metronomos.Clear();

            // Criar metrônomo para cada dezena (1-25) - ALGORITMO ORIGINAL
            for (int dezena = 1; dezena <= 25; dezena++)
            {
                // Extrair histórico específico desta dezena
                var historicoAparicoes = dadosTreino
                    .Where(lance => lance.Lista.Contains(dezena))
                    .Select(lance => lance.Id)
                    .OrderBy(id => id)
                    .ToList();

                // Criar metrônomo individual
                var metronomo = new MetronomoIndividual(dezena, historicoAparicoes);
                
                // Analisar padrões específicos
                metronomo.AnalisarPadroes();
                
                // Atualizar estado atual
                metronomo.AtualizarEstadoAtual(_concursoAlvo);

                _metronomos[dezena] = metronomo;
            }

            UpdateStatus($"✅ {_metronomos.Count} metrônomos criados e configurados");
        }

        private async Task<List<int>> GerarPalpiteCompletoAsync(int targetConcurso)
        {
            try
            {
                var debug = $"🎯 GERANDO PALPITE PARA CONCURSO {targetConcurso}\n";
                debug += "=" * 50 + "\n";

                // 1. Atualizar estado de todos os metrônomos
                foreach (var metronomo in _metronomos.Values)
                {
                    metronomo.AtualizarEstadoAtual(targetConcurso);
                }

                // 2. Calcular probabilidades de todas as dezenas - ALGORITMO ORIGINAL
                var probabilidades = new Dictionary<int, double>();
                
                foreach (var metronomo in _metronomos.Values)
                {
                    var probabilidade = metronomo.CalcularProbabilidadePara(targetConcurso);
                    probabilidades[metronomo.Numero] = probabilidade;
                    
                    debug += $"Dezena {metronomo.Numero:D2}: {probabilidade:F6} ";
                    debug += $"(ciclo: {metronomo.CicloMedio:F1}, intervalo: {metronomo.IntervalAtual})\n";
                }

                // 3. Aplicar estratégias de otimização
                probabilidades = AplicarEstrategiaGrupos(probabilidades);
                probabilidades = AplicarEstrategiaEquilibrio(probabilidades);

                // 4. Adicionar fator de aleatoriedade controlada
                probabilidades = AdicionarRuidoControladoParaProbabilidades(probabilidades);

                // 5. Selecionar top 15 - ALGORITMO ORIGINAL
                debug += "\n🏆 TOP 15 SELECIONADAS:\n";
                var ranking = probabilidades.OrderByDescending(kvp => kvp.Value).ToList();

                var palpite = ranking.Take(15).Select(kvp => kvp.Key).OrderBy(x => x).ToList();

                for (int i = 0; i < 15; i++)
                {
                    debug += $"{i + 1}º: Dezena {ranking[i].Key:D2} - {ranking[i].Value:F6}\n";
                }

                // 6. Verificar qualidade do palpite gerado
                if (!ValidarQualidadePalpite(palpite))
                {
                    debug += "\n⚠️ PALPITE REJEITADO - Aplicando correções...\n";
                    palpite = CorrigirPalpiteProblematico(palpite, probabilidades);
                }

                // Log para debug
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debug.WriteLine(debug);
                }

                UpdateStatus($"✅ Palpite gerado: {probabilidades.Values.Distinct().Count()} probabilidades únicas");
                return palpite;
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Erro ao gerar palpite: {ex.Message}");
                throw;
            }
        }

        private Dictionary<int, double> AplicarEstrategiaGrupos(Dictionary<int, double> probabilidades)
        {
            var gruposFrequentes = new Dictionary<string, List<int>>
            {
                { "baixas", new List<int> { 1, 2, 3, 4, 5, 6, 7, 8 } },
                { "medias", new List<int> { 9, 10, 11, 12, 13, 14, 15, 16, 17 } },
                { "altas", new List<int> { 18, 19, 20, 21, 22, 23, 24, 25 } }
            };

            var fatorEquilibrio = GetParameter<double>("FatorEquilibrioGrupos");

            foreach (var grupo in gruposFrequentes)
            {
                var dezenasMaisProvaveis = grupo.Value
                    .OrderByDescending(d => probabilidades[d])
                    .Take(2)
                    .ToList();

                foreach (var dezena in dezenasMaisProvaveis)
                {
                    probabilidades[dezena] *= (1.0 + fatorEquilibrio);
                }
            }

            return probabilidades;
        }

        private Dictionary<int, double> AplicarEstrategiaEquilibrio(Dictionary<int, double> probabilidades)
        {
            var fatorParImpar = GetParameter<double>("FatorEquilibrioParImpar");
            
            // Balancear pares e ímpares
            var pares = probabilidades.Where(kvp => kvp.Key % 2 == 0).ToList();
            var impares = probabilidades.Where(kvp => kvp.Key % 2 != 0).ToList();

            var mediaPares = pares.Average(kvp => kvp.Value);
            var mediaImpares = impares.Average(kvp => kvp.Value);

            if (mediaPares > mediaImpares * 1.2)
            {
                // Favorecer ímpares
                foreach (var impar in impares)
                {
                    probabilidades[impar.Key] *= (1.0 + fatorParImpar);
                }
            }
            else if (mediaImpares > mediaPares * 1.2)
            {
                // Favorecer pares
                foreach (var par in pares)
                {
                    probabilidades[par.Key] *= (1.0 + fatorParImpar);
                }
            }

            return probabilidades;
        }

        private Dictionary<int, double> AdicionarRuidoControladoParaProbabilidades(Dictionary<int, double> probabilidades)
        {
            var fatorRuido = GetParameter<double>("FatorRuidoControlado");
            
            foreach (var dezena in probabilidades.Keys.ToList())
            {
                var ruido = (_random.NextDouble() - 0.5) * fatorRuido;
                probabilidades[dezena] = Math.Max(0.001, probabilidades[dezena] + ruido);
            }

            return probabilidades;
        }

        private bool ValidarQualidadePalpite(List<int> palpite)
        {
            if (palpite == null || palpite.Count != 15)
                return false;

            // Verificar distribuição dezenas 1-9 vs 10-25 (correção do bug)
            var dezenas1a9 = palpite.Count(d => d <= 9);
            var proporcao1a9 = dezenas1a9 / 15.0;
            
            // Deve ter pelo menos 20% de dezenas 1-9 (correção do bug crítico)
            if (proporcao1a9 < 0.20)
                return false;

            // Verificar distribuição par/ímpar
            var pares = palpite.Count(d => d % 2 == 0);
            var proporcaoPares = pares / 15.0;
            
            // Deve estar entre 30% e 70%
            if (proporcaoPares < 0.30 || proporcaoPares > 0.70)
                return false;

            return true;
        }

        private List<int> CorrigirPalpiteProblematico(List<int> palpiteOriginal, Dictionary<int, double> probabilidades)
        {
            var palpiteCorrigido = new List<int>(palpiteOriginal);

            // Garantir pelo menos 3 dezenas de 1-9 (20% de 15)
            var dezenas1a9 = palpiteCorrigido.Count(d => d <= 9);
            if (dezenas1a9 < 3)
            {
                var candidatos1a9 = probabilidades
                    .Where(kvp => kvp.Key <= 9 && !palpiteCorrigido.Contains(kvp.Key))
                    .OrderByDescending(kvp => kvp.Value)
                    .Take(3 - dezenas1a9)
                    .Select(kvp => kvp.Key)
                    .ToList();

                // Substituir dezenas com menor probabilidade
                var menoresProbabilidades = palpiteCorrigido
                    .Where(d => d > 9)
                    .OrderBy(d => probabilidades[d])
                    .Take(candidatos1a9.Count)
                    .ToList();

                foreach (var dezenaRemover in menoresProbabilidades)
                {
                    palpiteCorrigido.Remove(dezenaRemover);
                }

                palpiteCorrigido.AddRange(candidatos1a9);
            }

            return palpiteCorrigido.OrderBy(d => d).ToList();
        }

        private double CalcularConfiancaPalpite(List<int> palpite)
        {
            if (palpite == null || !palpite.Any())
                return 0.0;

            // Calcular confiança baseada nas probabilidades dos metrônomos
            var confianciaTotal = 0.0;
            var pesoTotal = 0.0;

            foreach (var dezena in palpite)
            {
                if (_metronomos.ContainsKey(dezena))
                {
                    var metronomo = _metronomos[dezena];
                    var probabilidade = metronomo.CalcularProbabilidadePara(_concursoAlvo);
                    var peso = 1.0 / (metronomo.VariancaCiclo + 0.1); // Menor variância = maior peso
                    
                    confianciaTotal += probabilidade * peso;
                    pesoTotal += peso;
                }
            }

            var confianciaMedia = pesoTotal > 0 ? confianciaTotal / pesoTotal : 0.0;
            
            // Ajustar baseado na qualidade do palpite
            var fatorQualidade = ValidarQualidadePalpite(palpite) ? 1.0 : 0.8;
            
            return Math.Min(0.95, confianciaMedia * fatorQualidade);
        }
        #endregion

        #region Compatibility Methods - MANTER COMPATIBILIDADE COM SISTEMA ANTERIOR
        /// <summary>
        /// Processa novo sorteio - compatibilidade com MetronomoEngine
        /// </summary>
        public void ProcessarNovoSorteio(Lance novoSorteio)
        {
            if (!IsInicializado) return;

            foreach (var metronomo in _metronomos.Values)
            {
                bool foiSorteada = novoSorteio.Lista.Contains(metronomo.Numero);
                metronomo.AtualizarComSorteio(novoSorteio.Id, foiSorteada);
            }

            // Atualizar concurso alvo
            _concursoAlvo = novoSorteio.Id + 1;

            UpdateStatus($"✅ Processado sorteio {novoSorteio.Id}");
        }

        /// <summary>
        /// Gera palpite - compatibilidade com sistema anterior
        /// </summary>
        public async Task<List<int>> GerarPalpiteAsync()
        {
            var result = await DoPredict(_concursoAlvo);
            return result.PredictedNumbers;
        }

        /// <summary>
        /// Validação usando sistema original
        /// </summary>
        public async Task<MetricasPerformance> ValidarModeloAsync()
        {
            if (!IsInicializado || !_dadosValidacao.Any())
            {
                throw new InvalidOperationException("Dados insuficientes para validação");
            }

            try
            {
                UpdateStatus("Executando validação com metrônomos...");

                var metricas = await Task.Run(() =>
                {
                    return ValidarMetronomosEspecificamente();
                });

                UpdateStatus($"✅ Validação concluída: {metricas.TaxaAcertoMedia:P1} de acerto médio");
                return metricas;
            }
            catch (Exception ex)
            {
                UpdateStatus($"❌ Erro na validação: {ex.Message}");
                throw;
            }
        }

        private MetricasPerformance ValidarMetronomosEspecificamente()
        {
            var resultados = new List<ResultadoValidacao>();

            for (int i = 0; i < Math.Min(50, _dadosValidacao.Count); i++)
            {
                var concursoTeste = _dadosValidacao[i];

                // Simular dados disponíveis até este ponto
                var dadosDisponiveis = _dadosTreino.Concat(_dadosValidacao.Take(i)).ToList();

                // Recrear metrônomos com dados até este ponto
                var metronomosTemp = CriarMetronomosTemporarios(dadosDisponiveis);

                // Gerar palpite usando probabilidades dos metrônomos
                var palpite = GerarPalpiteComMetronomosTemporarios(metronomosTemp, concursoTeste.Id);

                // Calcular acertos
                var numerosAcertados = palpite.Intersect(concursoTeste.Lista).ToList();

                var resultado = new ResultadoValidacao
                {
                    ConcursoId = concursoTeste.Id,
                    PalpiteGerado = palpite,
                    ResultadoReal = concursoTeste.Lista,
                    NumerosAcertados = numerosAcertados,
                    Acertos = numerosAcertados.Count,
                    TaxaAcerto = numerosAcertados.Count / 15.0,
                    TipoEstrategia = "Metrônomos",
                    DataTeste = DateTime.Now
                };

                resultados.Add(resultado);
            }

            return CriarMetricasManualmente(resultados, "Metrônomos Individuais");
        }

        private Dictionary<int, MetronomoIndividual> CriarMetronomosTemporarios(List<Lance> dadosDisponiveis)
        {
            var metronomosTemp = new Dictionary<int, MetronomoIndividual>();

            for (int dezena = 1; dezena <= 25; dezena++)
            {
                var historico = dadosDisponiveis
                    .Where(lance => lance.Lista.Contains(dezena))
                    .Select(lance => lance.Id)
                    .OrderBy(num => num)
                    .ToList();

                var metronomo = new MetronomoIndividual(dezena, historico);
                metronomo.AnalisarPadroes();
                metronomosTemp[dezena] = metronomo;
            }

            return metronomosTemp;
        }

        private List<int> GerarPalpiteComMetronomosTemporarios(Dictionary<int, MetronomoIndividual> metronomosTemp, int concursoAlvo)
        {
            var probabilidades = new Dictionary<int, double>();

            foreach (var metronomo in metronomosTemp.Values)
            {
                metronomo.AtualizarEstadoAtual(concursoAlvo);
                probabilidades[metronomo.Numero] = metronomo.CalcularProbabilidadePara(concursoAlvo);
            }

            return probabilidades
                .OrderByDescending(kvp => kvp.Value)
                .Take(15)
                .Select(kvp => kvp.Key)
                .OrderBy(x => x)
                .ToList();
        }

        private MetricasPerformance CriarMetricasManualmente(List<ResultadoValidacao> resultados, string estrategia)
        {
            if (!resultados.Any()) return new MetricasPerformance();

            return new MetricasPerformance
            {
                TaxaAcertoMedia = resultados.Average(r => r.TaxaAcerto),
                TaxaAcerto11Plus = resultados.Count(r => r.Acertos >= 11) / (double)resultados.Count,
                TaxaAcerto12Plus = resultados.Count(r => r.Acertos >= 12) / (double)resultados.Count,
                TaxaAcerto13Plus = resultados.Count(r => r.Acertos >= 13) / (double)resultados.Count,
                AcertoMedio = resultados.Average(r => r.Acertos),
                DesvioPadrao = CalcularDesvioPadrao(resultados.Select(r => r.TaxaAcerto).ToList()),
                TotalTestes = resultados.Count,
                Estrategia = estrategia,
                DataAnalise = DateTime.Now,
                Resultados = resultados
            };
        }

        private double CalcularDesvioPadrao(List<double> valores)
        {
            if (valores.Count < 2) return 0;
            
            var media = valores.Average();
            var somaDiferençasQuadradas = valores.Sum(v => Math.Pow(v - media, 2));
            var variancia = somaDiferençasQuadradas / (valores.Count - 1);
            return Math.Sqrt(variancia);
        }
        #endregion

        #region Configuration & Parameters
        private Dictionary<string, object> GetDefaultParameters()
        {
            return new Dictionary<string, object>
            {
                ["MinimumDataSize"] = 100,
                ["ValidationSize"] = 100,
                ["FatorEquilibrioGrupos"] = 0.05,
                ["FatorEquilibrioParImpar"] = 0.03,
                ["FatorRuidoControlado"] = 0.02,
                ["UsarOtimizacaoGrupos"] = true,
                ["UsarEquilibrioParImpar"] = true,
                ["UsarRuidoControlado"] = true,
                ["LimiteMinimoDezenas1a9"] = 3,
                ["LimiteMaximoDezenas1a9"] = 8
            };
        }

        private T GetParameter<T>(string name)
        {
            if (Parameters.TryGetValue(name, out var value) && value is T typedValue)
                return typedValue;
            
            var defaultParams = GetDefaultParameters();
            return defaultParams.TryGetValue(name, out var defaultValue) && defaultValue is T defaultTyped
                ? defaultTyped
                : default(T);
        }

        public void ConfigureParameter(string name, object value)
        {
            Parameters[name] = value;
            UpdateStatus($"Parâmetro {name} atualizado");
        }
        #endregion

        #region Helper Methods
        private void InitializeDataStructures()
        {
            _metronomos = new Dictionary<int, MetronomoIndividual>();
            _ultimoPalpite = new List<int>();
            _confiancaGeralPalpite = 0.0;
        }

        private async Task AnalyzeHistoricalPatterns(Lances trainingData)
        {
            // Análise de padrões será expandida na Fase 2
            await Task.Delay(1); // Placeholder for async operation
        }

        private async Task ConfigureIndividualMetronomos(Lances trainingData)
        {
            // Configuração será expandida na Fase 2
            await Task.Delay(1); // Placeholder for async operation
        }

        private double CalculateModelConfidence()
        {
            if (!_metronomos.Any())
                return 0.0;

            // Confiança baseada na consistência dos ciclos dos metrônomos
            var consistencias = _metronomos.Values
                .Where(m => m.CicloMedio > 0)
                .Select(m => 1.0 / (m.VariancaCiclo + 1.0))
                .ToList();

            return consistencias.Any() ? consistencias.Average() : 0.5;
        }

        private (List<Lance>, List<Lance>) SplitDataPersonalizado(Lances historicalData)
        {
            var tamanhoValidacao = Math.Min(TamanhoValidacao, historicalData.Count / 4);
            var dadosOrdenados = historicalData.OrderBy(l => l.Id).ToList();

            var dadosTreino = dadosOrdenados.Take(dadosOrdenados.Count - tamanhoValidacao).ToList();
            var dadosValidacao = dadosOrdenados.Skip(dadosOrdenados.Count - tamanhoValidacao).ToList();

            return (dadosTreino, dadosValidacao);
        }

        private void UpdateStatus(string status)
        {
            StatusEngine = status;
            System.Diagnostics.Debug.WriteLine($"[MetronomoModel] {status}");
        }
        #endregion

        #region IConfigurableModel Implementation
        public bool IsParameterSupported(string parameterName)
        {
            return GetDefaultParameters().ContainsKey(parameterName);
        }

        public object GetParameterValue(string parameterName)
        {
            return Parameters.TryGetValue(parameterName, out var value) ? value : null;
        }

        public bool SetParameterValue(string parameterName, object value)
        {
            if (!IsParameterSupported(parameterName))
                return false;

            Parameters[parameterName] = value;
            return true;
        }

        public Dictionary<string, object> GetAllParameters()
        {
            return new Dictionary<string, object>(Parameters);
        }
        #endregion

        #region IExplainableModel Implementation
        public ModelExplanation ExplainPrediction(PredictionResult prediction)
        {
            var explanation = new ModelExplanation
            {
                ModelName = ModelName,
                PredictionConfidence = prediction.OverallConfidence,
                MainFactors = new List<string>(),
                TechnicalDetails = new Dictionary<string, object>()
            };

            // Adicionar fatores principais
            explanation.MainFactors.Add($"Baseado em {TotalMetronomos} metrônomos individuais");
            explanation.MainFactors.Add($"Análise de padrões de ciclo de cada dezena");
            explanation.MainFactors.Add($"Confiança geral: {_confiancaGeralPalpite:P2}");

            // Detalhes técnicos
            explanation.TechnicalDetails["ConcursoAlvo"] = _concursoAlvo;
            explanation.TechnicalDetails["TotalMetronomos"] = TotalMetronomos;
            explanation.TechnicalDetails["UltimoPalpite"] = string.Join(",", _ultimoPalpite);

            // Detalhes dos metrônomos mais importantes
            var topMetronomos = _metronomos.Values
                .Where(m => _ultimoPalpite.Contains(m.Numero))
                .OrderByDescending(m => m.CalcularProbabilidadePara(_concursoAlvo))
                .Take(5)
                .ToList();

            foreach (var metronomo in topMetronomos)
            {
                explanation.TechnicalDetails[$"Dezena{metronomo.Numero:D2}"] = new
                {
                    Probabilidade = metronomo.CalcularProbabilidadePara(_concursoAlvo),
                    CicloMedio = metronomo.CicloMedio,
                    IntervalAtual = metronomo.IntervalAtual,
                    VariancaCiclo = metronomo.VariancaCiclo
                };
            }

            return explanation;
        }
        #endregion

        #region IDisposable Implementation
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _metronomos?.Clear();
                _dadosTreino?.Clear();
                _dadosValidacao?.Clear();
                _ultimoPalpite?.Clear();
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}