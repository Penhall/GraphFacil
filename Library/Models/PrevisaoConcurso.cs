// LotoLibrary/Services/PrevisaoConcurso.cs

// LotoLibrary/Services/PrevisaoConcurso.cs
using LotoLibrary.Enums;

namespace LotoLibrary.Models
{
    public class PrevisaoConcurso
    {
        public int Concurso { get; set; }
        public double Probabilidade { get; set; }
        public int IntervaloEsperado { get; set; }
        public ConfiancaPrevisao Confianca { get; set; }

        public override string ToString()
        {
            return $"Concurso {Concurso}: {Probabilidade:P1}";
        }
    }

}