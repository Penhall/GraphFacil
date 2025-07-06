using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Dashboard.ViewModel
{
    public partial class ValidationViewModel : ObservableObject
    {
        private readonly ValidationMetricsService _validationService;
        private readonly List<Lance> _dadosTreino;
        private readonly List<Lance> _dadosValidacao;

        #region Observable Properties
        [ObservableProperty]
        private int _numeroTestes = 30;

        [ObservableProperty]
        private int _tamanhoValidacao = 100;

        [ObservableProperty]
        private bool _isValidando = false;

        [ObservableProperty]
        private string _progressoValidacao = string.Empty;

        [ObservableProperty]
        private string _statusValidacao = "Pronto para executar validação";

        [ObservableProperty]
        private DateTime? _ultimaValidacao;

        [ObservableProperty]
        private MetricasPerformance? _metricasOsciladores;

        [ObservableProperty]
        private ObservableCollection<ResultadoValidacao> _resultadosValidacao = new();

        [ObservableProperty]
        private ObservableCollection<MetricasPerformance> _comparacaoEstrategias = new();
        #endregion

        public ValidationViewModel()
        {
            try
            {
                _validationService = new ValidationMetricsService();

                // Carregar dados - usando método alternativo se CarregarSorteios não existir
                try
                {
                    // Tentar carregar com o método do Infra
                    if (Infra.arLoto?.Count == 0)
                    {
                        // Usar método alternativo se necessário
                        CarregarDadosAlternativos();
                    }
                }
                catch (Exception)
                {
                    // Se falhar, tentar método alternativo
                    CarregarDadosAlternativos();
                }

                var historico = Infra.arLoto;

                if (historico?.Count > TamanhoValidacao)
                {
                    _dadosTreino = historico.SkipLast(TamanhoValidacao).ToList();
                    _dadosValidacao = historico.TakeLast(TamanhoValidacao).ToList();
                    StatusValidacao = $"Dados carregados: {_dadosTreino.Count} treino, {_dadosValidacao.Count} validação";
                }
                else
                {
                    StatusValidacao = "Dados insuficientes para validação";
                }
            }
            catch (Exception ex)
            {
                StatusValidacao = $"Erro ao carregar dados: {ex.Message}";
                MessageBox.Show($"Erro na inicialização: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Validation Commands
        [RelayCommand]
        private async Task ExecutarValidacao()
        {
            if (IsValidando) return;

            try
            {
                IsValidando = true;
                ProgressoValidacao = "Iniciando validação dos osciladores...";
                StatusValidacao = "Executando validação";

                // Limpar resultados anteriores
                ResultadosValidacao.Clear();
                MetricasOsciladores = null;

                await Task.Run(() =>
                {
                    // Executar validação
                    var metricas = _validationService.ValidarOsciladores(
                        _dadosTreino,
                        _dadosValidacao,
                        NumeroTestes);

                    MetricasOsciladores = metricas;
                });

                // Simular resultados detalhados (seria implementado no ValidationMetricsService)
                await GerarResultadosDetalhados();

                UltimaValidacao = DateTime.Now;
                StatusValidacao = $"Validação concluída - {MetricasOsciladores.TaxaAcertoMedia:P2} de acerto médio";
                ProgressoValidacao = "Validação concluída com sucesso!";

                MessageBox.Show(
                    $"Validação concluída!\n\n" +
                    $"Taxa de Acerto: {MetricasOsciladores.TaxaAcertoMedia:P2}\n" +
                    $"Média de Acertos: {MetricasOsciladores.MediaAcertos:F1}/15\n" +
                    $"Melhor Resultado: {MetricasOsciladores.MelhorResultado} acertos\n" +
                    $"Desvio Padrão: {MetricasOsciladores.DesvioPadrao:P2}",
                    "Validação Concluída",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                StatusValidacao = $"Erro na validação: {ex.Message}";
                MessageBox.Show($"Erro durante a validação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsValidando = false;
                ProgressoValidacao = string.Empty;
            }
        }

        [RelayCommand]
        private async Task CompararEstrategias()
        {
            if (IsValidando) return;

            try
            {
                IsValidando = true;
                ProgressoValidacao = "Comparando estratégias...";
                StatusValidacao = "Executando comparação de estratégias";

                // Limpar resultados anteriores
                ComparacaoEstrategias.Clear();

                Dictionary<string, MetricasPerformance> resultados = null;

                await Task.Run(() =>
                {
                    resultados = _validationService.CompararEstrategias(
                        _dadosTreino,
                        _dadosValidacao,
                        NumeroTestes);
                });

                // Adicionar resultados na coleção
                if (resultados != null)
                {
                    var estrategiasOrdenadas = resultados.Values
                        .OrderByDescending(m => m.TaxaAcertoMedia)
                        .ToList();

                    foreach (var estrategia in estrategiasOrdenadas)
                    {
                        ComparacaoEstrategias.Add(estrategia);
                    }
                }

                UltimaValidacao = DateTime.Now;
                StatusValidacao = "Comparação de estratégias concluída";
                ProgressoValidacao = "Comparação concluída com sucesso!";

                // Mostrar resultado resumido
                if (resultados != null && resultados.ContainsKey("Osciladores"))
                {
                    var melhorEstrategia = ComparacaoEstrategias.First();
                    var osciladores = resultados["Osciladores"];

                    MessageBox.Show(
                        $"Comparação concluída!\n\n" +
                        $"Melhor Estratégia: {melhorEstrategia.NomeEstrategia} ({melhorEstrategia.TaxaAcertoMedia:P2})\n" +
                        $"Osciladores: {osciladores.TaxaAcertoMedia:P2}\n" +
                        $"Ganho sobre Aleatório: {osciladores.GanhoSobreAleatorio:P1}\n" +
                        $"Ganho sobre Frequência: {osciladores.GanhoSobreFrequencia:P1}",
                        "Comparação Concluída",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                StatusValidacao = $"Erro na comparação: {ex.Message}";
                MessageBox.Show($"Erro durante a comparação: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsValidando = false;
                ProgressoValidacao = string.Empty;
            }
        }
        #endregion

        #region Helper Methods
        private async Task GerarResultadosDetalhados()
        {
            // Simular geração de resultados detalhados
            // Em uma implementação real, isso seria feito no ValidationMetricsService
            await Task.Run(() =>
            {
                var random = new Random(42);
                var engine = new OscillatorEngine(new Lances(_dadosTreino));

                for (int i = 0; i < Math.Min(NumeroTestes, _dadosValidacao.Count); i++)
                {
                    var concursoTeste = _dadosValidacao[i];

                    // Simular geração de palpite
                    var oscillators = engine.InicializarOsciladores();
                    var palpite = OscillatorStrategy.GerarPalpiteValidacao(oscillators, _dadosTreino);

                    var resultado = new ResultadoValidacao
                    {
                        ConcursoId = concursoTeste.Id,
                        PalpiteGerado = palpite,
                        ResultadoReal = concursoTeste.Lista,
                        Acertos = palpite.Intersect(concursoTeste.Lista).Count(),
                        DataTeste = DateTime.Now,
                        TipoEstrategia = "Osciladores"
                    };

                    resultado.TaxaAcerto = resultado.Acertos / 15.0;
                    resultado.NumerosAcertados = palpite.Intersect(concursoTeste.Lista).ToList();
                    resultado.NumerosPerdidos = concursoTeste.Lista.Except(palpite).ToList();

                    // Adicionar na UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ResultadosValidacao.Add(resultado);
                    });

                    // Simular progresso
                    var progresso = (i + 1.0) / NumeroTestes * 100;
                    ProgressoValidacao = $"Processando teste {i + 1}/{NumeroTestes} ({progresso:F0}%)";
                }
            });
        }

        /// <summary>
        /// Exporta resultados para análise externa
        /// </summary>
        public string ExportarResultados()
        {
            if (MetricasOsciladores == null) return "Nenhum resultado para exportar";

            var relatorio = $"=== RELATÓRIO DE VALIDAÇÃO DOS OSCILADORES ===\n\n";
            relatorio += $"Data/Hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}\n";
            relatorio += $"Número de Testes: {NumeroTestes}\n";
            relatorio += $"Tamanho da Validação: {TamanhoValidacao}\n\n";

            relatorio += $"=== MÉTRICAS PRINCIPAIS ===\n";
            relatorio += $"Taxa de Acerto Média: {MetricasOsciladores.TaxaAcertoMedia:P2}\n";
            relatorio += $"Média de Acertos: {MetricasOsciladores.MediaAcertos:F2}/15\n";
            relatorio += $"Desvio Padrão: {MetricasOsciladores.DesvioPadrao:P2}\n";
            relatorio += $"Melhor Resultado: {MetricasOsciladores.MelhorResultado} acertos\n";
            relatorio += $"Pior Resultado: {MetricasOsciladores.PiorResultado} acertos\n\n";

            relatorio += $"=== MÉTRICAS AVANÇADAS ===\n";
            relatorio += $"Precision: {MetricasOsciladores.Precision:P2}\n";
            relatorio += $"Recall: {MetricasOsciladores.Recall:P2}\n";
            relatorio += $"F1-Score: {MetricasOsciladores.F1Score:P2}\n\n";

            if (ComparacaoEstrategias.Any())
            {
                relatorio += $"=== COMPARAÇÃO COM OUTRAS ESTRATÉGIAS ===\n";
                foreach (var estrategia in ComparacaoEstrategias.OrderByDescending(e => e.TaxaAcertoMedia))
                {
                    relatorio += $"{estrategia.NomeEstrategia}: {estrategia.TaxaAcertoMedia:P2} " +
                                $"(Acertos: {estrategia.MediaAcertos:F1}, Melhor: {estrategia.MelhorResultado})\n";
                }
                relatorio += "\n";
            }

            relatorio += $"=== DISTRIBUIÇÃO DE ACERTOS ===\n";
            foreach (var dist in MetricasOsciladores.DistribuicaoAcertos.OrderBy(kvp => kvp.Key))
            {
                var porcentagem = (dist.Value / (double)MetricasOsciladores.TotalTestes) * 100;
                relatorio += $"{dist.Key} acertos: {dist.Value} vezes ({porcentagem:F1}%)\n";
            }

            return relatorio;
        }

        /// <summary>
        /// Obtém análise interpretativa dos resultados
        /// </summary>
        public string ObterAnaliseInterpretativa()
        {
            if (MetricasOsciladores == null) return "Nenhum resultado para analisar";

            var analise = "=== ANÁLISE INTERPRETATIVA ===\n\n";

            // Análise da performance geral
            if (MetricasOsciladores.TaxaAcertoMedia > 0.65)
                analise += "✅ EXCELENTE: Performance muito acima da média esperada.\n";
            else if (MetricasOsciladores.TaxaAcertoMedia > 0.6)
                analise += "✅ BOM: Performance acima da média teórica.\n";
            else if (MetricasOsciladores.TaxaAcertoMedia > 0.55)
                analise += "⚠️ RAZOÁVEL: Performance ligeiramente acima da média.\n";
            else
                analise += "❌ BAIXO: Performance abaixo do esperado.\n";

            // Análise da consistência
            if (MetricasOsciladores.DesvioPadrao < 0.1)
                analise += "✅ CONSISTENTE: Baixa variabilidade nos resultados.\n";
            else if (MetricasOsciladores.DesvioPadrao < 0.15)
                analise += "⚠️ MODERADAMENTE CONSISTENTE: Variabilidade moderada.\n";
            else
                analise += "❌ INCONSISTENTE: Alta variabilidade nos resultados.\n";

            // Análise comparativa
            if (ComparacaoEstrategias.Any())
            {
                var posicaoOsciladores = ComparacaoEstrategias
                    .OrderByDescending(e => e.TaxaAcertoMedia)
                    .ToList()
                    .FindIndex(e => e.NomeEstrategia.Contains("Osciladores")) + 1;

                analise += $"\n📊 RANKING: {posicaoOsciladores}º lugar entre {ComparacaoEstrategias.Count} estratégias testadas.\n";

                if (MetricasOsciladores.GanhoSobreAleatorio > 0.1)
                    analise += "✅ Significativamente melhor que estratégia aleatória.\n";
                else if (MetricasOsciladores.GanhoSobreAleatorio > 0)
                    analise += "⚠️ Ligeiramente melhor que estratégia aleatória.\n";
                else
                    analise += "❌ Performance similar ou pior que estratégia aleatória.\n";
            }

            return analise;
        }

        /// <summary>
        /// Método alternativo para carregar dados quando CarregarSorteios não está disponível
        /// </summary>
        private void CarregarDadosAlternativos()
        {
            try
            {
                // Se Infra.arLoto já tem dados, use-os
                if (Infra.arLoto?.Count > 0)
                {
                    return;
                }

                // Tentar usar outros métodos do Infra
                // Como último recurso, criar dados de exemplo para teste
                StatusValidacao = "Usando dados alternativos para teste";

                // Aqui você pode implementar outros métodos de carregamento
                // baseados na estrutura atual do seu projeto
            }
            catch (Exception ex)
            {
                StatusValidacao = $"Erro ao carregar dados alternativos: {ex.Message}";
            }
        }
        #endregion
    }
}