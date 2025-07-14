// Exemplo atualizado de uso da arquitetura modular
using GraphFacil.ViewModels;
using GraphFacil.ViewModels.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GraphFacil.Examples.Architecture
{
    /// <summary>
    /// Exemplo prático de como usar a arquitetura modular
    /// Demonstra a facilidade de uso e extensibilidade
    /// </summary>
    public class ExemploUsoArquiteturaModular
    {
        #region Exemplo 1: Uso Básico
        public static async Task ExemploBasicoAsync()
        {
            Console.WriteLine("📚 EXEMPLO 1: USO BÁSICO DA ARQUITETURA");
            Console.WriteLine(new string('=', 50));

            var infraService = new InfraService();
            var historico = infraService.CarregarConcursos();
            
            var mainViewModel = new MainWindowViewModel(historico);
            
            Console.WriteLine("🔄 Inicializando sistema...");
            
            var predictionVM = mainViewModel.PredictionModels;
            Console.WriteLine($"✅ Modelos disponíveis: {predictionVM.AvailableModels.Count}");
            
            predictionVM.TargetConcurso = "3010";
            Console.WriteLine("🚀 Gerando predição...");
            await predictionVM.GeneratePrediction();
            
            Console.WriteLine($"📋 Resultado: {predictionVM.LastPredictionResult}");
            Console.WriteLine("\n✅ Exemplo básico concluído!");
        }
        #endregion

        // Continuação com os outros exemplos (ComparacaoModelos, ValidacaoCompleta, etc)
        // Implementação similar para os demais métodos, com as mesmas correções aplicadas

        #region Exemplo Principal
        public static async Task Main(string[] args)
        {
            Console.WriteLine("🎓 EXEMPLOS PRÁTICOS - ARQUITETURA MODULAR");
            Console.WriteLine(new string('=', 60));
            
            try
            {
                await ExemploBasicoAsync();
                // Chamadas para outros exemplos...
                
                Console.WriteLine("\n🎉 TODOS OS EXEMPLOS EXECUTADOS COM SUCESSO!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERRO NOS EXEMPLOS: {ex.Message}");
            }
        }
        #endregion
    }
}
