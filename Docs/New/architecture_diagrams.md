# 📊 DIAGRAMAS DE ARQUITETURA - SISTEMA LOTOFÁCIL

## 🏗️ **DIAGRAMA DE CLASSES PRINCIPAL**

```mermaid
classDiagram
    class IPredictionModel {
        <<interface>>
        +ModelName: string
        +ModelType: string
        +IsInitialized: bool
        +Confidence: double
        +InitializeAsync(Lances): Task~bool~
        +TrainAsync(Lances): Task~bool~
        +PredictAsync(int): Task~PredictionResult~
        +ValidateAsync(Lances): Task~ValidationResult~
        +Reset(): void
    }

    class IEnsembleModel {
        <<interface>>
        +SubModels: List~IPredictionModel~
        +ModelWeights: Dictionary~string,double~
        +AddModel(IPredictionModel, double): void
        +RemoveModel(string): void
        +UpdateWeights(Dictionary): void
    }

    class IMetaModel {
        <<interface>>
        +RegisterModel(IPredictionModel): void
        +DecideStrategyAsync(MarketConditions): Task~MetaDecision~
        +UpdatePerformanceHistory(string, double): void
    }

    class PredictionEngine {
        -models: Dictionary~string,IPredictionModel~
        -ensemble: IEnsembleModel
        -metaModel: IMetaModel
        -analyzer: IPerformanceAnalyzer
        +RegisterModel(IPredictionModel): void
        +GeneratePrediction(int): Task~PredictionResult~
        +OptimizeEnsemble(): Task~void~
    }

    class MetronomoModel {
        -individuais: List~MetronomoIndividual~
        +ModelName: "Metronomo"
        +PredictAsync(): Task~PredictionResult~
    }

    class OscillatorModel {
        -dezenas: List~DezenaSincronizada~
        -acoplamento: double
        +ModelName: "Oscillator"
        +AtualizarFases(): void
        +GetSincronizadas(): List~int~
    }

    class AntiFrequencyModel {
        -strategiaAtual: AntiFrequencyStrategy
        +ModelName: "AntiFrequency"
        +CalcularDivida(): Dictionary~int,double~
    }

    class EnsembleModel {
        -subModels: List~IPredictionModel~
        -weights: Dictionary~string,double~
        +CombinePredictions(): PredictionResult
        +OptimizeWeights(): void
    }

    class MetaLearningModel {
        -decisionTree: DecisionEngine
        -performanceHistory: Dictionary
        +DecideStrategy(): MetaDecision
        +AdaptWeights(): void
    }

    IPredictionModel <|-- MetronomoModel
    IPredictionModel <|-- OscillatorModel
    IPredictionModel <|-- AntiFrequencyModel
    IEnsembleModel <|-- EnsembleModel
    IMetaModel <|-- MetaLearningModel
    IPredictionModel <|-- EnsembleModel
    IPredictionModel <|-- MetaLearningModel

    PredictionEngine --> IPredictionModel
    PredictionEngine --> IEnsembleModel
    PredictionEngine --> IMetaModel
    EnsembleModel --> IPredictionModel
    MetaLearningModel --> IPredictionModel
```

## 🔄 **DIAGRAMA DE SEQUÊNCIA - GERAÇÃO DE PALPITE**

```mermaid
sequenceDiagram
    participant UI as MainWindow
    participant PE as PredictionEngine
    participant MM as MetaModel
    participant EM as EnsembleModel
    participant M1 as MetronomoModel
    participant M2 as OscillatorModel
    participant M3 as AntiFrequencyModel
    participant PA as PerformanceAnalyzer

    UI->>PE: GeneratePrediction(concurso)
    
    PE->>MM: DecideStrategyAsync(conditions)
    MM->>PA: GetRecentPerformances()
    PA-->>MM: performances
    MM->>MM: AnalyzeMarketConditions()
    MM-->>PE: MetaDecision(weights)
    
    PE->>EM: UpdateWeights(weights)
    PE->>EM: PredictAsync(concurso)
    
    par Parallel Model Execution
        EM->>M1: PredictAsync(concurso)
        M1->>M1: ProcessMetronomos()
        M1-->>EM: Result1
    and
        EM->>M2: PredictAsync(concurso)
        M2->>M2: SincronizarFases()
        M2-->>EM: Result2
    and
        EM->>M3: PredictAsync(concurso)
        M3->>M3: CalcularDivida()
        M3-->>EM: Result3
    end
    
    EM->>EM: CombinePredictions()
    EM-->>PE: FinalPrediction
    
    PE->>PA: LogPrediction(result)
    PE-->>UI: PredictionResult
```

## 🏭 **DIAGRAMA DE COMPONENTES**

```mermaid
graph TB
    subgraph "UI Layer"
        MW[MainWindow]
        VM[ViewModel]
    end
    
    subgraph "Engine Layer"
        PE[PredictionEngine]
        MF[ModelFactory]
        PA[PerformanceAnalyzer]
    end
    
    subgraph "Model Layer"
        subgraph "Individual Models"
            MM[MetronomoModel]
            OM[OscillatorModel]
            AFM[AntiFrequencyModel]
            GNN[GraphNNModel]
            AE[AutoencoderModel]
            RL[RLModel]
        end
        
        subgraph "Composite Models"
            EM[EnsembleModel]
            MLM[MetaLearningModel]
        end
    end
    
    subgraph "Data Layer"
        DS[DataService]
        LS[LotofacilScraper]
        FR[FileRepository]
    end
    
    subgraph "Infrastructure"
        LOG[Logger]
        CONF[Configuration]
        CACHE[Cache]
    end

    MW --> VM
    VM --> PE
    PE --> MF
    PE --> PA
    PE --> EM
    PE --> MLM
    
    MF --> MM
    MF --> OM
    MF --> AFM
    MF --> GNN
    MF --> AE
    MF --> RL
    
    EM --> MM
    EM --> OM
    EM --> AFM
    
    MLM --> EM
    MLM --> PA
    
    MM --> DS
    OM --> DS
    AFM --> DS
    
    DS --> LS
    DS --> FR
    
    PE --> LOG
    PE --> CONF
    PE --> CACHE
```

## 📊 **DIAGRAMA DE DEPENDÊNCIAS**

```mermaid
graph LR
    subgraph "Core Interfaces"
        IP[IPredictionModel]
        IE[IEnsembleModel]
        IM[IMetaModel]
        IC[IConfigurableModel]
    end
    
    subgraph "Anti-Frequency Models"
        AF1[AntiFrequencySimple]
        AF2[StatisticalDebtModel]
        AF3[SaturationModel] 
        AF4[PendularOscillator]
    end
    
    subgraph "Advanced Models"
        GNN[GraphNeuralNetwork]
        AE[Autoencoder]
        RL[ReinforcementLearning]
    end
    
    subgraph "Existing Models"
        MM[MetronomoModel]
        OM[OscillatorModel]
        MLN[MLNetModel]
    end
    
    subgraph "Composite"
        EM[EnsembleModel]
        META[MetaModel]
    end

    IP -.-> AF1
    IP -.-> AF2
    IP -.-> AF3
    IP -.-> AF4
    IP -.-> GNN
    IP -.-> AE
    IP -.-> RL
    IP -.-> MM
    IP -.-> OM
    IP -.-> MLN
    
    IE -.-> EM
    IM -.-> META
    IC -.-> AF1
    IC -.-> AF2
    IC -.-> GNN
    
    EM --> IP
    META --> IP
    META --> IE
```

## 🔄 **FLUXO DE DADOS TEMPORAL**

```mermaid
graph TD
    A[Dados Históricos] --> B[Preprocessamento]
    B --> C[Divisão Train/Validation]
    
    C --> D1[Treinamento Individual]
    C --> D2[Treinamento Ensemble]
    C --> D3[Treinamento Meta-Modelo]
    
    D1 --> E1[Modelo Metrônomo]
    D1 --> E2[Modelo Oscilador]
    D1 --> E3[Anti-Frequencista]
    D1 --> E4[Graph NN]
    D1 --> E5[Autoencoder]
    D1 --> E6[RL Model]
    
    E1 --> F[Ensemble Weights]
    E2 --> F
    E3 --> F
    E4 --> F
    E5 --> F
    E6 --> F
    
    D2 --> F
    F --> G[Ensemble Model]
    
    D3 --> H[Meta-Decision Engine]
    G --> H
    
    H --> I[Final Prediction]
    I --> J[Performance Feedback]
    J --> K[Weight Updates]
    K --> F
```
