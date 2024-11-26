using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork.Services;
using Microsoft.ML.Trainers.FastTree;
using System;
using System.Threading.Tasks;

namespace LotoLibrary.NeuralNetwork;

public class ModelOrchestrator
{
    private readonly PreparacaoService _preparacaoService;
    private readonly ModelTrainingService _trainingService;
    private readonly CrossValidationService _cvService;
    private readonly IMLLogger _logger;

    public ModelOrchestrator(
        PreparacaoService preparacaoService,
        ModelTrainingService trainingService,
        CrossValidationService cvService,
        IMLLogger logger)
    {
        _preparacaoService = preparacaoService;
        _trainingService = trainingService;
        _cvService = cvService;
        _logger = logger;
    }

    public async Task ExecutarTreinamentoCompleto(string caminhoPercentuais, bool usarCrossValidation = true)
    {
        try
        {
            _logger.LogInformation("Iniciando processo completo de treinamento...");

            // Preparar dados
            var dados = _preparacaoService.PrepararDados(caminhoPercentuais);

            // Realizar cross-validation se solicitado
            if (usarCrossValidation)
            {
                _cvService.PerformCrossValidation(dados);
            }

            // Configurar opções de treinamento
            var options = new FastForestRegressionTrainer.Options
            {
                NumberOfTrees = 100,
                NumberOfLeaves = 20,
                MinimumExampleCountPerLeaf = 10
            };

            // Treinar modelo final
            _trainingService.TrainModel(dados, options);

            // Avaliar modelo
            _trainingService.EvaluateModel(dados);

            _logger.LogInformation("Processo de treinamento concluído com sucesso!");
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro durante o processo de treinamento", ex);
            throw;
        }
    }

    public float RealizarPredicao(PercentualInput input)
    {
        try
        {
            return _trainingService.Predict(input);
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao realizar predição", ex);
            throw;
        }
    }
}