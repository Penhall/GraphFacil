using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Busisness
{
    public class Gene
    {
        /// <summary>
        /// Método para gerar uma lista de inteiros aleatórios
        /// </summary>
        /// <param name="T"> Tamanho da lista de aleatórios</param>
        /// <param name="M">Limite máximo para o inteiro aleatório</param>
        /// <returns></returns>
        public static List<int> Aleatorio(int T, int M)
        {
            List<int> A = new();
            Random rnd = new Random();

            while (A.Count < T)
            {
                int x = rnd.Next(0, M);

                if (!A.Contains(x)) { A.Add(x); }
            }

            A.Sort();

            return A;
        }

        public static List<int> Aleatorio(int T, int M, Lances lustres)
        {
            List<int> A = new();
            List<int> B = new();

            Random rnd = new Random();

            while (A.Count < T)
            {
                int x = rnd.Next(0, M);
                if (!B.Contains(x))
                {
                    if (!A.Contains(x))
                    {

                        A.Add(x);

                        Lance u = lustres[x];

                        B.AddRange(u.Lista);

                    }

                }

            }

            A.Sort();

            return A;
        }


        /// <summary>
        /// Retorna uma lista de Lances a partir de uma lista de inteiro fornecida  e uma 
        /// lista fonte
        /// </summary>
        /// <param name="L">Lista de índices</param>
        /// <param name="G">Lista Fonte</param>
        /// <returns></returns>
        public static Lances GetLances(List<int> L, Lances G)
        {
            Lances S = new();

            foreach (int i in L)
            {
                S.Add(G[i]);
            }
            return S;

        }

        public static List<int> CruzaXY(List<int> A, List<int> B)
        {
            List<int> S = new();
            int m = System.Math.Abs(A.Count / 2) + 2;
            Random rnd = new Random();

            int x = rnd.Next(1, m);

            for (int i = 0; i < A.Count; i++)
            {
                if (i < x) { S.Add(A[i]); }
                else
                {
                    if (!S.Contains(B[i]))
                    {
                        S.Add(B[i]);
                    }

                }
            }

            S.Sort();

            return S;
        }

        public static List<Gema> CruzaXY(List<Gema> A, List<Gema> B, Lances Geral)
        {
            List<Gema> S = new List<Gema>();


            Random rnd = new Random();

            foreach (Gema o in A)
            {
                foreach (Gema p in B)
                {
                    var l = CruzaXY(o.Lista, p.Lista);

                    while (l.Count < o.Lista.Count)
                    {
                        int x = rnd.Next(0, Geral.Count);
                        if (!l.Contains(x)) { l.Add(x); }
                    }


                    Gema g = new Gema(S.Count, l);

                    g.Grupo.AddRange(Pop(l, Geral));
                    g.Pontos();

                    S.Add(g);
                }
            }

            return S;
        }

        public static Lances Pop(List<int> idx, Lances Bas)
        {
            Lances S = new();

            foreach (int i in idx)
            {
                S.Add(Bas[i]);
            }
            return S;
        }

        public static int Pontuacao(Lances T)
        {
            int m = 0;

            List<int> S = new();
            for (int i = 1; i < 25; i++) { S.Add(0); }

            foreach (Lance o in T)
            {
                foreach (int p in o.Lista)
                {
                    S[p - 1]++;
                }
            }

            for (int i = 0; i < 25; i++)
            {
                if (S[i] == 15) { m++; }
            }

            return m;
        }

        public static List<Gema> GeraPop(int T, Lances Geral)
        {
            List<Gema> S = new List<Gema>();

            for (int i = 0; i < T; i++)
            {
                var l = Aleatorio(15, Geral.Count);
                Gema g = new Gema(S.Count, l);

                g.Grupo.AddRange(Pop(l, Geral));
                g.Pontos();

                S.Add(g);
            }

            S.Sort();

            return S;
        }

    }
}
