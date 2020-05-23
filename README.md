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

This project does a rather poor job of offering three "bounded contexts" (see [Domain Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design)).  In a proper DDD bounded context each context would have its own database.  Here for simplicity all share the same database.

There are 3 services: a Store Locator, a Customer Service, and an Order Service.

The Store Locator offers a list of all stores or getting a single store by ID.  For simplicity there are no features to add, remove, or edit stores.

The Customer Service is a simple lookup of a customer based on their Membership Number (a Guid).  For simplicity there are no features to add, remove, or edit customers.  Note that customers can have a Discount.  However, that is not part of the Customer Service.  That belongs to the Order Service bounded context, which is where decisions as to product pricing are made.

The Order Service allows for creation of orders.  To do that the Order Service first provides a list of available product inventory at each store.  It also includes product pricing information, including options for globally available sales prices or individual customer discounts.  Product inventory at each store is decremented when an order is submitted.  For simplicity it only allows for order creation, and not editing of existing orders.

## Retail.Data.Abstractions
This project defines an abstraction for what the business layer needs from the data layer.  

The person designing the needs of the business (i.e. the Retail.Services project) gets to decide what goes into this Retail.Data.Abstractions layer.  It is not the DBA or the owner of the Data layer that gets to decide what is in this library.  The design of the Data.Abstraction should not simply be a carbon copy of the SQL Server database table structure.  If the two are extremely similar, then this is a red flag that your business and data layers are being designed independently of each other.  It is an [Anemic Design](https://martinfowler.com/bliki/AnemicDomainModel.html).

This goes back to *Dependency Inversion Principle*:
* High-level modules should not depend on low-level modules. Both should depend on abstractions.
* Abstractions should not depend on details. Details should depend on abstractions.

This Retail.Data.Abstractions is that abstraction.

The interfaces in this library are: IStoreRepository, ICustomerRepository, and IOrderRepository.

## Retail.Services.UnitTests
Yes, your software solutions should have automated tests.  One of the advantages of have a very separated business and data layer is that it is extremely easy to test the business logic (Retail.Services) without any database.  This is exactly what this Retail.Services.UnitTests project does.

For the Store Locator a simple POCO test double class is used to implement the Retail.Data.Abstractions.  For the Customer and Order Services, Moq is used to provide an implementation for those abstractions.

## Retail.Data.SqlDb
This library is the implementation of the Retail.Data.Abstraction library.  In its implementation it has a hard dependency on the SQL Server database, althought there is still an ORM layer between Retail.Data.SqlDb and the database.

This library has a dependency on Retail.Data.SqlDb.EfModels, which is the next layer down.  That library uses EF Core to access the SQL Server database.  In theory the Retail.Data.SqlDb.EfModels library could be merged into the Retail.Data.SqlDb.  In this example, I've kept them separate to clearly note the separation of layers.

In theory this library could use multiple ORMs or microservice layers to provide access the SQL Server database.  In fact, the OrderRepository.GetAvailableProducts method uses Dapper instead of EF to query the database.  In that case I choose to keep that code in the Retail.Data.SqlDb library instead of creating a separate Retail.Data.SqlDb.Dapper library like was done for the EfModels.

## Retail.Data.SqlDb.EfModels
This library hosts the DbContext and Models generated by EF Power Tools.

In this example project the database was generated first, then EF Power Tools was used to create the Models and RetailDbContext.  This example project does not use the EF Migrations feature.

## Retail.Data.SqlDb.Tests
This project has automated tests for the Retail.Data.SqlDb implementation.

The project is purposely suffixed with "Tests" to avoid a symantec arguement over whether these are Unit Tests or Integration Tests.  These tests do hit the database and rollback the transaction after each test.  Because they hit the database, most people would refer to these as Integration Tests.  Others may argue that this is just testing a single Data Layer (although there are sub-layers in there), therefore the term "Unit Test" may still be applicable.

These tests do use a SQL Server LocalDb instance to run the tests against (See Retail.Data.SqlDb.Database).  However, it is extremely easy to change the code to run these tests against a different database.  In fact, the StoreLocatorRepository tests use an InMemory EF database instead of a LocalDb.  Switching between databases is as easy as using a different subclass of the `UnitOfWork` (see TimothyK.Data.UnitOfWork).

In theory switching between databases and following Liskov Substitution Principle should be easy, however that is rarely the case.  Each DBMS has its own quirks (e.g. SQL syntax/dialect) and limitations.  You want to use the fastest DBMS you can for your automated tests that still supports all the features required by the implementation of the Retail.Data.SqlDb library.  I'd recommend starting writing tests against an InMemory EF database but structuring the tests that they can very easily be switched to use a Sqlite or SQL Server LocalDb when required.

I highly discourage adding IF statements to the production Retail.Data.SqlDb code to accomodate different DBMS types.  All that code behind the IF statements still needs to be tested.  Adding to the production code IF statements to have it behave differently under test than in production is a huge code smell.  Instead just change out the test database to one that is closer to the production environment when required.

## Retail.Data.SqlDb.TestRecordFactory
This isn't yet another layer in the design of the system.  This library is simply some helper functions to easily create test records.  This code is placed in a separate project so that it can be shared between the Retail.Data.SqlDb.Tests and Retail.Service.IntegrationTests projects.

Creating test records is one of the most painful parts of writing tests against an actual database.  It is worth the effort to make this task as simple as possible.  It removes duplication (DRY) in the tests.  Making sure test records are created consistently improves the reliability of the tests.  It creates a Single Source of Truth so that if there is a need to change how test records are created this can be done in the TestRecordFactory instead of having to change every test.  

Having a TestRecordFactory makes it easy and enjoyable to write tests.  Without this library, I find developers are reluctant to do automated testing.

This Test Record Factory is simple and is just a few extension methods.  A Test Record Factory library can have classes a Builder or [Object Mother](https://martinfowler.com/bliki/ObjectMother.html) design pattern to create more complex test records.

---
This Retail.Data.SqlDb.TestRecordFactory library creates test records using the same `RetailDbContext` defined in the Retail.Data.SqlDb.EfModels library.  That is the same DbContext that the production Retail.Data.SqlDb library uses.  Some may consider this as a bit of a code smell.

There is no reason that the creation of test records needs to use the same DbContext or code base that the production code uses.  Often the two libraries have two very different responsiblities.  

The test projects typically need a data layer that is incredibly good at creating test records (and perhaps asserting the correct values were written to the database).  The production code may have different requirement.  A production reporting library might only need to do complex queries on the database and might not need any create/update functionality at all.  

So, to follow the Single Responsibility Principle the libraries should be separate.  However, that's not what is done in this example.

## Retail.Data.SqlDb.Database
The LocalDb MDF & LDF files are checked into this git repository in this Retail.Data.SqlDb.Database project.  Automated test projects (Retail.Data.SqlDb.Tests and Retail.Service.IntegrationTests) can easily include this project to have the MDF & LDF files copies to the bin output folder.

This project also has a `RetailLocalDbAttacher` class that can be used from the test classes to create a copy of the of the database.  Each test class creates its own copy of the database.  It is attached to the local DB instance on the Test Class Initialize method and the database is drop (implying detacted and files deleted) in the Test Class Cleanup method.

If database changes are required, the MDF and LDF files can be edited by attaching them to a SQL Server instance, making the changes, detaching the database, then committing the changed MDF/LDF files.  However, these are binary files.  It can be difficult for others to see what changes are being made to the database.  I'd recommend that some where there are SQL scripts to more easily track the changes to the database.  This gets into a much larger discussion on database versioning, which is beyond the scope of this example.  The SQL scripts used to create the tables in the MDF file are in the 'Database\Create Retail.sql' file in this project.

After the changes to the MDF and LDF files are done, the EF Power Tools can be run again to update the Retail.Data.SqlDb.EfModels project.

## TimothyK.Data.UnitOfWork
This isn't yet another layer in the design of the system.  This is an implementation of the Unit Of Work design pattern.

The repository implementation (Retail.Data.SqlDb, i.e. the Data layer) should not be the component that controls when data is committed to the database [Evans, Domain Driven Design, p 156].  Instead this should be controlled by the calling program.  Typically, it would be the Controller in an MVC program (the UI layer) that controls when the Commit method.

The Unit Of Work allows for the calling program to control the database transactions.  The automated tests can rollback the database to reset the database to a clean state before the next test is run.

Although EF's DbContext class does already implement the Unit Of Work design pattern, that turns over control of the unit of work to the data layer.  It also does not allow for multiple DbContext objects or other ORMs (e.g. Dapper) to partipate in the same unit of work.

The `UnitOfWork` class contains a `CreateDbContext` method that allows for creation of multiple DbContext objects, all which share the same database transaction.  This allows for the use of multiple DbContext classes to partipate in the same unit of work (not present in this example).  It also allows for using different DbContext instances to be used in the Arrange, Act, and Assert sections of the test methods.  It is recommended that different DbContext objects are used in these 3 sections of the code.  This way cached model objects aren't used, and the data read is always coming from the database.

Another advantage of this UnitOfWork implementation is that it controls which DBMS to connect to.  `UnitOfWork` is an abstract base class.  Separate subclasses are implements that connect to an InMemory EF database, an in-memory Sqlite datbase, or SqlServer database.

## Retail.Services.IntegrationTests
This project contains a [small number](https://martinfowler.com/bliki/TestPyramid.html "Test Pyramid") (currently one) integration test.  It ties the Retail.Service, Retail.Data.SqlDb, and Retail.Data.SqlDb.Database layers together to make sure that all these layers align properly when put together.

#Further Reading
* [Domain Driven Design](https://dddcommunity.org/book/evans_2003/), Eric Evans
* [Patterns of Enterprise Application Architecture](https://martinfowler.com/books/eaa.html), Martin Fowler
* [TDD is dead. Long Live testing.](https://dhh.dk/2014/tdd-is-dead-long-live-testing.html), David Heinemeier Hansson
  * and the fallout dicussion with Martin Fowler and Kent Beck on YouTube, [Part 1](https://www.youtube.com/watch?v=z9quxZsLcfo) of 5
* [Using the Repository and Unit Of Work Pattern in .net core](https://garywoodfine.com/generic-repository-pattern-net-core/), Gary Woodfine
