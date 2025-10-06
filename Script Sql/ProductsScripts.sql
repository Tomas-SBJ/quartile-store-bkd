IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'quartile')
BEGIN
    EXEC('CREATE SCHEMA quartile');
END
GO

CREATE TABLE quartile.products (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    code INT NOT NULL,
    name NVARCHAR(250) NOT NULL,
    description NVARCHAR(250) NOT NULL,
    price DECIMAL(18, 2) NOT NULL,
    store_id UNIQUEIDENTIFIER NOT NULL,
    
    CONSTRAINT fk_products_to_stores FOREIGN KEY (store_id) REFERENCES quartile.stores(id) ON DELETE NO ACTION,
    CONSTRAINT uq_products_code_per_store UNIQUE (store_id, code)
);
GO

CREATE OR ALTER FUNCTION quartile.udf_get_products_as_json_by_store_id 
    (@store_id UNIQUEIDENTIFIER)
RETURNS NVARCHAR(MAX)
AS
BEGIN
    RETURN (
        SELECT 
            p.id,
            p.code,
            p.name,
            p.description,
            p.price
        FROM 
            quartile.products AS p
        WHERE 
            p.store_id = @store_id
        FOR JSON PATH
    );
END
GO

CREATE OR ALTER PROCEDURE quartile.usp_insert_product
    @code INT,
    @name NVARCHAR(250),
    @description NVARCHAR(250),
    @price DECIMAL(18, 2),
    @store_id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO quartile.products (code, name, description, price, store_id)
    VALUES (@code, @name, @description, @price, @store_id);
END
GO