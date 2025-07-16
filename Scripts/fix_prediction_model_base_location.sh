# D:\PROJETOS\GraphFacil\Scripts\fix_prediction_model_base_location.sh
#!/bin/bash

# Correção da localização da PredictionModelBase
# Manter: Library/Models/Base/PredictionModelBase.cs
# Remover: Library/PredictionModels/Base/PredictionModelBase.cs

PROJECT_ROOT="E:/PROJETOS/GraphFacil"
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

log_info "🔧 Corrigindo localização da PredictionModelBase..."

# Caminhos dos arquivos
EXISTING_FILE="$LIBRARY_DIR/Models/Base/PredictionModelBase.cs"
NEW_FILE="$LIBRARY_DIR/PredictionModels/Base/PredictionModelBase.cs"

# Verificar se ambos existem
if [ -f "$EXISTING_FILE" ] && [ -f "$NEW_FILE" ]; then
    log_warning "Ambos os arquivos existem. Analisando diferenças..."
    
    # Comparar arquivos
    if cmp -s "$EXISTING_FILE" "$NEW_FILE"; then
        log_info "Arquivos são idênticos. Removendo duplicata..."
        rm "$NEW_FILE"
        log_success "Arquivo duplicado removido: $NEW_FILE"
    else
        log_warning "Arquivos são diferentes. Fazendo backup e mesclando..."
        
        # Fazer backup do arquivo existente
        cp "$EXISTING_FILE" "$EXISTING_FILE.backup"
        log_info "Backup criado: $EXISTING_FILE.backup"
        
        # Verificar qual é mais recente/completo
        existing_lines=$(wc -l < "$EXISTING_FILE")
        new_lines=$(wc -l < "$NEW_FILE")
        
        if [ "$new_lines" -gt "$existing_lines" ]; then
            log_info "Arquivo novo é mais completo ($new_lines vs $existing_lines linhas)"
            log_info "Substituindo arquivo existente pelo novo..."
            
            # Substituir pelo arquivo mais completo
            cp "$NEW_FILE" "$EXISTING_FILE"
            log_success "Arquivo existente atualizado com versão mais completa"
        else
            log_info "Arquivo existente é mais completo ou igual. Mantendo existente."
        fi
        
        # Remover arquivo da nova localização
        rm "$NEW_FILE"
        log_success "Arquivo removido da nova localização: $NEW_FILE"
    fi
    
elif [ -f "$NEW_FILE" ] && [ ! -f "$EXISTING_FILE" ]; then
    log_info "Apenas arquivo novo existe. Movendo para localização correta..."
    
    # Criar diretório se não existir
    mkdir -p "$(dirname "$EXISTING_FILE")"
    
    # Mover arquivo para localização correta
    mv "$NEW_FILE" "$EXISTING_FILE"
    log_success "Arquivo movido para localização correta: $EXISTING_FILE"
    
elif [ -f "$EXISTING_FILE" ] && [ ! -f "$NEW_FILE" ]; then
    log_success "Apenas arquivo existente presente. Nenhuma ação necessária."
    
else
    log_error "Nenhum arquivo PredictionModelBase encontrado!"
    exit 1
fi

# Atualizar referências nos arquivos que podem estar usando o caminho errado
log_info "🔍 Atualizando referências nos arquivos..."

# Buscar e atualizar referências
find "$LIBRARY_DIR" -name "*.cs" -type f -exec grep -l "LotoLibrary.PredictionModels.Base" {} \; | while read -r file; do
    log_info "Atualizando referência em: $file"
    sed -i 's/using LotoLibrary\.PredictionModels\.Base;/using LotoLibrary.Models.Base;/g' "$file"
    sed -i 's/LotoLibrary\.PredictionModels\.Base\.PredictionModelBase/LotoLibrary.Models.Base.PredictionModelBase/g' "$file"
    log_success "Referência atualizada: $file"
done

# Remover diretório vazio se existir
if [ -d "$LIBRARY_DIR/PredictionModels/Base" ] && [ ! "$(ls -A "$LIBRARY_DIR/PredictionModels/Base")" ]; then
    rmdir "$LIBRARY_DIR/PredictionModels/Base"
    log_success "Diretório vazio removido: $LIBRARY_DIR/PredictionModels/Base"
fi

# Verificar se o arquivo final existe e está correto
if [ -f "$EXISTING_FILE" ]; then
    log_success "✅ PredictionModelBase.cs está na localização correta:"
    log_success "   $EXISTING_FILE"
    
    # Verificar namespace
    if grep -q "namespace LotoLibrary.Models.Base" "$EXISTING_FILE"; then
        log_success "✅ Namespace correto: LotoLibrary.Models.Base"
    else
        log_warning "⚠️ Corrigindo namespace..."
        sed -i 's/namespace LotoLibrary\.PredictionModels\.Base/namespace LotoLibrary.Models.Base/g' "$EXISTING_FILE"
        log_success "✅ Namespace corrigido"
    fi
    
    # Exibir estatísticas do arquivo
    lines=$(wc -l < "$EXISTING_FILE")
    log_info "📊 Arquivo final: $lines linhas"
    
else
    log_error "❌ Arquivo PredictionModelBase.cs não encontrado na localização esperada!"
    exit 1
fi

log_success "🎉 Correção de localização concluída!"
echo ""
log_info "📋 RESUMO:"
echo "   ✅ Localização correta: Library/Models/Base/PredictionModelBase.cs"
echo "   ✅ Namespace correto: LotoLibrary.Models.Base"
echo "   ✅ Referências atualizadas"
echo "   ✅ Arquivos duplicados removidos"
echo ""
log_info "🎯 PRÓXIMOS PASSOS:"
echo "   1. Verificar se os imports nos modelos estão corretos"
echo "   2. Compilar o projeto para validar"
echo "   3. Testar funcionalidades dos modelos"