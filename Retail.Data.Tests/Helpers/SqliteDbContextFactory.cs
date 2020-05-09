using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;

namespace Retail.Data.Tests.Helpers
{
    public class SqliteDbContextFactory<T> : IDisposable where T : DbContext
    {
        private DbConnection _connection;

        public T CreateContext()
        {
            if (_connection == null)
            {
                var connStrBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };

                _connection = new SqliteConnection(connStrBuilder.ConnectionString);
                _connection.Open();

                var context = CreateNewContext();
                context.Database.EnsureCreated();
                return context;
            }

            return CreateNewContext();
        }

        private T CreateNewContext()
        {
            var options = CreateOptions();
            return (T)Activator.CreateInstance(typeof(T), options);
        }

        private DbContextOptions<RetailContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<RetailContext>()
                .UseSqlite(_connection).Options;
        }

        public void Dispose()
        {
            _connection?.Dispose();
            _connection = null;
        }
    }

}
