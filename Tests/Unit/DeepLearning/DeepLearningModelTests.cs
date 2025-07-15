using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using LotoLibrary.PredictionModels.Individual;
using LotoLibrary.DeepLearning.DataProcessing;
using LotoLibrary.DeepLearning.Architectures;
using LotoLibrary.DeepLearning.Training;
using LotoLibrary.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Tests.Unit.DeepLearning
{
    [TestClass]
    public class DeepLearningModelTests
    {
        private Mock<DataProcessorService> _mockDataProcessor;
        private Mock<LstmAttentionNetwork> _mockNeuralNetwork;
        private Mock<ModelTrainer> _mockTrainer;
        private DeepLearningModel _model;
        private Lances _historicalData;

        [TestInitialize]
        public void Setup()
        {
            // Mocks for all dependencies
            _mockDataProcessor = new Mock<DataProcessorService>();
            _mockNeuralNetwork = new Mock<LstmAttentionNetwork>();
            _mockTrainer = new Mock<ModelTrainer>();

            // Create sample historical data
            _historicalData = new Lances();
            for (int i = 1; i <= 60; i++)
            {
                _historicalData.Add(new Lance { Id = i, Lista = Enumerable.Range(1, 15).ToList() });
            }

            // Instantiate the model with mocked services
            _model = new DeepLearningModel(
                _mockDataProcessor.Object,
                _mockNeuralNetwork.Object,
                _mockTrainer.Object
            );

            // Initialize the model with the base method to populate HistoricalData
            _model.InitializeAsync(_historicalData);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task PredictAsync_WhenInitialized_ShouldReturnCorrectPrediction()
        {
            // ARRANGE
            // Setup the data processor to return a specific sequence
            var fakeSequence = new float[49][]; // sequenceLength - 1
            _mockDataProcessor
                .Setup(p => p.CreateSequences(It.IsAny<Lances>(), It.IsAny<int>()))
                .Returns((new List<float[][]> { fakeSequence }, new List<float[]>()));

            // Setup the neural network to return a specific probability distribution
            var fakeProbabilities = Enumerable.Range(1, 25).Select(i => 1.0f / i).ToArray();
            _mockNeuralNetwork
                .Setup(n => n.Predict(It.IsAny<float[][]>()))
                .Returns(fakeProbabilities);

            // Set the SequenceLength parameter for prediction
            _model.CurrentParameters["SequenceLength"] = 50;

            // ACT
            var result = await _model.PredictAsync(101);

            // ASSERT
            // Verify that dependencies were called
            _mockDataProcessor.Verify(p => p.CreateSequences(It.IsAny<Lances>(), 49), Times.Once);
            _mockNeuralNetwork.Verify(n => n.Predict(fakeSequence), Times.Once);

            // Verify the result
            Assert.IsNotNull(result);
            Assert.AreEqual("Deep Learning (LSTM)", result.ModelName);
            Assert.AreEqual(15, result.PredictedNumbers.Count, "Should predict exactly 15 numbers.");
            Assert.IsTrue(result.Confidence > 0, "Confidence should be greater than zero.");
            // The highest probability numbers are 1, 2, 3...
            CollectionAssert.AreEqual(Enumerable.Range(1, 15).ToList(), result.PredictedNumbers);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task TrainAsync_ShouldCallTrainerAndSaveFolder()
        {
            // ARRANGE
            _mockTrainer
                .Setup(t => t.TrainAsync(It.IsAny<LstmAttentionNetwork>(), It.IsAny<Lances>(), It.IsAny<DataProcessorService>()))
                .Returns(Task.CompletedTask);

            // ACT
            var success = await _model.TrainAsync(_historicalData);

            // ASSERT
            Assert.IsTrue(success, "Training should be reported as successful.");
            _mockTrainer.Verify(t => t.TrainAsync(_mockNeuralNetwork.Object, _historicalData, _mockDataProcessor.Object), Times.Once);
            _mockNeuralNetwork.Verify(n => n.Save(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task TrainAsync_WhenTrainerFails_ShouldReturnFalse()
        {
            // ARRANGE
            _mockTrainer
                .Setup(t => t.TrainAsync(It.IsAny<LstmAttentionNetwork>(), It.IsAny<Lances>(), It.IsAny<DataProcessorService>()))
                .ThrowsAsync(new Exception("Training failed!"));

            // ACT
            var success = await _model.TrainAsync(_historicalData);

            // ASSERT
            Assert.IsFalse(success, "Training should be reported as failed.");
            _mockNeuralNetwork.Verify(n => n.Save(It.IsAny<string>()), Times.Never, "Save should not be called if training fails.");
        }
    }
}