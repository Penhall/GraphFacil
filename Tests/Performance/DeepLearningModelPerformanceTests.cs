using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LotoLibrary.Engines;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.PredictionModels.Individual;

namespace Tests.Performance
{
    [TestClass]
    public class DeepLearningModelPerformanceTests
    {
        private Lances _historicalData;
        private IModelFactory _modelFactory;

        [TestInitialize]
        public void Setup()
        {
            _historicalData = CreateTestHistoricalData(3000);
            _modelFactory = new ModelFactory();
            _modelFactory.RegisterModel<DeepLearningModel>();
            _modelFactory.RegisterModel<MetronomoModel>();
            //  _modelFactory.RegisterModel<StatisticalDebtModel>(); // Requires parameters
        }

        [TestMethod]
        [TestCategory("Performance")]
        public async Task DeepLearningModel_PredictionPerformance_ShouldBeComparable()
        {
            // ARRANGE
            var deepLearningModel = (DeepLearningModel)_modelFactory.CreateModel(LotoLibrary.Enums.ModelType.DeepLearning);
            var metronomoModel = _modelFactory.CreateModel(LotoLibrary.Enums.ModelType.Metronomo);
            //var statisticalDebtModel = _modelFactory.CreateModel(LotoLibrary.Enums.ModelType.StatisticalDebt);

            // Initialize models
            await deepLearningModel.InitializeAsync(_historicalData);
            await metronomoModel.InitializeAsync(_historicalData);
            //await statisticalDebtModel.InitializeAsync(_historicalData);

            // ACT & ASSERT
            await MeasurePredictionPerformance(deepLearningModel);
            await MeasurePredictionPerformance(metronomoModel);
            //await MeasurePredictionPerformance(statisticalDebtModel);

            // You can add more specific assertions here, e.g.,
            // Assert.IsTrue(deepLearningTime < statisticalDebtTime * 1.2, "DeepLearningModel should be reasonably fast compared to StatisticalDebtModel.");
        }

        private async Task MeasurePredictionPerformance(IPredictionModel model)
        {
            var stopwatch = new Stopwatch();
            int numPredictions = 10;
            double totalConfidence = 0;

            for (int i = 0; i < numPredictions; i++)
            {
                stopwatch.Start();
                var prediction = await model.PredictAsync(3000 + i);
                stopwatch.Stop();
                totalConfidence += prediction.Confidence;
            }

            var averageTime = stopwatch.ElapsedMilliseconds / (double)numPredictions;
            var averageConfidence = totalConfidence / numPredictions;
            Console.WriteLine($"Model: {model.ModelName}, Average Prediction Time: {averageTime:F2} ms, Average Confidence: {averageConfidence:F4}");
        }

        private Lances CreateTestHistoricalData(int count)
        {
            var lances = Enumerable.Range(1, count).Select(i => new Lance { Id = 2000 + i }).ToList();
            return new Lances(lances);
        }
    }
}