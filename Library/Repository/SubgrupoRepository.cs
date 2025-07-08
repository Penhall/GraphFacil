using LotoLibrary.Infrastructure;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using System.Collections.Generic;

namespace LotoLibrary.Repository;

public class SubgrupoRepository : ISubgrupoRepository
{
    private readonly FileService _fileService = new FileService();
    private readonly string _filePathContagemSS = "ContagemSS.json";
    private readonly string _filePathContagemNS = "ContagemNS.json";

    public List<Subgrupo> CarregarSubgruposSS()
    {
        return _fileService.CarregarDados<List<Subgrupo>>(_filePathContagemSS) ?? new List<Subgrupo>();
    }

    public List<Subgrupo> CarregarSubgruposNS()
    {
        return _fileService.CarregarDados<List<Subgrupo>>(_filePathContagemNS) ?? new List<Subgrupo>();
    }

    public void SalvarSubgruposSS(List<Subgrupo> subgrupos)
    {
        _fileService.SalvarDados(_filePathContagemSS, subgrupos);
    }

    public void SalvarSubgruposNS(List<Subgrupo> subgrupos)
    {
        _fileService.SalvarDados(_filePathContagemNS, subgrupos);
    }
}
