using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using LotoLibrary.Models;
using Dashboard.ViewModels.Specialized;
using System.Linq;
using System;
using System.Collections.Generic;
using LotoLibrary.Services.Validation;

namespace Tests.Integration.ViewModels
{
    /// <summary>
    /// Testa a integração do ValidationViewModel com os serviços reais de backend.
    /// Garante que a camada de apresentação (ViewModel) está se comunicando
    /// corretamente com a camada de lógica de negócios (Services).
    /// </summary>
    [TestClass]
    public class ValidationViewModelIntegrationTests
    {
        private Lances _historicalData;

        [TestInitialize]
        public void Setup()
        {
            // ARRANGE: Cria um conjunto de dados históricos realístico para os testes.
            _historicalData = CreateTestHistoricalData();
        }

        /// <summary>
        /// Este teste verifica o fluxo completo:
        /// 1. O usuário clica no botão "Validação Rápida" (acionando o comando).
        /// 2. O ViewModel invoca o serviço de validação real do backend (`Phase1ValidationService`).
        /// 3. O serviço executa sua suíte de testes.
        /// 4. O ViewModel recebe os resultados e popula suas propriedades observáveis.
        /// </summary>
        [TestMethod]
        [TestCategory("Integration")]
        public async Task RunQuickValidation_ExecutesRealBackendSuite_AndPopulatesResultsCorrectly()
        {
            // ARRANGE
            // 1. Instancia o ViewModel. Internamente, ele irá criar uma instância real do `Phase1ValidationService`.
            var viewModel = new ValidationViewModel(_historicalData);

            // 2. Para uma asserção precisa, obtemos o número esperado de testes diretamente do serviço de backend.
            var realValidationService = new Phase1ValidationService();
            var expectedTestCount = realValidationService.GetValidationSuite().Count;
            Assert.IsTrue(expectedTestCount > 0, "O serviço de validação do backend deve ter pelo menos um teste.");

            // ACT
            // Executa o comando, simulando o clique do usuário na interface.
            await viewModel.RunQuickValidationCommand.ExecuteAsync(null);

            // ASSERT
            // 1. O processo de validação deve ter sido concluído.
            Assert.IsFalse(viewModel.IsValidationRunning, "A flag 'IsValidationRunning' deveria ser falsa após a conclusão.");

            // 2. A coleção de resultados deve ser preenchida com o número exato de testes do backend.
            Assert.IsNotNull(viewModel.ValidationResults, "A coleção de resultados não pode ser nula.");
            Assert.AreEqual(expectedTestCount, viewModel.ValidationResults.Count, "O número de resultados no ViewModel deve ser igual ao número de testes na suíte do backend.");

            // 3. As propriedades de resumo devem ser atualizadas.
            Assert.AreNotEqual("Nenhuma validação executada", viewModel.LastValidationSummary, "O sumário da validação deveria ter sido atualizado.");
            Assert.IsTrue(viewModel.OverallAccuracy >= 0, "A acurácia geral deveria ter sido calculada."); // Pode ser 0 se todos os testes falharem.
            Assert.AreEqual(100, viewModel.ValidationProgress, "O progresso deveria ser 100% ao final.");

            // 4. Verifica a integridade de um dos resultados para garantir que os dados foram mapeados corretamente.
            var firstResult = viewModel.ValidationResults.First();
            Assert.IsFalse(string.IsNullOrWhiteSpace(firstResult.TestName), "O nome do teste (TestName) não pode estar vazio.");
            Assert.IsTrue(firstResult.Status.Contains("✅") || firstResult.Status.Contains("❌"), "O status deve indicar sucesso (✅) ou falha (❌).");
            Assert.IsFalse(string.IsNullOrWhiteSpace(firstResult.Details), "Os detalhes do resultado não podem estar vazios.");
        }

        private Lances CreateTestHistoricalData(int count = 200)
        {
            var lances = new Lances();
            var random = new Random();
            for (int i = 1; i <= count; i++)
            {
                lances.Add(new Lance { Id = 2000 + i, Data = DateTime.Now.AddDays(-i), Lista = Enumerable.Range(1, 25).OrderBy(n => random.Next()).Take(15).OrderBy(n => n).ToList() });
            }
            return lances;
        }
    }
}