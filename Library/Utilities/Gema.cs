using LotoLibrary.Models;
using System;
using System.Collections.Generic;


namespace LotoLibrary.Utilities
{
    public class Gema : IComparable
    {

        public int Id;
        public List<int> Lista;

        public Lances Grupo = new();

        public int N;

        public Gema(int id, List<int> lista)
        {
            Id = id;
            Lista = lista;
        }

        public void Pontos()
        {
            int m = 0;
            if (Grupo.Count > 0)
            {

                List<int> S = new();
                for (int i = 0; i < 25; i++) { S.Add(0); }

                foreach (Lance o in Grupo)
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


            }

            N = m;
        }



        public int CompareTo(object obj)
        {
            Gema l = (Gema)obj;
            return -N.CompareTo(l.N);
        }
    }

    public class Gemas : List<Gema> { }
}
