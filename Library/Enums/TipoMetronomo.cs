// LotoLibrary/Services/TipoMetronomo.cs
namespace LotoLibrary.Enums
{
    #region Enums and Helper Classes
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
    #endregion
}