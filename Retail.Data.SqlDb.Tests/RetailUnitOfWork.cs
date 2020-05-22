using Retail.Data.SqlDb.Database;
using TimothyK.Data.UnitOfWork;

namespace Retail.Data.SqlDb.Tests
{
    public class RetailUnitOfWork : SqlServerUnitOfWork
    {
        public RetailUnitOfWork(RetailLocalDbAttacher attachedDatabase) : base(attachedDatabase.ConnectionString)
        {
            AddBuilderOptions(builder => builder.UseLoggerFactory(BaseTests.LoggerFactory));
            AddBuilderOptions(builder => builder.EnableSensitiveDataLogging());
        }
    }
}
