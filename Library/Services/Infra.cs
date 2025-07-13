using System.Collections.Generic;
using LotoLibrary.Models;

namespace LotoLibrary.Services
{
    public static class Infra
    {
        public static Lances CarregarConcursos()
        {
            // Retorna uma lista de lances vazia para evitar erros de referência nula.
            // Em um ambiente real, isso carregaria os dados de um arquivo ou banco de dados.
            return new Lances();
        }
    }
}
