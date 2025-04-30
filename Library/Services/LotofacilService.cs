using LotoLibrary.Models;
using LotoLibrary.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LotoLibrary.Services
{
    public class LotofacilService
    {
        private readonly LotofacilRepository _repository;

        public LotofacilService(string baseDirectory = null)
        {
            _repository = new LotofacilRepository(baseDirectory);
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