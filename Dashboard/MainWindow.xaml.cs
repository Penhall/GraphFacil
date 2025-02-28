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

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly PalpiteService _palpiteService;

        private readonly IMLLogger _logger;
        private readonly MLNetModel _modelSS;
        private readonly MLNetModel _modelNS;
        private PalpiteService1 _palpiteService1;



        public MainWindow()
        {
            InitializeComponent();
            Infra.CarregarConcursos();
            Infra.CombinarGeral();

            T1.Text = Infra.arLoto.Count.ToString();

            _logger = new MLLogger(LoggerFactory.Create(builder =>
                builder.AddConsole()).CreateLogger<MLLogger>());

            _modelSS = new MLNetModel(_logger, "ModeloSS.zip", "SS");
            _modelNS = new MLNetModel(_logger, "ModeloNS.zip", "NS");

            try
            {
                _modelSS.CarregarModelo();
                _modelNS.CarregarModelo();
                // _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS);
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
        /// Método associado ao clique no botão Quarto. Atualmente não implementado.
        /// </summary>
        private async void Quarto_Click(object sender, RoutedEventArgs e)
        {
            await MLAnalise();

            // Após o treinamento, inicializar o PalpiteService1

        }


        /// <summary>
        /// Método associado ao clique no botão Quinto. Atualmente não implementado.
        /// </summary>
        private void Quinto_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Sexto. Atualmente não implementado.
        /// </summary>
        private void Sexto_Click(object sender, RoutedEventArgs e)
        {

            // _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS);
            try
            {
                // Verificar se o serviço está inicializado
                if (_palpiteService1 == null)
                {
                    MessageBox.Show("Execute o treinamento dos modelos primeiro (botão Quarto)",
                                  "Aviso", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                _logger.LogInformation("Iniciando geração de palpites.");
                var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(10);
                var palpitesClassificados = _palpiteService1.ClassificarPalpites(palpitesAleatorios);
                ExibirResultados(palpitesClassificados);
                _logger.LogInformation("Geração de palpites concluída.");
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante a geração dos palpites", ex);
                MessageBox.Show($"Erro durante a geração dos palpites: {ex.Message}",
                               "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Método associado ao clique no botão Sétimo. Atualmente não implementado.
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

                _logger.LogInformation("Iniciando geração de palpites.");
                var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(10000, 9);


                var palpitesClassificados = _palpiteService1.ClassificarPalpites(palpitesAleatorios);

                List<int> lsm = new List<int>();
                List<int> lsn = new List<int>();

                Lances arsm = _palpiteService1.GetGrupoSS();
                Lances arsn = _palpiteService1.GetGrupoNS();


                foreach (Lance o in palpitesClassificados) { lsm.Add(o.M); lsn.Add(o.N); }


                Lances OcorrenciaSS = Infra.ContaOcorrencia(lsm, arsm);
                Lances OcorrenciaNS = Infra.ContaOcorrencia(lsn, arsn);


                ExibirResultados(palpitesClassificados);


                Infra.SalvaSaidaW(palpitesClassificados.ToList(), Infra.NomeSaida("Calculado", concursoBase + 1));
                //Infra.SalvaSaidaW(palpitesClassificados.ObterValoresF(0), Infra.NomeSaida("PosiçãoF0    ", concursoBase + 1));
                //Infra.SalvaSaidaW(palpitesClassificados.ObterValoresF(1), Infra.NomeSaida("PosiçãoF1", concursoBase + 1));
                //Infra.SalvaSaidaW(palpitesClassificados.ObterValoresF(2), Infra.NomeSaida("PosiçãoF2", concursoBase + 1));

                Infra.SalvaSaidaW(lsm, Infra.NomeSaida("ListaM", concursoBase + 1));
                Infra.SalvaSaidaW(lsn, Infra.NomeSaida("ListaN", concursoBase + 1));

                Infra.SalvaSaidaW(arsm, Infra.NomeSaida("GrupoSS", concursoBase + 1));
                Infra.SalvaSaidaW(arsn, Infra.NomeSaida("GrupoNS", concursoBase + 1));


                Infra.SalvaSaidaW(OcorrenciaSS, Infra.NomeSaida("OcorrenciasSS", concursoBase + 1));
                Infra.SalvaSaidaW(OcorrenciaNS, Infra.NomeSaida("OcorrenciasNS", concursoBase + 1));




            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {ex.Message}");
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        /// <summary>
        /// Método associado ao clique no botão Oitavo. Atualmente não implementado.
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

                _logger.LogInformation("Iniciando geração de palpites.");
                var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios();
                var palpitesClassificados = _palpiteService1.ClassificarPalpites(palpitesAleatorios);

                palpitesClassificados.OrdenarPorF1().Take(1000);

                var palpitesClassificadosFiltrados = palpitesClassificados.OrdenarPorF1().Take(1000).ToLances(); ;

                //        var palpitesClassificadosFiltrados = palpitesClassificados.FiltrarPorValores(f0Filter: f0 => f0 == 41.9, f1Filter: f1 => f1 == 61.9, f2Filter: f2 => f2 == 61.9);

                ExibirResultados(palpitesClassificadosFiltrados);


                Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ToList(), Infra.NomeSaida("Calculado", concursoBase + 1));
                Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(0), Infra.NomeSaida("PosiçãoF0-Filtrado    ", concursoBase + 1));
                Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(1), Infra.NomeSaida("PosiçãoF1-Filtrado", concursoBase + 1));
                Infra.SalvaSaidaW(palpitesClassificadosFiltrados.ObterValoresF(2), Infra.NomeSaida("PosiçãoF2-Filtrado", concursoBase + 1));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {ex.Message}");
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Nono. Atualmente não implementado.
        /// </summary>
        private void Nono_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo. Atualmente não implementado.
        /// </summary>
        private void Dez_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();

            int concursoBase = Convert.ToInt32(T1.Text) + 1;
            string nomeArq = "Calculado-" + concursoBase.ToString();

            Lance oBase = Infra.arLoto[concursoBase - 2];

            Lance oBaseOposto = Infra.DevolveOposto(oBase);

            Lances arq = Infra.AbrirArquivo(nomeArq);

            Lances arq1 = new();

            foreach (Lance o in arq)
            {
                arq1.Add(Infra.DevolveOposto(o));
            }

            List<int> N = Enumerable.Range(1, 25).ToList();

            Lances ars1 = GerarCombinacoes.Combinar25a15(N);

            Lances ars2 = new Lances();

            Lance maiores = Infra.DevolveMaisFrequentes(arq, 15);

            List<int> PTbase = new();
            List<int> PTbaseSaida = new();

            foreach (Lance o in arq)
            {
                PTbase.Add(Infra.Contapontos(o, maiores));
            }

            for (int x = 0; x < arq.Count; x++)
            {
                Lance lance = arq[x];

                List<int> list = new List<int>();

                foreach (Lance o in arq)
                {
                    list.Add(Infra.Contapontos(lance, o));
                }
                int a = 0;
                for (int y = 0; y < list.Count; y++)
                {
                    if (list[y] == PTbase[y]) a++;
                }

                PTbaseSaida.Add(a);
            }


            PTbaseSaida.Sort();

            int oMin = 20;
            int oMax = 30;


            List<int> LancesMax = new List<int>();

            Lances ars3 = new();


            int m = 0;
            while ((ars3.Count < 1000))
            {
                Lance lance = ars1[random.Next(ars1.Count)];


                List<int> list = new List<int>();

                foreach (Lance o in arq)
                {
                    list.Add(Infra.Contapontos(lance, o));
                }
                int a = 0;
                for (int y = 0; y < list.Count; y++)
                {
                    if (list[y] == PTbase[y]) a++;
                }

                if ((a > oMin) && (a < oMax)) { ars3.Add(lance); LancesMax.Add(a); }
                m++;

            }





            //int a = 0;

            //foreach (Lance p in arq1)
            //{
            //    int b = Infra.Contapontos(o, p);
            //    if ((b == 5) || (b == 6)) a += 10;

            //}

            //if (a > 8000) ars2.Add(o);


            //    }




            Infra.SalvaSaidaW(PTbaseSaida, Infra.NomeSaida("PTBase", concursoBase));
            Infra.SalvaSaidaW(LancesMax, Infra.NomeSaida("PTAlter", concursoBase));
            Infra.SalvaSaidaW(ars3, Infra.NomeSaida("Alter", concursoBase));


            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Primeiro. Atualmente não implementado.
        /// </summary>
        private void Onze_Click(object sender, RoutedEventArgs e)
        {

            Lances ars = new();

            int concursoBase = Convert.ToInt32(T1.Text);
            string nomeArq = "Baseado-" + concursoBase.ToString();

            Lances arq = Infra.AbrirArquivo(nomeArq);

            int s = 0;


            while (s < arq.Count - 25)
            {
                Lances ars1 = new();


                for (int i = s; i < s + 25; i++) { ars1.Add(arq[i]); }

                if (ars1.Count == 25)
                {

                    foreach (Lance o in Infra.arGeral)
                    {
                        int a = 0;

                        foreach (Lance p in ars1)
                        {
                            if (Infra.Contapontos(o, p) != 9) { a++; break; }

                        }

                        if (a == 0)
                            ars.Add(o);
                    }
                }
                s++;


            }


            Infra.SalvaSaidaW(ars, Infra.NomeSaida("Eleitos-", concursoBase));


            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Segundo. Atualmente não implementado.
        /// </summary>
        private void Doze_Click(object sender, RoutedEventArgs e)
        {

            Lances ars1 = new();
            Lances ars2 = new();


            int concursoBase = Convert.ToInt32(T1.Text);
            string nomeArq1 = "Baseado1-" + concursoBase.ToString();
            string nomeArq2 = "Baseado2-" + concursoBase.ToString();

            Lances arq1 = Infra.AbrirArquivo(nomeArq1);
            Lances arq2 = Infra.AbrirArquivo(nomeArq2);





            foreach (Lance o in Infra.arGeral)
            {
                int s = 0;
                int t = 0;

                foreach (Lance p in arq1) { if (Infra.Contapontos(o, p) == 9) { s++; } }
                foreach (Lance p in arq2) { if (Infra.Contapontos(o, p) == 9) { t++; } }

                if (s == 2) ars1.Add(o);
                if (t == 2) ars2.Add(o);
            }



            Infra.SalvaSaidaW(ars1, Infra.NomeSaida("Eleitos1-", concursoBase));
            Infra.SalvaSaidaW(ars2, Infra.NomeSaida("Eleitos2-", concursoBase));


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
        /// Método associado ao clique no botão Décimo Quarto. Atualmente não implementado.
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

                    arsm.AddRange(_palpiteService1.GetGrupoSS());
                    arsn.AddRange(_palpiteService1.GetGrupoNS());
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");


                }


                for (int i = 0; i < 4000; i++)
                {
                    var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(10000, 9);



                    List<int> lsm = new List<int>();
                    List<int> lsn = new List<int>();


                    foreach (Lance o in palpitesAleatorios) { lsm.Add(o.M); lsn.Add(o.N); }


                    OcorrenciaSS.AddRange(Infra.ContaOcorrencia(lsm, arsm));
                    OcorrenciaNS.AddRange(Infra.ContaOcorrencia(lsn, arsn));

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {ex.Message}");
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Infra.SalvaSaidaW(arsm, Infra.NomeSaida("GrupoSS", concursoBase + 1));
            Infra.SalvaSaidaW(arsn, Infra.NomeSaida("GrupoNS", concursoBase + 1));


            Infra.SalvaSaidaW(OcorrenciaSS, Infra.NomeSaida("OcorrenciasSS", concursoBase + 1));
            Infra.SalvaSaidaW(OcorrenciaNS, Infra.NomeSaida("OcorrenciasNS", concursoBase + 1));


            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Quinto. Atualmente não implementado.
        /// </summary>
        private void Quinze_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text) - 1;

            List<int> lsMediaSS = new();
            List<int> lsMediaNS = new();

            Lances palpites = new Lances();

            Lances ars1 = new();
            Lances ars2 = new();
            Lances ars3 = new();


            Lances arsm = new();
            Lances arsn = new();


            try
            {
                if (_palpiteService1 == null)
                {

                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);

                    arsm.AddRange(_palpiteService1.GetGrupoSS());
                    arsn.AddRange(_palpiteService1.GetGrupoNS());
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");


                }


                for (int i = 0; i < 1000; i++)
                {

                    Lances arSS = new();
                    Lances arNS = new();


                    var palpitesAleatorios = _palpiteService1.GerarPalpitesAleatorios(1000, 9);

                    foreach (Lance o in palpitesAleatorios)
                    {
                        arSS.Add(arsm[o.M]);
                        arNS.Add(arsn[o.N]);
                    }


                    var maisFreq1 = Infra.DevolveMaisFrequentes(arSS, 9);
                    var maisFreq2 = Infra.DevolveMaisFrequentes(arNS, 6);


                    lsMediaSS.Add(maisFreq1.PT);
                    lsMediaNS.Add(maisFreq2.PT);

                    ars1.Add(maisFreq1);
                    ars2.Add(maisFreq2);

                }

            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {ex.Message}");
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Infra.SalvaSaidaW(ars1, Infra.NomeSaida("GrupoSaidaMaisFreqSS", concursoBase + 1));
            Infra.SalvaSaidaW(ars2, Infra.NomeSaida("GrupoSaidaMaisFreqNS", concursoBase + 1));

            Infra.SalvaSaidaW(lsMediaSS, Infra.NomeSaida("MediaSS", concursoBase + 1));
            Infra.SalvaSaidaW(lsMediaNS, Infra.NomeSaida("MediaNS", concursoBase + 1));



            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Sexto. Atualmente não implementado.
        /// </summary>
        private void Dezesseis_Click(object sender, RoutedEventArgs e)
        {
            int concursoBase = Convert.ToInt32(T1.Text) - 1;
            int concursoAnterior = Convert.ToInt32(T1.Text) - 3;


            Lance oAlvo = Infra.arLoto[concursoBase];
            Lance oAnterior = Infra.arLoto[concursoAnterior];

            List<int> lsMediaSS = new();
            List<int> lsMediaNS = new();

            Lances palpites = new Lances();

            Lances ars1 = new();
            Lances ars2 = new();
            Lances ars3 = new();


            Lances arsm = new();
            Lances arsn = new();


            try
            {
                if (_palpiteService1 == null)
                {

                    _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);

                    arsm.AddRange(_palpiteService1.GetGrupoSS());
                    arsn.AddRange(_palpiteService1.GetGrupoNS());
                    _logger.LogInformation($"Serviço inicializado com concurso base: {concursoBase}");


                    palpites.AddRange(_palpiteService1.GerarPalpitesAleatorios(1000, 9));

                }


                foreach (Lance o in palpites)
                {
                    List<int> ls = new List<int>();

                    ls.Add(o.M);
                    ls.Add(o.N);

                    ls.Add(Infra.ObterProximos(arsm[o.M], palpites, 8));
                    ls.Add(Infra.ObterProximos(arsn[o.N], palpites, 5));

                    ls.Add(Infra.Contapontos(o, oAlvo));
                    ls.Add(Infra.Contapontos(arsm[o.M], oAnterior));
                    ls.Add(Infra.Contapontos(arsn[o.N], oAnterior));

                    Lance u = new Lance(ars1.Count, ls);

                    ars1.Add(u);

                }



            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro: {ex.Message}");
                MessageBox.Show(ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            Infra.SalvaSaidaW(ars1, Infra.NomeSaida("FrequenciaTotal", concursoBase + 1));
            Infra.SalvaSaidaW(palpites, Infra.NomeSaida("Palpites", concursoBase + 1));


            TerminarPrograma();
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

        #region Extras
        /// <summary>
        /// Método que fecha a aplicação.
        /// </summary>
        private void TerminarPrograma()
        {
            Application.Current.Shutdown();
        }

        private async Task MLAnalise()
        {
            try
            {
                int concursoBase = Convert.ToInt32(T1.Text);

                _logger.LogInformation("Iniciando análise completa");
                AnaliseService.ExecutarAnalise();

                _modelSS.Train("PercentuaisSS.json", usarCrossValidation: true);
                _modelNS.Train("PercentuaisNS.json", usarCrossValidation: true);

                _palpiteService1 = new PalpiteService1(_logger, _modelSS, _modelNS, concursoBase);

                MessageBox.Show("Treinamento Finalizado!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError("Erro durante a execução da análise", ex);
                MessageBox.Show($"Erro durante a execução: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ExibirResultadosV0(Lances palpitesClassificados)
        {
            // Limpar resultados anteriores
            listBoxResultados.Items.Clear();

            foreach (var palpite in palpitesClassificados)
            {
                // Formatar números para melhor visualização
                var numerosFormatados = string.Join(", ", palpite.Lista);
                string resultado = $"Pontuação: {palpite.F0:F2} - Números: {numerosFormatados}";
                listBoxResultados.Items.Add(resultado);
            }
        }

        private void ExibirResultados(Lances palpitesClassificados)
        {
            listBoxResultados.Items.Clear();

            foreach (var palpite in palpitesClassificados)
            {
                var numerosFormatados = string.Join(", ", palpite.Lista.Select(n => n.ToString("00")));
                string resultado = $"Nums: {numerosFormatados} - " +
                                 $"SS:{palpite.M} NS:{palpite.N} - " +
                                 $"Pontuação: {palpite.F0:F2}";
                listBoxResultados.Items.Add(resultado);
            }
        }


        private void SalvarResultados()
        {

            Infra.SalvaSaidaW(Infra.arLoto, Infra.NomeSaida("Resultados", T1.Text));
        }

        #endregion


    }
}
