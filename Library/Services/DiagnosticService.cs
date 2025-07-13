using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models;
using LotoLibrary.Enums;

namespace LotoLibrary.Services.Auxiliar
{
    public static class DiagnosticService
    {
        public static DezenasDistributionReport AnalyzeDezenasDistribution(List<List<int>> dezenasLists)
        {
            if (dezenasLists == null || !dezenasLists.Any())
                return new DezenasDistributionReport();

            var flatDezenas = dezenasLists.SelectMany(x => x).ToList();
            
            return new DezenasDistributionReport
            {
                IsDistributionNormal = CheckNormalDistribution(flatDezenas),
                CountDezenas1a9 = flatDezenas.Count(d => d >= 1 && d <= 9),
                CountDezenas10a25 = flatDezenas.Count(d => d >= 10 && d <= 25),
                PercentualDezenas1a9 = flatDezenas.Any() ? 
                    (double)flatDezenas.Count(d => d >= 1 && d <= 9) / flatDezenas.Count : 0,
                PercentualDezenas10a25 = flatDezenas.Any() ? 
                    (double)flatDezenas.Count(d => d >= 10 && d <= 25) / flatDezenas.Count : 0,
                GravidadeProblema = CalculateProblemSeverity(flatDezenas)
            };
        }

        private static bool CheckNormalDistribution(List<int> dezenas)
        {
            var count1a9 = dezenas.Count(d => d >= 1 && d <= 9);
            var percent1a9 = (double)count1a9 / dezenas.Count;
            return percent1a9 >= 0.3 && percent1a9 <= 0.7;
        }

        private static GravidadeProblema CalculateProblemSeverity(List<int> dezenas)
        {
            var percent1a9 = (double)dezenas.Count(d => d >= 1 && d <= 9) / dezenas.Count;
            
            if (percent1a9 < 0.2 || percent1a9 > 0.8)
                return GravidadeProblema.Alta;
            if (percent1a9 < 0.25 || percent1a9 > 0.75)
                return GravidadeProblema.Media;
            return GravidadeProblema.Baixa;
        }
    }

    public class DezenasDistributionReport
    {
        public bool IsDistributionNormal { get; set; }
        public GravidadeProblema GravidadeProblema { get; set; } = GravidadeProblema.Baixa;
        public int CountDezenas1a9 { get; set; }
        public double PercentualDezenas1a9 { get; set; }
        public int CountDezenas10a25 { get; set; }
        public double PercentualDezenas10a25 { get; set; }
    }
}
