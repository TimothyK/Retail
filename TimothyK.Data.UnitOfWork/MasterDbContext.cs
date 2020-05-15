using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Text;

namespace TimothyK.Data.UnitOfWork
{
    /// <summary>
    /// DbContext for the master database of a SQL Server database
    /// </summary>
    internal class MasterDbContext : DbContext
    {
        public MasterDbContext()
        {
        }

        public MasterDbContext(DbContextOptions<MasterDbContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ConnectionString.ConnectionString);
            }
        }

        private static readonly SqlConnectionStringBuilder _defaultConnectionString =          
            new SqlConnectionStringBuilder
            {
                DataSource = @"(localdb)\mssqllocaldb",
                IntegratedSecurity = true,
                InitialCatalog = "master"
            };
        private static SqlConnectionStringBuilder _connectionString = _defaultConnectionString;

        public static SqlConnectionStringBuilder ConnectionString
        {
            get => new SqlConnectionStringBuilder(_connectionString.ConnectionString);
            set => _connectionString = new SqlConnectionStringBuilder((value ?? _defaultConnectionString).ConnectionString);
        }

        public void DropDatabase(string dbName)
        {
            var sql = BuildSqlDropDatabase(dbName);
            Database.ExecuteSqlRaw(sql);
        }

        private string BuildSqlDropDatabase(string dbName)
        {
            var sql = new StringBuilder();
            sql.AppendLine($"if Exists(Select 1 From sys.databases db Where (db.name = '{dbName}'))");
            sql.AppendLine("Begin")
                .AppendLine();

            sql.AppendLine($"Alter Database [{dbName}]")
                .AppendLine("Set Offline")
                .AppendLine("With Rollback Immediate")
                .AppendLine();

            sql.AppendLine($"Drop Database [{dbName}]")
                .AppendLine();

            sql.AppendLine("End")
                .AppendLine();

            return sql.ToString();
        }

        public void AttachDatabase(string dbName, params FileInfo[] files)
        {
            var sql = new StringBuilder()
                .AppendLine(BuildSqlDropDatabase(dbName))
                .AppendLine(BuildSqlAttachDatabase(dbName, files))
                .ToString();

            Database.ExecuteSqlRaw(sql);
        }

        private string BuildSqlAttachDatabase(string dbName, FileInfo[] files)
        {
            var sql = new StringBuilder();
            sql.AppendLine($"Create Database [{dbName}]");
            sql.Append("On").AppendLine(string.Join("\r\n\t, ",
                files.Select(dbFile => $"(Filename = '{dbFile.FullName}')")));
            sql.AppendLine("For Attach");

            return sql.ToString();
        }

    }
}
