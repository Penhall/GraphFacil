# 👨‍💻 **GUIA DO DESENVOLVEDOR - SISTEMA LOTOFÁCIL**

## 🚀 **CONFIGURAÇÃO DO AMBIENTE**

### **📋 Pré-requisitos**
```
- .NET 8.0 SDK ou superior
- Visual Studio 2022 ou VS Code
- Git para controle de versão
- Conhecimento de C# e MVVM
```

### **🔧 Setup Inicial**
```bash
# Clone o repositório
git clone [repository-url]

# Restaurar pacotes
dotnet restore

# Build da solução
dotnet build

# Executar testes
dotnet test

# Executar aplicação
dotnet run --project Dashboard
```

---

## 🏗️ **ARQUITETURA PARA DESENVOLVEDORES**

### **📁 Estrutura de Pastas**
```
LotoFacil/
├── 📁 LotoLibrary/              ← Business Logic
│   ├── 📁 Interfaces/           ← Contratos
│   │   ├── IPredictionModel.cs
│   │   ├── IConfigurableModel.cs
│   │   └── IEnsembleModel.cs
│   ├── 📁 Models/               ← Entidades
│   │   ├── Base/
│   │   ├── Prediction/
│   │   └── Configuration/
│   ├── 📁 Engines/              ← Coordenação
│   │   ├── PredictionEngine.cs
│   │   └── ModelFactory.cs
│   ├── 📁 Services/             ← Serviços
│   │   ├── ValidationService.cs
│   │   └── DiagnosticService.cs
│   ├── 📁 PredictionModels/     ← Modelos de IA
│   │   ├── Base/
│   │   ├── Individual/
│   │   ├── AntiFrequency/
│   │   └── Ensemble/
│   └── 📁 Utilities/            ← Utilitários
│
├── 📁 Dashboard/                ← Interface
│   ├── 📁 ViewModels/           ← MVVM
│   │   ├── Base/
│   │   ├── Specialized/
│   │   └── MainWindowViewModel.cs
│   ├── 📁 Views/                ← UI
│   │   └── MainWindow.xaml
│   ├── 📁 Services/             ← Serviços UI
│   └── 📁 Converters/           ← Conversores
│
└── 📁 Tests/                    ← Testes
    ├── 📁 Unit/
    ├── 📁 Integration/
    └── 📁 Performance/
```

### **🔗 Dependências Principais**
```xml
<PackageReference Include="CommunityToolkit.Mvvm" Version="8.2.2" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="8.0.0" />
<PackageReference Include="System.Text.Json" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
```

---

## 🎯 **IMPLEMENTANDO UM NOVO MODELO**

### **📝 Template Básico**
```csharp
// Library/PredictionModels/Custom/MyCustomModel.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Models.Base;
using LotoLibrary.Models.Prediction;
using LotoLibrary.Models;

namespace LotoLibrary.PredictionModels.Custom
{
    /// <summary>
    /// Exemplo de modelo customizado
    /// </summary>
    public class MyCustomModel : PredictionModelBase
    {
        #region Properties
        public override string ModelName => "My Custom Model";
        public override string ModelType => "Custom";
        #endregion

        #region Abstract Methods Implementation
        protected override async Task<bool> DoInitializeAsync(Lances historicalData)
        {
            try
            {
                // Simular inicialização
                await Task.Delay(100);
                
                // Validar dados de entrada
                if (historicalData?.Any() != true)
                    return false;
                
                // Realizar inicialização específica
                // ... sua lógica aqui ...
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na inicialização: {ex.Message}");
                return false;
            }
        }

        protected override async Task<PredictionResult> DoPredictAsync(int concurso)
        {
            try
            {
                // Simular processamento
                await Task.Delay(50);
                
                // Implementar algoritmo de predição
                var predictedNumbers = GenerateMyPrediction(concurso);
                
                return new PredictionResult
                {
                    ModelName = ModelName,
                    TargetConcurso = concurso,
                    PredictedNumbers = predictedNumbers,
                    Confidence = CalculateConfidence(),
                    GeneratedAt = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro na predição: {ex.Message}", ex);
            }
        }

        protected override async Task<ValidationResult> DoValidateAsync(Lances testData)
        {
            try
            {
                await Task.Delay(30);
                
                // Implementar validação específica
                var accuracy = CalculateAccuracy(testData);
                
                return new ValidationResult
                {
                    IsValid = true,
                    Accuracy = accuracy,
                    Message = "Validação do modelo customizado concluída",
                    TotalTests = testData?.Count ?? 0
                };
            }
            catch (Exception ex)
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Accuracy = 0.0,
                    Message = $"Erro na validação: {ex.Message}"
                };
            }
        }
        #endregion

        #region Private Methods
        private List<int> GenerateMyPrediction(int concurso)
        {
            // Implementar seu algoritmo aqui
            var random = new Random(concurso);
            return Enumerable.Range(1, 25)
                .OrderBy(x => random.Next())
                .Take(15)
                .OrderBy(x => x)
                .ToList();
        }

        private double CalculateAccuracy(Lances testData)
        {
            // Implementar cálculo de accuracy
            return 0.65; // Placeholder
        }

        protected override double CalculateConfidence()
        {
            // Implementar cálculo de confiança
            return IsInitialized ? 0.65 : 0.0;
        }
        #endregion
    }
}
```

### **🔧 Registro do Modelo**
```csharp
// No ModelFactory ou na inicialização
public class ModelFactory : IModelFactory
{
    private readonly Dictionary<string, Type> _registeredModels = new();

    public void RegisterModel<T>() where T : IPredictionModel, new()
    {
        var modelType = typeof(T);
        var modelName = modelType.Name;
        _registeredModels[modelName] = modelType;
    }

    // Registrar o novo modelo
    public void RegisterCustomModels()
    {
        RegisterModel<MyCustomModel>();
        // Outros modelos...
    }
}
```

---

## 🎭 **IMPLEMENTANDO MODELOS AVANÇADOS**

### **🧮 Modelo Configurável**
```csharp
public class ConfigurableCustomModel : PredictionModelBase, IConfigurableModel
{
    #region IConfigurableModel Implementation
    public Dictionary<string, object> CurrentParameters { get; private set; }
    public Dictionary<string, object> DefaultParameters { get; private set; }

    public bool SetParameter(string name, object value)
    {
        if (DefaultParameters.ContainsKey(name))
        {
            CurrentParameters[name] = value;
            return true;
        }
        return false;
    }

    public bool ValidateParameters(Dictionary<string, object> parameters)
    {
        foreach (var param in parameters)
        {
            if (!DefaultParameters.ContainsKey(param.Key))
                return false;
                
            // Validar tipo e range
            if (param.Key == "WindowSize" && param.Value is int windowSize)
            {
                if (windowSize < 10 || windowSize > 1000)
                    return false;
            }
        }
        return true;
    }
    #endregion

    #region Constructor
    public ConfigurableCustomModel()
    {
        InitializeParameters();
    }

    private void InitializeParameters()
    {
        DefaultParameters = new Dictionary<string, object>
        {
            { "WindowSize", 100 },
            { "ThresholdValue", 0.5 },
            { "WeightFactor", 0.8 }
        };
        
        CurrentParameters = new Dictionary<string, object>(DefaultParameters);
    }
    #endregion
}
```

### **🎯 Modelo com Explicação**
```csharp
public class ExplainableCustomModel : PredictionModelBase, IExplainableModel
{
    public ModelExplanation ExplainPrediction(PredictionResult result)
    {
        return new ModelExplanation
        {
            ModelName = ModelName,
            Summary = "Predição baseada em algoritmo customizado",
            KeyFactors = new List<string>
            {
                $"Análise de janela: {GetParameter<int>("WindowSize")} concursos",
                $"Threshold aplicado: {GetParameter<double>("ThresholdValue")}",
                $"Peso temporal: {GetParameter<double>("WeightFactor")}"
            },
            TechnicalDetails = new List<string>
            {
                "Algoritmo: Custom Analysis",
                $"Confiança: {result.Confidence:P2}",
                $"Tempo de execução: {DateTime.Now - result.GeneratedAt}"
            },
            Confidence = result.Confidence
        };
    }
}
```

---

## 🎲 **TESTANDO MODELOS**

### **🧪 Testes Unitários**
```csharp
// Tests/Unit/MyCustomModelTests.cs
using Microsoft.VisualStudio.TestTools.UnitTesting;
using LotoLibrary.PredictionModels.Custom;
using LotoLibrary.Models;

[TestClass]
public class MyCustomModelTests
{
    private MyCustomModel _model;
    private Lances _testData;

    [TestInitialize]
    public void Setup()
    {
        _model = new MyCustomModel();
        _testData = CreateTestData();
    }

    [TestMethod]
    public async Task Initialize_WithValidData_ReturnsTrue()
    {
        // Arrange
        var validData = CreateValidTestData();

        // Act
        var result = await _model.InitializeAsync(validData);

        // Assert
        Assert.IsTrue(result);
        Assert.IsTrue(_model.IsInitialized);
    }

    [TestMethod]
    public async Task Predict_WhenInitialized_ReturnsValidResult()
    {
        // Arrange
        await _model.InitializeAsync(_testData);
        var concurso = 3001;

        // Act
        var result = await _model.PredictAsync(concurso);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(15, result.PredictedNumbers.Count);
        Assert.IsTrue(result.Confidence > 0);
        Assert.AreEqual(concurso, result.TargetConcurso);
    }

    [TestMethod]
    public async Task Validate_WithTestData_ReturnsValidationResult()
    {
        // Arrange
        await _model.InitializeAsync(_testData);

        // Act
        var result = await _model.ValidateAsync(_testData);

        // Assert
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsValid);
        Assert.IsTrue(result.Accuracy > 0);
    }

    private Lances CreateTestData()
    {
        // Criar dados de teste
        var lances = new Lances();
        for (int i = 1; i <= 100; i++)
        {
            lances.Add(new Lance
            {
                Id = i,
                Lista = GenerateRandomNumbers()
            });
        }
        return lances;
    }

    private List<int> GenerateRandomNumbers()
    {
        var random = new Random();
        return Enumerable.Range(1, 25)
            .OrderBy(x => random.Next())
            .Take(15)
            .OrderBy(x => x)
            .ToList();
    }
}
```

### **🔗 Testes de Integração**
```csharp
[TestClass]
public class ModelIntegrationTests
{
    [TestMethod]
    public async Task PredictionEngine_WithCustomModel_WorksCorrectly()
    {
        // Arrange
        var engine = new PredictionEngine();
        var customModel = new MyCustomModel();
        var testData = CreateTestData();

        // Act
        await engine.InitializeAsync(testData);
        var registrationResult = await engine.RegisterModelAsync("Custom", customModel);
        var predictionResult = await engine.GeneratePredictionAsync(3001);

        // Assert
        Assert.IsTrue(registrationResult);
        Assert.IsNotNull(predictionResult);
        Assert.AreEqual(15, predictionResult.PredictedNumbers.Count);
    }
}
```

---

## 🔧 **EXTENSÕES DE UI**

### **📊 ViewModel para Novo Modelo**
```csharp
// Dashboard/ViewModels/Specialized/CustomModelViewModel.cs
public partial class CustomModelViewModel : ViewModelBase
{
    [ObservableProperty]
    private MyCustomModel _customModel;

    [ObservableProperty]
    private string _modelStatus = "Não inicializado";

    [ObservableProperty]
    private double _modelConfidence;

    [RelayCommand]
    private async Task InitializeCustomModel()
    {
        try
        {
            IsLoading = true;
            ModelStatus = "Inicializando...";

            CustomModel = new MyCustomModel();
            var result = await CustomModel.InitializeAsync(_historicalData);

            if (result)
            {
                ModelStatus = "Inicializado com sucesso";
                ModelConfidence = CustomModel.Confidence;
            }
            else
            {
                ModelStatus = "Falha na inicialização";
            }
        }
        catch (Exception ex)
        {
            ModelStatus = $"Erro: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task GenerateCustomPrediction()
    {
        if (CustomModel?.IsInitialized != true)
        {
            await InitializeCustomModel();
        }

        try
        {
            IsLoading = true;
            var result = await CustomModel.PredictAsync(TargetConcurso);
            
            LastPredictionResult = string.Join(", ", result.PredictedNumbers);
            ModelConfidence = result.Confidence;
        }
        catch (Exception ex)
        {
            LastPredictionResult = $"Erro: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

### **🎨 Interface XAML**
```xml
<!-- Dashboard/Views/CustomModelView.xaml -->
<UserControl x:Class="Dashboard.Views.CustomModelView">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="Auto"/>
            <RowDefinition Height="*"/>
        </Grid.RowDefinitions>

        <!-- Status -->
        <TextBlock Grid.Row="0" Text="{Binding ModelStatus}" 
                   FontWeight="Bold" Margin="5"/>

        <!-- Controles -->
        <StackPanel Grid.Row="1" Orientation="Horizontal" Margin="5">
            <Button Content="Inicializar" 
                    Command="{Binding InitializeCustomModelCommand}"
                    IsEnabled="{Binding IsLoading, Converter={StaticResource InverseBoolConverter}}"/>
            
            <Button Content="Gerar Predição" 
                    Command="{Binding GenerateCustomPredictionCommand}"
                    IsEnabled="{Binding IsLoading, Converter={StaticResource InverseBoolConverter}}"
                    Margin="5,0"/>
        </StackPanel>

        <!-- Resultados -->
        <Grid Grid.Row="2" Margin="5">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="*"/>
            </Grid.RowDefinitions>

            <TextBlock Grid.Row="0" Text="Última Predição:" FontWeight="Bold"/>
            <TextBlock Grid.Row="1" Text="{Binding LastPredictionResult}" 
                       FontSize="16" Foreground="Blue" Margin="0,5"/>
            
            <TextBlock Grid.Row="2" 
                       Text="{Binding ModelConfidence, StringFormat='Confiança: {0:P2}'}"
                       FontSize="14" Foreground="Green"/>
        </Grid>
    </Grid>
</UserControl>
```

---

## 📊 **DEBUGGING E TROUBLESHOOTING**

### **🔍 Logging**
```csharp
// Usar logging estruturado
public class MyCustomModel : PredictionModelBase
{
    private readonly ILogger<MyCustomModel> _logger;

    public MyCustomModel(ILogger<MyCustomModel> logger = null)
    {
        _logger = logger;
    }

    protected override async Task<bool> DoInitializeAsync(Lances historicalData)
    {
        _logger?.LogInformation("Iniciando inicialização do modelo customizado");
        
        try
        {
            // Lógica de inicialização
            _logger?.LogInformation("Modelo inicializado com sucesso");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Erro na inicialização do modelo");
            return false;
        }
    }
}
```

### **🐛 Debugging Comum**
```csharp
// Problemas comuns e soluções
public class DebuggingHelpers
{
    // Verificar se modelo está inicializado
    public static void ValidateModelState(IPredictionModel model)
    {
        if (!model.IsInitialized)
            throw new InvalidOperationException("Modelo não inicializado");
        
        if (model.Confidence <= 0)
            throw new InvalidOperationException("Confiança inválida");
    }

    // Verificar dados de entrada
    public static void ValidateInputData(Lances data)
    {
        if (data?.Any() != true)
            throw new ArgumentException("Dados históricos inválidos");
        
        foreach (var lance in data)
        {
            if (lance.Lista?.Count != 15)
                throw new ArgumentException($"Lance {lance.Id} tem {lance.Lista?.Count} números, esperado 15");
        }
    }

    // Verificar resultado de predição
    public static void ValidatePredictionResult(PredictionResult result)
    {
        if (result == null)
            throw new ArgumentNullException(nameof(result));
        
        if (result.PredictedNumbers?.Count != 15)
            throw new ArgumentException("Predição deve ter exatamente 15 números");
        
        if (result.Confidence <= 0 || result.Confidence > 1)
            throw new ArgumentException("Confiança deve estar entre 0 e 1");
    }
}
```

---

## 🚀 **DEPLOYMENT E VERSIONAMENTO**

### **📦 Build e Deploy**
```bash
# Build para produção
dotnet build --configuration Release

# Publicar aplicação
dotnet publish --configuration Release --output ./publish

# Criar executável
dotnet publish --configuration Release --self-contained --runtime win-x64
```

### **🔄 Versionamento**
```csharp
// AssemblyInfo.cs
[assembly: AssemblyVersion("2.1.0.0")]
[assembly: AssemblyFileVersion("2.1.0.0")]
[assembly: AssemblyInformationalVersion("2.1.0-beta")]
```

---

## 🎊 **MELHORES PRÁTICAS**

### **✅ Código Limpo**
```csharp
// Boas práticas para modelos
public class BestPracticesModel : PredictionModelBase
{
    // 1. Usar constantes para valores mágicos
    private const int DEFAULT_WINDOW_SIZE = 100;
    private const double MIN_CONFIDENCE = 0.5;
    
    // 2. Validar entrada sempre
    protected override async Task<bool> DoInitializeAsync(Lances historicalData)
    {
        if (historicalData?.Any() != true)
        {
            throw new ArgumentException("Dados históricos são obrigatórios");
        }
        
        // Implementação...
    }
    
    // 3. Usar async/await adequadamente
    protected override async Task<PredictionResult> DoPredictAsync(int concurso)
    {
        // Não usar Task.Run() desnecessariamente
        await Task.Delay(1); // Apenas se necessário
        
        // Implementação síncrona é OK se rápida
        return GeneratePrediction(concurso);
    }
    
    // 4. Tratamento de erros específico
    private PredictionResult GeneratePrediction(int concurso)
    {
        try
        {
            // Lógica de predição
            return new PredictionResult { /* ... */ };
        }
        catch (ArgumentException ex)
        {
            // Erro de entrada
            throw new InvalidOperationException($"Erro nos parâmetros: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            // Erro geral
            throw new InvalidOperationException($"Erro interno: {ex.Message}", ex);
        }
    }
}
```

### **📋 Checklist de Qualidade**
```
□ Modelo herda de PredictionModelBase
□ Implementa métodos abstratos corretamente
□ Usa async/await adequadamente
□ Valida entrada de dados
□ Trata exceções apropriadamente
□ Tem testes unitários
□ Documentação XML nas classes públicas
□ Logging estruturado implementado
□ Performance adequada (< 3s)
□ Código limpo e legível
```

---

## 🎯 **CONCLUSÃO**

Este guia fornece tudo o que você precisa para:

- 🏗️ **Entender a arquitetura** do sistema
- 🎯 **Implementar novos modelos** facilmente
- 🧪 **Testar adequadamente** suas implementações
- 📊 **Integrar com a UI** existente
- 🔧 **Debuggar problemas** eficientemente

**Com essas informações, você pode contribuir efetivamente para o desenvolvimento do sistema! 🚀**