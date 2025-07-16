using LotoLibrary.Models.Core;
using LotoLibrary.PredictionModels.AntiFrequency.Simple;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Moq;
using System.Linq;
using System.Threading.Tasks;

namespace LotoLibrary.Tests.Unit.PredictionModels

{
    [TestClass]
    public class AntiFrequencySimpleModelTests
    {
        private AntiFrequencySimpleModel _model;
        private Mock<IAntiFrequencyProfileFactory> _mockProfileFactory;

        [TestInitialize]
        public void Setup()
        {
            // Mock do perfil e sua fábrica
            var mockProfile = new Mock<IAntiFrequencyProfile>();
            // Configurar um retorno padrão para GetLeastFrequent (poderia ser mais específico se necessário)
            mockProfile.Setup(p => p.GetLeastFrequent(It.IsAny<int>())).Returns(Enumerable.Range(1, 15).ToList());
            // Configurar o retorno para a confiança
            mockProfile.Setup(p => p.Confidence).Returns(0.65);

        _mockProfileFactory = new Mock<IAntiFrequencyProfileFactory>();
        _mockProfileFactory.Setup(f => f.Create(It.IsAny<Lances>())).Returns(mockProfile.Object);

            // Criar o modelo com o mock da fábrica
            _model = new AntiFrequencySimpleModel(_mockProfileFactory.Object);
        }

        [TestMethod]
        public async Task PredictAsync_WhenInitialized_ShouldReturn15LeastFrequentNumbers()
        {
            // Arrange
            var historicalData = CreateTestHistoricalData();
            await _model.InitializeAsync(historicalData); // Inicializa o modelo para este teste

            // Act
            var result = await _model.PredictAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(15, result.PredictedNumbers.Count);
            Assert.AreEqual(0.65, result.Confidence); // Verifica se a confiança é a do mock
        }

        [TestMethod]
        public async Task ValidateAsync_WhenInitialized_ShouldReturnSuccessfulValidation()
        {
            // Arrange
            var historicalData = CreateTestHistoricalData();
            await _model.InitializeAsync(historicalData);

            // Act
            var validationResult = await _model.ValidateAsync(historicalData);

            // Assert
            Assert.IsNotNull(validationResult);
            Assert.IsTrue(validationResult.Success, "A validação deve ser bem-sucedida.");
            Assert.IsTrue(validationResult.Accuracy >= 0, "A acurácia deve ser calculada.");
        }
    }
}