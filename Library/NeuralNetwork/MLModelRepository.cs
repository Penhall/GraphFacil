using Microsoft.ML;
using LotoLibrary.Interfaces;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Threading;
using LotoLibrary.Infrastructure;

namespace LotoLibrary.NeuralNetwork;

public class MLModelRepository : IMLModelRepository
{
    private readonly FileService _fileService;
    private readonly IMLLogger _logger;
    private readonly string _pathPercentuaisSS = "PercentuaisSS.json";
    private readonly string _pathPercentuaisNS = "PercentuaisNS.json";
    private readonly string _pathModeloSS = "ModeloSS.zip";
    private readonly string _pathModeloNS = "ModeloNS.zip";

    public MLModelRepository(FileService fileService, IMLLogger logger)
    {
        _fileService = fileService;
        _logger = logger;

        // Garantir cultura invariant para operações numéricas
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }

    public Dictionary<int, Dictionary<int, double>> CarregarPercentuaisSS()
    {
        try
        {
            return _fileService.CarregarDados<Dictionary<int, Dictionary<int, double>>>(_pathPercentuaisSS)
                ?? new Dictionary<int, Dictionary<int, double>>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar percentuais SS: {ex.Message}", ex);
            throw;
        }
    }

    public Dictionary<int, Dictionary<int, double>> CarregarPercentuaisNS()
    {
        try
        {
            return _fileService.CarregarDados<Dictionary<int, Dictionary<int, double>>>(_pathPercentuaisNS)
                ?? new Dictionary<int, Dictionary<int, double>>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar percentuais NS: {ex.Message}", ex);
            throw;
        }
    }

    public void SalvarPercentuaisSS(Dictionary<int, Dictionary<int, double>> percentuais)
    {
        try
        {
            _fileService.SalvarDados(_pathPercentuaisSS, percentuais);
            _logger.LogInformation("Percentuais SS salvos com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao salvar percentuais SS: {ex.Message}", ex);
            throw;
        }
    }

    public void SalvarPercentuaisNS(Dictionary<int, Dictionary<int, double>> percentuais)
    {
        try
        {
            _fileService.SalvarDados(_pathPercentuaisNS, percentuais);
            _logger.LogInformation("Percentuais NS salvos com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao salvar percentuais NS: {ex.Message}", ex);
            throw;
        }
    }

    public void SalvarModelo(string tipo, ITransformer model, DataViewSchema schema)
    {
        try
        {
            string path = tipo.ToUpper() == "SS" ? _pathModeloSS : _pathModeloNS;
            var mlContext = new MLContext();
            mlContext.Model.Save(model, schema, path);
            _logger.LogInformation($"Modelo {tipo} salvo com sucesso em: {path}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao salvar modelo {tipo}: {ex.Message}", ex);
            throw;
        }
    }

    public ITransformer CarregarModelo(string tipo, out DataViewSchema schema)
    {
        try
        {
            string path = tipo.ToUpper() == "SS" ? _pathModeloSS : _pathModeloNS;
            var mlContext = new MLContext();
            var model = mlContext.Model.Load(path, out schema);
            _logger.LogInformation($"Modelo {tipo} carregado com sucesso de: {path}");
            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar modelo {tipo}: {ex.Message}", ex);
            throw;
        }
    }
}
