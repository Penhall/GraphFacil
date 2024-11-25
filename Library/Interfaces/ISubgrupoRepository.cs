using LotoLibrary.Models;
using System.Collections.Generic;

namespace LotoLibrary.Interfaces;

public interface ISubgrupoRepository
{
    List<Subgrupo> CarregarSubgruposSS();
    List<Subgrupo> CarregarSubgruposNS();
    void SalvarSubgruposSS(List<Subgrupo> subgrupos);
    void SalvarSubgruposNS(List<Subgrupo> subgrupos);
}
