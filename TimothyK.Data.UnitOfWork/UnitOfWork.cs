using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data.Common;

namespace TimothyK.Data.UnitOfWork
{
    public abstract class UnitOfWork : IDisposable
    {
        #region Database Connection

        protected DbConnection DbConnection { get; private set; }
        protected DbTransaction DbTransaction { get; private set; }

        public virtual TContext CreateDbContext<TContext>() where TContext : DbContext
        {
            var builder = CreateOptionsBuilder<TContext>();
            foreach (var addOptions in AddOptions)
                addOptions.Invoke(builder);

            var dbContext = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options);

            if (DbConnection != null && dbContext.Database.GetDbConnection() != DbConnection)
            {
                throw new InvalidOperationException($"DbContext was created with a different database connection that was previously used.  Check {nameof(CreateOptionsBuilder)} implementation");
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

        protected abstract DbContextOptionsBuilder<TContext> CreateOptionsBuilder<TContext>() where TContext : DbContext;

        private readonly List<Func<DbContextOptionsBuilder, DbContextOptionsBuilder>> AddOptions = 
            new List<Func<DbContextOptionsBuilder, DbContextOptionsBuilder>>();

        public UnitOfWork AddBuilderOptions(Func<DbContextOptionsBuilder, DbContextOptionsBuilder> addOptions)
        {
            AddOptions.Add(addOptions);
            return this;
        }

        public void Commit()
        {
            DbTransaction.Commit();
        }

        public virtual void Dispose()
        {
            DbTransaction?.Dispose();
            DbConnection?.Dispose();
        }

        #endregion

    }
}
