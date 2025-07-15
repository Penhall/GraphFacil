using Tensorflow;
using Tensorflow.Keras;
using Tensorflow.Keras.Layers;
using System;
using static Tensorflow.Binding;
using System.Collections.Generic;
using Tensorflow.Keras.Utils;
using Tensorflow.Common.Types;

namespace LotoLibrary.DeepLearning.Architectures
{
    /// <summary>
    /// Implementa uma rede neural LSTM com mecanismo de atenção para predição de sequências.
    /// </summary>
    public class LstmAttentionNetwork
    {
        public Functional Model { get; private set; }
        private int _sequenceLength;
        private int _featureSize;

        public LstmAttentionNetwork(int sequenceLength = 50, int featureSize = 25)
        {
            _sequenceLength = sequenceLength;
            _featureSize = featureSize;
            BuildModel();
        }

        private void BuildModel()
        {
            // Input layer
            var inputs = keras.Input(shape: (_sequenceLength, _featureSize), name: "input_sequence");

            // LSTM layer(s)
            var lstm1 = new LSTM(64, return_sequences: true, name: "lstm_1").Apply(inputs);
            var lstm2 = new LSTM(32, return_sequences: true, name: "lstm_2").Apply(lstm1); // Added second LSTM layer

            // Attention mechanism
            var attention = new AttentionLayer(name: "attention").Apply(lstm2);
            var contextVector = new Lambda<ITensor>(x => tf.reduce_sum(x, axis: 1), name: "context_vector").Apply(attention);

            // Output layer
            var outputs = new Dense(_featureSize, activation: "sigmoid", name: "output_probabilities").Apply(contextVector);

            // Define the model
            Model = keras.Model(inputs: inputs, outputs: outputs);
        }

        /// <summary>
        /// Carrega os pesos do modelo a partir de um arquivo.
        /// </summary>
        /// <param name="path">Caminho para o arquivo de pesos.</param>
        public void Load(string path)
        {
            if (Model == null)
            {
                throw new InvalidOperationException("O modelo ainda não foi construído. Chame BuildModel() primeiro.");
            }

            Model.load_weights(path);
        }

        /// <summary>
        /// Salva os pesos do modelo em um arquivo.
        /// </summary>
        /// <param name="path">Caminho para salvar o arquivo de pesos.</param>
        public void Save(string path)
        {
            if (Model == null)
            {
                throw new InvalidOperationException("O modelo ainda não foi construído. Chame BuildModel() primeiro.");
            }

            Model.save_weights(path);
        }

        /// <summary>
        /// Realiza uma predição com o modelo.
        /// </summary>
        /// <param name="sequence">Sequência de entrada.</param>
        /// <returns>Array de probabilidades de predição.</returns>
        public float[] Predict(float[][] sequence)
        {
            // Converte a sequência para um tensor com a forma [1, sequence_length, feature_size]
            var inputTensor = np.expand_dims(np.array(sequence), axis: 0);

            // Realiza a predição
            var prediction = Model.predict(inputTensor);

            // Retorna o resultado como um array de floats
            return prediction.numpy()[0] as float[];
        }
    }
}