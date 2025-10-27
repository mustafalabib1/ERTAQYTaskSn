-- Use IF NOT EXISTS to make the script idempotent (runnable multiple times)
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ServiceProviders]') AND type in (N'U'))
BEGIN
    CREATE TABLE ServiceProviders (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Email NVARCHAR(256) NOT NULL UNIQUE,
        Phone NVARCHAR(20) NULL,
        Address NVARCHAR(200) NULL,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
BEGIN
    CREATE TABLE Products (
        Id INT PRIMARY KEY IDENTITY(1,1),
        Name NVARCHAR(100) NOT NULL,
        Price DECIMAL(18, 2) NOT NULL,
        CreationDate DATE NOT NULL DEFAULT GETDATE(),
        ServiceProviderId INT NOT NULL,
        -- Use ON DELETE CASCADE to automatically delete products when a service provider is deleted.
        -- This is more robust than handling it in a stored procedure.
        FOREIGN KEY (ServiceProviderId) REFERENCES ServiceProviders(Id) ON DELETE CASCADE
    );
END
GO

--------------------------------------------------
-- Stored Procedures for ServiceProviders Table
--------------------------------------------------

-- 1. Create ServiceProvider
-- Use CREATE OR ALTER for idempotency (requires SQL Server 2016 SP1+)
CREATE OR ALTER PROCEDURE sp_CreateServiceProvider
    @Name NVARCHAR(100),
    @Email NVARCHAR(256),
    @Phone NVARCHAR(20) = NULL,
    @Address NVARCHAR(200) = NULL
AS
BEGIN
    INSERT INTO ServiceProviders (Name, Email, Phone, Address)
    VALUES (@Name, @Email, @Phone, @Address);
END
GO

-- 2. Read ServiceProvider by ID
CREATE OR ALTER PROCEDURE sp_GetServiceProviderById
    @Id INT
AS
BEGIN
    SELECT * FROM ServiceProviders WHERE Id = @Id;
END
GO

-- 3. Read All ServiceProviders
CREATE OR ALTER PROCEDURE sp_GetAllServiceProviders
AS
BEGIN
    SELECT * FROM ServiceProviders;
END
GO

-- 4. Update ServiceProvider
CREATE OR ALTER PROCEDURE sp_UpdateServiceProvider
    @Id INT,
    @Name NVARCHAR(100),
    @Email NVARCHAR(256),
    @Phone NVARCHAR(20) = NULL,
    @Address NVARCHAR(200) = NULL
AS
BEGIN
    UPDATE ServiceProviders
    SET Name = @Name,
        Email = @Email,
        Phone = @Phone,
        Address = @Address
    WHERE Id = @Id;
END
GO

-- 5. Delete ServiceProvider
CREATE OR ALTER PROCEDURE sp_DeleteServiceProvider
    @Id INT
AS
BEGIN
    -- With ON DELETE CASCADE on the foreign key, we only need to delete the provider.
    -- The database will automatically handle deleting associated products.
    DELETE FROM ServiceProviders WHERE Id = @Id;
END
GO

--------------------------------------------------
-- Stored Procedures for Products Table (using CREATE OR ALTER)
--------------------------------------------------

-- 1. Create Product
CREATE OR ALTER PROCEDURE sp_CreateProduct
    @Name NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @ServiceProviderId INT,
    @CreationDate DATE = NULL
AS
BEGIN
    INSERT INTO Products (Name, Price, CreationDate, ServiceProviderId)
    VALUES (@Name, @Price, ISNULL(@CreationDate, CAST(GETDATE() AS DATE)), @ServiceProviderId);
END
GO

-- 2. Read Product by ID sp_GetProductsById
CREATE OR ALTER PROCEDURE sp_GetProductsById
    @Id INT
AS
BEGIN
    SELECT * FROM Products WHERE Id = @Id;
END
GO

-- 3. Read All Products
CREATE OR ALTER PROCEDURE sp_GetAllProducts
AS
BEGIN
    SELECT * FROM Products;
END
GO

-- 4. Update Product
CREATE OR ALTER PROCEDURE sp_UpdateProduct
    @Id INT,
    @Name NVARCHAR(100),
    @Price DECIMAL(18, 2),
    @CreationDate DATE,
    @ServiceProviderId INT
AS
BEGIN
    UPDATE Products
    SET Name = @Name,
        Price = @Price,
        CreationDate = @CreationDate,
        ServiceProviderId = @ServiceProviderId
    WHERE Id = @Id;
END
GO

-- 5. Delete Product
CREATE OR ALTER PROCEDURE sp_DeleteProduct
    @Id INT
AS
BEGIN
    DELETE FROM Products WHERE Id = @Id;
END
GO

-- 6. Read Products by ServiceProviderId (very common)
CREATE OR ALTER PROCEDURE sp_GetProductsByServiceProviderId
    @ServiceProviderId INT
AS
BEGIN
    SELECT * FROM Products WHERE ServiceProviderId = @ServiceProviderId;
END
GO

--------------------------------------------------
-- Seed Data (Optional)
--------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM ServiceProviders WHERE Email = 'contact@example.com')
BEGIN
    INSERT INTO ServiceProviders (Name, Email, Phone, Address)
    VALUES ('Example Corp', 'contact@example.com', '555-1234', '123 Example St');

    -- Get the ID of the newly inserted provider
    DECLARE @ProviderId INT = SCOPE_IDENTITY();

    -- Insert a product for this provider
    INSERT INTO Products (Name, Price, ServiceProviderId)
    VALUES ('Sample Product', 99.99, @ProviderId);
END