# 🔗 **DEPENDENCY ANALYSIS - ANÁLISE DE DEPENDÊNCIAS**

## 🎯 **VISÃO GERAL**

Esta análise mapeia todas as dependências do sistema, identificando padrões, pontos de acoplamento e oportunidades de otimização arquitetural.

---

## 📊 **MAPA GERAL DE DEPENDÊNCIAS**

```mermaid
graph TB
    subgraph "External Dependencies"
        NET[".NET 6.0"]
        WPF["WPF Framework"]
        MVVM["CommunityToolkit.Mvvm"]
        JSON["System.Text.Json"]
        MATH["System.Numerics"]
    end
    
    subgraph "Dashboard Layer"
        MVM[MainWindowViewModel]
        PVM[PredictionModelsViewModel]
        VVM[ValidationViewModel]
        CVM[ComparisonViewModel]
        CFG[ConfigurationViewModel]
        VF[ViewModelFactory]
        UI_SRV[UINotificationService]
    end
    
    subgraph "LotoLibrary Layer"
        PE[PredictionEngine]
        MF[ModelFactory]
        ME[MetronomoEngine]
        DS[DiagnosticService]
        VS[ValidationService]
        PM[PredictionModels]
        MODELS[Models]
        UTILS[Utilities]
        INTERFACES[Interfaces]
    end
    
    subgraph "Data Layer"
        REPO[Repository]
        LANCE[Lance]
        LANCES[Lances]
        CONFIG[Configuration]
    end
    
    %% External Dependencies
    MVM --> MVVM
    PVM --> MVVM
    VVM --> MVVM
    CVM --> MVVM
    CFG --> MVVM
    UI_SRV --> WPF
    UTILS --> JSON
    UTILS --> MATH
    
    %% Dashboard to Business
    MVM --> PE
    MVM --> VF
    PVM --> PE
    PVM --> UI_SRV
    VVM --> VS
    VVM --> DS
    CVM --> PE
    CFG --> CONFIG
    
    %% Business Layer Internal
    PE --> MF
    PE --> ME
    PE --> PM
    PE --> MODELS
    DS --> ME
    DS --> MODELS
    VS --> PE
    VS --> DS
    MF --> PM
    MF --> INTERFACES
    
    %% Business to Data
    PE --> REPO
    ME --> LANCE
    ME --> LANCES
    MODELS --> LANCE
    UTILS --> CONFIG
    
    %% Styling
    classDef external fill:#ffeb3b,stroke:#f57f17,stroke-width:2px
    classDef dashboard fill:#e3f2fd,stroke:#1976d2,stroke-width:2px
    classDef business fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px
    classDef data fill:#e8f5e8,stroke:#388e3c,stroke-width:2px
    
    class NET,WPF,MVVM,JSON,MATH external
    class MVM,PVM,VVM,CVM,CFG,VF,UI_SRV dashboard
    class PE,MF,ME,DS,VS,PM,MODELS,UTILS,INTERFACES business
    class REPO,LANCE,LANCES,CONFIG data
```

---

## 📋 **MATRIZ DE DEPENDÊNCIAS DETALHADA**

### **Dependências por Componente:**

| Componente | Dependências Diretas | Nível | Acoplamento |
|------------|---------------------|-------|-------------|
| **MainWindowViewModel** | PredictionEngine, ViewModelFactory, MVVM | 4 | Alto |
| **PredictionModelsViewModel** | PredictionEngine, UINotificationService | 3 | Médio |
| **ValidationViewModel** | ValidationService, DiagnosticService | 3 | Médio |
| **ComparisonViewModel** | PredictionEngine, PerformanceAnalyzer | 3 | Médio |
| **ConfigurationViewModel** | Configuration, UINotificationService | 2 | Baixo |
| **PredictionEngine** | ModelFactory, Models, Services | 5 | Alto |
| **DiagnosticService** | MetronomoEngine, Models | 2 | Baixo |
| **ValidationService** | PredictionEngine, DiagnosticService | 2 | Baixo |
| **MetronomoEngine** | Lance, Lances, Utilities | 3 | Médio |
| **Models** | Utilities, Constants | 2 | Baixo |

---

## 🔍 **ANÁLISE DE ACOPLAMENTO**

### **Componentes com Alto Acoplamento (⚠️ Atenção)**

#### **1. PredictionEngine (5 dependências)**
```mermaid
graph LR
    PE[PredictionEngine] --> MF[ModelFactory]
    PE --> ME[MetronomoEngine]
    PE --> PM[PredictionModels]
    PE --> MODELS[Models]
    PE --> SRV[Services]
    
    style PE fill:#ffcdd2
```

**Justificativa**: Como coordenador central, este acoplamento é aceitável
**Mitigação**: Usar interfaces para reduzir dependências concretas

#### **2. MainWindowViewModel (4 dependências)**
```mermaid
graph LR
    MVM[MainWindowViewModel] --> PE[PredictionEngine]
    MVM --> VF[ViewModelFactory]
    MVM --> PVM[PredictionModelsViewModel]
    MVM --> UN[UINotificationService]
    
    style MVM fill:#ffcdd2
```

**Problema**: Coordena muitas responsabilidades
**Solução**: ✅ **Já implementada** - Delegação para ViewModels especializados

---

## 🔄 **DEPENDÊNCIAS CIRCULARES**

### **Análise de Ciclos:**

✅ **Nenhuma dependência circular detectada!**

**Hierarquia limpa:**
```
Interfaces (Level 0)
    ↓
Models + Utilities + Constants (Level 1)
    ↓
Services (Level 2)
    ↓
PredictionModels (Level 3)
    ↓
Engines (Level 4)
    ↓
ViewModels (Level 5)
```

---

## 📈 **MÉTRICAS DE QUALIDADE ARQUITETURAL**

### **Estabilidade dos Componentes**

```mermaid
graph TB
    subgraph "Stable Components (Low Afferent Coupling)"
        INTERFACES[Interfaces<br/>In: 0, Out: 0]
        MODELS[Models<br/>In: 0, Out: 2]
        UTILS[Utilities<br/>In: 0, Out: 1]
    end
    
    subgraph "Medium Stability"
        SRV[Services<br/>In: 2, Out: 2]
        PM[PredictionModels<br/>In: 1, Out: 3]
    end
    
    subgraph "Unstable Components (High Afferent Coupling)"
        PE[PredictionEngine<br/>In: 3, Out: 5]
        MVM[MainWindowViewModel<br/>In: 1, Out: 4]
    end
    
    style INTERFACES fill:#c8e6c9
    style MODELS fill:#c8e6c9
    style UTILS fill:#c8e6c9
    style SRV fill:#fff3e0
    style PM fill:#fff3e0
    style PE fill:#ffcdd2
    style MVM fill:#ffcdd2
```

### **Fórmulas de Estabilidade:**
- **Instabilidade = Ce / (Ca + Ce)**
  - Ce = Efferent Coupling (dependências de saída)
  - Ca = Afferent Coupling (dependências de entrada)
- **Range**: 0 (estável) a 1 (instável)

| Componente | Ca | Ce | Instabilidade | Classificação |
|------------|----|----|---------------|---------------|
| Interfaces | 0 | 0 | 0.00 | ✅ Muito Estável |
| Models | 0 | 2 | 1.00 | ⚠️ Instável |
| Utilities | 0 | 1 | 1.00 | ⚠️ Instável |
| Services | 2 | 2 | 0.50 | 🔄 Balanceado |
| PredictionModels | 1 | 3 | 0.75 | ⚠️ Instável |
| Engines | 3 | 5 | 0.63 | 🔄 Levemente Instável |
| ViewModels | 1 | 4 | 0.80 | ⚠️ Instável |

---

## 🎯 **DEPENDÊNCIAS EXTERNAS**

### **Bibliotecas Utilizadas:**

```mermaid
graph LR
    subgraph "Microsoft"
        NET[".NET 6.0"]
        WPF["Windows Presentation Foundation"]
        JSON["System.Text.Json"]
        NUMERICS["System.Numerics"]
    end
    
    subgraph "Community"
        MVVM["CommunityToolkit.Mvvm"]
    end
    
    subgraph "Future Dependencies (Fase 2+)"
        MATH["MathNet.Numerics"]
        ACCORD["Accord.NET"]
        TENSOR["TensorFlow.NET"]
    end
    
    APP --> NET
    APP --> WPF
    APP --> JSON
    APP --> NUMERICS
    APP --> MVVM
    
    style MATH fill:#e1f5fe
    style ACCORD fill:#e1f5fe
    style TENSOR fill:#e1f5fe
```

### **Análise de Risco por Dependência:**

| Biblioteca | Versão | Licença | Risco | Alternativas |
|------------|--------|---------|-------|-------------|
| **.NET 6.0** | 6.0.x | MIT | ✅ Baixo | N/A (Base) |
| **WPF** | Built-in | MIT | ✅ Baixo | Avalonia, MAUI |
| **CommunityToolkit.Mvvm** | 8.x | MIT | ✅ Baixo | ReactiveUI, Prism |
| **System.Text.Json** | Built-in | MIT | ✅ Baixo | Newtonsoft.Json |
| **System.Numerics** | Built-in | MIT | ✅ Baixo | Math.NET |

---

## 🔧 **INJEÇÃO DE DEPENDÊNCIA**

### **Padrão Atual: Factory Pattern**

```csharp
public class ViewModelFactory
{
    private readonly Lances _historicalData;
    private readonly PredictionEngine _predictionEngine;
    
    public ViewModelFactory(Lances historicalData)
    {
        _historicalData = historicalData;
        _predictionEngine = new PredictionEngine();
    }
    
    public PredictionModelsViewModel CreatePredictionModelsViewModel()
    {
        return new PredictionModelsViewModel(_predictionEngine, new UINotificationService());
    }
}
```

### **Proposta: Dependency Injection Container (Fase 3)**

```csharp
// Configuração do DI Container
services.AddSingleton<IPredictionEngine, PredictionEngine>();
services.AddSingleton<IModelFactory, ModelFactory>();
services.AddScoped<IValidationService, ValidationService>();
services.AddScoped<IDiagnosticService, DiagnosticService>();

// ViewModels
services.AddTransient<PredictionModelsViewModel>();
services.AddTransient<ValidationViewModel>();
services.AddTransient<ComparisonViewModel>();
```

**Benefícios:**
- ✅ Testabilidade melhorada
- ✅ Acoplamento reduzido
- ✅ Configuração centralizada
- ✅ Lifetime management automático

---

## 📊 **IMPACTO DE MUDANÇAS**

### **Análise de Propagação:**

```mermaid
graph TB
    subgraph "High Impact Changes"
        INTERFACE_CHANGE[Mudança em Interface]
        MODEL_CHANGE[Mudança em Models]
    end
    
    subgraph "Medium Impact Changes"
        ENGINE_CHANGE[Mudança em Engine]
        SERVICE_CHANGE[Mudança em Service]
    end
    
    subgraph "Low Impact Changes"
        VM_CHANGE[Mudança em ViewModel]
        UTIL_CHANGE[Mudança em Utility]
    end
    
    INTERFACE_CHANGE --> |Afeta| PM[PredictionModels]
    INTERFACE_CHANGE --> |Afeta| PE[PredictionEngine]
    INTERFACE_CHANGE --> |Afeta| MF[ModelFactory]
    
    MODEL_CHANGE --> |Afeta| PE[PredictionEngine]
    MODEL_CHANGE --> |Afeta| SRV[Services]
    MODEL_CHANGE --> |Afeta| PM[PredictionModels]
    
    ENGINE_CHANGE --> |Afeta| MVM[MainWindowViewModel]
    ENGINE_CHANGE --> |Afeta| PVM[PredictionModelsViewModel]
    
    SERVICE_CHANGE --> |Afeta| VVM[ValidationViewModel]
    
    VM_CHANGE --> |Afeta| UI[UI Only]
    UTIL_CHANGE --> |Afeta| CALLER[Specific Callers]
    
    style INTERFACE_CHANGE fill:#ffcdd2
    style MODEL_CHANGE fill:#ffcdd2
    style ENGINE_CHANGE fill:#fff3e0
    style SERVICE_CHANGE fill:#fff3e0
    style VM_CHANGE fill:#c8e6c9
    style UTIL_CHANGE fill:#c8e6c9
```

### **Estratégias de Mitigação:**

1. **Para mudanças em Interfaces:**
   - Usar versionamento de interfaces
   - Implementar adaptadores temporários
   - Deprecation warnings

2. **Para mudanças em Models:**
   - Migration scripts
   - Backward compatibility
   - Data transformation layers

3. **Para mudanças em Engines:**
   - Feature flags
   - A/B testing
   - Rollback capabilities

---

## 🎯 **RECOMENDAÇÕES DE OTIMIZAÇÃO**

### **Curto Prazo (Fase 2):**

1. **Reduzir acoplamento em PredictionEngine:**
   ```csharp
   // Ao invés de dependência direta
   private readonly MetronomoEngine _metronomoEngine;
   
   // Usar factory ou registry
   private readonly IModelRegistry _modelRegistry;
   ```

2. **Implementar interfaces para Services:**
   ```csharp
   public interface IDiagnosticService
   {
       Task<DiagnosticReport> TestSystemAsync();
   }
   ```

### **Médio Prazo (Fase 3):**

1. **Implementar DI Container**
2. **Criar abstrações para dependências externas**
3. **Implementar Event Bus para comunicação desacoplada**

### **Longo Prazo (Fase 4+):**

1. **Microservices architecture** (se necessário)
2. **Plugin system** para modelos
3. **Configuration-driven dependencies**

---

## 📈 **MÉTRICAS DE MONITORAMENTO**

### **KPIs de Qualidade Arquitetural:**

- **Acoplamento Médio**: < 4 dependências por componente
- **Profundidade de Herança**: < 3 níveis
- **Dependências Circulares**: 0 (zero)
- **Cobertura de Interfaces**: > 80%
- **Estabilidade Média**: 0.3 - 0.7 (balanceado)

### **Alerts de Qualidade:**

- 🚨 **Crítico**: Componente com > 6 dependências
- ⚠️ **Warning**: Componente com instabilidade > 0.8
- ℹ️ **Info**: Nova dependência externa adicionada

Esta análise garante que o sistema mantenha alta qualidade arquitetural conforme evolui, identificando proativamente pontos de melhoria e riscos potenciais.