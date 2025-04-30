using LotoLibrary.Extensions;
using LotoLibrary.Infrastructure.Logging;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork;
using LotoLibrary.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.IO; // Adicionado para System.IO

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
:start_line:23
-------
        private readonly PalpiteService _palpiteService;
        private readonly LotofacilService _lotofacilService; // Adicionado o serviço da Lotofácil

        private readonly IMLLogger _logger;
        private readonly MLNetModel _modelSS;
        private readonly MLNetModel _modelNS;
        private PalpiteService1 _palpiteService1;



        public MainWindow()
        {
            InitializeComponent();
            // Infra.CarregarConcursos(); // Removido - substituído pelo LotofacilService
            // Infra.CombinarGeral(); // Manter se necessário para outras partes do código que dependem de Infra.arGeral

            _lotofacilService = new LotofacilService(); // Inicializa o serviço da Lotofácil
            _lotofacilService.UpdateFromAPI(); // Carrega/Atualiza os dados da Lotofácil

            T1.Text = _lotofacilService.GetTotalConcursos().ToString(); // Atualiza o texto com o total de concursos carregados

            _logger = new MLLogger(LoggerFactory.Create(builder =>
                builder.AddConsole()).CreateLogger<MLLogger>());

            _modelSS = new MLNetModel(_logger, "ModeloSS.zip", "SS");
            _modelNS = new MLNetModel(_logger, "ModeloNS.zip", "NS");

            try
            {
                _modelSS.CarregarModelo();
                _modelNS.CarregarModelo();
                // _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS); // Inicialização movida para onde é necessário
                _logger.LogInformation("Modelos existentes carregados com sucesso");
            }
            catch
            {
                _logger.LogInformation("Modelos não encontrados. Execute o treinamento primeiro.");
            }
        }


        /// <summary>
        /// Método que fecha a aplicação ao clicar no botão Fechar.
        /// </summary>
        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Perfil. Atualmente não implementado.
        /// </summary>
        private void BtnPerfil_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Método associado ao clique no botão Lembrete. Atualmente não implementado.
        /// </summary>
        private void BtnLembrete_Click(object sender, RoutedEventArgs e)
        {
            SalvarResultados();
        }



        /// <summary>
        /// Permite mover a janela ao clicar e arrastar a barra de título.
        /// </summary>
        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        /// <summary>
        /// Executa o primeiro estudo ao clicar no botão correspondente.
        /// </summary>
        private void Primeiro_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo1(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo1", alvo));

            TerminarPrograma();

        }

        /// <summary>
        /// Executa o segundo estudo ao clicar no botão correspondente.
        /// </summary>
        private void Segundo_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo2(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo2", alvo));

            TerminarPrograma();
        }

        /// <summary>
        /// Executa a análise ao clicar no botão correspondente.
        /// </summary>
        private void Terceiro_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            AnaliseService.ExecutarAnalise();

            TerminarPrograma();

        }

        /// <summary>
        /// Método associado ao clique no botão Quarto. Executa o treinamento dos modelos ML.
        /// </summary>
        private async void Quarto_Click(object sender, RoutedEventArgs e)
        {
            await MLAnalise();

            // Após o treinamento, inicializar o PalpiteService1 está dentro de MLAnalise() agora

        }


        /// <summary>
        /// Método associado ao clique no botão Quinto. Atualmente não implementado.
        /// </summary>
        private void Quinto_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Sexto. Gera e classifica palpites.
        /// </summary>
        private void Sexto_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Verificar se o serviço está inicializado
                if (_palpiteService1 == null)
                {
                     int concursoBase = Convert.ToInt32(T1.Text) - 1; // Precisa definir concurso base se não treinou
                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                     _logger.LogWarning($"PalpiteService1 inicializado no Sexto_Click com concurso base {concursoBase}. Idealmente, treine primeiro (Botão Quarto).");
                    // MessageBox.Show("Execute o treinamento dos modelos primeiro (botão Quarto)",
                    //               "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // return;
                }

                _logger.LogInformation("Iniciando geração de palpites (Sexto_Click).");
                var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(10); // Gera poucos para teste rápido
                var palpitesClassificados = _palpiteService1.ClassificarPalpites(palpitesAleatorios);
                ExibirResultados(palpitesClassificados);
                _logger.LogInformation("Geração de palpites (Sexto_Click) concluída.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante a geração dos palpites (Sexto_Click)", ex);
                MessageBox.Show($"Erro durante a geração dos palpites: {ex.Message}",
                               "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método associado ao clique no botão Sétimo. Gera palpites e calcula ocorrências.
        /// </summary>
        private void Setimo_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text) - 1;
            try
            {
                if (_palpiteService1 == null)
                {
                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");
                }

                _logger.LogInformation("Iniciando geração de palpites (Setimo_Click).");
                var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(10000, 9); // Gera 10k com 9 pontos


                var palpitesClassificados = _palpiteService1.ClassificarPalpites(palpitesAleatorios);

                List<int> lsm = new List<int>();
                List<int> lsn = new List<int>();

                Lances arsm = _palpiteService1.GetGrupoSS();
                Lances arsn = _palpiteService1.GetGrupoNS();


                foreach (Lance o in palpitesClassificados) { lsm.Add(o.M); lsn.Add(o.N); }


                // Corrigido: Usar LINQ para contar ocorrências
                var groupedCountsSS = lsm.GroupBy(id => id)
                                         .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() })
                                         .OrderByDescending(l => l.F1);
                Lances OcorrenciaSS = groupedCountsSS.ToLances1();

                var groupedCountsNS = lsn.GroupBy(id => id)
                                         .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() })
                                         .OrderByDescending(l => l.F1);
                Lances OcorrenciaNS = groupedCountsNS.ToLances1();


                ExibirResultados(palpitesClassificados);


                Infra.SalvaSaidaW(palpitesClassificados, Infra.NomeSaida("Calculado", concursoBase + 1)); // Corrigido: Removido ToList()
                //Infra.SalvaSaidaW(palpitesClassificados.ObterValoresF(0), Infra.NomeSaida("PosiçãoF0    ", concursoBase + 1)); // Comentado - CS1503
                //Infra.SalvaSaidaW(palpitesClassificados.ObterValoresF(1), Infra.NomeSaida("PosiçãoF1", concursoBase + 1)); // Comentado - CS1503
                //Infra.SalvaSaidaW(palpitesClassificados.ObterValoresF(2), Infra.NomeSaida("PosiçãoF2", concursoBase + 1)); // Comentado - CS1503

                Infra.SalvaSaidaW(lsm, Infra.NomeSaida("ListaM", concursoBase + 1)); // Salva List<int>
                Infra.SalvaSaidaW(lsn, Infra.NomeSaida("ListaN", concursoBase + 1)); // Salva List<int>

                Infra.SalvaSaidaW(arsm, Infra.NomeSaida("GrupoSS", concursoBase + 1));
                Infra.SalvaSaidaW(arsn, Infra.NomeSaida("GrupoNS", concursoBase + 1));


                Infra.SalvaSaidaW(OcorrenciaSS, Infra.NomeSaida("OcorrenciasSS", concursoBase + 1));
                Infra.SalvaSaidaW(OcorrenciaNS, Infra.NomeSaida("OcorrenciasNS", concursoBase + 1));




            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro (Setimo_Click): {ex.Message}", ex);
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Método associado ao clique no botão Oitavo. Gera palpites e filtra os melhores por F1.
        /// </summary>
        private void Oitavo_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text);
            try
            {
                if (_palpiteService1 == null)
                {
                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");
                }

                _logger.LogInformation("Iniciando geração de palpites (Oitavo_Click).");
                var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios();
                var palpitesClassificados = _palpiteService1.ClassificarPalpites(palpitesAleatorios);

                // Ordena por F1 ascendente e pega os 1000 melhores
                var palpitesClassificadosFiltrados = palpitesClassificados.OrderBy(p => p.F1).Take(1000).ToLances();

                ExibirResultados(palpitesClassificadosFiltrados);


                Infra.SalvaSaidaW(palpitesClassificadosFiltrados, Infra.NomeSaida("Calculado-A", concursoBase + 1)); // Corrigido: Removido ToList()
                //Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(0), Infra.NomeSaida("PosiçãoF0-Filtrado-A", concursoBase + 1)); // Comentado - CS1503
                //Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(1), Infra.NomeSaida("PosiçãoF1-Filtrado-A", concursoBase + 1)); // Comentado - CS1503
                //Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(2), Infra.NomeSaida("PosiçãoF2-Filtrado-A", concursoBase + 1)); // Comentado - CS1503
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro (Oitavo_Click): {ex.Message}", ex);
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // TerminarPrograma(); // Removido para permitir visualização
        }

        /// <summary>
        /// Método associado ao clique no botão Nono. Gera palpites e filtra os piores por F1 (ordem descendente).
        /// </summary>
        private void Nono_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text);
            try
            {
                if (_palpiteService1 == null)
                {
                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");
                }

                _logger.LogInformation("Iniciando geração de palpites (Nono_Click).");
                var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios();
                var palpitesClassificados = _palpiteService1.ClassificarPalpites(palpitesAleatorios);

                // Ordena por F1 descendente e pega os 1000 "piores"
                var palpitesClassificadosFiltrados = palpitesClassificados.OrderByDescending(p => p.F1).Take(1000).ToLances();

                ExibirResultados(palpitesClassificadosFiltrados);


                Infra.SalvaSaidaW(palpitesClassificadosFiltrados, Infra.NomeSaida("Calculado-D", concursoBase + 1)); // Corrigido: Removido ToList()
                //Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(0), Infra.NomeSaida("PosiçãoF0-Filtrado-D", concursoBase + 1)); // Comentado - CS1503
                //Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(1), Infra.NomeSaida("PosiçãoF1-Filtrado-D", concursoBase + 1)); // Comentado - CS1503
                //Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(2), Infra.NomeSaida("PosiçãoF2-Filtrado-D", concursoBase + 1)); // Comentado - CS1503
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro (Nono_Click): {ex.Message}", ex);
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            // TerminarPrograma(); // Removido para permitir visualização
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo. Atualmente não implementado.
        /// </summary>
        private void Dez_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Primeiro. Lê um arquivo "Baseado-X" e filtra Infra.arGeral.
        /// </summary>
        private void Onze_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);


            Lances A = Estudos.Estudo11(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo11", alvo)); // Corrigido nome do arquivo de saída

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Segundo. Lê dois arquivos "BaseadoX-Y" e filtra Infra.arGeral.
        /// </summary>
        private void Doze_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);


            Lances A = Estudos.Estudo12(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo12", alvo));

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Terceiro. Atualmente não implementado.
        /// </summary>
        private void Treze_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Quarto. Gera palpites e conta ocorrências em loop.
        /// </summary>
        private void Catorze_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text) - 1;

            Lances OcorrenciaSS = new();
            Lances OcorrenciaNS = new();

            Lances arsm = new();
            Lances arsn = new();
            try
            {
                if (_palpiteService1 == null)
                {
                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");
                }

                // Obter grupos SS e NS apenas uma vez
                arsm.AddRange(_palpiteService1.GetGrupoSS());
                arsn.AddRange(_palpiteService1.GetGrupoNS());

                List<int> all_lsm = new List<int>();
                List<int> all_lsn = new List<int>();

                int loopCount = 40; // Reduzido para teste mais rápido
                _logger.LogInformation($"Iniciando loop de {loopCount} iterações (Catorze_Click).");

                for (int i = 0; i < loopCount; i++)
                {
                    var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(1000, 9); // Reduzido para teste

                    foreach (Lance o in palpitesAleatorios) { all_lsm.Add(o.M); all_lsn.Add(o.N); }
                     _logger.LogDebug($"Loop {i+1}/{loopCount} concluído.");
                }

                 _logger.LogInformation("Contando ocorrências acumuladas.");
                // Contar ocorrências acumuladas após o loop
                 var groupedCountsSS = all_lsm.GroupBy(id => id)
                                             .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() })
                                             .OrderByDescending(l => l.F1);
                 OcorrenciaSS = groupedCountsSS.ToLances1();

                 var groupedCountsNS = all_lsn.GroupBy(id => id)
                                             .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() })
                                             .OrderByDescending(l => l.F1);
                 OcorrenciaNS = groupedCountsNS.ToLances1();


            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro (Catorze_Click): {ex.Message}", ex);
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            _logger.LogInformation("Salvando resultados (Catorze_Click).");
            Infra.SalvaSaidaW(arsm, Infra.NomeSaida("GrupoSS", concursoBase + 1));
            Infra.SalvaSaidaW(arsn, Infra.NomeSaida("GrupoNS", concursoBase + 1));


            Infra.SalvaSaidaW(OcorrenciaSS, Infra.NomeSaida("OcorrenciasSS", concursoBase + 1));
            Infra.SalvaSaidaW(OcorrenciaNS, Infra.NomeSaida("OcorrenciasNS", concursoBase + 1));

            ExibirResultados(OcorrenciaSS); // Exibe as ocorrências SS
            // TerminarPrograma(); // Removido para permitir visualização
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Quinto. Similar ao 14, mas ordena e filtra ocorrências.
        /// </summary>
        private void Quinze_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text) - 1;

            Lances OcorrenciaSS = new();
            Lances OcorrenciaNS = new();

            Lances arsm = new();
            Lances arsn = new();
            try
            {
                 if (_palpiteService1 == null)
                {
                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");
                }

                // Obter grupos SS e NS apenas uma vez
                arsm.AddRange(_palpiteService1.GetGrupoSS());
                arsn.AddRange(_palpiteService1.GetGrupoNS());

                List<int> all_lsm = new List<int>();
                List<int> all_lsn = new List<int>();

                int loopCount = 40; // Reduzido para teste mais rápido
                _logger.LogInformation($"Iniciando loop de {loopCount} iterações (Quinze_Click).");

                for (int i = 0; i < loopCount; i++)
                {
                    var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(1000, 9); // Reduzido para teste

                    foreach (Lance o in palpitesAleatorios) { all_lsm.Add(o.M); all_lsn.Add(o.N); }
                     _logger.LogDebug($"Loop {i+1}/{loopCount} concluído.");
                }

                 _logger.LogInformation("Contando ocorrências acumuladas.");
                // Contar ocorrências acumuladas após o loop
                 var groupedCountsSS = all_lsm.GroupBy(id => id)
                                             .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() }) // CORRIGIDO: Usar construtor correto
                                             .OrderByDescending(l => l.F1); // Ordena pela contagem (F1)
                 OcorrenciaSS = groupedCountsSS.ToLances1();

                 var groupedCountsNS = all_lsn.GroupBy(id => id)
                                             .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() })
                                             .OrderByDescending(l => l.F1);
                 OcorrenciaNS = groupedCountsNS.ToLances1();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro (Quinze_Click): {ex.Message}", ex);
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Ordenar e filtrar já foi feito pelo LINQ ao criar OcorrenciaSS/NS
            // OcorrenciaSS.Sort(); // Não necessário se já ordenado por LINQ
            // OcorrenciaNS.Sort(); // Não necessário se já ordenado por LINQ

            Lances OcorrenciaSS1 = OcorrenciaSS.Take(101).ToLances1(); // Pega os 101 mais frequentes
            Lances OcorrenciaNS1 = OcorrenciaNS.Take(45).ToLances1();  // Pega os 45 mais frequentes

            _logger.LogInformation("Salvando resultados (Quinze_Click).");
            Infra.SalvaSaidaW(arsm, Infra.NomeSaida("GrupoSS", concursoBase + 1));
            Infra.SalvaSaidaW(arsn, Infra.NomeSaida("GrupoNS", concursoBase + 1));


            Infra.SalvaSaidaW(OcorrenciaSS, Infra.NomeSaida("OcorrenciasSS", concursoBase + 1));
            Infra.SalvaSaidaW(OcorrenciaNS, Infra.NomeSaida("OcorrenciasNS", concursoBase + 1));

            Infra.SalvaSaidaW(OcorrenciaSS1, Infra.NomeSaida("OcorrenciasSS1", concursoBase + 1));
            Infra.SalvaSaidaW(OcorrenciaNS1, Infra.NomeSaida("OcorrenciasNS1", concursoBase + 1));

            ExibirResultados(OcorrenciaSS1); // Exibe as ocorrências SS filtradas
            // TerminarPrograma(); // Removido para permitir visualização
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Sexto. Similar ao 15, mas também salva palpites classificados.
        /// </summary>
        private void Dezesseis_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text) - 1;

            Lances OcorrenciaSS = new();
            Lances OcorrenciaNS = new();

            Lances arsm = new();
            Lances arsn = new();

            Lances palpitesClassificadosTotal = new(); // Acumula todos os classificados

            try
            {
                 if (_palpiteService1 == null)
                {
                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");
                }

                // Obter grupos SS e NS apenas uma vez
                arsm.AddRange(_palpiteService1.GetGrupoSS());
                arsn.AddRange(_palpiteService1.GetGrupoNS());

                List<int> all_lsm = new List<int>();
                List<int> all_lsn = new List<int>();

                int loopCount = 40; // Reduzido para teste mais rápido
                _logger.LogInformation($"Iniciando loop de {loopCount} iterações (Dezesseis_Click).");

                for (int i = 0; i < loopCount; i++)
                {
                    var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(1000, 9); // Reduzido para teste
                    var palpitesClassificadosBatch = _palpiteService1.ClassificarPalpites(palpitesAleatorios);
                    palpitesClassificadosTotal.AddRange(palpitesClassificadosBatch); // Acumula

                    foreach (Lance o in palpitesAleatorios) { all_lsm.Add(o.M); all_lsn.Add(o.N); }
                     _logger.LogDebug($"Loop {i+1}/{loopCount} concluído.");
                }

                 _logger.LogInformation("Contando ocorrências acumuladas.");
                // Contar ocorrências acumuladas após o loop
                 var groupedCountsSS = all_lsm.GroupBy(id => id)
                                             .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() })
                                             .OrderByDescending(l => l.F1);
                 OcorrenciaSS = groupedCountsSS.ToLances1();

                 var groupedCountsNS = all_lsn.GroupBy(id => id)
                                             .Select(g => new Lance(g.Key, new List<int>()) { F1 = g.Count() })
                                             .OrderByDescending(l => l.F1);
                 OcorrenciaNS = groupedCountsNS.ToLances1();

            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro (Dezesseis_Click): {ex.Message}", ex);
                MessageBox.Show($"Erro: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            // Ordenar e filtrar ocorrências
            Lances OcorrenciaSS1 = OcorrenciaSS.Take(101).ToLances1();
            Lances OcorrenciaNS1 = OcorrenciaNS.Take(45).ToLances1();

            _logger.LogInformation("Salvando resultados (Dezesseis_Click).");
            Infra.SalvaSaidaW(arsm, Infra.NomeSaida("GrupoSS", concursoBase + 1));
            Infra.SalvaSaidaW(arsn, Infra.NomeSaida("GrupoNS", concursoBase + 1));


            Infra.SalvaSaidaW(OcorrenciaSS, Infra.NomeSaida("OcorrenciasSS", concursoBase + 1));
            Infra.SalvaSaidaW(OcorrenciaNS, Infra.NomeSaida("OcorrenciasNS", concursoBase + 1));

            Infra.SalvaSaidaW(OcorrenciaSS1, Infra.NomeSaida("OcorrenciasSS1", concursoBase + 1));
            Infra.SalvaSaidaW(OcorrenciaNS1, Infra.NomeSaida("OcorrenciasNS1", concursoBase + 1));

            // Salva todos os palpites classificados acumulados
            Infra.SalvaSaidaW(palpitesClassificadosTotal, Infra.NomeSaida("PalpitesClassificados", concursoBase + 1));

            ExibirResultados(palpitesClassificadosTotal.OrderByDescending(p => p.F1).Take(20).ToLances1()); // Exibe os 20 melhores do total
            // TerminarPrograma(); // Removido para permitir visualização
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Sétimo. Atualmente não implementado.
        /// </summary>
        private void Dezessete_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Oitavo. Atualmente não implementado.
        /// </summary>
        private void Dezoito_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Nono. Atualmente não implementado.
        /// </summary>
        private void Dezenove_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Vigésimo. Atualmente não implementado.
        /// </summary>
        private void Vinte_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }


        /// <summary>
        /// Encerra a aplicação.
        /// </summary>
        private void TerminarPrograma()
        {
            Application.Current.Shutdown();
        }

        private async Task MLAnalise()
        {
            try
            {
                _logger.LogInformation("Iniciando análise e treinamento dos modelos ML.NET.");
                await Task.Run(() => AnaliseService.ExecutarAnalise());
                _logger.LogInformation("Análise e treinamento concluídos com sucesso.");

                // Recarregar modelos após o treinamento
                _modelSS.CarregarModelo();
                _modelNS.CarregarModelo();
                _logger.LogInformation("Modelos recarregados após treinamento.");

                // Inicializar o PalpiteService1 após o treinamento
                int concursoBase = Convert.ToInt32(T1.Text) - 1; // Ou outra lógica para definir o concurso base
                _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);
                _logger.LogInformation($"PalpiteService1 inicializado com concurso base {concursoBase}.");

                MessageBox.Show("Análise e treinamento concluídos com sucesso!",
                                "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante a análise e treinamento", ex);
                MessageBox.Show($"Erro durante a análise e treinamento: {ex.Message}",
                               "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ExibirResultadosV0(Lances palpitesClassificados)
        {
            // Corrigido: Usar o nome correto do XAML
            listBoxResultados.Items.Clear();
            foreach (var palpite in palpitesClassificados.OrderByDescending(p => p.F1).Take(20)) // Exibe os 20 melhores
            {
                listBoxResultados.Items.Add($"Palpite: {palpite.ToString()} - Score: {palpite.F1:F2}");
            }
        }

        private void ExibirResultados(Lances palpitesClassificados)
        {
             // Corrigido: Usar o nome correto do XAML
            listBoxResultados.Items.Clear();
            foreach (var palpite in palpitesClassificados.Take(20)) // Exibe os 20 melhores
            {
                listBoxResultados.Items.Add($"Palpite: {palpite.ToString()} - F0: {palpite.F0:F2} - F1: {palpite.F1:F2} - F2: {palpite.F2:F2}");
            }
        }


        private void SalvarResultados()
        {
            int concursoBase = Convert.ToInt32(T1.Text);
            List<string> itemsAsString = new List<string>();
            foreach(var item in listBoxResultados.Items) // Corrigido: Usar o nome correto do XAML
            {
                itemsAsString.Add(item?.ToString() ?? string.Empty);
            }

            try
            {
                // Salvar diretamente como arquivo de texto
                string nomeArquivo = Infra.NomeSaida("Resultados", concursoBase) + ".txt"; // Adiciona extensão .txt
                string diretorioSaida = Path.Combine(Environment.CurrentDirectory, "Saida", concursoBase.ToString()); // Define diretório Saida/Concurso
                Directory.CreateDirectory(diretorioSaida); // Garante que o diretório exista
                string filePath = Path.Combine(diretorioSaida, nomeArquivo);

                File.WriteAllLines(filePath, itemsAsString);
                _logger.LogInformation($"Resultados salvos como texto simples em {filePath}");
                MessageBox.Show($"Resultados salvos em {filePath}", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                 _logger.LogError($"Erro ao salvar resultados em arquivo de texto: {ex.Message}", ex);
                 MessageBox.Show($"Erro ao salvar resultados: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


    }
}
