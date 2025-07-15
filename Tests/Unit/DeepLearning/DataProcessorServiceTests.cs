using Microsoft.VisualStudio.TestTools.UnitTesting;
using LotoLibrary.DeepLearning.DataProcessing;
using LotoLibrary.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Unit.DeepLearning
{
    [TestClass]
    public class DataProcessorServiceTests
    {
        private DataProcessorService _service;

        [TestInitialize]
        public void Setup()
        {
            _service = new DataProcessorService();
        }

        #region MultiHotEncode Tests

        [TestMethod]
        [TestCategory("Unit")]
        public void MultiHotEncode_WithValidLance_ReturnsCorrectlyEncodedVector()
        {
            // ARRANGE
            var lance = new Lance { Lista = new List<int> { 1, 5, 15, 25 } };
            var expected = new float[25];
            expected[0] = 1.0f;  // Index 0 for number 1
            expected[4] = 1.0f;  // Index 4 for number 5
            expected[14] = 1.0f; // Index 14 for number 15
            expected[24] = 1.0f; // Index 24 for number 25

            // ACT
            var result = _service.MultiHotEncode(lance);

            // ASSERT
            Assert.IsNotNull(result);
            Assert.AreEqual(25, result.Length);
            CollectionAssert.AreEqual(expected, result, "The encoded vector does not match the expected output.");
            Assert.AreEqual(4, result.Sum(), "The sum of the vector should be equal to the count of valid numbers.");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MultiHotEncode_WithNullLance_ReturnsZeroVector()
        {
            // ARRANGE
            Lance lance = null;
            var expected = new float[25]; // Array of zeros

            // ACT
            var result = _service.MultiHotEncode(lance);

            // ASSERT
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void MultiHotEncode_WithEmptyNumberList_ReturnsZeroVector()
        {
            // ARRANGE
            var lance = new Lance { Lista = new List<int>() };
            var expected = new float[25];

            // ACT
            var result = _service.MultiHotEncode(lance);

            // ASSERT
            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(expected, result);
        }

        #endregion

        #region CreateSequences Tests

        [TestMethod]
        [TestCategory("Unit")]
        public void CreateSequences_WithSufficientData_CreatesCorrectNumberOfSequences()
        {
            // ARRANGE
            var historicalData = CreateTestLances(10);
            int sequenceLength = 3;
            // Expected sequences: (0,1,2)->3, (1,2,3)->4, (2,3,4)->5, (3,4,5)->6, (4,5,6)->7, (5,6,7)->8, (6,7,8)->9
            int expectedCount = 10 - sequenceLength;

            // ACT
            var (features, labels) = _service.CreateSequences(historicalData, sequenceLength);

            // ASSERT
            Assert.AreEqual(expectedCount, features.Count, "The number of feature sequences is incorrect.");
            Assert.AreEqual(expectedCount, labels.Count, "The number of label sequences is incorrect.");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CreateSequences_WithSufficientData_FirstSequenceAndLabelAreCorrect()
        {
            // ARRANGE
            var historicalData = CreateTestLances(5); // Creates lances with Id 1 to 5
            int sequenceLength = 2;

            // ACT
            var (features, labels) = _service.CreateSequences(historicalData, sequenceLength);

            // ASSERT
            // First feature sequence should be based on lances with Id 1 and 2
            var firstFeatureSequence = features.First();
            Assert.AreEqual(sequenceLength, firstFeatureSequence.Length);
            CollectionAssert.AreEqual(_service.MultiHotEncode(historicalData.First(l => l.Id == 1)), firstFeatureSequence[0]);
            CollectionAssert.AreEqual(_service.MultiHotEncode(historicalData.First(l => l.Id == 2)), firstFeatureSequence[1]);

            // First label should be based on lance with Id 3
            var firstLabel = labels.First();
            CollectionAssert.AreEqual(_service.MultiHotEncode(historicalData.First(l => l.Id == 3)), firstLabel);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void CreateSequences_WithInsufficientData_ReturnsEmptyLists()
        {
            // ARRANGE
            var historicalData = CreateTestLances(5);
            int sequenceLength = 5; // Exactly the same as data count, not enough to create a label.

            // ACT
            var (features, labels) = _service.CreateSequences(historicalData, sequenceLength);

            // ASSERT
            Assert.IsFalse(features.Any(), "Features list should be empty.");
            Assert.IsFalse(labels.Any(), "Labels list should be empty.");
        }

        #endregion

        private Lances CreateTestLances(int count)
        {
            var lances = new Lances();
            for (int i = 1; i <= count; i++)
            {
                lances.Add(new Lance { Id = i, Lista = new List<int> { i, i + 1, i + 2 } });
            }
            return lances;
        }
    }
}