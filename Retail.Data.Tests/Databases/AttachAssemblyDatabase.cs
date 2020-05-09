using Microsoft.VisualStudio.TestTools.UnitTesting;
using Retail.Data.Tests.Extensions;
using System.Reflection;

namespace Retail.Data.Tests.Databases
{
    /// <summary>
    /// Attaches a localDb database that all test classes could use, if they want.  
    /// Test class can use <see cref="LocalDbContextFactory.CreateContext"/> to connect to this database.
    /// </summary>
    [TestClass]
    public class AttachAssemblyDatabase
    {
        /// <summary>
        /// Test Context for a localDb that can be used by all test classes
        /// </summary>
        public static LocalDbContextFactory<RetailDbContext> LocalDbFactory { get; } = 
            new LocalDbContextFactory<RetailDbContext>(Assembly.GetExecutingAssembly());

        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            LocalDbFactory.AttachDatabase(@"Databases\Retail.mdf", @"Databases\Retail_log.ldf");
        }

        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            LocalDbFactory.Dispose();
        }
    }
}
