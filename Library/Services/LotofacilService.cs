using LotoLibrary.Constantes;
using LotoLibrary.Models;
using LotoLibrary.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LotoLibrary.Services
{
    public class LotofacilService
    {
        private readonly LotofacilRepository _repository;

        public LotofacilService(string baseDirectory = null)
        {
            // Determina o caminho base usando a mesma lógica do Infra.cs se nenhum for fornecido
            string effectiveBaseDirectory = string.IsNullOrEmpty(baseDirectory) ?
                                            (Directory.Exists(Constante.PT) ? Constante.PT : Constante.PT1) :
                                            baseDirectory;

            _repository = new LotofacilRepository(effectiveBaseDirectory);
        }

        public List<Lotofacil> GetAllConcursos()
        {
            return _repository.GetAll().ToList();
        }

        public Lotofacil GetConcurso(int numero)
        {
            return _repository.GetByKey(numero);
        }

        public Lotofacil GetLatestConcurso()
        {
            return _repository.GetAll().OrderByDescending(c => c.numero).FirstOrDefault();
        }

        public void UpdateFromAPI()
        {
            _repository.UpdateFromAPI();
        }

        public List<Lotofacil> GetConcursosByDateRange(DateTime startDate, DateTime endDate)
        {
            return _repository.GetAll()
                .Where(c => DateTime.TryParse(c.dataApuracao, out DateTime date) &&
                           date >= startDate && date <= endDate)
                .ToList();
        }

        public List<Lotofacil> GetConcursosByNumberRange(int startNumber, int endNumber)
        {
            return _repository.GetAll()
                .Where(c => c.numero >= startNumber && c.numero <= endNumber)
                .ToList();
        }

        public int GetTotalConcursos()
        {
            return _repository.Count();
        }
    }
}