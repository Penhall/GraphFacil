using LotoLibrary.Infrastructure.Logging;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace LotoLibrary.Services;

public static class AnaliseService
{
    private static readonly IMLLogger _logger = new MLLogger(
        LoggerFactory.Create(builder => builder.AddConsole())
        .CreateLogger<MLLogger>());

    private static Lances LotoData = new Lances();

    static AnaliseService()
    {
        // Definir cultura padrão para invariant
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
    }

    public static void ExecutarAnalise()
    {
        try
        {
            LotoData = Infra.AbrirArquivoJson("Lotofacil.json"); // Corrigido: Usar AbrirArquivoJson

            _logger.LogInformation("Iniciando análise com ML.NET");

            // Separar dados de treinamento e validação
            SepararDadosTreinamentoEValidacao(out Lances trainSorteios, out Lances valSorteios);

            // Gerar percentuais de treinamento
            TreinamentoComPercentuais(trainSorteios);

            // Criar e treinar modelos ML.NET com os tipos corretos
            var modelSS = new MLNetModel(_logger, "ModeloSS.zip", "SS");
            var modelNS = new MLNetModel(_logger, "ModeloNS.zip", "NS");

            // Treinar modelos
            _logger.LogInformation("Iniciando treinamento do modelo SS");
            modelSS.Train("PercentuaisSS.json", usarCrossValidation: true);

            _logger.LogInformation("Iniciando treinamento do modelo NS");
            modelNS.Train("PercentuaisNS.json", usarCrossValidation: true);

            // Validar modelos com dados de validação se necessário
            ValidarModelos(modelSS, modelNS, valSorteios);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro durante execução da análise: {ex.Message}", ex);
            throw;
        }
    }


    public static void TreinamentoComPercentuais(Lances arLotoTreino)
    {
        try
        {
            _logger.LogInformation($"Iniciando geração de percentuais com {arLotoTreino.Count} sorteios");

            // Inicializar subgrupos e dicionários de contagem
            Lance oAlvo = arLotoTreino[0];
            Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);
            Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            Dictionary<int, Dictionary<int, int>> contagemSS = ars9.ToDictionary(
                lance => lance.Id,
                lance => Enumerable.Range(3, 7).ToDictionary(i => i, i => 0));

            Dictionary<int, Dictionary<int, int>> contagemNS = ars6.ToDictionary(
                lance => lance.Id,
                lance => Enumerable.Range(2, 5).ToDictionary(i => i, i => 0));

            ProcessarContagens(arLotoTreino, contagemSS, contagemNS);

            // Calcular e salvar percentuais
            SalvarPercentuais(contagemSS, contagemNS, arLotoTreino.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro no processamento de percentuais: {ex.Message}", ex);
            throw;
        }
    }

    private static void ProcessarContagens(Lances arLotoTreino, Dictionary<int, Dictionary<int, int>> contagemSS, Dictionary<int, Dictionary<int, int>> contagemNS)
    {
        int ix = arLotoTreino.Count - 1;
        int ax = ix - 1;

        while ((ix > 1) && (ax > 0))
        {
            Lance o = arLotoTreino[ix];
            Lance p = arLotoTreino[ax];
            int m = Infra.Contapontos(o, p);

            if (m == 9)
            {
                ProcessarAcertos(p, o, contagemSS, contagemNS);
                ix--;
                ax = ix - 1;
            }
            else
            {
                ax--;
            }
        }
    }

    private static void ProcessarAcertos(Lance p, Lance o, Dictionary<int, Dictionary<int, int>> contagemSS, Dictionary<int, Dictionary<int, int>> contagemNS)
    {
        Lances ars9Tmp = GerarCombinacoes.Combinar15a9(p.Lista);
        Lances ars6Tmp = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(p).Lista);

        // Processar SS (3-9 acertos)
        for (int h = 3; h <= 9; h++)
        {
            foreach (var z in ars9Tmp.Where(q => Infra.Contapontos(q, o) == h))
            {
                if (contagemSS.TryGetValue(z.Id, out var contagem))
                {
                    contagem[h]++;
                }
            }
        }

        // Processar NS (2-6 acertos)
        for (int h = 2; h <= 6; h++)
        {
            foreach (var z in ars6Tmp.Where(q => Infra.Contapontos(q, o) == h))
            {
                if (contagemNS.TryGetValue(z.Id, out var contagem))
                {
                    contagem[h]++;
                }
            }
        }
    }

    private static void SalvarPercentuais(Dictionary<int, Dictionary<int, int>> contagemSS, Dictionary<int, Dictionary<int, int>> contagemNS, int totalSorteios)
    {
        var fileService = new FileService();

        var percentuaisSS = CalcularPercentuais(contagemSS, totalSorteios);
        var percentuaisNS = CalcularPercentuais(contagemNS, totalSorteios);

        fileService.SalvarDados("PercentuaisSS.json", percentuaisSS);
        fileService.SalvarDados("PercentuaisNS.json", percentuaisNS);

        _logger.LogInformation("Percentuais calculados e salvos com sucesso");
    }

    private static Dictionary<int, Dictionary<int, double>> CalcularPercentuais(Dictionary<int, Dictionary<int, int>> contagens, int totalSorteios)
    {
        return contagens.ToDictionary(
            subgrupo => subgrupo.Key,
            subgrupo => subgrupo.Value.ToDictionary(
                contagem => contagem.Key,
                contagem => totalSorteios > 0 ? (double)contagem.Value / totalSorteios * 100.0 : 0.0 // Evitar divisão por zero
            )
        );
    }

    private static void ValidarModelos(MLNetModel modelSS, MLNetModel modelNS, Lances valSorteios)
    {
        _logger.LogInformation("Iniciando validação dos modelos com dados de teste");

        foreach (var sorteio in valSorteios) // Iterar diretamente sobre Lances
        {
            // Gerar subgrupos para validação
            var subgruposSS = GerarCombinacoes.Combinar15a9(sorteio.Lista);
            var subgruposNS = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(sorteio).Lista);

            // Validar predições
            foreach (var subgrupo in subgruposSS) // Iterar diretamente sobre Lances
            {
                var predicaoSS = modelSS.Predict(subgrupo.Lista.Select(x => (float)x).ToArray());
                _logger.LogInformation($"Predição SS para subgrupo {subgrupo.Id}: {predicaoSS}");
            }

            foreach (var subgrupo in subgruposNS) // Iterar diretamente sobre Lances
            {
                var predicaoNS = modelNS.Predict(subgrupo.Lista.Select(x => (float)x).ToArray());
                _logger.LogInformation($"Predição NS para subgrupo {subgrupo.Id}: {predicaoNS}");
            }
        }
    }

    public static void SepararDadosTreinamentoEValidacao(out Lances trainSorteios, out Lances valSorteios)
    {
        // int totalSorteios = Infra.arLoto.Count;
        int totalSorteios = LotoData.Count;
        int quantidadeValidacao = 100;
        int quantidadeTreinamento = totalSorteios - quantidadeValidacao;

        // Garantir que não tentemos pegar mais do que existe ou quantidades negativas
        quantidadeTreinamento = Math.Max(0, quantidadeTreinamento);
        quantidadeValidacao = Math.Min(totalSorteios, quantidadeValidacao);
        if (quantidadeTreinamento + quantidadeValidacao > totalSorteios)
        {
            quantidadeValidacao = totalSorteios - quantidadeTreinamento;
        }


        trainSorteios = LotoData.Take(quantidadeTreinamento).ToLances1();
        valSorteios = LotoData.Skip(quantidadeTreinamento).Take(quantidadeValidacao).ToLances1(); // Adicionado Take

        _logger.LogInformation($"Dados separados: {trainSorteios.Count} para treino, {valSorteios.Count} para validação");

    }
}
