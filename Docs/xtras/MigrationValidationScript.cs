using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models;
using LotoLibrary.Services;
using Dashboard.ViewModels;
using Dashboard.ViewModels.Specialized;

namespace CorrecaoErros
{
    public class MigrationValidationScript
    {
        public void Execute()
        {
            // 1. Carregar dados históricos
            var infra = new Infra();
            var historico = infra.CarregaEntradaW("dados_migracao.txt");

            // 2. Inicializar ViewModels
            var predictionVM = new PredictionModelsViewModel(historico);
            var validationVM = new ValidationViewModel(historico, UINotificationService.Instance);

            // 3. Validar cada modelo
            foreach (var model in predictionVM.AvailableModels)
            {
                predictionVM.SelectedModel = model;
                validationVM.ValidateAllModelsAsync().Wait();

                Console.WriteLine($"Modelo: {model.Name}");
                Console.WriteLine($"Status: {(validationVM.LastValidationResult.IsValid ? "VÁLIDO" : "INVÁLIDO")}");
                Console.WriteLine($"Acurácia: {validationVM.LastValidationResult.Accuracy:P2}");
                Console.WriteLine("----------------------");
            }

            // 4. Salvar relatório
            infra.SalvaSaidaW(validationVM.LastValidationResult.ToReportLines(), 
                $"relatorio_migracao_{DateTime.Now:yyyyMMdd}.txt");
        }
    }
}
