// D:\PROJETOS\GraphFacil\Library\Models\PercentualPrediction.cs
﻿using Microsoft.ML.Data;

namespace LotoLibrary.Models;

// Saída (previsão)
public class PercentualPrediction
{
    [ColumnName("Score")]
    public float Resultado { get; set; }
}
