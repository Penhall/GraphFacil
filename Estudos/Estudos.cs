using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Busisness;

public static class Estudos
{

    /// <summary>
    /// Executa o primeiro estudo combinando dados de loteria.
    /// </summary>
    /// <param name="alvo">O alvo para o estudo.</param>
    /// <returns>Uma lista de resultados do tipo Lances.</returns>
    public static Lances Estudo1(int alvo)
    {

        Infra.CombinarGeral();

        Lances ars = new();
        Lances ars1 = new();
        Lances ars2 = new();
        Lances ars3 = new();

        Lances arGeral = Infra.DevolveBaseAleatoria(10000);


        Lance oAlvo = Infra.arLoto[alvo - 2];

        Lances arBase = Infra.DevolveBaseCompleta(oAlvo, 9);

        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        string arq1 = "SixSeletos-" + alvo.ToString();

        Lances arJson = Infra.AbrirArquivoJson(arq1);

        List<int> l9 = Enumerable.Repeat(0, ars9.Count).ToList();
        List<int> l6 = Enumerable.Repeat(0, ars6.Count).ToList();

        Random random = new Random();

        int ax = 200000;

        while (ars.Count < 10000)
        {

            Lance a = arBase[ax];
            // Lance a = arBase[random.Next(arBase.Count)];

            List<int> list = new List<int>();

            foreach (Lance o in ars9) { if (Infra.Contapontos(o, a) == 9) { list.Add(o.Id); l9[o.Id]++; a.M = o.Id; break; } }
            foreach (Lance o in ars6) { if (Infra.Contapontos(o, a) == 6) { list.Add(o.Id); l6[o.Id]++; a.N = o.Id; break; } }
            ars.Add(a);
            ars1.Add(new Lance(ars1.Count, list));

            ax++;
        }

        foreach (Lance o in ars)
        {
            ars9[o.M].ListaM.Add(o.N);
            ars6[o.N].ListaM.Add(o.M);

            ars9[o.M].ListaX.Add(o);
            ars9[o.M].ListaY.Add(ars6[o.N]);

            ars6[o.N].ListaX.Add(o);
            ars6[o.N].ListaY.Add(ars9[o.M]);

        }

        foreach (Lance o in ars9) { o.Ordena(); }
        foreach (Lance o in ars6) { o.Ordena(); }

        ars9.Sort();
        ars6.Sort();


        Lances ars9Principais = new();
        Lances ars6Principais = new();

        ars9Principais.AddRange(ars9.Take(101));
        ars6Principais.AddRange(ars6.Take(45));

        List<int> ints6 = new();

        foreach (Lance o in ars9Principais) { foreach (Lance p in o.ListaX) ars2.Add(p); }

        foreach (Lance o in ars6Principais) { ints6.Add(o.Id); foreach (Lance p in o.ListaX) ars3.Add(p); }


        Infra.SalvaSaidaW(arGeral, Infra.NomeSaida("Controle", alvo));

        Infra.SalvaSaidaW(ars1, Infra.NomeSaida("Misturas", alvo));
        //    Infra.SalvaSaidaW(l9, Infra.NomeSaida("Base9PT", alvo));
        Infra.SalvaSaidaW(l6, Infra.NomeSaida("Base6PT", alvo));

        Infra.SalvaSaidaW(ars9, Infra.NomeSaida("Base9", alvo));
        Infra.SalvaSaidaW(ars6, Infra.NomeSaida("Base6", alvo));

        //           Infra.SalvaSaidaW(ars9Principais, Infra.NomeSaida("Base9Principais", alvo));

        Infra.SalvaSaidaW(ars6Principais, Infra.NomeSaida("Base6Principais", alvo));
        Infra.SalvaSaidaW(ints6, Infra.NomeSaida("Base6PrincipaisID", alvo));

        //          Infra.SalvaSaidaW(ars2, Infra.NomeSaida("Base9Seleta", alvo));
        //          Infra.SalvaSaidaW(ars3, Infra.NomeSaida("Base6Seleta", alvo));


        foreach (Lance o in ars9Principais) { o.LimpaListas(); }
        foreach (Lance o in ars6Principais) { o.LimpaListas(); }

        Infra.SalvaSaidaJson(ars9Principais, Infra.NomeJson("NineSeletos", alvo));
        Infra.SalvaSaidaJson(ars6Principais, Infra.NomeJson("SixSeletos", alvo));


        return ars;
    }
    public static Lances Estudo2(int alvo)
    {
        Lances ars2 = new Lances();
        Lances ar9 = new Lances();
        Lances ar6 = new Lances();

        Lances anteriores = new Lances();


        Infra.CombinarGeral();

        Lance oAnterior = Infra.arLoto[alvo - 3];
        Lance oAlvo = Infra.arLoto[alvo - 2];

        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);
        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        int s = alvo - 3;
        while (anteriores.Count < 10)
        {
            Lance o = Infra.arLoto[s];
            if (Infra.Contapontos(o, oAlvo) == 9) { anteriores.Add(o); }

            s--;
        }

        Lances arGeral1 = Infra.DevolveBaseGeralFiltrada(Infra.arGeral, oAlvo, 9);

        Lances arGeral = Infra.DevolveBaseAleatoria(arGeral1, 10000);
        Lances ars1 = Infra.DevolveBaseAleatoria(Infra.arGeral, 10000);

        foreach (Lance z in anteriores)
        {

            Lance Encontrado9 = ars9.FirstOrDefault(o => Infra.Contapontos(o, z) == 9);
            Lance Encontrado6 = ars6.FirstOrDefault(o => Infra.Contapontos(o, z) == 6);

            Lances osPares9 = new(); osPares9.AddRange(ars9.Where(o => Infra.Contapontos(o, Encontrado9) == 8).ToList());
            Lances osPares6 = new(); osPares6.AddRange(ars6.Where(o => Infra.Contapontos(o, Encontrado6) == 5).ToList());

            osPares9.Insert(0, Encontrado9);
            osPares6.Insert(0, Encontrado6);

            List<int> l9 = Enumerable.Repeat(0, osPares9.Count).ToList();
            List<int> l6 = Enumerable.Repeat(0, osPares6.Count).ToList();

            foreach (Lance o in arGeral)
            {
                foreach (Lance p in osPares9)
                {
                    if (Infra.Contapontos(o, p) == 9)
                    {
                        foreach (Lance q in osPares6)
                        {
                            if (Infra.Contapontos(o, q) == 6)
                            { ar9.Add(p); ar6.Add(q); break; }
                        }
                    }
                }
            }

            string s1 = "Pares9-v" + z.Id.ToString();
            string s2 = "Pares6-v" + z.Id.ToString();
            //string s3 = "Saida6-v" + z.Id.ToString();
            //string s4 = "Saida9-v" + z.Id.ToString();

            //Infra.SalvaSaidaW(osPares9, Infra.NomeSaida(s1, alvo));
            //Infra.SalvaSaidaW(osPares6, Infra.NomeSaida(s2, alvo));

        }

        Infra.SalvaSaidaW(ars6, Infra.NomeSaida("Lista6", alvo));
        Infra.SalvaSaidaW(ars9, Infra.NomeSaida("Lista9", alvo));

        Infra.SalvaSaidaW(ar6, Infra.NomeSaida("Saida6", alvo));
        Infra.SalvaSaidaW(ar9, Infra.NomeSaida("Saida9", alvo));

        return arGeral;
    }


    public static Lances Estudo3(Lances arLotoTreino)
    {
        Lances ars1 = new Lances();
        Lances anteriores = new Lances();

        // Obter subgrupos de 9 e 6 a partir dos sorteios

        Treinamento(arLotoTreino);

        return ars1;
    }

    private static void Treinamento(Lances arLotoTreino)
    {
        Lance oAlvo = arLotoTreino[0];
        Lances ars9 = GerarCombinacoes.Combinar15a9(arLotoTreino[0].Lista);
        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        // Inicializar dicionários de contagem
        Dictionary<int, Dictionary<int, int>> contagemSS = ars9.ToDictionary(lance => lance.Id, lance => Enumerable.Range(3, 7).ToDictionary(i => i, i => 0));
        Dictionary<int, Dictionary<int, int>> contagemNS = ars6.ToDictionary(lance => lance.Id, lance => Enumerable.Range(2, 5).ToDictionary(i => i, i => 0));

        int ix = arLotoTreino.Count - 1;
        int ax = ix - 1;

        while ((ix > 1) && (ax > 0))
        {
            Lance o = arLotoTreino[ix];
            Lance p = arLotoTreino[ax];

            int m = Infra.Contapontos(o, p);

            if (m == 9)
            {
                Lances ars9Tmp = GerarCombinacoes.Combinar15a9(p.Lista);
                Lances ars6Tmp = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(p).Lista);

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

        // Salvar as contagens para análises futuras
        FileService fileService = new FileService();
        fileService.SalvarDados("ContagemSS.json", contagemSS);
        fileService.SalvarDados("ContagemNS.json", contagemNS);
    }


    #region MÉTODOS INATIVOS


    #endregion


    #region MÉTODOS AUXILIARES



    #endregion


}
