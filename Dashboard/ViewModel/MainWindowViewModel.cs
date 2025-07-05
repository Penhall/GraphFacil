using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dashboard.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly OscillatorEngine _engine;
        private readonly Lances _historico;

        [ObservableProperty]
        private ObservableCollection<DezenaOscilante> _dezenasOscilantes;

        [ObservableProperty]
        private bool _mostrarOsciladores;

        [ObservableProperty]
        private string _textoConcurso = "2020";

        // Comandos
        public ICommand PrimeiroCommand { get; }
        public ICommand SegundoCommand { get; }
        public ICommand TerceiroCommand { get; }
        public ICommand QuartoCommand { get; }
        public ICommand QuintoCommand { get; }
        public ICommand SextoCommand { get; }
        public ICommand SetimoCommand { get; }
        public ICommand OitavoCommand { get; }
        public ICommand IniciarSincronizacaoCommand { get; }
        public ICommand SalvarResultadosCommand { get; }
        public ICommand TerminarProgramaCommand { get; }

        public MainWindowViewModel(Lances historico)
        {
            _historico = historico;
            _engine = new OscillatorEngine(historico);
            DezenasOscilantes = new ObservableCollection<DezenaOscilante>(_engine.InicializarOsciladores());

            // Inicialização dos comandos
            PrimeiroCommand = new RelayCommand(Primeiro);
            SegundoCommand = new RelayCommand(Segundo);
            TerceiroCommand = new RelayCommand(Terceiro);
            QuartoCommand = new AsyncRelayCommand(Quarto);
            QuintoCommand = new RelayCommand(Quinto);
            SextoCommand = new RelayCommand(Sexto);
            SetimoCommand = new RelayCommand(Setimo);
            OitavoCommand = new RelayCommand(Oitavo);
            IniciarSincronizacaoCommand = new AsyncRelayCommand(ProcessarOsciladoresAvancado);
            SalvarResultadosCommand = new RelayCommand(SalvarResultados);
            TerminarProgramaCommand = new RelayCommand(TerminarPrograma);
        }

        #region Study Commands
        private void Primeiro()
        {
            ExecuteStudy(Estudos.Estudo1, "ListaEstudo1");
        }

        private void Segundo()
        {
            ExecuteStudy(Estudos.Estudo2, "ListaEstudo2");
        }

        private void Terceiro()
        {
            int alvo = Convert.ToInt32(TextoConcurso);
            AnaliseService.ExecutarAnalise();
            TerminarPrograma();
        }

        private async Task Quarto()
        {
            await ExecuteStudyAsync(Estudos.Estudo4, "ListaEstudo4");
        }

        private void Quinto()
        {
            ExecuteStudy(Estudos.Estudo5, "ListaEstudo5");
        }

        private void Sexto()
        {
            ExecuteStudy(Estudos.Estudo6, "ListaEstudo6");
        }

        private void Setimo()
        {
            ExecuteStudy(Estudos.Estudo7, "ListaEstudo7");
        }

        private void Oitavo()
        {
            int concursoBase = Convert.ToInt32(TextoConcurso);
            // Implementação específica do estudo 8
        }
        #endregion

        #region Utility Methods
        private void ExecuteStudy(Func<int, Lances> studyMethod, string studyName)
        {
            int alvo = Convert.ToInt32(TextoConcurso);
            Lances result = studyMethod(alvo);
            Infra.SalvaSaidaW(result, Infra.NomeSaida(studyName, alvo));
            TerminarPrograma();
        }

        private async Task ExecuteStudyAsync(Func<int, Lances> studyMethod, string studyName)
        {
            int alvo = Convert.ToInt32(TextoConcurso);
            Lances result = await Task.Run(() => studyMethod(alvo));
            Infra.SalvaSaidaW(result, Infra.NomeSaida(studyName, alvo));
            TerminarPrograma();
        }
        #endregion

        #region File Operations
        private void SalvarResultados()
        {
            int concursoBase = Convert.ToInt32(TextoConcurso);
            try
            {
                string nomeArquivo = Infra.NomeSaida("Resultados", concursoBase);
                string diretorioSaida = Path.Combine(Environment.CurrentDirectory, "Saida", concursoBase.ToString());
                Directory.CreateDirectory(diretorioSaida);

                var itemsAsString = Infra.arLoto.Select(o => o.Saida).ToList();
                File.WriteAllLines(Path.Combine(diretorioSaida, nomeArquivo), itemsAsString);

                MessageBox.Show($"Resultados salvos em {diretorioSaida}", "Sucesso",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao salvar resultados: {ex.Message}", "Erro",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        #endregion

        #region Oscillator Logic
        private async Task ProcessarOsciladoresAvancado()
        {
            MostrarOsciladores = true;

            // 1. Apply strategies
            OscillatorStrategy.AplicarTendenciaCurtoPrazo(
                DezenasOscilantes.ToList(),
                _engine.DadosTreino.ToList());

            // 2. Real-time processing with visualization
            for (int i = 0; i < 30; i++)
            {
                AtualizarFases();
                await Task.Delay(150); // Visualization delay
            }

            // 3. Generate and validate guess
            var palpite = OscillatorStrategy.GerarPalpiteValidacao(
                DezenasOscilantes.ToList(),
                _engine.DadosValidacao.ToList());

            MessageBox.Show($"Palpite Gerado: {string.Join(", ", palpite)}");
        }

        private void AtualizarFases()
        {
            foreach (var dezena in DezenasOscilantes)
            {
                double influenciaTotal = CalculateInfluence(dezena);
                UpdatePhase(dezena, influenciaTotal);
                UpdateSyncStatus(dezena, influenciaTotal);
            }
        }

        private double CalculateInfluence(DezenaOscilante dezena)
        {
            return DezenasOscilantes
                .Where(d => d.Numero != dezena.Numero)
                .Sum(d => Math.Sin((d.Fase - dezena.Fase) * Math.PI / 180) * d.ForcaSincronizacao);
        }

        private void UpdatePhase(DezenaOscilante dezena, double influence)
        {
            dezena.Fase += 0.1 * (dezena.Frequencia + influence);
            dezena.Fase = (dezena.Fase % 360 + 360) % 360; // Normalize to 0-360
        }

        private void UpdateSyncStatus(DezenaOscilante dezena, double influence)
        {
            dezena.EstaSincronizada = influence > 0.5;
        }
        #endregion

        #region Application Control
        private void TerminarPrograma()
        {
            Application.Current.Shutdown();
        }
        #endregion
    }
}