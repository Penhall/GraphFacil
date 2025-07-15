using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Dashboard.ViewModels.Specialized;
using LotoLibrary.Interfaces;
using LotoLibrary.Engines;
using LotoLibrary.Models;
using LotoLibrary.Enums;
using LotoLibrary.PredictionModels.AntiFrequency.Simple;
using System.Linq;

namespace Tests.Integration.ViewModels
{
    [TestClass]
    public class ConfigurationViewModelIntegrationTests
    {
        private Lances _historicalData;
        private IModelFactory _modelFactory;

        [TestInitialize]
        public void Setup()
        {
            _historicalData = CreateTestHistoricalData(100);
            _modelFactory = new ModelFactory();
            _modelFactory.RegisterModel<AntiFrequencySimpleModel>();
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task SelectConfigurableModel_ShouldLoadParametersCorrectly()
        {
            // ARRANGE
            var viewModel = new ConfigurationViewModel(_historicalData);
            var modelInfo = _modelFactory.GetModelInfo(ModelType.AntiFrequencySimple);

            // ACT
            viewModel.SelectedModel = modelInfo;
            await Task.Delay(100); // Allow binding to update

            // ASSERT
            Assert.IsNotNull(viewModel.ModelParameters, "ModelParameters should not be null.");
            Assert.IsTrue(viewModel.ModelParameters.Any(), "ModelParameters should not be empty.");

            // Check for specific parameters (adjust based on actual model)
            Assert.IsTrue(viewModel.ModelParameters.Any(p => p.Name == "Invertendo tendência" && p.Value is bool), "Parameter 'Invertendo tendência' not found or incorrect type.");
            Assert.IsTrue(viewModel.ModelParameters.Any(p => p.Name == "Janela de análise (concursos)" && p.Value is int), "Parameter 'Janela de análise (concursos)' not found or incorrect type.");
            Assert.IsTrue(viewModel.ModelParameters.Any(p => p.Name == "Frequência mínima (%)" && p.Value is double), "Parameter 'Frequência mínima (%)' not found or incorrect type.");
        }

        [TestMethod]
        [TestCategory("Integration")]
        public async Task ChangeAndSaveModelParameters_ShouldUpdateModelConfiguration()
        {
            // ARRANGE
            var viewModel = new ConfigurationViewModel(_historicalData);
            var modelInfo = _modelFactory.GetModelInfo(ModelType.AntiFrequencySimple);
            viewModel.SelectedModel = modelInfo;
            await Task.Delay(100);

            // Get the actual model instance from the factory
            var model = _modelFactory.CreateModel(ModelType.AntiFrequencySimple) as IConfigurableModel;
            Assert.IsNotNull(model, "Model should be IConfigurableModel.");

            // ACT
            // Modify a parameter
            var janelaAnaliseParam = viewModel.ModelParameters.First(p => p.Name == "Janela de análise (concursos)");
            janelaAnaliseParam.Value = 150;

            // Save the configuration
            await viewModel.SaveModelConfigurationCommand.ExecuteAsync(null);

            // Get the updated model parameters
            var updatedParameters = model.CurrentParameters;

            // ASSERT
            // Check if the parameter was updated in the model
            Assert.AreEqual(150, updatedParameters["Janela de análise (concursos)"], "Parameter 'Janela de análise (concursos)' should be updated.");
        }

        private Lances CreateTestHistoricalData(int count)
        {
            var random = new System.Random();
            var lances = Enumerable.Range(1, count)
                .Select(i => new Lance
                {
                    Id = 2000 + i,
                    Data = System.DateTime.Now.AddDays(-i),
                    Lista = Enumerable.Range(1, 25).OrderBy(n => random.Next()).Take(15).OrderBy(n => n).ToList()
                }).ToList();
            return new Lances(lances);
        }
    }
}