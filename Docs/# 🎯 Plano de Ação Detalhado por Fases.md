# 🎯 Plano de Ação Detalhado por Fases - Integração `Dashboard` e `LotoLibrary`

Este documento detalha o plano de implementação para corrigir as funcionalidades pendentes na aplicação `Dashboard`, conectando-a totalmente ao backend `LotoLibrary`. O plano está dividido em três fases sequenciais, cada uma com objetivos, tarefas específicas e critérios de aceitação claros.

---

## 🚀 Fase 1: Integração da Funcionalidade Essencial de Predição

**Objetivo:** Conectar a funcionalidade principal da aplicação — a geração de predições. Esta fase foca em fazer o `PredictionModelsViewModel` utilizar o backend real, substituindo toda a lógica simulada.

**Critério de Aceitação da Fase:** O usuário pode iniciar a aplicação, selecionar qualquer modelo de predição disponível na `LotoLibrary`, clicar em "Gerar Predição" e receber um resultado real, com nível de confiança, exibido na interface.

### ✅ Tarefas da Fase 1

1.  **Conectar Carregamento de Modelos:**
    *   [x] **Local:** `PredictionModelsViewModel.cs`
    *   **Ação:** Refatorar o método `LoadAvailableModelsAsync`.
    *   **Detalhes:**
        *   Substituir `Task.Delay` e a lógica simulada por uma chamada real ao `_modelFactory.GetAvailableModelTypes()`.
        *   Iterar sobre os tipos de modelo retornados e usar `_modelFactory.GetModelInfo()` para popular a coleção `ObservableCollection<ModelInfo>`.
    *   **Validação:** Verificar se a `ComboBox` na UI é populada com a lista real de modelos (`MetronomoModel`, `StatisticalDebtModel`, `MetaLearningModel`, etc.).

2.  **Implementar Instanciação de Modelo sob Demanda:**
    *   [x] **Local:** `PredictionModelsViewModel.cs`
    *   **Ação:** Implementar a lógica que responde à seleção de um modelo na UI.
    *   **Detalhes:**
        *   Quando a propriedade `SelectedModelInfo` for alterada, utilizar `_modelFactory.CreateModel(SelectedModelInfo.ModelType)` para criar uma instância real do `IPredictionModel` selecionado.
        *   Armazenar a instância do modelo criado em um campo privado (ex: `_currentModelInstance`) para uso posterior.

3.  **Implementar Execução de Predição Real:**
    *   [x] **Local:** `PredictionModelsViewModel.cs`
    *   **Ação:** Refatorar o comando `QuickPredict` (ou similar).
    *   **Detalhes:**
        *   Remover completamente a lógica de `Task.Delay` e geração de números aleatórios.
        *   Verificar se a `_currentModelInstance` não é nula e se não está inicializada (`!IsInitialized`). Se necessário, chamar `await _currentModelInstance.InitializeAsync(_historicalData)` e `await _currentModelInstance.TrainAsync(_historicalData)`.
        *   Invocar `var predictionResult = await _currentModelInstance.PredictAsync(concurso)`.
        *   Atualizar as propriedades da UI (`LastPredictionResult`, `SelectedModelConfidence`, etc.) com os dados reais do objeto `predictionResult`.

---

## 🚀 Fase 2: Habilitação dos Módulos Auxiliares

**Objetivo:** Ativar as funcionalidades avançadas de validação, comparação e configuração, conectando os respectivos ViewModels aos serviços correspondentes no backend.

**Critério de Aceitação da Fase:** As seções de "Validação", "Comparação" e "Configuração de Modelos" estão totalmente funcionais, exibindo dados reais e permitindo a interação do usuário com a `LotoLibrary`.

### ✅ Tarefas da Fase 2

1.  **Conectar o Módulo de Validação:**
    *   [x] **Local:** `ValidationViewModel.cs`
    *   **Ação:** Refatorar os comandos `RunQuickValidation` e `RunFullValidation`.
    *   **Detalhes:**
        *   No lugar de `Task.Delay` e resultados aleatórios, invocar os serviços reais da `LotoLibrary`, como `Phase1ValidationService` e `DiagnosticService`.
        *   Mapear os resultados retornados pelos serviços para a coleção `ObservableCollection<ValidationResult>`, exibindo o status real (`Passou`, `Falhou`) e as métricas de cada teste.
        *   Conectar o comando `ValidateAllModels` para iterar sobre os modelos carregados, executar o método `ValidateAsync()` de cada um e agregar os resultados na UI.

2.  **Implementar o Módulo de Comparação de Modelos:**
    *   [x] **Local:** `ComparisonViewModel.cs`
    *   **Ação:** Implementar a lógica do comando `CompareModelsCommand`.
    *   **Detalhes:**
        *   Utilizar o serviço `PerformanceComparer` da `LotoLibrary`.
        *   O comando deve executar predições para um conjunto de concursos de teste com cada um dos modelos carregados.
        *   Passar os resultados para o `PerformanceComparer` para obter métricas como Acurácia, Correlação e Tempo de Execução.
        *   Exibir os resultados da comparação em um formato claro na UI (ex: `DataGrid`).

3.  **Desenvolver a Configuração Dinâmica de Modelos:**
    *   [x] **Local:** `ConfigurationViewModel.cs` e uma nova `View/Dialog`.
    *   **Ação:** Criar a funcionalidade de configuração de parâmetros.
    *   **Detalhes:**
        *   Quando um modelo é selecionado, verificar se ele implementa a interface `IConfigurableModel`.
        *   Se implementar, ler a propriedade `DefaultParameters` para obter a lista de parâmetros configuráveis (nome, tipo, valor padrão).
        *   Renderizar dinamicamente na UI os controles apropriados para cada parâmetro (ex: `TextBox` para números, `CheckBox` para booleanos).
        *   Implementar um botão "Salvar" que chama o método `model.SetParameter(name, value)` na instância do modelo para aplicar as alterações.

---

## 🚀 Fase 3: Polimento, Testes Finais e Finalização

**Objetivo:** Garantir a robustez, estabilidade e qualidade da experiência do usuário (UX) da aplicação agora totalmente integrada.

**Critério de Aceitação da Fase:** A aplicação está estável, livre de bugs críticos, com uma experiência de usuário fluida e intuitiva. Todos os casos de uso descritos no `user_guide.md` podem ser executados com sucesso.

### ✅ Tarefas da Fase 3

1.  **Revisão e Refinamento da Experiência do Usuário (UX):**
    *   [x] **Foco:** Resposta da UI e feedback ao usuário.
    *   **Ação:** Revisar todas as operações agora conectadas ao backend.
    *   **Detalhes:**
        *   Garantir que os indicadores de carregamento (`IsProcessing`, barras de progresso) são exibidos corretamente durante as operações reais (que podem ser mais longas que as simulações).
        *   Assegurar que mensagens de status, sucesso e erro sejam claras, informativas e não-bloqueantes.

2.  **Execução de Testes de Aceitação do Usuário (UAT):**
    *   [x] **Foco:** Validação funcional completa.
    *   **Ação:** Seguir o `user_guide.md` como um script de teste.
    *   **Detalhes:**
        *   Executar manualmente todos os cenários descritos no guia do usuário, desde a geração de predições simples até a comparação de modelos.
        *   Validar que os resultados e comportamentos na UI correspondem ao esperado.

3.  **Execução de Testes de Diagnóstico e Integridade:**
    *   [x] **Foco:** Saúde e estabilidade do sistema.
    *   **Ação:** Utilizar o `DIAGNÓSTICO PARA DEFINIR AJUSTES FINOS NECESSÁRIOS.md` como guia.
    *   **Detalhes:**
        *   Executar todos os serviços de validação e diagnóstico na aplicação totalmente integrada para identificar possíveis regressões ou problemas de integração que não foram pegos nos testes unitários.

4.  **Ciclo Final de Correção de Bugs (Hardening):**
    *   [ ] **Foco:** Estabilização da release.
    *   **Ação:** Identificar, priorizar e corrigir os bugs encontrados.
    *   **Detalhes:**
        *   Focar em resolver bugs críticos ou de alta prioridade que afetam a funcionalidade principal ou a experiência do usuário.
        *   Realizar uma última rodada de testes de fumaça (`smoke testing`) após as correções para garantir que nada foi quebrado.
