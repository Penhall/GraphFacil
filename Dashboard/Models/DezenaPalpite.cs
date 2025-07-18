// Dashboard/Models/DezenaPalpite.cs
using CommunityToolkit.Mvvm.ComponentModel;

namespace Dashboard.Models
{
    /// <summary>
    /// Representa uma dezena em um palpite gerado, com propriedades visuais
    /// </summary>
    public partial class DezenaPalpite : ObservableObject
    {
        [ObservableProperty]
        private int _numero;

        [ObservableProperty]
        private double _confianca;

        [ObservableProperty]
        private System.Windows.Media.Brush _backgroundColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.DarkSlateBlue);

        [ObservableProperty]
        private System.Windows.Media.Brush _foregroundColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);

        [ObservableProperty]
        private System.Windows.Media.Brush _borderColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGreen);

        [ObservableProperty]
        private bool _isHighConfidence;

        [ObservableProperty]
        private bool _foiAcertada = false;

        [ObservableProperty]
        private bool _isEscolhida = true; // Por padrão, dezenas do palpite são escolhidas

        public DezenaPalpite(int numero, double confianca = 1.0)
        {
            Numero = numero;
            Confianca = confianca;
            UpdateVisualProperties();
        }

        private void UpdateVisualProperties()
        {
            var cc = new System.Windows.Media.ColorConverter();
            
            // Prioridade: Acertada > Escolhida > Confiança
            if (FoiAcertada)
            {
                // Dezena foi acertada - Verde vibrante
                BackgroundColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF5E9B47"));
                ForegroundColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                BorderColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF7FB348"));
                IsHighConfidence = true;
            }
            else if (!IsEscolhida)
            {
                // Dezena não foi escolhida no palpite - Cores neutras/cinza
                BackgroundColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF3B4252"));
                ForegroundColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF616E88"));
                BorderColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF4C566A"));
                IsHighConfidence = false;
            }
            else
            {
                // Dezena escolhida - cores baseadas na confiança
                if (Confianca >= 0.8)
                {
                    // Alta confiança - Azul forte
                    BackgroundColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF5E81AC"));
                    ForegroundColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    BorderColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF81A1C1"));
                    IsHighConfidence = true;
                }
                else if (Confianca >= 0.6)
                {
                    // Média confiança - Azul médio
                    BackgroundColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF81A1C1"));
                    ForegroundColor = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                    BorderColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF88C0D0"));
                    IsHighConfidence = false;
                }
                else
                {
                    // Baixa confiança - Azul claro
                    BackgroundColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF88C0D0"));
                    ForegroundColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF2E3440"));
                    BorderColor = new System.Windows.Media.SolidColorBrush((System.Windows.Media.Color)cc.ConvertFrom("#FF8FBCBB"));
                    IsHighConfidence = false;
                }
            }
        }

        /// <summary>
        /// Atualiza cores baseado no contraste com outras dezenas
        /// </summary>
        public void AtualizarCoresContraste(bool estaEscolhida)
        {
            IsEscolhida = estaEscolhida;
            UpdateVisualProperties();
        }

        partial void OnConfiancaChanged(double value)
        {
            UpdateVisualProperties();
        }

        partial void OnFoiAcertadaChanged(bool value)
        {
            UpdateVisualProperties();
        }

        partial void OnIsEscolhidaChanged(bool value)
        {
            UpdateVisualProperties();
        }
    }
}