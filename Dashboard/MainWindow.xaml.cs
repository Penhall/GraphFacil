using Busisness;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System;
using System.Windows;
using System.Windows.Input;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Infra.CarregarConcursos();

            T1.Text = Infra.arLoto.Count.ToString();
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
        private void Quarto_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();

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

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Sétimo. Atualmente não implementado.
        /// </summary>
        private void Setimo_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Oitavo. Atualmente não implementado.
        /// </summary>
        private void Oitavo_Click(object sender, RoutedEventArgs e)
        {

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

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Primeiro. Atualmente não implementado.
        /// </summary>
        private void Onze_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Segundo. Atualmente não implementado.
        /// </summary>
        private void Doze_Click(object sender, RoutedEventArgs e)
        {
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

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Quinto. Atualmente não implementado.
        /// </summary>
        private void Quinze_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        /// <summary>
        /// Método associado ao clique no botão Décimo Sexto. Atualmente não implementado.
        /// </summary>
        private void Dezesseis_Click(object sender, RoutedEventArgs e)
        {

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
        #endregion
    }
}
