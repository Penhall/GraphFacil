// D:\PROJETOS\GraphFacil\Library\Suporte\ListaMunicipioUFGanhadore.cs
﻿using System.Text.Json.Serialization;

namespace LotoLibrary.Suporte;

public class ListaMunicipioUFGanhadore
{
    [JsonIgnore]
    public int ganhadores { get; set; }
    [JsonIgnore]
    public string municipio { get; set; }
    [JsonIgnore]
    public string nomeFatansiaUL { get; set; }
    [JsonIgnore]
    public int posicao { get; set; }
    [JsonIgnore]
    public string serie { get; set; }
    [JsonIgnore]
    public string uf { get; set; }
}

