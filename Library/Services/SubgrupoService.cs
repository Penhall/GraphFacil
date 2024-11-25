using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using System;
using System.Collections.Generic;

namespace LotoLibrary.Services;

public class SubgrupoService
{
    private readonly ISubgrupoRepository _subgrupoRepository;

    public SubgrupoService(ISubgrupoRepository subgrupoRepository)
    {
        _subgrupoRepository = subgrupoRepository;
    }

    // Processa os sorteios para atualizar frequências dos subgrupos e calcula valores percentuais
    public void ProcessarSorteiosECalcularPercentuais(Lances sorteios, int totalSorteios)
    {
        // Carregar subgrupos do repositório
        var subgruposSS = _subgrupoRepository.CarregarSubgruposSS() ?? new List<Subgrupo>();
        var subgruposNS = _subgrupoRepository.CarregarSubgruposNS() ?? new List<Subgrupo>();

        if (subgruposSS.Count == 0 || subgruposNS.Count == 0)
        {
            Console.WriteLine("Aviso: Não foram encontrados subgrupos no repositório. Inicializando listas vazias.");
        }

        // Atualizar frequência dos subgrupos SS e NS
        if (sorteios?.Lista != null)
        {
            foreach (var sorteio in sorteios.Lista)
            {
                AtualizarFrequenciaSubgrupos(subgruposSS, sorteio);
                AtualizarFrequenciaSubgrupos(subgruposNS, sorteio);
            }
        }
        else
        {
            Console.WriteLine("Aviso: Não há sorteios disponíveis para processar.");
        }

        // Calcular percentuais dos subgrupos SS e NS, apenas se houver sorteios para calcular
        if (totalSorteios > 0)
        {
            foreach (var subgrupo in subgruposSS)
            {
                subgrupo.CalcularPercentual(totalSorteios);
            }
            foreach (var subgrupo in subgruposNS)
            {
                subgrupo.CalcularPercentual(totalSorteios);
            }
        }
        else
        {
            Console.WriteLine("Aviso: Total de sorteios é zero. Percentuais não serão calculados.");
        }

        // Salvar os subgrupos atualizados no repositório
        _subgrupoRepository.SalvarSubgruposSS(subgruposSS);
        _subgrupoRepository.SalvarSubgruposNS(subgruposNS);
    }

    // Método para atualizar a frequência dos subgrupos
    private void AtualizarFrequenciaSubgrupos(List<Subgrupo> subgrupos, Lance sorteio)
    {
        foreach (var subgrupo in subgrupos)
        {
            if (subgrupo.VerificarAcerto(sorteio))
            {
                subgrupo.IncrementarFrequencia();
            }
        }
    }
}