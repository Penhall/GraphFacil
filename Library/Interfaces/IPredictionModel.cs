// D:\PROJETOS\GraphFacil\Library\Interfaces\IPredictionModel.cs
using LotoLibrary.Models;
using LotoLibrary.Models.Prediction; // ← IMPORTANTE: namespace correto para ValidationResult
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LotoLibrary.Interfaces;

/// <summary>
/// Interface completa para modelos de predição
/// CORRIGIDA para usar ValidationResult do namespace correto
/// </summary>
public interface IPredictionModel : IDisposable
{
    // ===== PROPRIEDADES BÁSICAS =====
    /// <summary>
    /// Nome amigável do modelo
    /// </summary>
    string ModelName { get; }

    /// <summary>
    /// Indica se o modelo está inicializado e pronto para uso
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Nível de confiança do modelo (0.0 a 1.0)
    /// </summary>
    double Confidence { get; }

    // ===== MÉTODOS ESSENCIAIS =====
    /// <summary>
    /// Inicializa o modelo com dados históricos
    /// </summary>
    /// <param name="historicalData">Dados históricos para inicialização</param>
    /// <returns>True se inicializado com sucesso</returns>
    Task<bool> InitializeAsync(Lances historicalData);

    /// <summary>
    /// Treina o modelo com dados fornecidos
    /// </summary>
    /// <param name="trainingData">Dados para treinamento</param>
    /// <returns>True se treinamento foi bem-sucedido</returns>
    Task<bool> TrainAsync(Lances trainingData);

    /// <summary>
    /// Gera predição para o concurso especificado
    /// </summary>
    /// <param name="concurso">Número do concurso alvo</param>
    /// <returns>Resultado da predição</returns>
    Task<PredictionResult> PredictAsync(int concurso);

    // ===== MÉTODOS ADICIONAIS =====
    /// <summary>
    /// Valida o modelo com dados de teste
    /// ✅ CORRIGIDO: Usando ValidationResult qualificado do namespace correto
    /// </summary>
    /// <param name="testData">Dados para validação</param>
    /// <returns>Resultado da validação</returns>
    Task<LotoLibrary.Models.Prediction.ValidationResult> ValidateAsync(Lances testData);

    /// <summary>
    /// Atualiza parâmetros do modelo
    /// </summary>
    /// <param name="parameters">Novos parâmetros</param>
    void UpdateParameters(Dictionary<string, object> parameters);

    /// <summary>
    /// Obtém estado atual do modelo
    /// </summary>
    /// <returns>Estado em formato de string</returns>
    string GetStatus();
}