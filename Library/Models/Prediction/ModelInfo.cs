// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ModelInfo.cs - Modelos de dados
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    public class ModelInfo
    {
        public ModelType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> RequiredParameters { get; set; } = new List<string>();
        public Dictionary<string, object> DefaultParameters { get; set; } = new Dictionary<string, object>();
        public bool SupportsExplanation { get; set; }
        public bool IsConfigurable { get; set; }
        public TimeSpan TypicalTrainingTime { get; set; }
    }
}
