using Busisness;
using LotoLibrary.Constantes;
using LotoLibrary.Models;
using LotoLibrary.Services;
using System.Collections.Generic;
using System.Windows.Input;
using System.Windows;
using System;

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


            Lances A = Estudos.Estudo3(alvo);


            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo3", alvo));

            TerminarPrograma();

        }

        private void Quarto_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);


            Lances A = Estudos.Estudo4(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo4", alvo));

            TerminarPrograma();

        }

        private void Quinto_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo5(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo5", alvo));

            TerminarPrograma();
        }

        private void Sexto_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo6(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo6", alvo));

            TerminarPrograma();
        }

        private void Setimo_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo7(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo7", alvo));

            TerminarPrograma();
        }

        private void Oitavo_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo8(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo8", alvo));

            TerminarPrograma();
        }

        private void Nono_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo9(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaEstudo9", alvo));

            TerminarPrograma();
        }

        private void Dez_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo10(alvo);


            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaPossivel10", alvo));

            TerminarPrograma();
        }

        private void Onze_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo11(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaPossivel11", alvo));

            TerminarPrograma();
        }

        private void Doze_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo12(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaPossivel12", alvo));

            TerminarPrograma();
        }

        private void Treze_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo13(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("ListaPossivel13", alvo));

            TerminarPrograma();
        }

        private void Catorze_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo14(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("Estudo14", alvo));

            TerminarPrograma();
        }

        private void Quinze_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo15(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("PossíveisEstudo15", alvo));

            TerminarPrograma();
        }


        private void Dezesseis_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo16(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("Estudo16", alvo));


            TerminarPrograma();
        }


        private void Dezessete_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo17(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("Estudo17", alvo));

            TerminarPrograma();
        }

        private void Dezoito_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo18(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("Estudo18", alvo));

            TerminarPrograma();
        }


        private void Dezenove_Click(object sender, RoutedEventArgs e)
        {

            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo19(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("Estudo19", alvo));


            TerminarPrograma();
        }


        private void Vinte_Click(object sender, RoutedEventArgs e)
        {
            int alvo = Convert.ToInt32(T1.Text);

            Lances A = Estudos.Estudo20(alvo);

            Infra.SalvaSaidaW(A, Infra.NomeSaida("Estudo20", alvo));


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
