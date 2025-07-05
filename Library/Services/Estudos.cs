using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services;

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

        Lances arGeral = Infra.Aleatorios(arGeral1, 10000); // Corrigido: Usar Aleatorios
        Lances ars1 = Infra.Aleatorios(Infra.arGeral, 10000); // Corrigido: Usar Aleatorios

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



    public static Lances Estudo4(int alvo)
    {

        Infra.CombinarGeral();

        Lances ars = new();
        Lances ars2 = new();
        Lances ars3 = new();

        Lance oAlvo = Infra.arLoto[alvo - 2];
        Lance oposto = Infra.DevolveOposto(oAlvo);


        Random random = new Random();

        while (ars.Count < 10000)
        {
            Lances ars1 = new();

            while (ars1.Count < 5)
            {
                Lance a = Infra.arGeral[random.Next(Infra.arGeral.Count)];
                if (Infra.Contapontos(a, oAlvo) == 13) ars1.Add(a);
            }

            ars.Add(Infra.DevolveMenosFrequentes(ars1, 4));
        }

        while (ars3.Count < 10000)
        {
            Lances ars4 = new();

            while (ars4.Count < 5)
            {
                Lance a = Infra.arGeral[random.Next(Infra.arGeral.Count)];
                if (Infra.Contapontos(a, oposto) == 7) ars4.Add(a);
            }

            ars3.Add(Infra.DevolveMenosFrequentes(ars4, 4));
        }

        while (ars2.Count < 1000) ars2.Add(Infra.arGeral[random.Next(Infra.arGeral.Count)]);



        Infra.SalvaSaidaW(ars3, Infra.NomeSaida("ListaEstudoM", alvo));
        Infra.SalvaSaidaW(ars2, Infra.NomeSaida("Geral", alvo));

        return ars;
    }

    public static Lances Estudo5(int alvo)
    {

        Infra.CombinarGeral();

        Lances ars = new();
        Lances ars2 = new();
        Lances ars3 = new();

        Lance oAlvo = Infra.arLoto[alvo - 2];
        Lance oposto = Infra.DevolveOposto(oAlvo);


        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);




        Random random = new Random();

        while (ars.Count < 10000)
        {
            Lances ars1 = new();

            while (ars1.Count < 5)
            {
                Lance a = Infra.arGeral[random.Next(Infra.arGeral.Count)];
                if (Infra.Contapontos(a, oAlvo) == 13) ars1.Add(a);
            }

            ars.Add(Infra.DevolveMenosFrequentes(ars1, 4));
        }

        while (ars3.Count < 10000)
        {
            Lances ars4 = new();

            while (ars4.Count < 5)
            {
                Lance a = Infra.arGeral[random.Next(Infra.arGeral.Count)];
                if (Infra.Contapontos(a, oposto) == 7) ars4.Add(a);
            }

            ars3.Add(Infra.DevolveMenosFrequentes(ars4, 4));
        }

        while (ars2.Count < 1000) ars2.Add(Infra.arGeral[random.Next(Infra.arGeral.Count)]);

        List<double> l6 = new();
        List<double> l9 = new();





        foreach (Lance o in ars6)
        {
            var pontos = ars.Select(p => Infra.Contapontos(o, p)).ToList();
            int b = pontos.Count(x => x == 0);
            int c = pontos.Count(x => x == 4);

            double d = Math.Round((double)b / c * 100, 2);

            l6.Add(d);
        }

        foreach (Lance o in ars9)
        {
            var pontos = ars3.Select(p => Infra.Contapontos(o, p)).ToList();
            int b = pontos.Count(x => x == 0);
            int c = pontos.Count(x => x == 4);

            double d = Math.Round((double)b / c * 100, 2);
            l9.Add(d);
        }


        Infra.SalvaSaidaW(l6, Infra.NomeSaida("ListaEstudo-PT", alvo));
        Infra.SalvaSaidaW(l9, Infra.NomeSaida("ListaEstudoM-PT", alvo));

        Infra.SalvaSaidaW(ars9, Infra.NomeSaida("Lista9", alvo));
        Infra.SalvaSaidaW(ars6, Infra.NomeSaida("Lista6", alvo));

        Infra.SalvaSaidaW(ars3, Infra.NomeSaida("ListaEstudoM", alvo));
        Infra.SalvaSaidaW(ars2, Infra.NomeSaida("Geral", alvo));

        return ars;
    }

    //public static Lances Estudo6(int alvo)
    //{

    //    Infra.CombinarGeral();

    //    Lances ars = new();
    //    Lances ars1 = new();

    //    Lance oAlvo = Infra.arLoto[alvo - 2];
    //    Lance oposto = Infra.DevolveOposto(oAlvo);

    //    Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

    //    Lance complementar = Infra.DevolveComplementar(oposto, ars6[0]);


    //    Random random = new Random();


    //    foreach (Lance o in ars6)
    //    {
    //        List<int> ls = o.Lista;

    //        List<int> ln = complementar.Lista;

    //        for (int i = 1; i < 5; i++)
    //        {

    //            while (ars.Count < 1000)
    //            {

    //                Lance a = Infra.arGeral[random.Next(Infra.arGeral.Count)];
    //                int s = Infra.Contapontos(a, ls);
    //                int n = Infra.Contapontos(a, ln);




    //                if ((s == ls.Count) && (n == 0)) ars.Add(a);
    //            }



    //            Uvas lsa = Infra.DevolveMaisFrequentesUvas(ars, 25);


    //            int firstZeroIndex = lsa.FindIndex(u => u.Pt == 0);
    //            int last1000Index = lsa.FindLastIndex(u => u.Pt == 1000);

    //            int adx = (last1000Index != -1) ? last1000Index + 1 : -1;
    //            int ndx = (firstZeroIndex != -1) ? firstZeroIndex - 1 : -1;

    //            // Verificação de limites
    //            if (adx >= lsa.Count) adx = -1;
    //            if (ndx < 0) ndx = -1;


    //            Uva uva1 = lsa[adx];
    //            Uva uva2 = lsa[ndx];
    //            Uva uva3 = lsa[ndx - 1];

    //            ls.Add(uva3.Id);
    //            ls.Add(uva2.Id);
    //            ln.Add(uva1.Id);

    //            ars.Clear();

    //        }

    //        ars1.Add(new Lance(ars1.Count, ls));
    //    }

    //    Infra.SalvaSaidaW(ars, Infra.NomeSaida("Lista6Especial", alvo));

    //    return ars;
    //}


    public static Lances Estudo6(int alvo)
    {
        Infra.CombinarGeral();

        Lances ars = new();


        Lances resultados = new Lances();
        Lance lanceAlvo = Infra.arLoto[alvo - 2];
        Lance oposto = Infra.DevolveOposto(lanceAlvo);

        Random random = new Random();

        // Todas combinações de 6 números entre os 10 opostos
        Lances combinacoesBase = GerarCombinacoes.Combinar10a6(oposto.Lista);

        foreach (Lance combinacao in combinacoesBase)
        {
            List<int> ls = new List<int>(combinacao.Lista); // Inicia com 6 números
            List<int> ln = new List<int>(Infra.DevolveComplementar(oposto, combinacao).Lista); // 4 números

            for (int iteracao = 0; iteracao < 4; iteracao++)
            {
                Lances combinacoesValidas = FiltrarCombinacoes(ls, ln);

                //if (combinacoesValidas.Count == 0)
                //{
                //    // Registra falha no refinamento
                //   // Infra.LogErro($"Falha no refinamento para combinação {combinacao.Id}");
                //    break;
                //}

                Uvas frequencias = Infra.DevolveMaisFrequentesUvas(combinacoesValidas, 25);

                var expansao = SelecionarNumerosExpansao(frequencias, ls, ln);

                // Atualiza listas
                ls.AddRange(expansao.adicionarLs);
                ln.Add(expansao.adicionarLn);

                // Garante que não ultrapasse os limites
                if (ls.Count >= 15) break;
            }

            // Garante ter exatamente 15 números
            resultados.Add(new Lance(resultados.Count, ls.Count > 15 ? ls.Take(15).ToList() : ls));

            if (resultados.Count > 105) break;
        }


        while (ars.Count < 1000) ars.Add(Infra.arGeral[random.Next(Infra.arGeral.Count)]);


        Infra.SalvaSaidaW(ars, Infra.NomeSaida("Aleatorios", alvo));

        return resultados;
    }

    public static Lances Estudo7(int alvo)
    {
        Infra.CombinarGeral();

        Lances resultados = new Lances();
        Lance lanceAlvo = Infra.arLoto[alvo - 2];
        Lance oposto = Infra.DevolveOposto(lanceAlvo);

        // Todas combinações de 9 números entre os 10 opostos
        Lances combinacoesBase = GerarCombinacoes.Combinar15a9(lanceAlvo.Lista);

        foreach (Lance combinacao in combinacoesBase)
        {
            List<int> ls = new List<int>(combinacao.Lista); // Inicia com 9 números
            List<int> ln = new List<int>(Infra.DevolveComplementar(lanceAlvo, combinacao).Lista); // 6 números

            for (int iteracao = 0; iteracao < 4; iteracao++)
            {
                Lances combinacoesValidas = FiltrarCombinacoes(ls, ln);

                //if (combinacoesValidas.Count == 0)
                //{
                //    // Registra falha no refinamento
                //   // Infra.LogErro($"Falha no refinamento para combinação {combinacao.Id}");
                //    break;
                //}

                Uvas frequencias = Infra.DevolveMaisFrequentesUvas(combinacoesValidas, 25);

                var expansao = SelecionarNumerosExpansao(frequencias, ls, ln);

                // Atualiza listas
                ls.AddRange(expansao.adicionarLs);
                ln.Add(expansao.adicionarLn);

                // Garante que não ultrapasse os limites
                if (ls.Count >= 15) break;
            }

            // Garante ter exatamente 15 números
            resultados.Add(new Lance(resultados.Count, ls.Count > 15 ? ls.Take(15).ToList() : ls));
        }

        return resultados;
    }

    private static (List<int> adicionarLs, int adicionarLn) SelecionarNumerosExpansao(Uvas frequencias, List<int> ls, List<int> ln)
    {
        var classificados = new HashSet<int>(ls.Concat(ln));

        // Ordena por frequência (do menos frequente ao mais frequente)
        var candidatos = frequencias
            .Where(u => !classificados.Contains(u.Id))
            .OrderBy(u => u.Pt)
            .ToList();

        if (candidatos.Count < 3)
        {
            throw new InvalidOperationException("Números insuficientes para expansão");
        }

        // Dois números mais raros para LS
        var adicionarLs = new List<int> { candidatos[0].Id, candidatos[1].Id };

        // Número mais frequente não classificado para LN
        int adicionarLn = candidatos.Last().Id;

        return (adicionarLs, adicionarLn);
    }

    private static Lances FiltrarCombinacoes(List<int> ls, List<int> ln)
    {
        Lances resultado = new Lances();
        var setLs = new HashSet<int>(ls);
        var setLn = new HashSet<int>(ln);

        // Otimização: pré-computa o tamanho para verificação rápida
        int requiredLsCount = setLs.Count;

        foreach (Lance combinacao in Infra.arGeral)
        {
            // Verifica se contém todos os números de LS
            int contagemLs = 0;
            foreach (int num in combinacao.Lista)
            {
                if (setLs.Contains(num))
                    contagemLs++;
            }

            // Verifica se não contém nenhum número de LN
            bool contemLn = false;
            foreach (int num in combinacao.Lista)
            {
                if (setLn.Contains(num))
                {
                    contemLn = true;
                    break;
                }
            }

            if (contagemLs == requiredLsCount && !contemLn)
            {
                resultado.Add(combinacao);
            }
        }

        return resultado;
    }



    public static Lances Estudo11(int alvo)
    {

        Infra.CombinarGeral();

        Lances ars = new();
        Lances ars1 = new();
        Lances ars2 = new();
        Lances ars3 = new();


        Lance oAlvo = Infra.arLoto[alvo - 2];


        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        Lances arBase = Infra.DevolveBaseCombinada(ars9, ars6);


        string arq1 = "SixSeletos-" + alvo.ToString();

        Lances arJson = Infra.AbrirArquivoJson(arq1);



        Random random = new Random();

        int ax = 1000;

        while (ars.Count < 2000)
        {

            List<int> l9 = Enumerable.Repeat(0, ars9.Count).ToList();
            List<int> l6 = Enumerable.Repeat(0, ars6.Count).ToList();

            Lances lances = Infra.DevolveBaseAleatoria(arBase, ax);

            foreach (Lance a in lances) { l9[a.M]++; l6[a.N]++; }

            Uvas uvas9 = new();
            Uvas uvas6 = new();

            foreach (int i in l9) { uvas9.Add(new Uva(uvas9.Count, i)); }
            foreach (int i in l6) { uvas6.Add(new Uva(uvas6.Count, i)); }

            uvas9.Sort();
            uvas6.Sort();

            int uv9 = uvas9[0].Id;
            int uv6 = uvas6[0].Id;

            ars.Add(ars9[uv9]);
            ars1.Add(ars6[uv6]);


        }



        Infra.SalvaSaidaW(ars, Infra.NomeSaida("Lista9", alvo));
        Infra.SalvaSaidaW(ars1, Infra.NomeSaida("Lista6", alvo));



        return ars;
    }

    public static Lances Estudo12(int alvo)
    {

        Infra.CombinarGeral();

        Lances ars = new();
        Lances ars1 = new();
        Lances ars2 = new();
        Lances ars3 = new();


        Lance oAlvo = Infra.arLoto[alvo - 2];


        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        Lances arBase = Infra.DevolveBaseCombinada(ars9, ars6);


        string arq1 = "SixSeletos-" + alvo.ToString();

        Lances arJson = Infra.AbrirArquivoJson(arq1);

        List<int> l9 = Enumerable.Repeat(0, ars9.Count).ToList();
        List<int> l6 = Enumerable.Repeat(0, ars6.Count).ToList();

        Random random = new Random();

        int ax = 1000;



        Lances lances = Infra.DevolveBaseAleatoria(arBase, ax);

        Lance oMaisFrequente = Infra.DevolveMaisFrequentes(lances, 15);

        Lance oGrupoNNmaisFrequente = ars6.FirstOrDefault(o => Infra.Contapontos(o, oMaisFrequente) == 6);


        foreach (Lance a in lances) { l9[a.M]++; l6[a.N]++; }

        List<int> l6paraContagem = lances.Select(o => Infra.Contapontos(o, oGrupoNNmaisFrequente)).ToList();


        List<int> l6Posicao = ars6.Select(q => lances.Select((o, i) => Infra.Contapontos(o, q) - l6paraContagem[i]).Sum()).ToList();
        //
        //List<int> l6PosicaoPeso = ars6.Select(q => lances.Select((o, i) => Infra.Contapontos(o, q)).Sum()).ToList();

        List<int> l6PosicaoPeso = new List<int>();

        foreach (Lance q in ars6)
        {
            int soma = 0;
            foreach (Lance o in lances)
            {
                soma += Infra.Contapontos(o, q);
            }
            l6PosicaoPeso.Add(soma);
        }

        List<int> LTPosicaoMult = new List<int>();

        foreach (Lance q in lances)
        {
            int soma = 0;
            foreach (Lance o in ars6)
            {
                soma += (Infra.Contapontos(o, q) * l6[o.Id]);
            }

            foreach (Lance o in ars9)
            {
                soma += (Infra.Contapontos(o, q) * l9[o.Id]);
            }
            LTPosicaoMult.Add(soma);
        }


        foreach (Lance a in lances)
        {
            l9[a.M]++;
            l6[a.N]++;

            List<int> list = new List<int>();
            list.Add(a.M);
            list.Add(a.N);

            ars.Add(new Lance(ars.Count, list));


        }



        //for (int i = 0; i < l6.Count - 1; i++)
        //{
        //    l6PosicaoPeso.Add(l6Posicao[i] * l6[i]);
        //}






        Infra.SalvaSaidaW(ars9, Infra.NomeSaida("Lista9", alvo));
        Infra.SalvaSaidaW(ars6, Infra.NomeSaida("Lista6", alvo));

        Infra.SalvaSaidaW(l9, Infra.NomeSaida("Lista9PT", alvo));
        Infra.SalvaSaidaW(l6, Infra.NomeSaida("Lista6PT", alvo));
        Infra.SalvaSaidaW(l6Posicao, Infra.NomeSaida("Lista6Posicao", alvo));
        Infra.SalvaSaidaW(l6PosicaoPeso, Infra.NomeSaida("Lista6PosicaoPeso", alvo));
        Infra.SalvaSaidaW(LTPosicaoMult, Infra.NomeSaida("ListaTPosicaoMulti", alvo));


        Infra.SalvaSaidaW(ars, Infra.NomeSaida("Mistura", alvo));



        return lances;
    }

    public static Lances Estudo13(int alvo)
    {

        Infra.CombinarGeral();

        Lances ars = new();
        Lances ars1 = new();
        Lances ars2 = new();
        Lances ars3 = new();


        Lance oAlvo = Infra.arLoto[alvo - 2];


        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        Lances arBase = Infra.DevolveBaseCombinada(ars9, ars6);


        List<int> l9 = Enumerable.Repeat(0, ars9.Count).ToList();
        List<int> l6 = Enumerable.Repeat(0, ars6.Count).ToList();



        Random random = new Random();

        int ax = 1000;
        Lances lances = Infra.DevolveBaseAleatoria(arBase, ax);

        ParesXY paresXY = new ParesXY();

        foreach (Lance o in lances)
        {
            paresXY.Add(new ParXY(o.M, o.N));

            foreach (Lance p in ars9)
            {
                if (Infra.Contapontos(o, p) == 9)
                {
                    p.X++;
                    break;
                }
            }

            foreach (Lance p in ars6)
            {
                if (Infra.Contapontos(o, p) == 6)
                {
                    p.Y++;
                    break;
                }
            }
        }


        Lance oXmaisFrequente = ars9.ObterLanceMaiorX();
        Lance oYmaisFrequente = ars6.ObterLanceMaiorY();


        Lances avaliaY5 = new(); avaliaY5.AddRange(ars6.Where(o => Infra.Contapontos(o, oYmaisFrequente) == 5).ToList());
        Lances avaliaY4 = new(); avaliaY4.AddRange(ars6.Where(o => Infra.Contapontos(o, oYmaisFrequente) == 4).ToList());
        Lances avaliaY3 = new(); avaliaY3.AddRange(ars6.Where(o => Infra.Contapontos(o, oYmaisFrequente) == 3).ToList());
        Lances avaliaY2 = new(); avaliaY2.AddRange(ars6.Where(o => Infra.Contapontos(o, oYmaisFrequente) == 2).ToList());


        Lances drs5 = new();
        Lances drs4 = new();
        Lances drs3 = new();
        Lances drs2 = new();

        int ix = oYmaisFrequente.Id;

        foreach (ParXY o in paresXY)
        {

            //if (o.Y == ix) { drs5.Add(Infra.CriaLanceAPartirDoPar(ars9[o.X], oYmaisFrequente)); }
            //else
            //{ drs5.Add(Infra.CriaLanceAPartirDoPar(ars9[o.X], avaliaY5[random.Next(avaliaY5.Count)])); }


            drs5.Add(Infra.CriaLanceAPartirDoPar(ars9[o.X], o.Y == ix ? oYmaisFrequente : avaliaY5[random.Next(avaliaY5.Count)]));
            drs4.Add(Infra.CriaLanceAPartirDoPar(ars9[o.X], o.Y == ix ? oYmaisFrequente : avaliaY4[random.Next(avaliaY4.Count)]));
            drs3.Add(Infra.CriaLanceAPartirDoPar(ars9[o.X], o.Y == ix ? oYmaisFrequente : avaliaY3[random.Next(avaliaY3.Count)]));
            drs2.Add(Infra.CriaLanceAPartirDoPar(ars9[o.X], o.Y == ix ? oYmaisFrequente : avaliaY2[random.Next(avaliaY2.Count)]));

        }


        foreach (Lance a in lances)
        {
            l9[a.M]++;
            l6[a.N]++;

            List<int> list = new List<int>();
            list.Add(a.M);
            list.Add(a.N);


        }


        //Infra.SalvaSaidaW(drs5, Infra.NomeSaida("VersaoParaCinco", alvo));
        //Infra.SalvaSaidaW(drs4, Infra.NomeSaida("VersaoParaQuatro", alvo));
        //Infra.SalvaSaidaW(drs3, Infra.NomeSaida("VersaoParaTres", alvo));
        //Infra.SalvaSaidaW(drs2, Infra.NomeSaida("VersaoParaDois", alvo));

        Infra.SalvaSaidaW(ars9, Infra.NomeSaida("Lista9", alvo));
        Infra.SalvaSaidaW(ars6, Infra.NomeSaida("Lista6", alvo));

        Infra.SalvaSaidaW(l9, Infra.NomeSaida("Lista9PT", alvo));
        Infra.SalvaSaidaW(l6, Infra.NomeSaida("Lista6PT", alvo));

        return lances;
    }


    public static Lances Estudo14(int alvo)
    {

        Infra.CombinarGeral();

        Lances ars = new();


        Lance oAlvo = Infra.arLoto[alvo - 2];


        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        Lances arBase = Infra.DevolveBaseCombinada(ars9, ars6);


        List<int> l9 = Enumerable.Repeat(0, ars9.Count).ToList();


        Random random = new Random();

        int ax = 1000;
        Lances lances = Infra.DevolveBaseAleatoria(arBase, ax);

        ParesXY paresXY = new ParesXY();

        foreach (Lance o in lances)
        {
            paresXY.Add(new ParXY(o.M, o.N));

            foreach (Lance p in ars9)
            {
                if (Infra.Contapontos(o, p) == 9)
                {
                    p.X++;
                    break;
                }
            }

            foreach (Lance p in ars6)
            {
                if (Infra.Contapontos(o, p) == 6)
                {
                    p.Y++;
                    break;
                }
            }
        }


        Lance oXmaisFrequente = ars9.ObterLanceMaiorX();


        Lances avaliaX8 = new(); avaliaX8.AddRange(ars9.Where(o => Infra.Contapontos(o, oXmaisFrequente) == 8).ToList());
        Lances avaliaX7 = new(); avaliaX7.AddRange(ars9.Where(o => Infra.Contapontos(o, oXmaisFrequente) == 7).ToList());
        Lances avaliaX6 = new(); avaliaX6.AddRange(ars9.Where(o => Infra.Contapontos(o, oXmaisFrequente) == 6).ToList());
        Lances avaliaX5 = new(); avaliaX5.AddRange(ars9.Where(o => Infra.Contapontos(o, oXmaisFrequente) == 5).ToList());
        Lances avaliaX4 = new(); avaliaX4.AddRange(ars9.Where(o => Infra.Contapontos(o, oXmaisFrequente) == 4).ToList());
        Lances avaliaX3 = new(); avaliaX3.AddRange(ars9.Where(o => Infra.Contapontos(o, oXmaisFrequente) == 3).ToList());



        Lances drs8 = new(); Lances drs7 = new(); Lances drs6 = new(); Lances drs5 = new(); Lances drs4 = new(); Lances drs3 = new(); Lances drs2 = new();

        int ix = oXmaisFrequente.Id;

        foreach (ParXY o in paresXY)
        {

            drs8.Add(Infra.CriaLanceAPartirDoPar(ars6[o.Y], o.X == ix ? oXmaisFrequente : avaliaX8[random.Next(avaliaX8.Count)]));
            drs7.Add(Infra.CriaLanceAPartirDoPar(ars6[o.Y], o.X == ix ? oXmaisFrequente : avaliaX7[random.Next(avaliaX7.Count)]));
            drs6.Add(Infra.CriaLanceAPartirDoPar(ars6[o.Y], o.X == ix ? oXmaisFrequente : avaliaX6[random.Next(avaliaX6.Count)]));
            drs5.Add(Infra.CriaLanceAPartirDoPar(ars6[o.Y], o.X == ix ? oXmaisFrequente : avaliaX5[random.Next(avaliaX5.Count)]));
            drs4.Add(Infra.CriaLanceAPartirDoPar(ars6[o.Y], o.X == ix ? oXmaisFrequente : avaliaX4[random.Next(avaliaX4.Count)]));
            drs3.Add(Infra.CriaLanceAPartirDoPar(ars6[o.Y], o.X == ix ? oXmaisFrequente : avaliaX3[random.Next(avaliaX3.Count)]));

        }


        foreach (Lance a in lances)
        {
            l9[a.M]++;

            List<int> list = new List<int>();
            list.Add(a.M);

        }


        Infra.SalvaSaidaW(drs8, Infra.NomeSaida("SuperParaOito", alvo));
        Infra.SalvaSaidaW(drs7, Infra.NomeSaida("SuperParaSete", alvo));
        Infra.SalvaSaidaW(drs6, Infra.NomeSaida("SuperParaSeis", alvo));
        Infra.SalvaSaidaW(drs5, Infra.NomeSaida("SuperParaCinco", alvo));
        Infra.SalvaSaidaW(drs4, Infra.NomeSaida("SuperParaQuatro", alvo));
        Infra.SalvaSaidaW(drs3, Infra.NomeSaida("SuperParaTres", alvo));



        Infra.SalvaSaidaW(l9, Infra.NomeSaida("Lista9PT", alvo));

        return lances;
    }

    public static Lances Estudo17(int alvo)
    {

        Infra.CombinarGeral();

        Lances arsOMaisVisado = new();
        Lances arsOPerfilMaisFrequente = new();

        Lance oAlvo = Infra.arLoto[alvo - 2];

        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        Lances arBase = Infra.DevolveBaseCombinada(ars9, ars6);

        Random random = new Random();

        int ax = 1000;

        for (int h = 0; h < 100; h++)
        {
            Lances lances = Infra.DevolveBaseAleatoria(arBase, ax);

            ParesXY paresXY = new ParesXY();

            foreach (Lance o in lances)
            {
                paresXY.Add(new ParXY(o.M, o.N));

                foreach (Lance p in ars9)
                {
                    if (Infra.Contapontos(o, p) == 9)
                    {
                        p.X++;
                        break;
                    }
                }

                foreach (Lance p in ars6)
                {
                    if (Infra.Contapontos(o, p) == 6)
                    {
                        p.Y++;
                        break;
                    }
                }
            }

            Lance oXmaisFrequente = ars9.ObterLanceMaiorX();
            Lance oYmaisFrequente = ars6.ObterLanceMaiorY();


            Lances avaliaY3 = new(); avaliaY3.AddRange(ars6.Where(o => Infra.Contapontos(o, oYmaisFrequente) == 4).ToList());

            Lances drs3 = new();

            int ix = oYmaisFrequente.Id;

            foreach (ParXY o in paresXY)
            {
                drs3.Add(Infra.CriaLanceAPartirDoPar(ars9[o.X], o.Y == ix ? oYmaisFrequente : avaliaY3[random.Next(avaliaY3.Count)]));
            }

            Lance OmaisVisado = Infra.DevolveMaisFrequentes(drs3, 4);

            arsOMaisVisado.Add(OmaisVisado);
            arsOPerfilMaisFrequente.Add(oYmaisFrequente);

            ars9.LimpaXY();
            ars6.LimpaXY();

        }

        Infra.SalvaSaidaW(arsOMaisVisado, Infra.NomeSaida("OsMaisVisados", alvo));
        Infra.SalvaSaidaW(arsOPerfilMaisFrequente, Infra.NomeSaida("OsPerfisMaisFrequentes", alvo));


        return new Lances();
    }

    public static Lances Estudo18(int alvo)
    {

        Infra.CombinarGeral();

        Lances arsOMaisVisado = new();
        Lances arsOPerfilMaisFrequente = new();

        Lance oAlvo = Infra.arLoto[alvo - 2];

        Lances ars9 = GerarCombinacoes.Combinar15a9(oAlvo.Lista);

        Lances ars6 = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

        Lances arBase = Infra.DevolveBaseCombinada(ars9, ars6);

        Random random = new Random();

        int ax = 1000;

        for (int h = 0; h < 100; h++)
        {
            Lances lances = Infra.DevolveBaseAleatoria(arBase, ax);

            ParesXY paresXY = new ParesXY();

            foreach (Lance o in lances)
            {
                paresXY.Add(new ParXY(o.M, o.N));

                foreach (Lance p in ars9)
                {
                    if (Infra.Contapontos(o, p) == 9)
                    {
                        p.X++;
                        break;
                    }
                }

                foreach (Lance p in ars6)
                {
                    if (Infra.Contapontos(o, p) == 6)
                    {
                        p.Y++;
                        break;
                    }
                }
            }

            Lance oXmaisFrequente = ars9.ObterLanceMaiorX();
            Lance oYmaisFrequente = ars6.ObterLanceMaiorY();


            Lances avaliaX6 = new(); avaliaX6.AddRange(ars9.Where(o => Infra.Contapontos(o, oXmaisFrequente) == 6).ToList());


            Lances drs6 = new();

            int ix = oXmaisFrequente.Id;

            foreach (ParXY o in paresXY)
            {
                drs6.Add(Infra.CriaLanceAPartirDoPar(ars6[o.Y], o.X == ix ? oXmaisFrequente : avaliaX6[random.Next(avaliaX6.Count)]));
            }

            Lance OmaisVisado = Infra.DevolveMaisFrequentes(drs6, 6);

            arsOMaisVisado.Add(OmaisVisado);
            arsOPerfilMaisFrequente.Add(oXmaisFrequente);

            ars9.LimpaXY();
            ars6.LimpaXY();

        }

        Infra.SalvaSaidaW(arsOMaisVisado, Infra.NomeSaida("OsSupersMaisVisados", alvo));
        Infra.SalvaSaidaW(arsOPerfilMaisFrequente, Infra.NomeSaida("OsSupersComPerfisMaisFrequentes", alvo));


        return new Lances();
    }

    #region MÉTODOS AUXILIARES

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

        while (ix > 1 && ax > 0)
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
                    Lances encontrados9 = ars9Tmp.Where(q => Infra.Contapontos(q, o) == h).ToLances1();
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
                    Lances encontrados6 = ars6Tmp.Where(q => Infra.Contapontos(q, o) == h).ToLances1();
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


    #endregion


}
