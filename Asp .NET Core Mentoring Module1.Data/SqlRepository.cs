using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Asp_.NET_Core_Mentoring_Module1.Data
{
    public class SqlRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly NorthWindContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public SqlRepository(NorthWindContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.AsNoTracking();
        }

        public IEnumerable<TEntity> GetAllWithInclude(IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
        {
            var getAllQuery = _dbSet.AsNoTracking();
            return Include(getAllQuery, includeProperties).ToList();
        }

        public TEntity GetById(int id)
        {
            return _dbSet.Find(id);
        }

        public TEntity GetByIdWithInclude(Func<TEntity, bool> predicate, 
            IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
        {
            return Include(_dbSet.AsNoTracking(), includeProperties).SingleOrDefault(predicate);
        }


        public void Create(TEntity item)
        {
            throw new NotImplementedException();
        }

        public void Update(TEntity item)
        {
            _context.Entry(item).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }


        public int Count()
        {
            return _dbSet.Count();
        }

        private static IEnumerable<TEntity> Include(IQueryable<TEntity> query,
            IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
        {
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}
