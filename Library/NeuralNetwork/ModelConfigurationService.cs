using LotoLibrary.Interfaces;
using Microsoft.ML.Trainers.FastTree;
using System;
using System.IO;
using System.Text.Json;

namespace LotoLibrary.NeuralNetwork;

public class ModelConfigurationService
{
    private readonly IMLLogger _logger;
    private readonly string _configPath = "model_config.json";

    public class ModelConfiguration
    {
        public int NumberOfTrees { get; set; } = 100;
        public int NumberOfLeaves { get; set; } = 20;
        public int MinimumExampleCountPerLeaf { get; set; } = 10;
        public float LearningRate { get; set; } = 0.001f;
        public bool UseNormalization { get; set; } = true;
        public int CrossValidationFolds { get; set; } = 5;
        public float ValidationSplit { get; set; } = 0.2f;
    }

    public ModelConfigurationService(IMLLogger logger)
    {
        _logger = logger;
    }

    public FastForestRegressionTrainer.Options GetTrainerOptions(ModelConfiguration config)
    {
        return new FastForestRegressionTrainer.Options
        {
            NumberOfTrees = config.NumberOfTrees,
            NumberOfLeaves = config.NumberOfLeaves,
            MinimumExampleCountPerLeaf = config.MinimumExampleCountPerLeaf
        };
    }

    public void SaveConfiguration(ModelConfiguration config)
    {
        try
        {
            var jsonString = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_configPath, jsonString);
            _logger.LogInformation($"Configuração salva em: {_configPath}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao salvar configuração: {ex.Message}", ex);
            throw;
        }
    }

    public ModelConfiguration LoadConfiguration()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                var defaultConfig = new ModelConfiguration();
                SaveConfiguration(defaultConfig);
                return defaultConfig;
            }

            var jsonString = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<ModelConfiguration>(jsonString);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao carregar configuração: {ex.Message}", ex);
            throw;
        }
    }
}