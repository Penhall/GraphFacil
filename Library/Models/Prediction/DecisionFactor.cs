// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionModels.cs - Modelos de dados
namespace LotoLibrary.Models.Prediction
{
   /// <summary>
/// Fator de decisão usado pelos modelos
/// </summary>
public class DecisionFactor
{
    /// <summary>
    /// Nome do fator
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary>
    /// Peso do fator na decisão (0.0 a 1.0)
    /// </summary>
    public double Weight { get; set; } = 0.0;

    /// <summary>
    /// Descrição do fator
    /// </summary>
    public string Description { get; set; } = "";

    /// <summary>
    /// Valor atual do fator
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Tipo de dado do valor
    /// </summary>
    public string ValueType { get; set; } = "";

    /// <summary>
    /// Se o fator está ativo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Impacto do fator (positivo, negativo, neutro)
    /// </summary>
    public string Impact { get; set; } = "Neutro";

    public override string ToString()
    {
        return $"{Name}: {Weight:P2} - {Value} ({Impact})";
    }
}
}
