// D:\PROJETOS\GraphFacil\Library\Services\Analysis\CorrelationMatrix.cs - Comparador de performance entre modelos
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using LotoLibrary.Models.Prediction;

namespace LotoLibrary.Services.Analysis;

public class CorrelationMatrix
{
    private Dictionary<(string, string), double> _correlations = new();

    public async Task BuildMatrixAsync(List<string> modelNames, Dictionary<string, List<PredictionResult>> modelHistory)
    {
        _correlations.Clear();

        for (int i = 0; i < modelNames.Count; i++)
        {
            for (int j = i + 1; j < modelNames.Count; j++)
            {
                var model1 = modelNames[i];
                var model2 = modelNames[j];

                var correlation = await CalculateCorrelationBetweenModels(model1, model2, modelHistory);
                _correlations[(model1, model2)] = correlation;
                _correlations[(model2, model1)] = correlation; // Matriz simétrica
            }
        }
    }

    public double GetCorrelation(string model1, string model2)
    {
        if (model1 == model2) return 1.0;
        return _correlations.TryGetValue((model1, model2), out var correlation) ? correlation : 0.0;
    }

    public double GetAverageCorrelation(string model)
    {
        var correlations = _correlations
            .Where(kvp => kvp.Key.Item1 == model || kvp.Key.Item2 == model)
            .Where(kvp => kvp.Key.Item1 != kvp.Key.Item2) // Excluir auto-correlação
            .Select(kvp => Math.Abs(kvp.Value))
            .ToList();

        return correlations.Any() ? correlations.Average() : 0.0;
    }

    public List<ModelPair> GetAllPairs()
    {
        return _correlations
            .Where(kvp => string.Compare(kvp.Key.Item1, kvp.Key.Item2) < 0) // Evitar duplicatas
            .Select(kvp => new ModelPair
            {
                Model1 = kvp.Key.Item1,
                Model2 = kvp.Key.Item2,
                Correlation = kvp.Value
            })
            .ToList();
    }

    private async Task<double> CalculateCorrelationBetweenModels(
        string model1, string model2,
        Dictionary<string, List<PredictionResult>> modelHistory)
    {
        await Task.Delay(1); // Placeholder for async operation

        if (!modelHistory.ContainsKey(model1) || !modelHistory.ContainsKey(model2))
            return 0.0;

        var predictions1 = modelHistory[model1];
        var predictions2 = modelHistory[model2];

        var commonConcursos = predictions1
            .Select(p => p.TargetConcurso)
            .Intersect(predictions2.Select(p => p.TargetConcurso))
            .ToList();

        if (commonConcursos.Count < 10) return 0.0;

        var similarities = new List<double>();

        foreach (var concurso in commonConcursos)
        {
            var pred1 = predictions1.First(p => p.TargetConcurso == concurso);
            var pred2 = predictions2.First(p => p.TargetConcurso == concurso);

            var intersection = pred1.PredictedNumbers.Intersect(pred2.PredictedNumbers).Count();
            var union = pred1.PredictedNumbers.Union(pred2.PredictedNumbers).Count();

            var similarity = (double)intersection / union;
            similarities.Add(similarity);
        }

        return similarities.Average();
    }
}

