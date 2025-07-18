using LotoLibrary.Models.Core;
using LotoLibrary.Models.Validation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Engines
{
    public class MetronomoEngine
    {
        // Métodos sem modificadores explicitos (padrão é private)
        private List<PredictionValidationResult> ValidarMetronomo(Metronomo metronomo, Lances dados, int startIndex = 0)
        {
            var resultados = new List<PredictionValidationResult>();

            // Implementação do método...
            return resultados;
        }

        private PredictionValidationResult CriarPredictionValidationResult(int concursoId, List<int> predicao, Lance resultado)
        {
            var validationResult = new PredictionValidationResult(concursoId, predicao, resultado.Lista)
            {
                TipoEstrategia = "Metrônomo",
                Confidence = 0.75
            };

            return validationResult;
        }

        private void CalcularEstatisticas(List<PredictionValidationResult> resultados)
        {
            if (!resultados.Any()) return;

            var taxasAcerto = resultados.Select(r => r.TaxaAcerto).ToList();
            double mediaAcertos = taxasAcerto.Average();

            var acertos = resultados.Select(r => (double)r.Acertos).ToList();
            CalcularDesvio(acertos);

            var confiancas = resultados.Select(r => r.Confidence).ToList();
            double mediaConfianca = confiancas.Average();

            Console.WriteLine($"Média de acertos: {mediaAcertos:F2}");
            Console.WriteLine($"Média de confiança: {mediaConfianca:F2}");
        }

        private double CalcularDesvio(List<double> valores)
        {
            if (!valores.Any()) return 0.0;

            double media = valores.Average();
            double somaQuadrados = valores.Sum(x => Math.Pow(x - media, 2));
            return Math.Sqrt(somaQuadrados / valores.Count);
        }
    }
}
