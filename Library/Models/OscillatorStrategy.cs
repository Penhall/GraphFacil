// D:\PROJETOS\GraphFacil\Library\Models\OscillatorStrategy.cs
using System.Collections.Generic;
using System.Linq;
using System;
using LotoLibrary.Models.Core;

// D:\PROJETOS\GraphFacil\Dashboard\ViewModel\OscillatorStrategy.cs
namespace LotoLibrary.Models
{
    /// <summary>
    /// Estratégias para osciladores - VERSÃO CORRIGIDA E COMPLETA
    /// </summary>
    public static class OscillatorStrategy
    {
        /// <summary>
        /// Aplica estratégia de tendência de curto prazo
        /// </summary>
        public static void AplicarTendenciaCurtoPrazo(List<DezenaOscilante> osciladores, List<Lance> historico)
        {
            if (!historico.Any() || !osciladores.Any()) return;

            try
            {
                // Analisar os últimos 10 concursos
                var recentGames = historico.TakeLast(10).ToList();

                foreach (var osc in osciladores)
                {
                    // Calcular frequência recente
                    var frequency = recentGames.Count(g => g.Lista?.Contains(osc.Numero) == true);

                    // Ajustar frequência baseada na tendência
                    osc.Frequencia = Math.Max(0.5, Math.Min(2.0, 1.0 + frequency * 0.15));

                    // Aumentar força de sincronização para números "quentes"
                    if (frequency >= 3)
                    {
                        osc.ForcaSincronizacao = Math.Min(1.0, osc.ForcaSincronizacao + 0.2);
                    }
                    else if (frequency == 0)
                    {
                        // Números "frios" também podem estar para sair
                        osc.ForcaSincronizacao = Math.Min(1.0, osc.ForcaSincronizacao + 0.1);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em AplicarTendenciaCurtoPrazo: {ex.Message}");
            }
        }

        /// <summary>
        /// Aplica estratégia de números quentes e frios
        /// </summary>
        public static void AplicarQuentesFrios(List<DezenaOscilante> osciladores, List<Lance> historico)
        {
            if (!historico.Any() || !osciladores.Any()) return;

            try
            {
                // Analisar últimos 20 concursos para identificar padrões
                var recentHistory = historico.TakeLast(20).ToList();

                // Calcular frequências
                var frequencias = new Dictionary<int, int>();
                foreach (var lance in recentHistory)
                {
                    if (lance.Lista != null)
                    {
                        foreach (var numero in lance.Lista)
                        {
                            frequencias[numero] = frequencias.GetValueOrDefault(numero, 0) + 1;
                        }
                    }
                }

                // Identificar quartis
                var valores = frequencias.Values.OrderBy(x => x).ToList();
                if (valores.Count >= 4)
                {
                    var q1 = valores[valores.Count / 4];
                    var q3 = valores[3 * valores.Count / 4];

                    foreach (var osc in osciladores)
                    {
                        var freq = frequencias.GetValueOrDefault(osc.Numero, 0);

                        if (freq >= q3) // Número quente
                        {
                            osc.Amplitude = Math.Min(2.0, osc.Amplitude * 1.1);
                            osc.ForcaSincronizacao = Math.Min(1.0, osc.ForcaSincronizacao + 0.15);
                        }
                        else if (freq <= q1) // Número frio
                        {
                            osc.Amplitude = Math.Min(2.0, osc.Amplitude * 1.05);
                            osc.ForcaSincronizacao = Math.Min(1.0, osc.ForcaSincronizacao + 0.1);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em AplicarQuentesFrios: {ex.Message}");
            }
        }

        /// <summary>
        /// Aplica estratégia de padrões de grupos
        /// </summary>
        public static void AplicarPadroesGrupos(List<DezenaOscilante> osciladores, List<int[]> gruposFrequentes)
        {
            if (!gruposFrequentes.Any() || !osciladores.Any()) return;

            try
            {
                foreach (var grupo in gruposFrequentes)
                {
                    var membros = osciladores.Where(d => grupo.Contains(d.Numero)).ToList();

                    if (membros.Count >= 2)
                    {
                        double mediaForca = membros.Average(d => d.ForcaSincronizacao);
                        double mediaFase = membros.Average(d => d.Fase);

                        foreach (var dezena in membros)
                        {
                            // Sincronizar força
                            dezena.ForcaSincronizacao = (dezena.ForcaSincronizacao + mediaForca) / 2;

                            // Aproximar fases
                            var diferencaFase = mediaFase - dezena.Fase;
                            if (Math.Abs(diferencaFase) > 180)
                            {
                                diferencaFase = diferencaFase > 0 ? diferencaFase - 360 : diferencaFase + 360;
                            }
                            dezena.Fase += diferencaFase * 0.1; // Ajuste gradual
                            dezena.Fase = (dezena.Fase % 360 + 360) % 360; // Normalizar
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em AplicarPadroesGrupos: {ex.Message}");
            }
        }

        /// <summary>
        /// Aplica estratégia baseada em ciclos históricos
        /// </summary>
        public static void AplicarCiclos(List<DezenaOscilante> osciladores, Dictionary<int, int> ciclosMedios)
        {
            if (!ciclosMedios.Any() || !osciladores.Any()) return;

            try
            {
                foreach (var osc in osciladores)
                {
                    if (ciclosMedios.TryGetValue(osc.Numero, out var ciclo) && ciclo > 0)
                    {
                        // Calcular posição no ciclo
                        double posicaoNoCiclo = (double)osc.UltimoAtraso / ciclo;

                        // Ajustar frequência baseada na posição no ciclo
                        double ajuste = Math.Sin(posicaoNoCiclo * Math.PI * 2) * 0.3;
                        osc.Frequencia = Math.Max(0.5, Math.Min(2.0, osc.Frequencia * (1 + ajuste)));

                        // Ajustar probabilidade
                        double probabilidadeCiclo = 1.0 - Math.Abs(posicaoNoCiclo - 1.0);
                        osc.Probabilidade = Math.Max(0.1, Math.Min(0.9,
                            (osc.Probabilidade + probabilidadeCiclo) / 2));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em AplicarCiclos: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera palpite para validação baseado nos osciladores
        /// </summary>
        public static List<int> GerarPalpiteValidacao(List<DezenaOscilante> osciladores, List<Lance> dadosValidacao)
        {
            if (!osciladores.Any()) return GerarPalpiteAleatorio();

            try
            {
                // Atualizar probabilidades baseadas no estado atual
                AtualizarProbabilidades(osciladores);

                // Estratégia 1: Dezenas sincronizadas com alta força
                var sincronizadas = osciladores
                    .Where(o => o.EstaSincronizada && o.ForcaSincronizacao > 0.5)
                    .OrderByDescending(o => o.ForcaSincronizacao)
                    .ThenByDescending(o => o.Probabilidade)
                    .Take(8)
                    .Select(o => o.Numero)
                    .ToList();

                // Estratégia 2: Dezenas com alta probabilidade
                var altaProbabilidade = osciladores
                    .Where(o => o.Probabilidade > 0.6)
                    .OrderByDescending(o => o.Probabilidade)
                    .ThenByDescending(o => o.Amplitude)
                    .Take(10)
                    .Select(o => o.Numero)
                    .ToList();

                // Estratégia 3: Dezenas em fase ótima (pico da onda)
                var faseOtima = osciladores
                    .Where(o => Math.Abs(Math.Sin(o.Fase * Math.PI / 180)) > 0.7)
                    .OrderByDescending(o => Math.Abs(Math.Sin(o.Fase * Math.PI / 180)))
                    .Take(7)
                    .Select(o => o.Numero)
                    .ToList();

                // Combinar estratégias sem duplicatas
                var candidatos = sincronizadas
                    .Union(altaProbabilidade)
                    .Union(faseOtima)
                    .Distinct()
                    .ToList();

                // Se não temos candidatos suficientes, adicionar os melhores por score geral
                if (candidatos.Count < 15)
                {
                    var restantes = osciladores
                        .Where(o => !candidatos.Contains(o.Numero))
                        .OrderByDescending(o => CalcularScoreGeral(o))
                        .Take(15 - candidatos.Count)
                        .Select(o => o.Numero);

                    candidatos.AddRange(restantes);
                }

                // Retornar os 15 melhores, ordenados
                return candidatos.Take(15).OrderBy(n => n).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em GerarPalpiteValidacao: {ex.Message}");
                return GerarPalpiteAleatorio();
            }
        }

        /// <summary>
        /// Gera palpite para próximo concurso
        /// </summary>
        public static List<int> GerarPalpiteProximoConcurso(List<DezenaOscilante> osciladores)
        {
            return GerarPalpiteValidacao(osciladores, new List<Lance>());
        }

        /// <summary>
        /// Atualiza probabilidades de todos os osciladores
        /// </summary>
        private static void AtualizarProbabilidades(List<DezenaOscilante> osciladores)
        {
            foreach (var osc in osciladores)
            {
                // Fator baseado na fase (onda senoidal)
                double fatorFase = (Math.Sin(osc.Fase * Math.PI / 180) + 1) / 2; // 0-1

                // Fator baseado no atraso
                double fatorAtraso = Math.Min(1.0, osc.UltimoAtraso / 20.0);

                // Fator baseado na sincronização
                double fatorSync = osc.ForcaSincronizacao;

                // Fator baseado na amplitude
                double fatorAmplitude = Math.Min(1.0, osc.Amplitude / 2.0);

                // Combinar fatores
                osc.Probabilidade = (fatorFase * 0.3) +
                                   (fatorAtraso * 0.25) +
                                   (fatorSync * 0.25) +
                                   (fatorAmplitude * 0.2);

                // Normalizar
                osc.Probabilidade = Math.Max(0.1, Math.Min(0.9, osc.Probabilidade));
            }
        }

        /// <summary>
        /// Calcula score geral de um oscilador
        /// </summary>
        private static double CalcularScoreGeral(DezenaOscilante osc)
        {
            var scoreProb = osc.Probabilidade * 0.4;
            var scoreForca = osc.ForcaSincronizacao * 0.3;
            var scoreAmplitude = Math.Min(1.0, osc.Amplitude / 2.0) * 0.2;
            var scoreFase = Math.Abs(Math.Sin(osc.Fase * Math.PI / 180)) * 0.1;

            return scoreProb + scoreForca + scoreAmplitude + scoreFase;
        }

        /// <summary>
        /// Gera palpite aleatório como fallback
        /// </summary>
        private static List<int> GerarPalpiteAleatorio()
        {
            var random = new Random();
            return Enumerable.Range(1, 25)
                .OrderBy(x => random.Next())
                .Take(15)
                .OrderBy(x => x)
                .ToList();
        }

        /// <summary>
        /// Aplica sincronização mútua entre osciladores
        /// </summary>
        public static void AplicarSincronizacaoMutua(List<DezenaOscilante> osciladores, double intensidade = 0.1)
        {
            if (!osciladores.Any()) return;

            try
            {
                // Cada oscilador é influenciado pelos outros
                var influencias = new Dictionary<int, double>();

                foreach (var dezena in osciladores)
                {
                    double influenciaTotal = 0;

                    foreach (var outra in osciladores)
                    {
                        if (outra.Numero != dezena.Numero)
                        {
                            // Calcular influência baseada na diferença de fase
                            var diferencaFase = outra.Fase - dezena.Fase;
                            var influencia = Math.Sin(diferencaFase * Math.PI / 180) *
                                           outra.ForcaSincronizacao * intensidade;

                            influenciaTotal += influencia;
                        }
                    }

                    influencias[dezena.Numero] = influenciaTotal;
                }

                // Aplicar influências
                foreach (var dezena in osciladores)
                {
                    if (influencias.TryGetValue(dezena.Numero, out var influencia))
                    {
                        dezena.AplicarInfluencia(influencia);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro em AplicarSincronizacaoMutua: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se dois osciladores estão sincronizados
        /// </summary>
        public static bool EstaoSincronizados(DezenaOscilante osc1, DezenaOscilante osc2, double tolerancia = 30.0)
        {
            if (osc1 == null || osc2 == null) return false;

            var diferencaFase = Math.Abs(osc1.Fase - osc2.Fase);
            diferencaFase = Math.Min(diferencaFase, 360 - diferencaFase); // Distância circular

            return diferencaFase <= tolerancia;
        }

        /// <summary>
        /// Encontra grupos de osciladores sincronizados
        /// </summary>
        public static List<List<int>> EncontrarGruposSincronizados(List<DezenaOscilante> osciladores, double tolerancia = 30.0)
        {
            var grupos = new List<List<int>>();

            foreach (var osc in osciladores)
            {
                // Verificar se já está em algum grupo
                if (grupos.Any(g => g.Contains(osc.Numero))) continue;

                // Criar novo grupo
                var novoGrupo = new List<int> { osc.Numero };

                // Encontrar osciladores sincronizados
                foreach (var outro in osciladores)
                {
                    if (outro.Numero != osc.Numero && EstaoSincronizados(osc, outro, tolerancia))
                    {
                        novoGrupo.Add(outro.Numero);
                    }
                }

                // Adicionar grupo se tem mais de 1 membro
                if (novoGrupo.Count > 1)
                {
                    grupos.Add(novoGrupo);
                }
            }

            return grupos;
        }
    }
}
