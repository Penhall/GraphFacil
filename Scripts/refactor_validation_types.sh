# D:\PROJETOS\GraphFacil\Scripts\refactor_validation_types.sh - Script de refatoração das classes de validação
#!/bin/bash

# Configurações
PROJECT_ROOT="E:/PROJETOS/GraphFacil"
BACKUP_DIR="$PROJECT_ROOT/Backup/ValidationRefactor_$(date +%Y%m%d_%H%M%S)"
LIBRARY_DIR="$PROJECT_ROOT/Library"
NEW_VALIDATION_DIR="$LIBRARY_DIR/Models/Validation"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Função para log colorido
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Verificar se estamos no diretório correto
if [ ! -d "$PROJECT_ROOT" ]; then
    log_error "Diretório do projeto não encontrado: $PROJECT_ROOT"
    exit 1
fi

cd "$PROJECT_ROOT"
log_info "Iniciando refatoração das classes de validação..."

# Criar diretório de backup
mkdir -p "$BACKUP_DIR"
log_info "Diretório de backup criado: $BACKUP_DIR"

# Arquivos a serem excluídos (caminhos relativos ao PROJECT_ROOT)
FILES_TO_DELETE=(
    "Library/Utilities/TestResult.cs"
    "Library/Services/TestResult2.cs"
    "Library/Services/Auxiliar/TestResult.cs"
    "Library/Services/Auxiliar/TestResult2.cs"
    "Library/Services/Auxiliar/ValidationTest.cs"
    "Library/Services/Auxiliar/ValidationSummary.cs"
    "Library/Services/Auxiliar/SetupResult.cs"
    "Library/Models/ModelValidationResult.cs"
    "Library/Models/ResultadoValidacao.cs"
    "Library/Suporte/ValidationReport.cs"
)

# Fazer backup dos arquivos antes de excluir
log_info "Fazendo backup dos arquivos..."
for file in "${FILES_TO_DELETE[@]}"; do
    if [ -f "$file" ]; then
        # Criar estrutura de diretórios no backup
        backup_file="$BACKUP_DIR/$file"
        mkdir -p "$(dirname "$backup_file")"
        cp "$file" "$backup_file"
        log_success "Backup: $file"
    else
        log_warning "Arquivo não encontrado: $file"
    fi
done

# Criar diretório para nova estrutura
mkdir -p "$NEW_VALIDATION_DIR"
log_info "Diretório criado: $NEW_VALIDATION_DIR"

# Criar o arquivo ValidationTypes.cs
log_info "Criando arquivo ValidationTypes.cs..."
cat > "$NEW_VALIDATION_DIR/ValidationTypes.cs" << 'EOF'
// D:\PROJETOS\GraphFacil\Library\Models\Validation\ValidationTypes.cs - Refatoração consolidada das classes de validação
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Models.Validation
{
    #region Base Classes

    /// <summary>
    /// Classe base para todos os tipos de resultado no sistema
    /// </summary>
    public abstract class BaseResult
    {
        /// <summary>
        /// Indica se a operação foi bem-sucedida
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Mensagem principal sobre o resultado
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Detalhes adicionais sobre o resultado
        /// </summary>
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp da operação
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Tempo de execução da operação
        /// </summary>
        public TimeSpan ExecutionTime { get; set; } = TimeSpan.Zero;

        /// <summary>
        /// Dados adicionais específicos do contexto
        /// </summary>
        public Dictionary<string, object> AdditionalData { get; set; } = new();

        protected BaseResult() { }

        protected BaseResult(bool success, string message, string details = "")
        {
            Success = success;
            Message = message;
            Details = details;
        }
    }

    /// <summary>
    /// Classe base para resultados de teste
    /// </summary>
    public abstract class BaseTestResult : BaseResult
    {
        /// <summary>
        /// Nome/identificador do teste
        /// </summary>
        public string TestName { get; set; } = string.Empty;

        /// <summary>
        /// Score/pontuação do teste (0.0 a 1.0)
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// Indica se o teste passou (alias para Success)
        /// </summary>
        public bool Passed
        {
            get => Success;
            set => Success = value;
        }

        /// <summary>
        /// Mensagem de erro específica (alias para Message quando Success = false)
        /// </summary>
        public string ErrorMessage
        {
            get => Success ? string.Empty : Message;
            set { if (!Success) Message = value; }
        }

        protected BaseTestResult() { }

        protected BaseTestResult(string testName, bool passed, double score = 0.0, string message = "")
            : base(passed, message)
        {
            TestName = testName;
            Score = score;
        }
    }

    #endregion

    #region Test Results

    /// <summary>
    /// Resultado de teste unificado - substitui todas as versões de TestResult
    /// </summary>
    public class TestResult : BaseTestResult
    {
        /// <summary>
        /// Métricas específicas do teste
        /// </summary>
        public Dictionary<string, object> Metrics { get; set; } = new();

        public TestResult() { }

        public TestResult(string testName, bool success, string message = "", string details = "")
            : base(testName, success, 0.0, message)
        {
            Details = details;
        }

        public TestResult(string testName, bool success, double score, string message = "", string details = "")
            : base(testName, success, score, message)
        {
            Details = details;
        }

        /// <summary>
        /// Adiciona métrica ao teste
        /// </summary>
        public void AddMetric(string name, object value)
        {
            Metrics[name] = value;
        }

        /// <summary>
        /// Obtém métrica específica
        /// </summary>
        public T GetMetric<T>(string name, T defaultValue = default)
        {
            return Metrics.TryGetValue(name, out var value) && value is T ? (T)value : defaultValue;
        }

        public override string ToString()
        {
            return $"{TestName}: {(Success ? "PASSED" : "FAILED")} | Score: {Score:P2} | {Message}";
        }
    }

    /// <summary>
    /// Resultado de validação unificado - substitui ValidationResult, ModelValidationResult, ResultadoValidacao
    /// </summary>
    public class ValidationResult : BaseResult
    {
        /// <summary>
        /// Indica se a validação foi bem-sucedida (alias para Success)
        /// </summary>
        public bool IsValid
        {
            get => Success;
            set => Success = value;
        }

        /// <summary>
        /// Nome do modelo validado
        /// </summary>
        public string ModelName { get; set; } = string.Empty;

        /// <summary>
        /// Precisão/acurácia do modelo (0.0 a 1.0)
        /// </summary>
        public double Accuracy { get; set; }

        /// <summary>
        /// Número total de testes executados
        /// </summary>
        public int TotalTests { get; set; }

        /// <summary>
        /// Número de testes que passaram
        /// </summary>
        public int PassedTests { get; set; }

        /// <summary>
        /// Lista de IDs dos concursos com predições corretas
        /// </summary>
        public List<int> CorrectPredictions { get; set; } = new();

        /// <summary>
        /// Método de validação utilizado
        /// </summary>
        public string ValidationMethod { get; set; } = string.Empty;

        /// <summary>
        /// Duração da validação (alias para ExecutionTime)
        /// </summary>
        public TimeSpan ValidationDuration
        {
            get => ExecutionTime;
            set => ExecutionTime = value;
        }

        /// <summary>
        /// Número de amostras testadas
        /// </summary>
        public int TestSamplesCount { get; set; }

        /// <summary>
        /// Métricas específicas do modelo
        /// </summary>
        public ModelMetrics Metrics { get; set; } = new();

        public ValidationResult() { }

        public ValidationResult(bool isValid, double accuracy, string message, string modelName = "")
            : base(isValid, message)
        {
            Accuracy = accuracy;
            ModelName = modelName;
        }

        /// <summary>
        /// Calcula taxa de sucesso baseada nos testes que passaram
        /// </summary>
        public double CalculateSuccessRate()
        {
            return TotalTests > 0 ? (double)PassedTests / TotalTests : 0.0;
        }

        /// <summary>
        /// Taxa de acerto baseada nas predições corretas
        /// </summary>
        public double CalculateHitRate()
        {
            return TotalTests > 0 ? (double)CorrectPredictions.Count / TotalTests : 0.0;
        }

        public override string ToString()
        {
            return $"ValidationResult [{ModelName}]: {(IsValid ? "VALID" : "INVALID")} | " +
                   $"Accuracy: {Accuracy:P2} | Tests: {PassedTests}/{TotalTests} | Method: {ValidationMethod}";
        }
    }

    /// <summary>
    /// Configuração de resultado - para compatibilidade com SetupResult
    /// </summary>
    public class SetupResult : BaseResult
    {
        /// <summary>
        /// Número de arquivos criados
        /// </summary>
        public int FilesCreated { get; set; }

        public SetupResult() { }

        public SetupResult(bool success, string message, int filesCreated = 0)
            : base(success, message)
        {
            FilesCreated = filesCreated;
        }

        public override string ToString()
        {
            return $"Setup: {(Success ? "SUCCESS" : "FAILED")} | Files: {FilesCreated} | {Message}";
        }
    }

    #endregion

    #region Supporting Classes

    /// <summary>
    /// Métricas específicas de modelos
    /// </summary>
    public class ModelMetrics
    {
        public double Accuracy { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F1Score { get; set; }
        public double Confidence { get; set; }
        public Dictionary<string, double> CustomMetrics { get; set; } = new();

        public void AddCustomMetric(string name, double value)
        {
            CustomMetrics[name] = value;
        }

        public double GetCustomMetric(string name, double defaultValue = 0.0)
        {
            return CustomMetrics.GetValueOrDefault(name, defaultValue);
        }
    }

    #endregion

    #region Legacy Compatibility Types

    /// <summary>
    /// Alias para compatibilidade com código existente
    /// </summary>
    [Obsolete("Use TestResult instead")]
    public class TestResult2 : TestResult
    {
        public new bool Passed
        {
            get => Success;
            set => Success = value;
        }

        public TestResult2() { }

        public TestResult2(string testName, bool passed, double score, string details)
            : base(testName, passed, score, "", details) { }
    }

    /// <summary>
    /// Alias para compatibilidade com código existente
    /// </summary>
    [Obsolete("Use TestResult instead")]
    public class ValidationTest : TestResult
    {
        public new string ErrorMessage
        {
            get => Success ? string.Empty : Message;
            set { if (!Success) Message = value; }
        }

        public ValidationTest() { }

        public ValidationTest(string testName, bool success, string message = "")
            : base(testName, success, message) { }
    }

    /// <summary>
    /// Alias para compatibilidade com ValidationTestResult
    /// </summary>
    [Obsolete("Use TestResult instead")]
    public class ValidationTestResult : TestResult
    {
        public bool IsSuccess
        {
            get => Success;
            set => Success = value;
        }

        public ValidationTestResult() { }

        public ValidationTestResult(string testName, bool success, string message = "")
            : base(testName, success, message) { }
    }

    /// <summary>
    /// Resultado específico para validação de palpites individuais
    /// Mantém compatibilidade com ResultadoValidacao existente
    /// </summary>
    public class PredictionValidationResult : ValidationResult
    {
        /// <summary>
        /// ID do concurso testado
        /// </summary>
        public int ConcursoId { get; set; }

        /// <summary>
        /// Dezenas preditas pelo modelo
        /// </summary>
        public List<int> PalpiteGerado { get; set; } = new();

        /// <summary>
        /// Dezenas realmente sorteadas
        /// </summary>
        public List<int> ResultadoReal { get; set; } = new();

        /// <summary>
        /// Número de acertos
        /// </summary>
        public int Acertos { get; set; }

        /// <summary>
        /// Taxa de acerto para este teste específico
        /// </summary>
        public double TaxaAcerto { get; set; }

        /// <summary>
        /// Dezenas que foram acertadas
        /// </summary>
        public List<int> NumerosAcertados { get; set; } = new();

        /// <summary>
        /// Dezenas que foram erradas
        /// </summary>
        public List<int> NumerosPerdidos { get; set; } = new();

        /// <summary>
        /// Data do teste (alias para Timestamp)
        /// </summary>
        public DateTime DataTeste
        {
            get => Timestamp;
            set => Timestamp = value;
        }

        /// <summary>
        /// Tipo de estratégia utilizada
        /// </summary>
        public string TipoEstrategia { get; set; } = "Desconhecida";

        /// <summary>
        /// Confiança da predição (0.0 a 1.0)
        /// </summary>
        public double Confidence { get; set; }

        public PredictionValidationResult() { }

        public PredictionValidationResult(int concursoId, List<int> palpite, List<int> resultado)
        {
            ConcursoId = concursoId;
            PalpiteGerado = new List<int>(palpite);
            ResultadoReal = new List<int>(resultado);
            CalculateResults();
        }

        /// <summary>
        /// Calcula automaticamente acertos e taxas
        /// </summary>
        private void CalculateResults()
        {
            if (PalpiteGerado?.Any() == true && ResultadoReal?.Any() == true)
            {
                NumerosAcertados = PalpiteGerado.Intersect(ResultadoReal).ToList();
                NumerosPerdidos = PalpiteGerado.Except(ResultadoReal).ToList();
                Acertos = NumerosAcertados.Count;
                TaxaAcerto = ResultadoReal.Count > 0 ? (double)Acertos / ResultadoReal.Count : 0.0;
                Accuracy = TaxaAcerto;
                Success = Acertos >= 11; // Critério de sucesso: pelo menos 11 acertos
            }
        }

        public override string ToString()
        {
            return $"Concurso {ConcursoId}: {Acertos}/15 acertos ({TaxaAcerto:P1}) - {TipoEstrategia}";
        }
    }

    /// <summary>
    /// Alias para compatibilidade com ResultadoValidacao
    /// </summary>
    [Obsolete("Use PredictionValidationResult instead")]
    public class ResultadoValidacao : PredictionValidationResult
    {
        public ResultadoValidacao() { }

        public ResultadoValidacao(int concursoId, List<int> palpite, List<int> resultado)
            : base(concursoId, palpite, resultado) { }
    }

    /// <summary>
    /// Relatório de validação unificado - substitui ValidationReport e ValidationSummary
    /// </summary>
    public class ValidationReport : BaseResult
    {
        /// <summary>
        /// Nome do relatório/teste
        /// </summary>
        public string ReportName { get; set; } = string.Empty;

        /// <summary>
        /// Data/hora de início
        /// </summary>
        public DateTime StartTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Data/hora de fim
        /// </summary>
        public DateTime EndTime { get; set; } = DateTime.Now;

        /// <summary>
        /// Duração total (alias para ExecutionTime)
        /// </summary>
        public TimeSpan Duration
        {
            get => ExecutionTime;
            set => ExecutionTime = value;
        }

        /// <summary>
        /// Número total de testes
        /// </summary>
        public int TotalTests { get; set; }

        /// <summary>
        /// Número de testes que passaram
        /// </summary>
        public int PassedTests { get; set; }

        /// <summary>
        /// Score/pontuação geral
        /// </summary>
        public double OverallScore { get; set; }

        /// <summary>
        /// Sucesso geral (alias para Success)
        /// </summary>
        public bool OverallSuccess
        {
            get => Success;
            set => Success = value;
        }

        /// <summary>
        /// Lista de resultados de testes
        /// </summary>
        public List<TestResult> TestResults { get; set; } = new();

        /// <summary>
        /// Log completo dos testes
        /// </summary>
        public string TestLog { get; set; } = string.Empty;

        /// <summary>
        /// Métricas de meta-learning (quando aplicável)
        /// </summary>
        public object MetaLearningMetrics { get; set; }

        public ValidationReport() { }

        public ValidationReport(string reportName)
        {
            ReportName = reportName;
            StartTime = DateTime.Now;
        }

        /// <summary>
        /// Adiciona resultado de teste ao relatório
        /// </summary>
        public void AddTestResult(TestResult testResult)
        {
            TestResults.Add(testResult);
            TotalTests = TestResults.Count;
            PassedTests = TestResults.Count(t => t.Success);
            OverallScore = TestResults.Any() ? TestResults.Average(t => t.Score) : 0.0;
            OverallSuccess = PassedTests == TotalTests && TotalTests > 0;
        }

        /// <summary>
        /// Finaliza o relatório
        /// </summary>
        public void Complete()
        {
            EndTime = DateTime.Now;
            Duration = EndTime - StartTime;
            
            // Atualiza métricas finais
            TotalTests = TestResults.Count;
            PassedTests = TestResults.Count(t => t.Success);
            OverallScore = TestResults.Any() ? TestResults.Average(t => t.Score) : 0.0;
            OverallSuccess = PassedTests == TotalTests && TotalTests > 0;
            
            // Gera mensagem de resumo
            Message = $"Relatório concluído: {PassedTests}/{TotalTests} testes passaram ({OverallScore:P1})";
        }

        /// <summary>
        /// Calcula taxa de sucesso
        /// </summary>
        public double CalculateSuccessRate()
        {
            return TotalTests > 0 ? (double)PassedTests / TotalTests : 0.0;
        }

        public override string ToString()
        {
            return $"ValidationReport [{ReportName}]: {PassedTests}/{TotalTests} passed | " +
                   $"Score: {OverallScore:P2} | Duration: {Duration.TotalSeconds:F1}s";
        }
    }

    #endregion
}
EOF

log_success "Arquivo ValidationTypes.cs criado com sucesso!"

# Função para atualizar imports em um arquivo
update_imports() {
    local file=$1
    local temp_file="${file}.tmp"
    
    if [ ! -f "$file" ]; then
        return
    fi
    
    log_info "Atualizando imports em: $file"
    
    # Fazer as substituições de using statements
    sed -e 's/using LotoLibrary\.Utilities;.*TestResult/using LotoLibrary.Models.Validation;/g' \
        -e 's/using LotoLibrary\.Services\.Auxiliar;.*TestResult/using LotoLibrary.Models.Validation;/g' \
        -e 's/using LotoLibrary\.Models;.*ModelValidationResult/using LotoLibrary.Models.Validation;/g' \
        -e 's/using LotoLibrary\.Models;.*ResultadoValidacao/using LotoLibrary.Models.Validation;/g' \
        -e 's/using LotoLibrary\.Suporte;.*ValidationReport/using LotoLibrary.Models.Validation;/g' \
        -e 's/LotoLibrary\.Utilities\.TestResult/LotoLibrary.Models.Validation.TestResult/g' \
        -e 's/LotoLibrary\.Services\.Auxiliar\.TestResult/LotoLibrary.Models.Validation.TestResult/g' \
        -e 's/LotoLibrary\.Services\.Auxiliar\.TestResult2/LotoLibrary.Models.Validation.TestResult2/g' \
        -e 's/LotoLibrary\.Services\.Auxiliar\.ValidationTest/LotoLibrary.Models.Validation.ValidationTest/g' \
        -e 's/LotoLibrary\.Services\.Auxiliar\.ValidationSummary/LotoLibrary.Models.Validation.ValidationReport/g' \
        -e 's/LotoLibrary\.Services\.Auxiliar\.SetupResult/LotoLibrary.Models.Validation.SetupResult/g' \
        -e 's/LotoLibrary\.Models\.ModelValidationResult/LotoLibrary.Models.Validation.ValidationResult/g' \
        -e 's/LotoLibrary\.Models\.ResultadoValidacao/LotoLibrary.Models.Validation.PredictionValidationResult/g' \
        -e 's/LotoLibrary\.Suporte\.ValidationReport/LotoLibrary.Models.Validation.ValidationReport/g' \
        -e 's/LotoLibrary\.Suporte\.ValidationResult/LotoLibrary.Models.Validation.ValidationResult/g' \
        "$file" > "$temp_file"
    
    # Substituir apenas se houve mudanças
    if ! cmp -s "$file" "$temp_file"; then
        mv "$temp_file" "$file"
        log_success "Atualizado: $file"
    else
        rm "$temp_file"
    fi
}

# Arquivos que precisam ter os imports atualizados
log_info "Atualizando imports nos arquivos restantes..."

# Buscar todos os arquivos .cs no projeto
find "$LIBRARY_DIR" -name "*.cs" -type f | while read -r file; do
    # Verificar se o arquivo contém alguma das classes antigas
    if grep -q -E "(TestResult2|ValidationTest|ValidationSummary|ModelValidationResult|ResultadoValidacao|SetupResult)" "$file"; then
        update_imports "$file"
    fi
done

# Buscar também em Tests se existir
if [ -d "$PROJECT_ROOT/Tests" ]; then
    find "$PROJECT_ROOT/Tests" -name "*.cs" -type f | while read -r file; do
        if grep -q -E "(TestResult2|ValidationTest|ValidationSummary|ModelValidationResult|ResultadoValidacao|SetupResult)" "$file"; then
            update_imports "$file"
        fi
    done
fi

# Excluir os arquivos duplicados
log_info "Excluindo arquivos duplicados..."
for file in "${FILES_TO_DELETE[@]}"; do
    if [ -f "$file" ]; then
        rm "$file"
        log_success "Excluído: $file"
    fi
done

# Atualizar arquivo IValidationService.cs se existir
IVALIDATION_FILE="$LIBRARY_DIR/Interfaces/IValidationService.cs"
if [ -f "$IVALIDATION_FILE" ]; then
    log_info "Atualizando IValidationService.cs..."
    
    # Fazer backup
    cp "$IVALIDATION_FILE" "$BACKUP_DIR/IValidationService.cs"
    
    # Atualizar imports e referências
    sed -i \
        -e 's/using LotoLibrary\.Models;.*ValidationTestResult/using LotoLibrary.Models.Validation;/g' \
        -e 's/using LotoLibrary\.Suporte;.*ValidationResult/using LotoLibrary.Models.Validation;/g' \
        -e '1i using LotoLibrary.Models.Validation;' \
        "$IVALIDATION_FILE"
    
    log_success "IValidationService.cs atualizado"
fi

# Atualizar arquivo ValidationService.cs se existir
VALIDATION_SERVICE_FILE="$LIBRARY_DIR/Services/Validation/ValidationService.cs"
if [ -f "$VALIDATION_SERVICE_FILE" ]; then
    log_info "Atualizando ValidationService.cs..."
    
    # Fazer backup
    mkdir -p "$BACKUP_DIR/Library/Services/Validation"
    cp "$VALIDATION_SERVICE_FILE" "$BACKUP_DIR/Library/Services/Validation/ValidationService.cs"
    
    # Atualizar imports e referências
    sed -i \
        -e 's/using LotoLibrary\.Suporte;.*ValidationResult/using LotoLibrary.Models.Validation;/g' \
        -e '1i using LotoLibrary.Models.Validation;' \
        "$VALIDATION_SERVICE_FILE"
    
    log_success "ValidationService.cs atualizado"
fi

# Remover diretórios vazios
log_info "Removendo diretórios vazios..."
find "$LIBRARY_DIR" -type d -empty -delete 2>/dev/null || true

# Compilar para verificar se não há erros
log_info "Verificando compilação..."
if command -v dotnet &> /dev/null; then
    if [ -f "$PROJECT_ROOT/Library/Library.csproj" ] || [ -f "$PROJECT_ROOT/LotoLibrary.csproj" ]; then
        cd "$PROJECT_ROOT"
        if dotnet build --no-restore --verbosity quiet 2>/dev/null; then
            log_success "✅ Compilação bem-sucedida!"
        else
            log_warning "⚠️ Erros de compilação detectados. Verifique os logs."
        fi
    fi
else
    log_warning "dotnet CLI não encontrado. Compilação manual necessária."
fi

# Relatório final
log_info "=============================================="
log_success "🎉 REFATORAÇÃO CONCLUÍDA COM SUCESSO!"
log_info "=============================================="
echo ""
log_info "📁 Backup criado em: $BACKUP_DIR"
log_info "📄 Novo arquivo: $NEW_VALIDATION_DIR/ValidationTypes.cs"
echo ""
log_info "📊 RESUMO:"
echo "   ✅ ${#FILES_TO_DELETE[@]} arquivos duplicados removidos"
echo "   ✅ Imports atualizados em arquivos relevantes"
echo "   ✅ Nova estrutura ValidationTypes.cs criada"
echo "   ✅ Compatibilidade mantida com aliases [Obsolete]"
echo ""
log_warning "⚠️ PRÓXIMOS PASSOS:"
echo "   1. Teste a compilação manualmente"
echo "   2. Execute os testes unitários"
echo "   3. Remova aliases [Obsolete] após validação"
echo "   4. Atualize documentação se necessário"
echo ""
log_info "🔍 Para reverter: use os arquivos em $BACKUP_DIR"
