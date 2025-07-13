#!/bin/bash
# D:\PROJETOS\GraphFacil\Library\Scripts\correcao_automatica.sh - Script para correções automáticas

echo "=== SCRIPT DE CORREÇÃO AUTOMÁTICA ==="
echo "Iniciando correções de erros simples..."

# Definir variáveis de caminho
BASE_DIR="D:/PROJETOS/GraphFacil/Library"
BACKUP_DIR="D:/PROJETOS/GraphFacil/Backup_$(date +%Y%m%d_%H%M%S)"

# Função para criar backup
create_backup() {
    echo "📦 Criando backup em: $BACKUP_DIR"
    mkdir -p "$BACKUP_DIR"
    cp -r "$BASE_DIR"/* "$BACKUP_DIR/" 2>/dev/null || true
    echo "✅ Backup criado com sucesso"
}

# Função para limpar arquivos duplicados
clean_duplicates() {
    echo "🧹 Removendo arquivos duplicados..."
    
    # Remover StrategyRecommendation duplicado
    DUPLICATE_FILE="$BASE_DIR/PredictionModels/Ensemble/StrategyRecommendation.cs"
    if [ -f "$DUPLICATE_FILE" ]; then
        rm "$DUPLICATE_FILE"
        echo "   ✅ Removido: StrategyRecommendation.cs duplicado"
    fi
    
    # Remover outros possíveis duplicados
    find "$BASE_DIR" -name "*.cs.bak" -delete 2>/dev/null || true
    find "$BASE_DIR" -name "*.cs~" -delete 2>/dev/null || true
    find "$BASE_DIR" -name "Thumbs.db" -delete 2>/dev/null || true
    
    echo "   ✅ Limpeza de duplicados concluída"
}

# Função para corrigir estrutura de pastas
fix_folder_structure() {
    echo "📁 Corrigindo estrutura de pastas..."
    
    # Criar pastas necessárias se não existirem
    FOLDERS=(
        "$BASE_DIR/Models/Prediction"
        "$BASE_DIR/Models/Configuration" 
        "$BASE_DIR/Models/Base"
        "$BASE_DIR/Interfaces"
        "$BASE_DIR/Services/Analysis"
        "$BASE_DIR/Services/Utilities"
        "$BASE_DIR/PredictionModels/Base"
        "$BASE_DIR/PredictionModels/Individual"
        "$BASE_DIR/PredictionModels/AntiFrequency/Simple"
        "$BASE_DIR/PredictionModels/AntiFrequency/Statistical"
        "$BASE_DIR/PredictionModels/Ensemble"
    )
    
    for folder in "${FOLDERS[@]}"; do
        if [ ! -d "$folder" ]; then
            mkdir -p "$folder"
            echo "   ✅ Criada pasta: $(basename "$folder")"
        fi
    done
}

# Função para corrigir permissões de arquivos
fix_permissions() {
    echo "🔐 Corrigindo permissões de arquivos..."
    
    # Corrigir permissões dos arquivos .cs
    find "$BASE_DIR" -name "*.cs" -exec chmod 644 {} \; 2>/dev/null || true
    
    # Corrigir permissões das pastas
    find "$BASE_DIR" -type d -exec chmod 755 {} \; 2>/dev/null || true
    
    echo "   ✅ Permissões corrigidas"
}

# Função para adicionar headers padronizados
add_file_headers() {
    echo "📝 Adicionando headers padronizados..."
    
    # Processar todos os arquivos .cs que não têm header
    find "$BASE_DIR" -name "*.cs" -type f | while read -r file; do
        # Verificar se já tem header com caminho
        if ! head -1 "$file" | grep -q "^// D:" 2>/dev/null; then
            # Calcular caminho relativo
            rel_path=${file#$BASE_DIR/}
            rel_path="D:\\PROJETOS\\GraphFacil\\Library\\${rel_path//\//\\}"
            
            # Criar arquivo temporário com header
            temp_file=$(mktemp)
            echo "// $rel_path" > "$temp_file"
            cat "$file" >> "$temp_file"
            mv "$temp_file" "$file"
            
            echo "   ✅ Header adicionado: $(basename "$file")"
        fi
    done
}

# Função para validar sintaxe básica
validate_syntax() {
    echo "🔍 Validando sintaxe básica..."
    
    error_count=0
    
    # Verificar parênteses balanceados
    find "$BASE_DIR" -name "*.cs" -type f | while read -r file; do
        # Contar chaves
        open_braces=$(grep -o '{' "$file" | wc -l)
        close_braces=$(grep -o '}' "$file" | wc -l)
        
        if [ "$open_braces" -ne "$close_braces" ]; then
            echo "   ⚠️  Chaves desbalanceadas em: $(basename "$file")"
            ((error_count++))
        fi
        
        # Verificar using statements básicos
        if grep -q "namespace" "$file" && ! grep -q "using System" "$file"; then
            echo "   ⚠️  Faltando 'using System' em: $(basename "$file")"
        fi
    done
    
    if [ $error_count -eq 0 ]; then
        echo "   ✅ Validação de sintaxe passou"
    else
        echo "   ⚠️  Encontrados $error_count problemas de sintaxe"
    fi
}

# Função para organizar imports
organize_imports() {
    echo "📚 Organizando imports..."
    
    find "$BASE_DIR" -name "*.cs" -type f | while read -r file; do
        # Criar arquivo temporário
        temp_file=$(mktemp)
        
        # Extrair header de comentário
        head -1 "$file" | grep "^//" > "$temp_file" 2>/dev/null || true
        
        # Extrair e organizar usings
        {
            grep "^using System" "$file" 2>/dev/null | sort -u
            grep "^using " "$file" | grep -v "^using System" 2>/dev/null | sort -u
        } >> "$temp_file"
        
        # Adicionar linha em branco se existem usings
        if grep -q "^using " "$temp_file"; then
            echo "" >> "$temp_file"
        fi
        
        # Adicionar resto do arquivo (sem header e usings)
        awk '
        BEGIN { in_usings = 0; header_done = 0 }
        /^\/\// && !header_done { header_done = 1; next }
        /^using / { in_usings = 1; next }
        /^$/ && in_usings { in_usings = 0; next }
        !in_usings && header_done { print }
        ' "$file" >> "$temp_file"
        
        # Substituir arquivo original se houve mudanças
        if ! cmp -s "$file" "$temp_file"; then
            mv "$temp_file" "$file"
            echo "   ✅ Imports organizados: $(basename "$file")"
        else
            rm "$temp_file"
        fi
    done
}

# Função para aplicar correções de texto simples
apply_text_corrections() {
    echo "✏️  Aplicando correções de texto..."
    
    find "$BASE_DIR" -name "*.cs" -type f | while read -r file; do
        # Backup do arquivo
        cp "$file" "$file.bak"
        
        # Aplicar correções comuns
        sed -i 's/public class \([A-Za-z]*Model\)/public partial class \1/g' "$file" 2>/dev/null || true
        sed -i 's/: PredictionModelBase$/: ObservableObject/g' "$file" 2>/dev/null || true
        sed -i 's/string\.Empty/string.Empty/g' "$file" 2>/dev/null || true
        
        # Verificar se houve mudanças
        if ! cmp -s "$file" "$file.bak"; then
            echo "   ✅ Correções aplicadas: $(basename "$file")"
        fi
        
        # Remover backup
        rm "$file.bak"
    done
}

# Função para gerar relatório de status
generate_status_report() {
    echo "📊 Gerando relatório de status..."
    
    REPORT_FILE="$BASE_DIR/Scripts/relatorio_correcao_$(date +%Y%m%d_%H%M%S).txt"
    mkdir -p "$(dirname "$REPORT_FILE")"
    
    {
        echo "=== RELATÓRIO DE CORREÇÃO AUTOMÁTICA ==="
        echo "Data: $(date)"
        echo "Diretório: $BASE_DIR"
        echo ""
        
        echo "📊 ESTATÍSTICAS:"
        echo "- Arquivos .cs encontrados: $(find "$BASE_DIR" -name "*.cs" | wc -l)"
        echo "- Pastas criadas/verificadas: ${#FOLDERS[@]}"
        echo "- Backup criado em: $BACKUP_DIR"
        echo ""
        
        echo "🔍 ARQUIVOS PROCESSADOS:"
        find "$BASE_DIR" -name "*.cs" -type f | head -20
        if [ $(find "$BASE_DIR" -name "*.cs" | wc -l) -gt 20 ]; then
            echo "... e mais $(( $(find "$BASE_DIR" -name "*.cs" | wc -l) - 20 )) arquivos"
        fi
        echo ""
        
        echo "⚠️  PRÓXIMOS PASSOS:"
        echo "1. Executar o script C# de correção completa"
        echo "2. Compilar projeto para verificar erros restantes"
        echo "3. Executar testes de validação"
        echo "4. Verificar funcionalidade dos modelos"
        
    } > "$REPORT_FILE"
    
    echo "   ✅ Relatório salvo em: $REPORT_FILE"
}

# Função principal
main() {
    echo "🚀 Iniciando processo de correção automática..."
    echo ""
    
    # Executar todas as correções
    create_backup
    echo ""
    
    clean_duplicates
    echo ""
    
    fix_folder_structure
    echo ""
    
    fix_permissions
    echo ""
    
    add_file_headers
    echo ""
    
    organize_imports
    echo ""
    
    apply_text_corrections
    echo ""
    
    validate_syntax
    echo ""
    
    generate_status_report
    echo ""
    
    echo "✅ CORREÇÃO AUTOMÁTICA CONCLUÍDA!"
    echo ""
    echo "📋 PRÓXIMOS PASSOS:"
    echo "1. Execute o script C# CorrecaoCompleta.ExecutarCorrecaoCompleta()"
    echo "2. Compile o projeto (Build → Rebuild Solution)"
    echo "3. Verifique se ainda existem erros"
    echo "4. Execute testes de validação"
    echo ""
    echo "📁 Backup criado em: $BACKUP_DIR"
}

# Verificar se está rodando no diretório correto
if [ ! -d "$BASE_DIR" ]; then
    echo "❌ ERRO: Diretório base não encontrado: $BASE_DIR"
    echo "Por favor, ajuste a variável BASE_DIR no script"
    exit 1
fi

# Executar função principal
main

exit 0