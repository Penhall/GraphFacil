using Microsoft.ML.Data;

namespace LotoLibrary.Models;

// Classe para ML.NET output
public class SubgrupoMLOutput
{
    [ColumnName("Score")]
    public float PrevisaoAcertos { get; set; }
}
