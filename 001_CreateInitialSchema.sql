-- File: DatabaseScripts/001_CreateInitialSchema.sql
-- This script is intended to be run by your C# DatabaseMigrator
-- on a new or empty database.

PRINT 'Starting script 001_CreateInitialSchema.sql...';
GO

-- ====================================================================================
-- SECTION 1: CREATE SchemaVersions TABLE
-- ====================================================================================
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='SchemaVersions' AND xtype='U')
BEGIN
    CREATE TABLE dbo.SchemaVersions (
        Id INT IDENTITY(1,1) PRIMARY KEY NOT NULL,
        ScriptName NVARCHAR(255) NOT NULL UNIQUE,
        AppliedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
    PRINT 'Table [dbo].[SchemaVersions] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[SchemaVersions] already exists.';
END
GO

-- ====================================================================================
-- SECTION 2: DEFINE YOUR APPLICATION TABLES AND CONSTRAINTS
-- ====================================================================================

PRINT 'STEP 2: Starting table creations...';
GO

-- User Profile Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='UserProfile' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[UserProfile] (
        [UserId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [Name] NVARCHAR(100) NOT NULL,
        [PhoneNumber] VARCHAR(15) NOT NULL UNIQUE,
        [Email] NVARCHAR(150) NULL UNIQUE,
        [ProfileImageUrl] NVARCHAR(MAX) NULL,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_UserProfile] PRIMARY KEY CLUSTERED ([UserId])
    );
    PRINT 'Table [dbo].[UserProfile] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[UserProfile] already exists.';
END
GO

-- Otp Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Otp' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Otp] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [PhoneNumber] VARCHAR(15) NOT NULL,
        [OtpCode] VARCHAR(10) NOT NULL,
        [IsVerified] BIT NOT NULL DEFAULT 0,
        [ExpiryDate] DATETIME NOT NULL,
        [AttemptCount] INT NOT NULL DEFAULT 0,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_Otp] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[Otp] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Otp] already exists.';
END
GO

-- Category Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Category' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Category] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [Name] NVARCHAR(100) NOT NULL,
        [Description] NVARCHAR(500) NULL,
        [ImageUrl] NVARCHAR(MAX) NULL,
        [ParentCategoryId] UNIQUEIDENTIFIER NULL,
        [SortOrder] INT NULL,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_Category] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[Category] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Category] already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Category_ParentCategory' AND parent_object_id = OBJECT_ID('dbo.Category'))
BEGIN
    ALTER TABLE [dbo].[Category] ADD CONSTRAINT [FK_Category_ParentCategory] FOREIGN KEY ([ParentCategoryId]) REFERENCES [dbo].[Category]([Id]);
    PRINT 'Constraint FK_Category_ParentCategory added.';
END
ELSE
BEGIN
    PRINT 'Constraint FK_Category_ParentCategory already exists.';
END
GO


-- OrderStatus Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderStatus' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[OrderStatus] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [StatusName] NVARCHAR(50) NOT NULL UNIQUE,
        [Description] NVARCHAR(255) NULL,
        [SortOrder] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_OrderStatus] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[OrderStatus] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[OrderStatus] already exists.';
END
GO

-- PaymentMethod Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='PaymentMethod' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[PaymentMethod] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [Name] NVARCHAR(100) NOT NULL UNIQUE,
        [Description] NVARCHAR(255) NULL,
        [IconUrl] NVARCHAR(MAX) NULL,
        [IsEnabled] BIT NOT NULL DEFAULT 1,
        [SortOrder] INT NULL,
        [ConfigurationJson] NVARCHAR(MAX) NULL,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_PaymentMethod] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[PaymentMethod] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[PaymentMethod] already exists.';
END
GO

-- Product Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Product' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Product] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [Name] NVARCHAR(255) NOT NULL,
        [Description] NVARCHAR(MAX) NULL,
        [Price] DECIMAL(18, 2) NOT NULL,
        [SKU] VARCHAR(100) NULL UNIQUE,
        [StockQuantity] INT NOT NULL DEFAULT 0,
        [ImageUrl1] NVARCHAR(MAX) NULL,
        [ImageUrl2] NVARCHAR(MAX) NULL,
        [ImageUrl3] NVARCHAR(MAX) NULL,
        [CategoryId] UNIQUEIDENTIFIER NULL,
        [IsFeatured] BIT NOT NULL DEFAULT 0,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_Product] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[Product] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Product] already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Product_Category' AND parent_object_id = OBJECT_ID('dbo.Product'))
BEGIN
    ALTER TABLE [dbo].[Product] ADD CONSTRAINT [FK_Product_Category] FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Category]([Id]);
    PRINT 'Constraint FK_Product_Category added.';
END
ELSE
BEGIN
    PRINT 'Constraint FK_Product_Category already exists.';
END
GO

-- Address Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Address' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Address] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [FullName] NVARCHAR(150) NOT NULL,
        [PhoneNumber] VARCHAR(15) NOT NULL,
        [AddressLine1] NVARCHAR(255) NOT NULL,
        [AddressLine2] NVARCHAR(255) NULL,
        [City] NVARCHAR(100) NOT NULL,
        [State] NVARCHAR(100) NOT NULL,
        [PostalCode] VARCHAR(20) NOT NULL,
        [Country] NVARCHAR(100) NOT NULL DEFAULT 'India',
        [AddressType] VARCHAR(50) NULL,
        [IsDefaultShipping] BIT NOT NULL DEFAULT 0,
        [IsDefaultBilling] BIT NOT NULL DEFAULT 0,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[Address] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Address] already exists.';
END
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Address_UserProfile' AND parent_object_id = OBJECT_ID('dbo.Address'))
BEGIN
    ALTER TABLE [dbo].[Address] ADD CONSTRAINT [FK_Address_UserProfile] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserProfile]([UserId]);
    PRINT 'Constraint FK_Address_UserProfile added.';
END
ELSE
BEGIN
    PRINT 'Constraint FK_Address_UserProfile already exists.';
END
GO

-- Order Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Order' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Order] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [OrderNumber] VARCHAR(50) NOT NULL UNIQUE,
        [OrderDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ShippingAddressId] UNIQUEIDENTIFIER NOT NULL,
        [BillingAddressId] UNIQUEIDENTIFIER NULL,
        [SubTotalAmount] DECIMAL(18, 2) NOT NULL,
        [DeliveryCharge] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [DiscountAmount] DECIMAL(18, 2) NOT NULL DEFAULT 0,
        [TotalAmount] DECIMAL(18, 2) NOT NULL,
        [OrderStatusId] UNIQUEIDENTIFIER NOT NULL,
        [PaymentMethodId] UNIQUEIDENTIFIER NULL,
        [PaymentTransactionIdExternal] VARCHAR(100) NULL,
        [Notes] NVARCHAR(MAX) NULL,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_Order] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[Order] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Order] already exists.';
END
GO

-- Add FKs for Order table
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Order_UserProfile' AND parent_object_id = OBJECT_ID('dbo.[Order]'))
BEGIN
    ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_UserProfile] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserProfile]([UserId]);
    PRINT 'Constraint FK_Order_UserProfile added.';
END ELSE BEGIN PRINT 'Constraint FK_Order_UserProfile already exists.'; END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Order_ShippingAddress' AND parent_object_id = OBJECT_ID('dbo.[Order]'))
BEGIN
    ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_ShippingAddress] FOREIGN KEY ([ShippingAddressId]) REFERENCES [dbo].[Address]([Id]);
    PRINT 'Constraint FK_Order_ShippingAddress added.';
END ELSE BEGIN PRINT 'Constraint FK_Order_ShippingAddress already exists.'; END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Order_BillingAddress' AND parent_object_id = OBJECT_ID('dbo.[Order]'))
BEGIN
    ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_BillingAddress] FOREIGN KEY ([BillingAddressId]) REFERENCES [dbo].[Address]([Id]);
    PRINT 'Constraint FK_Order_BillingAddress added.';
END ELSE BEGIN PRINT 'Constraint FK_Order_BillingAddress already exists.'; END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Order_OrderStatus' AND parent_object_id = OBJECT_ID('dbo.[Order]'))
BEGIN
    ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_OrderStatus] FOREIGN KEY ([OrderStatusId]) REFERENCES [dbo].[OrderStatus]([Id]);
    PRINT 'Constraint FK_Order_OrderStatus added.';
END ELSE BEGIN PRINT 'Constraint FK_Order_OrderStatus already exists.'; END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Order_PaymentMethod' AND parent_object_id = OBJECT_ID('dbo.[Order]'))
BEGIN
    ALTER TABLE [dbo].[Order] ADD CONSTRAINT [FK_Order_PaymentMethod] FOREIGN KEY ([PaymentMethodId]) REFERENCES [dbo].[PaymentMethod]([Id]);
    PRINT 'Constraint FK_Order_PaymentMethod added.';
END ELSE BEGIN PRINT 'Constraint FK_Order_PaymentMethod already exists.'; END
GO


-- OrderItem Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='OrderItem' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[OrderItem] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [OrderId] UNIQUEIDENTIFIER NOT NULL,
        [ProductId] UNIQUEIDENTIFIER NOT NULL,
        [ProductName] NVARCHAR(255) NOT NULL,
        [Quantity] INT NOT NULL,
        [UnitPrice] DECIMAL(18, 2) NOT NULL,
        [TotalPrice] DECIMAL(18, 2) NOT NULL,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_OrderItem] PRIMARY KEY CLUSTERED ([Id])
    );
    PRINT 'Table [dbo].[OrderItem] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[OrderItem] already exists.';
END
GO
-- Add FKs for OrderItem table
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItem_Order' AND parent_object_id = OBJECT_ID('dbo.OrderItem'))
BEGIN
    ALTER TABLE [dbo].[OrderItem] ADD CONSTRAINT [FK_OrderItem_Order] FOREIGN KEY ([OrderId]) REFERENCES [dbo].[Order]([Id]);
    PRINT 'Constraint FK_OrderItem_Order added.';
END ELSE BEGIN PRINT 'Constraint FK_OrderItem_Order already exists.'; END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderItem_Product' AND parent_object_id = OBJECT_ID('dbo.OrderItem'))
BEGIN
    ALTER TABLE [dbo].[OrderItem] ADD CONSTRAINT [FK_OrderItem_Product] FOREIGN KEY ([ProductId]) REFERENCES [dbo].[Product]([Id]);
    PRINT 'Constraint FK_OrderItem_Product added.';
END ELSE BEGIN PRINT 'Constraint FK_OrderItem_Product already exists.'; END
GO

-- Cart Table
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Cart' AND xtype='U')
BEGIN
    CREATE TABLE [dbo].[Cart] (
        [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
        [UserId] UNIQUEIDENTIFIER NOT NULL,
        [StatusId] INT NULL,
        [CreatedBy] VARCHAR(100) NULL,
        [CreatedDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [ModifiedBy] VARCHAR(100) NULL,
        [ModifiedDate] DATETIME NULL,
        CONSTRAINT [PK_Cart] PRIMARY KEY CLUSTERED ([Id]),
        CONSTRAINT [UQ_Cart_UserId] UNIQUE ([UserId]) -- Unique constraint
    );
    PRINT 'Table [dbo].[Cart] created.';
END
ELSE
BEGIN
    PRINT 'Table [dbo].[Cart] already exists.';
END
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cart_UserProfile' AND parent_object_id = OBJECT_ID('dbo.Cart'))
BEGIN
    ALTER TABLE [dbo].[Cart] ADD CONSTRAINT [FK_Cart_UserProfile] FOREIGN KEY ([UserId]) REFERENCES [dbo].[UserProfile]([UserId]);
    PRINT 'Constraint FK_Cart_UserProfile added.';
END ELSE BEGIN PRINT 'Constraint FK_Cart_UserProfile already exists.'; END
GO

PRINT 'ALL APPLICATION TABLES AND CONSTRAINTS SHOULD BE DEFINED ABOVE THIS LINE.';
GO

-- ====================================================================================
-- SECTION 3: REGISTER THIS SCRIPT AS APPLIED
-- ====================================================================================
IF EXISTS (SELECT * FROM sysobjects WHERE name='SchemaVersions' AND xtype='U')
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.SchemaVersions WHERE ScriptName = '001_CreateInitialSchema.sql')
    BEGIN
        INSERT INTO dbo.SchemaVersions (ScriptName, AppliedDate) VALUES ('001_CreateInitialSchema.sql', GETDATE());
        PRINT 'Script 001_CreateInitialSchema.sql registered in SchemaVersions.';
    END
    ELSE
    BEGIN
        PRINT 'Script 001_CreateInitialSchema.sql already registered in SchemaVersions.';
    END
END
ELSE
BEGIN
    PRINT 'CRITICAL ERROR: SchemaVersions table not found. Cannot register script.';
    RAISERROR('SchemaVersions table not found. Halting script registration.', 16, 1); -- Raise an error to stop execution
END
GO

PRINT 'Finished script 001_CreateInitialSchema.sql.';
GO