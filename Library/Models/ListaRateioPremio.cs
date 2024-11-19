using System.Text.Json.Serialization;

namespace LotoLibrary.Models;

public class ListaRateioPremio
{
    [JsonIgnore]
    public string descricaoFaixa { get; set; }
    [JsonIgnore]
    public int faixa { get; set; }
    [JsonIgnore]
    public int numeroDeGanhadores { get; set; }
    [JsonIgnore]
    public double valorPremio { get; set; }
}

