using System.Collections.Generic;
using System.Linq;
using TF = Tensorflow.NumPy;

namespace LotoLibrary.Services;

public class AnaliseService
{
    private List<Subgrupo> _subgruposSS;
    private List<Subgrupo> _subgruposNS;

    public AnaliseService()
    {
        _subgruposSS = new List<Subgrupo>();
        _subgruposNS = new List<Subgrupo>();
    }

    // Método para adicionar um subgrupo SS
    public void AdicionarSubgrupoSS(List<int> numeros)
    {
        _subgruposSS.Add(new Subgrupo(numeros));
    }

    // Método para adicionar um subgrupo NS
    public void AdicionarSubgrupoNS(List<int> numeros)
    {
        _subgruposNS.Add(new Subgrupo(numeros));
    }

    // Método para processar os sorteios e calcular a frequência dos subgrupos
    public void ProcessarSorteios(List<List<int>> sorteios)
    {
        foreach (var sorteio in sorteios)
        {
            // Verificar frequência dos subgrupos SS
            foreach (var subgrupo in _subgruposSS)
            {
                int acertos = subgrupo.Numeros.Count(n => sorteio.Contains(n));
                if (acertos >= 3)
                {
                    subgrupo.IncrementarFrequencia();
                    subgrupo.AtualizarContagemAcertos(acertos);
                }
            }

            // Verificar frequência dos subgrupos NS
            foreach (var subgrupo in _subgruposNS)
            {
                int acertos = subgrupo.Numeros.Count(n => sorteio.Contains(n));
                if (acertos >= 2)
                {
                    subgrupo.IncrementarFrequencia();
                    subgrupo.AtualizarContagemAcertos(acertos);
                }
            }
        }
    }

    // Método para calcular o valor associado de cada subgrupo (exemplo simples de cálculo)
    public void CalcularValorAssociado()
    {
        foreach (var subgrupo in _subgruposSS)
        {
            // Exemplo: valor associado pode ser a frequência multiplicada por um fator arbitrário
            subgrupo.ValorAssociado = subgrupo.Frequencia * 1.5;
        }

        foreach (var subgrupo in _subgruposNS)
        {
            // Exemplo: valor associado pode ser a frequência multiplicada por um fator diferente
            subgrupo.ValorAssociado = subgrupo.Frequencia * 1.2;
        }
    }

    // Método para calcular o percentual de acertos de cada subgrupo
    public void CalcularPercentuais(int totalSorteios)
    {
        foreach (var subgrupo in _subgruposSS)
        {
            subgrupo.CalcularPercentual(totalSorteios);
        }

        foreach (var subgrupo in _subgruposNS)
        {
            subgrupo.CalcularPercentual(totalSorteios);
        }
    }

    // Método para obter os melhores agrupamentos de SS e NS
    public List<(Subgrupo SS, Subgrupo NS)> ObterMelhoresAgrupamentos(int quantidade)
    {
        var combinacoes = from ss in _subgruposSS
                          from ns in _subgruposNS
                          select (SS: ss, NS: ns, ValorTotal: ss.ValorAssociado + ns.ValorAssociado);

        return combinacoes.OrderByDescending(c => c.ValorTotal)
                          .Take(quantidade)
                          .Select(c => (c.SS, c.NS))
                          .ToList();
    }

    // Método para preparar os dados para a rede neural
    public (TF.NDArray inputs, TF.NDArray labels) PrepararDadosParaTreinamento()
    {
        var inputs = new List<TF.NDArray>();
        var labels = new List<TF.NDArray>();

        foreach (var subgrupo in _subgruposSS)
        {
            inputs.Add(subgrupo.ObterVetorCaracteristicas());
            // Exemplo: aqui definimos o label, você pode ajustar isso conforme necessário
            labels.Add(TF.np.array(new double[] { subgrupo.ValorAssociado }));
        }

        foreach (var subgrupo in _subgruposNS)
        {
            inputs.Add(subgrupo.ObterVetorCaracteristicas());
            // Exemplo: aqui definimos o label, você pode ajustar isso conforme necessário
            labels.Add(TF.np.array(new double[] { subgrupo.ValorAssociado }));
        }

        return (TF.np.concatenate(inputs.ToArray(), axis: 0), TF.np.concatenate(labels.ToArray(), axis: 0));
    }
}