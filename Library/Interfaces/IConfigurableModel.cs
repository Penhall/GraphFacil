// D:\PROJETOS\GraphFacil\Library\Interfaces\IConfigurableModel.cs - Interface principal
using System.Collections.Generic;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para modelos que suportam configuração de parâmetros
    /// </summary>
    public interface IConfigurableModel : IPredictionModel
    {
        Dictionary<string, object> Parameters { get; set; }
        void UpdateParameters(Dictionary<string, object> newParameters);
        Dictionary<string, object> GetDefaultParameters();
        bool ValidateParameters(Dictionary<string, object> parameters);
    }
}


