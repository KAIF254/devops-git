
-- ── Step 1: Create database if it doesn't exist ─────────────
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'OnlineBookStoreDB')
    CREATE DATABASE OnlineBookStoreDB;
GO
USE OnlineBookStoreDB;
GO

-- ── Step 2: Drop tables in reverse dependency order ─────────
-- (Safe to re-run the script multiple times)
IF OBJECT_ID('OrderItems', 'U') IS NOT NULL DROP TABLE OrderItems;
IF OBJECT_ID('Orders',     'U') IS NOT NULL DROP TABLE Orders;
IF OBJECT_ID('Books',      'U') IS NOT NULL DROP TABLE Books;
IF OBJECT_ID('Categories', 'U') IS NOT NULL DROP TABLE Categories;
IF OBJECT_ID('Users',      'U') IS NOT NULL DROP TABLE Users;
GO

-- ── TABLE: Users ─────────────────────────────────────────────
-- Role: "Admin" for the store manager, "User" for customers
CREATE TABLE Users (
    Id       INT PRIMARY KEY IDENTITY(1,1),
    Name     NVARCHAR(100)  NOT NULL,
    Email    NVARCHAR(150)  NOT NULL UNIQUE,
    Password NVARCHAR(100)  NOT NULL,
    Role     NVARCHAR(20)   NOT NULL DEFAULT 'User'
);
GO

-- ── TABLE: Categories ────────────────────────────────────────
CREATE TABLE Categories (
    Id   INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL
);
GO

-- ── TABLE: Books ─────────────────────────────────────────────
-- ReleaseDate: publication date shown on Details page
-- ImageUrl: optional URL for cover image (NULL = use gradient)
CREATE TABLE Books (
    Id          INT PRIMARY KEY IDENTITY(1,1),
    Title       NVARCHAR(200)  NOT NULL,
    Author      NVARCHAR(150)  NOT NULL,
    Description NVARCHAR(2000),
    Price       DECIMAL(10,2)  NOT NULL,
    Stock       INT            NOT NULL DEFAULT 0,
    CategoryId  INT            NOT NULL,
    ReleaseDate DATE           NULL,
    ImageUrl    NVARCHAR(300)  NULL,
    FOREIGN KEY (CategoryId) REFERENCES Categories(Id)
);
GO

-- ── TABLE: Orders ────────────────────────────────────────────
-- Status: Pending → Shipped → Delivered (admin can also set Cancelled)
CREATE TABLE Orders (
    Id          INT PRIMARY KEY IDENTITY(1,1),
    UserId      INT           NOT NULL,
    OrderDate   DATETIME      NOT NULL DEFAULT GETDATE(),
    TotalAmount DECIMAL(10,2) NOT NULL,
    Status      NVARCHAR(30)  NOT NULL DEFAULT 'Pending',
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- ── TABLE: OrderItems ────────────────────────────────────────
CREATE TABLE OrderItems (
    Id       INT PRIMARY KEY IDENTITY(1,1),
    OrderId  INT           NOT NULL,
    BookId   INT           NOT NULL,
    Quantity INT           NOT NULL,
    Price    DECIMAL(10,2) NOT NULL,
    FOREIGN KEY (OrderId) REFERENCES Orders(Id),
    FOREIGN KEY (BookId)  REFERENCES Books(Id)
);
GO

-- ── SEED DATA: Users ─────────────────────────────────────────
-- Admin account: login with admin@bookstore.com / admin123
INSERT INTO Users (Name, Email, Password, Role) VALUES
('Admin',       'admin@bookstore.com', 'admin123',   'Admin'),
('Alice Smith',  'alice@test.com',      'password123', 'User'),
('Bob Jones',    'bob@test.com',        'password123', 'User');
GO

-- ── SEED DATA: Categories ─────────────────────────────────────
INSERT INTO Categories (Name) VALUES
('Fiction'),
('Science & Technology'),
('History'),
('Self-Help'),
('Programming'),
('Biography'),
('Children');
GO

-- ── SEED DATA: Books ─────────────────────────────────────────
INSERT INTO Books (Title, Author, Description, Price, Stock, CategoryId, ReleaseDate) VALUES
('The Great Gatsby',          'F. Scott Fitzgerald', 'A classic novel about the American Dream set in the Jazz Age of the 1920s. A story of wealth, obsession, and the failure of dreams.', 9.99,  50, 1, '1925-04-10'),
('To Kill a Mockingbird',     'Harper Lee',          'A powerful story about racial injustice and moral growth in the American South, seen through the eyes of young Scout Finch.', 12.99, 30, 1, '1960-07-11'),
('1984',                      'George Orwell',        'A chilling dystopian novel about a totalitarian society where Big Brother watches your every move. A timeless warning about tyranny.', 10.99, 40, 1, '1949-06-08'),
('Harry Potter Vol. 1',       'J.K. Rowling',         'A young wizard discovers he is famous in the magical world and begins his adventure at Hogwarts School of Witchcraft and Wizardry.', 14.99, 100, 1, '1997-06-26'),
('The Alchemist',             'Paulo Coelho',         'A philosophical and inspirational novel about following your personal legend and listening to your heart on the journey of life.', 8.99,  60, 4, '1988-01-01'),
('Atomic Habits',             'James Clear',          'An easy and proven way to build good habits and break bad ones. Tiny changes lead to remarkable results — the compounding of 1% improvements.', 16.99, 80, 4, '2018-10-16'),
('A Brief History of Time',   'Stephen Hawking',      'A landmark volume in science writing that takes the reader on a journey through the history of the universe — from the Big Bang to black holes.', 13.99, 25, 2, '1988-04-01'),
('Clean Code',                'Robert C. Martin',     'A handbook of agile software craftsmanship. Learn how to write code that is clean, maintainable, and a pleasure to read from Uncle Bob himself.', 29.99, 35, 5, '2008-08-01'),
('Introduction to Algorithms','Thomas Cormen',        'A comprehensive and strikingly original textbook covering a broad range of algorithms in depth. The definitive resource for computer science students.', 49.99, 15, 5, '1990-01-01'),
('Sapiens',                   'Yuval Noah Harari',    'A brief history of humankind, from the Stone Age to the Silicon Age. Harari explores how biology and history shaped us and our societies.', 17.99, 45, 3, '2011-01-01'),
('Steve Jobs',                'Walter Isaacson',      'The exclusive biography of Apple co-founder Steve Jobs based on over forty interviews. A riveting story of a creative entrepreneur.', 19.99, 20, 6, '2011-10-24'),
('Elon Musk',                 'Walter Isaacson',      'The story of one of the most daring and controversial entrepreneurs alive — the man behind Tesla, SpaceX, and X (Twitter).', 21.99, 18, 6, '2023-09-12');
GO
