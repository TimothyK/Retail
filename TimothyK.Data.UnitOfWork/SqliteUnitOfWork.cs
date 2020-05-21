using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace TimothyK.Data.UnitOfWork
{
    public class SqliteUnitOfWork : UnitOfWork
    {
        private SqliteConnection _dbConnection;

        public override TContext CreateDbContext<TContext>()
        {
            if (_dbConnection == null)
            {
                var connStrBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };

                _dbConnection = new SqliteConnection(connStrBuilder.ConnectionString);
                _dbConnection.Open();

                var builder = CreateOptionsBuilder<TContext>();
                using (var dbContext = (TContext)Activator.CreateInstance(typeof(TContext), builder.Options))
                    dbContext.Database.EnsureCreated();            
            }

            var context = base.CreateDbContext<TContext>();
            return context;
        }

        protected override DbContextOptionsBuilder<TContext> CreateOptionsBuilder<TContext>() =>
            new DbContextOptionsBuilder<TContext>()
                .UseSqlite(DbConnection ?? _dbConnection);

    }
}
