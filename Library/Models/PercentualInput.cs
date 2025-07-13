// D:\PROJETOS\GraphFacil\Library\Models\PercentualInput.cs
﻿using Microsoft.ML.Data;

namespace LotoLibrary.Models;

// Entrada (7 dimensões de percentuais)
public class PercentualInput
{
    [VectorType(7)]
    [LoadColumn(0)]
    public float[] Dimensoes { get; set; }

    public PercentualInput()
    {
        Dimensoes = new float[7];
    }
}
