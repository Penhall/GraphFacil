using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Interfaces
{
    public interface IRepository<T> 
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(object parentId);
        T GetByKey(object keyValue);

        void Add(T entity, bool autoPersist = true);
        void Update(T entity, bool autoPersist = true);
        void Del(T entity, bool autoPersist = true);
        int Count();

        void Save();

    }
}
