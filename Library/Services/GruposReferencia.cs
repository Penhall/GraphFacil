using LotoLibrary.Models;

namespace LotoLibrary.Services;

public class GruposReferencia
{
    public Lance GrupoSSRef { get; set; }  // Grupo SS que faz 9 pontos no concursoBase
    public Lance GrupoNSRef { get; set; }  // Grupo NS que faz 6 pontos no concursoBase
}