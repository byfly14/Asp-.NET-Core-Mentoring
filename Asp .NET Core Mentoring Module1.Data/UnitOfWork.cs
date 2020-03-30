using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Asp_.NET_Core_Mentoring_Module1.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ILoggerFactory _loggerFactory;

        public DbContext Context { get; }

        public Dictionary<Type, object> Repositories { get; } = new Dictionary<Type, object>();

        public IRepository<T> Repository<T>() where T : class
        {
            if (Repositories.Keys.Contains(typeof(T)))
            {
                return Repositories[typeof(T)] as IRepository<T>;
            }

            IRepository<T> repo = new SqlRepository<T>(Context as NorthWindContext, _loggerFactory);
            Repositories.Add(typeof(T), repo);
            return repo;
        }

        public UnitOfWork(NorthWindContext dbContext, ILoggerFactory loggerFactory)
        {
            Context = dbContext;
            _loggerFactory = loggerFactory;
        }

        public int Commit()
        {
            return Context.SaveChanges();
        }
    }
}
