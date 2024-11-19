using System;
using Tensorflow;
using Tensorflow.NumPy;
using static Tensorflow.Binding;


namespace LotoLibrary.NeuralNetwork;

public class NeuralNetwork
{
    private int inputSize;
    private int hiddenLayerSize;
    private int outputSize;

    private Tensor x;
    private Tensor y;

    private Tensor weights1;
    private Tensor biases1;
    private Tensor weights2;
    private Tensor biases2;

    private Tensor logits;
    private Tensor loss;
    private Operation trainOp;

    public NeuralNetwork(int inputSize, int hiddenLayerSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenLayerSize = hiddenLayerSize;
        this.outputSize = outputSize;

        // Inicialização do Modelo
        BuildModel();
    }

    private void BuildModel()
    {
        tf.enable_eager_execution();

        // Placeholder para entrada (x) e saída esperada (y)
        x = tf.placeholder(TF_DataType.TF_FLOAT, shape: new int[] { -1, inputSize });
        y = tf.placeholder(TF_DataType.TF_FLOAT, shape: new int[] { -1, outputSize });

        // Camada Oculta 1
        weights1 = tf.Variable(tf.random.truncated_normal((inputSize, hiddenLayerSize)));
        biases1 = tf.Variable(tf.zeros(hiddenLayerSize));
        var hiddenLayer = tf.nn.relu(tf.matmul(x, weights1) + biases1);

        // Camada de Saída
        weights2 = tf.Variable(tf.random.truncated_normal((hiddenLayerSize, outputSize)));
        biases2 = tf.Variable(tf.zeros(outputSize));
        logits = tf.matmul(hiddenLayer, weights2) + biases2;

        // Definir função de perda e otimizador
        loss = tf.reduce_mean(tf.nn.softmax_cross_entropy_with_logits(labels: y, logits: logits));
        var optimizer = tf.train.AdamOptimizer(learning_rate: 0.01f);
        trainOp = optimizer.minimize(loss);
    }

    // Método para treinar a rede neural
    public void Train(NDArray inputs, NDArray labels, int epochs, int batchSize)
    {
        var sess = tf.Session();

        sess.run(tf.global_variables_initializer());

        for (int epoch = 0; epoch < epochs; epoch++)
        {
            var totalBatch = inputs.shape[0] / batchSize;

            for (int i = 0; i < totalBatch; i++)
            {
                var start = i * batchSize;
                var end = start + batchSize;

                var batchX = inputs[$"{start}:{end},:"];
                var batchY = labels[$"{start}:{end},:"];

                sess.run(trainOp, (x, batchX), (y, batchY));
            }

            var trainingLoss = sess.run(loss, (x, inputs), (y, labels));
            Console.WriteLine($"Epoch {epoch + 1}, Loss: {trainingLoss}");
        }
    }

    // Método para fazer previsões
    public NDArray Predict(NDArray inputs)
    {
        var sess = tf.Session();
        return sess.run(logits, (x, inputs));
    }
}