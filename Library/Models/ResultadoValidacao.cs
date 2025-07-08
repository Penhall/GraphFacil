using System;
using System.Collections.Generic;

namespace LotoLibrary.Models
{
    /// <summary>
    /// Resultado da validação de um palpite individual
    /// </summary>
    public class ResultadoValidacao
    {
        public int ConcursoId { get; set; }
        public List<int> PalpiteGerado { get; set; } = new();
        public List<int> ResultadoReal { get; set; } = new();
        public int Acertos { get; set; }
        public double TaxaAcerto { get; set; }
        public List<int> NumerosAcertados { get; set; } = new();
        public List<int> NumerosPerdidos { get; set; } = new();
        public DateTime DataTeste { get; set; }
        public string TipoEstrategia { get; set; } = "Osciladores";
    }
}