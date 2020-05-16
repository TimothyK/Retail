using Microsoft.EntityFrameworkCore;
using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.EfModels.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Data.SqlDb.Tests.TestRecordFactory
{
    internal static class StoreFactory
    {
        public static Store CreateStore(this RetailDbContext db)
        {
            var id = IdFactory.Next();
            var store = new Store
            {
                Address = $"{id} Main Street",
                City = $"{id}ville",
                Province = $"{id} District",
                Country = $"{id}land",
                PostalCode = $"{id}",
                PhoneNumber = $"({id}) {id}-{id}",
                StoreName = $"Store {id}",
                Active = true,
            };
            db.Add(store);

            return store;
        }
    }
}
