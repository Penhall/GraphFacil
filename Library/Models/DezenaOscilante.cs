// D:\PROJETOS\GraphFacil\Library\Models\DezenaOscilante.cs - Classe para oscilação de dezenas
using System.Collections.Generic;
using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotoLibrary.Models;

/// <summary>
/// Representa uma dezena com propriedades de oscilação
/// </summary>
public partial class DezenaOscilante : ObservableObject
{
    [ObservableProperty]
    private int _numero;

    [ObservableProperty]
    private double _fase; // 0° a 360°

    [ObservableProperty]
    private double _frequencia;

    [ObservableProperty]
    private bool _foiSorteada;

    [ObservableProperty]
    private double _forcaSincronizacao;

    [ObservableProperty]
    private int _ultimoAtraso;

    [ObservableProperty]
    private double _probabilidade;

    [ObservableProperty]
    private double _amplitude;

    [ObservableProperty]
    private bool _estaSincronizada;

    public DezenaOscilante()
    {
        Numero = 0;
        Fase = 0.0;
        Frequencia = 1.0;
        FoiSorteada = false;
        ForcaSincronizacao = 0.5;
        UltimoAtraso = 0;
        Probabilidade = 0.0;
        Amplitude = 1.0;
        EstaSincronizada = false;
    }

    public DezenaOscilante(int numero)
    {
        Numero = numero;
        Fase = new Random().Next(0, 360);
        Frequencia = 1.0;
        FoiSorteada = false;
        ForcaSincronizacao = 0.5;
        UltimoAtraso = 0;
        Probabilidade = 0.0;
        Amplitude = 1.0;
        EstaSincronizada = false;
    }

    public void AplicarInfluencia(double influencia)
    {
        // A influência ajusta a fase do oscilador
        Fase += influencia;
        Fase = (Fase % 360 + 360) % 360; // Normaliza a fase para 0-360
    }

    #region Métodos de Oscilação
    /// <summary>
    /// Atualiza a fase da dezena baseado na frequência e influências externas
    /// </summary>
    public void AtualizarFase()
    {
        // Incrementar fase baseado na frequência
        Fase += Frequencia;

        // Normalizar fase entre 0 e 360
        Fase = Fase % 360;
        if (Fase < 0) Fase += 360;

        // Calcular nova probabilidade baseada na fase
        Probabilidade = (Math.Sin(Fase * Math.PI / 180) + 1) / 2;
    }

    /// <summary>
    /// Atualiza a fase com influência de outras dezenas (acoplamento)
    /// </summary>
    public void AtualizarFase(List<DezenaOscilante> outrasDezenas, double fatorAcoplamento = 0.1)
    {
        if (outrasDezenas == null)
        {
            AtualizarFase();
            return;
        }

        double influenciaExterna = 0;
        foreach (var outra in outrasDezenas)
        {
            if (outra.Numero != this.Numero)
            {
                influenciaExterna += Math.Sin((outra.Fase - this.Fase) * Math.PI / 180);
            }
        }

        // Atualizar fase com influência externa
        Fase += Frequencia + (fatorAcoplamento * influenciaExterna);

        // Normalizar fase
        Fase = Fase % 360;
        if (Fase < 0) Fase += 360;

        // Atualizar probabilidade
        Probabilidade = (Math.Sin(Fase * Math.PI / 180) + 1) / 2;
    }
    #endregion

    #region Métodos Helper
    /// <summary>
    /// Reinicia a dezena com valores aleatórios
    /// </summary>
    public void Reiniciar()
    {
        var random = new Random();
        Fase = random.Next(0, 360);
        Frequencia = 1.0;
        FoiSorteada = false;
        UltimoAtraso = 0;
        Probabilidade = 0.0;
    }
    #endregion
}
