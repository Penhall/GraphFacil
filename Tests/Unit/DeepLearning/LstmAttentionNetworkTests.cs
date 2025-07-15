using Microsoft.VisualStudio.TestTools.UnitTesting;
using LotoLibrary.DeepLearning.Architectures;
using Tensorflow.Keras.Layers;
using System.Linq;

namespace Tests.Unit.DeepLearning
{
    [TestClass]
    public class LstmAttentionNetworkTests
    {
        [TestMethod]
        [TestCategory("Unit")]
        public void BuildModel_ShouldCreateNetworkWithExpectedLayersAndShape()
        {
            // ARRANGE
            int sequenceLength = 60;
            int featureSize = 20;

            // ACT
            var network = new LstmAttentionNetwork(sequenceLength, featureSize);

            // ASSERT
            Assert.IsNotNull(network.Model, "Model should not be null.");

            // Check input layer shape
            var inputLayer = network.Model.Input;
            Assert.AreEqual((None, sequenceLength, featureSize), inputLayer.shape, "Input shape mismatch.");

            // Check LSTM layers
            var lstm1 = network.Model.GetLayer("lstm_1") as LSTM;
            Assert.IsNotNull(lstm1, "LSTM layer 1 not found.");
            Assert.IsTrue(lstm1.return_sequences, "LSTM 1 should return sequences.");

            var lstm2 = network.Model.GetLayer("lstm_2") as LSTM;
            Assert.IsNotNull(lstm2, "LSTM layer 2 not found.");
            Assert.IsTrue(lstm2.return_sequences, "LSTM 2 should return sequences.");

            // Check attention layer
            var attention = network.Model.GetLayer("attention");
            Assert.IsNotNull(attention, "Attention layer not found.");

            // Check output layer
            var outputLayer = network.Model.GetLayer("output_probabilities") as Dense;
            Assert.IsNotNull(outputLayer, "Output layer not found.");
            Assert.AreEqual(featureSize, outputLayer.units, "Output layer units mismatch.");
            Assert.AreEqual("sigmoid", outputLayer.Activation.Config.Name, "Output activation should be sigmoid.");
        }
    }
}
}