CREATE DATABASE DemonstrationDB

GO

USE DemonstrationDB
GO

CREATE TABLE Users(
	Id INT IDENTITY  Primary key,
	UserName nvarchar(100) NOT NULL,
	PasswordHash nvarChar(100) NOT NULL
);

CREATE TABLE Products(
	Id INT IDENTITY Primary key,
	Name nvarchar(100) NOT NULL,
	Description nvarchar(200) NOT NULL,
	ImageSource nvarCHAR(400) NOT NULL,
	Price DECIMAL NOT NULL
);

CREATE TABLE Carts(
	Id INT IDENTITY PRIMARY key,
	UserID Int NOt null,
	ProductID INT NOT NULL,
	CONSTRAINT FK_CARTS_USERS FOREIGN KEY (UserID) REFERENCES Users(Id),
	CONSTRAINT FK_CARTS_PRODUCTS FOREIGN KEY (ProductID) REFERENCES Products(Id)
);