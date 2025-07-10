// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionResult.cs
// VERSÃO ÚNICA E DEFINITIVA - Remove todas as duplicações
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Resultado completo de uma predição - VERSÃO ÚNICA E DEFINITIVA
    /// Esta é a ÚNICA implementação de PredictionResult no projeto
    /// </summary>
    public class PredictionResult
    {
        #region Core Properties - INTERFACE BÁSICA
        /// <summary>
        /// Indica se a predição foi gerada com sucesso
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Mensagem de erro caso Success = false
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Nome do modelo que gerou a predição
        /// </summary>
        public string ModelName { get; set; } = "";

        /// <summary>
        /// Nível de confiança da predição (0.0 a 1.0)
        /// </summary>
        public double Confidence { get; set; } = 0.0;

        /// <summary>
        /// Momento em que a predição foi gerada
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Explicação sobre como a predição foi gerada
        /// </summary>
        public string Explanation { get; set; } = "";
        #endregion

        #region Prediction Data - DADOS DA PREDIÇÃO
        /// <summary>
        /// Dezenas preditas (sempre 15 dezenas para Lotofácil)
        /// </summary>
        public List<int> PredictedNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Concurso alvo da predição
        /// </summary>
        public int TargetConcurso { get; set; } = 0;

        /// <summary>
        /// Probabilidades individuais de cada dezena (1-25)
        /// </summary>
        public Dictionary<int, double> NumberProbabilities { get; set; } = new Dictionary<int, double>();

        /// <summary>
        /// Metadados específicos do modelo usado
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Tempo gasto para gerar a predição
        /// </summary>
        public TimeSpan ProcessingTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Versão do modelo usada
        /// </summary>
        public string ModelVersion { get; set; } = "1.0";
        #endregion

        #region Extended Properties - PROPRIEDADES ESTENDIDAS
        /// <summary>
        /// Momento da predição (alias para compatibilidade)
        /// </summary>
        public DateTime PredictionTime 
        { 
            get => GeneratedAt; 
            set => GeneratedAt = value; 
        }

        /// <summary>
        /// Modelo usado (alias para compatibilidade)
        /// </summary>
        public string ModelUsed 
        { 
            get => ModelName; 
            set => ModelName = value; 
        }

        /// <summary>
        /// Confiança geral (alias para compatibilidade)
        /// </summary>
        public double OverallConfidence 
        { 
            get => Confidence; 
            set => Confidence = value; 
        }

        /// <summary>
        /// Versão (alias para compatibilidade)
        /// </summary>
        public string Version 
        { 
            get => ModelVersion; 
            set => ModelVersion = value; 
        }
        #endregion

        #region Validation Properties - PROPRIEDADES DE VALIDAÇÃO
        /// <summary>
        /// Se essa predição foi validada contra um resultado real
        /// </summary>
        public bool IsValidated { get; set; } = false;

        /// <summary>
        /// Número de acertos (se validado)
        /// </summary>
        public int ActualHits { get; set; } = 0;

        /// <summary>
        /// Dezenas realmente sorteadas (se validado)
        /// </summary>
        public List<int> ActualNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Resultado da validação
        /// </summary>
        public ValidationResult ValidationResult { get; set; }
        #endregion

        #region Constructors - CONSTRUTORES
        /// <summary>
        /// Construtor padrão
        /// </summary>
        public PredictionResult()
        {
            GeneratedAt = DateTime.Now;
            PredictedNumbers = new List<int>();
            NumberProbabilities = new Dictionary<int, double>();
            Metadata = new Dictionary<string, object>();
        }

        /// <summary>
        /// Construtor com nome do modelo
        /// </summary>
        public PredictionResult(string modelName) : this()
        {
            ModelName = modelName ?? "";
        }

        /// <summary>
        /// Construtor completo
        /// </summary>
        public PredictionResult(string modelName, List<int> predictedNumbers, double confidence) : this(modelName)
        {
            if (predictedNumbers != null)
            {
                PredictedNumbers = new List<int>(predictedNumbers);
            }
            Confidence = Math.Max(0.0, Math.Min(1.0, confidence));
        }
        #endregion

        #region Factory Methods - MÉTODOS DE CRIAÇÃO
        /// <summary>
        /// Cria um resultado de sucesso
        /// </summary>
        public static PredictionResult CreateSuccess(string modelName, List<int> predictedNumbers, double confidence, string explanation = "")
        {
            // Validações
            if (string.IsNullOrWhiteSpace(modelName))
            {
                return CreateError("Unknown", "Nome do modelo não pode estar vazio");
            }

            if (predictedNumbers == null || predictedNumbers.Count == 0)
            {
                return CreateError(modelName, "Lista de números preditos não pode estar vazia");
            }

            // Validar que temos exatamente 15 dezenas
            if (predictedNumbers.Count != 15)
            {
                return CreateError(modelName, $"Predição deve conter exatamente 15 dezenas, mas contém {predictedNumbers.Count}");
            }

            // Validar que todas as dezenas estão no range 1-25
            var invalidNumbers = predictedNumbers.Where(n => n < 1 || n > 25).ToList();
            if (invalidNumbers.Any())
            {
                return CreateError(modelName, $"Predição contém dezenas fora do range 1-25: {string.Join(", ", invalidNumbers)}");
            }

            // Validar que não há dezenas duplicadas
            if (predictedNumbers.Distinct().Count() != 15)
            {
                var duplicates = predictedNumbers.GroupBy(n => n).Where(g => g.Count() > 1).Select(g => g.Key);
                return CreateError(modelName, $"Predição contém dezenas duplicadas: {string.Join(", ", duplicates)}");
            }

            // Criar resultado válido
            var result = new PredictionResult(modelName)
            {
                Success = true,
                PredictedNumbers = predictedNumbers.OrderBy(n => n).ToList(),
                Confidence = Math.Max(0.0, Math.Min(1.0, confidence)),
                Explanation = explanation ?? ""
            };

            return result;
        }

        /// <summary>
        /// Cria um resultado de erro
        /// </summary>
        public static PredictionResult CreateError(string modelName, string errorMessage)
        {
            return new PredictionResult(modelName ?? "Unknown")
            {
                Success = false,
                ErrorMessage = errorMessage ?? "Erro desconhecido"
            };
        }

        /// <summary>
        /// Cria um resultado de erro com exceção
        /// </summary>
        public static PredictionResult CreateError(string modelName, Exception exception)
        {
            var errorMessage = exception?.Message ?? "Erro desconhecido";
            var result = CreateError(modelName, errorMessage);
            
            if (exception != null)
            {
                result.AddMetadata("Exception", exception.GetType().Name);
                result.AddMetadata("StackTrace", exception.StackTrace);
            }
            
            return result;
        }
        #endregion

        #region Helper Methods - MÉTODOS AUXILIARES
        /// <summary>
        /// Verifica se a predição contém uma dezena específica
        /// </summary>
        public bool ContainsNumber(int number)
        {
            return PredictedNumbers.Contains(number);
        }

        /// <summary>
        /// Obtém a probabilidade de uma dezena específica
        /// </summary>
        public double GetNumberProbability(int number)
        {
            return NumberProbabilities.TryGetValue(number, out var prob) ? prob : 0.0;
        }

        /// <summary>
        /// Adiciona metadados do modelo
        /// </summary>
        public void AddMetadata(string key, object value)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                Metadata[key] = value;
            }
        }

        /// <summary>
        /// Obtém metadado tipado
        /// </summary>
        public T GetMetadata<T>(string key, T defaultValue = default(T))
        {
            if (Metadata.TryGetValue(key, out var value) && value is T)
            {
                return (T)value;
            }
            return defaultValue;
        }

        /// <summary>
        /// Valida esta predição contra um resultado real
        /// </summary>
        public void ValidateAgainst(Lance actualResult)
        {
            if (actualResult?.Lista == null) return;

            ActualNumbers = new List<int>(actualResult.Lista);
            ActualHits = PredictedNumbers.Intersect(actualResult.Lista).Count();
            IsValidated = true;

            // Adicionar metadados da validação
            AddMetadata("ValidationDate", DateTime.Now);
            AddMetadata("ActualConcurso", actualResult.Id);
            AddMetadata("HitRate", ActualNumbers.Count > 0 ? (double)ActualHits / ActualNumbers.Count : 0.0);
        }

        /// <summary>
        /// Valida contra uma lista de números
        /// </summary>
        public void ValidateAgainst(List<int> actualNumbers)
        {
            if (actualNumbers == null) return;

            ActualNumbers = new List<int>(actualNumbers);
            ActualHits = PredictedNumbers.Intersect(actualNumbers).Count();
            IsValidated = true;

            AddMetadata("ValidationDate", DateTime.Now);
            AddMetadata("HitRate", actualNumbers.Count > 0 ? (double)ActualHits / actualNumbers.Count : 0.0);
        }

        /// <summary>
        /// Retorna resumo textual da predição
        /// </summary>
        public string GetSummary()
        {
            if (!Success)
                return $"ERRO: {ErrorMessage}";

            var numbersStr = string.Join(", ", PredictedNumbers.OrderBy(n => n));
            var summary = $"Modelo: {ModelName}\n";
            summary += $"Dezenas: [{numbersStr}]\n";
            summary += $"Confiança: {Confidence:P2}\n";
            summary += $"Gerado em: {GeneratedAt:dd/MM/yyyy HH:mm:ss}";

            if (IsValidated)
            {
                summary += $"\nValidação: {ActualHits}/15 acertos";
                var hitRate = ActualNumbers.Count > 0 ? (double)ActualHits / ActualNumbers.Count : 0.0;
                summary += $" ({hitRate:P1})";
            }

            if (!string.IsNullOrWhiteSpace(Explanation))
            {
                summary += $"\nExplicação: {Explanation}";
            }

            return summary;
        }

        /// <summary>
        /// Retorna resumo resumido (uma linha)
        /// </summary>
        public string GetShortSummary()
        {
            if (!Success)
                return $"ERRO: {ErrorMessage}";

            var numbersStr = string.Join(",", PredictedNumbers.OrderBy(n => n));
            var result = $"{ModelName}: [{numbersStr}] (Conf: {Confidence:P1})";
            
            if (IsValidated)
            {
                result += $" - {ActualHits}/15 acertos";
            }
            
            return result;
        }

        /// <summary>
        /// Copia esta predição
        /// </summary>
        public PredictionResult Clone()
        {
            var clone = new PredictionResult(ModelName)
            {
                Success = this.Success,
                ErrorMessage = this.ErrorMessage,
                Confidence = this.Confidence,
                GeneratedAt = this.GeneratedAt,
                Explanation = this.Explanation,
                TargetConcurso = this.TargetConcurso,
                ModelVersion = this.ModelVersion,
                ProcessingTime = this.ProcessingTime,
                IsValidated = this.IsValidated,
                ActualHits = this.ActualHits
            };

            clone.PredictedNumbers = new List<int>(this.PredictedNumbers);
            clone.ActualNumbers = new List<int>(this.ActualNumbers);
            clone.NumberProbabilities = new Dictionary<int, double>(this.NumberProbabilities);
            clone.Metadata = new Dictionary<string, object>(this.Metadata);

            return clone;
        }
        #endregion

        #region Comparison and Analysis - ANÁLISE E COMPARAÇÃO
        /// <summary>
        /// Compara esta predição com outra
        /// </summary>
        public PredictionComparison CompareTo(PredictionResult other)
        {
            if (other == null)
                return new PredictionComparison { IsValid = false, Message = "Predição comparada é nula" };

            var comparison = new PredictionComparison
            {
                IsValid = true,
                Model1 = this.ModelName,
                Model2 = other.ModelName,
                ConfidenceDifference = this.Confidence - other.Confidence,
                ProcessingTimeDifference = this.ProcessingTime - other.ProcessingTime,
                CommonNumbers = this.PredictedNumbers.Intersect(other.PredictedNumbers).ToList(),
                UniqueNumbers1 = this.PredictedNumbers.Except(other.PredictedNumbers).ToList(),
                UniqueNumbers2 = other.PredictedNumbers.Except(this.PredictedNumbers).ToList()
            };

            comparison.Similarity = (double)comparison.CommonNumbers.Count / 15.0;
            comparison.Message = $"Similaridade: {comparison.Similarity:P1}, Comum: {comparison.CommonNumbers.Count} dezenas";

            return comparison;
        }

        /// <summary>
        /// Analisa a qualidade da predição
        /// </summary>
        public PredictionQuality AnalyzeQuality()
        {
            var quality = new PredictionQuality
            {
                OverallScore = 0.0,
                Factors = new Dictionary<string, double>()
            };

            if (!Success)
            {
                quality.Grade = "F";
                quality.Issues.Add("Predição falhou");
                return quality;
            }

            // Analisar fatores de qualidade
            double confidenceScore = Confidence * 100;
            double diversityScore = AnalyzeDiversity() * 100;
            double consistencyScore = AnalyzeConsistency() * 100;

            quality.Factors["Confiança"] = confidenceScore;
            quality.Factors["Diversidade"] = diversityScore;
            quality.Factors["Consistência"] = consistencyScore;

            // Calcular score geral
            quality.OverallScore = (confidenceScore + diversityScore + consistencyScore) / 3.0;

            // Determinar grade
            quality.Grade = quality.OverallScore switch
            {
                >= 90 => "A+",
                >= 80 => "A",
                >= 70 => "B",
                >= 60 => "C",
                >= 50 => "D",
                _ => "F"
            };

            return quality;
        }

        private double AnalyzeDiversity()
        {
            if (PredictedNumbers.Count != 15) return 0.0;

            // Analisar distribuição das dezenas
            var ranges = new[]
            {
                PredictedNumbers.Count(n => n >= 1 && n <= 5),   // Range 1-5
                PredictedNumbers.Count(n => n >= 6 && n <= 10),  // Range 6-10
                PredictedNumbers.Count(n => n >= 11 && n <= 15), // Range 11-15
                PredictedNumbers.Count(n => n >= 16 && n <= 20), // Range 16-20
                PredictedNumbers.Count(n => n >= 21 && n <= 25)  // Range 21-25
            };

            // Diversidade ideal: distribuição uniforme (3 dezenas por range)
            var idealDistribution = 3.0;
            var variance = ranges.Select(count => Math.Pow(count - idealDistribution, 2)).Average();
            
            // Converter variância em score de diversidade (menor variância = maior diversidade)
            return Math.Max(0.0, 1.0 - (variance / 9.0)); // 9 é a variância máxima possível
        }

        private double AnalyzeConsistency()
        {
            // Analisar se as probabilidades são consistentes com as seleções
            if (!NumberProbabilities.Any()) return 0.5; // Sem dados = mediano

            var selectedProbs = PredictedNumbers.Select(n => GetNumberProbability(n)).Where(p => p > 0).ToList();
            if (!selectedProbs.Any()) return 0.5;

            var avgSelectedProb = selectedProbs.Average();
            var allProbs = NumberProbabilities.Values.ToList();
            var avgAllProb = allProbs.Average();

            // Consistência: números selecionados devem ter probabilidades acima da média
            return avgAllProb > 0 ? Math.Min(1.0, avgSelectedProb / avgAllProb) : 0.5;
        }
        #endregion

        #region Overrides - SOBRESCRITAS
        public override string ToString()
        {
            return GetShortSummary();
        }

        public override bool Equals(object obj)
        {
            if (obj is PredictionResult other)
            {
                return this.ModelName == other.ModelName &&
                       this.PredictedNumbers.SequenceEqual(other.PredictedNumbers) &&
                       Math.Abs(this.Confidence - other.Confidence) < 0.001 &&
                       this.GeneratedAt == other.GeneratedAt;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(
                ModelName,
                string.Join(",", PredictedNumbers),
                Confidence,
                GeneratedAt
            );
        }
        #endregion
    }

    #region Supporting Classes - CLASSES DE APOIO
    /// <summary>
    /// Comparação entre duas predições
    /// </summary>
    public class PredictionComparison
    {
        public bool IsValid { get; set; }
        public string Model1 { get; set; }
        public string Model2 { get; set; }
        public double ConfidenceDifference { get; set; }
        public TimeSpan ProcessingTimeDifference { get; set; }
        public List<int> CommonNumbers { get; set; } = new List<int>();
        public List<int> UniqueNumbers1 { get; set; } = new List<int>();
        public List<int> UniqueNumbers2 { get; set; } = new List<int>();
        public double Similarity { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// Análise de qualidade de uma predição
    /// </summary>
    public class PredictionQuality
    {
        public double OverallScore { get; set; }
        public string Grade { get; set; }
        public Dictionary<string, double> Factors { get; set; } = new Dictionary<string, double>();
        public List<string> Issues { get; set; } = new List<string>();
        public List<string> Strengths { get; set; } = new List<string>();
    }
    #endregion
}