using System;

namespace LotoLibrary.Models.Configuration
{
    /// <summary>
    /// Configuração para treinamento de modelos
    /// </summary>
    public class ConfiguracaoTreinamento
    {
        /// <summary>
        /// Tamanho da validação em número de concursos
        /// </summary>
        public int TamanhoValidacao { get; set; } = 50;

        /// <summary>
        /// Concurso de início para treinamento (1 = sem filtro)
        /// </summary>
        public int ConcursoInicio { get; set; } = 1;

        /// <summary>
        /// Concurso de fim para treinamento (-1 = sem filtro)
        /// </summary>
        public int ConcursoFim { get; set; } = -1;

        /// <summary>
        /// Concurso mínimo disponível no dataset
        /// </summary>
        public int ConcursoMinimo { get; set; } = 1;

        /// <summary>
        /// Concurso máximo disponível no dataset
        /// </summary>
        public int ConcursoMaximo { get; set; } = 3000;

        /// <summary>
        /// Construtor padrão
        /// </summary>
        public ConfiguracaoTreinamento()
        {
        }

        /// <summary>
        /// Construtor com configuração de range
        /// </summary>
        /// <param name="concursoMinimo">Concurso mínimo disponível</param>
        /// <param name="concursoMaximo">Concurso máximo disponível</param>
        public ConfiguracaoTreinamento(int concursoMinimo, int concursoMaximo)
        {
            ConcursoMinimo = concursoMinimo;
            ConcursoMaximo = concursoMaximo;
        }

        /// <summary>
        /// Valida se a configuração está consistente
        /// </summary>
        /// <returns>True se válida, false caso contrário</returns>
        public bool IsValid()
        {
            if (TamanhoValidacao <= 0)
                return false;

            if (ConcursoInicio < ConcursoMinimo)
                return false;

            if (ConcursoFim > 0 && ConcursoFim > ConcursoMaximo)
                return false;

            if (ConcursoFim > 0 && ConcursoInicio >= ConcursoFim)
                return false;

            return true;
        }

        /// <summary>
        /// Retorna o range efetivo de concursos para treinamento
        /// </summary>
        /// <returns>Tupla com início e fim efetivos</returns>
        public (int inicio, int fim) GetEffectiveRange()
        {
            var inicio = Math.Max(ConcursoInicio, ConcursoMinimo);
            var fim = ConcursoFim > 0 ? Math.Min(ConcursoFim, ConcursoMaximo) : ConcursoMaximo;
            return (inicio, fim);
        }

        /// <summary>
        /// Cria uma cópia da configuração
        /// </summary>
        /// <returns>Nova instância com os mesmos valores</returns>
        public ConfiguracaoTreinamento Clone()
        {
            return new ConfiguracaoTreinamento(ConcursoMinimo, ConcursoMaximo)
            {
                TamanhoValidacao = TamanhoValidacao,
                ConcursoInicio = ConcursoInicio,
                ConcursoFim = ConcursoFim
            };
        }
    }
}