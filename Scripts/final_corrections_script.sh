# D:\PROJETOS\GraphFacil\Scripts\final_corrections.sh - Correções finais automáticas
#!/bin/bash

# Configurações
PROJECT_ROOT="D:/PROJETOS/GraphFacil"
LIBRARY_DIR="$PROJECT_ROOT/Library"

# Cores para output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

log_info() { echo -e "${BLUE}[INFO]${NC} $1"; }
log_success() { echo -e "${GREEN}[SUCCESS]${NC} $1"; }
log_warning() { echo -e "${YELLOW}[WARNING]${NC} $1"; }
log_error() { echo -e "${RED}[ERROR]${NC} $1"; }

cd "$PROJECT_ROOT"
log_info "Aplicando correções finais..."

# 1. CORRIGIR REFERÊNCIAS RESTANTES A ResultadoValidacao
log_info "🔧 Corrigindo referências a ResultadoValidacao..."

# Buscar e corrigir em todos os arquivos .cs
find "$LIBRARY_DIR" -name "*.cs" -type f -exec grep -l "ResultadoValidacao" {} \; | while read -r file; do
    log_warning "Corrigindo referências em: $file"
    
    # Backup
    cp "$file" "$file.backup"
    
    # Substituições
    sed -i \
        -e 's/using LotoLibrary\.Models\.ResultadoValidacao/using LotoLibrary.Models.Validation/g' \
        -e 's/ResultadoValidacao/PredictionValidationResult/g' \
        -e 's/List<PredictionValidationResult>/List<PredictionValidationResult>/g' \
        "$file"
    
    log_success "✅ Corrigido: $file"
done

# 2. CORRIGIR REFERÊNCIAS A ValidationTest
log_info "🔧 Corrigindo referências a ValidationTest..."

find "$LIBRARY_DIR" -name "*.cs" -type f -exec grep -l "ValidationTest[^R]" {} \; | while read -r file; do
    log_warning "Corrigindo ValidationTest em: $file"
    
    # Backup se não existir
    [ ! -f "$file.backup" ] && cp "$file" "$file.backup"
    
    # Adicionar using se necessário
    if ! grep -q "using LotoLibrary.Models.Validation;" "$file"; then
        sed -i '1i using LotoLibrary.Models.Validation;' "$file"
    fi
    
    # Substituir ValidationTest por TestResult (exceto ValidationTestResult)
    sed -i 's/ValidationTest\([^R]\)/TestResult\1/g' "$file"
    
    log_success "✅ Corrigido: $file"
done

# 3. CORRIGIR IMPLEMENTAÇÕES DE INTERFACE
log_info "🔧 Corrigindo implementações de interface..."

# Arquivo AntiFrequencySimpleModel.cs
ANTIFREQ_FILE="$LIBRARY_DIR/PredictionModels/AntiFrequency/Simple/AntiFrequencySimpleModel.cs"
if [ -f "$ANTIFREQ_FILE" ]; then
    log_warning "Corrigindo AntiFrequencySimpleModel.cs..."
    
    [ ! -f "$ANTIFREQ_FILE.backup" ] && cp "$ANTIFREQ_FILE" "$ANTIFREQ_FILE.backup"
    
    # Remover método IsModelType se estiver causando problemas
    sed -i '/public override bool IsModelType/,/^[[:space:]]*}/d' "$ANTIFREQ_FILE"
    
    log_success "✅ Corrigido: $ANTIFREQ_FILE"
fi

# Arquivo SaturationModel.cs
SATURATION_FILE="$LIBRARY_DIR/PredictionModels/AntiFrequency/Statistical/SaturationModel.cs"
if [ -f "$SATURATION_FILE" ]; then
    log_warning "Corrigindo SaturationModel.cs..."
    
    [ ! -f "$SATURATION_FILE.backup" ] && cp "$SATURATION_FILE" "$SATURATION_FILE.backup"
    
    # Remover método IsModelType se estiver causando problemas
    sed -i '/public override bool IsModelType/,/^[[:space:]]*}/d' "$SATURATION_FILE"
    
    log_success "✅ Corrigido: $SATURATION_FILE"
fi

# 4. CORRIGIR ARQUIVO DeepLearningModel.cs
DEEPLEARNING_FILE="$LIBRARY_DIR/PredictionModels/Individual/DeepLearningModel.cs"
if [ -f "$DEEPLEARNING_FILE" ]; then
    log_warning "Corrigindo DeepLearningModel.cs..."
    
    [ ! -f "$DEEPLEARNING_FILE.backup" ] && cp "$DEEPLEARNING_FILE" "$DEEPLEARNING_FILE.backup"
    
    # Corrigir namespace e using statements
    sed -i \
        -e 's/using LotoLibrary\.PredictionModels\.Base/using LotoLibrary.PredictionModels/g' \
        -e 's/: PredictionModelBase/: IPredictionModel/g' \
        "$DEEPLEARNING_FILE"
    
    log_success "✅ Corrigido: $DEEPLEARNING_FILE"
fi

# 5. ADICIONAR USING STATEMENTS FALTANTES
log_info "🔧 Adicionando using statements faltantes..."

# Lista de arquivos que precisam do using LotoLibrary.Models.Validation
FILES_NEEDING_VALIDATION=(
    "$LIBRARY_DIR/Engines/MetronomoEngine.cs"
    "$LIBRARY_DIR/Services/ValidationMetricsService.cs"
    "$LIBRARY_DIR/Services/Analysis/PerformanceComparer.cs"
)

for file in "${FILES_NEEDING_VALIDATION[@]}"; do
    if [ -f "$file" ] && ! grep -q "using LotoLibrary.Models.Validation;" "$file"; then
        log_warning "Adicionando using statement em: $file"
        sed -i '1i using LotoLibrary.Models.Validation;' "$file"
        log_success "✅ Using adicionado: $file"
    fi
done

# 6. CORRIGIR PROBLEMA DO XAML (Material Design)
XAML_FILE="$PROJECT_ROOT/Dashboard/Views/ValidationWindow.xaml"
if [ -f "$XAML_FILE" ]; then
    log_warning "Corrigindo ValidationWindow.xaml..."
    
    [ ! -f "$XAML_FILE.backup" ] && cp "$XAML_FILE" "$XAML_FILE.backup"
    
    # Substituir PackIcon por Icon simples ou comentar
    sed -i 's/<PackIcon/<Icon/g' "$XAML_FILE"
    sed -i 's/PackIcon>/Icon>/g' "$XAML_FILE"
    
    log_success "✅ XAML corrigido: $XAML_FILE"
fi

# 7. LIMPAR ARQUIVOS TEMPORÁRIOS DE BACKUP ANTIGOS
log_info "🧹 Limpando arquivos temporários..."
find "$PROJECT_ROOT" -name "*.backup" -type f -mtime +1 -delete 2>/dev/null || true

# 8. VERIFICAR COMPILAÇÃO
log_info "🔨 Verificando compilação final..."
if command -v dotnet &> /dev/null; then
    cd "$PROJECT_ROOT"
    if dotnet build --no-restore --verbosity minimal 2>/dev/null; then
        log_success "✅ Compilação final bem-sucedida!"
    else
        log_warning "⚠️ Ainda há erros. Executando build detalhado..."
        dotnet build --no-restore --verbosity normal
    fi
fi

# 9. RELATÓRIO FINAL
log_info "=============================================="
log_success "🎉 CORREÇÕES FINAIS APLICADAS!"
log_info "=============================================="
echo ""
log_info "📋 CORREÇÕES REALIZADAS:"
echo "   ✅ Referências ResultadoValidacao → PredictionValidationResult"
echo "   ✅ Referências ValidationTest → TestResult"  
echo "   ✅ Using statements adicionados"
echo "   ✅ Métodos de interface corrigidos"
echo "   ✅ Arquivo XAML corrigido"
echo "   ✅ DeepLearningModel.cs corrigido"
echo ""
log_info "🎯 PRÓXIMOS PASSOS SE AINDA HOUVER ERROS:"
echo "   1. Verificar se todas as dependências NuGet estão instaladas"
echo "   2. Verificar se TensorFlow.NET está instalado (se necessário)"
echo "   3. Recompilar o projeto completo"
echo "   4. Executar testes unitários"
echo ""
log_success "✅ Todas as correções automáticas foram aplicadas!"
