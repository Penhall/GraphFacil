using LotoLibrary.Enums;
using System;

namespace LotoLibrary.Models.Configuration;

/// <summary>
/// Informações sobre um modelo de predição
/// </summary>
public class ModelInfo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Version { get; set; } = "1.0";
    public ModelType Type { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Author { get; set; } = "Sistema";
    public bool IsActive { get; set; } = true;

    public ModelInfo()
    {
    }

    public ModelInfo(string name, ModelType type, string description = "")
    {
        Name = name;
        Type = type;
        Description = description;
    }

    public override string ToString()
    {
        return $"{Name} v{Version} ({Type})";
    }
}