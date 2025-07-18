// D:\PROJETOS\GraphFacil\Dashboard\Views\ConfiguracaoTreinamentoDialog.cs
using LotoLibrary.Services;
using LotoLibrary.Models.Configuration;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Dashboard.Views
{
    /// <summary>
    /// Dialog para configuração de treinamento (WPF)
    /// </summary>
    public partial class ConfiguracaoTreinamentoDialog : Window
    {
        public ConfiguracaoTreinamento Configuracao { get; private set; }

        private TextBox _textBoxValidacao;
        private TextBox _textBoxInicio;
        private TextBox _textBoxFim;

        public ConfiguracaoTreinamentoDialog(ConfiguracaoTreinamento configuracao)
        {
            InitializeComponent();
            Configuracao = configuracao;
        }

        private void InitializeComponent()
        {
            Title = "Configuração de Treinamento";
            Width = 450;
            Height = 320;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Background = System.Windows.Media.Brushes.White;

            var stackPanel = new StackPanel { Margin = new Thickness(20) };

            // Título
            var titulo = new TextBlock
            {
                Text = "🔧 Configurar Dados de Treinamento",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            // Informações atuais
            var info = new TextBlock
            {
                Text = $"Range disponível: {Configuracao.ConcursoMinimo} → {Configuracao.ConcursoMaximo}",
                Margin = new Thickness(0, 0, 0, 15),
                Foreground = System.Windows.Media.Brushes.Gray,
                FontSize = 12
            };

            // Tamanho da validação
            var labelValidacao = new Label { Content = "Tamanho da Validação (concursos):" };
            _textBoxValidacao = new TextBox
            {
                Text = Configuracao.TamanhoValidacao.ToString(),
                Margin = new Thickness(0, 0, 0, 10),
                Height = 25,
                Padding = new Thickness(5)
            };

            // Concurso início
            var labelInicio = new Label { Content = $"Concurso Início (mín: {Configuracao.ConcursoMinimo}, vazio=sem filtro):" };
            _textBoxInicio = new TextBox
            {
                Text = Configuracao.ConcursoInicio > 1 ? Configuracao.ConcursoInicio.ToString() : "",
                Margin = new Thickness(0, 0, 0, 10),
                Height = 25,
                Padding = new Thickness(5)
            };

            // Concurso fim
            var labelFim = new Label { Content = $"Concurso Fim (máx: {Configuracao.ConcursoMaximo}, vazio=sem filtro):" };
            _textBoxFim = new TextBox
            {
                Text = Configuracao.ConcursoFim > 0 ? Configuracao.ConcursoFim.ToString() : "",
                Margin = new Thickness(0, 0, 0, 15),
                Height = 25,
                Padding = new Thickness(5)
            };

            // Botões
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 20, 0, 0)
            };

            var okButton = new Button
            {
                Content = "OK",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5),
                IsDefault = true,
                Background = System.Windows.Media.Brushes.LightBlue,
                BorderBrush = System.Windows.Media.Brushes.DodgerBlue
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = "Cancelar",
                Width = 80,
                Height = 30,
                Margin = new Thickness(5),
                IsCancel = true,
                Background = System.Windows.Media.Brushes.LightGray
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(titulo);
            stackPanel.Children.Add(info);
            stackPanel.Children.Add(labelValidacao);
            stackPanel.Children.Add(_textBoxValidacao);
            stackPanel.Children.Add(labelInicio);
            stackPanel.Children.Add(_textBoxInicio);
            stackPanel.Children.Add(labelFim);
            stackPanel.Children.Add(_textBoxFim);
            stackPanel.Children.Add(buttonPanel);

            Content = stackPanel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(_textBoxValidacao.Text, out int validacao) || validacao <= 0)
                {
                    MessageBox.Show("Tamanho de validação deve ser um número maior que 0",
                        "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var inicio = int.TryParse(_textBoxInicio.Text, out int inicioValue) ? inicioValue : 1;
                var fim = int.TryParse(_textBoxFim.Text, out int fimValue) ? fimValue : -1;

                // Validações
                if (inicio < Configuracao.ConcursoMinimo)
                {
                    MessageBox.Show($"Concurso início deve ser >= {Configuracao.ConcursoMinimo}",
                        "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (fim > 0 && fim > Configuracao.ConcursoMaximo)
                {
                    MessageBox.Show($"Concurso fim deve ser <= {Configuracao.ConcursoMaximo}",
                        "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (fim > 0 && inicio >= fim)
                {
                    MessageBox.Show("Concurso início deve ser menor que concurso fim",
                        "Valor Inválido", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Atualizar configuração
                Configuracao.TamanhoValidacao = validacao;
                Configuracao.ConcursoInicio = inicio;
                Configuracao.ConcursoFim = fim;

                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro na validação: {ex.Message}",
                    "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}