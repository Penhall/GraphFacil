using LotoLibrary.Infrastructure.Validation;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Services;
using Microsoft.ML;
using Microsoft.ML.Trainers.FastTree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.NeuralNetwork.Services;

public class PreparacaoService
{
    private readonly MLContext _mlContext;
    private readonly IMLLogger _logger;
    private readonly FileService _fileService;

    public PreparacaoService(IMLLogger logger, FileService fileService)
    {
        _mlContext = new MLContext(seed: 42);
        _logger = logger;
        _fileService = fileService;
    }

    public IDataView PrepararDados(string caminhoPercentuais)
    {
        try
        {
            _logger.LogInformation($"Iniciando preparação dos dados do arquivo: {caminhoPercentuais}");

            var percentuais = _fileService.CarregarDados<Dictionary<int, Dictionary<int, double>>>(caminhoPercentuais);
            var dados = ConverterParaPercentualInput(percentuais);

            DataValidator.ValidateInputCollection(dados);

            var dataView = _mlContext.Data.LoadFromEnumerable(dados);

            _logger.LogInformation($"Dados preparados com sucesso. Total de registros: {dados.Count()}");

            return dataView;
        }
        catch (Exception ex)
        {
            _logger.LogError("Erro ao preparar dados", ex);
            throw;
        }
    }

    private IEnumerable<PercentualInput> ConverterParaPercentualInput(Dictionary<int, Dictionary<int, double>> percentuais)
    {
        return percentuais.Select(p => new PercentualInput
        {
            Dimensoes = p.Value.Values.Select(v => (float)v).ToArray()
        });
    }
}
