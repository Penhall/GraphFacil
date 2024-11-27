using Microsoft.ML;
using LotoLibrary.Models;
using LotoLibrary.Services;
using LotoLibrary.Interfaces;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Threading;

namespace LotoLibrary.NeuralNetwork.Models;

public class SubgrupoMLService
{
    private readonly IMLModelRepository _repository;
    private readonly IMLLogger _logger;
    private readonly MLContext _mlContext;

    public SubgrupoMLService(IMLModelRepository repository, IMLLogger logger)
    {
        _repository = repository;
        _logger = logger;
        _mlContext = new MLContext(seed: 42);

        // Garantir cultura invariant para operações numéricas
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    public void ProcessarSorteiosECalcularPercentuais(Lances sorteios)
    {
        try
        {
            _logger.LogInformation($"Iniciando processamento de {sorteios?.Count ?? 0} sorteios");

            var percentuaisSS = new Dictionary<int, Dictionary<int, double>>();
            var percentuaisNS = new Dictionary<int, Dictionary<int, double>>();

            if (sorteios?.Lista != null && sorteios.Count > 0)
            {
                // Processar subgrupos SS
                var primeiroSorteio = sorteios.Lista[0];
                var subgruposSS = GerarCombinacoes.Combinar15a9(primeiroSorteio.Lista);
                percentuaisSS = CalcularPercentuaisSubgrupos(subgruposSS, sorteios, 3, 9);

                // Processar subgrupos NS
                var subgruposNS = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(primeiroSorteio).Lista);
                percentuaisNS = CalcularPercentuaisSubgrupos(subgruposNS, sorteios, 2, 6);

                // Salvar resultados
                _repository.SalvarPercentuaisSS(percentuaisSS);
                _repository.SalvarPercentuaisNS(percentuaisNS);

                _logger.LogInformation("Processamento concluído com sucesso");
            }
            else
            {
                _logger.LogWarning("Nenhum sorteio disponível para processamento");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro durante processamento dos sorteios", ex);
            throw;
        }
    }

    private Dictionary<int, Dictionary<int, double>> CalcularPercentuaisSubgrupos(
             Lances subgrupos,
             Lances sorteios,
             int minAcertos,
             int maxAcertos)
    {
        // Usar formatação invariant para números
        var nfi = NumberFormatInfo.InvariantInfo;
        var resultado = new Dictionary<int, Dictionary<int, double>>();
        int totalSorteios = sorteios.Count;

        foreach (var subgrupo in subgrupos.Lista)
        {
            resultado[subgrupo.Id] = new Dictionary<int, double>();
            var contagem = new Dictionary<int, int>();

            for (int i = minAcertos; i <= maxAcertos; i++)
            {
                contagem[i] = 0;
            }

            foreach (var sorteio in sorteios.Lista)
            {
                int acertos = Infra.Contapontos(sorteio, subgrupo);
                if (acertos >= minAcertos && acertos <= maxAcertos)
                {
                    contagem[acertos]++;
                }
            }

            foreach (var item in contagem)
            {
                // Usar formatação invariant para conversões numéricas
                resultado[subgrupo.Id][item.Key] =
                    Convert.ToDouble(item.Value) / Convert.ToDouble(totalSorteios) * 100.0;
            }
        }

        return resultado;
    }
}


