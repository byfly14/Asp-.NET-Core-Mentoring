using System;
using Microsoft.EntityFrameworkCore;

namespace Asp._NET_Core_Mentoring_Module1.Helpers
{
    public static class TransactionHelper
    {
        public static bool TryPerformTransaction(DbContext context, Action action, out Exception exception)
        {
            exception = null;
            using var transaction = context.Database.BeginTransaction();
            try
            {
                action.Invoke();
                transaction.Commit();
                return true;
            }
            catch(Exception ex)
            {
                exception = ex;
                transaction.Rollback();
                return false;
            }
        }
    }
}
