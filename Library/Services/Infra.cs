using LotoLibrary.Constantes;
using LotoLibrary.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace LotoLibrary.Services;


public class Infra
{

    public static Lances arGeral = new();
    public static Lances arLoto = new();
    public static Lances arGeralFiltrado = new();


    #region "Abrir arquivos"
    public static Lances AbrirArquivo(string nome)
    {

        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;

        DirectoryInfo pt1 = new(path);

        var arq1 = pt1.GetFiles("*.txt", SearchOption.AllDirectories);

        Lances ar = new();

        string nomes = nome + ".txt";

        foreach (FileInfo o in arq1)
        {
            if (o.Name == nomes)
            {
                string file = File.ReadAllText(o.FullName);
                ar = PreencheCesta(file);
                break;
            }
        }


        return ar;
    }

    private static Lances PreencheCesta(string file)
    {

        Lances ar = new();

        var m = file.Split("\r");

        foreach (var o in m)
        {
            if (!(o == null))
            {

                string y = Regex.Replace(o, @"\r|\n", String.Empty);
                string[] p = y.Split("\t");

                List<int> l = new();


                foreach (string x in p)
                {
                    if (!(x == string.Empty) || !(x is not null))
                    {
                        l.Add(Convert.ToInt32(x));
                    }

                }

                Lance u = new Lance(ar.Count, l);

                ar.Add(u);

            }
        }

        return ar;
    }

    private static void AtualizarConcursosDiretoDoSite(string nomeArq)
    {
        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;
        string fullPath = Path.Combine(path, nomeArq);

        List<Lotofacil> concursos;

        if (File.Exists(fullPath))
        {
            // O arquivo existe, vamos carregar e atualizar
            concursos = CarregarConcursosDoArquivo(fullPath);
            var updater = new LotofacilUpdater();
            concursos = updater.UpdateConcursos(concursos);

            PreencheSorteios(concursos);
        }
        else
        {
            // O arquivo não existe, vamos extrair todos os concursos
            var scraper = new LotofacilScraper();
            concursos = scraper.ExtractAllConcursos();

            PreencheSorteios(concursos);
        }

        // Salvar os concursos atualizados no arquivo
        SalvarConcursosNoArquivo(fullPath, concursos);
    }


    private static List<Lotofacil> CarregarConcursosDoArquivo(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonConvert.DeserializeObject<List<Lotofacil>>(json);
    }
    private static async Task SalvarConcursosNoArquivo(string filePath, List<Lotofacil> concursos)
    {
        string json = JsonConvert.SerializeObject(concursos, Formatting.Indented);
        await File.WriteAllTextAsync(filePath, json);
    }


    private static void PreencheSorteios(List<Lotofacil> concursos)
    {
        foreach (var o in concursos)
        {
            List<int> l = new List<int>();
            foreach (string m in o.listaDezenas) { l.Add(Convert.ToInt32(m)); }

            arLoto.Add(new Lance(arLoto.Count, l));
        }
    }

    #endregion


    #region "Área JSON"


    public static Lances AbrirArquivoJson(string nome)
    {

        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;

        DirectoryInfo pt1 = new(path);

        var arq1 = pt1.GetFiles("*.json", SearchOption.AllDirectories);

        Lances ar = new();

        string nomes = nome + ".json";

        string file = string.Empty;

        foreach (FileInfo o in arq1)
        {
            if (o.Name == nomes)
            {
                file = File.ReadAllText(o.FullName);
                break;
            }
        }

        var retorno = JsonConvert.DeserializeObject<Lances>(file);

        return retorno;
    }

    public static void SalvaSaidaJson(Lances matrizSaida, string nome)
    {

        StringBuilder salvaKey = new();

        string json = JsonConvert.SerializeObject(matrizSaida);

        File.WriteAllText(nome, json);
    }

    public static string NomeJson(string nome, int alvo)
    {
        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;

        string caminho = path + "\\" + "Saida" + "\\" + alvo.ToString();

        if (!Directory.Exists(caminho)) { Directory.CreateDirectory(caminho); }

        return Path.Combine(caminho + "\\" + nome + "-" + alvo.ToString() + ".json");
    }

    #endregion

    #region "Salvar Arquivos"

    public static void SalvaSaidaW(Lances matrizSaida, string nome)
    {

        StringBuilder salvaKey = new();

        foreach (Lance o in matrizSaida)
        {
            salvaKey.AppendLine(o.Saida);

        }

        File.WriteAllText(nome, salvaKey.ToString());
    }

    public static void SalvaSaidaW(List<int> matrizSaida, string nome)
    {

        StringBuilder salvaKey = new StringBuilder();

        foreach (int o in matrizSaida)
        {
            salvaKey.AppendLine(o.ToString());

        }

        File.WriteAllText(nome, salvaKey.ToString());
    }



    #endregion

    #region "Métodos Especiais"
    public static void CarregarConcursos() => AtualizarConcursosDiretoDoSite(Constante.ArqLotoJson);




    public static int Contapontos(Lance Lsa, Lance Lsb)
    {
        return Lsa.Lista.Intersect(Lsb.Lista).ToList().Count;
    }

    public static int Contapontos(Lance Lsa, List<int> Lsb)
    {
        return Lsa.Lista.Intersect(Lsb).ToList().Count;
    }

    public static int Contapontos(List<int> Lsa, List<int> Lsb)
    {
        return Lsa.Intersect(Lsb).ToList().Count;
    }

    #endregion

    #region "EXTRAS"

    public static bool RetornaFiltrado(int a, int b, int c, int d, int e, int f, int g, int h, int i, int j, int k, int l, int m, int n, int o)
    {
        if (a > 3) { return false; }
        if ((b < 2) || (b > 5)) { return false; }
        if ((c < 3) || (c > 6)) { return false; }
        if ((d < 4) || (d > 8)) { return false; }
        if ((e < 6) || (e > 9)) { return false; }
        if ((f < 7) || (f > 12)) { return false; }
        if ((g < 9) || (g > 13)) { return false; }
        if ((h < 11) || (h > 15)) { return false; }
        if ((i < 13) || (i > 17)) { return false; }
        if ((j < 14) || (j > 19)) { return false; }
        if ((k < 16) || (k > 20)) { return false; }
        if ((l < 18) || (l > 22)) { return false; }
        if ((m < 20) || (m > 23)) { return false; }
        if ((n < 21) || (n > 24)) { return false; }
        if ((o < 23) || (o > 25)) { return false; }


        return true;
    }

    public static Lance DevolveComplementar(Lance Lsa, Lance Lsb)
    {
        List<int> l = Lsa.Lista.Except(Lsb.Lista).ToList();

        Lance v = new Lance(0, l);

        return v;

    }

    public static Lance DevolveIncomuns(Lance Lsa, Lance Lsb)
    {
        List<int> l = new();

        foreach (int i in Lsa.Lista)
        {
            if (!Lsb.Lista.Contains(i))
            {
                l.Add(i);
            }
        }

        Lance v = new Lance(0, l);

        return v;
    }

    public static Lances RetornaLancesRanqueados(Lances arsB, int v)
    {
        Lances ars = new Lances();

        List<int> lix = new();

        foreach (Lance o in arsB)
        {
            foreach (Lance p in arGeral)
            {
                if ((Infra.Contapontos(o, p) == v) && (!lix.Contains(p.Id))) { ars.Add(p); lix.Add(p.Id); }
            }
        }

        return ars;
    }
    public static Lance DevolveOposto(Lance U)
    {
        if ((U.Lista.Count > 0))
        {
            List<int> l = Constante.ListaG.Except(U.Lista).ToList();

            Lance v = new Lance(0, l);

            return v;
        }

        return U;

    }
    public static string NomeSaida(string nome, int alvo)
    {

        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;

        string caminho = path + "\\" + "Saida" + "\\" + alvo.ToString();

        if (!Directory.Exists(caminho)) { Directory.CreateDirectory(caminho); }

        return Path.Combine(caminho + "\\" + nome + "-" + alvo.ToString() + ".txt");
    }
    public static Lance DevolveMaisFrequentes(Lances L, int T)
    {
        List<int> ints = new();

        foreach (Lance o in L) { ints.AddRange(o.Lista); }

        Uvas uvas = new Uvas();

        for (int i = 1; i < 26; i++)
        {
            int s = ints.Count(num => num == i);

            Uva u = new(i, s);

            uvas.Add(u);
        }

        uvas.Sort();

        List<int> ls = new();

        for (int j = 0; j < T; j++)
        {
            var u = uvas[j];
            ls.Add(u.Id);
        }

        ls.Sort();

        Lance U = new Lance(0, ls);

        return U;
    }

    public static Lance DevolveMenosFrequentes(Lances L, int T)
    {
        List<int> ints = new();

        foreach (Lance o in L) { ints.AddRange(o.Lista); }

        Uvas uvas = new Uvas();

        for (int i = 1; i < 26; i++)
        {
            int s = ints.Count(num => num == i);

            Uva u = new(i, s);

            uvas.Add(u);
        }

        uvas.Sort();
        uvas.Reverse();

        List<int> ls = new();

        for (int j = 0; j < T; j++)
        {
            var u = uvas[j];
            ls.Add(u.Id);
        }

        ls.Sort();

        Lance U = new Lance(0, ls);

        return U;
    }

    public static bool ComparaListasDeInteiro(List<int> pesos, List<int> candidatos)
    {
        if (pesos.Count != candidatos.Count) return false;

        for (int i = 0; i < pesos.Count; i++)
        {
            if (pesos[i] != candidatos[i]) { return false; }
        }

        return true;
    }

    public static bool ListagemContemSequencia(Lances ars, Lance l)
    {
        l.Lista.Sort();
        foreach (Lance o in ars)
        {
            o.Lista.Sort();
            if (ComparaListasDeInteiro(o.Lista, l.Lista)) return true;
        }

        return false;

    }

    public static int Posto(Lances aBase, Lance o)
    {
        int a = 0;

        foreach (Lance p in aBase)
        {
            if (Contapontos(o, p) == p.Lista.Count) { a = p.Id; break; }
        }


        return a;

    }

    public static Lances DevolveBaseCompleta(Lance oAlvo, int v)
    {
        Lances ars = new();
        foreach (Lance o in arGeral)
        {
            if (Contapontos(o, oAlvo) == v) ars.Add(o);
        }

        return ars;
    }

    public static Lances DevolveBaseAleatoria(int quantidade)
    {
        Lances ars = new();
        Random random = new();

        while (ars.Count < quantidade)
        {
            ars.Add(arGeral[random.Next(arGeral.Count)]);
        }

        return ars;
    }

    public static Lances DevolveBaseAleatoria(Lances arG, int quantidade)
    {
        Lances ars = new();
        Random random = new();

        while (ars.Count < quantidade) { ars.Add(arG[random.Next(arG.Count)]); }

        return ars;
    }

    public static Lances Aleatorios(Lances abase, int quantidade)
    {
        Lances ars = new();
        Random random = new();

        while (ars.Count < quantidade)
        {
            ars.Add(abase[random.Next(abase.Count)]);
        }

        return ars;
    }

    public static Lances Ocorrencias(Lance o, Lances arGerador, int v)
    {
        Lances artmp = new();
        foreach (Lance p in arGerador)
        {
            if (Infra.Contapontos(o, p) == v) { artmp.Add(p); }
        }

        return artmp;
    }

    public static Lances CombinaLances(Lances As, Lances Bs)
    {
        Lances ars = new();

        foreach (Lance o in As)
        {
            foreach (Lance p in Bs)
            {
                List<int> list = new List<int>();

                list.AddRange(o.Lista);
                list.AddRange(p.Lista);

                list.Sort();

                ars.Add(new Lance(ars.Count, list));
            }
        }


        return ars;
    }

    public static Lance UnirLances(Lance E6, Lance E9)
    {
        List<int> ints = new();

        ints.AddRange(E6.Lista);
        ints.AddRange(E9.Lista);
        ints.Sort();


        return new Lance(0, ints);
    }

    public static Lance UnirLances(List<int> E6, List<int> E9)
    {
        List<int> ints = new();

        ints.AddRange(E6);
        ints.AddRange(E9);
        ints.Sort();


        return new Lance(0, ints);
    }


    public static Lances DevolveBaseGeralFiltrada(Lances arBase, Lance oAlvo, int v)
    {
        var resultadoParalelo = arBase.AsParallel()
                                      .Where(o => Contapontos(o, oAlvo) == v)
                                      .ToList();

        return new Lances(resultadoParalelo);
    }


    public static Lances DevolveBaseGeralFiltrada(Lances arBaseT, Lances arBase2, int v, int z)
    {
        ConcurrentBag<Lance> arsConcurrent = new ConcurrentBag<Lance>();
        Lances ars = new();

        arBaseT.AsParallel().ForAll(o =>
        {
            foreach (Lance p in arBase2)
            {
                int a = Infra.Contapontos(o, p);
                if (a >= v && a <= z) arsConcurrent.Add(p);
            }
        });

        ars.AddRange(arsConcurrent.ToList());

        return ars;
    }

    public static List<int> PontuacaoVizinhos(Lances arGeral, Lance aBase6, Lance aBase9)
    {
        List<int> list = new List<int>();

        foreach (Lance p in arGeral)
        {
            int a = 6 - Infra.Contapontos(p, aBase6);
            int b = 9 - Infra.Contapontos(p, aBase9);

            list.Add(a + b);
        }
        return list;
    }




    #endregion
}
