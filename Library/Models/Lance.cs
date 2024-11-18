using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace LotoLibrary.Models
{
    public class Lance : IComparable
    {
        public int Id;

        public int M = 0;
        public int N = 0;
        public int X = 0;
        public int Y = 0;
        public int PT = 0;

        public int Anel = 0;

        public List<int> Lista;
        public List<int> ListaN = new();
        public List<int> ListaM = new();

        public Lances ListaX = new();

        public Lances ListaY = new();

        public string Saida;
        public string Nome;

        public Lance(int id, List<int> lista)
        {
            Id = id;
            Lista = lista;

            foreach (int o in lista)
            {
                Saida += Convert.ToString(o) + "\t";
                Nome += Convert.ToString(o);
            }
        }

        public Lance()
        {
            this.Id = 0;
            this.Lista = new List<int>();
        }

        public string Atualiza()
        {
            Saida = string.Empty;
            Nome = string.Empty;

            foreach (int o in Lista)
            {
                Saida += Convert.ToString(o) + "\t";
                Nome += Convert.ToString(o);
            }


            return Saida;
        }

        public void Ordena() { SomaX(); ListaM.Sort(); ListaN.Sort(); }

        public void SomaX() => N = ListaX.Count;

        public void Pontos(Lance O)
        {
            PT = Lista.Intersect(O.Lista).ToList().Count;
            N = ListaN.Intersect(O.Lista).ToList().Count;

        }

        public void LimpaListas() { this.ListaX.Clear(); this.ListaY.Clear(); }

        public Boolean SomaComplementar(Lance O)
        {
            int h0 = Infra.Contapontos(O.Lista, this.Lista);
            int h1 = Infra.Contapontos(O.Lista, this.ListaN);

            if ((h0 == this.Lista.Count) && (h1 == 0)) return true; else return false;
        }

        public int CompareTo(object obj)
        {
            Lance l = (Lance)obj;
            return -N.CompareTo(l.N);
        }

        public void ComplementarPara(Lance O)
        {
            this.ListaN = O.Lista.Except(this.Lista).ToList();
            this.ListaN.Sort();
        }
    }

    public class Lances : List<Lance>
    {
        public Lances()
        {
        }

        public Lances(IEnumerable<Lance> collection) : base(collection)
        {
        }
    }

    public static class EnumerableExtensions
    {
        public static Lances ToLances(this IEnumerable<Lance> source)
        {
            return new Lances(source);
        }

        public static Lances DistintostById(this Lances lances)
        {
            var distinctLances = lances
                .GroupBy(lance => lance.Id)
                .Select(group => group.First())
                .ToList();

            return new Lances(distinctLances);
        }

        public static Lances RemoveObjetosDuplicados(this Lances lances)
        {
            // Encontrar os IDs duplicados
            var duplicatedIds = lances
                .GroupBy(lance => lance.Id)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);

            // Remover todos os lances com os IDs duplicados
            Lances result = new Lances(lances.Where(lance => !duplicatedIds.Contains(lance.Id)));

            return result;
        }

    }
}
