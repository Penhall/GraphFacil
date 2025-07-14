using System;
using System.Threading.Tasks;
using LotoLibrary.Models;
using LotoLibrary.Services;
using Dashboard.ViewModels;
using Dashboard.ViewModels.Specialized;

namespace CorrecaoErros
{
    public class ExemploUsoSistemaCompleto
    {
        public async Task ExecutarFluxoCompletoAsync()
        {
            // 1. Carregar dados históricos
            var infraService = new Infra();
            var historico = infraService.CarregaEntradaW("historico_lotofacil.txt");

            // 2. Inicializar ViewModels
            var predictionVM = new PredictionModelsViewModel(historico);
            var validationVM = new ValidationViewModel(historico, UINotificationService.Instance);

            // 3. Aguardar inicialização
            await predictionVM.InitializeAsync();
            await validationVM.InitializeAsync();

            // 4. Executar predição
            predictionVM.SelectedModel = predictionVM.AvailableModels.First();
            await predictionVM.GeneratePrediction();

            // 5. Validar modelos
            await validationVM.ValidateAllModelsAsync();

            // 6. Salvar resultados
            infraService.SalvaSaidaW(historico, "resultados_lotofacil.txt");
            Console.WriteLine("Processo concluído com sucesso!");
        }
    }
}
