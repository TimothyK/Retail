# Overview
This git repository is a working example of a fully separated Business and Data layer, using EF Core and SQL Server as the data layer.  This repo is a companion to my blog post [The 17-Layered App](http://geekswithblogs.net/TimothyK/archive/2019/07/30/the-17-layered-app.aspx).  This solution only has the Business and Data layers, no UI layers.  So that's one of the reasons it is only 10 projects and not 17.

As I stated in the blog post, there is no one right way to design all software solutions.  The code in this repo is not meant to be a gold standard that should be used as a template for every project.  This code is an example of how Business and Data layers can be written and have them completely separate from each other, and fully tested.  This is meant to be the start of a discussion.  From this example, please discuss what is good or bad about it.

# Tech Stack
* C#
* EF Core
* Dapper
* SQL Server LocalDb, and options for Sqlite and In-Memory test databases.
* AutoMapper
* MSTest
* Moq
* Serilog, with Console and Seq sinks

# Project Overview
## Retail.Services
This is the primary project for the Business layer.

This project does a rather poor job of offering three "bounded contexts" (see [Domain-driven design](https://en.wikipedia.org/wiki/Domain-driven_design)).  In a proper DDD bounded context each context would have its own database.  Here for simplicity all share the same database.

There are 3 services: a Store Locator, a Customer Service, and an Order Service.

The Store Locator offers a list of all stores or getting a single store by ID.  For simplicity there are no features to add, remove, or edit stores.

The Customer Service is a simple lookup of a customer based on their Membership Number (a Guid).  For simplicity there are no features to add, remove, or edit customers.  Note that customers can have a Discount.  However that is not part of the Customer Service.  That belongs to the Order Service bounded context, which is where decisions as to product pricing are made.

The Order Service is allows for creation of orders.  In order to do that the Order Service first provides a list of available product inventory at each store.  It also includes product pricing information, including options for globally available sales prices or individual customer discounts.  Product inventory at each store is decremented when an order is submitted.  For simplicity it only allows for order creation, and not editing of existing orders.

## Retail.Data.Abstractions
This project defines an abstraction for what the business layer needs from the data layer.  

The person designing the needs of the business (i.e. the Retail.Services project) gets to decide what goes into this Retail.Data.Abstractions layer.  It is not the DBA or the owner of the Data layer that gets to decide what is in this library.  The design of the Data.Abstraction should not simply be a carbon copy of the SQL Server database table structure.  If the two are extremely similar then this is a red flag that your business and data layers are being designed independently of each other.  It is an [Anemic Design](https://martinfowler.com/bliki/AnemicDomainModel.html).

This goes back to *Dependency Inversion Principle*:
* High-level modules should not depend on low-level modules. Both should depend on abstractions.
* Abstractions should not depend on details. Details should depend on abstractions.

This Retail.Data.Abstractions is that abstraction.

The interfaces in this library are: IStoreRepository, ICustomerRepository, and IOrderRepository.

## Retail.Services.UnitTests
Yes, your software solutions should have automated tests.  One of the advantages of have a very separated business and data layer is that it is very easy to test the business logic (Retail.Services) without any database.  This is exactly what this Retail.Services.UnitTests project does.

For the Store Locator a simple POCO test double class is used to implement the Retail.Data.Abstractions.  For the Customer and Order Services, Moq is used to provide an implementation for those abstractions.

## Retail.Data.SqlDb
This library is the implementation of the Retail.Data.Abstraction library.  In its implementation it has a hard dependency on the SQL Server database.

This library has a dependency on Retail.Data.SqlDb.EfModels, which is the next layer down.  That library uses EF Core to access the SQL Server database.  In theory the Retail.Data.SqlDb.EfModels library could be merged into the Retail.Data.SqlDb.  In this example, I've kept them separate to clearly note the separation of layers.

## Retail.Data.SqlDb.EfModels
This library has the DbContext and Models generated by EF Power Tools.

In this example project the database was generated first, then EF Power Tools was used to create the Models and RetailDbContext.  This example project does not use the EF Migrations feature.

## Retail.Data.SqlDb.Tests
## Retail.Data.SqlDb.TestRecordFactory
## Retail.Data.SqlDb.Database
## TimothyK.Data.UnitOfWork
## Retail.Services.IntegrationTests
