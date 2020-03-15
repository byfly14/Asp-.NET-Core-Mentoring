using System;
using System.Collections.Generic;

namespace Asp_.NET_Core_Mentoring_Module1.Data
{
    public interface IUnitOfWork
    {
        NorthWindContext Context { get; }
        Dictionary<Type, object> Repositories { get; }
        IRepository<T> Repository<T>() where T : class;
        int Commit();
        void Rollback();
    }
}
