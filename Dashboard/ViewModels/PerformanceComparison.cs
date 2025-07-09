// D:\PROJETOS\GraphFacil\Dashboard\ViewModel\PerformanceComparison.cs
namespace Dashboard.ViewModel
{
    public partial class MainWindowViewModel
    {
        private class PerformanceComparison
        {
            public double OldSystemAvgTime { get; set; }
            public double OldSystemDezenas1a9 { get; set; }
            public string OldSystemDistribution { get; set; }

            public double NewSystemAvgTime { get; set; }
            public double NewSystemDezenas1a9 { get; set; }
            public string NewSystemDistribution { get; set; }

            public double PerformanceImprovement { get; set; }
            public string QualityImprovement { get; set; }
        }


    }
}