using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace TimothyK.Data.UnitOfWork
{
    public class SqlServerUnitOfWork : UnitOfWork
    {
        private readonly SqlConnectionStringBuilder _connectionStringBuilder;

        public SqlServerUnitOfWork(SqlConnectionStringBuilder connectionStringBuilder)
        {
            _connectionStringBuilder = connectionStringBuilder;
        }

        protected override DbContextOptionsBuilder<TContext> CreateOptions<TContext>()
        {
            var contextBuilder = new DbContextOptionsBuilder<TContext>();
            if (DbConnection == null)
                contextBuilder.UseSqlServer(_connectionStringBuilder.ConnectionString);
            else
                contextBuilder.UseSqlServer(DbConnection);

            return contextBuilder;
        }

    }
}
