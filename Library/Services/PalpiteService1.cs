using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services;

public class PalpiteService1
{
    private readonly Random _random;
    private readonly IMLLogger _logger;
    private readonly MLNetModel _modelSS;
    private readonly MLNetModel _modelNS;
    private readonly Dictionary<int, Dictionary<int, double>> _percentuaisSS;
    private readonly Dictionary<int, Dictionary<int, double>> _percentuaisNS;
    private readonly MLModelRepository _repository;
    private readonly Lance _concursoBase;
    private readonly Lance _concursoAlvo;
    private readonly Lances _gruposSS;
    private readonly Lances _gruposNS;
    private readonly int _concursoBaseId;

    public PalpiteService1(IMLLogger logger, MLNetModel modeloSS, MLNetModel modeloNS, int concursoBaseId)
    {
        _random = new Random();
        _logger = logger;
        _modelSS = modeloSS;
        _modelNS = modeloNS;
        _repository = new MLModelRepository(new FileService(), logger);
        _percentuaisSS = _repository.CarregarPercentuaisSS();
        _percentuaisNS = _repository.CarregarPercentuaisNS();

        _concursoBaseId = concursoBaseId - 1;
        _concursoBase = Infra.arLoto.First(c => c.Id == _concursoBaseId);
        _concursoAlvo = Infra.arLoto.First(c => c.Id == concursoBaseId - 1);

        _gruposSS = GerarCombinacoes.Combinar15a9(_concursoBase.Lista);
        _gruposNS = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(_concursoBase).Lista);

        _logger.LogInformation($"Inicializado com concurso base: {_concursoBase.Id}");
        _logger.LogInformation($"Grupos gerados - SS: {_gruposSS.Count}, NS: {_gruposNS.Count}");
    }


    #region GERAR PALPITES ALEATÓRIOS

    public Lances GerarPalpitesAleatorios()
    {
        Lances palpites = new();

        foreach (Lance o in _gruposSS)
        {


            List<int> ls1 = o.Lista;
            foreach (Lance p in _gruposNS)
            {
                List<int> ls2 = p.Lista;


                List<int> ls3 = new List<int>(ls1.Concat(ls2));
                ls3.Sort();

                Lance u = new Lance { Id = palpites.Count, Lista = ls3, M = o.Id, N = p.Id };
                palpites.Add(u);


                _logger.LogInformation($"Palpite gerado: SS(ID:{u.M}) + NS(ID:{u.N})");
            }
        }

        return palpites;
    }
    public Lances GerarPalpitesAleatorios(int quantidade)
    {
        Lances palpites = new();

        for (int i = 0; i < quantidade; i++)
        {
            int ss = _random.Next(_gruposSS.Count);
            int ns = _random.Next(_gruposNS.Count);

            List<int> ls1 = _gruposSS[ss].Lista;
            List<int> ls2 = _gruposNS[ns].Lista;
            List<int> ls3 = new List<int>(ls1.Concat(ls2));
            ls3.Sort();

            Lance u = new Lance { Id = palpites.Count, Lista = ls3, M = ss, N = ns };
            palpites.Add(u);
            _logger.LogInformation($"Palpite gerado: SS(ID:{u.M}) + NS(ID:{u.N})");
        }

        return palpites;
    }
    public Lances GerarPalpitesAleatorios(int quantidade, int PT)
    {
        Lances palpites = new();

        while (palpites.Count < quantidade)
        {
            int ss = _random.Next(_gruposSS.Count);
            int ns = _random.Next(_gruposNS.Count);

            List<int> ls1 = _gruposSS[ss].Lista;
            List<int> ls2 = _gruposNS[ns].Lista;
            List<int> ls3 = new List<int>(ls1.Concat(ls2));
            ls3.Sort();

            Lance u = new Lance { Id = palpites.Count, Lista = ls3, M = ss, N = ns };
            if (Infra.Contapontos(u, _concursoAlvo) == PT) palpites.Add(u);

            _logger.LogInformation($"Palpite gerado: SS(ID:{u.M}) + NS(ID:{u.N})");
        }

        return palpites;
    }

    #endregion


    private float CalcularBonusDistanciaCinco(int pontos)
    {
        return pontos switch
        {
            5 => 1.0f,      // Máximo bônus
            4 or 6 => 0.8f, // Bônus alto
            3 or 7 => 0.5f, // Bônus médio
            2 or 8 => 0.2f, // Bônus baixo
            _ => 0f         // Sem bônus
        };
    }

    public Lances ClassificarPalpites(Lances palpites)
    {
        var classificados = new Lances();

        // Encontrar referências para as duas medidas
        var (grupoRefFreq, gruposNSFreq) = EncontrarGruposFrequentes(palpites);
        var (grupoRefAnt, gruposNSAnt) = EncontrarGruposAnteriores();

        foreach (var palpite in palpites)
        {
            // Pontuação base ML
            float pontuacaoBase = CalcularPontuacaoGrupo(palpite.M, palpite.N);

            // Medida 1: Baseada em frequência no conjunto de palpites
            float pontuacaoFreq = CalcularPontuacaoComFrequencia(palpite, grupoRefFreq, gruposNSFreq, pontuacaoBase);

            // Medida 2: Baseada em concurso anterior com 9 pontos
            float pontuacaoAnt = CalcularPontuacaoComAcertosAnteriores(palpite, grupoRefAnt, gruposNSAnt.Lista, pontuacaoBase);

            palpite.F0 = (float)Math.Round(pontuacaoBase, 4);
            palpite.F1 = (float)Math.Round(pontuacaoFreq, 4);
            palpite.F2 = (float)Math.Round(pontuacaoAnt, 4);

            classificados.Add(palpite);

            _logger.LogInformation($"Palpite: SS={palpite.M} NS={palpite.N} " +
                                 $"Base={pontuacaoBase:F2} " +
                                 $"Freq={pontuacaoFreq:F2} " +
                                 $"Ant={pontuacaoAnt:F2}");
        }

        // classificados.Sort();

        //var ordenados = classificados
        //.Where(p => !float.IsNaN(p.F1) && !float.IsInfinity(p.F1))
        //.OrderByDescending(p => p.F1)
        //.ToList();


        return classificados;
    }



    #region CÁLCULO DE PONTUAÇÃO BASE
    private float CalcularPontuacaoGrupo(int idSS, int idNS)
    {
        if (_percentuaisSS.TryGetValue(idSS, out var valoresSS) &&
            _percentuaisNS.TryGetValue(idNS, out var valoresNS))
        {
            var featuresSS = Enumerable.Range(3, 7)
                .Select(i => valoresSS.ContainsKey(i) ? (float)valoresSS[i] : 0f)
                .ToArray();

            var featuresNS = Enumerable.Range(2, 5)
                .Select(i => valoresNS.ContainsKey(i) ? (float)valoresNS[i] : 0f)
                .ToArray();

            var pontuacaoSS = _modelSS.Predict(featuresSS);
            var pontuacaoNS = _modelNS.Predict(featuresNS);

            return (pontuacaoSS + pontuacaoNS) / 2.0f;
        }
        return 0;
    }
    #endregion

    #region CÁLCULO BASEADO EM FREQUÊNCIA
    private (Lance grupoRef, List<int> gruposNS) EncontrarGruposFrequentes(Lances palpites)
    {
        var frequenciaGruposSS = new Dictionary<int, int>();
        foreach (var palpite in palpites)
        {
            if (!frequenciaGruposSS.ContainsKey(palpite.M))
                frequenciaGruposSS[palpite.M] = 0;
            frequenciaGruposSS[palpite.M]++;
        }

        var idSSMaisFrequente = frequenciaGruposSS.OrderByDescending(x => x.Value).First().Key;
        var grupoRef = _gruposSS[idSSMaisFrequente];

        var gruposNSAssociados = palpites
            .Where(p => p.M == idSSMaisFrequente)
            .Select(p => p.N)
            .Distinct()
            .ToList();

        return (grupoRef, gruposNSAssociados);
    }

    private float CalcularPontuacaoComFrequencia(Lance palpite, Lance grupoRef, List<int> gruposNSRef, float pontuacaoBase)
    {
        // Calcular pontos que o grupo SS faz no grupo referência
        var grupoSS = _gruposSS[palpite.M];
        int pontosNoRef = Infra.Contapontos(grupoRef, grupoSS);

        float bonusSS = CalcularBonusDistanciaCinco(pontosNoRef);
        float bonusNS = gruposNSRef.Contains(palpite.N) ? 0.3f : 0f;

        return pontuacaoBase * (1 + bonusSS + bonusNS);
    }
    #endregion

    #region CÁLCULO BASEADO EM CONCURSO ANTERIOR
    private (Lance grupoRef, Lance gruposNS) EncontrarGruposAnteriores()
    {
        // Encontrar primeiro concurso anterior que faz 9 pontos com o base
        Lance concursoAnterior = null;
        for (int i = _concursoBaseId - 1; i > 0; i--)
        {
            var concurso = Infra.arLoto.First(c => c.Id == i);
            if (Infra.Contapontos(_concursoBase, concurso) == 9)
            {
                concursoAnterior = concurso;
                break;
            }
        }

        // Encontrar grupos SS que fazem 9 pontos nesse concurso
        Lance grupoRef = null;
        Lance grupoNS = null;

        foreach (Lance o in _gruposSS) { if (Infra.Contapontos(o, concursoAnterior) == 9) { grupoRef = o; break; } }
        foreach (Lance o in _gruposNS) { if (Infra.Contapontos(o, concursoAnterior) == 6) { grupoNS = o; break; } }

        return (grupoRef, grupoNS);
    }

    private float CalcularPontuacaoComAcertosAnteriores(Lance palpite, Lance grupoRef, List<int> gruposNSRef, float pontuacaoBase)
    {
        if (grupoRef == null)
            return pontuacaoBase;

        // Calcular pontos que o grupo SS faz no grupo referência
        var grupoSS = _gruposSS[palpite.M];
        int pontosNoRef = Infra.Contapontos(grupoRef, grupoSS);

        float bonusSS = CalcularBonusDistanciaCinco(pontosNoRef);
        float bonusNS = gruposNSRef.Contains(palpite.N) ? 0.3f : 0f;

        return pontuacaoBase * (1 + bonusSS + bonusNS);
    }


    public Lances GetGrupoSS() { return (Lances)_gruposSS; }
    public Lances GetGrupoNS() { return (Lances)_gruposNS; }


    #endregion
}
