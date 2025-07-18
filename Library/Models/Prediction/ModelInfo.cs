// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ModelInfo.cs
using System.Collections.Generic;
using System;
using LotoLibrary.Enums;

﻿// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ModelInfo.cs - Modelos de dados
namespace LotoLibrary.Models.Prediction
{
    public partial class ModelInfo
    {
        public ModelType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; } = "1.0";
        public ModelCategory Category { get; set; }
        public List<string> RequiredParameters { get; set; } = new List<string>();
        public Dictionary<string, object> DefaultParameters { get; set; } = new Dictionary<string, object>();
        public int RequiredDataSize { get; set; }
        public double EstimatedAccuracy { get; set; }
        public bool SupportsExplanation { get; set; }
        public bool IsConfigurable { get; set; }
        public TimeSpan TypicalTrainingTime { get; set; }
        public ModelStatus Status { get; set; } = ModelStatus.Available;
    }
}
