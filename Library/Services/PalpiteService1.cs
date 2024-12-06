using LotoLibrary.Infrastructure.Logging;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.NeuralNetwork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services
{
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

            _concursoBaseId = concursoBaseId;
            _concursoBase = Infra.arLoto.First(c => c.Id == _concursoBaseId);
            _gruposSS = GerarCombinacoes.Combinar15a9(_concursoBase.Lista);
            _gruposNS = GerarCombinacoes.Combinar10a6(Infra.DevolveOposto(_concursoBase).Lista);

            _logger.LogInformation($"Inicializado com concurso base: {_concursoBase.Id}");
            _logger.LogInformation($"Grupos gerados - SS: {_gruposSS.Count}, NS: {_gruposNS.Count}");
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

        public Lances ClassificarPalpites(Lances palpites)
        {
            Lances classificados = new();

            foreach (var palpite in palpites)
            {
                // Obter IDs dos grupos que compõem o palpite


                // Calcular pontuação com base nos percentuais históricos
                float pontuacao = CalcularPontuacaoGrupo(palpite.M, palpite.N);

                palpite.F = pontuacao;

                classificados.Add(palpite);

                _logger.LogInformation($"Palpite classificado - SS:{palpite.M} NS:{palpite.N} Pontuação:{pontuacao:F2}");
            }

            classificados.Sort();

            return classificados;
        }

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
    }
}