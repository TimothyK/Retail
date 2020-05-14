using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data.Common;

namespace TimothyK.Data.UnitOfWork
{
    public abstract class UnitOfWork : IDisposable
    {
        protected DbConnection DbConnection { get; private set; }
        protected DbTransaction DbTransaction { get; private set; }

        public TContext CreateDbContext<TContext>() where TContext : DbContext
        {
            var options = CreateOptions<TContext>();
            var dbContext = (TContext)Activator.CreateInstance(typeof(TContext), options);

            if (DbConnection != null && dbContext.Database.GetDbConnection() != DbConnection)
            {
                throw new InvalidOperationException($"DbContext was created with a different database connection that was previously used.  Check {nameof(CreateOptions)} implementation");
            }
            DbConnection = dbContext.Database.GetDbConnection();

            if (DbTransaction == null)
            {
                dbContext.Database.BeginTransaction();
                DbTransaction = dbContext.Database.CurrentTransaction.GetDbTransaction();
            }
            else
            {
                dbContext.Database.UseTransaction(DbTransaction);
            }

            return dbContext;
        }

        protected abstract DbContextOptions<TContext> CreateOptions<TContext>() where TContext : DbContext;

        public void Commit()
        {
            DbTransaction.Commit();
        }

        public void Dispose()
        {
            DbTransaction?.Dispose();
            DbConnection?.Dispose();
            DbTransaction = null;
            DbConnection = null;
        }
    }
}
