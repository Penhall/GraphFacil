using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services;

public class PalpiteService1
{
    private readonly Random _random;
    private readonly IMLLogger _logger;
    private readonly MLModelRepository _repository;
    private readonly Dictionary<string, GrupoAvaliacao> _gruposAvaliados;
    private readonly MLNetModel _modeloSS;
    private readonly MLNetModel _modeloNS;

    public PalpiteService1(IMLLogger logger, MLNetModel modeloSS, MLNetModel modeloNS)
    {
        _random = new Random();
        _logger = logger;
        _repository = new MLModelRepository(new FileService(), logger);
        _modeloSS = modeloSS;
        _modeloNS = modeloNS;
        _gruposAvaliados = new Dictionary<string, GrupoAvaliacao>();

        // Pré-processar grupos na inicialização
        PreProcessarGrupos();
    }

    private void PreProcessarGrupos()
    {
        try
        {
            _logger.LogInformation("Iniciando pré-processamento dos grupos...");

            var percentuaisSS = _repository.CarregarPercentuaisSS();
            var percentuaisNS = _repository.CarregarPercentuaisNS();

            // Para cada combinação possível de SS e NS
            foreach (var ss in percentuaisSS)
            {
                foreach (var ns in percentuaisNS)
                {
                    var grupo = new GrupoAvaliacao
                    {
                        IdSS = ss.Key,
                        IdNS = ns.Key,
                        PercentuaisSS = ss.Value,
                        PercentuaisNS = ns.Value
                    };

                    // Calcular pontuação usando os modelos
                    var featuresSS = Enumerable.Range(3, 7)
                        .Select(i => ss.Value.ContainsKey(i) ? (float)ss.Value[i] : 0f)
                        .ToArray();

                    var featuresNS = Enumerable.Range(2, 5)
                        .Select(i => ns.Value.ContainsKey(i) ? (float)ns.Value[i] : 0f)
                        .ToArray();

                    var pontuacaoSS = _modeloSS.Predict(featuresSS);
                    var pontuacaoNS = _modeloNS.Predict(featuresNS);

                    grupo.PontuacaoCombinada = (pontuacaoSS + pontuacaoNS) / 2.0;
                    _gruposAvaliados[grupo.IdCC] = grupo;
                }
            }

            _logger.LogInformation($"Pré-processamento concluído. Total de grupos: {_gruposAvaliados.Count}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro no pré-processamento: {ex.Message}");
            throw;
        }
    }

    public List<int[]> GerarPalpitesAleatorios(int quantidade)
    {
        List<int[]> palpites = new List<int[]>();
        for (int i = 0; i < quantidade; i++)
        {
            var palpite = Enumerable.Range(1, 25).OrderBy(x => _random.Next()).Take(15).ToArray();
            palpites.Add(palpite);
        }
        return palpites;
    }

    public List<(int[], float)> ClassificarPalpites(List<int[]> palpites)
    {
        try
        {
            List<(int[], float)> palpitesClassificados = new List<(int[], float)>();

            foreach (var palpite in palpites)
            {
                try
                {
                    _logger.LogInformation($"Avaliando palpite: {string.Join(",", palpite)}");

                    // Gerar subgrupos do palpite
                    var subgruposSS = GerarCombinacoes.Combinar15a9(palpite.ToList());
                    var numerosOpostos = Infra.DevolveOposto(new Lance { Lista = palpite.ToList() }).Lista;
                    var subgruposNS = GerarCombinacoes.Combinar10a6(numerosOpostos);

                    // Avaliar cada combinação de subgrupos
                    float maxPontuacao = 0;
                    foreach (var ss in subgruposSS.Lista)
                    {
                        foreach (var ns in subgruposNS.Lista)
                        {
                            string idCC = $"{ss.Id}-{ns.Id}";
                            if (_gruposAvaliados.TryGetValue(idCC, out var grupo))
                            {
                                maxPontuacao = Math.Max(maxPontuacao, (float)grupo.PontuacaoCombinada);
                            }
                        }
                    }

                    _logger.LogInformation($"Pontuação do palpite: {maxPontuacao:F2}");
                    palpitesClassificados.Add((palpite, maxPontuacao));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao classificar palpite individual: {ex.Message}");
                }
            }

            return palpitesClassificados.OrderByDescending(p => p.Item2).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao classificar palpites: {ex.Message}");
            throw;
        }
    }
}
