-- Вставка данных в таблицу пользователей
-- Пароль для admin: admin123 (хешированный)
INSERT INTO Users (Username, PasswordHash, FullName, Email, IsActive) 
VALUES ('admin', '0DPiKGHhf8TALvV04232PsZ3Q4OtPmqf2d7zNIkWXFo=', 'Администратор', 'admin@xxasklad.local', TRUE);

INSERT INTO Users (Username, PasswordHash, FullName, Email, IsActive) 
VALUES ('user1', 'gSvqqUNQzSkOhpkATtViFuMxzxNHnWdHM/gM4VjJlHs=', 'Иван Иванов', 'ivan@xxasklad.local', TRUE);

-- Вставка категорий товаров
INSERT INTO Categories (CategoryName, Description, IsActive) 
VALUES 
('Электроника', 'Электронные товары и приборы', TRUE),
('Одежда', 'Одежда и обувь', TRUE),
('Продукты', 'Продукты питания', TRUE),
('Мебель', 'Мебель и предметы интерьера', TRUE),
('Инструменты', 'Ручные и электрические инструменты', TRUE);

-- Вставка образцов товаров
INSERT INTO Products (ProductName, ProductCode, CategoryId, Description, UnitOfMeasure, CostPrice, SellingPrice, IsActive) 
VALUES 
('Смартфон Samsung A12', 'PROD-001', 1, 'Смартфон Samsung Galaxy A12', 'шт', 100.00, 150.00, TRUE),
('Ноутбук ASUS VivoBook', 'PROD-002', 1, 'Ноутбук ASUS VivoBook 15', 'шт', 400.00, 550.00, TRUE),
('Футболка хлопок', 'PROD-003', 2, 'Футболка мужская из хлопка', 'шт', 5.00, 15.00, TRUE),
('Джинсы синие', 'PROD-004', 2, 'Джинсы синие классические', 'шт', 15.00, 40.00, TRUE),
('Масло подсолнечное', 'PROD-005', 3, 'Масло подсолнечное рафинированное 1л', 'л', 1.50, 3.00, TRUE),
('Хлеб белый', 'PROD-006', 3, 'Хлеб белый свежий', 'шт', 0.50, 1.20, TRUE),
('Стол обеденный', 'PROD-007', 4, 'Стол обеденный деревянный', 'шт', 80.00, 150.00, TRUE),
('Стул офисный', 'PROD-008', 4, 'Стул офисный с регулировкой высоты', 'шт', 50.00, 100.00, TRUE),
('Дрель электрическая', 'PROD-009', 5, 'Дрель электрическая Bosch', 'шт', 60.00, 120.00, TRUE),
('Отвертка набор', 'PROD-010', 5, 'Набор отверток из 10 шт', 'набор', 10.00, 25.00, TRUE);

-- Вставка остатков на складе
INSERT INTO Stock (ProductId, Quantity, MinimumLevel, MaximumLevel) 
VALUES 
(1, 50, 10, 200),
(2, 20, 5, 50),
(3, 100, 20, 500),
(4, 75, 20, 300),
(5, 200, 50, 1000),
(6, 150, 30, 500),
(7, 15, 5, 50),
(8, 25, 10, 100),
(9, 10, 5, 30),
(10, 30, 10, 100);

-- Вставка образцов клиентов
INSERT INTO Clients (ClientName, PhoneNumber, Email, Address, City, PostalCode, ContactPerson, IsActive) 
VALUES 
('ООО Торговый центр', '+7 (123) 456-7890', 'contact@tc.local', 'ул. Ленина, д. 10', 'Москва', '100000', 'Петров И.И.', TRUE),
('ИП Сергеев Сергей', '+7 (987) 654-3210', 'sergey@shop.local', 'ул. Пушкина, д. 5', 'Санкт-Петербург', '200000', 'Сергеев С.С.', TRUE),
('АО Розница', '+7 (456) 789-0123', 'info@roznica.local', 'пр. Проспект, д. 20', 'Казань', '300000', 'Иванов И.И.', TRUE),
('ООО Супермаркет', '+7 (234) 567-8901', 'manager@super.local', 'ул. Советская, д. 15', 'Новосибирск', '400000', 'Сидоров А.А.', TRUE),
('ИП Косметика', '+7 (789) 012-3456', 'sales@cosmo.local', 'ул. Красная, д. 7', 'Екатеринбург', '500000', 'Смирнова М.В.', TRUE);

-- Вставка примеров прихода товаров
INSERT INTO Income (IncomeNumber, IncomeDate, ProductId, Quantity, UnitPrice, TotalAmount, SupplierName, Notes, UserId) 
VALUES 
('INC-2024-001', '2024-01-05', 1, 20, 100.00, 2000.00, 'Samsung Distribution', 'Партия смартфонов', 1),
('INC-2024-002', '2024-01-10', 3, 50, 5.00, 250.00, 'Textile Factory', 'Партия футболок', 1),
('INC-2024-003', '2024-01-15', 5, 100, 1.50, 150.00, 'Oil Supplier', 'Масло подсолнечное', 1),
('INC-2024-004', '2024-01-20', 7, 10, 80.00, 800.00, 'Furniture Store', 'Столы обеденные', 2);

-- Вставка примеров расхода товаров
INSERT INTO Expense (ExpenseNumber, ExpenseDate, ClientId, ProductId, Quantity, UnitPrice, TotalAmount, Notes, UserId) 
VALUES 
('EXP-2024-001', '2024-01-08', 1, 1, 5, 150.00, 750.00, 'Продажа для торгового центра', 1),
('EXP-2024-002', '2024-01-12', 2, 3, 20, 15.00, 300.00, 'Поставка для ИП Сергеев', 1),
('EXP-2024-003', '2024-01-18', 3, 5, 30, 3.00, 90.00, 'Продажа розице', 2),
('EXP-2024-004', '2024-01-22', 4, 6, 50, 1.20, 60.00, 'Поставка для супермаркета', 1);

-- Вставка истории операций
INSERT INTO OperationHistory (UserId, OperationType, TableName, RecordId, OldValues, NewValues, OperationDate) 
VALUES 
(1, 'CREATE', 'Products', 1, NULL, 'ProductName=Смартфон Samsung A12', NOW()),
(1, 'CREATE', 'Income', 1, NULL, 'IncomeNumber=INC-2024-001, Quantity=20', NOW()),
(1, 'CREATE', 'Expense', 1, NULL, 'ExpenseNumber=EXP-2024-001, Quantity=5', NOW()),
(2, 'UPDATE', 'Clients', 1, 'ContactPerson=NULL', 'ContactPerson=Петров И.И.', NOW());
