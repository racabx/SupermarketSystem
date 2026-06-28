-- =====================================================
-- Local Supermarket Management System
-- Database Creation & Sample Data Script
-- CST2550 Reset Coursework 2026
-- =====================================================

-- Create and use the supermarket database
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'SupermarketDB')
    CREATE DATABASE SupermarketDB;
GO

USE SupermarketDB;
GO

-- =====================================================
-- DROP existing tables in reverse dependency order
-- =====================================================
IF OBJECT_ID('SaleItems', 'U') IS NOT NULL DROP TABLE SaleItems;
IF OBJECT_ID('Sales',     'U') IS NOT NULL DROP TABLE Sales;
IF OBJECT_ID('Products',  'U') IS NOT NULL DROP TABLE Products;
IF OBJECT_ID('Suppliers', 'U') IS NOT NULL DROP TABLE Suppliers;
IF OBJECT_ID('Categories','U') IS NOT NULL DROP TABLE Categories;
GO

-- =====================================================
-- CREATE TABLES
-- =====================================================

CREATE TABLE Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL
);

CREATE TABLE Suppliers (
    SupplierId   INT IDENTITY(1,1) PRIMARY KEY,
    Name         NVARCHAR(150) NOT NULL,
    ContactEmail NVARCHAR(150) NOT NULL,
    Phone        NVARCHAR(20)  NOT NULL
);

CREATE TABLE Products (
    ProductId         INT IDENTITY(1,1) PRIMARY KEY,
    Title             NVARCHAR(200)  NOT NULL,
    Barcode           NVARCHAR(50)   NOT NULL UNIQUE,
    Price             DECIMAL(18,2)  NOT NULL CHECK (Price > 0),
    QuantityInStock   INT            NOT NULL CHECK (QuantityInStock >= 0),
    LowStockThreshold INT            NOT NULL DEFAULT 5,
    ExpiryDate        DATETIME2      NOT NULL,
    CategoryId        INT            NOT NULL REFERENCES Categories(CategoryId),
    SupplierId        INT            NOT NULL REFERENCES Suppliers(SupplierId)
);

CREATE TABLE Sales (
    SaleId      INT IDENTITY(1,1) PRIMARY KEY,
    SaleDate    DATETIME2     NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(18,2) NOT NULL
);

CREATE TABLE SaleItems (
    SaleItemId INT IDENTITY(1,1) PRIMARY KEY,
    SaleId     INT           NOT NULL REFERENCES Sales(SaleId),
    ProductId  INT           NOT NULL REFERENCES Products(ProductId),
    Quantity   INT           NOT NULL CHECK (Quantity > 0),
    UnitPrice  DECIMAL(18,2) NOT NULL
);
GO

-- =====================================================
-- SAMPLE DATA
-- =====================================================

INSERT INTO Categories (Name) VALUES
    ('Dairy'), ('Bakery'), ('Beverages'),
    ('Snacks'), ('Fruits & Vegetables'),
    ('Meat & Fish'), ('Frozen Foods'), ('Household');

INSERT INTO Suppliers (Name, ContactEmail, Phone) VALUES
    ('FreshFoods Ltd',     'orders@freshfoods.com',         '01234567890'),
    ('DairyBest Co',       'supply@dairybest.com',          '01987654321'),
    ('BakerySupplies UK',  'info@bakerysupplies.co.uk',     '01765432198'),
    ('GlobalDrinks Inc',   'trade@globaldrinks.com',        '01654321987');

INSERT INTO Products
    (Title, Barcode, Price, QuantityInStock, LowStockThreshold, ExpiryDate, CategoryId, SupplierId)
VALUES
    ('Whole Milk 2L',        '5000112637922', 1.45, 40,  10, DATEADD(day,7,GETDATE()),   1, 2),
    ('Skimmed Milk 1L',      '5000112637923', 0.99, 3,   10, DATEADD(day,6,GETDATE()),   1, 2),
    ('Cheddar Cheese 400g',  '5010123456781', 2.85, 25,  5,  DATEADD(day,21,GETDATE()),  1, 2),
    ('Greek Yoghurt 500g',   '5010123456782', 1.65, 4,   8,  DATEADD(day,10,GETDATE()),  1, 2),
    ('Salted Butter 250g',   '5010123456783', 1.20, 18,  5,  DATEADD(day,30,GETDATE()),  1, 2),
    ('White Bread 800g',     '5010251001218', 1.10, 20,  8,  DATEADD(day,4,GETDATE()),   2, 3),
    ('Brown Bread 800g',     '5010251001219', 1.25, 2,   8,  DATEADD(day,4,GETDATE()),   2, 3),
    ('Croissants 4 Pack',    '5010251001220', 1.80, 12,  5,  DATEADD(day,3,GETDATE()),   2, 3),
    ('Orange Juice 1L',      '5449000000996', 1.35, 30,  10, DATEADD(day,14,GETDATE()),  3, 4),
    ('Sparkling Water 6pk',  '5449000000997', 2.50, 22,  6,  DATEADD(day,90,GETDATE()),  3, 4),
    ('Cola 2L',              '5449000000998', 1.75, 3,   8,  DATEADD(day,60,GETDATE()),  3, 4),
    ('Apple Juice 1L',       '5449000000999', 1.20, 17,  6,  DATEADD(day,20,GETDATE()),  3, 4),
    ('Ready Salted Crisps',  '5010251312342', 0.89, 45,  10, DATEADD(day,45,GETDATE()),  4, 1),
    ('Milk Chocolate Bar',   '5010251312343', 0.75, 60,  15, DATEADD(day,90,GETDATE()),  4, 1),
    ('Bananas Bunch',        '5010251312350', 0.65, 2,   10, DATEADD(day,5,GETDATE()),   5, 1),
    ('Broccoli 400g',        '5010251312351', 0.79, 15,  5,  DATEADD(day,6,GETDATE()),   5, 1),
    ('Cherry Tomatoes 250g', '5010251312352', 1.10, 20,  5,  DATEADD(day,7,GETDATE()),   5, 1),
    ('Chicken Breast 500g',  '5010251312360', 3.50, 4,   5,  DATEADD(day,3,GETDATE()),   6, 1),
    ('Salmon Fillet 300g',   '5010251312361', 4.25, 8,   4,  DATEADD(day,2,GETDATE()),   6, 1),
    ('Frozen Peas 900g',     '5010251312370', 1.45, 25,  6,  DATEADD(day,180,GETDATE()), 7, 1),
    ('Beef Burgers 4 Pack',  '5010251312371', 2.99, 3,   5,  DATEADD(day,90,GETDATE()),  7, 1),
    ('Washing Up Liquid',    '5010251312380', 1.05, 30,  8,  DATEADD(year,2,GETDATE()),  8, 1),
    ('Kitchen Roll 2 Pack',  '5010251312381', 1.89, 22,  6,  DATEADD(year,3,GETDATE()),  8, 1);

INSERT INTO Sales (SaleDate, TotalAmount) VALUES
    (DATEADD(day,-2, GETDATE()), 5.29),
    (DATEADD(day,-1, GETDATE()), 7.14),
    (GETDATE(),                  9.74);

INSERT INTO SaleItems (SaleId, ProductId, Quantity, UnitPrice) VALUES
    (1, 1,  2, 1.45),
    (1, 6,  1, 1.10),
    (1, 9,  1, 1.35),
    (2, 3,  1, 2.85),
    (2, 13, 2, 0.89),
    (2, 10, 1, 2.50),
    (3, 18, 1, 3.50),
    (3, 19, 1, 4.25),
    (3, 15, 3, 0.65);
GO