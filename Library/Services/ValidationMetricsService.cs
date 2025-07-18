using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models.Validation;

namespace LotoLibrary.Services
{
    public class ValidationMetricsService
    {
        // Métodos sem modificadores explicitos (padrão é private)
        private double CalcularPrecisao(List<PredictionValidationResult> resultados)
        {
            if (resultados == null || !resultados.Any())
                return 0.0;

            var totalAcertos = resultados.Sum(r => r.Acertos);
            var totalPossivel = resultados.Count * 15;
            return totalPossivel > 0 ? (double)totalAcertos / totalPossivel : 0.0;
        }

        private double CalcularRecall(List<PredictionValidationResult> resultados)
        {
            if (resultados == null || !resultados.Any())
                return 0.0;

            return resultados.Average(r => r.TaxaAcerto);
        }

        private double CalcularF1Score(List<PredictionValidationResult> resultados)
        {
            var precisao = CalcularPrecisao(resultados);
            var recall = CalcularRecall(resultados);
            return precisao + recall == 0 ? 0.0 : 2 * (precisao * recall) / (precisao + recall);
        }

        private double CalcularAcuracia(List<PredictionValidationResult> resultados)
        {
            if (resultados == null || !resultados.Any())
                return 0.0;
            
            return resultados.Average(r => r.TaxaAcerto);
        }

        public ValidationMetricsReport GerarRelatorioCompleto(List<PredictionValidationResult> resultados)
        {
            return new ValidationMetricsReport
            {
                TotalTestes = resultados?.Count ?? 0,
                Precisao = CalcularPrecisao(resultados),
                Recall = CalcularRecall(resultados),
                F1Score = CalcularF1Score(resultados),
                Acuracia = CalcularAcuracia(resultados),
                DataGeracao = DateTime.Now
            };
        }
    }

    public class ValidationMetricsReport
    {
        public int TotalTestes { get; set; }
        public double Precisao { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public double Acuracia { get; set; }
        public DateTime DataGeracao { get; set; }

        public override string ToString()
        {
            return $"Relatório de Métricas:\n" +
                   $"Total de Testes: {TotalTestes}\n" +
                   $"Precisão: {Precisao:P2}\n" +
                   $"Recall: {Recall:P2}\n" +
                   $"F1-Score: {F1Score:P2}\n" +
                   $"Acurácia: {Acuracia:P2}\n" +
                   $"Gerado em: {DataGeracao:dd/MM/yyyy HH:mm}";
        }
    }
}
