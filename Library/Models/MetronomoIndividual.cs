// LotoLibrary/Services/MetronomoIndividual.cs
using CommunityToolkit.Mvvm.ComponentModel;
using LotoLibrary.Enums;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services
{
    /// <summary>
    /// Representa um metrônomo individual para uma dezena específica
    /// Cada dezena tem seu padrão único baseado no histórico real
    /// VERSÃO CORRIGIDA - Inclui propriedade Fase para compatibilidade com Osciladores
    /// </summary>
    public partial class MetronomoIndividual : ObservableObject
    {
        #region Properties
        [ObservableProperty]
        private int _numero;

        [ObservableProperty]
        private List<int> _historicoAparicoes = new();

        [ObservableProperty]
        private double _cicloMedio;

        [ObservableProperty]
        private double _variancaCiclo;

        [ObservableProperty]
        private int _ultimaAparicao;

        [ObservableProperty]
        private int _intervalAtual;

        [ObservableProperty]
        private double _probabilidadeAtual;

        [ObservableProperty]
        private TipoMetronomo _tipoMetronomo;

        [ObservableProperty]
        private bool _emFaseOtima;

        [ObservableProperty]
        private string _descricaoPadrao = string.Empty;

        [ObservableProperty]
        private List<PrevisaoConcurso> _proximasPrevisoes = new();

        // CORREÇÃO: Adicionada propriedade Fase para compatibilidade com sistema de osciladores
        [ObservableProperty]
        private double _fase = 0.0;

        // CORREÇÃO: Propriedade Frequencia para compatibilidade
        [ObservableProperty]
        private double _frequencia = 1.0;

        public List<double> Intervalos { get; private set; } = new();
        public DateTime UltimaAnalise { get; private set; }
        public double Confiabilidade { get; private set; }
        #endregion

        #region Constructors

        // CORREÇÃO: Construtor padrão
        public MetronomoIndividual()
        {
            InicializarValoresPadrao();
        }

        // CORREÇÃO: Construtor que aceita apenas o número (para resolver CS1729)
        public MetronomoIndividual(int numero)
        {
            Numero = numero;
            InicializarValoresPadrao();
        }

        // Construtor original com histórico
        public MetronomoIndividual(int numero, List<int> historico)
        {
            Numero = numero;
            HistoricoAparicoes = historico.OrderBy(x => x).ToList();
            InicializarValoresPadrao();
            AnalisarPadroes();
        }

        private void InicializarValoresPadrao()
        {
            var random = new Random(Numero); // Seed baseado no número para consistência
            Fase = random.NextDouble() * 360;
            Frequencia = 1.0 + (random.NextDouble() - 0.5) * 0.4; // 0.8 - 1.2
        }
        #endregion

        #region Main Analysis Methods
        /// <summary>
        /// Analisa completamente o histórico e detecta padrões únicos
        /// </summary>
        public void AnalisarPadroes()
        {
            if (HistoricoAparicoes.Count < 3)
            {
                TipoMetronomo = TipoMetronomo.DadosInsuficientes;
                return;
            }

            // 1. Calcular intervalos entre aparições
            CalcularIntervalos();

            // 2. Estatísticas básicas
            CalcularEstatisticasBasicas();

            // 3. Detectar tipo de padrão
            DetectarTipoPadrao();

            // 4. Calcular confiabilidade
            CalcularConfiabilidade();

            // 5. Gerar descrição
            GerarDescricaoPadrao();

            // 6. Gerar previsões
            GerarPrevisoes();

            // 7. Atualizar fase baseada no padrão detectado
            AtualizarFaseComPadrao();

            UltimaAnalise = DateTime.Now;
        }

        /// <summary>
        /// Calcula probabilidade para um concurso específico - VERSÃO CORRIGIDA
        /// </summary>
        public double CalcularProbabilidadePara(int concurso)
        {
            try
            {
                // 1. Validações básicas
                if (HistoricoAparicoes.Count < 2)
                {
                    // Se não há histórico suficiente, usar probabilidade baseada na frequência teórica
                    return 0.15 + (Numero * 0.001); // Pequena variação para diferenciar
                }

                // 2. Calcular intervalo atual
                var ultimaAparicao = HistoricoAparicoes.LastOrDefault();
                if (ultimaAparicao == 0)
                {
                    return 0.15 + (Numero * 0.001); // Fallback
                }

                var intervaloAtual = concurso - ultimaAparicao;

                // 3. Verificar se ciclo médio é válido
                if (CicloMedio <= 0 || double.IsNaN(CicloMedio) || double.IsInfinity(CicloMedio))
                {
                    // Recalcular ciclo médio de emergência
                    CicloMedio = CalcularCicloMedioSeguro();
                }

                // 4. Verificar se variância é válida
                if (VariancaCiclo <= 0 || double.IsNaN(VariancaCiclo) || double.IsInfinity(VariancaCiclo))
                {
                    VariancaCiclo = Math.Max(1.0, CicloMedio * 0.3); // Variância padrão
                }

                // 5. Cálculo base da probabilidade usando distribuição normal
                var probabilidadeBase = CalcularProbabilidadeBaseCorrigida(intervaloAtual);

                // 6. Aplicar ajustes por tipo de padrão
                var ajusteTipo = AplicarAjustePorTipoCorrigido(intervaloAtual);

                // 7. Combinar probabilidades
                var probabilidadeFinal = probabilidadeBase * ajusteTipo;

                // 8. Garantir que está no range válido [0.01, 0.99] com variação
                probabilidadeFinal = Math.Max(0.01, Math.Min(0.99, probabilidadeFinal));

                // 9. Adicionar pequena variação única baseada na dezena para evitar empates
                probabilidadeFinal += (Numero * 0.0001) + (intervaloAtual * 0.00001);

                // 10. Atualizar propriedades relacionadas
                ProbabilidadeAtual = probabilidadeFinal;
                IntervalAtual = intervaloAtual;
                EmFaseOtima = probabilidadeFinal > 0.65;

                return probabilidadeFinal;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no cálculo de probabilidade da dezena {Numero}: {ex.Message}");
                // Retornar probabilidade baseada apenas na dezena como fallback
                return 0.15 + (Numero * 0.001);
            }
        }

        /// <summary>
        /// Atualiza o metrônomo com resultado de novo sorteio
        /// </summary>
        public void AtualizarComSorteio(int concurso, bool foiSorteada)
        {
            if (foiSorteada)
            {
                HistoricoAparicoes.Add(concurso);
                HistoricoAparicoes = HistoricoAparicoes.OrderBy(x => x).ToList();

                // Reanalisa padrões com novo dado
                AnalisarPadroes();
            }
            else
            {
                // Apenas atualiza estado atual
                AtualizarEstadoAtual(concurso);
            }
        }

        /// <summary>
        /// Atualiza a fase do metrônomo (compatibilidade com osciladores)
        /// </summary>
        public void AtualizarFase(double deltaTime = 1.0)
        {
            Fase += Frequencia * deltaTime;
            Fase = (Fase % 360 + 360) % 360; // Normaliza para 0-360

            // Atualizar probabilidade quando a fase muda
            if (HistoricoAparicoes.Any())
            {
                var proximoConcurso = HistoricoAparicoes.LastOrDefault() + 1;
                ProbabilidadeAtual = CalcularProbabilidadePara(proximoConcurso);
            }
        }
        #endregion

        #region Private Helper Methods
        private void CalcularIntervalos()
        {
            Intervalos.Clear();
            for (int i = 1; i < HistoricoAparicoes.Count; i++)
            {
                var intervalo = HistoricoAparicoes[i] - HistoricoAparicoes[i - 1];
                Intervalos.Add(intervalo);
            }
        }

        private void CalcularEstatisticasBasicas()
        {
            if (Intervalos.Count == 0) return;

            CicloMedio = Intervalos.Average();

            var variancia = Intervalos.Sum(x => Math.Pow(x - CicloMedio, 2)) / Intervalos.Count;
            VariancaCiclo = variancia;

            UltimaAparicao = HistoricoAparicoes.LastOrDefault();
        }

        private void DetectarTipoPadrao()
        {
            if (Intervalos.Count < 5)
            {
                TipoMetronomo = TipoMetronomo.DadosInsuficientes;
                return;
            }

            var coeficienteVariacao = Math.Sqrt(VariancaCiclo) / CicloMedio;

            // Padrão muito regular (baixa variação)
            if (coeficienteVariacao < 0.15)
            {
                TipoMetronomo = TipoMetronomo.Regular;
                return;
            }

            // Verificar alternância (padrão A-B-A-B)
            if (DetectarAlternancia())
            {
                TipoMetronomo = TipoMetronomo.Alternado;
                return;
            }

            // Verificar ciclo longo repetitivo
            if (DetectarCicloLongo())
            {
                TipoMetronomo = TipoMetronomo.CicloLongo;
                return;
            }

            // Verificar tendência crescente/decrescente
            if (DetectarTendencia())
            {
                TipoMetronomo = TipoMetronomo.Tendencial;
                return;
            }

            // Verificar se há múltiplos picos (multi-modal)
            if (DetectarMultiModal())
            {
                TipoMetronomo = TipoMetronomo.MultiModal;
                return;
            }

            // Padrão irregular
            TipoMetronomo = TipoMetronomo.Irregular;
        }

        private bool DetectarAlternancia()
        {
            if (Intervalos.Count < 8) return false;

            // Verificar padrão A-B-A-B nos últimos intervalos
            var grupos = new List<double>();
            for (int i = 0; i < Intervalos.Count - 1; i += 2)
            {
                if (i + 1 < Intervalos.Count)
                {
                    grupos.Add(Math.Abs(Intervalos[i] - Intervalos[i + 1]));
                }
            }

            var mediaGrupos = grupos.Average();
            return mediaGrupos > CicloMedio * 0.4; // Diferença significativa entre pares
        }

        private bool DetectarCicloLongo()
        {
            // Buscar padrão repetitivo de 3 a 8 elementos
            for (int tamanho = 3; tamanho <= Math.Min(8, Intervalos.Count / 3); tamanho++)
            {
                if (VerificarPadraoRepetitivo(tamanho))
                    return true;
            }
            return false;
        }

        private bool VerificarPadraoRepetitivo(int tamanho)
        {
            if (Intervalos.Count < tamanho * 2) return false;

            var padrao = Intervalos.Take(tamanho).ToList();
            var tolerancia = Math.Max(1.0, CicloMedio * 0.2);

            for (int inicio = tamanho; inicio + tamanho <= Intervalos.Count; inicio += tamanho)
            {
                var segmento = Intervalos.Skip(inicio).Take(tamanho).ToList();

                for (int j = 0; j < tamanho; j++)
                {
                    if (Math.Abs(padrao[j] - segmento[j]) > tolerancia)
                        return false;
                }
            }
            return true;
        }

        private bool DetectarTendencia()
        {
            if (Intervalos.Count < 10) return false;

            // Regressão linear simples
            var n = Intervalos.Count;
            var x = Enumerable.Range(0, n).Select(i => (double)i).ToList();
            var y = Intervalos;

            var mediaX = x.Average();
            var mediaY = y.Average();

            var numerador = x.Zip(y, (xi, yi) => (xi - mediaX) * (yi - mediaY)).Sum();
            var denominador = x.Sum(xi => Math.Pow(xi - mediaX, 2));

            if (denominador == 0) return false;

            var inclinacao = numerador / denominador;

            // Tendência significativa se > 10% do ciclo médio por posição
            return Math.Abs(inclinacao) > CicloMedio * 0.1;
        }

        private bool DetectarMultiModal()
        {
            // Detectar se há múltiplos "picos" de frequência nos intervalos
            var grupos = Intervalos.GroupBy(i => Math.Round(i)).Where(g => g.Count() > 1);
            return grupos.Count() >= 3; // Pelo menos 3 grupos com repetições
        }

        private void CalcularConfiabilidade()
        {
            if (Intervalos.Count < 5)
            {
                Confiabilidade = 0.2;
                return;
            }

            // Fatores que aumentam confiabilidade:
            var fatorQuantidade = Math.Min(1.0, Intervalos.Count / 20.0); // Mais dados = melhor
            var fatorConsistencia = 1.0 - Math.Min(1.0, Math.Sqrt(VariancaCiclo) / CicloMedio); // Menor variação = melhor
            var fatorTipo = TipoMetronomo switch
            {
                TipoMetronomo.Regular => 0.9,
                TipoMetronomo.CicloLongo => 0.8,
                TipoMetronomo.Alternado => 0.7,
                TipoMetronomo.Tendencial => 0.6,
                TipoMetronomo.MultiModal => 0.5,
                TipoMetronomo.Irregular => 0.3,
                _ => 0.2
            };

            Confiabilidade = (fatorQuantidade + fatorConsistencia + fatorTipo) / 3.0;
        }

        private void GerarDescricaoPadrao()
        {
            DescricaoPadrao = TipoMetronomo switch
            {
                TipoMetronomo.Regular => $"Ciclo regular ~{CicloMedio:F1} concursos",
                TipoMetronomo.Alternado => $"Padrão alternado, média {CicloMedio:F1}",
                TipoMetronomo.CicloLongo => $"Ciclo longo repetitivo, ~{CicloMedio:F1}",
                TipoMetronomo.Tendencial => $"Tendência evolutiva, atual ~{CicloMedio:F1}",
                TipoMetronomo.MultiModal => $"Múltiplos padrões, média {CicloMedio:F1}",
                TipoMetronomo.Irregular => $"Padrão irregular, ~{CicloMedio:F1}",
                _ => "Padrão indeterminado"
            };
        }

        public void AtualizarEstadoAtual(int? concursoAtual = null)
        {
            var concurso = concursoAtual ?? (UltimaAparicao + 1);
            IntervalAtual = concurso - UltimaAparicao;
            ProbabilidadeAtual = CalcularProbabilidadePara(concurso);
            EmFaseOtima = ProbabilidadeAtual > 0.65;
        }

        private void GerarPrevisoes()
        {
            ProximasPrevisoes.Clear();

            var baseConcurso = Math.Max(UltimaAparicao, HistoricoAparicoes.LastOrDefault());

            for (int i = 1; i <= 20; i++)
            {
                var concurso = baseConcurso + i;
                var probabilidade = CalcularProbabilidadePara(concurso);

                ProximasPrevisoes.Add(new PrevisaoConcurso
                {
                    Concurso = concurso,
                    Probabilidade = probabilidade,
                    IntervaloEsperado = concurso - UltimaAparicao,
                    Confianca = CalcularConfiancaPrevisao(probabilidade)
                });
            }

            // Ordenar por probabilidade decrescente
            ProximasPrevisoes = ProximasPrevisoes.OrderByDescending(p => p.Probabilidade).ToList();
        }

        /// <summary>
        /// Atualiza a fase baseada no padrão detectado
        /// </summary>
        private void AtualizarFaseComPadrao()
        {
            if (UltimaAparicao > 0 && CicloMedio > 0)
            {
                // Calcular posição no ciclo
                var posicaoNoCiclo = IntervalAtual % CicloMedio;
                Fase = (posicaoNoCiclo / CicloMedio) * 360;

                // Ajustar frequência baseada no tipo de padrão
                Frequencia = TipoMetronomo switch
                {
                    TipoMetronomo.Regular => 1.0,
                    TipoMetronomo.Alternado => 2.0,
                    TipoMetronomo.CicloLongo => 0.5,
                    TipoMetronomo.Tendencial => 1.2,
                    TipoMetronomo.MultiModal => 1.5,
                    _ => 1.0
                };
            }
        }

        /// <summary>
        /// Calcula ciclo médio de forma segura
        /// </summary>
        private double CalcularCicloMedioSeguro()
        {
            try
            {
                if (Intervalos.Count == 0)
                {
                    // Se não há intervalos, usar a frequência teórica da Lotofácil
                    // Em média, cada dezena aparece a cada ~7 concursos (25 dezenas, 15 sorteadas)
                    return 25.0 / 15.0 * 3.5; // Aproximadamente 5.8
                }

                var media = Intervalos.Average();

                // Verificar se a média está em um range razoável
                if (media <= 0 || media > 100)
                {
                    return 5.8; // Valor padrão baseado na teoria
                }

                return media;
            }
            catch
            {
                return 5.8; // Valor padrão
            }
        }

        /// <summary>
        /// Cálculo base corrigido da probabilidade
        /// </summary>
        private double CalcularProbabilidadeBaseCorrigida(double intervalo)
        {
            try
            {
                // Usar distribuição gaussiana centrada no ciclo médio
                var diferenca = Math.Abs(intervalo - CicloMedio);
                var desvio = Math.Max(1.0, Math.Sqrt(VariancaCiclo));

                // Fórmula da distribuição normal
                var expoente = -Math.Pow(diferenca / desvio, 2) / 2;
                var probabilidade = Math.Exp(expoente);

                // Normalizar para que o pico seja em torno de 0.8
                probabilidade *= 0.8;

                // Garantir valor mínimo
                return Math.Max(0.05, probabilidade);
            }
            catch
            {
                // Fallback simples: probabilidade inversamente proporcional à diferença do ciclo
                var diferenca = Math.Abs(intervalo - CicloMedio);
                return Math.Max(0.05, 0.8 / (1 + diferenca / CicloMedio));
            }
        }

        /// <summary>
        /// Aplicar ajustes por tipo com mais variação
        /// </summary>
        private double AplicarAjustePorTipoCorrigido(double intervalo)
        {
            try
            {
                return TipoMetronomo switch
                {
                    TipoMetronomo.Regular => AjusteRegularCorrigido(intervalo),
                    TipoMetronomo.Alternado => AjusteAlternadoCorrigido(intervalo),
                    TipoMetronomo.CicloLongo => AjusteCicloLongoCorrigido(intervalo),
                    TipoMetronomo.Tendencial => AjusteTendencialCorrigido(intervalo),
                    TipoMetronomo.MultiModal => AjusteMultiModalCorrigido(intervalo),
                    TipoMetronomo.Irregular => AjusteIrregularCorrigido(intervalo),
                    _ => 1.0
                };
            }
            catch
            {
                return 1.0;
            }
        }

        private double AjusteRegularCorrigido(double intervalo)
        {
            var proximidade = Math.Abs(intervalo - CicloMedio);
            var fator = proximidade < 1 ? 1.4 : Math.Max(0.6, 1.0 - (proximidade / CicloMedio) * 0.3);
            return fator + (Numero * 0.001); // Pequena variação por dezena
        }

        private double AjusteAlternadoCorrigido(double intervalo)
        {
            var posicao = HistoricoAparicoes.Count % 2;
            var esperado = posicao == 0 ? CicloMedio * 0.7 : CicloMedio * 1.3;
            var proximidade = Math.Abs(intervalo - esperado);
            var fator = proximidade < 2 ? 1.3 : Math.Max(0.7, 1.0 - (proximidade / CicloMedio) * 0.2);
            return fator + (Numero * 0.002);
        }

        private double AjusteCicloLongoCorrigido(double intervalo)
        {
            // Para ciclos longos, verificar posição no ciclo
            var posicaoNoCiclo = HistoricoAparicoes.Count % Math.Max(3, (int)Math.Round(CicloMedio));
            var fatorCiclo = Math.Sin((posicaoNoCiclo / CicloMedio) * Math.PI * 2) * 0.3 + 1.0;
            return Math.Max(0.5, fatorCiclo) + (Numero * 0.0015);
        }

        private double AjusteTendencialCorrigido(double intervalo)
        {
            // Para padrões com tendência, considerar direção da tendência
            var tendencia = CalcularTendenciaLocal();
            var esperado = CicloMedio + tendencia;
            var proximidade = Math.Abs(intervalo - esperado);
            var fator = proximidade < 2 ? 1.25 : Math.Max(0.75, 1.0 - (proximidade / esperado) * 0.25);
            return fator + (Numero * 0.0012);
        }

        private double AjusteMultiModalCorrigido(double intervalo)
        {
            // Para padrões multi-modais, verificar se está próximo de algum dos picos
            var grupos = Intervalos.GroupBy(i => Math.Round(i)).Where(g => g.Count() > 1);
            var proximoPico = grupos.Any(g => Math.Abs(intervalo - g.Key) < 1.5);
            var fator = proximoPico ? 1.2 : 0.8;
            return fator + (Numero * 0.0018);
        }

        private double AjusteIrregularCorrigido(double intervalo)
        {
            // Para padrões irregulares, usar variação mais aleatória mas determinística
            var seed = Numero + HistoricoAparicoes.Count;
            var random = new Random(seed);
            var fator = 0.7 + (random.NextDouble() * 0.6); // Entre 0.7 e 1.3
            return fator;
        }

        private double CalcularTendenciaLocal()
        {
            try
            {
                if (Intervalos.Count < 5) return 0;

                var recentes = Intervalos.TakeLast(5).ToList();
                var antigos = Intervalos.Take(5).ToList();

                return recentes.Average() - antigos.Average();
            }
            catch
            {
                return 0;
            }
        }

        private ConfiancaPrevisao CalcularConfiancaPrevisao(double probabilidade)
        {
            var confiancaBase = Confiabilidade * probabilidade;

            if (confiancaBase > 0.7) return ConfiancaPrevisao.Alta;
            if (confiancaBase > 0.4) return ConfiancaPrevisao.Media;
            if (confiancaBase > 0.2) return ConfiancaPrevisao.Baixa;
            return ConfiancaPrevisao.MuitoBaixa;
        }
        #endregion

        #region Public Info Methods
        public string ObterAnaliseDetalhada()
        {
            var analise = $"=== ANÁLISE DETALHADA - DEZENA {Numero:D2} ===\n\n";

            analise += $"📊 ESTATÍSTICAS:\n";
            analise += $"Aparições totais: {HistoricoAparicoes.Count}\n";
            analise += $"Ciclo médio: {CicloMedio:F1} concursos\n";
            analise += $"Variação: ±{Math.Sqrt(VariancaCiclo):F1} concursos\n";
            analise += $"Tipo de padrão: {TipoMetronomo}\n";
            analise += $"Confiabilidade: {Confiabilidade:P1}\n";
            analise += $"Fase atual: {Fase:F1}°\n";
            analise += $"Frequência: {Frequencia:F2}\n\n";

            analise += $"🎯 ESTADO ATUAL:\n";
            analise += $"Última aparição: Concurso {UltimaAparicao}\n";
            analise += $"Intervalo atual: {IntervalAtual} concursos\n";
            analise += $"Probabilidade atual: {ProbabilidadeAtual:P1}\n";
            analise += $"Em fase ótima: {(EmFaseOtima ? "SIM" : "NÃO")}\n\n";

            if (ProximasPrevisoes.Any())
            {
                analise += $"🔮 PRÓXIMAS PREVISÕES (Top 5):\n";
                foreach (var previsao in ProximasPrevisoes.Take(5))
                {
                    analise += $"Concurso {previsao.Concurso}: {previsao.Probabilidade:P1} " +
                              $"(Intervalo {previsao.IntervaloEsperado}, {previsao.Confianca})\n";
                }
                analise += "\n";
            }

            analise += $"📈 ÚLTIMOS 10 INTERVALOS:\n";
            if (Intervalos.Count >= 10)
            {
                var ultimos = Intervalos.TakeLast(10).Select(i => i.ToString("F0"));
                analise += $"[{string.Join(", ", ultimos)}]\n";
            }

            analise += $"\n💡 DESCRIÇÃO: {DescricaoPadrao}";

            return analise;
        }

        public override string ToString()
        {
            return $"Dezena {Numero:D2}: {DescricaoPadrao} ({ProbabilidadeAtual:P1}) - Fase: {Fase:F1}°";
        }
        #endregion

        #region Compatibility Methods for Oscillator System

        /// <summary>
        /// Verifica se o metrônomo está sincronizado (compatibilidade com osciladores)
        /// </summary>
        public bool EstaSincronizado => EmFaseOtima && Confiabilidade > 0.5;

        /// <summary>
        /// Força de sincronização baseada na confiabilidade
        /// </summary>
        public double ForcaSincronizacao => Confiabilidade * (EmFaseOtima ? 1.2 : 0.8);

        /// <summary>
        /// Valor atual da "onda" baseado na fase
        /// </summary>
        public double ValorAtual => Math.Sin(Fase * Math.PI / 180);

        /// <summary>
        /// Aplica influência externa (compatibilidade com sistema de osciladores)
        /// </summary>
        public void AplicarInfluencia(double influencia)
        {
            // Ajustar frequência baseada na influência
            var ajuste = influencia * 0.1;
            Frequencia = Math.Max(0.5, Math.Min(2.0, Frequencia + ajuste));

            // Atualizar fase
            AtualizarFase();
        }

        #endregion
    }
}