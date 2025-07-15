using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Threading.Tasks;
using LotoLibrary.Models;
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Interfaces;
using System.Collections.Generic;
using LotoLibrary.Services.Validation;
using System.Linq;
using System;

namespace Tests.Unit.ViewModels
{
    [TestClass]
    public class ValidationViewModelTests
    {
        private Mock<IValidationService> _mockValidationService;
        private Mock<IModelFactory> _mockModelFactory;
        private ValidationViewModel _viewModel;
        private Lances _testData;

        [TestInitialize]
        public void Setup()
        {
            _mockValidationService = new Mock<IValidationService>();
            _mockModelFactory = new Mock<IModelFactory>();
            _testData = CreateTestHistoricalData();

            // Setup mock validation service to return a predefined suite
            var mockValidationSuite = new List<ITestCase>
            {
                CreateMockTestCase("Test Case 1", true),
                CreateMockTestCase("Test Case 2", false, true)
            };
            _mockValidationService.Setup(s => s.GetValidationSuite()).Returns(mockValidationSuite);

            // Initialize ViewModel with mocked services
            _viewModel = new ValidationViewModel(_testData, _mockValidationService.Object, _mockModelFactory.Object);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task RunQuickValidation_WithMockedService_ShouldPopulateResultsCorrectly()
        {
            // ACT
            await _viewModel.RunQuickValidationCommand.ExecuteAsync(null);

            // ASSERT
            Assert.IsFalse(_viewModel.IsValidationRunning, "IsValidationRunning should be false after completion.");
            Assert.AreEqual(2, _viewModel.ValidationResults.Count, "Should have two validation results.");

            // Verify results of first test case
            var firstResult = _viewModel.ValidationResults.First();
            Assert.AreEqual("Test Case 1", firstResult.TestName);
            Assert.AreEqual("✅ Passou", firstResult.Status);
            Assert.AreEqual(100, firstResult.Accuracy); // Mocked to always pass
            Assert.AreEqual("Mocked test details", firstResult.Details);

            // Verify results of second test case
            var secondResult = _viewModel.ValidationResults.Last();
            Assert.AreEqual("Test Case 2", secondResult.TestName);
            Assert.AreEqual("❌ Falhou", secondResult.Status);
            Assert.AreEqual(0, secondResult.Accuracy); // Mocked to always fail
            Assert.AreEqual("Mocked test details", secondResult.Details);
        }

        [TestMethod]
        [TestCategory("Unit")]
        public async Task RunQuickValidation_WithMockedService_ShouldCalculateOverallAccuracyCorrectly()
        {
            // ACT
            await _viewModel.RunQuickValidationCommand.ExecuteAsync(null);

            // ASSERT
            // Expected accuracy: (100 + 0) / 2 = 50
            Assert.AreEqual(50.0, _viewModel.OverallAccuracy, "Overall accuracy should be calculated correctly.");
        }

        [TestMethod]
        [TestCategory("Unit")]
        public void ClearValidationResults_ShouldClearAllResultsAndResetProperties()
        {
            // ARRANGE
            _viewModel.ValidationResults.Add(new ValidationResult { TestName = "Initial Result" });
            _viewModel.LastValidationSummary = "Initial Summary";
            _viewModel.OverallAccuracy = 75.0;
            _viewModel.ValidationProgress = 50;

            // ACT
            _viewModel.ClearValidationResultsCommand.Execute(null);

            // ASSERT
            Assert.AreEqual(0, _viewModel.ValidationResults.Count, "ValidationResults should be empty.");
            Assert.AreEqual("Resultados de validação limpos", _viewModel.LastValidationSummary, "LastValidationSummary should be reset.");
            Assert.AreEqual(0.0, _viewModel.OverallAccuracy, "OverallAccuracy should be reset.");
            Assert.AreEqual(0, _viewModel.ValidationProgress, "ValidationProgress should be reset.");
        }

        // Helper method to create mock test cases
        private ITestCase CreateMockTestCase(string name, bool success, bool isCritical = false)
        {
            var mockTestCase = new Mock<ITestCase>();
            mockTestCase.Setup(t => t.Name).Returns(name);
            mockTestCase.Setup(t => t.IsCritical).Returns(isCritical);
            mockTestCase.Setup(t => t.ExecuteAsync(It.IsAny<Lances>()))
                        .ReturnsAsync(new LotoLibrary.Models.Validation.TestResult { TestName = name, Success = success, Details = "Mocked test details" });
            return mockTestCase.Object;
        }

        private Lances CreateTestHistoricalData(int count = 100)
        {
            var lances = new Lances();
            for (int i = 0; i < count; i++)
                lances.Add(new Lance()); // simplified for unit tests
            return lances;
        }
    }
}