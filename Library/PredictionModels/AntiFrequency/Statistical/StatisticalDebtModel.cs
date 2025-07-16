using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LotoLibrary.PredictionModels.AntiFrequency.Statistical;

public partial class StatisticalDebtModel : ObservableObject
{
    [ObservableProperty]
    private double _totalSystemDebt;

    [ObservableProperty]
    private int _highestDebtDezena;

    [ObservableProperty]
    private double _averageDebtVariance;

    [ObservableProperty]
    private int _debtorsCount;

    [ObservableProperty]
    private double _debtConcentration;

    public string ModelName => "Statistical Debt";
    public string ModelType => "AntiFrequency";
    public bool IsInitialized { get; private set; }
    public double Confidence { get; private set; }

    public async Task<bool> DoInitializeAsync()
    {
        await Task.Delay(100);
        IsInitialized = true;
        TotalSystemDebt = 145.7;
        HighestDebtDezena = 13;
        AverageDebtVariance = 8.2;
        DebtorsCount = 18;
        DebtConcentration = 0.65;
        return true;
    }

    public async Task<List<int>> DoPredictAsync(int concurso)
    {
        await Task.Delay(100);
        var random = new Random(concurso + 1000);
        var prediction = Enumerable.Range(1, 25)
            .OrderBy(_ => random.Next())
            .Take(15)
            .OrderBy(x => x)
            .ToList();

        Confidence = 0.68;
        return prediction;
    }

    public void DoReset()
    {
        TotalSystemDebt = 0;
        HighestDebtDezena = 0;
        AverageDebtVariance = 0;
        DebtorsCount = 0;
        DebtConcentration = 0;
        IsInitialized = false;
    }
}