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

        private void BtnFechar_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }


        private void BtnPerfil_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnLembrete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void GridBarraTitulo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Primeiro_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);


            Lances A = Estudos.Estudo1(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo1", alvo));

            TerminarPrograma();

        }

        private void Segundo_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);


            Lances A = Estudos.Estudo2(alvo);


            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo2", alvo));

            TerminarPrograma();
        }

        private void Terceiro_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            AnaliseService.ExecutarAnalise();



            TerminarPrograma();

        }

        private void Quarto_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();

        }

        private void Quinto_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        private void Sexto_Click(object sender, RoutedEventArgs e)
        {



            TerminarPrograma();
        }

        private void Setimo_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        private void Oitavo_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        private void Nono_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        private void Dez_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        private void Onze_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        private void Doze_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        private void Treze_Click(object sender, RoutedEventArgs e)
        {
            TerminarPrograma();
        }

        private void Catorze_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        private void Quinze_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }


        private void Dezesseis_Click(object sender, RoutedEventArgs e)
        {


            TerminarPrograma();
        }


        private void Dezessete_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }

        private void Dezoito_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }


        private void Dezenove_Click(object sender, RoutedEventArgs e)
        {


            TerminarPrograma();
        }


        private void Vinte_Click(object sender, RoutedEventArgs e)
        {

            TerminarPrograma();
        }



        #region Extras
        private void TerminarPrograma()
        {
            Application.Current.Shutdown();
        }
        #endregion


    }

}
