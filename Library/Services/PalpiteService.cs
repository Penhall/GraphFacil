using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services;

//public class PalpiteService
//{
//    private readonly Random _random;
//    private readonly IMLLogger _logger;
//    private readonly MLModelRepository _repository;

//    public PalpiteService(IMLLogger logger)
//    {
//        _random = new Random();
//        _logger = logger;
//        _repository = new MLModelRepository(new FileService(), logger);
//    }

//    public List<int[]> GerarPalpitesAleatorios(int quantidade)
//    {
//        List<int[]> palpites = new List<int[]>();

//        for (int i = 0; i < quantidade; i++)
//        {
//            var palpite = Enumerable.Range(1, 25).OrderBy(x => _random.Next()).Take(15).ToArray();
//            palpites.Add(palpite);
//        }

//        return palpites;
//    }

//    public List<(int[], float)> ClassificarPalpites(MLNetModel modeloSS, MLNetModel modeloNS, List<int[]> palpites)
//    {
//        try
//        {
//            // Carregar os percentuais históricos
//            var percentuaisSS = _repository.CarregarPercentuaisSS();
//            var percentuaisNS = _repository.CarregarPercentuaisNS();

//            _logger.LogInformation($"Percentuais SS carregados: {percentuaisSS.Count} subgrupos");
//            _logger.LogInformation($"Percentuais NS carregados: {percentuaisNS.Count} subgrupos");

//            List<(int[], float)> palpitesClassificados = new List<(int[], float)>();

//            foreach (var palpite in palpites)
//            {
//                try
//                {
//                    _logger.LogInformation($"Processando palpite: {string.Join(",", palpite)}");

//                    // Criar Lance do palpite atual
//                    var lancePalpite = new Lance { Lista = palpite.ToList() };

//                    // Gerar subgrupos de 9 números do palpite (SS)
//                    var subgruposSS = GerarCombinacoes.Combinar15a9(palpite.ToList());

//                    // Gerar subgrupos de 6 números dos não sorteados (NS)
//                    var numerosOpostos = Infra.DevolveOposto(lancePalpite).Lista;
//                    var subgruposNS = GerarCombinacoes.Combinar10a6(numerosOpostos);

//                    // Processamento SS
//                    float pontuacaoSS = ProcessarSubgruposSS(subgruposSS, percentuaisSS, modeloSS);
//                    float pontuacaoNS = ProcessarSubgruposNS(subgruposNS, percentuaisNS, modeloNS);

//                    // Calcular pontuação final combinada
//                    float pontuacaoFinal = (pontuacaoSS + pontuacaoNS) / 2.0f;

//                    _logger.LogInformation($"Pontuação final do palpite: SS={pontuacaoSS:F2}, NS={pontuacaoNS:F2}, Final={pontuacaoFinal:F2}");
//                    palpitesClassificados.Add((palpite, pontuacaoFinal));
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError($"Erro ao classificar palpite individual: {ex.Message}");
//                }
//            }

//            return palpitesClassificados.OrderByDescending(p => p.Item2).ToList();
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError($"Erro ao classificar palpites: {ex.Message}");
//            throw;
//        }
//    }

//    private float ProcessarSubgruposSS(Lances subgruposSS,
//        Dictionary<int, Dictionary<int, double>> percentuaisSS,
//        MLNetModel modeloSS)
//    {
//        float totalPontuacao = 0;
//        int subgruposProcessados = 0;

//        foreach (var subgrupo in subgruposSS.Lista)
//        {
//            if (percentuaisSS.TryGetValue(subgrupo.Id, out var percentuais))
//            {
//                // Ordenar percentuais por quantidade de acertos (3 a 9)
//                var features = Enumerable.Range(3, 7)
//                    .Select(i => percentuais.ContainsKey(i) ? (float)percentuais[i] : 0f)
//                    .ToArray();

//                var predicao = modeloSS.Predict(features);
//                totalPontuacao += predicao;
//                subgruposProcessados++;

//                _logger.LogInformation($"Subgrupo SS {subgrupo.Id} - Features: {string.Join(",", features)} - Predição: {predicao}");
//            }
//        }

//        return subgruposProcessados > 0 ? totalPontuacao / subgruposProcessados : 0;
//    }

//    private float ProcessarSubgruposNS(Lances subgruposNS,
//        Dictionary<int, Dictionary<int, double>> percentuaisNS,
//        MLNetModel modeloNS)
//    {
//        float totalPontuacao = 0;
//        int subgruposProcessados = 0;

//        foreach (var subgrupo in subgruposNS.Lista)
//        {
//            if (percentuaisNS.TryGetValue(subgrupo.Id, out var percentuais))
//            {
//                // Ordenar percentuais por quantidade de acertos (2 a 6)
//                var features = Enumerable.Range(2, 5)
//                    .Select(i => percentuais.ContainsKey(i) ? (float)percentuais[i] : 0f)
//                    .ToArray();

//                var predicao = modeloNS.Predict(features);
//                totalPontuacao += predicao;
//                subgruposProcessados++;

//                _logger.LogInformation($"Subgrupo NS {subgrupo.Id} - Features: {string.Join(",", features)} - Predição: {predicao}");
//            }
//        }

//        return subgruposProcessados > 0 ? totalPontuacao / subgruposProcessados : 0;
//    }
//}


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

    public List<(int[], float)> ClassificarPalpites(MLNetModel modeloSS, MLNetModel modeloNS, List<int[]> palpites)
    {
        try
        {
            // Carregar últimos sorteios para comparação
            var ultimosSorteios = Infra.arLoto.Take(50).ToList();  // Usar últimos 50 sorteios como referência
            var palpitesClassificados = new List<(int[], float)>();

            foreach (var palpite in palpites)
            {
                try
                {
                    _logger.LogInformation($"Processando palpite: {string.Join(",", palpite)}");
                    var lancePalpite = new Lance { Lista = palpite.ToList() };

                    // Verificar acertos com últimos sorteios
                    Dictionary<int, int> contagemAcertosSS = new Dictionary<int, int>();
                    Dictionary<int, int> contagemAcertosNS = new Dictionary<int, int>();

                    // Inicializar contagens
                    for (int i = 3; i <= 9; i++) contagemAcertosSS[i] = 0;
                    for (int i = 2; i <= 6; i++) contagemAcertosNS[i] = 0;

                    foreach (var sorteio in ultimosSorteios)
                    {
                        // Verificar subgrupos SS
                        var subgruposSS = GerarCombinacoes.Combinar15a9(lancePalpite.Lista);
                        foreach (var ss in subgruposSS.Lista)
                        {
                            int acertos = Infra.Contapontos(ss, sorteio);
                            if (acertos >= 3 && acertos <= 9)
                                contagemAcertosSS[acertos]++;
                        }

                        // Verificar subgrupos NS
                        var numerosOpostos = Infra.DevolveOposto(lancePalpite).Lista;
                        var subgruposNS = GerarCombinacoes.Combinar10a6(numerosOpostos);
                        foreach (var ns in subgruposNS.Lista)
                        {
                            int acertos = Infra.Contapontos(ns, sorteio);
                            if (acertos >= 2 && acertos <= 6)
                                contagemAcertosNS[acertos]++;
                        }
                    }

                    // Converter contagens para percentuais
                    float[] featuresSS = contagemAcertosSS
                        .OrderBy(kv => kv.Key)
                        .Select(kv => (float)kv.Value / ultimosSorteios.Count * 100)
                        .ToArray();

                    float[] featuresNS = contagemAcertosNS
                        .OrderBy(kv => kv.Key)
                        .Select(kv => (float)kv.Value / ultimosSorteios.Count * 100)
                        .ToArray();

                    // Fazer predições
                    float pontuacaoSS = modeloSS.Predict(featuresSS);
                    float pontuacaoNS = modeloNS.Predict(featuresNS);

                    float pontuacaoFinal = (pontuacaoSS + pontuacaoNS) / 2.0f;

                    _logger.LogInformation($"Percentuais SS: {string.Join(",", featuresSS)}");
                    _logger.LogInformation($"Percentuais NS: {string.Join(",", featuresNS)}");
                    _logger.LogInformation($"Pontuações - SS: {pontuacaoSS:F2}, NS: {pontuacaoNS:F2}, Final: {pontuacaoFinal:F2}");

                    palpitesClassificados.Add((palpite, pontuacaoFinal));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Erro ao classificar palpite: {ex.Message}");
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
}
