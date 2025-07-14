using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models;
using LotoLibrary.Interfaces;
using LotoLibrary.Services;
using Dashboard.ViewModels;
using Dashboard.ViewModels.Specialized;

namespace CorrecaoErros
{
    public class ExemploUsoArquiteturaModular
    {
        public void DemonstrarUso()
        {
            // 1. Criar dados históricos
            var historico = new Lances
            {
                new Lance { Concurso = 3431, DezenasSorteadas = new[] { 1, 2, 3, 4, 5 } },
                new Lance { Concurso = 3432, DezenasSorteadas = new[] { 6, 7, 8, 9, 10 } }
            };

            // 2. Criar ViewModel de predição
            var predictionVM = new PredictionModelsViewModel(historico);

            // 3. Configurar modelo selecionado
            predictionVM.SelectedModel = predictionVM.AvailableModels.First();

            // 4. Executar predição
            predictionVM.GeneratePredictionCommand.Execute(null);

            // 5. Mostrar resultado
            Console.WriteLine($"Resultado: {predictionVM.LastPredictionResult}");
            Console.WriteLine($"Confiança: {predictionVM.SelectedModelConfidence:P2}");
        }
    }
}
