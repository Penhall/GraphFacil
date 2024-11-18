using LotoLibrary.Constantes;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Busisness
{
    public static class Estudos
    {

        public static Lances Estudo1(int alvo)
        {

            Infra.CombinarGeral();

            Lances ars = new();
            Lances ars1 = new();
            Lances ars2 = new();
            Lances ars3 = new();

            Lances arGeral = Infra.DevolveBaseAleatoria(10000);



            Lance oAlvo = Infra.arLoto[alvo-2];

            Lances arBase = Infra.DevolveBaseCompleta(oAlvo, 9);

            Lances ars9 = Infra.Combinar15a9(oAlvo.Lista);

            Lances ars6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            string arq1 = "SixSeletos-"+alvo.ToString();

            Lances arJson = Infra.AbrirArquivoJson(arq1);

            List<int> l9 = Enumerable.Repeat(0, ars9.Count).ToList();
            List<int> l6 = Enumerable.Repeat(0, ars6.Count).ToList();

            Random random = new Random();

            int ax = 200000;

            while (ars.Count<10000)
            {

                Lance a = arBase[ax];
                // Lance a = arBase[random.Next(arBase.Count)];

                List<int> list = new List<int>();

                foreach (Lance o in ars9) { if (Infra.Contapontos(o, a)==9) { list.Add(o.Id); l9[o.Id]++; a.M=o.Id; break; } }
                foreach (Lance o in ars6) { if (Infra.Contapontos(o, a)==6) { list.Add(o.Id); l6[o.Id]++; a.N=o.Id; break; } }
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

            Lance oAnterior = Infra.arLoto[alvo-3];
            Lance oAlvo = Infra.arLoto[alvo-2];

            Lances ars9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances ars6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            int s = alvo-3;
            while (anteriores.Count<10)
            {
                Lance o = Infra.arLoto[s];
                if (Infra.Contapontos(o, oAlvo)==9) { anteriores.Add(o); }

                s--;
            }

            Lances arGeral1 = Infra.DevolveBaseGeralFiltrada(Infra.arGeral, oAlvo, 9);

            Lances arGeral = Infra.DevolveBaseAleatoria(arGeral1, 10000);
            Lances ars1 = Infra.DevolveBaseAleatoria(Infra.arGeral, 10000);

            foreach (Lance z in anteriores)
            {

                Lance Encontrado9 = ars9.FirstOrDefault(o => Infra.Contapontos(o, z)==9);
                Lance Encontrado6 = ars6.FirstOrDefault(o => Infra.Contapontos(o, z)==6);

                Lances osPares9 = new(); osPares9.AddRange(ars9.Where(o => Infra.Contapontos(o, Encontrado9)==8).ToList());
                Lances osPares6 = new(); osPares6.AddRange(ars6.Where(o => Infra.Contapontos(o, Encontrado6)==5).ToList());

                osPares9.Insert(0, Encontrado9);
                osPares6.Insert(0, Encontrado6);

                List<int> l9 = Enumerable.Repeat(0, osPares9.Count).ToList();
                List<int> l6 = Enumerable.Repeat(0, osPares6.Count).ToList();

                foreach (Lance o in arGeral)
                {
                    foreach (Lance p in osPares9)
                    {
                        if (Infra.Contapontos(o, p)==9)
                        {
                            foreach (Lance q in osPares6)
                            {
                                if (Infra.Contapontos(o, q)==6)
                                { ar9.Add(p); ar6.Add(q); break; }
                            }
                        }
                    }
                }

                string s1 = "Pares9-v"+z.Id.ToString();
                string s2 = "Pares6-v"+z.Id.ToString();
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

        public static Lances Estudo3(int alvo)
        {
            Lances ars1 = new Lances();


            Lances anteriores = new Lances();

            Lance oAnterior = Infra.arLoto[alvo-3];
            Lance oAlvo = Infra.arLoto[alvo-2];

            Lances ars9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances ars6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            List<int> L9 = Enumerable.Repeat(0, 5005).ToList();
            List<int> L6 = Enumerable.Repeat(0, 210).ToList();

            Infra.CombinarGeral();

            int ix = Infra.arLoto.Count-1;
            int ax = ix-1;

            while ((ix>1)&&(ax>0))
            {
                Lance o = Infra.arLoto[ix];
                Lance p = Infra.arLoto[ax];

                int m = Infra.Contapontos(o, p);

                if (m==9)
                {
                    Lances ars9Tmp = Infra.Combinar15a9(p.Lista);
                    Lances ars6Tmp = Infra.Combinar10a6(Infra.DevolveOposto(p).Lista);

                    Lances Encontrado9 = ars9Tmp.Where(q => Infra.Contapontos(q, o)==6).ToLances();
                    Lances Encontrado6 = ars6Tmp.Where(q => Infra.Contapontos(q, o)==4).ToLances();

                    foreach (Lance z in Encontrado9) { L9[z.Id]++; }
                    foreach (Lance z in Encontrado6) { L6[z.Id]++; }

                    ix--;
                    ax=ix-1;


                }
                else
                { ax--; }

            }

            Infra.SalvaSaidaW(L9, Infra.NomeSaida("ContagemPara9", alvo));
            Infra.SalvaSaidaW(L6, Infra.NomeSaida("ContagemPara6", alvo));

            Infra.SalvaSaidaW(ars9, Infra.NomeSaida("Base9", alvo));
            Infra.SalvaSaidaW(ars6, Infra.NomeSaida("Base6", alvo));


            return ars1;
        }

        public static Lances Estudo4(int alvo)
        {
            Lances ars = new Lances();
            Lances ars1 = new Lances();


            Lances anteriores = new Lances();

            Lance oAlvo = Infra.arLoto[alvo-2];

            while (ars.Count<5000)
            {

                if (ars.Count==0)
                {
                    Lances Encontrado9 = Infra.arLoto.Where(q => Infra.Contapontos(q, oAlvo)==9).ToLances();

                    Lance a = Infra.DevolveMaisFrequentes(Encontrado9, 15);

                    ars.Add(a);
                }
                else
                {
                    Lance b = ars[ars.Count-1];

                    Lances Encontrado9 = Infra.arLoto.Where(q => Infra.Contapontos(q, b)==9).ToLances();

                    Lance a = Infra.DevolveMaisFrequentes(Encontrado9, 15);

                    ars.Add(a);
                }

            }



            return ars;
        }

        public static Lances Estudo5(int alvo)
        {

            Lances ar6 = new Lances();

            List<int> ar = new List<int>() { 2, 6, 7, 8, 15, 16, 17, 19, 21, 23 };


            Lances ars6 = Infra.Combinar10a6(ar);

            List<int> l6 = Enumerable.Repeat(0, ars6.Count).ToList();

            foreach (Lance z in Infra.arLoto)
            {

                if (Infra.Contapontos(z, ar)==6)
                {
                    Lance Encontrado6 = ars6.FirstOrDefault(o => Infra.Contapontos(o, z)==6);
                    l6[Encontrado6.Id]++;
                }
            }

            Infra.SalvaSaidaW(l6, Infra.NomeSaida("Lista6", alvo));


            return ars6;
        }

        public static Lances Estudo6(int alvo)
        {

            Infra.CombinarGeral();

            Lance Prime = new(0, Constante.Prime);


            Lance oAlvo = Infra.arLoto[alvo-2];
            Lance Anterior = Infra.arLoto[alvo-3];

            Lances arBase9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances arBase6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            Lance Encontrado9 = arBase9.FirstOrDefault(o => Infra.Contapontos(o, Anterior)==9);
            Lance Encontrado6 = arBase6.FirstOrDefault(o => Infra.Contapontos(o, Anterior)==6);

            Lances ars9 = arBase9.Where(o => Infra.Contapontos(o, Encontrado9)==5).ToLances();
            Lances ars6 = arBase6.Where(o => Infra.Contapontos(o, Encontrado6)==4).ToLances();


            Random random = new Random();

            Lances ars1 = Infra.CombinaLances(ars9, ars6);

            Lance principais = Infra.DevolveMaisFrequentes(ars1, 21);

            Lances ars8TMP = Infra.Combinar21a8(principais);

            List<int> L1 = new List<int>() { 2, 6, 7, 13, 21, 24 };
            List<int> L2 = new List<int>() { 8, 9, 10, 16, 19, 25 };
            List<int> L3 = new List<int>() { 1, 4, 5, 11, 12, 15, 17, 18, 20 };

            Lances ars8 = new();
            foreach (Lance o in ars8TMP)
            {
                int a = Infra.Contapontos(o, L1);
                int b = Infra.Contapontos(o, L2);
                int c = Infra.Contapontos(o, L3);

                if ((a==2)&&(b==2)&&(c==4)) ars8.Add(o);
            }

            Lances ars2 = new();
            while (ars2.Count<10000)
            {
                ars2.Add(ars1[random.Next(ars1.Count)]);
            }

            Lances ars3 = new();
            foreach (Lance lance in ars8)
            {
                Lances tmp = ars1.Where(o => Infra.Contapontos(o, lance)==0).ToLances();

                if (tmp.Count>0) { ars3.Add(lance); }
            }

            Lances ars4 = new();


            foreach (Lance lance in ars3) { ars4.Add(Infra.DevolveComplementar(Prime, lance)); }

            Lances ars5 = new();

            while (ars5.Count<1000) { ars5.Add(ars4[random.Next(ars4.Count)]); }

            Infra.SalvaSaidaW(ars3, Infra.NomeSaida("Controle", alvo));
            Infra.SalvaSaidaW(ars4, Infra.NomeSaida("Prime", alvo));
            Infra.SalvaSaidaW(ars5, Infra.NomeSaida("Prime1000", alvo));


            return ars2;

        }
        public static Lances Estudo7(int alvo)
        {
            Lances ars = new();
            Lances ars1 = new();
            Lances ars2 = new();

            List<int> pesos = new() { 2, 15, 38, 34, 11, 1, 0 };

            Infra.CombinarGeral();


            Lances arGeral = Infra.Aleatorios(Infra.arGeral, 10000);

            Lances arBasesAnteriores = new();

            Random random = new Random();



            Lance oAlvo = Infra.arLoto[alvo-2];
            //  Lance oCerto = Infra.arLoto[alvo - 1];

            Lances arBase9 = Infra.Combinar15a9(oAlvo.Lista);

            Lances arBase9Complementar = new Lances();
            foreach (Lance o in arBase9) arBase9Complementar.Add(Infra.DevolveOposto(o));


            Lances arBase6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            int i = alvo-2;
            while (arBasesAnteriores.Count<70) { Lance a = Infra.arLoto[i]; int b = Infra.Contapontos(a, oAlvo); if (b==9) arBasesAnteriores.Add(a); i--; }

            foreach (Lance o in arBase9)
            {
                List<int> ints = new();

                foreach (Lance p in arBasesAnteriores)
                {
                    int x = Infra.Contapontos(o, p);
                    int y = pesos[x-3];

                    if (y==0) ars1.Add(o);

                    ints.Add(y);
                }
                ars.Add(new Lance(ars.Count, ints));
            }




            foreach (Lance o in arBase9)
            {
                int s = 0;
                foreach (Lance p in ars1)
                {
                    if (Infra.Contapontos(o, p)==5) s++; break;
                }
                if (s==0) ars2.Add(o);
            }




            Infra.SalvaSaidaW(ars1, Infra.NomeSaida("Seleta9", alvo));
            Infra.SalvaSaidaW(ars2, Infra.NomeSaida("Exilados", alvo));




            Infra.SalvaSaidaW(arBase9, Infra.NomeSaida("Base9", alvo));
            Infra.SalvaSaidaW(arBase9Complementar, Infra.NomeSaida("Base9Complementar", alvo));
            Infra.SalvaSaidaW(arBase6, Infra.NomeSaida("Base6", alvo));
            Infra.SalvaSaidaW(arGeral, Infra.NomeSaida("Controle", alvo));

            return ars;

        }
        public static Lances Estudo8(int alvo)
        {
            Lances ars = new();
            Lances ars1 = new();

            Infra.CombinarGeral();


            Lance oCerto = Infra.arLoto[alvo-1];
            Lance oAlvo = Infra.arLoto[alvo-2];
            Lance oAnterior = Infra.arLoto[alvo-3];

            Lance oControle1 = Infra.arLoto[1699];
            Lance oControle2 = Infra.arLoto[1098];



            Lance oposto = Infra.DevolveOposto(oAlvo);

            Lances arBase9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances arBase6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            Lance Encontrado9tmp = arBase9.FirstOrDefault(o => Infra.Contapontos(o, oAnterior)==9);
            Lance Encontrado6tmp = arBase6.FirstOrDefault(o => Infra.Contapontos(o, oAnterior)==6);

            int oEncontrado9Id = Encontrado9tmp.Id;
            int oEncontrado6Id = Encontrado6tmp.Id;
            //int oEncontradoId = 8;

            Lance Encontrado9 = arBase9[oEncontrado9Id];
            Lance Encontrado6 = arBase6[oEncontrado6Id];

            Lance oSense = Infra.UnirLances(Encontrado9, Encontrado6);


            Lances osMelhores = Infra.arGeral.Where(o => Infra.Contapontos(o, oSense)==5).ToLances();
            Lances osMelhoresRef = Infra.arGeral.Where(o => Infra.Contapontos(o, oCerto)==5).ToLances();

            Lances osMelhoresRef1 = Infra.arGeral.Where(o => Infra.Contapontos(o, oControle1)==5).ToLances();
            Lances osMelhoresRef2 = Infra.arGeral.Where(o => Infra.Contapontos(o, oControle2)==5).ToLances();


            //Lance Encontrado9Comp = Infra.DevolveComplementar(oAlvo, Encontrado9);
            //Lance Encontrado6Comp = Infra.DevolveComplementar(oposto, Encontrado6);


            ////     Lance Encontrado6Comp = Infra.DevolveComplementar(oposto, Encontrado6);

            //Lances Encontrado9Base = Infra.Combinar6a4(Encontrado9Comp);


            //Lances Encontrado6Base = Infra.Combinar6a4(Encontrado6);

            //          foreach (Lance o in Encontrado9Base) { o.ComplementarPara(Encontrado9Comp); }
            //foreach (Lance o in Encontrado6Base) { o.ComplementarPara(Encontrado6); }



            //while (ars1.Count < 15)
            //{
            //    List<int> encontrado = new List<int>();

            //    Lances arGeral = Infra.Aleatorios(Infra.arGeral, 10000);

            //    foreach (Lance o in Encontrado9Base)
            //    {
            //        Lances artmp = new();
            //        foreach (Lance p in arGeral)
            //        {
            //            if (Infra.Contapontos(p, oAlvo) == 9) { if (o.SomaComplementar(p)) artmp.Add(p); }

            //        }
            //        encontrado.Add(artmp.Count);
            //    }
            //    ars1.Add(new Lance(ars1.Count, encontrado));

            //}

            //Lances EncontradosComplementar = new();
            //foreach (Lance o in Encontrado9Base) { EncontradosComplementar.Add(new Lance(o.Id, o.ListaN)); }



            //while (ars1.Count < 100)
            //{
            //    Lances ars2 = new();
            //    Lances arGeral2 = Infra.Aleatorios(Infra.arGeral, 100000);

            //    foreach (Lance o in arGeral2)
            //    {
            //        int a = Infra.Contapontos(o, Encontrado9);
            //        int b = Infra.Contapontos(o, Encontrado9Comp);
            //        int c = Infra.Contapontos(o, Encontrado6);
            //        int d = Infra.Contapontos(o, Encontrado6Comp);

            //        if ((a == 5) && (b == 4) && (c == 4) && (d == 2)) ars2.Add(o);
            //    }

            //    ars1.Add(Infra.DevolveMaisFrequentes(ars2, 8));
            //}


            //Lances arGeral3 = Infra.Aleatorios(Infra.arGeral, 10000);


            Infra.SalvaSaidaW(arBase9, Infra.NomeSaida("Base9", alvo));
            Infra.SalvaSaidaW(arBase6, Infra.NomeSaida("Base6", alvo));

            //  Infra.SalvaSaidaW(encontrado, Infra.NomeSaida("Encontrado9BasePT", alvo));

            // Infra.SalvaSaidaW(ars1, Infra.NomeSaida("Encontrado9BasePT", alvo));

            //Infra.SalvaSaidaW(Encontrado9Base, Infra.NomeSaida("Encontrado9Base", alvo));
            //Infra.SalvaSaidaW(EncontradosComplementar, Infra.NomeSaida("Encontrado9BaseComplementar", alvo));


            //  Infra.SalvaSaidaW(ars1, Infra.NomeSaida("ControlFinal", alvo));
            Infra.SalvaSaidaW(osMelhores, Infra.NomeSaida("OsMelhores", alvo));
            Infra.SalvaSaidaW(osMelhoresRef, Infra.NomeSaida("OsMelhoresRef", alvo));

            Infra.SalvaSaidaW(osMelhoresRef1, Infra.NomeSaida("OsMelhoresRef1", alvo));
            Infra.SalvaSaidaW(osMelhoresRef2, Infra.NomeSaida("OsMelhoresRef2", alvo));
            //    Infra.SalvaSaidaW(arGeral3, Infra.NomeSaida("Controle", alvo));



            return ars;

        }
        public static Lances Estudo9(int alvo)
        {

            Lances ars3 = new();
            Lances ars4 = new();
            Lances ars5 = new();


            Infra.CombinarGeral();

            Lance oAnterior = Infra.arLoto[alvo-3];
            Lance oAlvo = Infra.arLoto[alvo-2];


            Lances arBase = Infra.DevolveBaseGeralFiltrada(Infra.arGeral, oAlvo, 9);


            Lances arBase9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances arBase6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);


            Lance Encontrado9tmp = arBase9.FirstOrDefault(o => Infra.Contapontos(o, oAnterior)==9);

            Lances aleatorios = Infra.Aleatorios(arBase, 5000);

            Lances osMelhores = aleatorios.Where(o => Infra.Contapontos(o, oAlvo)==9).ToLances();

            ConcurrentBag<Lance> ars = new ConcurrentBag<Lance>();
            ConcurrentBag<Lance> ars1 = new ConcurrentBag<Lance>();

            osMelhores.AsParallel().ForAll(o =>
            {
                bool condicaoA = osMelhores.Any(p => Infra.Contapontos(o, p)==5);
                bool condicaoB = osMelhores.Any(p => Infra.Contapontos(o, p)==13);

                if (!condicaoA) ars.Add(o);
                if (!condicaoB) ars1.Add(o);
            });

            // Se você precisar que o resultado esteja em uma List ao invés de ConcurrentBag

            Lances arsList = new();
            Lances ars1List = new();


            arsList.AddRange(ars.ToList());
            ars1List.AddRange(ars1.ToList());

            Lance oMelhor5 = Infra.DevolveMaisFrequentes(arsList, 15);
            Lance oMelhor13 = Infra.DevolveMaisFrequentes(ars1List, 15);

            Lances aleatorios2 = Infra.Aleatorios(arBase, 100000);

            ConcurrentBag<Lance> ars3Concurrent = new ConcurrentBag<Lance>();

            aleatorios2.AsParallel().ForAll(o =>
            {
                int a = Infra.Contapontos(o, oMelhor5);
                int b = Infra.Contapontos(o, oMelhor13);

                if (a==9&&b==9)
                {
                    ars3Concurrent.Add(o);
                }
            });

            ars3.AddRange(ars3Concurrent.ToList());


            //List<string> lix = new();


            //lix.AddRange(ars.Select(p => p.Nome).Distinct());

            //HashSet<string> lixSet = new HashSet<string>(lix);

            //// Usando PLINQ para processar em paralelo
            //var ars4Concurrent = new ConcurrentBag<Lance>(arBase.AsParallel().Where(o => !lixSet.Contains(o.Nome)));

            //// Se você precisar que o resultado esteja em uma List ao invés de ConcurrentBag
            //ars4.AddRange(ars4Concurrent.ToList());

            //Lances osMelhores = ars4.Where(o => Infra.Contapontos(o, oAnterior) == 7).ToLances();

            //Infra.SalvaSaidaW(osMelhores, Infra.NomeSaida("OsMelhores", alvo));
            //Infra.SalvaSaidaW(arsList, Infra.NomeSaida("OsMelhores5", alvo));
            //Infra.SalvaSaidaW(ars1List, Infra.NomeSaida("OsMelhores13", alvo));
            Infra.SalvaSaidaW(ars3, Infra.NomeSaida("Expressos", alvo));


            Infra.SalvaSaidaW(arBase9, Infra.NomeSaida("Base9", alvo));
            Infra.SalvaSaidaW(arBase6, Infra.NomeSaida("Base6", alvo));

            return aleatorios;

        }
        public static Lances Estudo10(int alvo)
        {


            Lances ars = new();
            Lances ars1 = new();

            Infra.CombinarGeral();

            Lance oAlvo = Infra.arLoto[alvo-2];


            Lances arBase1 = Infra.DevolveBaseGeralFiltrada(Infra.arGeral, oAlvo, 5);
            Lances arBase2 = Infra.DevolveBaseGeralFiltrada(Infra.arGeral, oAlvo, 14);

            List<int> ints1 = new List<int>();
            List<int> ints2 = new List<int>();


            Random random = new Random();

            while (ars1.Count<50)
            {
                while (ars.Count<3000)
                {

                    while (ints2.Count<5)
                    {
                        int a = random.Next(arBase2.Count);
                        if (!ints2.Contains(a)) ints2.Add(a);

                    }

                    Lance e1 = arBase2[ints2[0]];
                    Lance e2 = arBase2[ints2[1]];
                    Lance e3 = arBase2[ints2[2]];
                    Lance e4 = arBase2[ints2[3]];
                    Lance e5 = arBase2[ints2[4]];

                    foreach (Lance p in arBase1)
                    {
                        int a = Infra.Contapontos(p, e1);
                        int b = Infra.Contapontos(p, e2);
                        int c = Infra.Contapontos(p, e3);
                        int d = Infra.Contapontos(p, e4);
                        int e = Infra.Contapontos(p, e5);

                        if (a==5&&b==5&&c==5&&d==5&&e==5&&!ints1.Contains(p.Id)) { ars.Add(p); ints1.Add(p.Id); }

                    }

                    ints2.Clear();
                }

                foreach (Lance o in arBase1) { if (!ints1.Contains(o.Id)) ars1.Add(o); }

                ints1.Clear();
                ars.Clear();
            }

            Infra.SalvaSaidaW(arBase1, Infra.NomeSaida("Base6", alvo));

            //Infra.SalvaSaidaW(ints1, Infra.NomeSaida("Base6PT-1", alvo));
            //Infra.SalvaSaidaW(ints2, Infra.NomeSaida("Base6PT-2", alvo));

            //   Infra.SalvaSaidaW(arBase2, Infra.NomeSaida("Base14", alvo));

            return ars1;

        }
        public static Lances Estudo11(int alvo)
        {
            Random random = new Random();


            Lances ars = new();
            Lances ars1 = new();
            Lances ars2 = new();
            Lances ars3 = new();
            Lances ars4 = new();

            Infra.CombinarGeral();

            Lance oAlvo = Infra.arLoto[alvo-2];

            Lance oAnterior = Infra.arLoto[alvo-3];

            Lances arBase9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances arBase6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);


            Lance Encontrado9tmp = arBase9.FirstOrDefault(o => Infra.Contapontos(o, oAnterior)==9);
            Lances arAnteriores = Infra.DevolveBaseGeralFiltrada(arBase9, Encontrado9tmp, 8);

            Lance Encontrado9tmp2 = arBase9[351];
            Lance Encontrado9tmp3 = arBase9[391];
            Lance Encontrado9tmp4 = arBase9[1897];


            //   arAnteriores.Add(Encontrado9tmp);



            Lances arBaseT = Infra.DevolveBaseGeralFiltrada(Infra.arGeral, oAlvo, 9);
            Lances arBase1 = Infra.DevolveBaseGeralFiltrada(arBaseT, Encontrado9tmp, 5);
            Lances arBase2 = Infra.DevolveBaseGeralFiltrada(arBase1, Encontrado9tmp4, 4);
            Lances arBase3 = Infra.DevolveBaseGeralFiltrada(arBase1, Encontrado9tmp4, 5);
            Lances arBase4 = Infra.DevolveBaseGeralFiltrada(arBase1, Encontrado9tmp4, 6);




            while (ars.Count<10000) { ars.Add(arBase1[random.Next(arBase1.Count)]); }
            while (ars1.Count<10000) { ars1.Add(arBase1[random.Next(arBase1.Count)]); }
            while (ars2.Count<10000) { ars2.Add(arBase2[random.Next(arBase2.Count)]); }
            while (ars3.Count<10000) { ars3.Add(arBase3[random.Next(arBase3.Count)]); }
            while (ars4.Count<10000) { ars4.Add(arBase4[random.Next(arBase4.Count)]); }


            //Infra.SalvaSaidaW(ints1, Infra.NomeSaida("Base6PT-1", alvo));
            //Infra.SalvaSaidaW(ints2, Infra.NomeSaida("Base6PT-2", alvo));

            Infra.SalvaSaidaW(arBase9, Infra.NomeSaida("Base9", alvo));

            Infra.SalvaSaidaW(ars1, Infra.NomeSaida("FiltroGeral-1", alvo));
            Infra.SalvaSaidaW(ars2, Infra.NomeSaida("FiltroGeral-4", alvo));
            Infra.SalvaSaidaW(ars3, Infra.NomeSaida("FiltroGeral-5", alvo));
            Infra.SalvaSaidaW(ars4, Infra.NomeSaida("FiltroGeral-6", alvo));

            return ars;

        }
        public static Lances Estudo12(int alvo)
        {
            Random random = new Random();


            Lances ars = new();
            Lances ars1 = new();
            Lances ars3 = new();

            int n = 3014;

            Lance oAlvo = Infra.arLoto[alvo-2];
            Lance oAnterior = Infra.arLoto[n];

            //  Lance oCerto = Infra.arLoto[alvo - 1];


            Infra.CombinarGeral();

            Lances arBaseT = Infra.DevolveBaseGeralFiltrada(Infra.arGeral, oAlvo, 9);


            Lances arBase9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances arBase6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            Lance Encontrado9tmp = arBase9.FirstOrDefault(o => Infra.Contapontos(o, oAnterior)==9);
            Lance Encontrado6tmp = arBase6.FirstOrDefault(o => Infra.Contapontos(o, oAnterior)==6);

            Lances ars4 = new();
            Lances ars5 = new();
            Lances ars44 = new();
            Lances ars55 = new();

            ars44.Add(Encontrado9tmp);
            ars55.Add(Encontrado6tmp);

            ars4.AddRange(arBase9.Where(o => Infra.Contapontos(Encontrado9tmp, o)>7));
            ars5.AddRange(arBase6.Where(o => Infra.Contapontos(Encontrado6tmp, o)>4));





            Lances ars6 = Infra.CombinaLances(ars44, ars5);
            Lances ars7 = Infra.CombinaLances(ars4, ars55);

            List<int> lix = new();
            List<int> lixo = new();

            Lances ars8 = new();
            Lances ars88 = new();
            Lances ars9 = new();

            foreach (Lance o in ars4) { ars8.AddRange(arBase9.Where(o => Infra.Contapontos(Encontrado9tmp, o)>7)); }
            foreach (Lance o in arBase9)
            {
                int a = 0;
                foreach (Lance p in ars8)
                {
                    if (Infra.Contapontos(o, p)==9) { a++; break; }
                }
                if (a==0) { ars9.Add(o); }

            }

            foreach (Lance o in ars8) lix.Add(o.Id);
            foreach (Lance o in ars9) lixo.Add(o.Id);

            ars88.AddRange(ars8.DistintostById());



            Lances arBase = Infra.Aleatorios(arBaseT, 500);


            arBase.Clear();

            //   arBase.AddRange(Infra.Aleatorios(arBaseT, 500000));


            //    ars3.AddRange(arBase.Where(o => Infra.Contapontos(Encontrado9tmp, o) == 6));


            //   Infra.SalvaSaidaW(ints, Infra.NomeSaida("Encontrados", alvo));

            Infra.SalvaSaidaW(arBase9, Infra.NomeSaida("Base9", alvo));
            Infra.SalvaSaidaW(arBase6, Infra.NomeSaida("Base6", alvo));

            //      Infra.SalvaSaidaW(ars3, Infra.NomeSaida("BaseGeral", alvo));

            Infra.SalvaSaidaW(ars6, Infra.NomeSaida("Correria9", alvo));
            Infra.SalvaSaidaW(ars5, Infra.NomeSaida("Correria6", alvo));
            Infra.SalvaSaidaW(ars88, Infra.NomeSaida("CorreriaTotal", alvo));
            Infra.SalvaSaidaW(ars9, Infra.NomeSaida("SobraTotal", alvo));
            Infra.SalvaSaidaW(lix, Infra.NomeSaida("CorreriaTotal-Lixo", alvo));
            Infra.SalvaSaidaW(lixo, Infra.NomeSaida("SobraTotal-Lixo", alvo));


            return ars6;

        }
        public static Lances Estudo13(int alvo)
        {

            Lances ars = new();
            Lances ars2 = new();
            Lances ars5 = new();

            Random rnd = new Random();

            int n = alvo-3;

            List<int> ints = new();

            Infra.CombinarGeral();

            Infra.CarregarConcursos();

            Random random = new Random();

            Lance oAlvo = Infra.arLoto[alvo-2];
            Lance oposto = Infra.DevolveOposto(oAlvo);

            Lance oAnterior = Infra.arLoto[alvo-3];

            Lances arBase9 = Infra.Combinar15a9(oAlvo.Lista);
            Lances arBase6 = Infra.Combinar10a6(Infra.DevolveOposto(oAlvo).Lista);

            Lances Encontrado6 = new(); Encontrado6.AddRange(arBase6.Where(o => Infra.Contapontos(oAnterior, o)==2));
            Lances Encontrado9 = new(); Encontrado9.AddRange(arBase9.Where(o => Infra.Contapontos(oAnterior, o)==3));

            //  Lances ars3 = Infra.Aleatorios(Infra.arGeral, 10000);

            List<int> lt = new() { 4, 30, 40, 10 };

            foreach (Lance o in arBase9)
            {
                List<int> l = Enumerable.Repeat(0, lt.Count).ToList();
                foreach (Lance p in Encontrado9)
                {
                    int a = Infra.Contapontos(o, p);

                    switch (a)
                    {
                        case 4: l[0]++; break;
                        case 5: l[1]++; break;
                        case 6: l[2]++; break;
                        case 7: l[3]++; break;

                    }
                }
                int s = 0;
                for (int z = 0; z<l.Count; z++) { if (l[z]!=lt[z]) s++; break; };
                if (s==0) ars5.Add(o);
            }


            Lances ars4 = new();
            while (ars4.Count<10000) { Lance a = Infra.arGeral[rnd.Next(Infra.arGeral.Count)]; if (Infra.Contapontos(a, oAlvo)==9) ars4.Add(a); }






            //  Infra.SalvaSaidaW(ars3, Infra.NomeSaida("AleatóriosGeral", alvo));
            Infra.SalvaSaidaW(ars4, Infra.NomeSaida("AleatoriosPor9", alvo));
            Infra.SalvaSaidaW(ars5, Infra.NomeSaida("Vizinhos9", alvo));


            //   Infra.SalvaSaidaW(ars5, Infra.NomeSaida("MelhoresVizinhos", alvo));

            //     Infra.SalvaSaidaW(osVizinhosG, Infra.NomeSaida("VizinhosGeral", alvo));
            //   Infra.SalvaSaidaW(osVizinhosT, Infra.NomeSaida("VizinhosPor9", alvo));

            // Infra.SalvaSaidaW(vizinhança, Infra.NomeSaida("Vizinhança", alvo));

            return ars2;

        }


        #region MÉTODOS INATIVOS     


        public static Lances Estudo14(int alvo)
        {
            Lances ars = new();
            Lances ars1 = new();
            Lances ars2 = new();
            Lances ars3 = new();
            Lances ars4 = new();

            Infra.CarregarConcursos();

            Infra.CombinarGeral();

            Lance anterior = Infra.arLoto[alvo-3];
            Lance oAlvo = Infra.arLoto[alvo-2];

            Lances arBase = Infra.Combinar15a9(oAlvo.Lista);

            Lances arsB = Infra.DevolveBaseCompleta(oAlvo, 9);


            Random random = new();
            while (ars2.Count<1890)
            {
                ars.Clear();

                while (ars.Count<10000) { ars.Add(Infra.arGeral[random.Next(Infra.arGeral.Count)]); }

                Lance frequentes = Infra.DevolveMaisFrequentes(ars, 15);

                foreach (Lance o in arBase)
                {
                    if ((Infra.Contapontos(o, frequentes)==9)&&(!Infra.ListagemContemSequencia(ars2, o))) { ars2.Add(o); break; }
                }
            }

            foreach (Lance o in arBase) { if (!Infra.ListagemContemSequencia(ars2, o)) ars1.Add(o); }

            while (ars3.Count<10000)
            {
                Lance a = arsB[random.Next(arsB.Count)];



                foreach (Lance p in arBase)
                {
                    if ((Infra.Contapontos(a, p)==9)&&(Infra.Contapontos(a, anterior)==9)) { ars3.Add(p); ars4.Add(a); }


                }
            }



            Infra.SalvaSaidaW(ars1, Infra.NomeSaida("ConjuntoAdjacente", alvo));
            Infra.SalvaSaidaW(arBase, Infra.NomeSaida("Base9", alvo));
            Infra.SalvaSaidaW(ars2, Infra.NomeSaida("BaseParaExclusão", alvo));
            Infra.SalvaSaidaW(ars4, Infra.NomeSaida("BaseEstranha", alvo));



            return ars3;
        }

        public static Lances Estudo15(int alvo)
        {
            Lances ars = new();
            Lances ars1 = new();

            //List<int> l = new();
            // List<int> l = new() { 2, 3, 14, 15, 16, 17, 19, 20, 22, 23, 25 };  /*'2831*/

            List<int> l = new() { 1, 2, 3, 4, 5, 6, 8, 9, 10 };

            Lance lb = new Lance(0, l);

            Infra.CarregarConcursos();

            Infra.CombinarGeral();

            string arqNome = "ListaPossivel11-"+"-"+alvo.ToString();

            Lances arBonds = Infra.AbrirArquivo(arqNome);


            //Lance frequente = Infra.DevolveMaisFrequentes(arBonds, 15);


            Lance frequente = new Lance(0, new List<int>() { 2, 4, 5, 8, 9, 10, 11, 12, 13, 15, 18, 19, 20, 21, 25 }); /*'2871*/

            //  Lance frequente = new Lance(0, new List<int>() { 2, 3, 4, 7, 11, 12, 13, 15, 17, 18, 19, 20, 21, 23, 24 });   /*'2831*/


            Random random = new Random();

            foreach (Lance o in Infra.arGeral)
            {
                if (Infra.Contapontos(o, frequente)==14) { ars.Add(o); }
            }


            while (ars1.Count<300)
            {

                Lance o = Infra.arGeral[random.Next(0, Infra.arGeral.Count)];
                //foreach (Lance o in Infra.arGeral)
                //{
                if (Infra.Contapontos(o, lb)==lb.Lista.Count)
                {

                    int a = 0;
                    int b = 0;
                    int c = 0;

                    foreach (Lance p in ars)
                    {
                        int t = Infra.Contapontos(o, p);
                        if (t==8) { a++; }
                        if (t==9) { b++; }
                        if (t==10) { c++; }
                    }

                    if ((a==36)&&(b==78)&&(c==36)) { ars1.Add(o); }

                    //         if (ars1.Count > 10000) { break; }

                }
            }


            // Infra.SalvaSaidaW(frequente.Lista, Infra.NomeSaida("Frequente", alvo));


            return ars1;
        }

        public static Lances Estudo16(int alvo)
        {
            Lances ars = new();
            Lances ars1 = new();

            //  List<int> l = new();
            List<int> l = new() { 2, 3, 14, 15, 16, 17, 19, 20, 22, 23, 25 };

            Lance lb = new Lance(0, l);

            Infra.CarregarConcursos();

            Infra.CombinarGeral();

            string arqNome = "ListaPossivel11-"+"-"+alvo.ToString();

            Lances arBonds = Infra.AbrirArquivo(arqNome);


            //  Lance frequente = Infra.DevolveMaisFrequentes(arBonds, 15);

            //    Lance frequente = new Lance(0, new List<int>() { 2, 4, 5, 8, 9, 10, 11, 12, 13, 15, 18, 19, 20, 21, 25 });
            Lance frequente = new Lance(0, new List<int>() { 2, 3, 4, 7, 11, 12, 13, 15, 17, 18, 19, 20, 21, 23, 24 });   /*'2831*/


            Random random = new Random();

            foreach (Lance o in Infra.arGeral)
            {
                if (Infra.Contapontos(o, frequente)==14) { ars.Add(o); }
            }


            //while (ars1.Count < 10000)
            //{

            //    Lance o = Infra.arGeral[random.Next(0, Infra.arGeral.Count)];
            foreach (Lance o in Infra.arGeral)
            {
                if (Infra.Contapontos(o, lb)==lb.Lista.Count)
                {

                    int a = 0;
                    int b = 0;
                    int c = 0;

                    foreach (Lance p in ars)
                    {
                        int t = Infra.Contapontos(o, p);
                        if (t==8) { a++; }
                        if (t==9) { b++; }
                        if (t==10) { c++; }
                    }

                    if ((a==36)&&(b==78)&&(c==36)) { ars1.Add(o); }

                    // if (ars1.Count > 10000) { break; }

                }
            }





            return ars1;
        }

        public static Lances Estudo17(int alvo)
        {
            Lances ars1 = new();

            string arqNome = "Estudo14"+"-"+alvo.ToString();
            // string arqNome1 = "Bases" + "-" + alvo.ToString();


            Lance frequente = new Lance(0, new List<int>() { 2, 4, 5, 8, 9, 10, 11, 12, 13, 15, 18, 19, 20, 21, 25 });

            Lances arFrequentes = Infra.Combinar15a9(frequente.Lista);

            Lances arBonds = Infra.AbrirArquivo(arqNome);
            //   Lances arBases = Infra.AbrirArquivo(arqNome1);

            List<int> inteiros = new();


            foreach (Lance o in arFrequentes)
            {
                int a = 0;

                foreach (Lance p in arBonds)
                {
                    int t = Infra.Contapontos(o, p);
                    if (t==9) { a++; }
                }
                inteiros.Add(a);

            }

            Infra.SalvaSaidaW(inteiros, Infra.NomeSaida("FrequentesPT", alvo));


            return arFrequentes;
        }

        public static Lances Estudo18(int alvo)
        {
            Lances ars = new();
            Lances ars1 = new();

            Infra.CarregarConcursos();

            Infra.CombinarGeral();

            string arqNome = "Estudo14"+"-"+alvo.ToString();

            Lances arBonds = Infra.AbrirArquivo(arqNome);

            List<int> numeros = new List<int>() { 1, 21, 25, 2, 20 };



            foreach (Lance o in arBonds)
            {

                if (Infra.Contapontos(o, numeros)==numeros.Count) { ars1.Add(o); }

                if (ars1.Count==150000) { break; }
            }




            return ars1;
        }

        public static Lances Estudo19(int alvo)
        {

            Lances ars1 = new();
            Lances ars2 = new();
            List<int> ints = new List<int>();
            List<int> NinesPT = new List<int>();

            Infra.CarregarConcursos();

            Lance oAlvo = Infra.arLoto[alvo-1];


            string arqNome = "Estudo14"+"-"+alvo.ToString();

            Lances arBonds = Infra.AbrirArquivo(arqNome);

            string arqNome1 = "Bases"+"-"+alvo.ToString();

            Lances arBases = Infra.AbrirArquivo(arqNome1);


            for (int i = 0; i<15; i++)
            {
                int ini = i*10;

                List<int> numeros = Enumerable.Range(ini, 10).ToList();

                Lances BasesNine = Infra.Combinar10a3(numeros);


                foreach (Lance p in BasesNine)
                {
                    Lances ars = new();
                    Lances arstmp = new();

                    foreach (int q in p.Lista)
                    {
                        ars.Add(arBases[q]);
                    }
                    int xz = 0;
                    foreach (Lance lance in ars)
                    {
                        if (Infra.Contapontos(lance, oAlvo)==9) xz++;
                    }
                    NinesPT.Add(xz);

                    foreach (Lance o in arBonds)
                    {
                        int a = Infra.Contapontos(o, ars[0]);
                        int b = Infra.Contapontos(o, ars[1]);
                        int c = Infra.Contapontos(o, ars[2]);

                        if ((a==9)&&(b==9)&&(c==9)) { arstmp.Add(o); }
                    }
                    ints.Add(arstmp.Count);

                    ars2.Add(Infra.DevolveMaisFrequentes(arstmp, 4));
                }

                BasesNine.Clear();
                numeros.Clear();
            }

            Infra.SalvaSaidaW(ars2, Infra.NomeSaida("BaseLegitima", alvo));


            return ars1;
        }
        public static Lances Estudo20(int alvo)
        {

            Lances ars1 = new();
            Lances ars2 = new();
            List<int> ints = new List<int>();
            List<int> NinesPT = new List<int>();

            Infra.CarregarConcursos();

            //     Lance oAlvo = Infra.arLoto[alvo - 1];


            string arqNome = "Estudo14"+"-"+alvo.ToString();

            Lances arBonds = Infra.AbrirArquivo(arqNome);

            string arqNome1 = "Bases"+"-"+alvo.ToString();

            Lances arBases = Infra.AbrirArquivo(arqNome1);

            int ini = 0;

            string pt = Constante.PT+"\\Saida\\"+alvo.ToString()+"\\TMP";

            if (!Directory.Exists(pt)) Directory.CreateDirectory(pt);

            List<int> numeros = Enumerable.Range(ini, 10).ToList();

            Lances BasesNine = Infra.Combinar10a3(numeros);

            foreach (Lance p in BasesNine)
            {
                Lances ars = new();
                Lances arstmp = new();



                foreach (int q in p.Lista)
                {
                    ars.Add(arBases[q]);
                }
                //int xz = 0;
                //foreach (Lance lance in ars)
                //{
                //    if (Infra.Contapontos(lance, oAlvo) == 9) xz++;
                //}
                //NinesPT.Add(xz);

                foreach (Lance o in arBonds)
                {
                    int a = Infra.Contapontos(o, ars[0]);
                    int b = Infra.Contapontos(o, ars[1]);
                    int c = Infra.Contapontos(o, ars[2]);

                    if ((a==9)&&(b==9)&&(c==9)) { arstmp.Add(o); }
                }
                ints.Add(arstmp.Count);

                Infra.SalvaSaidaW(arstmp, Infra.NomeSaida("TMP\\"+"BaseLegitimaInicial-V"+p.Id.ToString(), alvo));

                ars2.Add(Infra.DevolveMaisFrequentes(arstmp, 4));
            }



            Infra.SalvaSaidaW(ars2, Infra.NomeSaida("BaseLegitimaInicial", alvo));
            Infra.SalvaSaidaW(ints, Infra.NomeSaida("BaseLegitimaInicialPT", alvo));


            return ars1;
        }

        #endregion


        #region MÉTODOS AUXILIARES


        private static int PostosComuns(Lances Base, Lances arEstudo, int v)
        {

            Lances ar = new();
            int saida = 0;

            foreach (Lance o in Base)
            {
                int s = 0;
                foreach (Lance p in arEstudo)
                {
                    if (Infra.Contapontos(o, p)==v) { s++; }
                }
                if (s==arEstudo.Count) { ar.Add(o); }
            }

            if (ar.Count==1) { saida=ar[0].Id; } else { saida=-1; }

            return saida;
        }

        private static void Quintas(ref Lances Base, Lances arEstudo)
        {

            List<int> ar = new();

            foreach (Lance o in Base)
            {
                int s = 0;
                foreach (Lance p in arEstudo)
                {
                    if (Infra.Contapontos(o, p)==5) { s++; }
                }
                if (s==arEstudo.Count)
                {
                    o.M++;
                    foreach (int z in arEstudo[0].ListaM) { if (!o.ListaM.Contains(z)) o.ListaM.Add(z); };
                }

            }
        }


        #endregion



    }


}




