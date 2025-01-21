using LotoLibrary.Constantes;
using LotoLibrary.Models;
using Newtonsoft.Json;
using System;
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

<<<<<<< HEAD
    public static string NomeSaida(string nome, string alvo)
    {

        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;

        string caminho = path + "\\" + "Saida" + "\\" + alvo.ToString();

        if (!Directory.Exists(caminho)) { Directory.CreateDirectory(caminho); }

        return Path.Combine(caminho + "\\" + nome + "-" + alvo + ".txt");
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

    public static Lances DevolveBaseGeralFiltrada(Lances arBase, Lance oAlvo, int v)
    {
        var resultadoParalelo = arBase.AsParallel()
                                      .Where(o => Contapontos(o, oAlvo) == v)
                                      .ToList();

        return new Lances(resultadoParalelo);
    }
    public static void CombinarGeral()
    {
        List<int> N = Enumerable.Range(1, 25).ToList();
        arGeral = GerarCombinacoes.Combinar25a15(N);
    }


    #endregion
}
=======
    public static bool ArquivoExiste(string nomeArquivo)
    {
        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;
        DirectoryInfo diretorio = new(path);

        // Garante que o nome do arquivo tenha a extensão .txt
        if (!nomeArquivo.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            nomeArquivo += ".txt";
        }

        // Procura o arquivo em todos os subdiretórios
        FileInfo[] arquivos = diretorio.GetFiles("*.txt", SearchOption.AllDirectories);

        return arquivos.Any(arquivo => arquivo.Name.Equals(nomeArquivo, StringComparison.OrdinalIgnoreCase));
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

    public static void SalvaSaidaW<T>(IEnumerable<T> matrizSaida, string nome)
    {
        StringBuilder salvaKey = new();

        foreach (var item in matrizSaida)
        {
            salvaKey.AppendLine(item?.ToString() ?? string.Empty);
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

    #endregion

    #region "EXTRAS"

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

    public static string NomeSaida(string nome, string alvo)
    {

        string path = Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1;

        string caminho = path + "\\" + "Saida" + "\\" + alvo.ToString();

        if (!Directory.Exists(caminho)) { Directory.CreateDirectory(caminho); }

        return Path.Combine(caminho + "\\" + nome + "-" + alvo + ".txt");
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

    public static Lances DevolveBaseGeralFiltrada(Lances arBase, Lance oAlvo, int v)
    {
        var resultadoParalelo = arBase.AsParallel()
                                      .Where(o => Contapontos(o, oAlvo) == v)
                                      .ToList();

        return new Lances(resultadoParalelo);
    }


    public static Lance DevolveMaisFrequentes(Lances L, int T)
    {

        List<int> arSaida = new();
        List<int> arSaidaTmp = new();
        List<int> aBase = DevolveListaZerada(25);
        Dictionary<int, int> Uva = new Dictionary<int, int>();

        foreach (Lance o in L)
        {
            foreach (int p in o.Lista)
            {
                aBase[p - 1]++;
            }
        }


        for (int k = 0; k < aBase.Count; k++)
        {
            Uva.Add(k + 1, aBase[k]);

        }

        var ls = Uva.OrderByDescending(key => key.Value);

        foreach (var item in ls)
        {
            arSaidaTmp.Add(item.Key);
        }

        for (int i = 0; i < T; i++)
        {
            arSaida.Add(arSaidaTmp[i]);
        }

        //  arSaida.Sort();

        Lance U = new Lance(0, arSaida);

        return U;
    }

    public static Lance DevolveMaisFrequentes(Lances L, int T, int score)
    {

        List<int> arSaida = new();
        List<int> arSaidaTmp = new();
        List<int> arSaidaCont = new();
        List<int> aBase = DevolveListaZerada(25);
        Dictionary<int, int> Uva = new Dictionary<int, int>();

        foreach (Lance o in L)
        {
            foreach (int p in o.Lista)
            {
                aBase[p - 1]++;
            }
        }


        for (int k = 0; k < aBase.Count; k++)
        {
            Uva.Add(k + 1, aBase[k]);

        }

        var ls = Uva.OrderByDescending(key => key.Value);


        foreach (var item in ls)
        {
            arSaidaTmp.Add(item.Key);
            arSaidaCont.Add(item.Value);
        }

        for (int i = 0; i < T; i++)
        {
            arSaida.Add(arSaidaTmp[i]);
        }

        int x = 0;


        for (int i = 0; i < T; i++)
        {
            var z = arSaidaCont[i];
            if (z == score) { x++; };
        }


        arSaida.Sort();

        Lance U = new Lance(x, arSaida);


        return U;
    }

    public static List<int> DevolveListaZerada(int i)
    {
        List<int> L = new();
        for (int z = 0; z < i; z++)
        {
            L.Add(0);
        }

        return L;
    }
    public static void CombinarGeral()
    {
        List<int> N = Enumerable.Range(1, 25).ToList();
        GerarCombinacoes.Combinar25a15(N);
    }

    public static void CombinarGeralB()
    {
        List<int> N = Enumerable.Range(1, 25).ToList();
        arGeral = GerarCombinacoes.Combinar25a15(N);
    }

    public static Lances CarregaLotoData()
    {
        Lances ars = new Lances();

        Random random = new Random();

        CombinarGeralB();

        foreach (Lance o in arLoto) { ars.Add(o); }

        int a = Infra.arLoto.Count;
        int y = 0;

        while (ars.Count < 75000)
        {
            Lance lance = ars[a - 1];

            while (y != 9)
            {
                int x = random.Next(arGeral.Count);

                Lance lance1 = arGeral[x];

                y = Infra.Contapontos(lance, lance1);
                if (y == 9) { ars.Add(lance1); a++; }

            }
            y = 0;

        }

        return ars;
    }


    #endregion
}
>>>>>>> 2f7142047f70fe1c8a9078c3800374577c0c0b29
