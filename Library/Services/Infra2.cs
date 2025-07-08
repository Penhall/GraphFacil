// D:\PROJETOS\GraphFacil\Library\Services\Infra.cs - Métodos adicionais para JSON
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace LotoLibrary.Services
{
    public static partial class Infra
    {
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

                var dados = JsonSerializer.Deserialize<Dictionary<string, float>>(conteudo);
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

                var json = JsonSerializer.Serialize(percentuais, options);

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

        /// <summary>
        /// Calcula pontuação de um palpite usando percentuais carregados
        /// </summary>
        public static float CalcularPontuacaoPalpite(Lance palpite, Dictionary<string, float> percentuaisSS, Dictionary<string, float> percentuaisNS)
        {
            try
            {
                var idsSubgrupos = GerarIdsSubgrupos(palpite);
                var pontuacoes = new List<float>();

                foreach (var id in idsSubgrupos)
                {
                    if (id.StartsWith("SS_") && percentuaisSS.TryGetValue(id, out float pontuacaoSS))
                    {
                        pontuacoes.Add(pontuacaoSS);
                    }
                    else if (id.StartsWith("NS_") && percentuaisNS.TryGetValue(id, out float pontuacaoNS))
                    {
                        pontuacoes.Add(pontuacaoNS);
                    }
                }

                return pontuacoes.Any() ? pontuacoes.Average() : 0f;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao calcular pontuação: {ex.Message}");
                return 0f;
            }
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