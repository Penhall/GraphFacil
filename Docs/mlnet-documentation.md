# Implementação ML.NET para Análise de Loterias

## Visão Geral
Este projeto implementa uma solução de machine learning usando ML.NET para analisar padrões em jogos de loteria. A solução substitui a implementação anterior baseada em TensorFlow.NET, oferecendo melhor integração com o ecossistema .NET e performance otimizada.

## Componentes Principais

### 1. MLNetModel
Principal classe responsável pelo treinamento e predição, com suporte para dois tipos de análise:
- SS (Subgrupos Sorteados): 7 dimensões de features
- NS (Números Não Sorteados): 5 dimensões de features

### 2. Estrutura de Classes
```plaintext
MLNetModel
├── SubgrupoMLFeatures (Base)
├── SubgrupoSSFeatures (7 dimensões)
└── SubgrupoNSFeatures (5 dimensões)
```

### 3. Pipeline de Processamento
1. Carregamento de dados históricos
2. Separação em treino/validação
3. Geração de percentuais
4. Treinamento do modelo
5. Validação cruzada
6. Avaliação de métricas

## Como Usar

### 1. Inicialização
```csharp
var logger = new MLLogger(/* configuração */);
var modelSS = new MLNetModel(logger, "ModeloSS.zip", "SS");
var modelNS = new MLNetModel(logger, "ModeloNS.zip", "NS");
```

### 2. Treinamento
```csharp
// Treinar modelos
modelSS.Train("PercentuaisSS.json", usarCrossValidation: true);
modelNS.Train("PercentuaisNS.json", usarCrossValidation: true);
```

### 3. Predição
```csharp
float[] features = /* array de features */;
float predicaoSS = modelSS.Predict(features);
float predicaoNS = modelNS.Predict(features);
```

## Configuração e Parâmetros

### 1. Hiperparâmetros
- NumberOfTrees: 100 (padrão)
- NumberOfLeaves: 20 (padrão)
- MinimumExampleCountPerLeaf: 10 (padrão)

### 2. Validação Cruzada
- Número de folds: 5 (padrão)
- Métricas avaliadas: R², RMSE, MAE

### 3. Arquivos de Dados
- PercentuaisSS.json: Percentuais para subgrupos sorteados
- PercentuaisNS.json: Percentuais para números não sorteados

## Sistema de Logging

### 1. Eventos Registrados
- Inicialização do modelo
- Preparação de dados
- Treinamento
- Validação cruzada
- Métricas de avaliação
- Erros e exceções

### 2. Métricas Monitoradas
- R² (Coeficiente de determinação)
- RMSE (Root Mean Square Error)
- MAE (Mean Absolute Error)
- MSE (Mean Squared Error)

## Fluxo de Processamento

### 1. Preparação de Dados
```plaintext
Dados Brutos → Percentuais → Normalização → Features
```

### 2. Treinamento
```plaintext
Features → Validação Cruzada → Treinamento → Avaliação → Modelo Salvo
```

### 3. Predição
```plaintext
Novos Dados → Normalização → Predição → Resultado
```

## Boas Práticas de Uso

### 1. Preparação
- Garantir qualidade dos dados de entrada
- Validar dimensões corretas (7 para SS, 5 para NS)
- Verificar normalização dos dados

### 2. Treinamento
- Usar validação cruzada para avaliar robustez
- Monitorar métricas de performance
- Salvar modelos regularmente

### 3. Produção
- Validar predições contra resultados reais
- Monitorar logs de erro
- Retreinar periodicamente com novos dados

## Extensibilidade

### 1. Possíveis Melhorias
- Implementação de outros algoritmos
- Otimização de hiperparâmetros
- Análise de features adicionais

### 2. Personalização
- Ajuste de parâmetros via arquivo de configuração
- Modificação de métricas de avaliação
- Implementação de novos tipos de análise

## Solução de Problemas

### 1. Erros Comuns
- Dimensões incorretas de features
- Dados não normalizados
- Problemas de serialização

### 2. Diagnóstico
- Verificar logs de erro
- Validar estrutura dos dados
- Conferir configurações do modelo

## Manutenção

### 1. Backup
- Modelos treinados (.zip)
- Arquivos de configuração
- Logs de treinamento

### 2. Monitoramento
- Performance do modelo
- Qualidade das predições
- Uso de recursos

Esta implementação fornece uma base sólida para análise de padrões em loterias, com possibilidade de expansão e personalização conforme necessário.

