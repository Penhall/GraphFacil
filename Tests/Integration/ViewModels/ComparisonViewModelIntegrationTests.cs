using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using LotoLibrary.Models;
using Dashboard.ViewModels.Specialized;
using System.Linq;
using LotoLibrary.Enums;
using LotoLibrary.Engines;
using LotoLibrary.PredictionModels.Individual;
using System.Collections.Generic;
using LotoLibrary.PredictionModels.AntiFrequency.Simple;
using LotoLibrary.Interfaces;

namespace Tests.Integration.ViewModels
{
    // Integration test to verify ComparisonViewModel with real ModelFactory and models
    [TestClass]
    public class ComparisonViewModelIntegrationTests
    {
        private Lances _historicalData;
        private IModelFactory _modelFactory; 

        [TestInitialize]
        public void Setup()
        {
            _historicalData = CreateTestHistoricalData(200);

            // Use a real ModelFactory for the integration test
            _modelFactory = new ModelFactory();

            // Register MetronomoModel and AntiFrequencySimpleModel in the factory
            _modelFactory.RegisterModel<MetronomoModel>();
            _modelFactory.RegisterModel<AntiFrequencySimpleModel>();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task CompareSelectedModels_WithTwoRealModels_ShouldGenerateValidComparison()
        {
            // ARRANGE
            // The ViewModel uses the same factory logic, ensuring consistency.
            var viewModel = new ComparisonViewModel(_historicalData) 
            { 
                TargetConcursoForComparison = "3001" 
            };

            viewModel.AddModelToComparisonCommand.Execute(ModelType.Metronomo);
            viewModel.AddModelToComparisonCommand.Execute(ModelType.AntiFrequencySimple);

            // ACT
            await viewModel.CompareSelectedModelsCommand.ExecuteAsync(null);

            // ASSERT
            Assert.AreEqual(1, viewModel.Comparisons.Count, "Exactly one comparison result should be generated.");

            var comparison = viewModel.Comparisons.First();
            var expectedNames = new List<string> { "Metronomo", "Anti-Frequência Simples" };

            Assert.IsTrue(expectedNames.Contains(comparison.Model1Name) && expectedNames.Contains(comparison.Model2Name),
                          "Model names in comparison should match selected models.");
            Assert.AreNotEqual(comparison.Model1Name, comparison.Model2Name, "The two compared models should be different.");

            Assert.AreEqual(3001, comparison.TargetConcurso, "Target concurso in the result should match the input.");
            Assert.IsTrue(comparison.Model1Confidence > 0, "Model 1 confidence should be greater than zero.");
            Assert.IsTrue(comparison.Model2Confidence > 0, "Model 2 confidence should be greater than zero.");
            Assert.IsTrue(comparison.CommonNumbers >= 0 && comparison.CommonNumbers <= 15, "Common numbers count should be valid.");
            Assert.IsTrue(comparison.DiversificationRate >= 0.0 && comparison.DiversificationRate <= 1.0, "Diversification rate should be between 0 and 1.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(comparison.Model1FormattedNumbers), "Formatted numbers for Model 1 should not be empty.");
            Assert.IsFalse(string.IsNullOrWhiteSpace(comparison.Model2FormattedNumbers), "Formatted numbers for Model 2 should not be empty.");
        }

        private Lances CreateTestHistoricalData(int count)
        {
            var random = new Random();
            var lances = Enumerable.Range(1, count).Select(i => new Lance { Id = 2000 + i, Data = DateTime.Now.AddDays(-i), Lista = Enumerable.Range(1, 25).OrderBy(n => random.Next()).Take(15).OrderBy(n => n).ToList() }).ToList();
            return new Lances(lances);
        }
    }
}