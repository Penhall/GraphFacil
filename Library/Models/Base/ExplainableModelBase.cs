using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Interfaces;
using LotoLibrary.Utilities;

namespace LotoLibrary.Models.Base;

public abstract class ExplainableModelBase : IExplainableModel
{
    public virtual bool SupportsDetailedExplanations => true;
    public virtual double ExplanationConfidence => 0.75;

    public virtual async Task<PredictionExplanation> ExplainPredictionAsync(int targetConcurso)
    {
        await Task.Delay(50);
        
        return new PredictionExplanation
        {
            Concurso = targetConcurso,
            ModelName = GetType().Name,
            MainReason = $"Predição baseada em análise para concurso {targetConcurso}",
            ConfidenceLevel = ExplanationConfidence
        };
    }

    public virtual List<string> GetKeyDecisionFactors()
    {
        return new List<string>
        {
            "Análise histórica",
            "Padrões detectados",
            "Estatísticas de frequência"
        };
    }

    public virtual async Task<string> ExplainNumberSelectionAsync(int concurso, int numero)
    {
        await Task.Delay(25);
        return $"Número {numero} selecionado baseado em análise estatística.";
    }
}
