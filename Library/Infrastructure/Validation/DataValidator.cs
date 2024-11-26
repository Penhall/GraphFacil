using LotoLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Infrastructure.Validation;

public static class DataValidator
{
    public static void ValidatePercentualInput(PercentualInput input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (input.Dimensoes == null || input.Dimensoes.Length != 7)
            throw new ArgumentException("Input deve conter exatamente 7 dimensões");

        if (input.Dimensoes.Any(d => float.IsNaN(d) || float.IsInfinity(d)))
            throw new ArgumentException("Input contém valores inválidos (NaN ou Infinity)");
    }

    public static void ValidateInputCollection(IEnumerable<PercentualInput> inputs)
    {
        if (inputs == null || !inputs.Any())
            throw new ArgumentException("Collection de inputs vazia ou nula");

        foreach (var input in inputs)
        {
            ValidatePercentualInput(input);
        }
    }
}