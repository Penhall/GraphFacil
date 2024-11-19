using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace LotoLibrary.Models;

public class Lotofacil
{
    [JsonIgnore]
    public bool acumulado { get; set; }
    public string dataApuracao { get; set; }
    [JsonIgnore]
    public string dataProximoConcurso { get; set; }
    public IList<string> dezenasSorteadasOrdemSorteio { get; set; }
    [JsonIgnore]
    public bool exibirDetalhamentoPorCidade { get; set; }
    public object id { get; set; }

    [JsonIgnore]
    public int indicadorConcursoEspecial { get; set; }
    public IList<string> listaDezenas { get; set; }
    public object listaDezenasSegundoSorteio { get; set; }
    [JsonIgnore]
    public IList<ListaMunicipioUFGanhadore> listaMunicipioUFGanhadores { get; set; }
    [JsonIgnore]
    public IList<ListaRateioPremio> listaRateioPremio { get; set; }
    [JsonIgnore]
    public object listaResultadoEquipeEsportiva { get; set; }
    [JsonIgnore]
    public string localSorteio { get; set; }
    [JsonIgnore]
    public string nomeMunicipioUFSorteio { get; set; }
    [JsonIgnore]
    public string nomeTimeCoracaoMesSorte { get; set; }
    public int numero { get; set; }
    public int numeroConcursoAnterior { get; set; }
    [JsonIgnore]
    public int numeroConcursoFinal_0_5 { get; set; }
    [JsonIgnore]
    public int numeroConcursoProximo { get; set; }
    public int numeroJogo { get; set; }
    [JsonIgnore]
    public string observacao { get; set; }
    [JsonIgnore]
    public object premiacaoContingencia { get; set; }

    [JsonIgnore]
    public string tipoJogo { get; set; }
    [JsonIgnore]
    public int tipoPublicacao { get; set; }
    [JsonIgnore]
    public bool ultimoConcurso { get; set; }
    [JsonIgnore]
    public double valorArrecadado { get; set; }
    [JsonIgnore]
    public double valorAcumuladoConcurso_0_5 { get; set; }
    [JsonIgnore]
    public double valorAcumuladoConcursoEspecial { get; set; }
    [JsonIgnore]
    public double valorAcumuladoProximoConcurso { get; set; }
    [JsonIgnore]
    public double valorEstimadoProximoConcurso { get; set; }
    [JsonIgnore]
    public double valorSaldoReservaGarantidora { get; set; }
    [JsonIgnore]
    public double valorTotalPremioFaixaUm { get; set; }
}

