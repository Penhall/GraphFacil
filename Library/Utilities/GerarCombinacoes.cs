// D:\PROJETOS\GraphFacil\Library\Utilities\GerarCombinacoes.cs
using System.Collections.Generic;
using System;
using LotoLibrary.Models.Core;

namespace LotoLibrary.Utilities
{
    public class GerarCombinacoes
    {
        /// <summary>
        /// Gera todas as combinações possíveis de uma lista de números, dado um tamanho específico.
        /// </summary>
        /// <param name="elementos">Lista de números para gerar combinações</param>
        /// <param name="tamanhoCombinacao">Tamanho desejado de cada combinação</param>
        /// <returns>Lances contendo todas as combinações possíveis</returns>
        public static Lances Combinar(List<int> elementos, int tamanhoCombinacao)
        {
            if (tamanhoCombinacao > elementos.Count)
                throw new ArgumentException("Tamanho da combinação não pode ser maior que o número de elementos");

            Lances resultado = new();
            List<int> combinacaoAtual = new(tamanhoCombinacao);

            // Iniciar com contador de ID em 0
            int contadorId = 0;

            GerarCombinacoesRecursivo(elementos, tamanhoCombinacao, 0, combinacaoAtual, resultado, ref contadorId);
            return resultado;
        }

        private static void GerarCombinacoesRecursivo(List<int> elementos, int tamanhoCombinacao, int indiceInicial,
            List<int> combinacaoAtual, Lances resultado, ref int contadorId)
        {
            if (combinacaoAtual.Count == tamanhoCombinacao)
            {
                // Usar o contador como ID e depois incrementá-lo
                resultado.Add(new Lance(contadorId, new List<int>(combinacaoAtual)));
                contadorId++;
                return;
            }

            for (int i = indiceInicial; i < elementos.Count; i++)
            {
                combinacaoAtual.Add(elementos[i]);
                GerarCombinacoesRecursivo(elementos, tamanhoCombinacao, i + 1, combinacaoAtual, resultado, ref contadorId);
                combinacaoAtual.RemoveAt(combinacaoAtual.Count - 1);
            }
        }

        // Métodos de conveniência para casos comuns
        public static Lances Combinar15a9(List<int> elementos)
        {
            return Combinar(elementos, 9);
        }

        public static Lances Combinar10a6(List<int> elementos)
        {
            return Combinar(elementos, 6);
        }

        public static Lances Combinar25a15(List<int> elementos)
        {
            return Combinar(elementos, 15);
        }

        public static Lances Combinar25a10(List<int> elementos)
        {
            return Combinar(elementos, 10);
        }
    }
}
