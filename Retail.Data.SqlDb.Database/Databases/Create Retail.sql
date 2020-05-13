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
Insert Into dbo.DBVersion (DBProduct, Version, Notes)
Values ('Retail', '1.0', @@SERVERNAME + '.' + DB_NAME())
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

/* Upgrade Retail 1.01
** Prepared: May 9, 2020 (TK)
**
** Active Flags
** Customers.MembershipNumber
** Customers.Discount
** Products.SalesPrice
** Add decimals to Price columns (Products & OrderLineItems)
** OrderLineItems.Quantity, UnitPrice and TotalPrice
** Inventory
*/
/* Modifications
rev2 - added Orders.TotalPrice
*/

Print 'Running script:  Upgrade Retail 1.01'
GO

/* Prerequisite Checks */

--Permission Check
If Not Exists(SELECT permission_name FROM fn_my_permissions(DB_NAME(), 'DATABASE') Where (permission_name In ('CREATE TABLE')))
	Begin
	RAISERROR ('Prerequisite Error: Insufficient Privileges.  Requires permissions to create and modify tables.  Check that this script is being run by the correct login.', 17, 1) 
	set nocount on
	set noexec on
	End
GO

--Check that the DBProduct is at the correct version to receive this script
Declare @DBProduct varchar(50)
Declare @MajorVersionNumber varchar(4)
Declare @MinorVersionBefore varchar(20)
Declare @MinorVersionAfter varchar(20)

Set @DBProduct = 'Retail'
Set @MajorVersionNumber = '1'
Set @MinorVersionBefore = '0'
Set @MinorVersionAfter  = '01'

If Not Exists (Select 1 From sys.tables Where (name = 'DBVersion'))
	Begin
	RAISERROR ('Prerequisite Error: No Process Solutions DB products are installed. Upgrade could not be applied.  Check that the current database is set.', 17, 1) 
	set nocount on
	set noexec on
	End
If Exists (Select 1 From dbo.DBVersion Where (DBProduct = @DBProduct) And (IsSuspect = 1))
	Begin
	RAISERROR ('Prerequisite Error: This database is marked as suspect.  Previous errors must be investigated and the integrity of the database verified.  Contact Process Solutions for assistance repairing this database.', 17, 1) 
	set nocount on
	set noexec on
	End
If Exists (
	Select Version 
	From dbo.DBVersion 
	Where (DBProduct = @DBProduct) And ((Cast(PARSENAME(Version ,2) as int) = Cast(@MajorVersionNumber as int) )) And ((Cast(PARSENAME(Version ,1 ) as int) >= Cast(@MinorVersionAfter as int) ))
)
	Begin
	Print '                 Upgrade ' + @DBProduct + ' ' + @MajorVersionNumber +'.' + @MinorVersionAfter + '...Skipping (already applied)'
	set nocount on
	set noexec on
	End
If Not Exists (Select 1 From dbo.DBVersion Where (DBProduct = @DBProduct) And ((Cast(PARSENAME(Version ,2) as int) = Cast(@MajorVersionNumber as int) )) And  ((Cast(PARSENAME(Version ,1 ) as int) = Cast(@MinorVersionBefore as int))))
	Begin
	Declare @v varchar(200)
	Select @v = DBProduct + ' ' + Version
	From dbo.DBVersion Where (DBProduct = @DBProduct)
	Print '  The current database version is:  ' + IsNull(@v, 'not found/installed')
	Print '  The expected database version is: ' + @DBProduct + ' ' + @MajorVersionNumber + '.' + @MinorVersionBefore

	RAISERROR ('Prerequisite Error: Version is not appropriate to receive this upgrade.  See above for version information.', 17, 1) 

	set nocount on
	set noexec on
	End
GO

/* Create DBHistory entry */
Set NOCOUNT ON

Insert Into dbo.DBHistory (
	ScriptName, Notes
	, DBProduct, BeforeVersion, BeforeSuspect
)
Select 'Upgrade Retail 1.01 (rev2)', 'Active Flags, Customers.MembershipNumber & discount, Products.SalesPrice, Inventory (T#1234)'
	, DBProduct, Version, IsSuspect
From dbo.DBVersion
Where (DBProduct = 'Retail')
GO
Set NOCOUNT OFF
GO

/****************************************************************************************************
************************************  Start of script  **********************************************
****************************************************************************************************/


/* Create Column dbo.Customers.Active */
Print ''
Print 'Create Column dbo.Customers.Active'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Customers') And (col.name = 'Active')
)
	Begin
	Print 'Create Column dbo.Customers.Active...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.Customers
	Add Active bit not null
		Constraint DF_Customers_Active
		Default (1)

	Print 'Create Column dbo.Customers.Active...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.Customers.Active...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.Customers.Active' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Column dbo.Products.Active */
Print ''
Print 'Create Column dbo.Products.Active'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Products') And (col.name = 'Active')
)
	Begin
	Print 'Create Column dbo.Products.Active...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.Products
	Add Active bit not null
		Constraint DF_Products_Active
		Default (1)

	Print 'Create Column dbo.Products.Active...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.Products.Active...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.Products.Active' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Column dbo.Stores.Active */
Print ''
Print 'Create Column dbo.Stores.Active'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Stores') And (col.name = 'Active')
)
	Begin
	Print 'Create Column dbo.Stores.Active...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.Stores
	Add Active bit not null
		Constraint DF_Stores_Active
		Default (1)

	Print 'Create Column dbo.Stores.Active...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.Stores.Active...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.Stores.Active' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Column dbo.Customers.MembershipNumber */
Print ''
Print 'Create Column dbo.Customers.MembershipNumber'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Customers') And (col.name = 'MembershipNumber')
)
	Begin
	Print 'Create Column dbo.Customers.MembershipNumber...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.Customers
	Add MembershipNumber uniqueidentifier not null
		Constraint DF_Customers_MembershipNumber
		Default NewId()

	Print 'Create Column dbo.Customers.MembershipNumber...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.Customers.MembershipNumber...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.Customers.MembershipNumber' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Index UQ_Customers_MembershipNumber */
Print ''
Print 'Create Index UQ_Customers_MembershipNumber'
GO

If Exists (
	Select 1 
	From sys.indexes idx
		Inner Join sys.tables tbl On (idx.object_id = tbl.object_id)
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.name = 'Customers') And (idx.name = 'UQ_Customers_MembershipNumber')
)
	Begin
		Print 'Create Index UQ_Customers_MembershipNumber...Skipping (already created)'
	End
Else
	Begin
		Create Unique Index UQ_Customers_MembershipNumber
		On dbo.Customers(MembershipNumber)

		Print 'Create Index UQ_Customers_MembershipNumber...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Index UQ_Customers_MembershipNumber...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Index UQ_Customers_MembershipNumber' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO

/* Create Column dbo.Customers.Discount */
Print ''
Print 'Create Column dbo.Customers.Discount'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Customers') And (col.name = 'Discount')
)
	Begin
	Print 'Create Column dbo.Customers.Discount...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.Customers
	Add Discount float not null
		Constraint DF_Customers_Discount
		Default 0.0
	, Constraint CK_Customers_Discount
		Check (Discount Between 0.0 And 1.0)

	Print 'Create Column dbo.Customers.Discount...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.Customers.Discount...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.Customers.Discount' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Column dbo.Products.SalesPrice */
Print ''
Print 'Create Column dbo.Products.SalesPrice'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Products') And (col.name = 'SalesPrice')
)
	Begin
	Print 'Create Column dbo.Products.SalesPrice...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.Products
	Add SalesPrice decimal(9,2) null

	Print 'Create Column dbo.Products.SalesPrice...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.Products.SalesPrice...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.Products.SalesPrice' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Alter dbo.Products.Price */
Print ''
Print 'Alter dbo.Products.Price'
GO

Alter Table dbo.Products
Alter Column Price decimal(9,2) null

Print 'Alter dbo.Products.Price...Done'
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Alter dbo.Products.Price...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Alter dbo.Products.Price' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Column dbo.OrderLineItems.Quantity */
Print ''
Print 'Create Column dbo.OrderLineItems.Quantity'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'OrderLineItem') And (col.name = 'Quantity')
)
	Begin
	Print 'Create Column dbo.OrderLineItems.Quantity...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.OrderLineItems
	Add Quantity int null
		, UnitPrice decimal(9,2) null
		, TotalPrice decimal(9,2) null

	Print 'Create Column dbo.OrderLineItems.Quantity...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.OrderLineItems.Quantity...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.OrderLineItems.Quantity' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Populate Quantity and Prices */
Print ''
Print 'Populate Quantity and Prices'
GO

Update dbo.OrderLineItems
Set Quantity = 1, UnitPrice = Price, TotalPrice = Price

Print 'Populate Quantity and Prices...Done'
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Populate Quantity and Prices...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Populate Quantity and Prices' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO

/* Alter dbo.OrderLineItems.Prices not null */
Print ''
Print 'Alter dbo.OrderLineItems.Price'
GO

Alter Table dbo.OrderLineItems
Alter Column Quantity int not null

Alter Table dbo.OrderLineItems
Alter Column UnitPrice decimal(9,2) not null

Alter Table dbo.OrderLineItems
Alter Column TotalPrice decimal(9,2) not null

Print 'Alter dbo.OrderLineItems.Price...Done'
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Alter dbo.OrderLineItems.Price...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Alter dbo.OrderLineItems.Price' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Drop dbo.OrderLineItems.Price */
Print ''
Print 'Drop dbo.OrderLineItems.Price'
GO

If Not Exists (
	Select 1 
	From sys.tables tbl 
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'OrderLineItems') And (col.name = 'Price')
)
	Begin
		Print 'Drop dbo.OrderLineItems.Price...Skipping (already dropped)'
	End
Else
	Begin
		Alter Table dbo.OrderLineItems
		Drop Column Price

		Print 'Drop dbo.OrderLineItems.Price...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Drop dbo.OrderLineItems.Price...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Drop dbo.OrderLineItems.Price' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Column dbo.Orders.TotalPrice */
Print ''
Print 'Create Column dbo.Orders.TotalPrice'
GO

If Exists (
	Select 1 
	From sys.schemas sch
		Inner Join sys.tables tbl On (sch.schema_id = tbl.schema_id)
		Inner Join sys.columns col On (tbl.object_id = col.object_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Orders') And (col.name = 'TotalPrice')
)
	Begin
	Print 'Create Column dbo.Orders.TotalPrice...Skipping (already added)'
	End
Else
	Begin
	Alter Table dbo.Orders
	Add TotalPrice decimal(9,2) null

	Print 'Create Column dbo.Orders.TotalPrice...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Column dbo.Orders.TotalPrice...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Column dbo.Orders.TotalPrice' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Calculate Order Price */
Print ''
Print 'Calculate Order Price'
GO

Update bill
Set TotalPrice = lineItem.TotalPrice
From dbo.Orders bill
	Left Join (Select lineItem.OrderId, IsNull(Sum(lineItem.TotalPrice), 0) As TotalPrice From dbo.OrderLineItems lineItem Group By lineItem.OrderId) lineItem On (bill.OrderId = lineItem.OrderId)

Print 'Calculate Order Price...Done'
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Calculate Order Price...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Calculate Order Price' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Alter dbo.Orders.TotalPrice */
Print ''
Print 'Alter dbo.Orders.TotalPrice'
GO

Alter Table dbo.Orders
Alter Column TotalPrice decimal(9,2) not null

Print 'Alter dbo.Orders.TotalPrice...Done'
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Alter dbo.Orders.TotalPrice...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Alter dbo.Orders.TotalPrice' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create dbo.Inventory table */
Print ''
Print 'Create dbo.Inventory table'
GO

If Exists (
	Select 1 
	From sys.tables tbl 
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.Name = 'Inventory')
)
	Begin
		Print 'Create dbo.Inventory table...Skipping (already created)'
	End
Else
	Begin
		Create Table dbo.Inventory (
			StoreId int not null
			, ProductId int not null
			, Quantity int not null
			, Constraint PK_Inventory
				Primary Key(StoreId, ProductId)			
		)

		Print 'Create dbo.Inventory table...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create dbo.Inventory table...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create dbo.Inventory table' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO


/* Create Index IX_Inventory_ProductId */
Print ''
Print 'Create Index IX_Inventory_ProductId'
GO

If Exists (
	Select 1 
	From sys.indexes idx
		Inner Join sys.tables tbl On (idx.object_id = tbl.object_id)
		Inner Join sys.schemas sch On (tbl.schema_id = sch.schema_id)
	Where (sch.name = 'dbo') And (tbl.name = 'Inventory') And (idx.name = 'IX_Inventory_ProductId')
)
	Begin
		Print 'Create Index IX_Inventory_ProductId...Skipping (already created)'
	End
Else
	Begin
		Create Index IX_Inventory_ProductId
		On dbo.Inventory(ProductId)

		Print 'Create Index IX_Inventory_ProductId...Done'
	End
GO
if (@@error <> 0)
	Begin
	While (@@TRANCOUNT > 0) Rollback Transaction
	Print 'Create Index IX_Inventory_ProductId...ERRORS OCCURRED - Marking DB as suspect'
	Update dbo.DBVersion Set IsSuspect = 1, SuspectTimeUtc = GETUTCDATE() Where (DBProduct = 'Retail')
	Update dbo.DBHistory Set AfterSuspect = 1, Notes = 'Error during Create Index IX_Inventory_ProductId' Where (ExecutionID In (Select Max(ExecutionID) From dbo.DBHistory)) And (DBProduct = 'Retail')
	set nocount on
	set noexec on
	End
GO




/****************************************************************************************************
*************************************  End of script  ***********************************************
****************************************************************************************************/

-- Update Database Version
Print ''
Print 'Updating database version'
GO
Update dbo.DBVersion
Set Version = '1.01'
Where (DBProduct = 'Retail')
	And (IsSuspect = 0)
GO
Print 'Updating database version...Done'
GO

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
Print 'Running script:  Upgrade Retail 1.01...Done'
Print ''
GO

