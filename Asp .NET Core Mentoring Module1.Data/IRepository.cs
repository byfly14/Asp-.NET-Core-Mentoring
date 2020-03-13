using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Asp_.NET_Core_Mentoring_Module1.Data
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAllWithInclude(IEnumerable<Expression<Func<T, object>>> includeProperties);
        T GetById(int id);
        T GetByIdWithInclude(Func<T, bool> predicate, IEnumerable<Expression<Func<T, object>>> includeProperties);
        void Create(T item);
        void Update(T item);
        void Delete(int id);

        int Count();
    }
}
