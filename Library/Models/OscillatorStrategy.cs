using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services
{
    public static class OscillatorStrategy
    {
        private static readonly Random _random = new Random();

        /// <summary>
        /// Aplica estratégia de tendência de curto prazo
        /// </summary>
        public static void AplicarTendenciaCurtoPrazo(List<DezenaOscilante> dezenas, List<Lance> ultimosSorteios)
        {
            if (ultimosSorteios == null || !ultimosSorteios.Any()) return;

            var recentes = ultimosSorteios.TakeLast(5).SelectMany(l => l.Lista).ToList();
            var contadorRecentes = recentes.GroupBy(n => n).ToDictionary(g => g.Key, g => g.Count());

            foreach (var dezena in dezenas)
            {
                if (contadorRecentes.TryGetValue(dezena.Numero, out var count))
                {
                    // Números que saíram recentemente têm maior força
                    dezena.ForcaSincronizacao = Math.Min(1.0, dezena.ForcaSincronizacao + (count * 0.1));
                    dezena.UltimoAtraso = 0;
                }
                else
                {
                    // Aumenta o atraso para números que não saíram
                    dezena.UltimoAtraso++;
                }
            }
        }

        /// <summary>
        /// Aplica estratégia baseada em padrões de grupos
        /// </summary>
        public static void AplicarPadroesGrupos(List<DezenaOscilante> dezenas, List<int[]> gruposFrequentes)
        {
            if (gruposFrequentes == null) return;

            foreach (var grupo in gruposFrequentes)
            {
                var membros = dezenas.Where(d => grupo.Contains(d.Numero)).ToList();
                if (membros.Count < 2) continue;

                double mediaForca = membros.Average(d => d.ForcaSincronizacao);
                double mediaFase = membros.Average(d => d.Fase);

                foreach (var dezena in membros)
                {
                    // Sincroniza a força entre membros do grupo
                    dezena.ForcaSincronizacao = (dezena.ForcaSincronizacao + mediaForca) / 2;

                    // Puxa as fases em direção à média do grupo
                    double diferencaFase = mediaFase - dezena.Fase;
                    if (Math.Abs(diferencaFase) > 180) // Considera o círculo
                        diferencaFase = diferencaFase > 0 ? diferencaFase - 360 : diferencaFase + 360;

                    dezena.Fase += diferencaFase * 0.1;
                    dezena.Fase = (dezena.Fase % 360 + 360) % 360;
                }
            }
        }

        /// <summary>
        /// Aplica estratégia baseada em ciclos históricos
        /// </summary>
        public static void AplicarCiclos(List<DezenaOscilante> dezenas, Dictionary<int, int> ciclosMedios)
        {
            if (ciclosMedios == null) return;

            foreach (var dezena in dezenas)
            {
                if (ciclosMedios.TryGetValue(dezena.Numero, out var ciclo) && ciclo > 0)
                {
                    // Ajusta a frequência baseada no ciclo médio
                    double fatorCiclo = Math.Sin((dezena.UltimoAtraso / (double)ciclo) * Math.PI);
                    dezena.Frequencia *= 1 + (fatorCiclo * 0.2);

                    // Números próximos ao final do ciclo ganham mais força
                    if (dezena.UltimoAtraso >= ciclo * 0.8)
                    {
                        dezena.ForcaSincronizacao = Math.Min(1.0, dezena.ForcaSincronizacao + 0.15);
                    }
                }
            }
        }

        /// <summary>
        /// Aplica estratégia de números quentes e frios
        /// </summary>
        public static void AplicarQuentesFrios(List<DezenaOscilante> dezenas, List<Lance> historico)
        {
            if (historico == null || !historico.Any()) return;

            // Calcula frequência de cada número nos últimos sorteios
            var frequencias = historico
                .SelectMany(l => l.Lista)
                .GroupBy(n => n)
                .ToDictionary(g => g.Key, g => g.Count());

            double mediaFrequencia = frequencias.Values.Average();

            foreach (var dezena in dezenas)
            {
                if (frequencias.TryGetValue(dezena.Numero, out var freq))
                {
                    dezena.FrequenciaHistorica = freq;

                    if (freq > mediaFrequencia * 1.2) // Número quente
                    {
                        dezena.Frequencia = Math.Min(3.0, dezena.Frequencia * 1.1);
                    }
                    else if (freq < mediaFrequencia * 0.8) // Número frio
                    {
                        dezena.ForcaSincronizacao = Math.Min(1.0, dezena.ForcaSincronizacao + 0.1);
                    }
                }
            }
        }

        /// <summary>
        /// Gera palpite inteligente baseado em múltiplos critérios (VERSÃO CORRIGIDA)
        /// </summary>
        public static List<int> GerarPalpiteValidacao(List<DezenaOscilante> dezenas, List<Lance> dadosValidacao)
        {
            if (dezenas == null || !dezenas.Any())
                return Enumerable.Range(1, 15).ToList(); // Fallback

            // Atualiza todas as probabilidades antes de gerar o palpite
            foreach (var dezena in dezenas)
            {
                AtualizarProbabilidadeCompleta(dezena, dadosValidacao);
            }

            // Estratégia híbrida para seleção
            var candidatos = dezenas.Select(d => new
            {
                Numero = d.Numero,
                Score = CalcularScoreAvancado(d),
                Probabilidade = d.Probabilidade,
                Sincronizada = d.EstaSincronizada
            }).ToList();

            // Ordena por score e adiciona elemento de aleatoriedade
            var selecionados = new List<int>();
            var candidatosOrdenados = candidatos.OrderByDescending(c => c.Score).ToList();

            // Seleciona os 10 melhores por score
            selecionados.AddRange(candidatosOrdenados.Take(10).Select(c => c.Numero));

            // Seleciona 3 dos próximos 10 (elemento de aleatoriedade)
            var proximosCandidatos = candidatosOrdenados.Skip(10).Take(10).ToList();
            var selecionadosAleatorios = proximosCandidatos
                .OrderBy(x => _random.Next())
                .Take(3)
                .Select(c => c.Numero);
            selecionados.AddRange(selecionadosAleatorios);

            // Seleciona 2 números sincronizados que ainda não foram selecionados
            var sincronizados = candidatos
                .Where(c => c.Sincronizada && !selecionados.Contains(c.Numero))
                .OrderByDescending(c => c.Probabilidade)
                .Take(2)
                .Select(c => c.Numero);
            selecionados.AddRange(sincronizados);

            // Se ainda faltam números, completa com os melhores restantes
            while (selecionados.Count < 15)
            {
                var proximo = candidatos
                    .Where(c => !selecionados.Contains(c.Numero))
                    .OrderByDescending(c => c.Score)
                    .FirstOrDefault();

                if (proximo != null)
                    selecionados.Add(proximo.Numero);
                else
                    break;
            }

            return selecionados.Take(15).OrderBy(x => x).ToList();
        }

        /// <summary>
        /// Atualiza probabilidade completa considerando contexto histórico
        /// </summary>
        private static void AtualizarProbabilidadeCompleta(DezenaOscilante dezena, List<Lance> dadosValidacao)
        {
            if (dadosValidacao == null || !dadosValidacao.Any()) return;

            // Fator baseado na fase atual
            double fatorFase = (Math.Sin(dezena.Fase * Math.PI / 180) + 1) / 2;

            // Fator baseado no atraso
            double fatorAtraso = Math.Min(1.0, dezena.UltimoAtraso / 15.0);

            // Fator baseado na frequência histórica
            double mediaFrequencia = dadosValidacao.SelectMany(l => l.Lista).Count() / 25.0;
            double fatorFrequencia = Math.Min(1.0, dezena.FrequenciaHistorica / mediaFrequencia);

            // Fator baseado na sincronização
            double fatorSync = dezena.ForcaSincronizacao;

            // Fator de aleatoriedade para evitar sempre os mesmos números
            double fatorAleatorio = _random.NextDouble() * 0.2; // 20% de aleatoriedade

            // Combina todos os fatores
            dezena.Probabilidade = (fatorFase * 0.25) +
                                  (fatorAtraso * 0.25) +
                                  (fatorFrequencia * 0.2) +
                                  (fatorSync * 0.2) +
                                  (fatorAleatorio * 0.1);

            dezena.Probabilidade = Math.Max(0.1, Math.Min(0.9, dezena.Probabilidade));
        }

        /// <summary>
        /// Calcula score avançado para ranking das dezenas
        /// </summary>
        private static double CalcularScoreAvancado(DezenaOscilante dezena)
        {
            double scoreBase = dezena.Probabilidade * 100;

            // Bônus para números sincronizados
            double bonusSync = dezena.EstaSincronizada ? 15 : 0;

            // Bônus/penalidade baseado no atraso
            double bonusAtraso = dezena.UltimoAtraso > 10 ? dezena.UltimoAtraso * 1.5 : 0;
            double penalidadeAtraso = dezena.UltimoAtraso > 25 ? dezena.UltimoAtraso * 0.5 : 0;

            // Bônus para força de sincronização alta
            double bonusForca = dezena.ForcaSincronizacao > 0.7 ? dezena.ForcaSincronizacao * 20 : 0;

            // Fator de aleatoriedade para quebrar empates
            double fatorAleatorio = _random.NextDouble() * 5;

            return scoreBase + bonusSync + bonusAtraso - penalidadeAtraso + bonusForca + fatorAleatorio;
        }

        /// <summary>
        /// Analisa padrões de grupos frequentes no histórico
        /// </summary>
        public static List<int[]> IdentificarGruposFrequentes(List<Lance> historico, int tamanhoGrupo = 3)
        {
            if (historico == null || !historico.Any()) return new List<int[]>();

            var grupos = new Dictionary<string, int>();

            foreach (var lance in historico)
            {
                var combinacoes = GerarCombinacoes(lance.Lista, tamanhoGrupo);
                foreach (var combo in combinacoes)
                {
                    var chave = string.Join(",", combo.OrderBy(x => x));
                    grupos[chave] = grupos.TryGetValue(chave, out var count) ? count + 1 : 1;
                }
            }

            return grupos
                .Where(g => g.Value >= historico.Count * 0.1) // Aparece em pelo menos 10% dos sorteios
                .OrderByDescending(g => g.Value)
                .Take(10)
                .Select(g => g.Key.Split(',').Select(int.Parse).ToArray())
                .ToList();
        }

        /// <summary>
        /// Gera todas as combinações de um tamanho específico
        /// </summary>
        private static IEnumerable<List<int>> GerarCombinacoes(List<int> lista, int tamanho)
        {
            if (tamanho == 1)
                return lista.Select(x => new List<int> { x });

            return GerarCombinacoes(lista, tamanho - 1)
                .SelectMany(t => lista.Where(o => o > t.Last()), (t, o) => t.Concat(new[] { o }).ToList());
        }
    }
}