using Newtonsoft.Json;
using System;
using System.IO;

namespace LotoLibrary.Services;

public class FileService
{
    private readonly string _baseDirectory;

    public FileService(string baseDirectory = "data")
    {
        _baseDirectory = baseDirectory;
        if (!Directory.Exists(_baseDirectory))
        {
            Directory.CreateDirectory(_baseDirectory);
        }
    }

    // Método para salvar dados em JSON
    public void SalvarDados<T>(string fileName, T data)
    {
        try
        {
            string filePath = Path.Combine(_baseDirectory, fileName);
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(filePath, jsonData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar dados em {fileName}: {ex.Message}");
        }
    }

    // Método para carregar dados de JSON
    public T CarregarDados<T>(string fileName)
    {
        try
        {
            string filePath = Path.Combine(_baseDirectory, fileName);
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<T>(jsonData);
            }
            else
            {
                Console.WriteLine($"Arquivo {fileName} não encontrado.");
                return default;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar dados de {fileName}: {ex.Message}");
            return default;
        }
    }

    // Método para salvar um modelo de treinamento
    public void SalvarModelo(string fileName, byte[] modelData)
    {
        try
        {
            string filePath = Path.Combine(_baseDirectory, fileName);
            File.WriteAllBytes(filePath, modelData);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao salvar o modelo em {fileName}: {ex.Message}");
        }
    }

    // Método para carregar um modelo de treinamento
    public byte[] CarregarModelo(string fileName)
    {
        try
        {
            string filePath = Path.Combine(_baseDirectory, fileName);
            if (File.Exists(filePath))
            {
                return File.ReadAllBytes(filePath);
            }
            else
            {
                Console.WriteLine($"Modelo {fileName} não encontrado.");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao carregar o modelo de {fileName}: {ex.Message}");
            return null;
        }
    }
}

// Exemplo de uso:
// FileService fileService = new FileService();
// fileService.SalvarDados("dadosTreinamento.json", dadosTreinamento);
// var dadosCarregados = fileService.CarregarDados<tipoDados>("dadosTreinamento.json");
// fileService.SalvarModelo("modelo.bin", modeloEmBytes);
// var modeloCarregado = fileService.CarregarModelo("modelo.bin");
