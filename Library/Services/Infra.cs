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

    #region "COMBINAÇÕES PRINCIPAIS"


    public static void CombinarGeral()
    {

        arGeral.AddRange(Combinar25());
    }

    public static void CombinarGeralFiltrado()
    {

        arGeralFiltrado.AddRange(Combinar25Filtrado());
    }

    // Combina 5 - 2 a 2
    public static Lances Combinar5a2(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                List<int> ls = new()
                            {
                                lst[A],
                                lst[B]

                            };

                Lance u = new Lance(ARS.Count, ls);
                ARS.Add(u);
            }
        }

        return ARS;
    }

    // Combina 4 - 3 a 3
    public static Lances Combinar4a3(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 1; A++)
        {
            for (var B = A + 1; B <= 2; B++)
            {
                for (var C = B + 1; C <= 3; C++)
                {

                    List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C]

                            };

                    Lance u = new Lance(ARS.Count, ls);
                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }

    // Combina 5 - 3 a 3
    public static Lances Combinar5a3(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 2; A++)
        {
            for (var B = A + 1; B <= 3; B++)
            {
                for (var C = B + 1; C <= 4; C++)
                {

                    List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C]

                            };

                    Lance u = new Lance(ARS.Count, ls);
                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }

    // Combina 6 - 3 a 3
    public static Lances Combinar6a3(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {

                    List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C]
                            };

                    Lance u = new Lance(ARS.Count, ls);
                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }

    // Combina 6 - 4 a 4
    public static Lances Combinar6a4(Lance oAlvo)
    {
        Lances ARS = new();

        for (var A = 0; A <= 2; A++)
        {
            for (var B = A + 1; B <= 3; B++)
            {
                for (var C = B + 1; C <= 4; C++)
                {
                    for (var D = C + 1; D <= 5; D++)
                    {

                        List<int> ls = new()
                            {
                                oAlvo.Lista[A],
                                oAlvo.Lista[B],
                                oAlvo.Lista[C],
                                oAlvo.Lista[D]
                            };

                        Lance u = new Lance(ARS.Count, ls);
                        ARS.Add(u);
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 6 - 5 a 5
    public static Lances Combinar6a5(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 1; A++)
        {
            for (var B = A + 1; B <= 2; B++)
            {
                for (var C = B + 1; C <= 3; C++)
                {
                    for (var D = C + 1; D <= 4; D++)
                    {
                        for (var E = D + 1; E <= 5; E++)
                        {
                            List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C],
                                lst[D],
                                lst[E]
                            };

                            Lance u = new Lance(ARS.Count, ls);
                            ARS.Add(u);
                        }
                    }
                }
            }
        }

        return ARS;
    }


    // Combina 7 - 4 a 4
    public static Lances Combinar7a4(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {
                    for (var D = C + 1; D <= 6; D++)
                    {

                        List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C],
                                lst[D],

                            };

                        Lance u = new Lance(ARS.Count, ls);
                        ARS.Add(u);

                    }
                }
            }
        }

        return ARS;
    }

    // Combina 8 - 5 a 5
    public static Lances Combinar8a5(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {
                    for (var D = C + 1; D <= 6; D++)
                    {
                        for (var E = D + 1; E <= 7; E++)
                        {
                            List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C],
                                lst[D],
                                lst[E]
                            };

                            Lance u = new Lance(ARS.Count, ls);
                            ARS.Add(u);
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 9 - 3 a 3
    public static Lances Combinar9a3(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 6; A++)
        {
            for (var B = A + 1; B <= 7; B++)
            {
                for (var C = B + 1; C <= 8; C++)
                {

                    List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C]

                                        };

                    Lance u = new Lance(ARS.Count, ls);

                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }

    // Combina 9 - 5 a 5
    public static Lances Combinar9a5(Lance oAlvo)
    {

        Lances ARS = new();

        for (var A = 0; A <= 4; A++)
        {
            for (var B = A + 1; B <= 5; B++)
            {
                for (var C = B + 1; C <= 6; C++)
                {
                    for (var D = C + 1; D <= 7; D++)
                    {
                        for (var E = D + 1; E <= 8; E++)
                        {
                            List<int> ls = new()
                            {
                                oAlvo.Lista[A],
                                oAlvo.Lista[B],
                                oAlvo.Lista[C],
                                oAlvo.Lista[D],
                                oAlvo.Lista[E]
                            };

                            Lance u = new Lance(ARS.Count, ls);
                            ARS.Add(u);
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 9 - 6 a 6
    public static Lances Combinar9a6(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {
                    for (var D = C + 1; D <= 6; D++)
                    {
                        for (var E = D + 1; E <= 7; E++)
                        {
                            for (var F = E + 1; F <= 8; F++)
                            {
                                List<int> ls = new()
                                {
                                    lst[A],
                                    lst[B],
                                    lst[C],
                                    lst[D],
                                    lst[E],
                                    lst[F]
                                };

                                Lance u = new Lance(ARS.Count, ls);
                                ARS.Add(u);
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 10 - 3 a 3
    public static Lances Combinar10a3(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 7; A++)
        {
            for (var B = A + 1; B <= 8; B++)
            {
                for (var C = B + 1; C <= 9; C++)
                {

                    List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C]

                            };

                    Lance u = new Lance(ARS.Count, ls);
                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }


    // Combina 10 - 4 a 4
    public static Lances Combinar10a4(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 6; A++)
        {
            for (var B = A + 1; B <= 7; B++)
            {
                for (var C = B + 1; C <= 8; C++)
                {
                    for (var D = C + 1; D <= 9; D++)
                    {

                        List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C],
                                lst[D],

                            };

                        Lance u = new Lance(ARS.Count, ls);
                        ARS.Add(u);

                    }
                }
            }
        }

        return ARS;
    }

    // Combina 10 - 5 a 5
    public static Lances Combinar10a5(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 5; A++)
        {
            for (var B = A + 1; B <= 6; B++)
            {
                for (var C = B + 1; C <= 7; C++)
                {
                    for (var D = C + 1; D <= 8; D++)
                    {
                        for (var E = D + 1; E <= 9; E++)
                        {
                            List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C],
                                lst[D],
                                lst[E]
                            };

                            Lance u = new Lance(ARS.Count, ls);
                            ARS.Add(u);
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 10 - 6 a 6
    public static Lances Combinar10a6(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 4; A++)
        {
            for (var B = A + 1; B <= 5; B++)
            {
                for (var C = B + 1; C <= 6; C++)
                {
                    for (var D = C + 1; D <= 7; D++)
                    {
                        for (var E = D + 1; E <= 8; E++)
                        {
                            for (var F = E + 1; F <= 9; F++)
                            {
                                List<int> ls = new()
                                {
                                    lst[A],
                                    lst[B],
                                    lst[C],
                                    lst[D],
                                    lst[E],
                                    lst[F]
                                };

                                Lance u = new Lance(ARS.Count, ls);
                                ARS.Add(u);
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 10 - 7 a 7
    public static Lances Combinar10a7(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {
                    for (var D = C + 1; D <= 6; D++)
                    {
                        for (var E = D + 1; E <= 7; E++)
                        {
                            for (var F = E + 1; F <= 8; F++)
                            {
                                for (var G = F + 1; G <= 9; G++)
                                {
                                    List<int> ls = new()
                                    {
                                        lst[A],
                                        lst[B],
                                        lst[C],
                                        lst[D],
                                        lst[E],
                                        lst[F],
                                        lst[G]
                                    };

                                    Lance u = new Lance(ARS.Count, ls);
                                    ARS.Add(u);
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 10 - 8 a 8
    public static Lances Combinar10a8(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {
                    for (var D = C + 1; D <= 6; D++)
                    {
                        for (var E = D + 1; E <= 7; E++)
                        {
                            for (var F = E + 1; F <= 8; F++)
                            {
                                for (var G = F + 1; G <= 9; G++)
                                {
                                    for (var H = G + 1; H <= 9; H++)
                                    {
                                        List<int> ls = new()
                                    {
                                        lst[A],
                                        lst[B],
                                        lst[C],
                                        lst[D],
                                        lst[E],
                                        lst[F],
                                        lst[G],
                                         lst[H]
                                    };

                                        Lance u = new Lance(ARS.Count, ls);
                                        ARS.Add(u);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 12 - 6 a 6
    public static Lances Combinar12a6(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 6; A++)
        {
            for (var B = A + 1; B <= 7; B++)
            {
                for (var C = B + 1; C <= 8; C++)
                {
                    for (var D = C + 1; D <= 9; D++)
                    {
                        for (var E = D + 1; E <= 10; E++)
                        {
                            for (var F = E + 1; F <= 11; F++)
                            {
                                List<int> ls = new()
                                {
                                    lst[A],
                                    lst[B],
                                    lst[C],
                                    lst[D],
                                    lst[E],
                                    lst[F]
                                };

                                Lance u = new Lance(ARS.Count, ls);
                                ARS.Add(u);
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 12 - 7 a 7
    public static Lances Combinar12a7(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 5; A++)
        {
            for (var B = A + 1; B <= 6; B++)
            {
                for (var C = B + 1; C <= 7; C++)
                {
                    for (var D = C + 1; D <= 8; D++)
                    {
                        for (var E = D + 1; E <= 9; E++)
                        {
                            for (var F = E + 1; F <= 10; F++)
                            {
                                for (var G = F + 1; G <= 11; G++)
                                {


                                    List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C],
                                            lst[D],
                                            lst[E],
                                            lst[F],
                                            lst[G],

                                        };


                                    Lance u = new Lance(ARS.Count, ls);

                                    ARS.Add(u);


                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 12 - 8 a 8
    public static Lances Combinar12a8(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 4; A++)
        {
            for (var B = A + 1; B <= 5; B++)
            {
                for (var C = B + 1; C <= 6; C++)
                {
                    for (var D = C + 1; D <= 7; D++)
                    {
                        for (var E = D + 1; E <= 8; E++)
                        {
                            for (var F = E + 1; F <= 9; F++)
                            {
                                for (var G = F + 1; G <= 10; G++)
                                {
                                    for (var H = G + 1; H <= 11; H++)
                                    {

                                        List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C],
                                            lst[D],
                                            lst[E],
                                            lst[F],
                                            lst[G],
                                             lst[H],

                                        };


                                        Lance u = new Lance(ARS.Count, ls);

                                        ARS.Add(u);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 13 - 7 a 7
    public static Lances Combinar13a7(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 6; A++)
        {
            for (var B = A + 1; B <= 7; B++)
            {
                for (var C = B + 1; C <= 8; C++)
                {
                    for (var D = C + 1; D <= 9; D++)
                    {
                        for (var E = D + 1; E <= 10; E++)
                        {
                            for (var F = E + 1; F <= 11; F++)
                            {
                                for (var G = F + 1; G <= 12; G++)
                                {


                                    List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C],
                                            lst[D],
                                            lst[E],
                                            lst[F],
                                            lst[G],

                                        };


                                    Lance u = new Lance(ARS.Count, ls);

                                    ARS.Add(u);


                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }


    // Combina 13 - 12 a 12
    public static Lances Combinar13a12(Lance oLance)
    {
        Lances ARS = new();

        List<int> lstX = new();
        lstX.AddRange(oLance.Lista);

        for (var A = 0; A <= 1; A++)
        {
            for (var B = A + 1; B <= 2; B++)
            {
                for (var C = B + 1; C <= 3; C++)
                {
                    for (var D = C + 1; D <= 4; D++)
                    {
                        for (var E = D + 1; E <= 5; E++)
                        {
                            for (var F = E + 1; F <= 6; F++)
                            {
                                for (var G = F + 1; G <= 7; G++)
                                {
                                    for (var H = G + 1; H <= 8; H++)
                                    {
                                        for (var I = H + 1; I <= 9; I++)
                                        {
                                            for (var J = I + 1; J <= 10; J++)
                                            {
                                                for (var K = J + 1; K <= 11; K++)
                                                {
                                                    for (var L = K + 1; L <= 12; L++)
                                                    {
                                                        List<int> ls = new()
                                                                    {
                                                                       lstX[A],
                                                                       lstX[B],
                                                                       lstX[C],
                                                                       lstX[D],
                                                                       lstX[E],
                                                                       lstX[F],
                                                                       lstX[G],
                                                                       lstX[H],
                                                                       lstX[I],
                                                                       lstX[J],
                                                                       lstX[K],
                                                                       lstX[L],

                                                                    };

                                                        Lance u = new Lance(ARS.Count, ls);

                                                        ARS.Add(u);


                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 14 - 13 a 13
    public static Lances Combinar14a13(Lance lance)
    {
        List<int> lst = new();

        lst.AddRange(lance.Lista);

        Lances ARS = new();

        for (var A = 0; A <= 1; A++)
        {
            for (var B = A + 1; B <= 2; B++)
            {
                for (var C = B + 1; C <= 3; C++)
                {
                    for (var D = C + 1; D <= 4; D++)
                    {
                        for (var E = D + 1; E <= 5; E++)
                        {
                            for (var F = E + 1; F <= 6; F++)
                            {
                                for (var G = F + 1; G <= 7; G++)
                                {
                                    for (var H = G + 1; H <= 8; H++)
                                    {
                                        for (var I = H + 1; I <= 9; I++)
                                        {
                                            for (var J = I + 1; J <= 10; J++)
                                            {
                                                for (var K = J + 1; K <= 11; K++)
                                                {
                                                    for (var L = K + 1; L <= 12; L++)
                                                    {
                                                        for (var M = L + 1; M <= 13; M++)
                                                        {
                                                            List<int> ls = new()
                                                            {
                                                                lst[A],
                                                                lst[B],
                                                                lst[C],
                                                                lst[D],
                                                                lst[E],
                                                                lst[F],
                                                                lst[G],
                                                                lst[H],
                                                                lst[I],
                                                                lst[J],
                                                                lst[K],
                                                                lst[L],
                                                                lst[M]
                                                            };

                                                            Lance u = new Lance(ARS.Count, ls);

                                                            ARS.Add(u);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 2 a 2
    public static Lances Combinar15a2(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 13; A++)
        {
            for (var B = A + 1; B <= 14; B++)
            {

                {
                    List<int> ls = new()
                                                {
                                                    lst[A],
                                                    lst[B]
                                                };

                    Lance u = new Lance(ARS.Count, ls);

                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 3 a 3
    public static Lances Combinar15a3(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 12; A++)
        {
            for (var B = A + 1; B <= 13; B++)
            {
                for (var C = B + 1; C <= 14; C++)
                {

                    List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C]

                                        };

                    Lance u = new Lance(ARS.Count, ls);

                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 5 a 5
    public static Lances Combinar15a5(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 10; A++)
        {
            for (var B = A + 1; B <= 11; B++)
            {
                for (var C = B + 1; C <= 12; C++)
                {
                    for (var D = C + 1; D <= 13; D++)
                    {
                        for (var E = D + 1; E <= 14; E++)
                        {

                            List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C],
                                            lst[D],
                                            lst[E]
                                        };

                            Lance u = new Lance(ARS.Count, ls);

                            ARS.Add(u);
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 6 a 6
    public static Lances Combinar15a6(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 9; A++)
        {
            for (var B = A + 1; B <= 10; B++)
            {
                for (var C = B + 1; C <= 11; C++)
                {
                    for (var D = C + 1; D <= 12; D++)
                    {
                        for (var E = D + 1; E <= 13; E++)
                        {
                            for (var F = E + 1; F <= 14; F++)
                            {
                                List<int> ls = new()
                                {
                                    lst[A],
                                    lst[B],
                                    lst[C],
                                    lst[D],
                                    lst[E],
                                    lst[F]
                                };

                                Lance u = new Lance(ARS.Count, ls);
                                ARS.Add(u);
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 7 a 7
    public static Lances Combinar15a7(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 8; A++)
        {
            for (var B = A + 1; B <= 9; B++)
            {
                for (var C = B + 1; C <= 10; C++)
                {
                    for (var D = C + 1; D <= 11; D++)
                    {
                        for (var E = D + 1; E <= 12; E++)
                        {
                            for (var F = E + 1; F <= 13; F++)
                            {
                                for (var G = F + 1; G <= 14; G++)
                                {


                                    List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C],
                                            lst[D],
                                            lst[E],
                                            lst[F],
                                            lst[G]

                                        };


                                    Lance u = new Lance(ARS.Count, ls);

                                    ARS.Add(u);


                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 8 a 8
    public static Lances Combinar15a8(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 7; A++)
        {
            for (var B = A + 1; B <= 8; B++)
            {
                for (var C = B + 1; C <= 9; C++)
                {
                    for (var D = C + 1; D <= 10; D++)
                    {
                        for (var E = D + 1; E <= 11; E++)
                        {
                            for (var F = E + 1; F <= 12; F++)
                            {
                                for (var G = F + 1; G <= 13; G++)
                                {
                                    for (var H = G + 1; H <= 14; H++)
                                    {

                                        List<int> ls = new()
                                        {
                                            lst[A],
                                            lst[B],
                                            lst[C],
                                            lst[D],
                                            lst[E],
                                            lst[F],
                                            lst[G],
                                            lst[H]
                                        };


                                        Lance u = new Lance(ARS.Count, ls);

                                        ARS.Add(u);

                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 9 a 9
    public static Lances Combinar15a9(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 6; A++)
        {
            for (var B = A + 1; B <= 7; B++)
            {
                for (var C = B + 1; C <= 8; C++)
                {
                    for (var D = C + 1; D <= 9; D++)
                    {
                        for (var E = D + 1; E <= 10; E++)
                        {
                            for (var F = E + 1; F <= 11; F++)
                            {
                                for (var G = F + 1; G <= 12; G++)
                                {
                                    for (var H = G + 1; H <= 13; H++)
                                    {
                                        for (var I = H + 1; I <= 14; I++)
                                        {
                                            List<int> ls = new()
                                            {
                                                lst[A],
                                                lst[B],
                                                lst[C],
                                                lst[D],
                                                lst[E],
                                                lst[F],
                                                lst[G],
                                                lst[H],
                                                lst[I]
                                            };

                                            Lance u = new Lance(ARS.Count, ls);

                                            ARS.Add(u);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 10 a 10
    public static Lances Combinar15a10(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 5; A++)
        {
            for (var B = A + 1; B <= 6; B++)
            {
                for (var C = B + 1; C <= 7; C++)
                {
                    for (var D = C + 1; D <= 8; D++)
                    {
                        for (var E = D + 1; E <= 9; E++)
                        {
                            for (var F = E + 1; F <= 10; F++)
                            {
                                for (var G = F + 1; G <= 11; G++)
                                {
                                    for (var H = G + 1; H <= 12; H++)
                                    {
                                        for (var I = H + 1; I <= 13; I++)
                                        {
                                            for (var J = I + 1; J <= 14; J++)
                                            {
                                                List<int> ls = new()
                                                {
                                                    lst[A],
                                                    lst[B],
                                                    lst[C],
                                                    lst[D],
                                                    lst[E],
                                                    lst[F],
                                                    lst[G],
                                                    lst[H],
                                                    lst[I],
                                                    lst[J]
                                                };

                                                Lance u = new Lance(ARS.Count, ls);

                                                ARS.Add(u);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 11 a 11
    public static Lances Combinar15a11(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 4; A++)
        {
            for (var B = A + 1; B <= 5; B++)
            {
                for (var C = B + 1; C <= 6; C++)
                {
                    for (var D = C + 1; D <= 7; D++)
                    {
                        for (var E = D + 1; E <= 8; E++)
                        {
                            for (var F = E + 1; F <= 9; F++)
                            {
                                for (var G = F + 1; G <= 10; G++)
                                {
                                    for (var H = G + 1; H <= 11; H++)
                                    {
                                        for (var I = H + 1; I <= 12; I++)
                                        {
                                            for (var J = I + 1; J <= 13; J++)
                                            {
                                                for (var K = J + 1; K <= 14; K++)
                                                {

                                                    List<int> ls = new()
                                                {
                                                    lst[A],
                                                    lst[B],
                                                    lst[C],
                                                    lst[D],
                                                    lst[E],
                                                    lst[F],
                                                    lst[G],
                                                    lst[H],
                                                    lst[I],
                                                    lst[J],
                                                    lst[K]
                                                };

                                                    Lance u = new Lance(ARS.Count, ls);

                                                    ARS.Add(u);

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 12 a 12
    public static Lances Combinar15a12(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {
                    for (var D = C + 1; D <= 6; D++)
                    {
                        for (var E = D + 1; E <= 7; E++)
                        {
                            for (var F = E + 1; F <= 8; F++)
                            {
                                for (var G = F + 1; G <= 9; G++)
                                {
                                    for (var H = G + 1; H <= 10; H++)
                                    {
                                        for (var I = H + 1; I <= 11; I++)
                                        {
                                            for (var J = I + 1; J <= 12; J++)
                                            {
                                                for (var K = J + 1; K <= 13; K++)
                                                {
                                                    for (var L = K + 1; L <= 14; L++)
                                                    {
                                                        List<int> ls = new()
                                                {
                                                    lst[A],
                                                    lst[B],
                                                    lst[C],
                                                    lst[D],
                                                    lst[E],
                                                    lst[F],
                                                    lst[G],
                                                    lst[H],
                                                    lst[I],
                                                    lst[J],
                                                    lst[K],
                                                    lst[L]
                                                };

                                                        Lance u = new Lance(ARS.Count, ls);

                                                        ARS.Add(u);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 13 a 13
    public static Lances Combinar15a13(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 2; A++)
        {
            for (var B = A + 1; B <= 3; B++)
            {
                for (var C = B + 1; C <= 4; C++)
                {
                    for (var D = C + 1; D <= 5; D++)
                    {
                        for (var E = D + 1; E <= 6; E++)
                        {
                            for (var F = E + 1; F <= 7; F++)
                            {
                                for (var G = F + 1; G <= 8; G++)
                                {
                                    for (var H = G + 1; H <= 9; H++)
                                    {
                                        for (var I = H + 1; I <= 10; I++)
                                        {
                                            for (var J = I + 1; J <= 11; J++)
                                            {
                                                for (var K = J + 1; K <= 12; K++)
                                                {
                                                    for (var L = K + 1; L <= 13; L++)
                                                    {
                                                        for (var M = L + 1; M <= 14; M++)
                                                        {
                                                            List<int> ls = new()
                                                            {
                                                                lst[A],
                                                                lst[B],
                                                                lst[C],
                                                                lst[D],
                                                                lst[E],
                                                                lst[F],
                                                                lst[G],
                                                                lst[H],
                                                                lst[I],
                                                                lst[J],
                                                                lst[K],
                                                                lst[L],
                                                                lst[M]
                                                            };

                                                            Lance u = new Lance(ARS.Count, ls);

                                                            ARS.Add(u);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 15 - 14 a 14
    public static Lances Combinar15a14(Lance oLance)
    {
        Lances ARS = new();

        List<int> lstX = new();
        lstX.AddRange(oLance.Lista);

        for (var A = 0; A <= 1; A++)
        {
            for (var B = A + 1; B <= 2; B++)
            {
                for (var C = B + 1; C <= 3; C++)
                {
                    for (var D = C + 1; D <= 4; D++)
                    {
                        for (var E = D + 1; E <= 5; E++)
                        {
                            for (var F = E + 1; F <= 6; F++)
                            {
                                for (var G = F + 1; G <= 7; G++)
                                {
                                    for (var H = G + 1; H <= 8; H++)
                                    {
                                        for (var I = H + 1; I <= 9; I++)
                                        {
                                            for (var J = I + 1; J <= 10; J++)
                                            {
                                                for (var K = J + 1; K <= 11; K++)
                                                {
                                                    for (var L = K + 1; L <= 12; L++)
                                                    {
                                                        for (var M = L + 1; M <= 13; M++)
                                                        {
                                                            for (var N = M + 1; N <= 14; N++)
                                                            {

                                                                List<int> ls = new()
                                                                    {
                                                                       lstX[A],
                                                                       lstX[B],
                                                                       lstX[C],
                                                                       lstX[D],
                                                                       lstX[E],
                                                                       lstX[F],
                                                                       lstX[G],
                                                                       lstX[H],
                                                                       lstX[I],
                                                                       lstX[J],
                                                                       lstX[K],
                                                                       lstX[L],
                                                                       lstX[M],
                                                                       lstX[N],
                                                                    };

                                                                Lance u = new Lance(ARS.Count, ls);

                                                                ARS.Add(u);

                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 17 - 13 a 13
    public static Lances Combinar17a13(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 4; A++)
        {
            for (var B = A + 1; B <= 5; B++)
            {
                for (var C = B + 1; C <= 6; C++)
                {
                    for (var D = C + 1; D <= 7; D++)
                    {
                        for (var E = D + 1; E <= 8; E++)
                        {
                            for (var F = E + 1; F <= 9; F++)
                            {
                                for (var G = F + 1; G <= 10; G++)
                                {
                                    for (var H = G + 1; H <= 11; H++)
                                    {
                                        for (var I = H + 1; I <= 12; I++)
                                        {
                                            for (var J = I + 1; J <= 13; J++)
                                            {
                                                for (var K = J + 1; K <= 14; K++)
                                                {
                                                    for (var L = K + 1; L <= 15; L++)
                                                    {
                                                        for (var M = L + 1; M <= 16; M++)
                                                        {
                                                            List<int> ls = new()
                                                            {
                                                                lst[A],
                                                                lst[B],
                                                                lst[C],
                                                                lst[D],
                                                                lst[E],
                                                                lst[F],
                                                                lst[G],
                                                                lst[H],
                                                                lst[I],
                                                                lst[J],
                                                                lst[K],
                                                                lst[L],
                                                                lst[M]
                                                            };

                                                            Lance u = new Lance(ARS.Count, ls);

                                                            ARS.Add(u);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 17 - 15 a 15
    public static Lances Combinar17a15(Lance oLance)
    {
        Lances ARS = new();

        List<int> lstX = new();
        lstX.AddRange(oLance.Lista);

        for (var A = 0; A <= 2; A++)
        {
            for (var B = A + 1; B <= 3; B++)
            {
                for (var C = B + 1; C <= 4; C++)
                {
                    for (var D = C + 1; D <= 5; D++)
                    {
                        for (var E = D + 1; E <= 6; E++)
                        {
                            for (var F = E + 1; F <= 7; F++)
                            {
                                for (var G = F + 1; G <= 8; G++)
                                {
                                    for (var H = G + 1; H <= 9; H++)
                                    {
                                        for (var I = H + 1; I <= 10; I++)
                                        {
                                            for (var J = I + 1; J <= 11; J++)
                                            {
                                                for (var K = J + 1; K <= 12; K++)
                                                {
                                                    for (var L = K + 1; L <= 13; L++)
                                                    {
                                                        for (var M = L + 1; M <= 14; M++)
                                                        {
                                                            for (var N = M + 1; N <= 15; N++)
                                                            {
                                                                for (var O = N + 1; O <= 16; O++)
                                                                {
                                                                    List<int> ls = new()
                                                                    {
                                                                       lstX[A],
                                                                       lstX[B],
                                                                       lstX[C],
                                                                       lstX[D],
                                                                       lstX[E],
                                                                       lstX[F],
                                                                       lstX[G],
                                                                       lstX[H],
                                                                       lstX[I],
                                                                       lstX[J],
                                                                       lstX[K],
                                                                       lstX[L],
                                                                       lstX[M],
                                                                       lstX[N],
                                                                       lstX[O]
                                                                    };

                                                                    Lance u = new Lance(ARS.Count, ls);

                                                                    ARS.Add(u);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }


    // Combina 18 - 7 a 7
    public static Lances Combinar18a7(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 11; A++)
        {
            for (var B = A + 1; B <= 12; B++)
            {
                for (var C = B + 1; C <= 13; C++)
                {
                    for (var D = C + 1; D <= 14; D++)
                    {
                        for (var E = D + 1; E <= 15; E++)
                        {
                            for (var F = E + 1; F <= 16; F++)
                            {
                                for (var G = F + 1; G <= 17; G++)
                                {

                                    List<int> ls = new()
                                                {
                                                    lst[A],
                                                    lst[B],
                                                    lst[C],
                                                    lst[D],
                                                    lst[E],
                                                    lst[F],
                                                    lst[G]

                                                };

                                    Lance u = new Lance(ARS.Count, ls);

                                    ARS.Add(u);

                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 18 - 10 a 10
    public static Lances Combinar18a10(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 8; A++)
        {
            for (var B = A + 1; B <= 9; B++)
            {
                for (var C = B + 1; C <= 10; C++)
                {
                    for (var D = C + 1; D <= 11; D++)
                    {
                        for (var E = D + 1; E <= 12; E++)
                        {
                            for (var F = E + 1; F <= 13; F++)
                            {
                                for (var G = F + 1; G <= 14; G++)
                                {
                                    for (var H = G + 1; H <= 15; H++)
                                    {
                                        for (var I = H + 1; I <= 16; I++)
                                        {
                                            for (var J = I + 1; J <= 17; J++)
                                            {
                                                List<int> ls = new()
                                                {
                                                    lst[A],
                                                    lst[B],
                                                    lst[C],
                                                    lst[D],
                                                    lst[E],
                                                    lst[F],
                                                    lst[G],
                                                    lst[H],
                                                    lst[I],
                                                    lst[J]
                                                };

                                                Lance u = new Lance(ARS.Count, ls);

                                                ARS.Add(u);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 18 - 15 a 15
    public static Lances Combinar18a15(Lance oLance)
    {
        Lances ARS = new();

        List<int> lstX = new();
        lstX.AddRange(oLance.Lista);

        for (var A = 0; A <= 3; A++)
        {
            for (var B = A + 1; B <= 4; B++)
            {
                for (var C = B + 1; C <= 5; C++)
                {
                    for (var D = C + 1; D <= 6; D++)
                    {
                        for (var E = D + 1; E <= 7; E++)
                        {
                            for (var F = E + 1; F <= 8; F++)
                            {
                                for (var G = F + 1; G <= 9; G++)
                                {
                                    for (var H = G + 1; H <= 10; H++)
                                    {
                                        for (var I = H + 1; I <= 11; I++)
                                        {
                                            for (var J = I + 1; J <= 12; J++)
                                            {
                                                for (var K = J + 1; K <= 13; K++)
                                                {
                                                    for (var L = K + 1; L <= 14; L++)
                                                    {
                                                        for (var M = L + 1; M <= 15; M++)
                                                        {
                                                            for (var N = M + 1; N <= 16; N++)
                                                            {
                                                                for (var O = N + 1; O <= 17; O++)
                                                                {
                                                                    List<int> ls = new()
                                                                    {
                                                                       lstX[A],
                                                                       lstX[B],
                                                                       lstX[C],
                                                                       lstX[D],
                                                                       lstX[E],
                                                                       lstX[F],
                                                                       lstX[G],
                                                                       lstX[H],
                                                                       lstX[I],
                                                                       lstX[J],
                                                                       lstX[K],
                                                                       lstX[L],
                                                                       lstX[M],
                                                                       lstX[N],
                                                                       lstX[O]
                                                                    };

                                                                    Lance u = new Lance(ARS.Count, ls);

                                                                    ARS.Add(u);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 20 - 3 a 3
    public static Lances Combinar20a3(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 17; A++)
        {
            for (var B = A + 1; B <= 18; B++)
            {
                for (var C = B + 1; C <= 19; C++)
                {


                    List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C]

                            };

                    Lance u = new Lance(ARS.Count, ls);
                    ARS.Add(u);
                }
            }
        }

        return ARS;
    }

    // Combina 20 - 4 a 4
    public static Lances Combinar20a4(List<int> lst)
    {


        Lances ARS = new();

        for (var A = 0; A <= 16; A++)
        {
            for (var B = A + 1; B <= 17; B++)
            {
                for (var C = B + 1; C <= 18; C++)
                {
                    for (var D = C + 1; D <= 19; D++)
                    {

                        List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C],
                                lst[D]

                            };

                        Lance u = new Lance(ARS.Count, ls);
                        ARS.Add(u);

                    }
                }
            }
        }

        return ARS;
    }

    // Combina 20 - 5 a 5
    public static Lances Combinar20a5(List<int> lst)
    {


        Lances ARS = new();

        for (var A = 0; A <= 15; A++)
        {
            for (var B = A + 1; B <= 16; B++)
            {
                for (var C = B + 1; C <= 17; C++)
                {
                    for (var D = C + 1; D <= 18; D++)
                    {
                        for (var E = D + 1; E <= 19; E++)
                        {
                            List<int> ls = new()
                            {
                                lst[A],
                                lst[B],
                                lst[C],
                                lst[D],
                                lst[E]
                            };

                            Lance u = new Lance(ARS.Count, ls);
                            ARS.Add(u);
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 20 - 15 a 15
    public static Lances Combinar20a15(Lance oLance)
    {
        Lances ARS = new();

        List<int> lstX = new();
        lstX.AddRange(oLance.Lista);

        for (var A = 0; A <= 5; A++)
        {
            for (var B = A + 1; B <= 6; B++)
            {
                for (var C = B + 1; C <= 7; C++)
                {
                    for (var D = C + 1; D <= 8; D++)
                    {
                        for (var E = D + 1; E <= 9; E++)
                        {
                            for (var F = E + 1; F <= 10; F++)
                            {
                                for (var G = F + 1; G <= 11; G++)
                                {
                                    for (var H = G + 1; H <= 12; H++)
                                    {
                                        for (var I = H + 1; I <= 13; I++)
                                        {
                                            for (var J = I + 1; J <= 14; J++)
                                            {
                                                for (var K = J + 1; K <= 15; K++)
                                                {
                                                    for (var L = K + 1; L <= 16; L++)
                                                    {
                                                        for (var M = L + 1; M <= 17; M++)
                                                        {
                                                            for (var N = M + 1; N <= 18; N++)
                                                            {
                                                                for (var O = N + 1; O <= 19; O++)
                                                                {
                                                                    List<int> ls = new()
                                                                    {
                                                                       lstX[A],
                                                                       lstX[B],
                                                                       lstX[C],
                                                                       lstX[D],
                                                                       lstX[E],
                                                                       lstX[F],
                                                                       lstX[G],
                                                                       lstX[H],
                                                                       lstX[I],
                                                                       lstX[J],
                                                                       lstX[K],
                                                                       lstX[L],
                                                                       lstX[M],
                                                                       lstX[N],
                                                                       lstX[O]
                                                                    };

                                                                    Lance u = new Lance(ARS.Count, ls);

                                                                    ARS.Add(u);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }


    // Combina 20 - 18 a 18
    public static Lances Combinar20a18(Lance oLance)
    {
        Lances ARS = new();

        List<int> lstX = new();
        lstX.AddRange(oLance.Lista);

        for (var A = 0; A <= 2; A++)
        {
            for (var B = A + 1; B <= 3; B++)
            {
                for (var C = B + 1; C <= 4; C++)
                {
                    for (var D = C + 1; D <= 5; D++)
                    {
                        for (var E = D + 1; E <= 6; E++)
                        {
                            for (var F = E + 1; F <= 7; F++)
                            {
                                for (var G = F + 1; G <= 8; G++)
                                {
                                    for (var H = G + 1; H <= 9; H++)
                                    {
                                        for (var I = H + 1; I <= 10; I++)
                                        {
                                            for (var J = I + 1; J <= 11; J++)
                                            {
                                                for (var K = J + 1; K <= 12; K++)
                                                {
                                                    for (var L = K + 1; L <= 13; L++)
                                                    {
                                                        for (var M = L + 1; M <= 14; M++)
                                                        {
                                                            for (var N = M + 1; N <= 15; N++)
                                                            {
                                                                for (var O = N + 1; O <= 16; O++)
                                                                {
                                                                    for (var P = O + 1; O <= 17; O++)
                                                                    {

                                                                        for (var Q = P + 1; O <= 18; O++)
                                                                        {

                                                                            for (var R = Q + 1; O <= 19; O++)
                                                                            {
                                                                                List<int> ls = new()
                                                                    {
                                                                       lstX[A],
                                                                       lstX[B],
                                                                       lstX[C],
                                                                       lstX[D],
                                                                       lstX[E],
                                                                       lstX[F],
                                                                       lstX[G],
                                                                       lstX[H],
                                                                       lstX[I],
                                                                       lstX[J],
                                                                       lstX[K],
                                                                       lstX[L],
                                                                       lstX[M],
                                                                       lstX[N],
                                                                       lstX[O],
                                                                        lstX[P],
                                                                       lstX[Q],
                                                                       lstX[R]
                                                                    };

                                                                                Lance u = new Lance(ARS.Count, ls);

                                                                                ARS.Add(u);
                                                                            }
                                                                        }
                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }


    // Combina 21 - 8 a 8
    public static Lances Combinar21a8(Lance OL)
    {
        List<int> lst = OL.Lista;

        Lances ARS = new();


        for (var C = 0; C <= 13; C++)
        {
            for (var D = C + 1; D <= 14; D++)
            {
                for (var E = D + 1; E <= 15; E++)
                {
                    for (var F = E + 1; F <= 16; F++)
                    {
                        for (var G = F + 1; G <= 17; G++)
                        {
                            for (var H = G + 1; H <= 18; H++)
                            {
                                for (var I = H + 1; I <= 19; I++)
                                {
                                    for (var J = I + 1; J <= 20; J++)
                                    {
                                        List<int> ls = new()
                                                {

                                                    lst[C],
                                                    lst[D],
                                                    lst[E],
                                                    lst[F],
                                                    lst[G],
                                                    lst[H],
                                                    lst[I],
                                                    lst[J]
                                                };

                                        Lance u = new Lance(ARS.Count, ls);

                                        ARS.Add(u);
                                    }
                                }
                            }
                        }
                    }
                }
            }

        }

        return ARS;
    }
    // Combina 21 - 10 a 10
    public static Lances Combinar21a10(List<int> lst)
    {
        Lances ARS = new();

        for (var A = 0; A <= 11; A++)
        {
            for (var B = A + 1; B <= 12; B++)
            {
                for (var C = B + 1; C <= 13; C++)
                {
                    for (var D = C + 1; D <= 14; D++)
                    {
                        for (var E = D + 1; E <= 15; E++)
                        {
                            for (var F = E + 1; F <= 16; F++)
                            {
                                for (var G = F + 1; G <= 17; G++)
                                {
                                    for (var H = G + 1; H <= 18; H++)
                                    {
                                        for (var I = H + 1; I <= 19; I++)
                                        {
                                            for (var J = I + 1; J <= 20; J++)
                                            {
                                                List<int> ls = new()
                                                {
                                                    lst[A],
                                                    lst[B],
                                                    lst[C],
                                                    lst[D],
                                                    lst[E],
                                                    lst[F],
                                                    lst[G],
                                                    lst[H],
                                                    lst[I],
                                                    lst[J]
                                                };

                                                Lance u = new Lance(ARS.Count, ls);

                                                ARS.Add(u);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 2 a 2

    public static Lances Combinar25a2()
    {
        Lances ARS = new();

        for (var A = 0; A <= 23; A++)
        {
            for (var B = A + 1; B <= 24; B++)
            {
                List<int> ls = new() { A, B };

                Lance u = new Lance(ARS.Count, ls);

                ARS.Add(u);
            }
        }

        return ARS;
    }


    // Combina 25 - 6 a 6
    public static Lances Combinar25a6()
    {

        Lances ARS = new();

        for (var A = 0; A <= 19; A++)
        {
            for (var B = A + 1; B <= 20; B++)
            {
                for (var C = B + 1; C <= 21; C++)
                {
                    for (var D = C + 1; D <= 22; D++)
                    {
                        for (var E = D + 1; E <= 23; E++)
                        {
                            for (var F = E + 1; F <= 24; F++)
                            {
                                List<int> ls = new()
                                {
                                    Constante.ListaG[A],
                                    Constante.ListaG[B],
                                    Constante.ListaG[C],
                                    Constante.ListaG[D],
                                   Constante.ListaG[E],
                                    Constante.ListaG[F]
                                };

                                Lance u = new Lance(ARS.Count, ls);
                                ARS.Add(u);
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 15 a 15
    public static Lances Combinar25()
    {
        Lances ARS = new();

        for (var A = 1; A <= 11; A++)
        {
            for (var B = A + 1; B <= 12; B++)
            {
                for (var C = B + 1; C <= 13; C++)
                {
                    for (var D = C + 1; D <= 14; D++)
                    {
                        for (var E = D + 1; E <= 15; E++)
                        {
                            for (var F = E + 1; F <= 16; F++)
                            {
                                for (var G = F + 1; G <= 17; G++)
                                {
                                    for (var H = G + 1; H <= 18; H++)
                                    {
                                        for (var I = H + 1; I <= 19; I++)
                                        {
                                            for (var J = I + 1; J <= 20; J++)
                                            {
                                                for (var K = J + 1; K <= 21; K++)
                                                {
                                                    for (var L = K + 1; L <= 22; L++)
                                                    {
                                                        for (var M = L + 1; M <= 23; M++)
                                                        {
                                                            for (var N = M + 1; N <= 24; N++)
                                                            {
                                                                for (var O = N + 1; O <= 25; O++)
                                                                {
                                                                    List<int> ls = new()
                                                                    {
                                                                        A,
                                                                        B,
                                                                        C,
                                                                        D,
                                                                        E,
                                                                        F,
                                                                        G,
                                                                        H,
                                                                        I,
                                                                        J,
                                                                        K,
                                                                        L,
                                                                        M,
                                                                        N,
                                                                        O
                                                                    };

                                                                    Lance u = new Lance(ARS.Count, ls);

                                                                    ARS.Add(u);


                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 15 a 15
    public static Lances Combinar25Filtrado()
    {
        Lances ARS = new();

        for (var A = 1; A <= 11; A++)
        {
            for (var B = A + 1; B <= 12; B++)
            {
                for (var C = B + 1; C <= 13; C++)
                {
                    for (var D = C + 1; D <= 14; D++)
                    {
                        for (var E = D + 1; E <= 15; E++)
                        {
                            for (var F = E + 1; F <= 16; F++)
                            {
                                for (var G = F + 1; G <= 17; G++)
                                {
                                    for (var H = G + 1; H <= 18; H++)
                                    {
                                        for (var I = H + 1; I <= 19; I++)
                                        {
                                            for (var J = I + 1; J <= 20; J++)
                                            {
                                                for (var K = J + 1; K <= 21; K++)
                                                {
                                                    for (var L = K + 1; L <= 22; L++)
                                                    {
                                                        for (var M = L + 1; M <= 23; M++)
                                                        {
                                                            for (var N = M + 1; N <= 24; N++)
                                                            {
                                                                for (var O = N + 1; O <= 25; O++)
                                                                {
                                                                    List<int> ls = new()
                                                                    {
                                                                        A,
                                                                        B,
                                                                        C,
                                                                        D,
                                                                        E,
                                                                        F,
                                                                        G,
                                                                        H,
                                                                        I,
                                                                        J,
                                                                        K,
                                                                        L,
                                                                        M,
                                                                        N,
                                                                        O
                                                                    };

                                                                    Lance u = new Lance(ARS.Count, ls);

                                                                    if (RetornaFiltrado(A, B, C, D, E, F, G, H, I, J, K, L, M, N, O)) { ARS.Add(u); }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    public static List<String> Combinar25Nome()
    {
        List<string> ARS = new List<string>();

        for (var A = 1; A <= 11; A++)
        {
            for (var B = A + 1; B <= 12; B++)
            {
                for (var C = B + 1; C <= 13; C++)
                {
                    for (var D = C + 1; D <= 14; D++)
                    {
                        for (var E = D + 1; E <= 15; E++)
                        {
                            for (var F = E + 1; F <= 16; F++)
                            {
                                for (var G = F + 1; G <= 17; G++)
                                {
                                    for (var H = G + 1; H <= 18; H++)
                                    {
                                        for (var I = H + 1; I <= 19; I++)
                                        {
                                            for (var J = I + 1; J <= 20; J++)
                                            {
                                                for (var K = J + 1; K <= 21; K++)
                                                {
                                                    for (var L = K + 1; L <= 22; L++)
                                                    {
                                                        for (var M = L + 1; M <= 23; M++)
                                                        {
                                                            for (var N = M + 1; N <= 24; N++)
                                                            {
                                                                for (var O = N + 1; O <= 25; O++)
                                                                {
                                                                    List<int> ls = new()
                                                                    {
                                                                        A,
                                                                        B,
                                                                        C,
                                                                        D,
                                                                        E,
                                                                        F,
                                                                        G,
                                                                        H,
                                                                        I,
                                                                        J,
                                                                        K,
                                                                        L,
                                                                        M,
                                                                        N,
                                                                        O
                                                                    };

                                                                    Lance u = new Lance(ARS.Count, ls);
                                                                    ARS.Add(u.Nome);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 9 a 9
    public static Lances Combinar25a9()
    {
        Lances ARS = new();

        for (var B = 1; B <= 17; B++)
        {
            for (var C = B + 1; C <= 18; C++)
            {
                for (var D = C + 1; D <= 19; D++)
                {
                    for (var E = D + 1; E <= 20; E++)
                    {
                        for (var F = E + 1; F <= 21; F++)
                        {
                            for (var G = F + 1; G <= 22; G++)
                            {
                                for (var H = G + 1; H <= 23; H++)
                                {
                                    for (var I = H + 1; I <= 24; I++)
                                    {
                                        for (var J = I + 1; J <= 25; J++)
                                        {

                                            {
                                                List<int> ls = new()
                                                    {

                                                        B,
                                                        C,
                                                        D,
                                                        E,
                                                        F,
                                                        G,
                                                        H,
                                                        I,
                                                        J
                                                    };


                                                Lance u = new Lance(ARS.Count, ls);

                                                ARS.Add(u);


                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 10 a 10
    public static Lances Combinar25a10()
    {
        Lances ARS = new();

        for (var A = 1; A <= 16; A++)
        {
            for (var B = A + 1; B <= 17; B++)
            {
                for (var C = B + 1; C <= 18; C++)
                {
                    for (var D = C + 1; D <= 19; D++)
                    {
                        for (var E = D + 1; E <= 20; E++)
                        {
                            for (var F = E + 1; F <= 21; F++)
                            {
                                for (var G = F + 1; G <= 22; G++)
                                {
                                    for (var H = G + 1; H <= 23; H++)
                                    {
                                        for (var I = H + 1; I <= 24; I++)
                                        {
                                            for (var J = I + 1; J <= 25; J++)
                                            {

                                                {
                                                    List<int> ls = new()
                                                    {
                                                        A,
                                                        B,
                                                        C,
                                                        D,
                                                        E,
                                                        F,
                                                        G,
                                                        H,
                                                        I,
                                                        J
                                                    };


                                                    Lance u = new Lance(ARS.Count, ls);

                                                    ARS.Add(u);

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 10 a 10
    public static Lances Combinar25a10BaseZero()
    {
        Lances ARS = new();

        for (var A = 0; A <= 15; A++)
        {
            for (var B = A + 1; B <= 16; B++)
            {
                for (var C = B + 1; C <= 17; C++)
                {
                    for (var D = C + 1; D <= 18; D++)
                    {
                        for (var E = D + 1; E <= 19; E++)
                        {
                            for (var F = E + 1; F <= 20; F++)
                            {
                                for (var G = F + 1; G <= 212; G++)
                                {
                                    for (var H = G + 1; H <= 22; H++)
                                    {
                                        for (var I = H + 1; I <= 23; I++)
                                        {
                                            for (var J = I + 1; J <= 24; J++)
                                            {

                                                {
                                                    List<int> ls = new()
                                                    {
                                                        A,
                                                        B,
                                                        C,
                                                        D,
                                                        E,
                                                        F,
                                                        G,
                                                        H,
                                                        I,
                                                        J
                                                    };


                                                    Lance u = new Lance(ARS.Count, ls);

                                                    ARS.Add(u);

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 16 a 16

    public static Lances Combinar25a16()
    {
        Lances ARS = new();

        for (var A = 1; A <= 10; A++)
        {
            for (var B = A + 1; B <= 11; B++)
            {
                for (var C = B + 1; C <= 12; C++)
                {
                    for (var D = C + 1; D <= 13; D++)
                    {
                        for (var E = D + 1; E <= 14; E++)
                        {
                            for (var F = E + 1; F <= 15; F++)
                            {
                                for (var G = F + 1; G <= 16; G++)
                                {
                                    for (var H = G + 1; H <= 17; H++)
                                    {
                                        for (var I = H + 1; I <= 18; I++)
                                        {
                                            for (var J = I + 1; J <= 19; J++)
                                            {
                                                for (var K = J + 1; K <= 20; K++)
                                                {
                                                    for (var L = K + 1; L <= 21; L++)
                                                    {
                                                        for (var M = L + 1; M <= 22; M++)
                                                        {
                                                            for (var N = M + 1; N <= 23; N++)
                                                            {
                                                                for (var O = N + 1; O <= 24; O++)
                                                                {
                                                                    for (var P = O + 1; P <= 25; P++)
                                                                    {
                                                                        List<int> ls = new()
                                                                    {
                                                                        A,
                                                                        B,
                                                                        C,
                                                                        D,
                                                                        E,
                                                                        F,
                                                                        G,
                                                                        H,
                                                                        I,
                                                                        J,
                                                                        K,
                                                                        L,
                                                                        M,
                                                                        N,
                                                                        O,
                                                                        P

                                                                    };

                                                                        Lance u = new Lance(ARS.Count, ls);

                                                                        ARS.Add(u);

                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }

    // Combina 25 - 17 a 17

    public static Lances Combinar25a17()
    {
        Lances ARS = new();


        for (var B = 1; B <= 9; B++)
        {
            for (var C = B + 1; C <= 10; C++)
            {
                for (var D = C + 1; D <= 11; D++)
                {
                    for (var E = D + 1; E <= 12; E++)
                    {
                        for (var F = E + 1; F <= 13; F++)
                        {
                            for (var G = F + 1; G <= 14; G++)
                            {
                                for (var H = G + 1; H <= 15; H++)
                                {
                                    for (var I = H + 1; I <= 16; I++)
                                    {
                                        for (var J = I + 1; J <= 17; J++)
                                        {
                                            for (var K = J + 1; K <= 18; K++)
                                            {
                                                for (var L = K + 1; L <= 19; L++)
                                                {
                                                    for (var M = L + 1; M <= 20; M++)
                                                    {
                                                        for (var N = M + 1; N <= 21; N++)
                                                        {
                                                            for (var O = N + 1; O <= 22; O++)
                                                            {
                                                                for (var P = O + 1; P <= 23; P++)
                                                                {
                                                                    for (var Q = P + 1; Q <= 24; Q++)
                                                                    {
                                                                        for (var R = Q + 1; R <= 25; R++)
                                                                        {

                                                                            List<int> ls = new()
                                                                    {
                                                                        B,
                                                                        C,
                                                                        D,
                                                                        E,
                                                                        F,
                                                                        G,
                                                                        H,
                                                                        I,
                                                                        J,
                                                                        K,
                                                                        L,
                                                                        M,
                                                                        N,
                                                                        O,
                                                                        P,
                                                                        Q,
                                                                        R
                                                                    };

                                                                            Lance u = new Lance(ARS.Count, ls);

                                                                            ARS.Add(u);

                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }
    // Combina 25 - 18 a 18

    public static Lances Combinar25a18()
    {
        Lances ARS = new();

        for (var A = 1; A <= 8; A++)
        {
            for (var B = A + 1; B <= 9; B++)
            {
                for (var C = B + 1; C <= 10; C++)
                {
                    for (var D = C + 1; D <= 11; D++)
                    {
                        for (var E = D + 1; E <= 12; E++)
                        {
                            for (var F = E + 1; F <= 13; F++)
                            {
                                for (var G = F + 1; G <= 14; G++)
                                {
                                    for (var H = G + 1; H <= 15; H++)
                                    {
                                        for (var I = H + 1; I <= 16; I++)
                                        {
                                            for (var J = I + 1; J <= 17; J++)
                                            {
                                                for (var K = J + 1; K <= 18; K++)
                                                {
                                                    for (var L = K + 1; L <= 19; L++)
                                                    {
                                                        for (var M = L + 1; M <= 20; M++)
                                                        {
                                                            for (var N = M + 1; N <= 21; N++)
                                                            {
                                                                for (var O = N + 1; O <= 22; O++)
                                                                {
                                                                    for (var P = O + 1; P <= 23; P++)
                                                                    {
                                                                        for (var Q = P + 1; Q <= 24; Q++)
                                                                        {
                                                                            for (var R = Q + 1; R <= 25; R++)
                                                                            {

                                                                                List<int> ls = new()
                                                                    {
                                                                        A,
                                                                        B,
                                                                        C,
                                                                        D,
                                                                        E,
                                                                        F,
                                                                        G,
                                                                        H,
                                                                        I,
                                                                        J,
                                                                        K,
                                                                        L,
                                                                        M,
                                                                        N,
                                                                        O,
                                                                        P,
                                                                        Q,
                                                                        R
                                                                    };

                                                                                Lance u = new Lance(ARS.Count, ls);

                                                                                ARS.Add(u);

                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
    }


    // Combina 25 - 20 a 20
    public static Lances Combinar25a20()
    {
        Lances ARS = new();

        for (var A = 1; A <= 6; A++)
        {
            for (var B = A + 1; B <= 7; B++)
            {
                for (var C = B + 1; C <= 8; C++)
                {
                    for (var D = C + 1; D <= 9; D++)
                    {
                        for (var E = D + 1; E <= 10; E++)
                        {
                            for (var F = E + 1; F <= 11; F++)
                            {
                                for (var G = F + 1; G <= 12; G++)
                                {
                                    for (var H = G + 1; H <= 13; H++)
                                    {
                                        for (var I = H + 1; I <= 14; I++)
                                        {
                                            for (var J = I + 1; J <= 15; J++)
                                            {
                                                for (var K = J + 1; K <= 16; K++)
                                                {
                                                    for (var L = K + 1; L <= 17; L++)
                                                    {
                                                        for (var M = L + 1; M <= 18; M++)
                                                        {
                                                            for (var N = M + 1; N <= 19; N++)
                                                            {
                                                                for (var O = N + 1; O <= 20; O++)
                                                                {
                                                                    for (var P = O + 1; P <= 21; P++)
                                                                    {
                                                                        for (var Q = P + 1; Q <= 22; Q++)
                                                                        {
                                                                            for (var R = Q + 1; R <= 23; R++)
                                                                            {
                                                                                for (var S = R + 1; S <= 24; S++)
                                                                                {
                                                                                    for (var T = S + 1; T <= 25; T++)
                                                                                    {


                                                                                        List<int> ls = new()
                                                                    {
                                                                        A,
                                                                        B,
                                                                        C,
                                                                        D,
                                                                        E,
                                                                        F,
                                                                        G,
                                                                        H,
                                                                        I,
                                                                        J,
                                                                        K,
                                                                        L,
                                                                        M,
                                                                        N,
                                                                        O,
                                                                        P,
                                                                        Q,
                                                                        R,
                                                                        S,
                                                                        T
                                                                    };

                                                                                        Lance u = new Lance(ARS.Count, ls);

                                                                                        ARS.Add(u);
                                                                                    }
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return ARS;
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
