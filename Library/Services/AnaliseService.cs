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
        // ExecutarTreinamento(trainSorteios);


        // Executar treinamento e gerar subgrupos identificados pelo ID

        TreinamentoComPercentuais(trainSorteios);

        // Processar os sorteios para atualizar frequências e calcular valores percentuais
        //   subgrupoService.ProcessarSorteiosECalcularPercentuais(trainSorteios, totalSorteiosTreinamento);

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

    public static void TreinamentoComPercentuais(Lances arLotoTreino)
    {
        // Inicializar subgrupos e dicionários de contagem
        Lance oAlvo = arLotoTreino[0];
        Lances ars9 = Infra.Combinar15a9(oAlvo.Lista);
        Lances ars6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        // Inicializar dicionários de contagem
        Dictionary<int, Dictionary<int, int>> contagemSS = ars9.ToDictionary(lance => lance.Id, lance => Enumerable.Range(3, 7).ToDictionary(i => i, i => 0));
        Dictionary<int, Dictionary<int, int>> contagemNS = ars6.ToDictionary(lance => lance.Id, lance => Enumerable.Range(2, 5).ToDictionary(i => i, i => 0));

        int ix = arLotoTreino.Count - 1;
        int ax = ix - 1;

        // Realizar a contagem dos acertos
        while ((ix > 1) && (ax > 0))
        {
            Lance o = arLotoTreino[ix];
            Lance p = arLotoTreino[ax];

            int m = Infra.Contapontos(o, p);

            if (m == 9)
            {
                Lances ars9Tmp = Infra.Combinar15a9(p.Lista);
                Lances ars6Tmp = Infra.Combinar10a6(Infra.DevolveOposto(p).Lista);

                // Loop para contagem de 3 a 9 acertos (Subgrupo SS)
                for (int h = 3; h <= 9; h++)
                {
                    Lances encontrados9 = ars9Tmp.Where(q => Infra.Contapontos(q, o) == h).ToLances();
                    foreach (Lance z in encontrados9)
                    {
                        if (contagemSS.TryGetValue(z.Id, out var contagem))
                        {
                            contagem[h]++;
                        }
                    }
                }

                // Loop para contagem de 2 a 6 acertos (Subgrupo NS)
                for (int h = 2; h <= 6; h++)
                {
                    Lances encontrados6 = ars6Tmp.Where(q => Infra.Contapontos(q, o) == h).ToLances();
                    foreach (Lance z in encontrados6)
                    {
                        if (contagemNS.TryGetValue(z.Id, out var contagem))
                        {
                            contagem[h]++;
                        }
                    }
                }

                ix--;
                ax = ix - 1;
            }
            else
            {
                ax--;
            }
        }

        // Salvar as contagens geradas para análises futuras
        FileService fileService = new FileService();
        fileService.SalvarDados("ContagemSS.json", contagemSS);
        fileService.SalvarDados("ContagemNS.json", contagemNS);

        // Calcular valores percentuais
        int totalSorteios = arLotoTreino.Count;
        Dictionary<int, Dictionary<int, double>> percentuaisSS = new Dictionary<int, Dictionary<int, double>>();
        Dictionary<int, Dictionary<int, double>> percentuaisNS = new Dictionary<int, Dictionary<int, double>>();

        foreach (var subgrupo in contagemSS)
        {
            int subgrupoId = subgrupo.Key;
            var contagens = subgrupo.Value;

            // Criar um novo dicionário para armazenar os percentuais
            percentuaisSS[subgrupoId] = new Dictionary<int, double>();

            foreach (var contagem in contagens)
            {
                int acertos = contagem.Key;
                int frequencia = contagem.Value;

                // Calcular o percentual em relação ao total de sorteios
                double percentual = (double)frequencia / totalSorteios * 100.0;
                percentuaisSS[subgrupoId][acertos] = percentual;
            }
        }

        foreach (var subgrupo in contagemNS)
        {
            int subgrupoId = subgrupo.Key;
            var contagens = subgrupo.Value;

            // Criar um novo dicionário para armazenar os percentuais
            percentuaisNS[subgrupoId] = new Dictionary<int, double>();

            foreach (var contagem in contagens)
            {
                int acertos = contagem.Key;
                int frequencia = contagem.Value;

                // Calcular o percentual em relação ao total de sorteios
                double percentual = (double)frequencia / totalSorteios * 100.0;
                percentuaisNS[subgrupoId][acertos] = percentual;
            }
        }

        // Salvar percentuais calculados para análises futuras
        fileService.SalvarDados("PercentuaisSS.json", percentuaisSS);
        fileService.SalvarDados("PercentuaisNS.json", percentuaisNS);
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
