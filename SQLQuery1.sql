-- USE MASTER
-- GO
-- IF NOT EXISTS (
--     SELECT [name]
--     FROM sys.databases
--     WHERE [name] = N'BangazonWorkforce'
-- )
-- CREATE DATABASE BangazonWorkforce
-- GO
-- USE BangazonWorkforce
-- GO
-- DELETE FROM OrderProduct;
-- DELETE FROM ComputerEmployee;
-- DELETE FROM EmployeeTraining;
-- DELETE FROM Employee;
-- DELETE FROM TrainingProgram;
-- DELETE FROM Computer;
-- DELETE FROM Department;
-- DELETE FROM [Order];
-- DELETE FROM PaymentType;
-- DELETE FROM Product;
-- DELETE FROM ProductType;
-- DELETE FROM Customer;
-- ALTER TABLE Employee DROP CONSTRAINT [FK_EmployeeDepartment];
-- ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Employee];
-- ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Computer];
-- ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Employee];
-- ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Training];
-- ALTER TABLE Product DROP CONSTRAINT [FK_Product_ProductType];
-- ALTER TABLE Product DROP CONSTRAINT [FK_Product_Customer];
-- ALTER TABLE PaymentType DROP CONSTRAINT [FK_PaymentType_Customer];
-- ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Customer];
-- ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Payment];
-- ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Product];
-- ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Order];
-- DROP TABLE IF EXISTS OrderProduct;
-- DROP TABLE IF EXISTS ComputerEmployee;
-- DROP TABLE IF EXISTS EmployeeTraining;
-- DROP TABLE IF EXISTS Employee;
-- DROP TABLE IF EXISTS TrainingProgram;
-- DROP TABLE IF EXISTS Computer;
-- DROP TABLE IF EXISTS Department;
-- DROP TABLE IF EXISTS [Order];
-- DROP TABLE IF EXISTS PaymentType;
-- DROP TABLE IF EXISTS Product;
-- DROP TABLE IF EXISTS ProductType;
-- DROP TABLE IF EXISTS Customer;
CREATE TABLE Department (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Budget 	INTEGER NOT NULL
);
CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	DepartmentId INTEGER NOT NULL,
	IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);
CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME,
	Make VARCHAR(55) NOT NULL,
	Manufacturer VARCHAR(55) NOT NULL
);
CREATE TABLE ComputerEmployee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	ComputerId INTEGER NOT NULL,
	AssignDate DATETIME NOT NULL,
	UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);
CREATE TABLE TrainingProgram (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(255) NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MaxAttendees INTEGER NOT NULL
);
CREATE TABLE EmployeeTraining (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);
CREATE TABLE ProductType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL
);
CREATE TABLE Customer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	CreationDate DATETIME NOT NULL,
	LastActiveDate DATETIME NOT NULL
);
CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	ProductTypeId INTEGER NOT NULL,
	CustomerId INTEGER NOT NULL,
	Price MONEY NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);
CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber VARCHAR(55) NOT NULL,
	[Name] VARCHAR(55) NOT NULL,
	CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);
CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);
CREATE TABLE OrderProduct (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);
--DELETE FROM OrderProduct;
--DELETE FROM ComputerEmployee;
--DELETE FROM EmployeeTraining;
--DELETE FROM Employee;
--DELETE FROM TrainingProgram;
--DELETE FROM Computer;
--DELETE FROM Department;
--DELETE FROM [Order];
--DELETE FROM PaymentType;
--DELETE FROM Product;
--DELETE FROM ProductType;
--DELETE FROM Customer;
INSERT INTO Department (Name, Budget) VALUES ('Food and Stuff', 10000)
INSERT INTO Department (Name, Budget) VALUES ('WalMart', 10000000)
INSERT INTO Department (Name, Budget) VALUES ('Target', 10000000)
insert into ProductType ([Name]) values ('Electronics');
insert into ProductType ([Name]) values ('Sporting Goods');
insert into ProductType ([Name]) values ('Home Goods');
Insert into Customer (FirstName, LastName, CreationDate, LastActiveDate) values ('Devin', 'Conroy', '2019-04-30', '2020-08-01');
Insert into Customer (FirstName, LastName, CreationDate, LastActiveDate) values ('Tom', 'Brady', '2019-02-30', '2020-07-01');
Insert into Customer (FirstName, LastName, CreationDate, LastActiveDate) values ('Jaime', 'Lannister', '2019-01-30', '2020-03-01');
insert into Product (ProductTypeId, CustomerId, Price, [Title], [Description], Quantity) Values (1, 1, 300, 'Nintendo Switch', 'Hand held Nintendo', 1);
insert into Product (ProductTypeId, CustomerId, Price, [Title], [Description], Quantity) Values (2, 2, 15, 'Football', 'Inflated Pigskin', 1);
insert into Product (ProductTypeId, CustomerId, Price, [Title], [Description], Quantity) Values (3, 3, 200, 'Golden Hand', 'Hand made of Gold', 1);
insert into [Order] (CustomerId, PaymentTypeId) values (1, 1);
insert into [Order] (CustomerId, PaymentTypeId) values (2, 2);
insert into [Order] (CustomerId, PaymentTypeId) values (3, 3);
insert into OrderProduct (OrderId, ProductId) values (1,1);
insert into OrderProduct (OrderId, ProductId) values (2, 2);
insert into OrderProduct (OrderId, ProductId) values (3, 3);
insert into PaymentType (AcctNumber, [Name], CustomerId) values (123456789, 'Visa', 1);
insert into PaymentType (AcctNumber, [Name], CustomerId) values (9876543210, 'Master Card', 2);
insert into PaymentType (AcctNumber, [Name], CustomerId) values (5469873210, 'American Express', 3);
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSupervisor) VALUES ('Stephen', 'Avila', 1, 'True')
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSupervisor) VALUES ('Devin', 'Conroy', 2, 'True')
INSERT INTO Employee (FirstName, LastName, DepartmentId, IsSupervisor) VALUES ('Mandy', 'Campbell', 3, 'True')
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('2020-05-05', '2020-06-08', 'TUF Gaming Laptop', 'ASUS')
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('2020-04-04', '2020-05-08', 'Workbook Pro', 'Acer')
INSERT INTO Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) VALUES ('2020-03-03', '2020-04-08', 'Macbook', 'Apple')
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (1, 1, '2020-08-08', '2020-06-08')
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (2, 2, '2020-07-07', '2020-05-08')
INSERT INTO ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) VALUES (3, 3, '2020-06-06', '2020-04-08')
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) VALUES ('Food Training', '2020-01-01','2020-03-03',30)
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) VALUES ('WalMart Training', '2020-02-02','2020-04-04',30)
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) VALUES ('Target Training', '2020-03-03','2020-05-05',30)
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (1, 1)
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (2, 2)
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (3, 3)


--Insert before testing TrainingProgram
INSERT INTO TrainingProgram (Name, StartDate, EndDate, MaxAttendees) VALUES ('Target Training', '2020-10-10','2020-11-11',30)
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (3, 4)
INSERT INTO EmployeeTraining (EmployeeId, TrainingProgramId) VALUES (2, 4)