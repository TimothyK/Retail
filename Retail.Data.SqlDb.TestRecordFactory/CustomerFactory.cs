using Retail.Data.SqlDb.EfModels;
using Retail.Data.SqlDb.EfModels.Models;

namespace Retail.Data.SqlDb.TestRecordFactory
{
    public static class CustomerFactory
    {
        public static Customer CreateCustomer(this RetailDbContext db)
        {
            var id = IdFactory.Next();
            var customer = new Customer
            {
                FirstName = $"Bob{id}",
                LastName = $"Robertson{id}",
                Address = $"{id} Main Street",
                City = $"{id}ville",
                Province = $"{id} District",
                Country = $"{id}land",
                PostalCode = $"{id}",
                PhoneNumber = $"({id}) {id}-{id}",
                Active = true,
                Discount = 0.0
            };
            db.Add(customer);

            return customer;
        }
    }
}
