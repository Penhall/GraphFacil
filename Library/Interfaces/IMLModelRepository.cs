// D:\PROJETOS\GraphFacil\Library\Interfaces\IMLModelRepository.cs
using System.Collections.Generic;

﻿using Microsoft.ML;
namespace LotoLibrary.Interfaces;

public interface IMLModelRepository
{
    Dictionary<int, Dictionary<int, double>> CarregarPercentuaisSS();
    Dictionary<int, Dictionary<int, double>> CarregarPercentuaisNS();
    void SalvarPercentuaisSS(Dictionary<int, Dictionary<int, double>> percentuais);
    void SalvarPercentuaisNS(Dictionary<int, Dictionary<int, double>> percentuais);
    void SalvarModelo(string tipo, ITransformer model, DataViewSchema schema);
    ITransformer CarregarModelo(string tipo, out DataViewSchema schema);
}
