// D:\PROJETOS\GraphFacil\Library\DeepLearning\Training\ModelTrainer.cs
using System;
using System.Collections.Generic;
using LotoLibrary.DeepLearning.Architectures;

namespace LotoLibrary.DeepLearning.Training
{
    public class ModelTrainer
    {
        private LstmAttentionNetwork _model;
        private TrainingConfiguration _config;
        private bool _isTraining;

        public bool IsTraining => _isTraining;
        public TrainingConfiguration Configuration => _config;
        public LstmAttentionNetwork Model => _model;

        public ModelTrainer()
        {
            _config = new TrainingConfiguration();
            _isTraining = false;
        }

        public ModelTrainer(TrainingConfiguration config)
        {
            _config = config ?? new TrainingConfiguration();
            _isTraining = false;
        }

        public bool SetupModel(int sequenceLength, int featureSize)
        {
            try
            {
                _model = new LstmAttentionNetwork(sequenceLength, featureSize);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public TrainingResult TrainModel(float[,] trainingData, float[,] targetData)
        {
            if (_model == null) return new TrainingResult { Success = false, ErrorMessage = "Modelo não configurado" };
            
            _isTraining = true;
            var result = new TrainingResult();
            
            try
            {
                bool trainSuccess = _model.Train(trainingData, targetData, _config.Epochs);
                result.Success = trainSuccess;
                result.TrainingDuration = TimeSpan.FromSeconds(2); // Mock
                result.FinalAccuracy = 0.75; // Mock
                result.EpochsCompleted = _config.Epochs;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            finally
            {
                _isTraining = false;
            }
            
            return result;
        }
    }

    public class TrainingConfiguration
    {
        public int Epochs { get; set; } = 100;
        public double LearningRate { get; set; } = 0.001;
        public int BatchSize { get; set; } = 32;
        public double ValidationSplit { get; set; } = 0.2;
        public bool EarlyStopping { get; set; } = true;
        public int Patience { get; set; } = 10;
    }

    public class TrainingResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public TimeSpan TrainingDuration { get; set; }
        public double FinalAccuracy { get; set; }
        public int EpochsCompleted { get; set; }
    }
}
