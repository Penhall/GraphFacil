using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.DeepLearning.DataProcessing
{
    /// <summary>
    /// Serviço responsável por pré-processar os dados da loteria para modelos de deep learning.
    /// Inclui codificação, sequenciamento e preparação dos dados para tensorização.
    /// </summary>
    public class DataProcessorService
    {
        private const int TotalNumbers = 25;

        /// <summary>
        /// Converte um único concurso (Lance) em um vetor multi-hot-encoded.
        /// </summary>
        /// <param name="lance">O concurso a ser codificado.</param>
        /// <returns>Um array de float de tamanho 25, com 1.0f para os números sorteados e 0.0f para os demais.</returns>
        public float[] MultiHotEncode(Lance lance)
        {
            var encoded = new float[TotalNumbers];
            if (lance?.Lista == null)
            {
                // Retorna um vetor de zeros se o lance for inválido
                return encoded;
            }

            foreach (var number in lance.Lista)
            {
                if (number >= 1 && number <= TotalNumbers)
                {
                    // O número 1 corresponde ao índice 0, o número 25 ao índice 24.
                    encoded[number - 1] = 1.0f;
                }
            }
            return encoded;
        }

        /// <summary>
        /// Cria sequências de entrada e seus respectivos rótulos (labels) a partir de dados históricos.
        /// Por exemplo, usa os últimos 50 concursos (sequência) para prever o próximo (rótulo).
        /// </summary>
        /// <param name="historicalData">A lista completa de concursos históricos.</param>
        /// <param name="sequenceLength">O número de concursos passados a serem usados como uma única sequência de entrada.</param>
        /// <returns>Uma tupla contendo a lista de sequências de features e a lista de rótulos correspondentes.</returns>
        public (List<float[][]> features, List<float[]> labels) CreateSequences(Lances historicalData, int sequenceLength)
        {
            var features = new List<float[][]>();
            var labels = new List<float[]>();

            if (historicalData == null || historicalData.Count <= sequenceLength)
            {
                // Não há dados suficientes para criar nenhuma sequência.
                return (features, labels);
            }

            // Garante que os dados estão em ordem cronológica
            var orderedLances = historicalData.OrderBy(l => l.Id).ToList();

            for (int i = 0; i <= orderedLances.Count - sequenceLength - 1; i++)
            {
                var sequence = new float[sequenceLength][];
                for (int j = 0; j < sequenceLength; j++)
                {
                    sequence[j] = MultiHotEncode(orderedLances[i + j]);
                }
                features.Add(sequence);

                var label = MultiHotEncode(orderedLances[i + sequenceLength]);
                labels.Add(label);
            }

            return (features, labels);
        }
    }
}