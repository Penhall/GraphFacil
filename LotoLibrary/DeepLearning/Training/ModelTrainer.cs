using LotoLibrary.DeepLearning.Architectures;
using LotoLibrary.DeepLearning.DataProcessing;
using LotoLibrary.Models;
using Microsoft.Extensions.Logging;
using Microsoft.ML.OnnxRuntime.Tensors;
using SciSharp.TensorFlow.Redist;
using System;
using System.Threading.Tasks;
using Tensorflow; // Make sure this is present
using Tensorflow.Keras;
using Tensorflow.Keras.Callbacks;
using Tensorflow.Keras.Engine;
using Tensorflow.Keras.Optimizers;
using Tensorflow.Keras.Layers;
using Tensorflow.NumPy;
using static Tensorflow.Binding;

namespace LotoLibrary.DeepLearning.Training
{
    /// <summary>
    /// Responsável por treinar modelos de redes neurais usando Keras.
    /// </summary>
    public class ModelTrainer
    {
        private readonly ILogger<ModelTrainer> _logger;
        private readonly IOptimizer _optimizer;

        public ModelTrainer(ILogger<ModelTrainer> logger = null)
        {
            _logger = logger;
            _optimizer = new Adam(); // Default optimizer, can be injected
        }
        /// <summary>
        /// Treina um modelo de rede neural.
        /// </summary>
        /// <param name="model">O modelo a ser treinado.</param>
        /// <param name="data">Dados históricos de treinamento.</param>
        /// <param name="processor">Serviço para pré-processar os dados.</param>
        /// <param name="epochs">Número de épocas de treinamento.</param>
        /// <param name="batchSize">Tamanho do lote para treinamento.</param>
        /// <param name="validationSplit">Proporção dos dados para validação (0.0 a 1.0).</param>
        public async Task TrainAsync(LstmAttentionNetwork model, Lances data, DataProcessorService processor, int epochs = 100, int batchSize = 32, double validationSplit = 0.2, float initialLearningRate = 0.001f)
        {
            _logger?.LogInformation($"Iniciando treinamento do modelo. Épocas: {epochs}, Batch Size: {batchSize}, Validação Split: {validationSplit}");

            try
            {
                // 1. Pré-processar os dados
                int sequenceLength = 50; // Ajustar conforme necessário
                var (features, labels) = processor.CreateSequences(data, sequenceLength);

                if (!features.Any() || !labels.Any())
                {
                    throw new InvalidOperationException("Não foi possível criar sequências de treinamento com os dados fornecidos.");
                }

                // 2. Converter para tensores
                var xs = ConvertToNDArrays(features, sequenceLength);
                var ys = ConvertToNDArray(labels);

                // 3. Configurar callbacks (ex: checkpoint para salvar o melhor modelo)
                 var callbacks = new List<ICallback>();

                 // Learning rate scheduler
                 var scheduler = new ReduceLROnPlateau(
                 monitor: "val_loss",
                 factor: 0.2f,
                 patience: 5,
                 min_lr: 0.0001f
                );
                 callbacks.Add(scheduler);
                 
                 // ModelCheckpoint
                 callbacks.Add(new ModelCheckpoint("checkpoints/best_model.h5", save_best_only: true, monitor: "val_accuracy"));
                // 4. Treinar o modelo
                _logger?.LogInformation($"Iniciando treinamento com {xs.Length} amostras, {xs[0].shape[1]} features e {ys.Length} labels.");
                // *** Compile and train the model here ***
                _optimizer.LearningRate = initialLearningRate;
                model.Model.compile(optimizer: _optimizer, 
                                    loss: "binary_crossentropy",
                                    metrics: new[] { "accuracy"});

                await model.Model.fit(xs, ys, 
                                      batch_size: batchSize, 
                                      epochs: epochs, 
                                      validation_split: validationSplit, callbacks: callbacks.ToArray());
                _logger?.LogInformation("Treinamento concluído com sucesso.");

                // 5. Avaliar o modelo (opcional)
                var metrics = model.Model.evaluate(xs, ys, verbose: 0);
                _logger?.LogInformation($"Resultados da avaliação: Loss = {metrics[0]}, Accuracy = {metrics[1]}");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Erro durante o treinamento do modelo.");
                throw; // Re-throw para ser tratado em camadas superiores
            }
        }

        /// <summary>
        /// Converte uma lista de sequências de features para um array multidimensional NumPy (NDArray).
        /// </summary>
        private NDArray ConvertToNDArrays(List<float[][]> features, int sequenceLength)
        {
            // Assumindo que todas as sequências têm o mesmo tamanho
            int numSequences = features.Count;
            int featureSize = features[0][0].Length; // Tamanho do vetor de features (25)

            // Criar um NDArray vazio com a forma [num_sequences, sequence_length, feature_size]
            var result = np.zeros((numSequences, sequenceLength, featureSize));

            // Copiar os dados para o NDArray
            for (int i = 0; i < numSequences; i++)
            {
                for (int j = 0; j < sequenceLength; j++)
                {
                    result[i, j] = features[i][j];
                }
            }

            return result;
        }

        /// <summary>
        /// Converte uma lista de rótulos para um array NumPy (NDArray).
        /// </summary>
        private NDArray ConvertToNDArray(List<float[]> labels)
        {
            int numLabels = labels.Count;
            int labelSize = labels[0].Length; // Tamanho do vetor de rótulos (25)

            // Cria um NDArray vazio com a forma [num_labels, label_size]
            var result = np.zeros((numLabels, labelSize));

            // Copia os dados para o NDArray
            for (int i = 0; i < numLabels; i++)
            {
                result[i] = labels[i];
            }

            return result;
        }



    }
}