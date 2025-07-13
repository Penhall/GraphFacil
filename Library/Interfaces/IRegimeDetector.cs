// D:\PROJETOS\GraphFacil\Library\Interfaces\IRegimeDetector.cs - Interface para modelos de meta-aprendizado
using System.Collections.Generic;
using System.Threading.Tasks;
using LotoLibrary.Suporte;

namespace LotoLibrary.Interfaces;

/// <summary>
/// Interface para detectores de regime de mercado/padrão
/// </summary>
public interface IRegimeDetector
{
    /// <summary>
    /// Detecta o regime atual baseado nos dados históricos
    /// </summary>
    /// <param name="historicalData">Dados históricos</param>
    /// <param name="windowSize">Tamanho da janela de análise</param>
    /// <returns>Regime detectado com confiança</returns>
    Task<RegimeDetectionResult> DetectCurrentRegimeAsync(
        List<ConcursoResult> historicalData,
        int windowSize = 20);

    /// <summary>
    /// Lista todos os regimes conhecidos pelo detector
    /// </summary>
    /// <returns>Lista de regimes detectáveis</returns>
    List<RegimePattern> GetKnownRegimes();

    /// <summary>
    /// Treina o detector com dados históricos
    /// </summary>
    /// <param name="trainingData">Dados para treinamento</param>
    /// <returns>Task de treinamento</returns>
    Task TrainAsync(List<ConcursoResult> trainingData);

    /// <summary>
    /// Confiança do detector no regime atual
    /// </summary>
    double CurrentConfidence { get; }

    /// <summary>
    /// Regime atualmente detectado
    /// </summary>
    string CurrentRegime { get; }

    /// <summary>
    /// Histórico de mudanças de regime
    /// </summary>
    List<RegimeTransition> RegimeHistory { get; }
}
