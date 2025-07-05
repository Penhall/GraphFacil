using CommunityToolkit.Mvvm.ComponentModel;


namespace LotoLibrary.Models;

public partial class DezenaOscilante : ObservableObject
{
    [ObservableProperty] private int _numero;
    [ObservableProperty] private double _fase;
    [ObservableProperty] private double _frequencia;
    [ObservableProperty] private bool _estaSincronizada;
    [ObservableProperty] private int _ultimoAtraso;
    [ObservableProperty] private double _forcaSincronizacao;
    [ObservableProperty] private double _probabilidade;

    public void AtualizarComLance(Lance lance)
    {
        // Customize com base nos dados do Lance
        ForcaSincronizacao = lance.F1 * 0.5; // Exemplo de ajuste
        Probabilidade = lance.F2;
    }
}