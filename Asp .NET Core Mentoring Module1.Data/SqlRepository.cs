using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Asp_.NET_Core_Mentoring_Module1.Data
{
    public class SqlRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly NorthWindContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private readonly ILogger _logger;

        public SqlRepository(NorthWindContext context, 
            ILoggerFactory loggerFactory)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _logger = loggerFactory.CreateLogger(nameof(SqlRepository<TEntity>));
        }

        public IEnumerable<TEntity> GetAll()
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(GetAll)}");
                return _dbSet.AsNoTracking();
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(GetAll)}");
            }
        }

        public IEnumerable<TEntity> GetAllWithInclude(IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(GetAllWithInclude)} {includeProperties}");
                var getAllQuery = _dbSet.AsNoTracking();
                return Include(getAllQuery, includeProperties).ToList();
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(GetAllWithInclude)}");
            }
        }

        public TEntity GetById(int id)
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(GetById)} {id}");
                return _dbSet.Find(id);
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(GetById)}");
            }
        }

        public TEntity GetByIdWithInclude(Func<TEntity, bool> predicate, 
            IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(GetByIdWithInclude)} {predicate} {includeProperties}");
                return Include(_dbSet.AsNoTracking(), includeProperties).SingleOrDefault(predicate);
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(GetByIdWithInclude)}");
            }
        }


        public void Create(TEntity item)
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(Create)} {item}");
                _dbSet.Add(item);
                _context.SaveChanges();
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(Create)}");
            }

        }

        public void Update(TEntity item)
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(Update)} {item}");
                _context.Entry(item).State = EntityState.Modified;
                _context.SaveChanges();
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(Update)}");
            }
        }

        public void Delete(TEntity item)
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(Delete)} {item}");
                _dbSet.Remove(item);
                _context.SaveChanges();
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(Delete)}");
            }
        }


        public int Count()
        {
            try
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} Start of {nameof(Count)}");
                return _dbSet.Count();
            }
            finally
            {
                _logger.LogDebug($"{nameof(SqlRepository<TEntity>)} End of {nameof(Count)}");
            }
        }

        private static IEnumerable<TEntity> Include(IQueryable<TEntity> query,
            IEnumerable<Expression<Func<TEntity, object>>> includeProperties)
        {
            return includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
        }
    }
}
