using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Asp_.NET_Core_Mentoring_Module1.Data
{
    public interface IUnitOfWork
    {
        DbContext Context { get; }
        Dictionary<Type, object> Repositories { get; }
        IRepository<T> Repository<T>() where T : class;
        int Commit();
    }
}
