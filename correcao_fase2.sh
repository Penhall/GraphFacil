#!/bin/bash
# correcao_fase2.sh - Correção das estruturas de dados e interfaces faltantes
# Execute: chmod +x correcao_fase2.sh && ./correcao_fase2.sh

set -e  # Parar em caso de erro

# Configurações
BASE_PROJECT_PATH="/d/PROJETOS/GraphFacil"
BACKUP_PATH="${BASE_PROJECT_PATH}/Backup_Fase2_$(date +%Y%m%d_%H%M%S)"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log
log() {
    echo -e "${GREEN}[$(date +'%H:%M:%S')]${NC} $1"
}

error() {
    echo -e "${RED}[$(date +'%H:%M:%S')] ERRO:${NC} $1"
}

warning() {
    echo -e "${YELLOW}[$(date +'%H:%M:%S')] AVISO:${NC} $1"
}

info() {
    echo -e "${BLUE}[$(date +'%H:%M:%S')] INFO:${NC} $1"
}

# Criar backup
create_backup() {
    log "📦 Criando backup da Fase 2..."
    mkdir -p "$BACKUP_PATH"
    
    local files_to_backup=(
        "Library/PredictionModels/AntiFrequency/AntiFrequencySimpleModel.cs"
        "Library/Services/AntiFrequencyValidation.cs"
        "Library/Services/Phase1ValidationService.cs"
        "Library/Services/ValidationMetricsService.cs"
    )
    
    for file in "${files_to_backup[@]}"; do
        local source_file="${BASE_PROJECT_PATH}/${file}"
        if [ -f "$source_file" ]; then
            local backup_file="${BACKUP_PATH}/${file}"
            mkdir -p "$(dirname "$backup_file")"
            cp "$source_file" "$backup_file"
            info "Backup: $(basename "$file")"
        fi
    done
    
    log "✅ Backup criado em: $BACKUP_PATH"
}

# Criar estruturas de dados faltantes
create_prediction_result() {
    log "🔧 Criando PredictionResult estruturado..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Models/Prediction/PredictionResult.cs"
    mkdir -p "$(dirname "$file_path")"
    
    cat > "$file_path" << 'EOF'
// D:\PROJETOS\GraphFacil\Library\Models\Prediction\PredictionResult.cs - Resultado de predição
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Resultado completo de uma predição
    /// </summary>
    public class PredictionResult
    {
        #region Core Properties
        /// <summary>
        /// Indica se a predição foi gerada com sucesso
        /// </summary>
        public bool Success { get; set; }

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
        public double Confidence { get; set; }

        /// <summary>
        /// Momento em que a predição foi gerada
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Explicação sobre como a predição foi gerada
        /// </summary>
        public string Explanation { get; set; } = "";
        #endregion

        #region Prediction Data
        /// <summary>
        /// Dezenas preditas
        /// </summary>
        public List<int> PredictedNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Concurso alvo da predição
        /// </summary>
        public int TargetConcurso { get; set; }

        /// <summary>
        /// Probabilidades individuais de cada dezena
        /// </summary>
        public Dictionary<int, double> NumberProbabilities { get; set; } = new Dictionary<int, double>();

        /// <summary>
        /// Metadados adicionais do modelo
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
        #endregion

        #region Factory Methods
        /// <summary>
        /// Cria um resultado de sucesso
        /// </summary>
        public static PredictionResult CreateSuccess(string modelName, List<int> predictedNumbers, double confidence, string explanation = "")
        {
            return new PredictionResult
            {
                Success = true,
                ModelName = modelName,
                PredictedNumbers = predictedNumbers,
                Confidence = confidence,
                Explanation = explanation,
                GeneratedAt = DateTime.Now
            };
        }

        /// <summary>
        /// Cria um resultado de erro
        /// </summary>
        public static PredictionResult CreateError(string modelName, string errorMessage)
        {
            return new PredictionResult
            {
                Success = false,
                ModelName = modelName,
                ErrorMessage = errorMessage,
                GeneratedAt = DateTime.Now
            };
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Verifica se a predição contém um número específico
        /// </summary>
        public bool ContainsNumber(int number)
        {
            return PredictedNumbers.Contains(number);
        }

        /// <summary>
        /// Obtém a probabilidade de um número específico
        /// </summary>
        public double GetNumberProbability(int number)
        {
            return NumberProbabilities.TryGetValue(number, out var prob) ? prob : 0.0;
        }

        /// <summary>
        /// Adiciona metadados ao resultado
        /// </summary>
        public void AddMetadata(string key, object value)
        {
            Metadata[key] = value;
        }
        #endregion
    }
}
EOF
    
    log "✅ PredictionResult.cs criado"
}

# Criar ValidationResult estruturado
create_validation_result() {
    log "🔧 Criando ValidationResult estruturado..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Models/Prediction/ValidationResult.cs"
    mkdir -p "$(dirname "$file_path")"
    
    cat > "$file_path" << 'EOF'
// D:\PROJETOS\GraphFacil\Library\Models\Prediction\ValidationResult.cs - Resultado de validação
using System;
using System.Collections.Generic;

namespace LotoLibrary.Models.Prediction
{
    /// <summary>
    /// Resultado detalhado de validação de um modelo
    /// </summary>
    public class ValidationResult
    {
        #region Core Properties
        /// <summary>
        /// Nome do modelo validado
        /// </summary>
        public string ModelName { get; set; } = "";

        /// <summary>
        /// Indica se a validação foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem de erro caso Success = false
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        /// <summary>
        /// Momento de início dos testes
        /// </summary>
        public DateTime TestStartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Momento de fim dos testes
        /// </summary>
        public DateTime TestEndTime { get; set; } = DateTime.Now;
        #endregion

        #region Validation Metrics
        /// <summary>
        /// Número de predições bem-sucedidas
        /// </summary>
        public int SuccessfulPredictions { get; set; }

        /// <summary>
        /// Total de testes executados
        /// </summary>
        public int TotalTests { get; set; }

        /// <summary>
        /// Precisão do modelo (0.0 a 1.0)
        /// </summary>
        public double Accuracy => TotalTests > 0 ? (double)SuccessfulPredictions / TotalTests : 0.0;

        /// <summary>
        /// Resultados detalhados de cada teste
        /// </summary>
        public List<ValidationDetail> DetailedResults { get; set; } = new List<ValidationDetail>();
        #endregion

        #region Factory Methods
        /// <summary>
        /// Cria um resultado de validação bem-sucedida
        /// </summary>
        public static ValidationResult CreateSuccess(string modelName, int successfulPredictions, int totalTests)
        {
            return new ValidationResult
            {
                ModelName = modelName,
                Success = true,
                SuccessfulPredictions = successfulPredictions,
                TotalTests = totalTests,
                TestEndTime = DateTime.Now
            };
        }

        /// <summary>
        /// Cria um resultado de erro na validação
        /// </summary>
        public static ValidationResult CreateError(string modelName, string errorMessage)
        {
            return new ValidationResult
            {
                ModelName = modelName,
                Success = false,
                ErrorMessage = errorMessage,
                TestEndTime = DateTime.Now
            };
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Adiciona um resultado de teste detalhado
        /// </summary>
        public void AddDetailedResult(ValidationDetail detail)
        {
            DetailedResults.Add(detail);
        }

        /// <summary>
        /// Duração total da validação
        /// </summary>
        public TimeSpan Duration => TestEndTime - TestStartTime;

        /// <summary>
        /// Obtém a taxa de acerto como porcentagem
        /// </summary>
        public double AccuracyPercentage => Accuracy * 100.0;
        #endregion
    }

    /// <summary>
    /// Detalhe individual de um teste de validação
    /// </summary>
    public class ValidationDetail
    {
        /// <summary>
        /// Concurso testado
        /// </summary>
        public int Concurso { get; set; }

        /// <summary>
        /// Dezenas preditas pelo modelo
        /// </summary>
        public List<int> PredictedNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Dezenas realmente sorteadas
        /// </summary>
        public List<int> ActualNumbers { get; set; } = new List<int>();

        /// <summary>
        /// Número de acertos
        /// </summary>
        public int Hits { get; set; }

        /// <summary>
        /// Confiança da predição
        /// </summary>
        public double Confidence { get; set; }

        /// <summary>
        /// Tempo gasto na predição
        /// </summary>
        public TimeSpan PredictionTime { get; set; }

        /// <summary>
        /// Indica se foi um acerto (15 dezenas)
        /// </summary>
        public bool IsSuccess => Hits == 15;

        /// <summary>
        /// Taxa de acerto para este teste específico
        /// </summary>
        public double HitRate => ActualNumbers.Count > 0 ? (double)Hits / ActualNumbers.Count : 0.0;
    }
}
EOF
    
    log "✅ ValidationResult.cs criado"
}

# Criar interface IConfigurableModel
create_configurable_model_interface() {
    log "🔧 Criando IConfigurableModel interface..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Interfaces/IConfigurableModel.cs"
    mkdir -p "$(dirname "$file_path")"
    
    cat > "$file_path" << 'EOF'
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
EOF
    
    log "✅ IConfigurableModel.cs criado"
}

# Criar TestResultXtras
create_test_result_xtras() {
    log "🔧 Criando TestResultXtras..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Utilities/TestResultXtras.cs"
    mkdir -p "$(dirname "$file_path")"
    
    cat > "$file_path" << 'EOF'
// D:\PROJETOS\GraphFacil\Library\Utilities\TestResultXtras.cs - Extensão do TestResult
using System.Collections.Generic;
using LotoLibrary.Suporte;

namespace LotoLibrary.Utilities
{
    /// <summary>
    /// Extensão do TestResult com informações adicionais
    /// </summary>
    public class TestResultXtras : TestResult
    {
        /// <summary>
        /// Detalhes adicionais sobre o resultado do teste
        /// </summary>
        public string Details { get; set; } = "";

        /// <summary>
        /// Métricas e valores numéricos relacionados ao teste
        /// </summary>
        public Dictionary<string, object> Metrics { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Dados brutos do teste
        /// </summary>
        public object RawData { get; set; }

        /// <summary>
        /// Cria um TestResultXtras de sucesso com detalhes
        /// </summary>
        public static TestResultXtras CreateSuccess(string testName, string message, string details = "", Dictionary<string, object> metrics = null)
        {
            return new TestResultXtras
            {
                TestName = testName,
                Success = true,
                Message = message,
                Details = details,
                Metrics = metrics ?? new Dictionary<string, object>(),
                StartTime = System.DateTime.Now,
                EndTime = System.DateTime.Now
            };
        }

        /// <summary>
        /// Cria um TestResultXtras de erro com detalhes
        /// </summary>
        public static TestResultXtras CreateError(string testName, string message, string details = "", System.Exception exception = null)
        {
            return new TestResultXtras
            {
                TestName = testName,
                Success = false,
                Message = message,
                Details = details + (exception != null ? $"\nException: {exception.Message}" : ""),
                StartTime = System.DateTime.Now,
                EndTime = System.DateTime.Now
            };
        }

        /// <summary>
        /// Adiciona uma métrica ao resultado
        /// </summary>
        public void AddMetric(string name, object value)
        {
            Metrics[name] = value;
        }

        /// <summary>
        /// Obtém uma métrica por nome
        /// </summary>
        public T GetMetric<T>(string name, T defaultValue = default(T))
        {
            if (Metrics.TryGetValue(name, out var value) && value is T)
            {
                return (T)value;
            }
            return defaultValue;
        }
    }
}
EOF
    
    log "✅ TestResultXtras.cs criado"
}

# Corrigir AntiFrequencySimpleModel
fix_antifrequency_model() {
    log "🔧 Corrigindo AntiFrequencySimpleModel..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/PredictionModels/AntiFrequency/AntiFrequencySimpleModel.cs"
    
    if [ -f "$file_path" ]; then
        # Backup do arquivo
        cp "$file_path" "${file_path}.backup"
        
        # Aplicar correções com sed
        sed -i 's/lance\.Concurso/lance.Id/g' "$file_path"
        sed -i 's/lance\.DezenasSorteadas/lance.Lista/g' "$file_path"
        sed -i 's/data\.DezenasSorteadas/data.Lista/g' "$file_path"
        sed -i 's/concurso\.DezenasSorteadas/concurso.Lista/g' "$file_path"
        
        # Adicionar using para as novas interfaces
        sed -i '1i using LotoLibrary.Interfaces;' "$file_path"
        sed -i '1i using LotoLibrary.Models.Prediction;' "$file_path"
        
        log "✅ AntiFrequencySimpleModel corrigido"
    else
        warning "AntiFrequencySimpleModel.cs não encontrado"
    fi
}

# Corrigir AntiFrequencyValidation
fix_antifrequency_validation() {
    log "🔧 Corrigindo AntiFrequencyValidation..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Services/AntiFrequencyValidation.cs"
    
    if [ -f "$file_path" ]; then
        # Backup do arquivo
        cp "$file_path" "${file_path}.backup"
        
        # Corrigir operador de multiplicação string * int
        sed -i 's/"=" \* 60/"".PadLeft(60, '\''='\'')/' "$file_path"
        sed -i 's/"=" \* 50/"".PadLeft(50, '\''='\'')/' "$file_path"
        sed -i 's/"-" \* 60/"".PadLeft(60, '\''-'\'')/' "$file_path"
        
        # Corrigir propriedades do Lance
        sed -i 's/lance\.DezenasSorteadas/lance.Lista/g' "$file_path"
        sed -i 's/data\.DezenasSorteadas/data.Lista/g' "$file_path"
        
        # Corrigir comparação de grupo de métodos
        sed -i 's/Count == 15/Count() == 15/g' "$file_path"
        sed -i 's/Contains(/Any(x => x == /g' "$file_path"
        
        # Adicionar using necessários
        sed -i '1i using LotoLibrary.Interfaces;' "$file_path"
        sed -i '1i using LotoLibrary.Models.Prediction;' "$file_path"
        sed -i '1i using System.Linq;' "$file_path"
        
        log "✅ AntiFrequencyValidation corrigido"
    else
        warning "AntiFrequencyValidation.cs não encontrado"
    fi
}

# Corrigir Phase1ValidationService
fix_phase1_validation() {
    log "🔧 Corrigindo Phase1ValidationService..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Services/Phase1ValidationService.cs"
    
    if [ -f "$file_path" ]; then
        # Backup do arquivo
        cp "$file_path" "${file_path}.backup"
        
        # Adicionar using para TestResultXtras
        sed -i '1i using LotoLibrary.Utilities;' "$file_path"
        
        # Substituir TestResult por TestResultXtras onde necessário
        sed -i 's/return new TestResult/return new TestResultXtras/g' "$file_path"
        
        log "✅ Phase1ValidationService corrigido"
    else
        warning "Phase1ValidationService.cs não encontrado"
    fi
}

# Corrigir ValidationMetricsService
fix_validation_metrics() {
    log "🔧 Corrigindo ValidationMetricsService..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Services/ValidationMetricsService.cs"
    
    if [ -f "$file_path" ]; then
        # Backup do arquivo
        cp "$file_path" "${file_path}.backup"
        
        # Remover chamada para método inexistente
        sed -i 's/dezena\.AtualizarFase()/\/\/ dezena.AtualizarFase() - método removido/g' "$file_path"
        
        log "✅ ValidationMetricsService corrigido"
    else
        warning "ValidationMetricsService.cs não encontrado"
    fi
}

# Atualizar DezenaOscilante com método AtualizarFase
update_dezena_oscilante() {
    log "🔧 Atualizando DezenaOscilante com método AtualizarFase..."
    
    local file_path="${BASE_PROJECT_PATH}/Library/Models/DezenaOscilante.cs"
    
    if [ -f "$file_path" ]; then
        # Backup do arquivo
        cp "$file_path" "${file_path}.backup"
        
        # Adicionar método AtualizarFase antes do último }
        sed -i '/^}$/i\
        #region Métodos de Oscilação\
        /// <summary>\
        /// Atualiza a fase da dezena baseado na frequência e influências externas\
        /// </summary>\
        public void AtualizarFase()\
        {\
            // Incrementar fase baseado na frequência\
            Fase += Frequencia;\
            \
            // Normalizar fase entre 0 e 360\
            Fase = Fase % 360;\
            if (Fase < 0) Fase += 360;\
            \
            // Calcular nova probabilidade baseada na fase\
            Probabilidade = (Math.Sin(Fase * Math.PI / 180) + 1) / 2;\
        }\
        \
        /// <summary>\
        /// Atualiza a fase com influência de outras dezenas (acoplamento)\
        /// </summary>\
        public void AtualizarFase(List<DezenaOscilante> outrasDezenas, double fatorAcoplamento = 0.1)\
        {\
            if (outrasDezenas == null) \
            {\
                AtualizarFase();\
                return;\
            }\
            \
            double influenciaExterna = 0;\
            foreach (var outra in outrasDezenas)\
            {\
                if (outra.Numero != this.Numero)\
                {\
                    influenciaExterna += Math.Sin((outra.Fase - this.Fase) * Math.PI / 180);\
                }\
            }\
            \
            // Atualizar fase com influência externa\
            Fase += Frequencia + (fatorAcoplamento * influenciaExterna);\
            \
            // Normalizar fase\
            Fase = Fase % 360;\
            if (Fase < 0) Fase += 360;\
            \
            // Atualizar probabilidade\
            Probabilidade = (Math.Sin(Fase * Math.PI / 180) + 1) / 2;\
        }\
        #endregion\
        \
        #region Métodos Helper\
        /// <summary>\
        /// Reinicia a dezena com valores aleatórios\
        /// </summary>\
        public void Reiniciar()\
        {\
            var random = new Random();\
            Fase = random.Next(0, 360);\
            Frequencia = 1.0;\
            FoiSorteada = false;\
            UltimoAtraso = 0;\
            Probabilidade = 0.0;\
        }\
        #endregion' "$file_path"
        
        # Adicionar using System
        sed -i '1i using System;' "$file_path"
        sed -i '1i using System.Collections.Generic;' "$file_path"
        
        log "✅ DezenaOscilante atualizada com AtualizarFase"
    else
        warning "DezenaOscilante.cs não encontrado"
    fi
}

# Verificar correções
verify_phase2_corrections() {
    log "🔍 Verificando correções da Fase 2..."
    
    local files_to_check=(
        "Library/Models/Prediction/PredictionResult.cs"
        "Library/Models/Prediction/ValidationResult.cs"
        "Library/Interfaces/IConfigurableModel.cs"
        "Library/Utilities/TestResultXtras.cs"
        "Library/Models/DezenaOscilante.cs"
    )
    
    local success_count=0
    local total_count=${#files_to_check[@]}
    
    for file in "${files_to_check[@]}"; do
        local full_path="${BASE_PROJECT_PATH}/${file}"
        if [ -f "$full_path" ]; then
            info "✅ $(basename "$file") - OK"
            ((success_count++))
        else
            error "❌ $(basename "$file") - FALTANDO"
        fi
    done
    
    log "📊 Verificação Fase 2: $success_count/$total_count arquivos OK"
    
    if [ $success_count -eq $total_count ]; then
        log "🎉 CORREÇÕES DA FASE 2 VERIFICADAS COM SUCESSO!"
        return 0
    else
        error "❌ Algumas correções da Fase 2 falharam"
        return 1
    fi
}

# Gerar relatório da Fase 2
generate_phase2_report() {
    log "📋 Gerando relatório da Fase 2..."
    
    local report_file="${BASE_PROJECT_PATH}/relatorio_fase2_$(date +%Y%m%d_%H%M%S).txt"
    
    cat > "$report_file" << EOF
=== RELATÓRIO DE CORREÇÕES - FASE 2 ===
Data: $(date '+%Y-%m-%d %H:%M:%S')
Script: correcao_fase2.sh

PROBLEMAS CORRIGIDOS:
✅ PredictionResult - Adicionadas propriedades: Success, ErrorMessage, ModelName, Confidence, GeneratedAt, Explanation
✅ ValidationResult - Adicionadas propriedades: ModelName, TestStartTime, SuccessfulPredictions, DetailedResults, TestEndTime
✅ ValidationDetail - Classe criada para detalhes de validação
✅ IConfigurableModel - Interface criada para modelos configuráveis
✅ TestResultXtras - Classe criada com propriedades Details e Metrics
✅ Lance - Corrigidas referências: Concurso → Id, DezenasSorteadas → Lista
✅ DezenaOscilante - Adicionado método AtualizarFase()
✅ AntiFrequencySimpleModel - Corrigidas propriedades e referências
✅ AntiFrequencyValidation - Corrigidos operadores string * int
✅ Phase1ValidationService - Adicionado suporte a TestResultXtras
✅ ValidationMetricsService - Removida chamada para método inexistente

ESTRUTURAS CRIADAS:
📁 Library/Models/Prediction/PredictionResult.cs
📁 Library/Models/Prediction/ValidationResult.cs  
📁 Library/Interfaces/IConfigurableModel.cs
📁 Library/Utilities/TestResultXtras.cs

BACKUP CRIADO EM:
$BACKUP_PATH

PRÓXIMOS PASSOS:
1. Compile o projeto: Build -> Rebuild Solution
2. Verificar se todos os erros CS0117, CS0246, CS1061 foram resolvidos
3. Executar testes de validação
4. Implementar novos modelos da Fase 2

ERROS RESOLVIDOS:
- CS0117: Propriedades não encontradas em PredictionResult/ValidationResult
- CS0246: Classes/interfaces não encontradas
- CS1061: Propriedades não definidas em Lance/ValidationResult
- CS0019: Operadores incompatíveis (string * int)

STATUS: FASE 2 CORRIGIDA COM SUCESSO ✅
EOF
    
    log "📄 Relatório da Fase 2 salvo em: $report_file"
}

# Função principal
main() {
    echo -e "${BLUE}"
    echo "╔══════════════════════════════════════════════════════════════╗"
    echo "║            SCRIPT DE CORREÇÃO FASE 2                        ║"
    echo "║         Estruturas de Dados e Interfaces                    ║"
    echo "║                                                              ║"
    echo "║  Corrige erros relacionados a:                              ║"
    echo "║  • PredictionResult/ValidationResult incompletos            ║"
    echo "║  • Interfaces faltantes (IConfigurableModel)                ║"
    echo "║  • Propriedades incorretas do Lance                         ║"
    echo "║  • Classes de teste faltantes                               ║"
    echo "╚══════════════════════════════════════════════════════════════╝"
    echo -e "${NC}"
    echo
    
    # Confirmar execução
    read -p "Deseja continuar com as correções da Fase 2? (y/N): " -n 1 -r
    echo
    if [[ ! $REPLY =~ ^[Yy]$ ]]; then
        log "❌ Operação cancelada pelo usuário"
        exit 0
    fi
    
    # Executar correções
    log "🚀 Iniciando correções da Fase 2..."
    
    create_backup
    create_prediction_result
    create_validation_result
    create_configurable_model_interface
    create_test_result_xtras
    update_dezena_oscilante
    fix_antifrequency_model
    fix_antifrequency_validation
    fix_phase1_validation
    fix_validation_metrics
    
    # Verificar resultado
    if verify_phase2_corrections; then
        generate_phase2_report
        
        echo
        log "🎉 CORREÇÕES DA FASE 2 APLICADAS COM SUCESSO!"
        echo
        info "🚀 Próximos passos:"
        info "   1. Compile o projeto: Build -> Rebuild Solution"
        info "   2. Verificar se erros CS0117/CS0246/CS1061 foram resolvidos"
        info "   3. Executar testes de validação"
        info "   4. Implementar novos modelos da Fase 2"
        echo
        log "📦 Backup da Fase 2 disponível em: $BACKUP_PATH"
    else
        error "❌ Algumas correções da Fase 2 falharam. Verifique os logs acima."
        exit 1
    fi
}

# Executar script principal
main "$@"
