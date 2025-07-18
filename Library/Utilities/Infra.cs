// D:\PROJETOS\GraphFacil\Library\Utilities\Infra.cs
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;
using System;
using LotoLibrary.Models;
using LotoLibrary.Services;
using Newtonsoft.Json;

﻿using LotoLibrary.Constantes;
using LotoLibrary.Models.Core;

namespace LotoLibrary.Utilities
{

    public partial class Infra
    {

        public static Lances arGeral = new();
        public static Lances arLoto = new();
        public static Lances arGeralFiltrado = new();


        #region "Abrir arquivos"
        public static Lances AbrirArquivo(string nome)
        {

            string path = Directory.Exists(Constante.PT) ? Constante.PT : 
                         Directory.Exists(Constante.PT1) ? Constante.PT1 : Constante.PT2;

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

                    string y = Regex.Replace(o, @"\r|\n", string.Empty);
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


        #endregion

        #region "Área JSON"


        public static Lances AbrirArquivoJson(string nome)
        {

            string path = Directory.Exists(Constante.PT) ? Constante.PT : 
                         Directory.Exists(Constante.PT1) ? Constante.PT1 : Constante.PT2;

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
            string path = Directory.Exists(Constante.PT) ? Constante.PT : 
                         Directory.Exists(Constante.PT1) ? Constante.PT1 : Constante.PT2;

            string caminho = Path.Combine(path, "Saida", alvo.ToString());

            if (!Directory.Exists(caminho)) { Directory.CreateDirectory(caminho); }

            return Path.Combine(caminho, nome + "-" + alvo.ToString() + ".json");
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


        public static void SalvaSaidaW(List<double> matrizSaida, string nome)
        {
            StringBuilder salvaKey = new StringBuilder();

            foreach (double o in matrizSaida)
            {
                // Formata o número com duas casas decimais
                salvaKey.AppendLine(o.ToString("F2"));
            }

            File.WriteAllText(nome, salvaKey.ToString());
        }


        #endregion

        #region "Métodos Especiais"

        public static void CarregarConcursos() => AtualizarConcursosDiretoDoSite(Constante.ArqLotoJson);

        private static void AtualizarConcursosDiretoDoSite(string nomeArq)
        {
            string path = Directory.Exists(Constante.PT) ? Constante.PT : 
                         Directory.Exists(Constante.PT1) ? Constante.PT1 : Constante.PT2;
            string fullPath = Path.Combine(path, nomeArq);

            System.Diagnostics.Debug.WriteLine($"Tentando carregar de: {fullPath}");
            System.Diagnostics.Debug.WriteLine($"Arquivo existe: {File.Exists(fullPath)}");

            List<Lotofacil> concursos;

            if (File.Exists(fullPath))
            {
                // O arquivo existe, vamos carregar e atualizar
                concursos = CarregarConcursosDoArquivo(fullPath);
                System.Diagnostics.Debug.WriteLine($"Concursos carregados: {concursos?.Count ?? 0}");
                
                var updater = new LotofacilUpdater();
                concursos = updater.UpdateConcursos(concursos);

                PreencheSorteios(concursos);
                System.Diagnostics.Debug.WriteLine($"arLoto preenchido com: {arLoto?.Count ?? 0} itens");
            }
            else
            {
                // O arquivo não existe, vamos extrair todos os concursos
                System.Diagnostics.Debug.WriteLine("Arquivo não encontrado, tentando scraper...");
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

            arLoto.Clear();

            foreach (var o in concursos)
            {
                List<int> l = new List<int>();
                foreach (string m in o.listaDezenas) { l.Add(Convert.ToInt32(m)); }

                arLoto.Add(new Lance(arLoto.Count, l));
            }
        }

        public static int Contapontos(Lance Lsa, Lance Lsb)
        {
            return Lsa.Lista.Intersect(Lsb.Lista).ToList().Count;
        }


        #endregion

        #region "COMBINAÇÕES PRINCIPAIS"

        public static void CombinarGeral()
        {

            arGeral.AddRange(Combinar25());
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


        #endregion

        #region "EXTRAS"

        public static Lance DevolveComplementar(Lance Lsa, Lance Lsb)
        {
            List<int> l = Lsa.Lista.Except(Lsb.Lista).ToList();

            Lance v = new Lance(0, l);

            return v;

        }

        public static Lance DevolveOposto(Lance U)
        {
            if (U.Lista.Count > 0)
            {
                List<int> l = Constante.ListaG.Except(U.Lista).ToList();

                Lance v = new Lance(0, l);

                return v;
            }

            return U;

        }
        public static string NomeSaida(string nome, int alvo)
        {

            string path = Directory.Exists(Constante.PT) ? Constante.PT : 
                         Directory.Exists(Constante.PT1) ? Constante.PT1 : Constante.PT2;

            string caminho = Path.Combine(path, "Saida", alvo.ToString());

            if (!Directory.Exists(caminho)) { Directory.CreateDirectory(caminho); }

            return Path.Combine(caminho, nome + "-" + alvo.ToString() + ".txt");
        }

        public static Uvas DevolveMaisFrequentesUvas(Lances L, int T)
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

            return uvas;
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

        public static Lances DevolveBaseGeralFiltrada(Lances arBase, Lance oAlvo, int v)
        {
            var resultadoParalelo = arBase.AsParallel()
                                          .Where(o => Contapontos(o, oAlvo) == v)
                                          .ToList();

            return new Lances(resultadoParalelo);
        }

        internal static Lances DevolveBaseCombinada(Lances ars9, Lances ars6)
        {
            Lances saida = new();

            foreach (Lance o in ars9)
            {
                foreach (Lance p in ars6)
                {
                    List<int> list = new List<int>();
                    list.AddRange(o.Lista);
                    list.AddRange(p.Lista);
                    list.Sort();

                    Lance z = new Lance(saida.Count, list);

                    z.M = o.Id;
                    z.N = p.Id;

                    saida.Add(z);
                }
            }


            return saida;
        }

        internal static Lances DevolveBaseAleatoria(Lances arBase, int ax)
        {
            Lances saida = new();

            Random random = new();

            while (saida.Count < ax)
            {
                Lance o = arBase[random.Next(arBase.Count)];
                if (!saida.Contains(o))
                {
                    saida.Add(o);
                }
            }


            return saida;
        }

        internal static Lance CriaLanceAPartirDoPar(Lance lance, Lance oMaisFrequente)
        {
            List<int> ints = new();

            ints.AddRange(lance.Lista);
            ints.AddRange(oMaisFrequente.Lista);

            ints.Sort();

            return new Lance(0, ints);

        }


        #endregion

        #region Carregamento Seguro de Concursos
        /// <summary>
        /// Carrega concursos com tratamento de arquivo travado
        /// </summary>
        public static Lances CarregarConcursosSeguro()
        {
            const int maxTentativas = 5;
            const int delayMs = 1000;

            for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
            {
                try
                {
                    CarregarConcursos();
                    return arLoto;
                }
                catch (IOException ex) when (ex.Message.Contains("being used by another process"))
                {
                    if (tentativa == maxTentativas)
                    {
                        throw new InvalidOperationException(
                            $"Não foi possível acessar o arquivo após {maxTentativas} tentativas. " +
                            "Feche outros programas que possam estar usando o arquivo e tente novamente.", ex);
                    }

                    System.Diagnostics.Debug.WriteLine($"Tentativa {tentativa}: Arquivo em uso, aguardando {delayMs}ms...");
                    System.Threading.Thread.Sleep(delayMs);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Erro ao carregar concursos: {ex.Message}", ex);
                }
            }

            return new Lances();
        }

        /// <summary>
        /// Força liberação de recursos e recarrega concursos
        /// </summary>
        public static void ForcarLiberacaoERecarregar()
        {
            try
            {
                // Força garbage collection para liberar qualquer FileStream
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                // Pequena pausa
                System.Threading.Thread.Sleep(500);

                // Recarrega
                arLoto = CarregarConcursosSeguro();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao forçar recarregamento: {ex.Message}", ex);
            }
        }
        #endregion

        #region Métodos JSON Migrados do PalpiteService1
        /// <summary>
        /// Carrega percentuais SS de arquivo JSON com tratamento seguro
        /// </summary>
        public static Dictionary<string, float> CarregarPercentuaisSS()
        {
            return CarregarPercentuaisJSON("PercentuaisSS.json");
        }

        /// <summary>
        /// Carrega percentuais NS de arquivo JSON com tratamento seguro
        /// </summary>
        public static Dictionary<string, float> CarregarPercentuaisNS()
        {
            return CarregarPercentuaisJSON("PercentuaisNS.json");
        }

        /// <summary>
        /// Carrega percentuais de arquivo JSON genérico
        /// </summary>
        public static Dictionary<string, float> CarregarPercentuaisJSON(string nomeArquivo)
        {
            try
            {
                if (!File.Exists(nomeArquivo))
                {
                    System.Diagnostics.Debug.WriteLine($"Arquivo {nomeArquivo} não encontrado. Retornando dicionário vazio.");
                    return new Dictionary<string, float>();
                }

                string conteudo = string.Empty;

                // Leitura segura com retry
                const int maxTentativas = 3;
                for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
                {
                    try
                    {
                        using var stream = new FileStream(nomeArquivo, FileMode.Open, FileAccess.Read, FileShare.Read);
                        using var reader = new StreamReader(stream);
                        conteudo = reader.ReadToEnd();
                        break;
                    }
                    catch (IOException) when (tentativa < maxTentativas)
                    {
                        System.Threading.Thread.Sleep(200);
                        continue;
                    }
                }

                if (string.IsNullOrWhiteSpace(conteudo))
                {
                    return new Dictionary<string, float>();
                }

                // Fix: Use JsonConvert.DeserializeObject instead of Newtonsoft.Json.JsonSerializer.Deserialize
                Dictionary<string, float> dados = JsonConvert.DeserializeObject<Dictionary<string, float>>(conteudo);
                System.Diagnostics.Debug.WriteLine($"Carregados {dados?.Count ?? 0} percentuais de {nomeArquivo}");

                return dados ?? new Dictionary<string, float>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar {nomeArquivo}: {ex.Message}");
                return new Dictionary<string, float>();
            }
        }

        /// <summary>
        /// Salva percentuais em arquivo JSON
        /// </summary>
        public static void SalvarPercentuaisJSON(Dictionary<string, float> percentuais, string nomeArquivo)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

                var json = System.Text.Json.JsonSerializer.Serialize(percentuais, options);

                // Escrita segura
                const int maxTentativas = 3;
                for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
                {
                    try
                    {
                        File.WriteAllText(nomeArquivo, json);
                        System.Diagnostics.Debug.WriteLine($"Salvos {percentuais.Count} percentuais em {nomeArquivo}");
                        break;
                    }
                    catch (IOException) when (tentativa < maxTentativas)
                    {
                        System.Threading.Thread.Sleep(200);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao salvar {nomeArquivo}: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Gera IDs de subgrupos para um palpite
        /// </summary>
        public static List<string> GerarIdsSubgrupos(Lance palpite)
        {
            var ids = new List<string>();

            try
            {
                // Grupos SS (combinações de palpite.M)
                if (palpite.M >= 0 && palpite.M < GruposSS.Count)
                {
                    var grupoSS = GruposSS[palpite.M];
                    for (int tamanho = 3; tamanho <= Math.Min(6, grupoSS.Count); tamanho++)
                    {
                        var combinacoes = GerarCombinacoes(grupoSS, tamanho);
                        foreach (var comb in combinacoes)
                        {
                            var id = "SS_" + string.Join("_", comb.OrderBy(x => x));
                            ids.Add(id);
                        }
                    }
                }

                // Grupos NS (combinações de palpite.N)
                if (palpite.N >= 0 && palpite.N < GruposNS.Count)
                {
                    var grupoNS = GruposNS[palpite.N];
                    for (int tamanho = 3; tamanho <= Math.Min(6, grupoNS.Count); tamanho++)
                    {
                        var combinacoes = GerarCombinacoes(grupoNS, tamanho);
                        foreach (var comb in combinacoes)
                        {
                            var id = "NS_" + string.Join("_", comb.OrderBy(x => x));
                            ids.Add(id);
                        }
                    }
                }

                return ids;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao gerar IDs de subgrupos: {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Gera combinações de uma lista
        /// </summary>
        public static List<List<int>> GerarCombinacoes(List<int> lista, int tamanho)
        {
            var result = new List<List<int>>();

            if (tamanho == 1)
            {
                foreach (var item in lista)
                {
                    result.Add(new List<int> { item });
                }
                return result;
            }

            for (int i = 0; i <= lista.Count - tamanho; i++)
            {
                var subCombinacoes = GerarCombinacoes(lista.Skip(i + 1).ToList(), tamanho - 1);
                foreach (var subComb in subCombinacoes)
                {
                    var novaComb = new List<int> { lista[i] };
                    novaComb.AddRange(subComb);
                    result.Add(novaComb);
                }
            }

            return result;
        }


        #endregion

        #region Propriedades Estáticas (se não existirem)
        /// <summary>
        /// Grupos SS (referência estática)
        /// </summary>
        public static List<List<int>> GruposSS { get; set; } = new List<List<int>>();

        /// <summary>
        /// Grupos NS (referência estática)
        /// </summary>
        public static List<List<int>> GruposNS { get; set; } = new List<List<int>>();
        #endregion

        #region Diagnostics
        /// <summary>
        /// Verifica status dos arquivos JSON
        /// </summary>
        public static string VerificarStatusJSON()
        {
            var status = "=== STATUS DOS ARQUIVOS JSON ===\n\n";

            var arquivos = new[] { "PercentuaisSS.json", "PercentuaisNS.json", "Lotofacil.json" };

            foreach (var arquivo in arquivos)
            {
                try
                {
                    if (File.Exists(arquivo))
                    {
                        var info = new FileInfo(arquivo);
                        status += $"✅ {arquivo}:\n";
                        status += $"   Tamanho: {info.Length:N0} bytes\n";
                        status += $"   Modificado: {info.LastWriteTime}\n";
                        status += $"   Acesso: {(info.IsReadOnly ? "Somente leitura" : "Leitura/Escrita")}\n\n";
                    }
                    else
                    {
                        status += $"❌ {arquivo}: Não encontrado\n\n";
                    }
                }
                catch (Exception ex)
                {
                    status += $"⚠️ {arquivo}: Erro - {ex.Message}\n\n";
                }
            }

            return status;
        }

        #endregion
    }
}
