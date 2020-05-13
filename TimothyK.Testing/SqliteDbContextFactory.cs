using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Reflection;

namespace TimothyK.Testing
{
    public class SqliteDbContextFactory<TContext> : DbContextFactory<TContext> where TContext : DbContext
    {
        public SqliteDbContextFactory(Type testClassType, string testMethodName = null) : base(testClassType, testMethodName)
        {
        }

        public SqliteDbContextFactory(Assembly assembly, string testContext = null) : base(assembly, testContext)
        {
        }

        private DbConnection _connection;

        public override TContext CreateContext()
        {
            if (_connection == null)
            {
                var connStrBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };

                _connection = new SqliteConnection(connStrBuilder.ConnectionString);
                _connection.Open();

                var context = base.CreateContext();
                context.Database.EnsureCreated();
                return context;
            }

            return base.CreateContext();
        }

        protected override DbContextOptions<TContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<TContext>()
                .UseSqlite(_connection).Options;
        }

        public override void Dispose()
        {
            _connection?.Dispose();
            _connection = null;

            base.Dispose();
        }
    }

}
