# 🎯 **FASE 3 - ENSEMBLE E OTIMIZAÇÃO [PLANEJADA]**

## 🎯 **STATUS: AGUARDANDO CONCLUSÃO DA FASE 2**

### **Duração:** 2-3 semanas
### **Objetivo:** Criar ensemble avançado com otimização automática de pesos
### **Performance Target:** >70% com baixa volatilidade

---

## 🧠 **FUNDAMENTAÇÃO TEÓRICA ENSEMBLE**

### **Hipótese Central:**
**"Combinação inteligente de modelos independentes supera performance individual"**

### **Base Científica:**
- **Teorema do Ensemble**: Erro diminui com diversificação
- **Bias-Variance Tradeoff**: Ensemble reduz variância mantendo bias
- **Condorcet's Jury Theorem**: Maioria de classificadores independentes
- **Portfolio Theory**: Otimização de Markowitz adaptada para modelos

### **Diversificação de Estratégias:**
```
MetronomoModel        → Frequencista (explora padrões)
AntiFrequencySimple   → Anti-frequencista básico
StatisticalDebt       → Fundamentalista (dívida estatística)
Saturation           → Técnico (indicadores de reversão)
PendularOscillator   → Físico (análise de ciclos)
```

---

## 📋 **DELIVERABLES DA FASE 3**

### **🎯 SISTEMA DE ENSEMBLE AVANÇADO**

#### **1. AdvancedEnsembleModel.cs**
```csharp
// Localização: Library/PredictionModels/Composite/AdvancedEnsembleModel.cs
// Complexidade: ⭐⭐⭐⭐ Alta (otimização multi-objetivo)

public class AdvancedEnsembleModel : PredictionModelBase, IEnsembleModel, IAdaptiveModel
{
    public override string ModelName => "Ensemble Avançado";
    public override string ModelType => "Composite-Advanced";
    
    // Estratégias de combinação disponíveis
    public enum CombinationStrategy
    {
        WeightedAverage,     // Média ponderada simples
        Stacking,            // Meta-modelo para combinação
        Voting,              // Sistema de votação
        BayesianAverage,     // Média bayesiana
        DynamicWeighting,    // Pesos adaptativos
        RegimeAware          // Consciente de regimes de mercado
    }
}
```

#### **🧩 Algoritmos de Combinação:**

##### **A) Weighted Average (Baseline):**
```csharp
public async Task<PredictionResult> WeightedAverageAsync(int concurso)
{
    var scores = new Dictionary<int, double>();
    var confidenceTotal = 0.0;
    
    foreach (var model in SubModels)
    {
        var result = await model.PredictAsync(concurso);
        var weight = ModelWeights[model.ModelName] * result.Confidence;
        confidenceTotal += weight;
        
        foreach (var number in result.PredictedNumbers)
        {
            scores[number] = scores.GetValueOrDefault(number, 0) + weight;
        }
    }
    
    // Normalizar scores
    var normalizedScores = scores.ToDictionary(
        kvp => kvp.Key, 
        kvp => kvp.Value / confidenceTotal
    );
    
    return SelectTopNumbers(normalizedScores, 15);
}
```

##### **B) Stacking com Meta-Modelo:**
```csharp
public class StackingEnsemble : AdvancedEnsembleModel
{
    private IMetaLearningModel _metaModel;
    
    public async Task<PredictionResult> StackingAsync(int concurso)
    {
        // Nível 1: Predições dos modelos base
        var baseResults = new List<PredictionResult>();
        foreach (var model in SubModels)
        {
            var result = await model.PredictAsync(concurso);
            baseResults.Add(result);
        }
        
        // Nível 2: Meta-modelo combina as predições
        var features = ExtractFeaturesFromResults(baseResults);
        var metaPrediction = await _metaModel.PredictAsync(features);
        
        return metaPrediction;
    }
    
    private Dictionary<string, double> ExtractFeaturesFromResults(List<PredictionResult> results)
    {
        var features = new Dictionary<string, double>();
        
        foreach (var result in results)
        {
            features[$"{result.ModelName}_Confidence"] = result.Confidence;
            features[$"{result.ModelName}_TopNumber"] = result.PredictedNumbers.First();
            features[$"{result.ModelName}_Diversity"] = CalculateDiversity(result.PredictedNumbers);
            // ... mais features
        }
        
        return features;
    }
}
```

##### **C) Voting System:**
```csharp
public async Task<PredictionResult> VotingAsync(int concurso)
{
    var votes = new Dictionary<int, VoteInfo>();
    
    foreach (var model in SubModels)
    {
        var result = await model.PredictAsync(concurso);
        var weight = ModelWeights[model.ModelName];
        
        for (int i = 0; i < result.PredictedNumbers.Count; i++)
        {
            var number = result.PredictedNumbers[i];
            var position = i + 1; // 1-15
            var positionWeight = (16 - position) / 15.0; // Peso decrescente por posição
            
            if (!votes.ContainsKey(number))
                votes[number] = new VoteInfo();
                
            votes[number].TotalVotes += weight * positionWeight;
            votes[number].VotingModels.Add(model.ModelName);
            votes[number].AveragePosition += position;
        }
    }
    
    // Normalizar e selecionar
    foreach (var vote in votes.Values)
    {
        vote.AveragePosition /= vote.VotingModels.Count;
    }
    
    var selectedNumbers = votes
        .OrderByDescending(kvp => kvp.Value.TotalVotes)
        .Take(15)
        .Select(kvp => kvp.Key)
        .OrderBy(x => x)
        .ToList();
    
    return new PredictionResult
    {
        ModelName = ModelName,
        PredictedNumbers = selectedNumbers,
        Confidence = CalculateVotingConfidence(votes),
        VotingDetails = votes
    };
}
```

##### **D) Dynamic Weighting:**
```csharp
public class DynamicWeightingEnsemble : AdvancedEnsembleModel
{
    private Dictionary<string, Queue<double>> _recentPerformance;
    private readonly int _performanceWindow = 20;
    
    public async Task<PredictionResult> DynamicWeightingAsync(int concurso)
    {
        // Atualizar pesos baseado em performance recente
        UpdateDynamicWeights();
        
        // Aplicar decaimento temporal aos pesos
        var temporalWeights = ApplyTemporalDecay();
        
        // Combinar com pesos base
        var finalWeights = CombineWeights(ModelWeights, temporalWeights);
        
        return await WeightedAverageWithWeights(concurso, finalWeights);
    }
    
    private void UpdateDynamicWeights()
    {
        foreach (var model in SubModels)
        {
            var recentPerf = CalculateRecentPerformance(model.ModelName);
            
            if (!_recentPerformance.ContainsKey(model.ModelName))
                _recentPerformance[model.ModelName] = new Queue<double>();
                
            var queue = _recentPerformance[model.ModelName];
            queue.Enqueue(recentPerf);
            
            if (queue.Count > _performanceWindow)
                queue.Dequeue();
                
            // Ajustar peso baseado na tendência de performance
            var trend = CalculatePerformanceTrend(queue);
            var adjustment = Math.Tanh(trend) * 0.2; // ±20% ajuste máximo
            
            ModelWeights[model.ModelName] = Math.Max(0.05, 
                ModelWeights[model.ModelName] * (1 + adjustment));
        }
        
        // Renormalizar pesos
        NormalizeWeights();
    }
}
```

---

### **⚙️ SISTEMA DE OTIMIZAÇÃO DE PESOS**

#### **1. WeightOptimizer.cs**
```csharp
// Localização: Library/Services/Optimization/WeightOptimizer.cs

public class WeightOptimizer : IWeightOptimizer
{
    public enum OptimizationMethod
    {
        GridSearch,          // Busca em grade
        GeneticAlgorithm,    // Algoritmo genético
        ParticleSwarm,       // Enxame de partículas
        BayesianOptimization, // Otimização bayesiana
        GradientDescent,     // Descida do gradiente
        SimulatedAnnealing   // Recozimento simulado
    }
}
```

##### **A) Grid Search (Baseline):**
```csharp
public async Task<Dictionary<string, double>> GridSearchAsync(
    List<IPredictionModel> models, 
    Lances validationData,
    GridSearchParams parameters)
{
    var bestWeights = new Dictionary<string, double>();
    var bestPerformance = 0.0;
    
    var combinations = GenerateWeightCombinations(models, parameters);
    
    foreach (var weights in combinations)
    {
        var performance = await EvaluateWeights(weights, models, validationData);
        
        if (performance.Accuracy > bestPerformance)
        {
            bestPerformance = performance.Accuracy;
            bestWeights = new Dictionary<string, double>(weights);
        }
    }
    
    return bestWeights;
}

private IEnumerable<Dictionary<string, double>> GenerateWeightCombinations(
    List<IPredictionModel> models, GridSearchParams parameters)
{
    var modelNames = models.Select(m => m.ModelName).ToList();
    var steps = parameters.Steps;
    var minWeight = parameters.MinWeight;
    var maxWeight = parameters.MaxWeight;
    
    return GenerateRecursive(modelNames, 0, new Dictionary<string, double>(), 
                           steps, minWeight, maxWeight);
}
```

##### **B) Genetic Algorithm:**
```csharp
public class GeneticWeightOptimizer : WeightOptimizer
{
    public async Task<Dictionary<string, double>> GeneticAlgorithmAsync(
        List<IPredictionModel> models,
        Lances validationData,
        GeneticParams parameters)
    {
        var population = InitializePopulation(models.Count, parameters.PopulationSize);
        
        for (int generation = 0; generation < parameters.MaxGenerations; generation++)
        {
            // Avaliar fitness de cada indivíduo
            var fitness = await EvaluatePopulation(population, models, validationData);
            
            // Seleção dos melhores
            var selected = Selection(population, fitness, parameters.SelectionRate);
            
            // Crossover
            var offspring = Crossover(selected, parameters.CrossoverRate);
            
            // Mutação
            Mutation(offspring, parameters.MutationRate);
            
            // Nova população
            population = selected.Concat(offspring).Take(parameters.PopulationSize).ToList();
            
            // Log do progresso
            var bestFitness = fitness.Max();
            Console.WriteLine($"Generation {generation}: Best fitness = {bestFitness:F4}");
        }
        
        var bestIndividual = population[fitness.ToList().IndexOf(fitness.Max())];
        return ConvertToWeights(bestIndividual, models);
    }
    
    private List<double[]> Crossover(List<double[]> parents, double crossoverRate)
    {
        var offspring = new List<double[]>();
        
        for (int i = 0; i < parents.Count - 1; i += 2)
        {
            if (_random.NextDouble() < crossoverRate)
            {
                var (child1, child2) = UniformCrossover(parents[i], parents[i + 1]);
                offspring.Add(child1);
                offspring.Add(child2);
            }
        }
        
        return offspring;
    }
    
    private void Mutation(List<double[]> individuals, double mutationRate)
    {
        foreach (var individual in individuals)
        {
            for (int i = 0; i < individual.Length; i++)
            {
                if (_random.NextDouble() < mutationRate)
                {
                    // Mutação gaussiana
                    individual[i] += _random.NextGaussian() * 0.1;
                    individual[i] = Math.Max(0, Math.Min(1, individual[i]));
                }
            }
            
            // Renormalizar para que soma = 1
            NormalizeWeights(individual);
        }
    }
}
```

##### **C) Bayesian Optimization:**
```csharp
public class BayesianWeightOptimizer : WeightOptimizer
{
    private GaussianProcessRegressor _surrogate;
    private List<(double[] weights, double performance)> _observations;
    
    public async Task<Dictionary<string, double>> BayesianOptimizationAsync(
        List<IPredictionModel> models,
        Lances validationData,
        BayesianParams parameters)
    {
        _observations = new List<(double[], double)>();
        
        // Inicialização com pontos aleatórios
        for (int i = 0; i < parameters.InitialPoints; i++)
        {
            var weights = GenerateRandomWeights(models.Count);
            var performance = await EvaluateWeights(ConvertToWeights(weights, models), 
                                                  models, validationData);
            _observations.Add((weights, performance.Accuracy));
        }
        
        // Iterações principais
        for (int iter = 0; iter < parameters.MaxIterations; iter++)
        {
            // Treinar modelo surrogate
            _surrogate.Fit(_observations.Select(o => o.weights).ToArray(),
                          _observations.Select(o => o.performance).ToArray());
            
            // Encontrar próximo ponto via acquisition function
            var nextWeights = OptimizeAcquisition(parameters.AcquisitionFunction);
            
            // Avaliar ponto real
            var actualPerformance = await EvaluateWeights(
                ConvertToWeights(nextWeights, models), models, validationData);
                
            _observations.Add((nextWeights, actualPerformance.Accuracy));
            
            Console.WriteLine($"Iteration {iter}: Best so far = {_observations.Max(o => o.performance):F4}");
        }
        
        var bestObservation = _observations.OrderByDescending(o => o.performance).First();
        return ConvertToWeights(bestObservation.weights, models);
    }
    
    private double[] OptimizeAcquisition(AcquisitionFunction function)
    {
        // Expected Improvement ou Upper Confidence Bound
        var bestWeights = new double[_observations.First().weights.Length];
        var bestAcquisition = double.MinValue;
        
        // Otimização via BFGS ou similar
        for (int trial = 0; trial < 1000; trial++)
        {
            var candidateWeights = GenerateRandomWeights(bestWeights.Length);
            var acquisition = CalculateAcquisition(candidateWeights, function);
            
            if (acquisition > bestAcquisition)
            {
                bestAcquisition = acquisition;
                bestWeights = candidateWeights;
            }
        }
        
        return bestWeights;
    }
}
```

---

### **📊 SISTEMA DE ANÁLISE DE DIVERSIFICAÇÃO**

#### **1. DiversificationAnalyzer.cs**
```csharp
// Localização: Library/Services/Analysis/DiversificationAnalyzer.cs

public class DiversificationAnalyzer : IDiversificationAnalyzer
{
    public DiversificationReport AnalyzeEnsemble(List<IPredictionModel> models, Lances testData)
    {
        var report = new DiversificationReport();
        
        // 1. Matriz de Correlação
        report.CorrelationMatrix = CalculateCorrelationMatrix(models, testData);
        
        // 2. Diversificação por Pares
        report.PairwiseDiversification = CalculatePairwiseDiversification(models, testData);
        
        // 3. Métricas de Ensemble
        report.EnsembleDiversity = CalculateEnsembleDiversity(models, testData);
        report.EffectiveDiversity = CalculateEffectiveDiversity(report.CorrelationMatrix);
        
        // 4. Análise de Contribuição Individual
        report.IndividualContributions = AnalyzeIndividualContributions(models, testData);
        
        return report;
    }
    
    private double[,] CalculateCorrelationMatrix(List<IPredictionModel> models, Lances testData)
    {
        var predictions = new Dictionary<string, List<List<int>>>();
        
        // Coletar predições de todos os modelos
        foreach (var model in models)
        {
            predictions[model.ModelName] = new List<List<int>>();
            
            foreach (var lance in testData.Take(50)) // Amostra para análise
            {
                var result = model.PredictAsync(lance.Id + 1).Result;
                predictions[model.ModelName].Add(result.PredictedNumbers);
            }
        }
        
        // Calcular correlações par-a-par
        var correlationMatrix = new double[models.Count, models.Count];
        
        for (int i = 0; i < models.Count; i++)
        {
            for (int j = 0; j < models.Count; j++)
            {
                if (i == j)
                {
                    correlationMatrix[i, j] = 1.0;
                }
                else
                {
                    var correlation = CalculatePredictionCorrelation(
                        predictions[models[i].ModelName],
                        predictions[models[j].ModelName]
                    );
                    correlationMatrix[i, j] = correlation;
                }
            }
        }
        
        return correlationMatrix;
    }
    
    private double CalculatePredictionCorrelation(List<List<int>> pred1, List<List<int>> pred2)
    {
        var overlaps = new List<double>();
        
        for (int i = 0; i < pred1.Count; i++)
        {
            var intersection = pred1[i].Intersect(pred2[i]).Count();
            var overlap = intersection / 15.0; // Overlap normalizado
            overlaps.Add(overlap);
        }
        
        return overlaps.Average();
    }
    
    private double CalculateEffectiveDiversity(double[,] correlationMatrix)
    {
        // Baseado na teoria de portfolio: diversificação efetiva
        int n = correlationMatrix.GetLength(0);
        double avgCorrelation = 0;
        int pairs = 0;
        
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                avgCorrelation += correlationMatrix[i, j];
                pairs++;
            }
        }
        
        avgCorrelation /= pairs;
        
        // Diversificação efetiva = N / (1 + (N-1) * correlação_média)
        return n / (1 + (n - 1) * avgCorrelation);
    }
}
```

---

### **🎛️ SISTEMA DE REGIME DETECTION**

#### **1. RegimeDetector.cs**
```csharp
// Localização: Library/Services/Analysis/RegimeDetector.cs

public class RegimeDetector : IRegimeDetector
{
    public enum MarketRegime
    {
        Trending,           // Tendência clara (frequencista favorável)
        Reverting,          // Reversão à média (anti-freq favorável)  
        Sideways,           // Lateral (ensemble balanceado)
        Volatile,           // Alta volatilidade (modelos conservadores)
        Transitional        // Mudança de regime (adaptação necessária)
    }
    
    public RegimeAnalysis DetectCurrentRegime(Lances recentData, int windowSize = 50)
    {
        var window = recentData.TakeLast(windowSize);
        
        // 1. Análise de Tendência
        var trendStrength = CalculateTrendStrength(window);
        var trendDirection = CalculateTrendDirection(window);
        
        // 2. Análise de Volatilidade  
        var volatility = CalculateVolatility(window);
        var volatilityRegime = ClassifyVolatility(volatility);
        
        // 3. Análise de Momentum
        var momentum = CalculateMomentum(window);
        var momentumRegime = ClassifyMomentum(momentum);
        
        // 4. Detecção de Mudança de Regime
        var regimeChangeProb = DetectRegimeChange(window);
        
        // 5. Classificação Final
        var regime = ClassifyRegime(trendStrength, volatilityRegime, momentumRegime);
        
        return new RegimeAnalysis
        {
            CurrentRegime = regime,
            TrendStrength = trendStrength,
            TrendDirection = trendDirection,
            Volatility = volatility,
            Momentum = momentum,
            RegimeChangeProb = regimeChangeProb,
            RecommendedWeights = GetRegimeOptimalWeights(regime),
            Confidence = CalculateRegimeConfidence(regime, window)
        };
    }
    
    private Dictionary<string, double> GetRegimeOptimalWeights(MarketRegime regime)
    {
        return regime switch
        {
            MarketRegime.Trending => new()
            {
                ["MetronomoModel"] = 0.4,        // Favorece frequencista
                ["AntiFrequencySimple"] = 0.15,
                ["StatisticalDebt"] = 0.15,
                ["Saturation"] = 0.15,
                ["PendularOscillator"] = 0.15
            },
            
            MarketRegime.Reverting => new()
            {
                ["MetronomoModel"] = 0.15,       // Favorece anti-frequencistas
                ["AntiFrequencySimple"] = 0.3,
                ["StatisticalDebt"] = 0.25,
                ["Saturation"] = 0.2,
                ["PendularOscillator"] = 0.1
            },
            
            MarketRegime.Sideways => new()
            {
                ["MetronomoModel"] = 0.2,        // Balanceado
                ["AntiFrequencySimple"] = 0.2,
                ["StatisticalDebt"] = 0.2,
                ["Saturation"] = 0.2,
                ["PendularOscillator"] = 0.2
            },
            
            MarketRegime.Volatile => new()
            {
                ["MetronomoModel"] = 0.3,        // Modelos mais estáveis
                ["AntiFrequencySimple"] = 0.1,
                ["StatisticalDebt"] = 0.3,
                ["Saturation"] = 0.1,
                ["PendularOscillator"] = 0.2
            },
            
            _ => GetDefaultWeights() // Transitional ou unknown
        };
    }
}
```

---

## 📊 **CRONOGRAMA DA FASE 3**

### **📅 SEMANA 1 - Ensemble Avançado**
```
Dia 1-2: AdvancedEnsembleModel com múltiplas estratégias
Dia 3: Sistema de Stacking com meta-modelo
Dia 4: Sistema de Voting avançado
Dia 5: Dynamic Weighting implementation
```

### **📅 SEMANA 2 - Otimização de Pesos**
```
Dia 6-7: Grid Search e Genetic Algorithm
Dia 8: Bayesian Optimization
Dia 9: Particle Swarm e Simulated Annealing
Dia 10: Benchmarking de métodos de otimização
```

### **📅 SEMANA 3 - Análise e Interface**
```
Dia 11-12: DiversificationAnalyzer completo
Dia 13: RegimeDetector e adaptive weighting
Dia 14: Interface avançada para análise
Dia 15: Validação final e otimização
```

---

## 🎯 **CRITÉRIOS DE SUCESSO DA FASE 3**

### **✅ FUNCIONAIS**
- [ ] Ensemble com 5+ estratégias de combinação
- [ ] Sistema de otimização automática de pesos
- [ ] Detecção de regimes de mercado
- [ ] Interface avançada para análise

### **📊 PERFORMANCE**
- [ ] Ensemble performance >70%
- [ ] Redução de volatilidade vs modelos individuais
- [ ] Robustez em diferentes períodos históricos
- [ ] Adaptação automática a mudanças

### **🔧 TÉCNICOS**
- [ ] Otimização converge em <10 iterações
- [ ] Sistema de cache para performance
- [ ] Parallel processing para backtesting
- [ ] API para configuração externa

### **💼 NEGÓCIO**
- [ ] Dashboard interativo de análise
- [ ] Relatórios automáticos de diversificação
- [ ] Sistema de alertas para mudanças de regime
- [ ] Export para ferramentas externas

---

## 📋 **ARQUIVOS A SEREM CRIADOS**

### **🎯 Ensemble Avançado:**
```
Library/PredictionModels/Composite/
├── AdvancedEnsembleModel.cs         ← Múltiplas estratégias
├── StackingEnsemble.cs              ← Meta-modelo
├── VotingEnsemble.cs                ← Sistema de votação
├── DynamicEnsemble.cs               ← Pesos adaptativos
└── RegimeAwareEnsemble.cs           ← Consciente de regimes
```

### **⚙️ Otimização:**
```
Library/Services/Optimization/
├── WeightOptimizer.cs               ← Interface base
├── GridSearchOptimizer.cs           ← Busca em grade
├── GeneticOptimizer.cs              ← Algoritmo genético
├── BayesianOptimizer.cs             ← Otimização bayesiana
├── ParticleSwarmOptimizer.cs        ← Enxame de partículas
└── OptimizationResult.cs            ← Resultados padronizados
```

### **📊 Análise:**
```
Library/Services/Analysis/
├── DiversificationAnalyzer.cs       ← Análise de diversificação
├── RegimeDetector.cs                ← Detecção de regimes
├── PerformanceAnalyzer.cs           ← Análise expandida
├── RiskAnalyzer.cs                  ← Análise de risco
└── BacktestingFramework.cs          ← Framework expandido
```

### **🖥️ Interface Avançada:**
```
Dashboard/ViewModels/Advanced/
├── EnsembleAnalysisViewModel.cs     ← Análise de ensemble
├── OptimizationViewModel.cs         ← Controle de otimização
├── RegimeAnalysisViewModel.cs       ← Análise de regimes
└── PerformanceDashboardViewModel.cs ← Dashboard completo

Dashboard/Views/Advanced/
├── EnsembleAnalysisWindow.xaml      ← Interface de análise
├── OptimizationControl.xaml         ← Controles de otimização
├── PerformanceCharts.xaml           ← Gráficos avançados
└── RegimeIndicator.xaml             ← Indicador de regime
```

---

## 🚀 **PREPARAÇÃO PARA FASE 4**

### **📈 Base para IA Avançada:**
- ✅ **Ensemble robusto** com >70% performance
- ✅ **Sistema de otimização** automática
- ✅ **Framework de análise** completo
- ✅ **Detecção de regimes** para adaptação
- ✅ **Pipeline de dados** estruturado para ML

### **🎯 Performance Target:**
- **Ensemble**: >70% accuracy
- **Volatilidade**: <3% desvio padrão
- **Sharpe Ratio**: >1.5
- **Maximum Drawdown**: <10%

**Status: 🔮 AGUARDANDO CONCLUSÃO DA FASE 2**