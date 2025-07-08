using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Engines;

public class OscillatorEngine
{
    private readonly List<Lance> _historicoCompleto;
    private readonly List<Lance> _dadosTreino;
    private readonly List<Lance> _dadosValidacao;
    
    public IReadOnlyList<Lance> DadosTreino => _dadosTreino.AsReadOnly();
    public IReadOnlyList<Lance> DadosValidacao => _dadosValidacao.AsReadOnly();

    public OscillatorEngine(Lances historico)
    {
        _historicoCompleto = historico.ToList();
        (_dadosTreino, _dadosValidacao) = SplitData(historico, 100);
    }

    private (List<Lance> treino, List<Lance> validacao) SplitData(Lances historico, int validacaoSize)
    {
        var treino = historico.SkipLast(validacaoSize).ToList();
        var validacao = historico.TakeLast(validacaoSize).ToList();
        return (treino, validacao);
    }

    public List<DezenaOscilante> InicializarOsciladores()
    {
        var dezenas = Enumerable.Range(1, 25).Select(num => new DezenaOscilante
        {
            Numero = num,
            Fase = new Random().Next(0, 360),
            Frequencia = 1.0
        }).ToList();

        AplicarEstrategiasIniciais(dezenas);
        return dezenas;
    }

    private void AplicarEstrategiasIniciais(List<DezenaOscilante> dezenas)
    {
        // 1. Estratégia de Frequência Base
        var freqMedia = _dadosTreino
            .SelectMany(l => l.Lista)
            .GroupBy(n => n)
            .ToDictionary(g => g.Key, g => g.Count() / (double)_dadosTreino.Count);

        // 2. Estratégia de Atraso
        var ultimosSorteios = _dadosTreino.TakeLast(10).SelectMany(l => l.Lista).ToList();

        foreach (var dezena in dezenas)
        {
            dezena.ForcaSincronizacao = freqMedia.TryGetValue(dezena.Numero, out var freq) ? freq : 0.1;
            dezena.UltimoAtraso = ultimosSorteios.Contains(dezena.Numero) ? 0 : 10;
            dezena.Frequencia = CalcularFrequenciaInicial(dezena);
        }
    }

    private double CalcularFrequenciaInicial(DezenaOscilante dezena)
    {
        // Fórmula híbrida considerando múltiplos fatores
        double baseValue = 1.0;
        double freqFactor = Math.Log(1 + dezena.ForcaSincronizacao * 10);
        double delayFactor = 1 / (1 + Math.Exp(-dezena.UltimoAtraso / 5.0));

        return baseValue * freqFactor * delayFactor;
    }
}