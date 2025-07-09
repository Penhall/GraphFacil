
// INSTRUCOES_PARA_NOVO_VIEWMODEL.txt
/*
📋 COMO CRIAR UM NOVO VIEWMODEL ESPECIALIZADO:

1️⃣ COPIAR TEMPLATE:
   - Copie a classe NovoExemploViewModel
   - Renomeie para seu ViewModel (ex: ReportsViewModel)

2️⃣ HERDAR CORRETAMENTE:
   - Sempre herde de ModelOperationBase (acesso a dados)
   - Ou ViewModelBase (se não precisar de dados)

3️⃣ IMPLEMENTAR FUNCIONALIDADES:
   - Adicione [ObservableProperty] para propriedades
   - Use [RelayCommand] para comandos
   - Override InitializeSpecificAsync() se necessário

4️⃣ USAR PADRÕES ESTABELECIDOS:
   - SetStatus() para mensagens de status
   - ExecuteWithLoadingAsync() para operações longas
   - UINotificationService.Instance para notificações
   - ValidateHistoricalData() antes de usar dados

5️⃣ INTEGRAR NA FACTORY:
   - Adicione método CreateSeuViewModel() na ViewModelFactory
   - Use padrão GetOrCreateSingleton<>()

6️⃣ INTEGRAR NO MAINWINDOWVIEWMODEL:
   - Adicione propriedade pública
   - Inicialize no construtor via factory
   - Adicione na inicialização e cleanup

7️⃣ ATUALIZAR XAML:
   - Adicione controles necessários
   - Use bindings: {Binding SeuViewModel.Propriedade}
   - Adicione comandos: Command="{Binding SeuViewModel.ComandoCommand}"

8️⃣ TESTAR:
   - Compile (deve ser zero erros)
   - Execute aplicação
   - Teste todas as funcionalidades
   - Verifique se não quebrou nada existente

✅ RESULTADO:
   - Novo ViewModel funcionando
   - Zero impacto no código existente
   - Pronto para testes unitários independentes
   - Fácil manutenção e extensão

🚀 TEMPO ESTIMADO: 30-60 minutos
🛡️ RISCO: ZERO (não afeta código existente)
🧪 TESTABILIDADE: ALTA (ViewModel isolado)
*/