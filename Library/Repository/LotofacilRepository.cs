// D:\PROJETOS\GraphFacil\Library\Repository\LotofacilRepository.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System;
using LotoLibrary.Infrastructure;
using LotoLibrary.Interfaces;
using LotoLibrary.Models;
using LotoLibrary.Services;

namespace LotoLibrary.Repository
{
    public class LotofacilRepository : IRepository<Lotofacil>
    {
        private readonly FileService _fileService;
        private readonly string _fileName = "Lotofacil.json";
        private List<Lotofacil> _concursos;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public LotofacilRepository(string baseDirectory = null)
        {
            _fileService = new FileService(baseDirectory);
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                _lock.EnterWriteLock();
                _concursos = _fileService.CarregarDados<List<Lotofacil>>(_fileName) ?? new List<Lotofacil>();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public IEnumerable<Lotofacil> GetAll()
        {
            try
            {
                _lock.EnterReadLock();
                return _concursos.ToList();
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public IEnumerable<Lotofacil> GetAll(object parentId)
        {
            return GetAll();
        }

        public Lotofacil GetByKey(object keyValue)
        {
            if (keyValue is int concursoNumero)
            {
                try
                {
                    _lock.EnterReadLock();
                    return _concursos.FirstOrDefault(c => c.numero == concursoNumero);
                }
                finally
                {
                    _lock.ExitReadLock();
                }
            }
            return null;
        }

        public void Add(Lotofacil entity, bool autoPersist = true)
        {
            try
            {
                _lock.EnterWriteLock();
                
                // Verificar se já existe um concurso com o mesmo número
                var existingConcurso = _concursos.FirstOrDefault(c => c.numero == entity.numero);
                if (existingConcurso == null)
                {
                    _concursos.Add(entity);
                    
                    if (autoPersist)
                    {
                        Save();
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Update(Lotofacil entity, bool autoPersist = true)
        {
            try
            {
                _lock.EnterWriteLock();
                
                var index = _concursos.FindIndex(c => c.numero == entity.numero);
                if (index >= 0)
                {
                    _concursos[index] = entity;
                    
                    if (autoPersist)
                    {
                        Save();
                    }
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public void Del(Lotofacil entity, bool autoPersist = true)
        {
            try
            {
                _lock.EnterWriteLock();
                
                _concursos.RemoveAll(c => c.numero == entity.numero);
                
                if (autoPersist)
                    {
                        Save();
                    }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        public int Count()
        {
            try
            {
                _lock.EnterReadLock();
                return _concursos.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void Save()
        {
            try
            {
                _lock.EnterReadLock();
                _fileService.SalvarDados(_fileName, _concursos);
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }

        public void UpdateFromAPI()
        {
            try
            {
                _lock.EnterWriteLock();
                
                if (_concursos.Count == 0)
                {
                    // Se não houver concursos, extrair todos
                    var scraper = new LotofacilScraper();
                    _concursos = scraper.ExtractAllConcursos();
                }
                else
                {
                    // Se já houver concursos, apenas atualizar
                    var updater = new LotofacilUpdater();
                    _concursos = updater.UpdateConcursos(_concursos);
                }
                
                // Ordenar por número do concurso
                _concursos = _concursos.OrderBy(c => c.numero).ToList();
                
                // Salvar os dados atualizados
                Save();
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }
    }
}
