// D:\PROJETOS\GraphFacil\Library\Interfaces\IConfigurableModel.cs - Interface para modelos configuráveis
using System.Collections.Generic;

namespace LotoLibrary.Interfaces
{
    /// <summary>
    /// Interface para modelos que podem ser configurados com parâmetros
    /// </summary>
    public interface IConfigurableModel
    {
        /// <summary>
        /// Parâmetros atuais do modelo
        /// </summary>
        Dictionary<string, object> CurrentParameters { get; }

        /// <summary>
        /// Parâmetros padrão do modelo
        /// </summary>
        Dictionary<string, object> DefaultParameters { get; }

        /// <summary>
        /// Atualiza os parâmetros do modelo
        /// </summary>
        /// <param name="parameters">Novos parâmetros</param>
        void UpdateParameters(Dictionary<string, object> parameters);

        /// <summary>
        /// Valida se os parâmetros são válidos para este modelo
        /// </summary>
        /// <param name="parameters">Parâmetros a validar</param>
        /// <returns>True se válidos</returns>
        bool ValidateParameters(Dictionary<string, object> parameters);

        /// <summary>
        /// Obtém a descrição de um parâmetro específico
        /// </summary>
        /// <param name="parameterName">Nome do parâmetro</param>
        /// <returns>Descrição do parâmetro</returns>
        string GetParameterDescription(string parameterName);

        /// <summary>
        /// Obtém os valores permitidos para um parâmetro (se aplicável)
        /// </summary>
        /// <param name="parameterName">Nome do parâmetro</param>
        /// <returns>Lista de valores permitidos, ou null se qualquer valor é permitido</returns>
        List<object> GetAllowedValues(string parameterName);

        /// <summary>
        /// Restaura os parâmetros para os valores padrão
        /// </summary>
        void ResetToDefaults();
    }
}
