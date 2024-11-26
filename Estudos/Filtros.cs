using LotoLibrary.Constantes;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Busisness
{
    public class Filtros
    {

        /// <summary>
        /// Aplica um filtro em listas de dados de loteria.
        /// </summary>
        /// <param name="ListaT">Lista de entrada a ser filtrada.</param>
        /// <param name="ListaAlvo">Lista de critérios para o filtro.</param>
        /// <returns>Uma nova lista de resultados filtrados.</returns>
        public static Lances Filtro1(Lances ListaT, Lances ListaAlvo)
        {
            Lances ars = new();

            foreach (Lance o in ListaT)
            {
                int m = 0;
                foreach (Lance p in ListaAlvo)
                {
                    if (Infra.Contapontos(o, p) == 9)
                    {
                        m++;
                    }
                }
                if (m == 3)
                {
                    ars.Add(o);
                }
            }

            return ars;
        }

        public static Lances Filtro2(Lances ListaT)
        {
            Lances ars = new();

            foreach (Lance o in ListaT)
            {
                int m = Infra.Contapontos(o, Constante.ListaA);
                int n = Infra.Contapontos(o, Constante.ListaC);

                if ((m == 6) | (m == 7) | (m == 8))
                {
                    if ((n == 6) | (n == 7) | (n == 8))
                    {
                        ars.Add(o);
                    }
                }

            }

            return ars;
        }

        public static Lances Filtro3(Lances ListaT)
        {
            Lances ars = new();

            Random rnd = new Random();
            int x = rnd.Next(0, ListaT.Count);

            Lance lin = ListaT[x];
            ars.Add(lin);

            for (int i = 0; i < 400; i++)
            {
                foreach (Lance o in ListaT)
                {
                    int m = 0;

                    foreach (Lance p in ars)
                    {
                        if (Infra.Contapontos(o, p) == 9)
                        {
                            m++;
                        }
                    }

                    if (ars.Count == m)
                    {
                        ars.Add(o);
                    }

                }
            }

            return ars;
        }

        public static List<int> Filtro4(Lances ListaT)
        {
            List<int> ars = new();

            foreach (Lance o in ListaT)
            {
                //int m = 0;
                //int x = 0;
                int y = 0;


                foreach (Lance p in ListaT)
                {
                    //if (Infra.Contapontos(o, p) == 5)
                    //{
                    //    m++;
                    //}
                    //if (Infra.Contapontos(o, p) == 8)
                    //{
                    //    x++;
                    //}
                    if (Infra.Contapontos(o, p) == 9)
                    {
                        y++;
                    }
                }
                //ars.Add(y - x);

                ars.Add(y);

            }


            return ars;
        }

        public static Lances Filtro5(Lances ListaT, Lances ListaP)
        {
            Lances ars = new();

            foreach (Lance o in ListaT)
            {

                int y = 0;


                foreach (Lance p in ListaP)
                {


                    if (Infra.Contapontos(o, p) == 6)
                    {
                        y++;
                    }
                }

                if (y == ListaP.Count)
                {
                    ars.Add(o);
                }
            }


            return ars;
        }

        public static Lances Filtro6(Lances ListaT)
        {
            Lances ars = new


            Random rnd = new Random();

            for (int z = 0; z < 20; z++)
            {
                int y = rnd.Next(0, ListaT.Count);

                ars.AddRange(ContaCompativeis(ListaT, y));

            }

            Infra.SalvaSaidaW(ars, Infra.NomeSaida("ListaEscolhidaBase", 0));

            return ars;
        }

        private static Lances ContaCompativeis(Lances ListaT, int z)
        {
            Lances ars = new();

            Lance lin = ListaT[z];

            ars.Add(lin);


            for (int i = 0; i < 200; i++)
            {
                foreach (Lance o in ListaT)
                {
                    int m = 0;

                    foreach (Lance p in ars)
                    {
                        if (Infra.Contapontos(o, p) == 9)
                        {
                            m++;
                        }
                    }

                    if (ars.Count == m)
                    {
                        ars.Add(o);
                    }

                }
            }


            return ars;
        }

        public static Lances Filtro7(Lances ListaT, Lances ListaP)
        {
            Lances ars = new();
            Random rnd = new Random();

            foreach (Lance o in ListaT)
            {
                int y = 0;
                foreach (Lance p in ListaP)
                {
                    int m = Infra.Contapontos(o, p);
                    if (m == 5) { ars.Add(p); y++; }

                    if (y == 8) { break; }

                }
            }

            return ars;
        }

        public static Lances Filtro8(Lances ListaT, Lances ListaP)
        {
            Lances ars = new();

            List<string> arsId = new List<string>();

            foreach (Lance p in ListaP)
            {
                string s = p.Saida;
                if (!arsId.Contains(s))
                {
                    arsId.Add(s);
                }
            }

            foreach (Lance p in ListaT)
            {
                string s = p.Saida;

                if (!arsId.Contains(s))
                {
                    ars.Add(p);
                }
            }


            return ars;
        }

        public static Lances Filtro9(Lances ListaT)
        {
            Lances ars = new();

            List<string> arsId = new List<string>();

            foreach (Lance p in ListaT)
            {
                string s = p.Saida;
                if (!arsId.Contains(s))
                {
                    arsId.Add(s);
                    ars.Add(p);
                }
            }

            return ars;
        }


        public static bool Filtro10(Lance L)
        {

            for (int i = 0; i < 15; i++)
            {
                int x = L.Lista[i];
                int a = Constante.ListaDmin[i];
                int b = Constante.ListaDmax[i];
                if ((x < a) || (x > b))
                {
                    return false;
                }
            }


            return true;
        }

        public static bool Filtrolimiar(Lance L, List<int> ls, int menor, int maior)
        {
            int x = Infra.Contapontos(L, ls);

            if ((x < menor) || (x > maior))
            {
                return false;
            }
            return true;

        }


    }
}
