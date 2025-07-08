// D:\PROJETOS\GraphFacil\Dashboard\ViewModel\SeletorConcursoDialog.cs
using System.Windows;

namespace Dashboard.ViewModel
{
    /// <summary>
    /// Dialog para seleção de concurso (mantido do código original)
    /// </summary>
    public class SeletorConcursoDialog : Window
    {
        public int ConcursoSelecionado { get; private set; }
        public string TipoSelecao { get; private set; } = "Personalizado";
        public bool GerarPalpiteAutomatico { get; private set; }

        private System.Windows.Controls.TextBox _textBoxConcurso;
        private System.Windows.Controls.ComboBox _comboTipo;
        private System.Windows.Controls.CheckBox _checkGerarPalpite;

        public SeletorConcursoDialog(int concursoAtual, int minimo, int maximo)
        {
            Title = "Selecionar Concurso Alvo";
            Width = 400;
            Height = 280;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var stackPanel = new System.Windows.Controls.StackPanel
            {
                Margin = new Thickness(20)
            };

            // Título
            var titulo = new System.Windows.Controls.TextBlock
            {
                Text = "🎯 Selecionar Concurso para Análise",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Informações
            var info = new System.Windows.Controls.TextBlock
            {
                Text = $"Concurso atual: {concursoAtual}\nRange disponível: {minimo} - {maximo}",
                Margin = new Thickness(0, 0, 0, 15),
                Foreground = System.Windows.Media.Brushes.Gray
            };

            // Tipo de seleção
            var labelTipo = new System.Windows.Controls.Label { Content = "Tipo de Análise:" };
            _comboTipo = new System.Windows.Controls.ComboBox
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Próximo Concurso", Tag = "Proximo" });
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Concurso Histórico", Tag = "Historico" });
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Previsão Futura", Tag = "Futuro" });
            _comboTipo.Items.Add(new System.Windows.Controls.ComboBoxItem { Content = "Personalizado", Tag = "Personalizado" });
            _comboTipo.SelectedIndex = 0;

            // Número do concurso
            var labelConcurso = new System.Windows.Controls.Label { Content = "Número do Concurso:" };
            _textBoxConcurso = new System.Windows.Controls.TextBox
            {
                Text = concursoAtual.ToString(),
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Checkbox para gerar palpite automaticamente
            _checkGerarPalpite = new System.Windows.Controls.CheckBox
            {
                Content = "Gerar palpite automaticamente",
                IsChecked = true,
                Margin = new Thickness(0, 0, 0, 15)
            };

            // Botões
            var buttonPanel = new System.Windows.Controls.StackPanel
            {
                Orientation = System.Windows.Controls.Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new System.Windows.Controls.Button
            {
                Content = "OK",
                Width = 70,
                Margin = new Thickness(5),
                IsDefault = true
            };
            okButton.Click += (s, e) =>
            {
                if (int.TryParse(_textBoxConcurso.Text, out int numero) && numero >= minimo && numero <= maximo + 50)
                {
                    ConcursoSelecionado = numero;
                    var selectedItem = (System.Windows.Controls.ComboBoxItem)_comboTipo.SelectedItem;
                    TipoSelecao = selectedItem.Tag.ToString();
                    GerarPalpiteAutomatico = _checkGerarPalpite.IsChecked == true;
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show($"Digite um número entre {minimo} e {maximo + 50}", "Valor Inválido");
                }
            };

            var cancelButton = new System.Windows.Controls.Button
            {
                Content = "Cancelar",
                Width = 70,
                Margin = new Thickness(5),
                IsCancel = true
            };
            cancelButton.Click += (s, e) => { DialogResult = false; Close(); };

            // Eventos
            _comboTipo.SelectionChanged += (s, e) =>
            {
                var selectedItem = (System.Windows.Controls.ComboBoxItem)_comboTipo.SelectedItem;
                switch (selectedItem.Tag.ToString())
                {
                    case "Proximo":
                        _textBoxConcurso.Text = (maximo + 1).ToString();
                        break;
                    case "Historico":
                        _textBoxConcurso.Text = maximo.ToString();
                        break;
                    case "Futuro":
                        _textBoxConcurso.Text = (maximo + 10).ToString();
                        break;
                }
            };

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            stackPanel.Children.Add(titulo);
            stackPanel.Children.Add(info);
            stackPanel.Children.Add(labelTipo);
            stackPanel.Children.Add(_comboTipo);
            stackPanel.Children.Add(labelConcurso);
            // stackPanel.Children.Add(_textBoxFim);
            stackPanel.Children.Add(_textBoxConcurso);
            stackPanel.Children.Add(_checkGerarPalpite);
            stackPanel.Children.Add(buttonPanel);

            Content = stackPanel;
        }
    }
}