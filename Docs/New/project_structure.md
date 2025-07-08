# 📁 ESTRUTURA COMPLETA DO PROJETO

## 🏗️ **ORGANIZAÇÃO DE PASTAS**

```
LotoLibrary/
├── 📁 Interfaces/
│   ├── IPredictionModel.cs
│   ├── IEnsembleModel.cs
│   ├── IMetaModel.cs
│   ├── IConfigurableModel.cs
│   ├── IExplainableModel.cs
│   ├── IModelFactory.cs
│   └── IPerformanceAnalyzer.cs
│
├── 📁 Models/
│   ├── 📁 Core/
│   │   ├── Lance.cs
│   │   ├── Lances.cs
│   │   ├── Lotofacil.cs
│   │   └── PredictionModels.cs (novos modelos de dados)
│   │
│   ├── 📁 Prediction/
│   │   ├── PredictionResult.cs
│   │   ├── ValidationResult.cs
│   │   ├── ModelExplanation.cs
│   │   ├── EnsembleMetrics.cs
│   │   └── MetaDecision.cs
│   │
│   └── 📁 Configuration/
│       ├── ModelParameters.cs
│       ├── EnsembleConfiguration.cs
│       └── MetaModelSettings.cs
│
├── 📁 Engines/
│   ├── PredictionEngine.cs (novo coordenador principal)
│   ├── MetronomoEngine.cs (refatorado como IPredictionModel)
│   └── ModelFactory.cs
│
├── 📁 PredictionModels/
│   ├── 📁 Individual/
│   │   ├── MetronomoModel.cs
│   │   ├── OscillatorModel.cs
│   │   └── MLNetModel.cs (refatorado)
│   │
│   ├── 📁 AntiFrequency/
│   │   ├── AntiFrequencyBaseModel.cs
│   │   ├── AntiFrequencySimpleModel.cs
│   │   ├── StatisticalDebtModel.cs
│   │   ├── SaturationModel.cs
│   │   └── PendularOscillatorModel.cs
│   │
│   ├── 📁 Advanced/
│   │   ├── GraphNeuralNetworkModel.cs
│   │   ├── AutoencoderModel.cs
│   │   └── ReinforcementLearningModel.cs
│   │
│   ├── 📁 Composite/
│   │   ├── EnsembleModel.cs
│   │   └── MetaLearningModel.cs
│   │
│   └── 📁 Base/
│       ├── PredictionModelBase.cs
│       ├── ConfigurableModelBase.cs
│       └── ExplainableModelBase.cs
│
├── 📁 Services/
│   ├── 📁 Analysis/
│   │   ├── PerformanceAnalyzer.cs
│   │   ├── StatisticalAnalyzer.cs
│   │   └── PatternDetector.cs
│   │
│   ├── 📁 Optimization/
│   │   ├── HyperparameterOptimizer.cs
│   │   ├── WeightOptimizer.cs
│   │   └── CrossValidator.cs
│   │
│   ├── 📁 Data/
│   │   ├── DataPreprocessor.cs
│   │   ├── FeatureExtractor.cs
│   │   └── DataSplitter.cs
│   │
│   └── 📁 Utilities/
│       ├── ModelSerializer.cs
│       ├── CacheManager.cs
│       └── ConfigurationManager.cs
│
├── 📁 Algorithms/
│   ├── 📁 AntiFrequency/
│   │   ├── DebtCalculator.cs
│   │   ├── SaturationDetector.cs
│   │   ├── CycleAnalyzer.cs
│   │   └── ReversalPredictor.cs
│   │
│   ├── 📁 Oscillation/
│   │   ├── PhaseCalculator.cs
│   │   ├── SynchronizationEngine.cs
│   │   ├── FrequencyAnalyzer.cs
│   │   └── CouplingOptimizer.cs
│   │
│   ├── 📁 MachineLearning/
│   │   ├── FeatureEngineering.cs
│   │   ├── ModelTraining.cs
│   │   ├── CrossValidation.cs
│   │   └── HyperparameterTuning.cs
│   │
│   └── 📁 MetaLearning/
│       ├── StrategySelector.cs
│       ├── PerformancePredictor.cs
│       ├── AdaptiveWeighting.cs
│       └── RegimeDetector.cs
│
├── 📁 Repository/ (existente)
│   ├── LotofacilRepository.cs
│   └── SubgrupoRepository.cs
│
├── 📁 NeuralNetwork/ (expandido)
│   ├── 📁 Traditional/
│   │   ├── MLNetModel.cs
│   │   └── ModelConfiguration.cs
│   │
│   ├── 📁 Advanced/
│   │   ├── GraphNeuralNetwork.cs
│   │   ├── Autoencoder.cs
│   │   └── DeepQLearning.cs
│   │
│   └── 📁 Infrastructure/
│       ├── TensorFlowService.cs
│       ├── PyTorchInterface.cs
│       └── ModelConverter.cs
│
└── 📁 Constants/
    ├── ModelTypes.cs
    ├── PredictionConstants.cs
    └── AlgorithmParameters.cs

Dashboard/
├── 📁 ViewModels/
│   ├── MainWindowViewModel.cs (expandido)
│   ├── ModelComparisonViewModel.cs (novo)
│   ├── EnsembleConfigurationViewModel.cs (novo)
│   └── PerformanceAnalysisViewModel.cs (novo)
│
├── 📁 Views/
│   ├── MainWindow.xaml (expandido)
│   ├── ModelComparisonWindow.xaml (novo)
│   ├── EnsembleConfigurationDialog.xaml (novo)
│   ├── PerformanceChartWindow.xaml (novo)
│   └── ModelExplanationDialog.xaml (novo)
│
├── 📁 Controls/
│   ├── ModelPerformanceChart.xaml (novo)
│   ├── PredictionVisualization.xaml (novo)
│   ├── EnsembleWeightSliders.xaml (novo)
│   └── MetaModelDashboard.xaml (novo)
│
└── 📁 Services/
    ├── UINotificationService.cs
    ├── ChartDataService.cs
    └── ExportService.cs
```

## 🎯 **CLASSES CORE NECESSÁRIAS**

### **1. PredictionEngine.cs (Novo Coordenador Principal)**
```csharp
// Substitui parcialmente o MetronomoEngine atual
// Coordena todos os modelos e ensembles
// Implementa otimização automática
// Gerencia cache e performance
```

### **2. ModelFactory.cs**
```csharp
// Factory para criação de todos os tipos de modelo
// Configuração automática baseada em parâmetros
// Registro e descoberta dinâmica de modelos
// Validação de dependências
```

### **3. Modelos Anti-Frequencistas Específicos:**

#### **AntiFrequencySimpleModel.cs**
```csharp
// Implementa estratégia de "dezenas frias"
// Ranking inverso por frequência histórica
// Configurável por período de análise
// Threshold de "frio" ajustável
```

#### **StatisticalDebtModel.cs**
```csharp
// Calcula "dívida estatística" de cada dezena
// Baseado em expectativa vs. realidade
// Ponderação temporal (mais recente = mais peso)
// Normalização por desvio padrão
```

#### **SaturationModel.cs**
```csharp
// Detecta "saturação" de dezenas quentes
// Identifica reversões usando indicadores técnicos
// RSI adaptado para loteria
// Bandas de Bollinger para dezenas
```

#### **PendularOscillatorModel.cs**
```csharp
// Modelo de oscilação pendular
// Fases de aquecimento/resfriamento
// Sincronização entre dezenas "gêmeas"
// Momentum reverso
```

### **4. Modelos Avançados:**

#### **GraphNeuralNetworkModel.cs**
```csharp
// Dezenas como nós de grafo
// Relações de co-ocorrência como arestas
// Propagação de "influência" entre nós
// Embedding de dezenas em espaço latente
```

#### **AutoencoderModel.cs**
```csharp
// Compressão de padrões de sorteios
// Detecção de anomalias
// Reconstrução de sorteios "normais"
// Espaço latente para clustering
```

#### **ReinforcementLearningModel.cs**
```csharp
// Agente que aprende estratégia ótima
// Recompensa baseada em acertos
// Exploração vs. Exploração
// Q-Learning adaptado
```

## 🔄 **FLUXO DE IMPLEMENTAÇÃO SUGERIDO**

### **Fase 1: Refatoração da Base**
1. Criar interfaces principais
2. Refatorar MetronomoEngine para IPredictionModel
3. Implementar PredictionEngine
4. Criar ModelFactory básico

### **Fase 2: Anti-Frequencistas**
1. AntiFrequencySimpleModel
2. StatisticalDebtModel  
3. SaturationModel
4. PendularOscillatorModel

### **Fase 3: Ensemble Básico**
1. EnsembleModel simples
2. Weight optimization
3. Performance tracking
4. UI para comparação

### **Fase 4: Modelos Avançados**
1. GraphNeuralNetworkModel
2. AutoencoderModel
3. ReinforcementLearningModel

### **Fase 5: Meta-Learning**
1. MetaLearningModel
2. Strategy selection
3. Adaptive weighting
4. Market regime detection

### **Fase 6: Otimização e UI**
1. Hyperparameter optimization
2. Cross-validation temporal
3. UI avançada para análise
4. Export/import de modelos

## 📊 **MÉTRICAS DE SUCESSO**

### **Performance Individual:**
- Accuracy > 65% (vs. 60.8% atual)
- Precision/Recall balanceados
- Diversidade de estratégias

### **Ensemble Performance:**
- Accuracy > 70%
- Volatilidade reduzida
- Robustez em diferentes períodos

### **Meta-Model Performance:**
- Detecção correta de regimes
- Adaptação rápida a mudanças
- Otimização contínua de pesos

## 🎯 **PRÓXIMOS PASSOS RECOMENDADOS**

1. **Aprovação da arquitetura** proposta
2. **Priorização** das fases de implementação
3. **Definição** de métricas específicas
4. **Setup** do ambiente de desenvolvimento
5. **Implementação** iterativa com testes contínuos
