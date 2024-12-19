using LotoLibrary.Models;
using System;
using System.Collections.Generic;

namespace LotoLibrary.Services
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
            GerarCombinacoesRecursivo(elementos, tamanhoCombinacao, 0, combinacaoAtual, resultado);
            return resultado;
        }

        private static void GerarCombinacoesRecursivo(List<int> elementos, int tamanhoCombinacao, int indiceInicial,
            List<int> combinacaoAtual, Lances resultado)
        {
            if (combinacaoAtual.Count == tamanhoCombinacao)
            {
                resultado.Add(new Lance(resultado.Count, new List<int>(combinacaoAtual)));
                return;
            }

            for (int i = indiceInicial; i < elementos.Count; i++)
            {
                combinacaoAtual.Add(elementos[i]);
                GerarCombinacoesRecursivo(elementos, tamanhoCombinacao, i + 1, combinacaoAtual, resultado);
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
