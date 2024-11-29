using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services
{
    public class PalpiteService
    {
        private readonly Random _random;
        private readonly IMLLogger _logger;
        private readonly MLModelRepository _repository;

        public PalpiteService(IMLLogger logger)
        {
            _random = new Random();
            _logger = logger;
            _repository = new MLModelRepository(new FileService(), logger);
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

        public List<(int[], float)> ClassificarPalpites(MLNetModel modeloSS, MLNetModel modeloNS, List<int[]> palpites)
        {
            try
            {
                // Carregar os percentuais históricos
                var percentuaisSS = _repository.CarregarPercentuaisSS();
                var percentuaisNS = _repository.CarregarPercentuaisNS();

                _logger.LogInformation($"Percentuais SS carregados: {percentuaisSS.Count} subgrupos");
                _logger.LogInformation($"Percentuais NS carregados: {percentuaisNS.Count} subgrupos");

                List<(int[], float)> palpitesClassificados = new List<(int[], float)>();

                foreach (var palpite in palpites)
                {
                    try
                    {
                        _logger.LogInformation($"Processando palpite: {string.Join(",", palpite)}");

                        // Gerar todos os subgrupos possíveis do palpite
                        Lances subgruposSS = GerarCombinacoes.Combinar15a9(palpite.ToList());
                        var listaOposta = Infra.DevolveOposto(new Lance { Lista = palpite.ToList() }).Lista;
                        Lances subgruposNS = GerarCombinacoes.Combinar10a6(listaOposta);

                        // Processar subgrupos SS
                        float pontuacaoTotalSS = 0;
                        int subgruposSSProcessados = 0;

                        foreach (var ss in subgruposSS.Lista)
                        {
                            if (percentuaisSS.TryGetValue(ss.Id, out var percentuais))
                            {
                                var features = percentuais.Values
                                    .OrderBy(kvp => kvp)
                                    .Select(v => (float)v)
                                    .ToArray();

                                var predicao = modeloSS.Predict(features);
                                pontuacaoTotalSS += predicao;
                                subgruposSSProcessados++;

                                _logger.LogInformation($"Subgrupo SS {ss.Id} - Percentuais: {string.Join(",", features)} - Predição: {predicao}");
                            }
                        }

                        // Processar subgrupos NS
                        float pontuacaoTotalNS = 0;
                        int subgruposNSProcessados = 0;

                        foreach (var ns in subgruposNS.Lista)
                        {
                            if (percentuaisNS.TryGetValue(ns.Id, out var percentuais))
                            {
                                var features = percentuais.Values
                                    .OrderBy(kvp => kvp)
                                    .Select(v => (float)v)
                                    .ToArray();

                                var predicao = modeloNS.Predict(features);
                                pontuacaoTotalNS += predicao;
                                subgruposNSProcessados++;

                                _logger.LogInformation($"Subgrupo NS {ns.Id} - Percentuais: {string.Join(",", features)} - Predição: {predicao}");
                            }
                        }

                        // Calcular pontuação média
                        float pontuacaoMedia = 0;
                        if (subgruposSSProcessados > 0 || subgruposNSProcessados > 0)
                        {
                            float mediaSSAjustada = subgruposSSProcessados > 0 ? pontuacaoTotalSS / subgruposSSProcessados : 0;
                            float mediaNSAjustada = subgruposNSProcessados > 0 ? pontuacaoTotalNS / subgruposNSProcessados : 0;
                            pontuacaoMedia = (mediaSSAjustada + mediaNSAjustada) /
                                (subgruposSSProcessados > 0 && subgruposNSProcessados > 0 ? 2 : 1);
                        }

                        _logger.LogInformation($"Pontuação final do palpite: {pontuacaoMedia}");
                        palpitesClassificados.Add((palpite, pontuacaoMedia));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Erro ao classificar palpite individual: {ex.Message}");
                    }
                }

                // Ordenar palpites por pontuação
                var resultado = palpitesClassificados.OrderByDescending(p => p.Item2).ToList();
                _logger.LogInformation($"Total de palpites classificados: {resultado.Count}");

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao classificar palpites: {ex.Message}");
                throw;
            }
        }

        // Método auxiliar para debug de dados
        private void LogFeatures(float[] features, string tipo)
        {
            _logger.LogInformation($"Features {tipo}: {string.Join(", ", features)}");
        }

        // Método auxiliar para formatar números
        private string FormatarNumeros(IEnumerable<int> numeros)
        {
            return string.Join(", ", numeros.Select(n => n.ToString("00")));
        }
    }
}