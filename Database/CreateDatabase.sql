-- Создание базы данных xxaSklad
CREATE DATABASE IF NOT EXISTS xxaSklad;
USE xxaSklad;

-- Таблица пользователей (Авторизация)
CREATE TABLE IF NOT EXISTS Users (
    UserId INT PRIMARY KEY AUTO_INCREMENT,
    Username VARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARCHAR(255) NOT NULL,
    FullName VARCHAR(100),
    Email VARCHAR(100),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    LastLoginDate DATETIME
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Таблица клиентов
CREATE TABLE IF NOT EXISTS Clients (
    ClientId INT PRIMARY KEY AUTO_INCREMENT,
    ClientName VARCHAR(100) NOT NULL,
    PhoneNumber VARCHAR(20),
    Email VARCHAR(100),
    Address VARCHAR(255),
    City VARCHAR(50),
    PostalCode VARCHAR(20),
    ContactPerson VARCHAR(100),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Таблица категорий товаров
CREATE TABLE IF NOT EXISTS Categories (
    CategoryId INT PRIMARY KEY AUTO_INCREMENT,
    CategoryName VARCHAR(100) NOT NULL UNIQUE,
    Description VARCHAR(255),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Таблица товаров
CREATE TABLE IF NOT EXISTS Products (
    ProductId INT PRIMARY KEY AUTO_INCREMENT,
    ProductName VARCHAR(150) NOT NULL,
    ProductCode VARCHAR(50) UNIQUE,
    CategoryId INT NOT NULL,
    Description VARCHAR(255),
    UnitOfMeasure VARCHAR(20),
    CostPrice DECIMAL(10, 2),
    SellingPrice DECIMAL(10, 2),
    IsActive BOOLEAN DEFAULT TRUE,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Таблица остатков на складе
CREATE TABLE IF NOT EXISTS Stock (
    StockId INT PRIMARY KEY AUTO_INCREMENT,
    ProductId INT NOT NULL UNIQUE,
    Quantity INT DEFAULT 0,
    MinimumLevel INT DEFAULT 10,
    MaximumLevel INT DEFAULT 1000,
    LastUpdated DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Таблица прихода товаров
CREATE TABLE IF NOT EXISTS Income (
    IncomeId INT PRIMARY KEY AUTO_INCREMENT,
    IncomeNumber VARCHAR(50) UNIQUE,
    IncomeDate DATE NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    TotalAmount DECIMAL(15, 2),
    SupplierName VARCHAR(100),
    Notes VARCHAR(255),
    UserId INT NOT NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    INDEX idx_income_date (IncomeDate),
    INDEX idx_product_id (ProductId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Таблица расхода товаров
CREATE TABLE IF NOT EXISTS Expense (
    ExpenseId INT PRIMARY KEY AUTO_INCREMENT,
    ExpenseNumber VARCHAR(50) UNIQUE,
    ExpenseDate DATE NOT NULL,
    ClientId INT NOT NULL,
    ProductId INT NOT NULL,
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10, 2) NOT NULL,
    TotalAmount DECIMAL(15, 2),
    Notes VARCHAR(255),
    UserId INT NOT NULL,
    CreatedDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (ClientId) REFERENCES Clients(ClientId),
    FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    INDEX idx_expense_date (ExpenseDate),
    INDEX idx_client_id (ClientId),
    INDEX idx_product_id (ProductId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Таблица истории операций
CREATE TABLE IF NOT EXISTS OperationHistory (
    HistoryId INT PRIMARY KEY AUTO_INCREMENT,
    UserId INT NOT NULL,
    OperationType VARCHAR(50) NOT NULL,
    TableName VARCHAR(50),
    RecordId INT,
    OldValues VARCHAR(500),
    NewValues VARCHAR(500),
    OperationDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    INDEX idx_operation_date (OperationDate),
    INDEX idx_user_id (UserId)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Индексы для производительности
CREATE INDEX idx_users_username ON Users(Username);
CREATE INDEX idx_clients_active ON Clients(IsActive);
CREATE INDEX idx_products_category ON Products(CategoryId);
CREATE INDEX idx_products_active ON Products(IsActive);
CREATE INDEX idx_stock_product ON Stock(ProductId);
