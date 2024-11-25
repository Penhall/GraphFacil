using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services;


public static class AnaliseService
{
    public static void ExecutarAnalise()
    {
        // Separar dados de treinamento e validação
        SepararDadosTreinamentoEValidacao(out Lances trainSorteios, out Lances valSorteios);

        // Realizar o estudo de contagem usando apenas os dados de treinamento
        ISubgrupoRepository subgrupoRepository = new SubgrupoRepository();
        SubgrupoService subgrupoService = new SubgrupoService(subgrupoRepository);
        int totalSorteiosTreinamento = trainSorteios.Count;

        // Executar treinamento e gerar subgrupos identificados pelo ID
        ExecutarTreinamento(trainSorteios);

        // Processar os sorteios para atualizar frequências e calcular valores percentuais
        subgrupoService.ProcessarSorteiosECalcularPercentuais(trainSorteios, totalSorteiosTreinamento);

        // (Opcional) Avaliar o modelo usando os dados de validação
        // AvaliarModelo(valSorteios);
    }

    public static void ExecutarTreinamento(Lances sorteios)
    {
        // Gerar subgrupos SS e NS
        Dictionary<int, Dictionary<int, int>> contagemSS = new Dictionary<int, Dictionary<int, int>>();
        Dictionary<int, Dictionary<int, int>> contagemNS = new Dictionary<int, Dictionary<int, int>>();

        foreach (var sorteio in sorteios.Lista)
        {
            // Gerar subgrupos de 9 números a partir de 15 sorteados (SS)
            Lances ars9 = Infra.Combinar15a9(sorteio.Lista);
            foreach (var subgrupo in ars9.Lista)
            {
                if (!contagemSS.ContainsKey(subgrupo.Id))
                {
                    contagemSS[subgrupo.Id] = new Dictionary<int, int>();
                }
                // Incrementar contagem para acertos de 3 a 9
                int acertos = Infra.Contapontos(sorteio, subgrupo);
                if (acertos >= 3 && acertos <= 9)
                {
                    if (!contagemSS[subgrupo.Id].ContainsKey(acertos))
                    {
                        contagemSS[subgrupo.Id][acertos] = 0;
                    }
                    contagemSS[subgrupo.Id][acertos]++;
                }
            }

            // Gerar subgrupos de 6 números a partir dos 10 não sorteados (NS)
            Lances ars6 = Infra.Combinar10a6(Infra.DevolveOposto(sorteio).Lista);
            foreach (var subgrupo in ars6.Lista)
            {
                if (!contagemNS.ContainsKey(subgrupo.Id))
                {
                    contagemNS[subgrupo.Id] = new Dictionary<int, int>();
                }
                // Incrementar contagem para acertos de 2 a 6
                int acertos = Infra.Contapontos(sorteio, subgrupo);
                if (acertos >= 2 && acertos <= 6)
                {
                    if (!contagemNS[subgrupo.Id].ContainsKey(acertos))
                    {
                        contagemNS[subgrupo.Id][acertos] = 0;
                    }
                    contagemNS[subgrupo.Id][acertos]++;
                }
            }
        }

        // Salvar contagens geradas nos arquivos JSON
        FileService fileService = new FileService();
        fileService.SalvarDados("ContagemSS.json", contagemSS);
        fileService.SalvarDados("ContagemNS.json", contagemNS);
    }

    public static void SepararDadosTreinamentoEValidacao(out Lances trainSorteios, out Lances valSorteios)
    {
        int totalSorteios = Infra.arLoto.Count;

        // Definir os índices para treinamento e validação
        int quantidadeValidacao = 100;
        int quantidadeTreinamento = totalSorteios - quantidadeValidacao;

        // Separar os sorteios para treinamento e validação
        trainSorteios = Infra.arLoto.Take(quantidadeTreinamento).ToLances();
        valSorteios = Infra.arLoto.Skip(quantidadeTreinamento).ToLances();
    }
}
