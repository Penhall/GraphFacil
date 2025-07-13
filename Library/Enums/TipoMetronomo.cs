// D:\PROJETOS\GraphFacil\Library\Enums\TipoMetronomo.cs
// LotoLibrary/Services/TipoMetronomo.cs
namespace LotoLibrary.Enums;

public enum TipoMetronomo
{
    DadosInsuficientes,
    Regular,        // Intervalos consistentes
    Alternado,      // Padrão A-B-A-B
    CicloLongo,     // Padrão repetitivo longo
    Tendencial,     // Intervalos com tendência
    MultiModal,     // Múltiplos picos de frequência
    Irregular       // Sem padrão claro
}
