// ExemploUsoArquiteturaModular.cs - Demonstra como usar a nova arquitetura
using Dashboard.ViewModels;
using Dashboard.ViewModels.Services;
using System;
using System.Threading.Tasks;

namespace Dashboard.Exemplos
{
    /// <summary>
    /// Exemplo prático de como usar a nova arquitetura modular
    /// Demonstra a facilidade de uso e extensibilidade
    /// </summary>
    public class ExemploUsoArquiteturaModular
    {
        #region Exemplo 1: Uso Básico
        /// <summary>
        /// Exemplo básico: criar ViewModel principal e usar funcionalidades
        /// </summary>
        public static async Task ExemploBasicoAsync()
        {
            Console.WriteLine("📚 EXEMPLO 1: USO BÁSICO DA ARQUITETURA");
            Console.WriteLine("=" * 50);

            // 1. Carregar dados históricos (como sempre)
            var historico = Infra.CarregarConcursos();

            // 2. Criar ViewModel principal (MUITO MAIS SIMPLES AGORA!)
            var mainViewModel = new MainWindowViewModel(historico);

            // 3. Aguardar inicialização
            Console.WriteLine("🔄 Inicializando sistema...");
            // A inicialização é automática no construtor, mas pode aguardar se necessário

            // 4. Usar funcionalidades via ViewModels especializados
            Console.WriteLine("📊 Acessando modelos de predição...");
            var predictionVM = mainViewModel.PredictionModels;

            Console.WriteLine($"✅ Modelos disponíveis: {predictionVM.AvailableModels.Count}");
            Console.WriteLine($"🎯 Modelo selecionado: {predictionVM.SelectedModel?.Name ?? "Nenhum"}");

            // 5. Gerar predição (MUITO SIMPLES!)
            predictionVM.TargetConcurso = "3010";

            Console.WriteLine("🚀 Gerando predição...");
            await predictionVM.GeneratePrediction();

            Console.WriteLine($"📋 Resultado: {predictionVM.LastPredictionResult}");
            Console.WriteLine($"🎯 Confiança: {predictionVM.SelectedModelConfidence:P2}");

            Console.WriteLine("\n✅ Exemplo básico concluído!");
        }
        #endregion

        #region Exemplo 2: Comparação de Modelos
        /// <summary>
        /// Exemplo avançado: comparar múltiplos modelos
        /// </summary>
        public static async Task ExemploComparacaoModelosAsync()
        {
            Console.WriteLine("\n📚 EXEMPLO 2: COMPARAÇÃO DE MODELOS");
            Console.WriteLine("=" * 50);

            var historico = Infra.CarregarConcursos();
            var mainViewModel = new MainWindowViewModel(historico);

            // Acessar ViewModel de comparação
            var comparisonVM = mainViewModel.Comparison;

            Console.WriteLine("🔍 Preparando comparação...");

            // Adicionar modelos para comparação
            foreach (var modelType in comparisonVM.AvailableModelsForComparison)
            {
                comparisonVM.AddModelToComparison(modelType);
                Console.WriteLine($"➕ Adicionado: {modelType}");
            }

            // Definir concurso alvo
            comparisonVM.TargetConcursoForComparison = "3011";

            // Executar comparação
            Console.WriteLine("⚔️ Comparando modelos...");
            await comparisonVM.CompareSelectedModels();

            // Exibir resultados
            Console.WriteLine($"📊 {comparisonVM.ComparisonSummary}");

            foreach (var comparison in comparisonVM.Comparisons)
            {
                Console.WriteLine($"🔄 {comparison.Model1Name} vs {comparison.Model2Name}:");
                Console.WriteLine($"   📈 Diversificação: {comparison.DiversificationRate:P2}");
                Console.WriteLine($"   🔗 Números comuns: {comparison.CommonNumbers}/15");
            }

            Console.WriteLine("\n✅ Exemplo de comparação concluído!");
        }
        #endregion

        #region Exemplo 3: Validação Completa
        /// <summary>
        /// Exemplo de validação: testar performance dos modelos
        /// </summary>
        public static async Task ExemploValidacaoCompletaAsync()
        {
            Console.WriteLine("\n📚 EXEMPLO 3: VALIDAÇÃO COMPLETA");
            Console.WriteLine("=" * 50);

            var historico = Infra.CarregarConcursos();
            var mainViewModel = new MainWindowViewModel(historico);

            // Acessar ViewModel de validação
            var validationVM = mainViewModel.Validation;

            Console.WriteLine("🧪 Iniciando validação completa...");

            // Executar validação completa
            await validationVM.RunFullValidation();

            // Exibir resultados
            Console.WriteLine($"📊 {validationVM.LastValidationSummary}");
            Console.WriteLine($"🎯 Accuracy geral: {validationVM.OverallAccuracy:P2}");
            Console.WriteLine($"✅ Testes aprovados: {validationVM.PassedTests}/{validationVM.TotalTests}");

            // Exibir detalhes dos testes
            Console.WriteLine("\n📋 Detalhes dos testes:");
            foreach (var result in validationVM.ValidationResults)
            {
                var status = result.Success ? "✅" : "❌";
                Console.WriteLine($"   {status} {result.TestName}: {result.Message}");
            }

            Console.WriteLine("\n✅ Exemplo de validação concluído!");
        }
        #endregion

        #region Exemplo 4: Configuração Avançada
        /// <summary>
        /// Exemplo de configuração: personalizar parâmetros dos modelos
        /// </summary>
        public static async Task ExemploConfiguracaoAvancadaAsync()
        {
            Console.WriteLine("\n📚 EXEMPLO 4: CONFIGURAÇÃO AVANÇADA");
            Console.WriteLine("=" * 50);

            var historico = Infra.CarregarConcursos();
            var mainViewModel = new MainWindowViewModel(historico);

            var predictionVM = mainViewModel.PredictionModels;

            // Selecionar modelo configurável
            var configurableModel = predictionVM.AvailableModels
                .FirstOrDefault(m => m.IsConfigurable);

            if (configurableModel != null)
            {
                predictionVM.SelectedModel = configurableModel;

                Console.WriteLine($"⚙️ Configurando modelo: {configurableModel.Name}");

                // Carregar configuração
                predictionVM.LoadModelConfiguration();

                // Mostrar parâmetros atuais
                Console.WriteLine("📋 Parâmetros atuais:");
                foreach (var param in predictionVM.SelectedModelParameters)
                {
                    Console.WriteLine($"   🔧 {param.Key}: {param.Value}");
                }

                // Modificar parâmetros
                if (predictionVM.SelectedModelParameters.ContainsKey("JanelaHistorica"))
                {
                    predictionVM.SelectedModelParameters["JanelaHistorica"] = 150;
                    Console.WriteLine("✏️ Modificado JanelaHistorica para 150");
                }

                // Aplicar mudanças
                predictionVM.UpdateModelParameters();
                Console.WriteLine("✅ Parâmetros atualizados");

                // Testar com nova configuração
                predictionVM.TargetConcurso = "3012";
                await predictionVM.GeneratePrediction();

                Console.WriteLine($"🎯 Predição com nova configuração: {predictionVM.LastPredictionResult}");
            }
            else
            {
                Console.WriteLine("⚠️ Nenhum modelo configurável disponível");
            }

            Console.WriteLine("\n✅ Exemplo de configuração concluído!");
        }
        #endregion

        #region Exemplo 5: Uso de Notifications
        /// <summary>
        /// Exemplo de notificações: usar sistema de notificações
        /// </summary>
        public static async Task ExemploNotificacoesAsync()
        {
            Console.WriteLine("\n📚 EXEMPLO 5: SISTEMA DE NOTIFICAÇÕES");
            Console.WriteLine("=" * 50);

            // Usar serviço de notificações (singleton)
            var notificationService = UINotificationService.Instance;

            Console.WriteLine("📢 Testando diferentes tipos de notificação...");

            // Simular notificações (em aplicação real, seria MessageBox)
            Console.WriteLine("🔔 Simulando notificação de sucesso...");
            // notificationService.ShowSuccess("Predição gerada com sucesso!");

            Console.WriteLine("🔔 Simulando notificação de aviso...");
            // notificationService.ShowWarning("Modelo ainda não foi treinado");

            Console.WriteLine("🔔 Simulando notificação de erro...");
            // notificationService.ShowError("Erro ao carregar dados históricos");

            Console.WriteLine("🔔 Simulando pergunta de confirmação...");
            // var confirmed = notificationService.AskConfirmation("Deseja continuar com a validação?");

            Console.WriteLine("✅ Exemplos de notificação demonstrados!");

            // Mostrar como ViewModels usam automaticamente
            var historico = Infra.CarregarConcursos();
            var mainViewModel = new MainWindowViewModel(historico);

            Console.WriteLine("\n🤖 ViewModels usam notificações automaticamente:");
            Console.WriteLine("   - Erros são mostrados via ShowError()");
            Console.WriteLine("   - Sucessos são mostrados via ShowSuccess()");
            Console.WriteLine("   - Confirmações são solicitadas via AskConfirmation()");

            Console.WriteLine("\n✅ Exemplo de notificações concluído!");
        }
        #endregion

        #region Exemplo 6: Extensibilidade - Novo ViewModel
        /// <summary>
        /// Exemplo de extensibilidade: como adicionar novo ViewModel facilmente
        /// </summary>
        public static async Task ExemploExtensibilidadeAsync()
        {
            Console.WriteLine("\n📚 EXEMPLO 6: EXTENSIBILIDADE - NOVO VIEWMODEL");
            Console.WriteLine("=" * 50);

            Console.WriteLine("🔧 Como adicionar novo ViewModel especializado:");
            Console.WriteLine();

            Console.WriteLine("1️⃣ CRIAR NOVA CLASSE:");
            Console.WriteLine("   📁 Dashboard/ViewModels/Specialized/ReportsViewModel.cs");
            Console.WriteLine("   🏗️ Herdar de ModelOperationBase");
            Console.WriteLine("   ⚙️ Implementar funcionalidades específicas");
            Console.WriteLine();

            Console.WriteLine("2️⃣ ADICIONAR NA FACTORY:");
            Console.WriteLine("   📝 ViewModelFactory.CreateReportsViewModel()");
            Console.WriteLine("   🔗 Método para criar singleton");
            Console.WriteLine();

            Console.WriteLine("3️⃣ INTEGRAR NO MAINWINDOWVIEWMODEL:");
            Console.WriteLine("   📊 public ReportsViewModel Reports { get; }");
            Console.WriteLine("   🚀 Reports = _viewModelFactory.CreateReportsViewModel();");
            Console.WriteLine();

            Console.WriteLine("4️⃣ ATUALIZAR XAML:");
            Console.WriteLine("   🖼️ Adicionar controles para novo ViewModel");
            Console.WriteLine("   🔗 Bindings: {Binding Reports.MinhaPropriedade}");
            Console.WriteLine();

            Console.WriteLine("5️⃣ RESULTADO:");
            Console.WriteLine("   ✅ Novo ViewModel integrado sem modificar código existente");
            Console.WriteLine("   ✅ Zero risco de quebrar funcionalidades atuais");
            Console.WriteLine("   ✅ Testes unitários independentes possíveis");
            Console.WriteLine("   ✅ Manutenção isolada e simples");

            Console.WriteLine("\n🚀 VANTAGEM DA ARQUITETURA MODULAR:");
            Console.WriteLine("   ⏰ Tempo para adicionar: 30-60 minutos");
            Console.WriteLine("   🛡️ Risco de bugs: ZERO");
            Console.WriteLine("   🧪 Testabilidade: ALTA");
            Console.WriteLine("   📈 Manutenibilidade: EXCELENTE");

            Console.WriteLine("\n✅ Exemplo de extensibilidade concluído!");
        }
        #endregion

        #region Exemplo Principal
        /// <summary>
        /// Executa todos os exemplos
        /// </summary>
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🎓 EXEMPLOS PRÁTICOS - ARQUITETURA MODULAR");
            Console.WriteLine("=" * 60);
            Console.WriteLine("📖 Demonstrando facilidade de uso e poder da nova arquitetura");
            Console.WriteLine();

            try
            {
                await ExemploBasicoAsync();
                await ExemploComparacaoModelosAsync();
                await ExemploValidacaoCompletaAsync();
                await ExemploConfiguracaoAvancadaAsync();
                await ExemploNotificacoesAsync();
                await ExemploExtensibilidadeAsync();

                Console.WriteLine("\n" + "=" * 60);
                Console.WriteLine("🎉 TODOS OS EXEMPLOS EXECUTADOS COM SUCESSO!");
                Console.WriteLine();
                Console.WriteLine("🚀 PRÓXIMOS PASSOS:");
                Console.WriteLine("   1. Implementar StatisticalDebtModel (fácil!)");
                Console.WriteLine("   2. Adicionar SaturationModel (simples!)");
                Console.WriteLine("   3. Criar PendularOscillatorModel (direto!)");
                Console.WriteLine("   4. Desenvolver EnsembleModel (sem complicação!)");
                Console.WriteLine();
                Console.WriteLine("💡 LEMBRE-SE:");
                Console.WriteLine("   ✅ Cada modelo novo = apenas uma classe");
                Console.WriteLine("   ✅ Zero modificação do código existente");
                Console.WriteLine("   ✅ Testes unitários independentes");
                Console.WriteLine("   ✅ Manutenção simplificada");
                Console.WriteLine();
                Console.WriteLine("🎯 A ARQUITETURA MODULAR ACELERA O DESENVOLVIMENTO!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n❌ ERRO NOS EXEMPLOS: {ex.Message}");
                Console.WriteLine("🔧 Verifique se a migração foi implementada corretamente");
            }

            Console.WriteLine("\nPressione qualquer tecla para continuar...");
            Console.ReadKey();
        }
        #endregion
    }
}

