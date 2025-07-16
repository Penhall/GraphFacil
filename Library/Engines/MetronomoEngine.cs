// E:\PROJETOS\GraphFacil\Library\Engines\MetronomoEngine.cs - Correções nas linhas problemáticas
// Este arquivo contém apenas as correções para as linhas com erro
// Substitua as linhas indicadas no arquivo original

using System;
using System.Collections.Generic;
using System.Linq;
using LotoLibrary.Models;
using LotoLibrary.Models.Validation;

// CORREÇÃO LINHA 409: Substituir PredictionValidationResult por PredictionValidationResult
// ANTES: private List<PredictionValidationResult> ValidarMetronomo(...)
// DEPOIS:
private List<PredictionValidationResult> ValidarMetronomo(Metronomo metronomo, Lances dados, int startIndex = 0)
{
    var resultados = new List<PredictionValidationResult>();
    
    // Implementação do método...
    // (manter o resto da implementação original, apenas mudando o tipo de retorno)
    
    return resultados;
}

// CORREÇÃO LINHA 427: Substituir PredictionValidationResult por PredictionValidationResult
// ANTES: private PredictionValidationResult CriarPredictionValidationResult(...)
// DEPOIS:
private PredictionValidationResult CriarPredictionValidationResult(int concursoId, List<int> predicao, Lance resultado)
{
    var validationResult = new PredictionValidationResult(concursoId, predicao, resultado.Lista)
    {
        TipoEstrategia = "Metrônomo",
        Confidence = 0.75 // Valor padrão de confiança
    };
    
    return validationResult;
}

// CORREÇÃO LINHA 480: Substituir PredictionValidationResult por PredictionValidationResult
// ANTES: private void CalcularEstatisticas(List<PredictionValidationResult> resultados)
// DEPOIS:
private void CalcularEstatisticas(List<PredictionValidationResult> resultados)
{
    if (!resultados.Any()) return;
    
    // CORREÇÃO LINHA 491: Adicionar System.Linq para Average
    var taxasAcerto = resultados.Select(r => r.TaxaAcerto).ToList();
    double mediaAcertos = taxasAcerto.Average();
    
    // CORREÇÃO LINHA 492: Converter para List<double>
    var acertos = resultados.Select(r => (double)r.Acertos).ToList();
    CalcularDesvio(acertos);
    
    // CORREÇÃO LINHA 495: Adicionar System.Linq para Average
    var confiancas = resultados.Select(r => r.Confidence).ToList();
    double mediaConfianca = confiancas.Average();
    
    // Armazenar estatísticas calculadas
    Console.WriteLine($"Média de acertos: {mediaAcertos:F2}");
    Console.WriteLine($"Média de confiança: {mediaConfianca:F2}");
}

// MÉTODO AUXILIAR para cálculo de desvio
private double CalcularDesvio(List<double> valores)
{
    if (!valores.Any()) return 0.0;
    
    double media = valores.Average();
    double somaQuadrados = valores.Sum(x => Math.Pow(x - media, 2));
    return Math.Sqrt(somaQuadrados / valores.Count);
}

// ADICIONAR no início do arquivo se não estiver presente:
using System.Linq;