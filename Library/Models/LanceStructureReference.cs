// Library/Models/LanceStructureReference.cs
// REFERÊNCIA DE USO - Estrutura correta da classe Lance

using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models
{
    /// <summary>
    /// GUIA DE USO DA CLASSE LANCE - Propriedades Corretas
    /// Use este arquivo como referência para evitar erros de propriedades
    /// </summary>
    public static class LanceUsageGuide
    {
        /// <summary>
        /// PROPRIEDADES PRINCIPAIS DA CLASSE LANCE:
        /// 
        /// ✅ CORRETO:
        /// - lance.Id (int) - Número do concurso
        /// - lance.Lista (List<int>) - Dezenas sorteadas
        /// 
        /// ❌ INCORRETO:
        /// - lance.Numero (NÃO EXISTE!)
        /// - lance.Dezenas (NÃO EXISTE!)
        /// </summary>
        public static void ExemplosDeUsoCorreto()
        {
            var lance = new Lance(3001, new List<int> { 1, 2, 3, 4, 5 });

            // ✅ CORRETO - Acessar número do concurso
            int numeroConcurso = lance.Id;

            // ✅ CORRETO - Acessar dezenas sorteadas
            List<int> dezenasSorteadas = lance.Lista;

            // ✅ CORRETO - Verificar se uma dezena foi sorteada
            bool dezena10FoiSorteada = lance.Lista.Contains(10);

            // ✅ CORRETO - Contar quantas dezenas foram sorteadas
            int totalDezenas = lance.Lista.Count;

            // ❌ INCORRETO - Estas propriedades NÃO EXISTEM:
            // int numero = lance.Numero; // ERRO!
            // var dezenas = lance.Dezenas; // ERRO!
        }

        /// <summary>
        /// EXEMPLOS DE OPERAÇÕES COMUNS COM LANCE
        /// </summary>
        public static class ExemplosComuns
        {
            // Extrair histórico de aparições de uma dezena
            public static List<int> ExtrairHistoricoAparicoes(List<Lance> historico, int dezena)
            {
                return historico
                    .Where(lance => lance.Lista.Contains(dezena))  // ✅ Lista, não Dezenas
                    .Select(lance => lance.Id)                    // ✅ Id, não Numero
                    .OrderBy(num => num)
                    .ToList();
            }

            // Processar um novo sorteio
            public static void ProcessarSorteio(Lance novoSorteio)
            {
                Console.WriteLine($"Processando concurso {novoSorteio.Id}");          // ✅ Id
                Console.WriteLine($"Dezenas: [{string.Join(", ", novoSorteio.Lista)}]"); // ✅ Lista
            }

            // Contar acertos entre palpite e sorteio
            public static int ContarAcertos(List<int> palpite, Lance sorteio)
            {
                return palpite.Intersect(sorteio.Lista).Count();  // ✅ Lista
            }

            // Obter último concurso
            public static int ObterUltimoConcurso(List<Lance> historico)
            {
                return historico.LastOrDefault()?.Id ?? 0;  // ✅ Id
            }
        }

        /// <summary>
        /// ESTRUTURA COMPLETA DA CLASSE LANCE PARA REFERÊNCIA
        /// </summary>
        public static class EstruturaCompleta
        {
            /*
            public class Lance
            {
                // IDENTIFICAÇÃO
                public int Id;                              // ✅ Número do concurso

                // MÉTRICAS
                public int M = 0;
                public int N = 0;
                public int X = 0;
                public int Y = 0;
                public int PT = 0;

                // FATORES
                public float F0 = 0;
                public float F1 = 0;
                public float F2 = 0;

                // CONTADORES
                public Dictionary<int, int> ContagemAcerto = new();
                public int Anel = 0;

                // LISTAS PRINCIPAIS
                public List<int> Lista;                     // ✅ Dezenas sorteadas
                public List<int> ListaN = new();
                public List<int> ListaM = new();
                public List<Double> MN = new();

                // SUBLISTAS
                public Lances ListaX = new();
                public Lances ListaY = new();

                // FORMATAÇÃO
                public string Saida;
                public string Nome;

                // CONSTRUTORES E MÉTODOS
                public Lance(int id, List<int> lista) { ... }
                public Lance() { ... }
                public string Atualiza() { ... }
                public void Ordena() { ... }
                public void LimpaListas() { ... }
                public override string ToString() { ... }
            }
            */
        }

        /// <summary>
        /// MIGRAÇÕES DE CÓDIGO ANTIGO
        /// Use estas substituições para corrigir código existente
        /// </summary>
        public static class MigracaoDeCodigoAntigo
        {
            /*
            SUBSTITUIÇÕES NECESSÁRIAS:

            ❌ lance.Numero           →  ✅ lance.Id
            ❌ lance.Dezenas          →  ✅ lance.Lista
            ❌ sorteio.Numero         →  ✅ sorteio.Id
            ❌ resultado.Numero       →  ✅ resultado.Id
            ❌ concurso.Numero        →  ✅ concurso.Id

            EXEMPLOS DE MIGRAÇÃO:

            // ANTES (incorreto):
            var ultimoConcurso = historico.Last().Numero;
            bool contemDezena = lance.Dezenas.Contains(10);
            
            // DEPOIS (correto):
            var ultimoConcurso = historico.Last().Id;
            bool contemDezena = lance.Lista.Contains(10);
            */
        }
    }
}