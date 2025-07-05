using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services;

public class ValidationService
{
    public static (double acuraciaMedia, double desvioPadrao) ValidarModelo(
    List<Lance> dadosTreino,
        List<Lance> dadosValidacao)
    {
        var engine = new OscillatorEngine(new Lances(dadosTreino));
        var osciladores = engine.InicializarOsciladores();

        var acuracias = new List<double>();

        foreach (var sorteio in dadosValidacao)
        {
            // Atualiza osciladores com o sorteio anterior
            OscillatorStrategy.AplicarTendenciaCurtoPrazo(osciladores, new List<Lance> { sorteio });

            // Gera palpite
            var palpite = osciladores
.OrderByDescending(d => d.ForcaSincronizacao)
                .Take(15)
.Select(d => d.Numero)
                    .ToList();

            // Calcula acurácia
            int acertos = palpite.Intersect(sorteio.Lista).Count();
            acuracias.Add(acertos / 15.0);
        }

        return (
        acuracias.Average(),
Math.Sqrt(acuracias.Select(a => Math.Pow(a - acuracias.Average(), 2)).Sum() / acuracias.Count
            ));
    }
}