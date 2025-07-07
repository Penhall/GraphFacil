using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services;

/// <summary>
/// Representa um metrônomo individual para uma dezena específica
/// Cada dezena tem seu padrão único baseado no histórico real
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

    public List<double> Intervalos { get; private set; } = new();
    public DateTime UltimaAnalise { get; private set; }
    public double Confiabilidade { get; private set; }
    #endregion

    #region Constructor
    public MetronomoIndividual()
    {
    }

    public MetronomoIndividual(int numero, List<int> historico)
    {
        Numero = numero;
        HistoricoAparicoes = historico.OrderBy(x => x).ToList();
        AnalisarPadroes();
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

        // 6. Atualizar estado atual
        AtualizarEstadoAtual();

        UltimaAnalise = DateTime.Now;
    }

    /// <summary>
    /// Calcula a probabilidade da dezena aparecer em um concurso específico
    /// </summary>
    public double CalcularProbabilidadePara(int concursoAlvo)
    {
        if (Intervalos.Count == 0 || UltimaAparicao == 0)
            return 0.4; // Probabilidade neutra

        var intervaloAtual = concursoAlvo - UltimaAparicao;

        // Probabilidade base usando o padrão detectado
        var probabilidadeBase = CalcularProbabilidadeBase(intervaloAtual);

        // Ajustes específicos por tipo de padrão
        var ajusteTipo = AplicarAjustePorTipo(intervaloAtual);

        // Ajuste por tendência temporal
        var ajusteTendencia = CalcularAjusteTendencia(concursoAlvo);

        // Ajuste por confiabilidade do padrão
        var ajusteConfiabilidade = 0.5 + (Confiabilidade * 0.5);

        var probabilidadeFinal = probabilidadeBase * ajusteTipo * ajusteTendencia * ajusteConfiabilidade;

        // Limitar entre 1% e 95%
        return Math.Max(0.01, Math.Min(0.95, probabilidadeFinal));
    }

    /// <summary>
    /// Gera previsões para os próximos concursos
    /// </summary>
    public void GerarPrevisoes(int concursoAtual, int quantidade = 20)
    {
        ProximasPrevisoes.Clear();

        for (int i = 1; i <= quantidade; i++)
        {
            var concurso = concursoAtual + i;
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

    private void AtualizarEstadoAtual(int? concursoAtual = null)
    {
        var concurso = concursoAtual ?? (UltimaAparicao + 1);
        IntervalAtual = concurso - UltimaAparicao;
        ProbabilidadeAtual = CalcularProbabilidadePara(concurso);
        EmFaseOtima = ProbabilidadeAtual > 0.65;
    }

    private double CalcularProbabilidadeBase(double intervalo)
    {
        // Distribuição normal centrada no ciclo médio
        var diferenca = Math.Abs(intervalo - CicloMedio);
        var desvio = Math.Sqrt(VariancaCiclo);

        // Função gaussiana normalizada
        var expoente = -Math.Pow(diferenca / desvio, 2) / 2;
        var probabilidade = Math.Exp(expoente);

        return probabilidade;
    }

    private double AplicarAjustePorTipo(double intervalo)
    {
        return TipoMetronomo switch
        {
            TipoMetronomo.Regular => AjusteRegular(intervalo),
            TipoMetronomo.Alternado => AjusteAlternado(intervalo),
            TipoMetronomo.CicloLongo => AjusteCicloLongo(intervalo),
            TipoMetronomo.Tendencial => AjusteTendencial(intervalo),
            TipoMetronomo.MultiModal => AjusteMultiModal(intervalo),
            _ => 1.0
        };
    }

    private double AjusteRegular(double intervalo)
    {
        // Para padrões regulares, forte preferência pelo ciclo exato
        var proximidade = Math.Abs(intervalo - CicloMedio);
        return proximidade < 1 ? 1.3 : 0.7;
    }

    private double AjusteAlternado(double intervalo)
    {
        // Para padrões alternados, considerar posição no ciclo
        var posicao = HistoricoAparicoes.Count % 2;
        var esperado = posicao == 0 ? CicloMedio * 0.8 : CicloMedio * 1.2;
        return Math.Abs(intervalo - esperado) < 1 ? 1.2 : 0.8;
    }

    private double AjusteCicloLongo(double intervalo)
    {
        // Verificar se estamos na posição correta do ciclo longo
        var posicaoNoCiclo = HistoricoAparicoes.Count % 6; // Assumindo ciclo de 6
        // Implementar lógica específica baseada na detecção do ciclo
        return 1.0; // Placeholder
    }

    private double AjusteTendencial(double intervalo)
    {
        // Ajustar baseado na tendência detectada
        var tendencia = CalcularTendenciaAtual();
        var esperado = CicloMedio + (tendencia * 0.1);
        return Math.Abs(intervalo - esperado) < 2 ? 1.1 : 0.9;
    }

    private double AjusteMultiModal(double intervalo)
    {
        // Verificar se o intervalo coincide com algum dos "picos"
        var grupos = Intervalos.GroupBy(i => Math.Round(i)).Where(g => g.Count() > 1);
        foreach (var grupo in grupos)
        {
            if (Math.Abs(intervalo - grupo.Key) < 1)
                return 1.15;
        }
        return 0.85;
    }

    private double CalcularTendenciaAtual()
    {
        if (Intervalos.Count < 5) return 0;

        var recentes = Intervalos.TakeLast(5).ToList();
        var antigos = Intervalos.Take(5).ToList();

        return recentes.Average() - antigos.Average();
    }

    private double CalcularAjusteTendencia(int concursoAlvo)
    {
        // Ajuste muito sutil baseado em posição temporal
        var posicaoRelativa = (double)concursoAlvo / (HistoricoAparicoes.Max() + 100);
        return 0.95 + (posicaoRelativa * 0.1); // Entre 0.95 e 1.05
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
        analise += $"Confiabilidade: {Confiabilidade:P1}\n\n";

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
        return $"Dezena {Numero:D2}: {DescricaoPadrao} ({ProbabilidadeAtual:P1})";
    }
    #endregion
}

#region Enums and Helper Classes
public enum TipoMetronomo
{
    DadosInsuficientes,
    Regular,        // Intervalos consistentes
    Alternado,      // Padrão A-B-A-B
    CicloLongo,     // Padrão repetitivo longo
    Tendencial,     // Intervalos com tendência
    MultiModal,     // Múltiplos picos de frequência
    Irregular       // Sem padrão claro
}

public enum ConfiancaPrevisao
{
    MuitoBaixa,
    Baixa,
    Media,
    Alta
}

public class PrevisaoConcurso
{
    public int Concurso { get; set; }
    public double Probabilidade { get; set; }
    public int IntervaloEsperado { get; set; }
    public ConfiancaPrevisao Confianca { get; set; }

    public override string ToString()
    {
        return $"Concurso {Concurso}: {Probabilidade:P1}";
    }
}
#endregion
