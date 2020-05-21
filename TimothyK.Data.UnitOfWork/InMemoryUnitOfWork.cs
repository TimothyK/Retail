using Microsoft.EntityFrameworkCore;
using System;

namespace TimothyK.Data.UnitOfWork
{
    public class InMemoryUnitOfWork : UnitOfWork
    {
        private string DatabaseName { get; } = Guid.NewGuid().ToString();

        protected override DbContextOptionsBuilder<TContext> CreateOptionsBuilder<TContext>()
        {
            return new DbContextOptionsBuilder<TContext>()
                .UseInMemoryDatabase(DatabaseName);
        }
    }
}
