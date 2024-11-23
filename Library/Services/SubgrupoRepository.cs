using System;
using System.Collections.Generic;


namespace LotoLibrary.Services;
public static class SubgrupoRepository
{
    private static Dictionary<int, Subgrupo> _subgrupos = new Dictionary<int, Subgrupo>();

    // Adiciona um subgrupo ao repositório
    public static void AdicionarSubgrupo(Subgrupo subgrupo)
    {
        if (!_subgrupos.ContainsKey(subgrupo.Id))
        {
            _subgrupos[subgrupo.Id] = subgrupo;
        }
        else
        {
            Console.WriteLine($"Subgrupo com ID {subgrupo.Id} já existe no repositório.");
        }
    }

    // Obtém um subgrupo pelo ID
    public static bool TryGetSubgrupoById(int id, out Subgrupo subgrupo)
    {
        return _subgrupos.TryGetValue(id, out subgrupo);
    }

    // Atualiza um subgrupo existente usando uma ação de atualização
    public static void AtualizarSubgrupo(int id, Action<Subgrupo> atualizar)
    {
        if (_subgrupos.TryGetValue(id, out Subgrupo subgrupo))
        {
            atualizar(subgrupo);
        }
        else
        {
            Console.WriteLine($"Subgrupo com ID {id} não encontrado no repositório para atualização.");
        }
    }

    // Atualiza um subgrupo existente
    public static void AtualizarSubgrupo(Subgrupo subgrupo)
    {
        if (_subgrupos.ContainsKey(subgrupo.Id))
        {
            _subgrupos[subgrupo.Id] = subgrupo;
        }
        else
        {
            Console.WriteLine($"Subgrupo com ID {subgrupo.Id} não encontrado no repositório para atualização.");
        }
    }

    // Lista todos os subgrupos
    public static List<Subgrupo> ListarSubgrupos()
    {
        return new List<Subgrupo>(_subgrupos.Values);
    }

    // Limpa o repositório
    public static void LimparRepositorio()
    {
        _subgrupos.Clear();
    }
}

// Exemplo de uso:
// Subgrupo subgrupo = new Subgrupo(new List<int> { 1, 2, 3, 4, 5 });
// subgrupo.Id = 123; // Certifique-se de definir o ID corretamente
// SubgrupoRepository.AdicionarSubgrupo(subgrupo);
// if (SubgrupoRepository.TryGetSubgrupoById(123, out Subgrupo encontrado))
// {
//     Console.WriteLine($"Subgrupo encontrado: {encontrado.Id}");
//     SubgrupoRepository.AtualizarSubgrupo(123, s => s.Frequencia++); // Exemplo de atualização
// }
