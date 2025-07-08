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
    public partial class MainWindowViewModel : ObservableObject
    {
       private readonly  MetronomoEngine _metronomoEngine;
        private readonly Lances _historico;

        #region Observable Properties
        [ObservableProperty]
        private ObservableCollection<DezenaOscilante> _dezenasOscilantes;

        [ObservableProperty]
        private bool _mostrarOsciladores = true;

        [ObservableProperty]
        private string _textoConcurso = string.Empty;

        [ObservableProperty]
        private string _ultimoPalpite = string.Empty;

        [ObservableProperty]
        private bool _isProcessing = false;
        #endregion


        public MainWindowViewModel()
        {
            try
            {
                // Inicialização dos dados
                Infra.CarregarConcursos();
                _historico = Infra.arLoto;
                _engine = new OscillatorEngine(_historico);

                // Inicialização dos osciladores
                DezenasOscilantes = new ObservableCollection<DezenaOscilante>(_engine.InicializarOsciladores());

                // Atualizar texto do concurso atual
                AtualizarTextoConcurso();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na inicialização: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region Oscillator Processing
        [RelayCommand]
        private async Task ProcessarOsciladoresAvancado()
        {
            if (IsProcessing) return;

            try
            {
                IsProcessing = true;

                // 1. Aplicar estratégias baseadas em dados históricos
                OscillatorStrategy.AplicarTendenciaCurtoPrazo(
                DezenasOscilantes.ToList(),
                _engine.DadosTreino.ToList());

                // 2. Processamento em tempo real com visualização
                for (int i = 0; i < 30; i++)
                {
                    AtualizarFases();
                    await Task.Delay(150); // Delay para visualização
                }

                // 3. Gerar palpite inteligente baseado na sincronização
                var palpite = GerarPalpiteInteligente();
                UltimoPalpite = $"Palpite: {string.Join(", ", palpite.OrderBy(x => x))}";

                MessageBox.Show(UltimoPalpite, "Palpite Gerado", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no processamento: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private async Task IniciarSincronizacao()
        {
            if (IsProcessing) return;

            try
            {
                IsProcessing = true;

                // Reinicializar osciladores
                DezenasOscilantes.Clear();
                var novosOsciladores = _engine.InicializarOsciladores();
                foreach (var osc in novosOsciladores)
                {
                    DezenasOscilantes.Add(osc);
                }

                // Processo de sincronização gradual
                for (int ciclo = 0; ciclo < 50; ciclo++)
                {
                    AtualizarFases();

                    // Aplica força de sincronização
                    AplicarForcaSincronizacao();

                    await Task.Delay(100);
                }

                UltimoPalpite = $"Sincronização concluída - Palpite: {string.Join(", ", GerarPalpiteInteligente().OrderBy(x => x))}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na sincronização: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private void AtualizarFases()
        {
            foreach (var dezena in DezenasOscilantes)
            {
                double influenciaTotal = CalcularInfluencia(dezena);
                AtualizarFase(dezena, influenciaTotal);
                AtualizarStatusSincronizacao(dezena, influenciaTotal);
            }
        }

        private double CalcularInfluencia(DezenaOscilante dezena)
        {
            return DezenasOscilantes
          .Where(d => d.Numero != dezena.Numero)
          .Sum(d => Math.Sin((d.Fase - dezena.Fase) * Math.PI / 180) * d.ForcaSincronizacao * 0.1);
        }

        private void AtualizarFase(DezenaOscilante dezena, double influencia)
        {
            dezena.Fase += dezena.Frequencia + influencia;
            dezena.Fase = (dezena.Fase % 360 + 360) % 360; // Normalizar para 0-360
        }

        private void AtualizarStatusSincronizacao(DezenaOscilante dezena, double influencia)
        {
            // Atualizar probabilidade baseada na sincronização
            dezena.Probabilidade = Math.Sin(dezena.Fase * Math.PI / 180) * dezena.ForcaSincronizacao +
            Math.Abs(influencia) * 0.5;
            dezena.EstaSincronizada = Math.Abs(influencia) > 0.3;
        }

        private void AplicarForcaSincronizacao()
        {
            // Identifica grupos com fases similares
            var gruposSincronizados = DezenasOscilantes
.GroupBy(d => (int)(d.Fase / 30)) // Agrupa por faixas de 30 graus
.Where(g => g.Count() > 2)
                        .ToList();

            foreach (var grupo in gruposSincronizados)
            {
                var mediaFase = grupo.Average(d => d.Fase);
                foreach (var dezena in grupo)
                {
                    // Puxa a fase em direção à média do grupo
                    dezena.Fase += (mediaFase - dezena.Fase) * 0.1;
                    dezena.ForcaSincronizacao = Math.Min(1.0, dezena.ForcaSincronizacao + 0.05);
                }
            }
        }

        private List<int> GerarPalpiteInteligente()
        {
            // Ordenar por uma combinação de fatores
            var candidatos = DezenasOscilantes
.Select(d => new
{
    Numero = d.Numero,
    Score = CalcularScore(d)
})
.OrderByDescending(x => x.Score)
                    .Take(15)
.Select(x => x.Numero)
                        .ToList();

            return candidatos;
        }

        private double CalcularScore(DezenaOscilante dezena)
        {
            // Score baseado em múltiplos fatores
            double scoreFase = Math.Sin(dezena.Fase * Math.PI / 180) + 1; // 0-2
            double scoreFrequencia = dezena.Frequencia * 0.5; // Peso da frequência
            double scoreSincronizacao = dezena.ForcaSincronizacao * 2; // Peso da sincronização
            double scoreProbabilidade = dezena.Probabilidade;

            // Penalidade para números muito atrasados ou muito frequentes
            double penalidade = Math.Max(0, dezena.UltimoAtraso - 15) * 0.1;

            return scoreFase + scoreFrequencia + scoreSincronizacao + scoreProbabilidade - penalidade;
        }

        [RelayCommand]
        private void ResetarOsciladores()
        {
            try
            {
                DezenasOscilantes.Clear();
                var novosOsciladores = _engine.InicializarOsciladores();
                foreach (var osc in novosOsciladores)
                {
                    DezenasOscilantes.Add(osc);
                }
                UltimoPalpite = "Osciladores resetados";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao resetar: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Study Commands
        [RelayCommand]
        private void Primeiro() => ExecutarEstudo(1);

        [RelayCommand]
        private void Segundo() => ExecutarEstudo(2);

        [RelayCommand]
        private void Terceiro() => ExecutarEstudo(3);

        [RelayCommand]
        private void Quarto() => ExecutarEstudo(4);

        [RelayCommand]
        private void Quinto() => ExecutarEstudo(5);

        [RelayCommand]
        private void Sexto() => ExecutarEstudo(6);

        [RelayCommand]
        private void Setimo() => ExecutarEstudo(7);

        [RelayCommand]
        private void Oitavo() => ExecutarEstudo(8);

        [RelayCommand]
        private void TerminarPrograma()
        {
            Application.Current.Shutdown();
        }

        [RelayCommand]
        private void AbrirValidacao()
        {
            try
            {
                var validationWindow = new Dashboard.Views.ValidationWindow();
                validationWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao abrir janela de validação: {ex.Message}",
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Study Execution
        private void ExecutarEstudo(int numeroEstudo)
        {
            try
            {
                Lances resultado = numeroEstudo switch
                {
                    1 => Estudos.Estudo1(Infra.arLoto.Count),
                    2 => new Lances(), // Implementar outros estudos conforme necessário
                    3 => new Lances(),
                    4 => new Lances(),
                    5 => new Lances(),
                    6 => new Lances(),
                    7 => new Lances(),
                    8 => new Lances(),
                    _ => new Lances()
                };

                MessageBox.Show($"Estudo {numeroEstudo} executado com {resultado.Count} resultados",
                "Estudo Concluído", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro no estudo {numeroEstudo}: {ex.Message}",
                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Utility Methods
        private void AtualizarTextoConcurso()
        {
            if (_historico?.Count > 0)
            {
                var ultimoConcurso = _historico.Last();
                TextoConcurso = $"Próximo Concurso: {ultimoConcurso.Id + 1}";
            }
            else
            {
                TextoConcurso = "Concurso: N/A";
            }
        }
        #endregion
    }
}