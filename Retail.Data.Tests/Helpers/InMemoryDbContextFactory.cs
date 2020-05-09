using Microsoft.EntityFrameworkCore;
using System;
using System.Reflection;

namespace Retail.Data.Tests.Helpers
{
    public class InMemoryDbContextFactory<TContext> : DbContextFactory<TContext> where TContext:DbContext
    {
        public InMemoryDbContextFactory(Type testClassType, string testMethodName = null) : base(testClassType, testMethodName)
        {
        }

        public InMemoryDbContextFactory(Assembly assembly, string testContext = null) : base(assembly, testContext)
        {
        }

        protected override DbContextOptions<TContext> CreateOptions()
        {
            return new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(DatabaseName)
                .Options;
        }
    }
}
