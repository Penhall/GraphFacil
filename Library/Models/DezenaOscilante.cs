using CommunityToolkit.Mvvm.ComponentModel;
using LotoLibrary.Models;
using System;

namespace LotoLibrary.Services
{
    public partial class DezenaOscilante : ObservableObject
    {
        [ObservableProperty]
        private int _numero;

        [ObservableProperty]
        private double _fase;

        [ObservableProperty]
        private double _frequencia;

        [ObservableProperty]
        private bool _estaSincronizada;

        [ObservableProperty]
        private int _ultimoAtraso;

        [ObservableProperty]
        private double _forcaSincronizacao;

        [ObservableProperty]
        private double _probabilidade;

        [ObservableProperty]
        private double _amplitude = 1.0;

        [ObservableProperty]
        private DateTime _ultimoSorteio;

        [ObservableProperty]
        private int _frequenciaHistorica;

        /// <summary>
        /// Atualiza os dados da dezena com base em um Lance (sorteio)
        /// </summary>
        public void AtualizarComLance(Lance lance)
        {
            if (lance?.Lista?.Contains(Numero) == true)
            {
                UltimoSorteio = DateTime.Now;
                UltimoAtraso = 0;

                // Aumenta a força de sincronização quando sorteada
                ForcaSincronizacao = Math.Min(1.0, ForcaSincronizacao + 0.1);

                // Reinicia a fase quando sorteada
                Fase = 0;
            }
            else
            {
                UltimoAtraso++;

                // Diminui gradualmente a força de sincronização
                ForcaSincronizacao = Math.Max(0.1, ForcaSincronizacao - 0.02);
            }

            // Atualiza probabilidade baseada em múltiplos fatores
            AtualizarProbabilidade();
        }

        /// <summary>
        /// Calcula a probabilidade da dezena ser sorteada baseada em diversos fatores
        /// </summary>
        private void AtualizarProbabilidade()
        {
            // Fator baseado na fase (onda senoidal)
            double fatorFase = (Math.Sin(Fase * Math.PI / 180) + 1) / 2; // 0-1

            // Fator baseado no atraso (números atrasados têm mais chance)
            double fatorAtraso = Math.Min(1.0, UltimoAtraso / 20.0);

            // Fator baseado na força de sincronização
            double fatorSincronizacao = ForcaSincronizacao;

            // Fator baseado na frequência histórica
            double fatorFrequencia = Math.Min(1.0, FrequenciaHistorica / 100.0);

            // Combina todos os fatores com pesos
            Probabilidade = (fatorFase * 0.3) +
                           (fatorAtraso * 0.25) +
                           (fatorSincronizacao * 0.25) +
                           (fatorFrequencia * 0.2);

            // Normaliza para 0-1
            Probabilidade = Math.Max(0, Math.Min(1, Probabilidade));
        }

        /// <summary>
        /// Atualiza a fase do oscilador
        /// </summary>
        public void AtualizarFase(double deltaTime = 1.0)
        {
            Fase += Frequencia * deltaTime;
            Fase = (Fase % 360 + 360) % 360; // Normaliza para 0-360

            AtualizarProbabilidade();
        }

        /// <summary>
        /// Aplica influência de outras dezenas (sincronização)
        /// </summary>
        public void AplicarInfluencia(double influenciaExterna)
        {
            // Ajusta a frequência baseada na influência externa
            double ajusteFrequencia = influenciaExterna * 0.1;
            Frequencia = Math.Max(0.1, Math.Min(3.0, Frequencia + ajusteFrequencia));

            // Determina se está sincronizada baseado na influência
            EstaSincronizada = Math.Abs(influenciaExterna) > 0.3;

            // Ajusta a força de sincronização
            if (EstaSincronizada)
            {
                ForcaSincronizacao = Math.Min(1.0, ForcaSincronizacao + 0.05);
            }
        }

        /// <summary>
        /// Reseta o oscilador para valores iniciais
        /// </summary>
        public void Reset()
        {
            var random = new Random();
            Fase = random.Next(0, 360);
            Frequencia = 1.0 + (random.NextDouble() - 0.5) * 0.4; // 0.8 - 1.2
            ForcaSincronizacao = 0.5;
            EstaSincronizada = false;
            Probabilidade = 0.5;
            Amplitude = 1.0;
        }

        /// <summary>
        /// Calcula o valor atual da onda baseado na fase
        /// </summary>
        public double ValorAtual => Amplitude * Math.Sin(Fase * Math.PI / 180);

        /// <summary>
        /// Indica se a dezena está em uma fase "quente" (alta probabilidade)
        /// </summary>
        public bool EstaQuente => Probabilidade > 0.7;

        /// <summary>
        /// Indica se a dezena está em uma fase "fria" (baixa probabilidade)
        /// </summary>
        public bool EstaFria => Probabilidade < 0.3;

        /// <summary>
        /// Calcula a distância de fase entre esta dezena e outra
        /// </summary>
        public double DistanciaFase(DezenaOscilante outra)
        {
            double diff = Math.Abs(Fase - outra.Fase);
            return Math.Min(diff, 360 - diff); // Menor distância circular
        }

        /// <summary>
        /// Verifica se está sincronizada com outra dezena (fases próximas)
        /// </summary>
        public bool EstaSincronizadaCom(DezenaOscilante outra, double tolerancia = 30)
        {
            return DistanciaFase(outra) <= tolerancia;
        }

        public override string ToString()
        {
            return $"Dezena {Numero}: Fase={Fase:F1}°, Prob={Probabilidade:F2}, Sync={EstaSincronizada}";
        }
    }
}