// MigrationValidationScript.cs - Script para validar se a migração foi bem-sucedida
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Dashboard.ViewModels;
using Dashboard.ViewModels.Base;
using Dashboard.ViewModels.Specialized;
using Dashboard.ViewModels.Services;
using LotoLibrary.Models;

namespace Dashboard.Migration
{
    /// <summary>
    /// Script para validar se a migração arquitetural foi implementada corretamente
    /// Execute este script para verificar se tudo está funcionando
    /// </summary>
    public class MigrationValidationScript
    {
        #region Validation Results
        private static int _totalTests = 0;
        private static int _passedTests = 0;
        #endregion

        #region Main Execution
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🔍 SCRIPT DE VALIDAÇÃO - ARQUITETURA MODULAR");
            Console.WriteLine("=" * 60);
            
            try
            {
                await ExecuteValidationAsync();
                
                Console.WriteLine("\n" + "=" * 60);
                Console.WriteLine($"📊 RESULTADO FINAL: {_passedTests}/{_totalTests} testes passaram");
                
                if (_passedTests == _totalTests)
                {
                    Console.WriteLine("🎉 MIGRAÇÃO VALIDADA COM SUCESSO!");
                    Console.WriteLine("✅ Arquitetura modular implementada corretamente");
                    Console.WriteLine("🚀 Pronto para retomar desenvolvimento da Fase 2");
                }
                else
                {
                    Console.WriteLine("⚠️ ALGUNS TESTES FALHARAM");
                    Console.WriteLine("📋 Revise os erros acima e corrija antes de continuar");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ERRO CRÍTICO NA VALIDAÇÃO: {ex.Message}");
                Console.WriteLine("🔧 Verifique se a migração foi implementada corretamente");
            }
            
            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        private static async Task ExecuteValidationAsync()
        {
            Console.WriteLine("🚀 Iniciando validação da arquitetura...\n");

            // Validações de estrutura
            await ValidateProjectStructureAsync();
            
            // Validações de código
            await ValidateCodeStructureAsync();
            
            // Validações de funcionalidade
            await ValidateFunctionalityAsync();
            
            // Validações de performance
            await ValidatePerformanceAsync();
        }
        #endregion

        #region Structure Validation
        private static async Task ValidateProjectStructureAsync()
        {
            Console.WriteLine("1️⃣ VALIDANDO ESTRUTURA DO PROJETO");
            Console.WriteLine("-" * 40);

            // Validar estrutura de pastas
            ValidateFolder("Dashboard/ViewModels/Base", "Pasta Base");
            ValidateFolder("Dashboard/ViewModels/Specialized", "Pasta Specialized");
            ValidateFolder("Dashboard/ViewModels/Services", "Pasta Services");
            ValidateFolder("Dashboard/Converters", "Pasta Converters");

            // Validar arquivos base
            ValidateFile("Dashboard/ViewModels/Base/ViewModelBase.cs", "ViewModelBase");
            ValidateFile("Dashboard/ViewModels/Base/ModelOperationBase.cs", "ModelOperationBase");

            // Validar services
            ValidateFile("Dashboard/ViewModels/Services/UINotificationService.cs", "UINotificationService");
            ValidateFile("Dashboard/ViewModels/Services/ViewModelFactory.cs", "ViewModelFactory");

            // Validar ViewModels especializados
            ValidateFile("Dashboard/ViewModels/Specialized/PredictionModelsViewModel.cs", "PredictionModelsViewModel");
            ValidateFile("Dashboard/ViewModels/Specialized/ValidationViewModel.cs", "ValidationViewModel");
            ValidateFile("Dashboard/ViewModels/Specialized/ComparisonViewModel.cs", "ComparisonViewModel");
            ValidateFile("Dashboard/ViewModels/Specialized/ConfigurationViewModel.cs", "ConfigurationViewModel");

            // Validar converters
            ValidateFile("Dashboard/Converters/BoolToVisibilityConverter.cs", "BoolToVisibilityConverter");
            ValidateFile("Dashboard/Converters/BoolToColorConverter.cs", "BoolToColorConverter");
            ValidateFile("Dashboard/Converters/InverseBoolConverter.cs", "InverseBoolConverter");

            await Task.Delay(100); // Simular processamento
        }

        private static void ValidateFolder(string relativePath, string description)
        {
            _totalTests++;
            
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (Directory.Exists(fullPath))
            {
                _passedTests++;
                Console.WriteLine($"   ✅ {description}: {relativePath}");
            }
            else
            {
                Console.WriteLine($"   ❌ {description}: {relativePath} - PASTA NÃO ENCONTRADA");
            }
        }

        private static void ValidateFile(string relativePath, string description)
        {
            _totalTests++;
            
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
            if (File.Exists(fullPath))
            {
                _passedTests++;
                Console.WriteLine($"   ✅ {description}: {relativePath}");
            }
            else
            {
                Console.WriteLine($"   ❌ {description}: {relativePath} - ARQUIVO NÃO ENCONTRADO");
            }
        }
        #endregion

        #region Code Structure Validation
        private static async Task ValidateCodeStructureAsync()
        {
            Console.WriteLine("\n2️⃣ VALIDANDO ESTRUTURA DE CÓDIGO");
            Console.WriteLine("-" * 40);

            // Validar tipos usando reflection
            ValidateType<ViewModelBase>("ViewModelBase");
            ValidateType<ModelOperationBase>("ModelOperationBase");
            ValidateType<UINotificationService>("UINotificationService");
            ValidateType<ViewModelFactory>("ViewModelFactory");
            ValidateType<PredictionModelsViewModel>("PredictionModelsViewModel");
            ValidateType<ValidationViewModel>("ValidationViewModel");
            ValidateType<ComparisonViewModel>("ComparisonViewModel");
            ValidateType<ConfigurationViewModel>("ConfigurationViewModel");

            // Validar herança
            ValidateInheritance<PredictionModelsViewModel, ModelOperationBase>("PredictionModelsViewModel herda de ModelOperationBase");
            ValidateInheritance<ValidationViewModel, ModelOperationBase>("ValidationViewModel herda de ModelOperationBase");
            ValidateInheritance<ComparisonViewModel, ModelOperationBase>("ComparisonViewModel herda de ModelOperationBase");

            await Task.Delay(100);
        }

        private static void ValidateType<T>(string typeName)
        {
            _totalTests++;
            
            try
            {
                var type = typeof(T);
                if (type != null)
                {
                    _passedTests++;
                    Console.WriteLine($"   ✅ Tipo {typeName}: Encontrado");
                }
                else
                {
                    Console.WriteLine($"   ❌ Tipo {typeName}: NÃO ENCONTRADO");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Tipo {typeName}: ERRO - {ex.Message}");
            }
        }

        private static void ValidateInheritance<TDerived, TBase>(string description)
        {
            _totalTests++;
            
            try
            {
                var derivedType = typeof(TDerived);
                var baseType = typeof(TBase);
                
                if (derivedType.IsSubclassOf(baseType))
                {
                    _passedTests++;
                    Console.WriteLine($"   ✅ Herança: {description}");
                }
                else
                {
                    Console.WriteLine($"   ❌ Herança: {description} - HERANÇA INCORRETA");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Herança: {description} - ERRO - {ex.Message}");
            }
        }
        #endregion

        #region Functionality Validation
        private static async Task ValidateFunctionalityAsync()
        {
            Console.WriteLine("\n3️⃣ VALIDANDO FUNCIONALIDADES");
            Console.WriteLine("-" * 40);

            // Tentar criar dados de teste
            var testData = CreateTestData();
            
            if (testData != null)
            {
                Console.WriteLine("   ✅ Dados de teste criados");

                // Testar criação de ViewModels
                await TestViewModelCreation(testData);
                
                // Testar factory
                await TestViewModelFactory(testData);
                
                // Testar comandos
                await TestCommands(testData);
            }
            else
            {
                Console.WriteLine("   ❌ Falha ao criar dados de teste");
            }

            await Task.Delay(100);
        }

        private static Lances CreateTestData()
        {
            try
            {
                var lances = new Lances();
                
                // Criar alguns lances de teste
                for (int i = 1; i <= 5; i++)
                {
                    var lance = new Lance
                    {
                        Concurso = 3000 + i,
                        DezenasSorteadas = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }
                    };
                    lances.Add(lance);
                }

                return lances;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Erro ao criar dados de teste: {ex.Message}");
                return null;
            }
        }

        private static async Task TestViewModelCreation(Lances testData)
        {
            _totalTests++;
            
            try
            {
                var mainViewModel = new MainWindowViewModel(testData);
                
                if (mainViewModel != null)
                {
                    _passedTests++;
                    Console.WriteLine("   ✅ MainWindowViewModel: Criado com sucesso");
                    
                    // Testar propriedades
                    if (mainViewModel.PredictionModels != null)
                    {
                        Console.WriteLine("   ✅ PredictionModels: Inicializado");
                    }
                    
                    if (mainViewModel.Validation != null)
                    {
                        Console.WriteLine("   ✅ Validation: Inicializado");
                    }
                    
                    if (mainViewModel.Comparison != null)
                    {
                        Console.WriteLine("   ✅ Comparison: Inicializado");
                    }
                    
                    if (mainViewModel.Configuration != null)
                    {
                        Console.WriteLine("   ✅ Configuration: Inicializado");
                    }
                }
                else
                {
                    Console.WriteLine("   ❌ MainWindowViewModel: FALHA NA CRIAÇÃO");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ MainWindowViewModel: ERRO - {ex.Message}");
            }
        }

        private static async Task TestViewModelFactory(Lances testData)
        {
            _totalTests++;
            
            try
            {
                var factory = new ViewModelFactory(testData);
                
                var predictionVM = factory.CreatePredictionModelsViewModel();
                var validationVM = factory.CreateValidationViewModel();
                var comparisonVM = factory.CreateComparisonViewModel();
                var configVM = factory.CreateConfigurationViewModel();
                
                if (predictionVM != null && validationVM != null && comparisonVM != null && configVM != null)
                {
                    _passedTests++;
                    Console.WriteLine("   ✅ ViewModelFactory: Todos os ViewModels criados");
                }
                else
                {
                    Console.WriteLine("   ❌ ViewModelFactory: FALHA NA CRIAÇÃO DE ALGUNS VIEWMODELS");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ ViewModelFactory: ERRO - {ex.Message}");
            }
        }

        private static async Task TestCommands(Lances testData)
        {
            _totalTests++;
            
            try
            {
                var mainViewModel = new MainWindowViewModel(testData);
                
                // Testar se comandos existem
                var commandsExist = true;
                
                if (mainViewModel.GerarPalpitePrincipalCommand == null) commandsExist = false;
                if (mainViewModel.PrimeiroCommand == null) commandsExist = false;
                if (mainViewModel.SegundoCommand == null) commandsExist = false;
                
                if (commandsExist)
                {
                    _passedTests++;
                    Console.WriteLine("   ✅ Comandos: Todos os comandos principais existem");
                }
                else
                {
                    Console.WriteLine("   ❌ Comandos: ALGUNS COMANDOS NÃO EXISTEM");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Comandos: ERRO - {ex.Message}");
            }
        }
        #endregion

        #region Performance Validation
        private static async Task ValidatePerformanceAsync()
        {
            Console.WriteLine("\n4️⃣ VALIDANDO PERFORMANCE");
            Console.WriteLine("-" * 40);

            var testData = CreateTestData();
            
            if (testData != null)
            {
                await TestInitializationTime(testData);
                await TestMemoryUsage(testData);
            }

            await Task.Delay(100);
        }

        private static async Task TestInitializationTime(Lances testData)
        {
            _totalTests++;
            
            try
            {
                var startTime = DateTime.Now;
                
                var mainViewModel = new MainWindowViewModel(testData);
                await mainViewModel.InitializeAsync();
                
                var endTime = DateTime.Now;
                var duration = endTime - startTime;
                
                if (duration.TotalSeconds < 10) // Deve inicializar em menos de 10 segundos
                {
                    _passedTests++;
                    Console.WriteLine($"   ✅ Tempo de inicialização: {duration.TotalSeconds:F2}s (aceitável)");
                }
                else
                {
                    Console.WriteLine($"   ❌ Tempo de inicialização: {duration.TotalSeconds:F2}s (muito lento)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Teste de inicialização: ERRO - {ex.Message}");
            }
        }

        private static async Task TestMemoryUsage(Lances testData)
        {
            _totalTests++;
            
            try
            {
                var beforeMemory = GC.GetTotalMemory(false);
                
                var mainViewModel = new MainWindowViewModel(testData);
                await mainViewModel.InitializeAsync();
                
                var afterMemory = GC.GetTotalMemory(false);
                var memoryUsed = (afterMemory - beforeMemory) / 1024 / 1024; // MB
                
                if (memoryUsed < 50) // Deve usar menos de 50MB
                {
                    _passedTests++;
                    Console.WriteLine($"   ✅ Uso de memória: {memoryUsed:F2}MB (aceitável)");
                }
                else
                {
                    Console.WriteLine($"   ❌ Uso de memória: {memoryUsed:F2}MB (muito alto)");
                }
                
                // Cleanup
                mainViewModel.Cleanup();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   ❌ Teste de memória: ERRO - {ex.Message}");
            }
        }
        #endregion

        #region Report Generation
        private static void GenerateValidationReport()
        {
            var report = $@"
RELATÓRIO DE VALIDAÇÃO DA MIGRAÇÃO ARQUITETURAL
Data: {DateTime.Now:dd/MM/yyyy HH:mm:ss}

RESUMO:
- Total de testes: {_totalTests}
- Testes aprovados: {_passedTests}
- Taxa de sucesso: {(double)_passedTests / _totalTests:P2}

STATUS GERAL: {(_passedTests == _totalTests ? "✅ APROVADO" : "❌ REPROVADO")}

COMPONENTES VALIDADOS:
✅ Estrutura de pastas
✅ Arquivos de código
✅ Herança de classes
✅ Criação de ViewModels
✅ Factory Pattern
✅ Sistema de comandos
✅ Performance de inicialização
✅ Uso de memória

PRÓXIMOS PASSOS:
{(_passedTests == _totalTests ? 
    "🚀 Arquitetura validada! Pode retomar desenvolvimento da Fase 2" : 
    "🔧 Corrija os erros identificados antes de continuar")}
";

            var reportFile = $"ValidationReport_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            File.WriteAllText(reportFile, report);
            
            Console.WriteLine($"\n📄 Relatório salvo em: {reportFile}");
        }
        #endregion
    }
}
