# 📋 Quadro Kanban - Projeto Lotofácil

Este quadro Kanban acompanha o progresso das tarefas de integração do `Dashboard` com a `LotoLibrary`.

---

##  backlog A Fazer

### 🚀 Fase 1: Integração da Funcionalidade Essencial

- [ ] **Conectar Carregamento de Modelos**: Refatorar `LoadAvailableModelsAsync` em `PredictionModelsViewModel` para usar a `ModelFactory` real.
- [ ] **Implementar Instanciação de Modelo**: No `PredictionModelsViewModel`, criar a instância do modelo `IPredictionModel` quando o usuário o selecionar na UI.
- [ ] **Implementar Execução de Predição Real**: Refatorar o comando `QuickPredict` para chamar o `PredictAsync` do modelo selecionado e exibir resultados reais.

### 🚀 Fase 2: Habilitação dos Módulos Auxiliares

- [ ] **Conectar Módulo de Validação**: No `ValidationViewModel`, conectar os comandos de validação para invocar os serviços reais (`Phase1ValidationService`, `DiagnosticService`).
- [ ] **Implementar Módulo de Comparação**: No `ComparisonViewModel`, implementar a lógica do comando `CompareModelsCommand` para usar o `PerformanceComparer`.
- [ ] **Desenvolver Configuração Dinâmica**: Criar a View e a lógica no `ConfigurationViewModel` para ler e alterar parâmetros de modelos que implementam `IConfigurableModel`.

### 🚀 Fase 3: Polimento e Testes Finais

- [ ] **Revisão de UX**: Garantir que indicadores de carregamento, mensagens de status e notificações funcionem corretamente com as operações reais.
- [ ] **Testes de Aceitação (UAT)**: Executar manualmente todos os cenários do `user_guide.md` para validar a experiência do usuário final.
- [ ] **Testes de Diagnóstico e Integridade**: Executar sistematicamente os testes do `DIAGNÓSTICO PARA DEFINIR AJUSTES FINOS NECESSÁRIOS.md`.
- [ ] **Ciclo de Correção de Bugs (Hardening)**: Identificar, priorizar e corrigir bugs encontrados durante os testes.

---

## 🏃 Em Andamento

*(Nenhuma tarefa em andamento)*

---

## ✅ Concluído

*(Nenhuma tarefa concluída)*

---