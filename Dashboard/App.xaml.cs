using System.Windows;

namespace Dashboard
{
    /// <summary>
    /// Lógica de interação para App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configurações globais da aplicação podem ser adicionadas aqui
            // Por exemplo: configuração de logging, inicialização de serviços, etc.
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Limpeza de recursos quando a aplicação for fechada
            base.OnExit(e);
        }
    }
}