/* Create Retail 1.0
** Prepared: May 5, 2020 (TK)
**
** Creates all tables for the Retail database product
*/

Print 'Running script:  Create Retail 1.0'
GO

/* Prerequisite Checks */
-- DBMS Version 
Declare @dbmsVersion varchar(255)
Set @dbmsVersion = Cast(SERVERPROPERTY('productversion') As varchar)
--Add leading zero to SQL Server version numbers before 10.
If (SUBSTRING(@dbmsVersion,2,1) = '.')
	Set @dbmsVersion = '0' + @dbmsVersion

if (@dbmsVersion < '10.5')
	Begin
	RAISERROR ('Prerequisite Error: Requires SQL Server 2008-R2 or later', 17, 1) 
	set nocount on
	set noexec on
	End
GO

--Permission Check
If Not Exists(SELECT permission_name FROM fn_my_permissions(DB_NAME(), 'DATABASE') Where (permission_name In ('CREATE TABLE')))
	Begin
	RAISERROR ('Prerequisite Error: Insufficient Privileges.  Requires permissions to create and modify tables.  Check that this script is being run by the correct login.', 17, 1) 
	set nocount on
	set noexec on
	End
GO


--Installed Product Check
Declare @DBProduct varchar(50)
Declare @MajorVersionNumber varchar(4)
Declare @MinorVersionNumber varchar(20)

Set @DBProduct = 'Retail'
Set @MajorVersionNumber = '1'
Set @MinorVersionNumber = '0'

If Exists (Select 1 From sys.tables Where (name = 'DBVersion'))
	If Exists (Select 1 From dbo.DBVersion Where (DBProduct = @DBProduct) And (IsSuspect = 1))
		Begin
		RAISERROR ('Prerequisite Error: This database is marked as suspect.  Previous errors must be investigated and the integrity of the database verified.  Contact Process Solutions for assistance repairing this database.', 17, 1) 
		set nocount on
		set noexec on
		End
	Else If Exists (Select 1 From dbo.DBVersion Where (DBProduct = @DBProduct) And ((Cast(PARSENAME(Version ,2) as int) = Cast(@MajorVersionNumber as int) )))
		Begin
		Print '                 Create ' + @DBProduct + ' ' + @MajorVersionNumber + '.' + @MinorVersionNumber + '...Skipping (already installed)'
		set nocount on
		set noexec on
		End
	Else If Exists (Select 1 From dbo.DBVersion Where (DBProduct = @DBProduct))
		Begin
		Declare @v varchar(200)
		Select @v = DBProduct + ' ' + Version
		From dbo.DBVersion Where (DBProduct = @DBProduct)
		Print '  The previously installed database version is: ' + IsNull(@v, 'not found/installed')

		RAISERROR ('Prerequisite Error: The product is already installed at a different major version.  Two separate major versions cannot coexist in the same database catalog.', 17, 1) 
		set nocount on
		set noexec on
		End

-- Is Current DB set?
If (DB_NAME() = 'master')
	Begin
	RAISERROR ('Prerequisite Error: This script is not appropriate to run against the [master] database catalog.  Select a different catalog.', 17, 1) 
	set nocount on
	set noexec on
	End
GO

/* DBHistory table */
-- This table is a standard for all Process Solutions Database Products.
-- It records audit information for all official Process Solutions scripts (versioned or not) 
-- that were released by a PSCL Developer and run against this database. 
If Not Exists (Select 1 From sys.tables Where (name = 'DBHistory'))
	Begin
	Print 'Creating DBHistory table'
	
	CREATE TABLE dbo.DBHistory(
		ExecutionID int Identity(1,1) NOT NULL
			Constraint PK_DBHistory
			Primary Key Clustered
		, ScriptName varchar(100) NOT NULL
		, ExecutionDateUtc datetime NOT NULL 
			Constraint DF_DBHistory_ExecutionDate 
			Default getutcdate()
		, ExecutedBy [varchar](50) 
			Constraint DF_DBHistory_ExecutedBy
			Default suser_sname()
		, DBProduct varchar(50) NULL
		, BeforeVersion varchar(50) NULL
		, AfterVersion varchar(50) NULL
		, BeforeSuspect bit NULL
		, AfterSuspect bit NULL
		, Notes varchar(2000) NULL
	)

	Print 'Creating DBHistory table...Done'
	End
GO

/* Create DBHistory entry */
Set NOCOUNT ON

Insert Into dbo.DBHistory (ScriptName, Notes, DBProduct)
Values ('Create Retail 1.0', 'Creates all objects for Retail', 'Retail')

Set NOCOUNT OFF
GO


/* Create DBVersion table */
-- This table is a standard for all Process Solutions Database Products.
-- It records all Process Solutions database products that are installed in the current catalog,
-- and their current version number. 
If Not Exists (Select 1 From sys.tables Where (name = 'DBVersion'))
	Begin
	Print 'Creating DBVersion table'
	
	CREATE TABLE dbo.DBVersion(
		DBProduct varchar(50) Not Null
			Constraint PK_DBVersion
			Primary Key Clustered
		, Version varchar(50) not null
		, SentryEmbeddedVersion varchar(50)
		, IsSuspect bit
			Constraint DF_DBVersion_IsSuspect
			Default 0
		, SuspectTimeUtc datetime
		, Notes varchar(2000)
	)

	Print 'Creating DBVersion table...Done'
	End
GO

-- Create Database Version record
Print ''
Print 'Creating database version row'
GO
Insert Into dbo.DBVersion (DBProduct, Version, SentryEmbeddedVersion, Notes)
Values ('Retail', '1.0', '1.0', @@SERVERNAME + '.' + DB_NAME())
GO
Print 'Creating database version row...Done'
GO

/****************************************************************************************************
************************************  Start of script  **********************************************
****************************************************************************************************/


/* Create dbo.Products table */
Print ''
Print 'Create dbo.Products table'
GO

If Exists (
	Select 1 
	From sys.tables tbl 
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Products')
)
	Begin
		Print 'Create dbo.Products table...Skipping (already created)'
	End
Else
	Begin
		Create Table dbo.Products (
			ProductId int not null
				Identity(1,1)
				Constraint PK_Products
				Primary Key
			, ProductName nvarchar(50) not null
			, Price decimal null
		)

		Print 'Create dbo.Products table...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create dbo.Products table...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create dbo.Products table' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create dbo.Customers table */
Print ''
Print 'Create dbo.Customers table'
GO

If Exists (
	Select 1 
	From sys.tables tbl 
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Customers')
)
	Begin
		Print 'Create dbo.Customers table...Skipping (already created)'
	End
Else
	Begin
		Create Table dbo.Customers (
			CustomerId int not null
				Identity(1,1)
				Constraint PK_Customers
				Primary Key
			, FirstName nvarchar(50) null
			, LastName nvarchar(50) null
			
			, Address nvarchar(100) null
			, City nvarchar(50) null
			, Province nvarchar(50) null
			, Country nvarchar(50) null
			, PostalCode varchar(20) null

			, PhoneNumber varchar(20) null
		)

		Print 'Create dbo.Customers table...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create dbo.Customers table...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create dbo.Customers table' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create dbo.Stores table */
Print ''
Print 'Create dbo.Stores table'
GO

If Exists (
	Select 1 
	From sys.tables tbl 
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Stores')
)
	Begin
		Print 'Create dbo.Stores table...Skipping (already created)'
	End
Else
	Begin
		Create Table dbo.Stores (
			StoreId int not null
				Identity(1, 1)
				Constraint PK_Stores
				Primary Key

			, StoreName nvarchar(50) null

			, Address nvarchar(50) not null
			, City nvarchar(50) not null
			, Province nvarchar(50) not null
			, Country nvarchar(50) not null
			, PostalCode varchar(20) not null

			, PhoneNumber varchar(20) not null
			
		)

		Print 'Create dbo.Stores table...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create dbo.Stores table...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create dbo.Stores table' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO



/* Create dbo.Orders table */
Print ''
Print 'Create dbo.Orders table'
GO

If Exists (
	Select 1 
	From sys.tables tbl 
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Orders')
)
	Begin
		Print 'Create dbo.Orders table...Skipping (already created)'
	End
Else
	Begin
		Create Table dbo.Orders (
			OrderId int not null
				Identity(1, 1)
				Constraint PK_Orders
				Primary Key
			, StoreId int not null
				Constraint FK_Orders_Store
				Foreign Key References dbo.Stores(StoreId)
			, CustomerId int null
				Constraint FK_Orders_Customer
				Foreign Key References dbo.Customers(CustomerId)
			, ShippingAddress nvarchar(100) null
			, ShippingCity nvarchar(50) null
			, ShippingProvince nvarchar(50) null
			, ShippingCountry nvarchar(50) null
			, ShippingPostalCode varchar(20) null
			, CustomerPhoneNumber varchar(20) null

		)

		Print 'Create dbo.Orders table...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create dbo.Orders table...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create dbo.Orders table' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create dbo.OrderLineItems table */
Print ''
Print 'Create dbo.OrderLineItems table'
GO

If Exists (
	Select 1 
	From sys.tables tbl 
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'OrderLineItems')
)
	Begin
		Print 'Create dbo.OrderLineItems table...Skipping (already created)'
	End
Else
	Begin
		Create Table dbo.OrderLineItems (
			LineItemId int not null
				Constraint PK_OrderLineItems
				Primary Key
			, OrderId int not null
				Constraint FK_OrderLineItems_Order
				Foreign Key References dbo.Orders(OrderId)
			, ProductId int not null
				Constraint FK_OrderLineItems_Product
				Foreign Key References dbo.Products(ProductId)
			, Price decimal not null
		)

		Print 'Create dbo.OrderLineItems table...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create dbo.OrderLineItems table...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create dbo.OrderLineItems table' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/****************************************************************************************************
*************************************  End of script  ***********************************************
****************************************************************************************************/

/* Record the execution of this script (After state) in the DBHistory table */
Set NOCOUNT ON

Update dbo.DBHistory
Set AfterVersion = DBVersion.Version
	, AfterSuspect = DBVersion.IsSuspect
From dbo.DBHistory, dbo.DBVersion
Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory))
	And (DBHistory.DBProduct = DBVersion.DBProduct)

Set NOCOUNT OFF
GO

/* Turn off noexec */
--If any errors occurred the script would have been set to noexec mode, which doesn't run the script.
--This must be set back to normal mode at the end of this script.
Print ''
set noexec off
set nocount off
GO

/* Finished message */
Print 'Running script:  Create Retail 1.0...Done'
Print ''
GO

