// Dashboard/Models/PalpiteCompleto.cs
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dashboard.Models
{
    /// <summary>
    /// Representa um palpite completo com análise de acertos
    /// </summary>
    public partial class PalpiteCompleto : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<DezenaPalpite> _dezenas = new();

        [ObservableProperty]
        private string _palpiteTexto;

        [ObservableProperty]
        private double _confianca;

        [ObservableProperty]
        private int _quantidadeAcertos = 0;

        [ObservableProperty]
        private string _modeloNome;

        [ObservableProperty]
        private int _concursoAlvo;

        [ObservableProperty]
        private bool _foiValidado = false;

        public PalpiteCompleto(int[] numeros, double confianca, string modeloNome, int concursoAlvo)
        {
            ModeloNome = modeloNome;
            Confianca = confianca;
            ConcursoAlvo = concursoAlvo;
            
            // Criar dezenas ordenadas
            Dezenas = new ObservableCollection<DezenaPalpite>(
                numeros.OrderBy(n => n).Select(n => new DezenaPalpite(n, confianca))
            );

            PalpiteTexto = string.Join(", ", numeros.OrderBy(n => n).Select(n => n.ToString("00")));
        }

        /// <summary>
        /// Valida o palpite contra um resultado conhecido
        /// </summary>
        public void ValidarContraResultado(int[] numerosSorteados)
        {
            if (numerosSorteados == null || numerosSorteados.Length == 0)
            {
                FoiValidado = false;
                return;
            }

            var numerosSet = numerosSorteados.ToHashSet();
            QuantidadeAcertos = 0;

            foreach (var dezena in Dezenas)
            {
                if (numerosSet.Contains(dezena.Numero))
                {
                    dezena.FoiAcertada = true;
                    QuantidadeAcertos++;
                }
                else
                {
                    dezena.FoiAcertada = false;
                }
            }

            FoiValidado = true;
        }

        /// <summary>
        /// Atualiza as cores das dezenas baseado na escolha global
        /// </summary>
        public void AtualizarCoresContraste(int[] todasDezenasPossivel)
        {
            var dezenasPossiveis = todasDezenasPossivel?.ToHashSet() ?? new HashSet<int>();
            
            foreach (var dezena in Dezenas)
            {
                dezena.AtualizarCoresContraste(dezenasPossiveis.Contains(dezena.Numero));
            }
        }
    }
}