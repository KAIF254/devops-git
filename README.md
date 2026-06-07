# 📚 Online Book Store — Premium ASP.NET Core MVC 9 Project

A complete, beginner-friendly, and visually stunning Online Book Store web application built for college projects. It features a modern Bootstrap 5 UI, a full category system, an admin dashboard, and a complete purchase flow.

---

## 🚀 Top Features

- **🎨 Modern Premium UI**: Built with Bootstrap 5, featuring gradients, card hover animations, and a sleek dark navbar.
- **⚡ Admin Dashboard**: Centralized dashboard to view total revenue, book counts, user lists, and recent orders.
- **🗂️ Categories**: Books are organized by genre (Fiction, Science, Programming, etc.) with filterable browsing.
- **🔍 Smart Search**: Search books by title and filter by category simultaneously.
- **🛒 Dynamic Orders**: Real-time stock reduction, order confirmation, and personal order history.
- **🔐 User Accounts**: Simple, session-based Register and Login system (no complex identity setup needed).

---

## 🛠️ Prerequisites

Before you start, make sure you have these installed:
1.  **.NET 9 SDK**
2.  **SQL Server Express**
3.  **SQL Server Management Studio (SSMS)**
4.  **VS Code** with **C# Dev Kit** extension.

---

## 📦 How to Set Up (Step-by-Step)

### 1. Database Setup (Crucial)
1.  Open **SQL Server Management Studio (SSMS)**.
2.  Press **Ctrl + N** (New Query).
3.  Open the file `SQL/setup.sql` from this project, copy EVERYTHING, and paste it into SSMS.
4.  Press **F5** (Execute).
    *   *This will create the `OnlineBookStoreDB` database, all 5 tables, and insert sample data.*

### 2. Configure Connection String
1.  Open `appsettings.json` in VS Code.
2.  Ensure your `DefaultConnection` matches your SQL Server name:
    *   **Commonly**: `Server=localhost\SQLEXPRESS;...`
    *   **Or**: `Server=.;...` (the dot means localhost)

### 3. Run the App
1.  Open a terminal in the `OnlineBookStore` folder.
2.  Run these commands:
    ```bash
    dotnet restore
    dotnet build
    dotnet run
    ```
3.  Open your browser to: `https://localhost:7xxx` (see terminal for exact port) or `http://localhost:5000`.

---

## 📁 Project Structure (Simple & Clean)

- **Controllers/**: The "Brains" — handles logic for Home, Books, Categories, Accounts, and Orders.
- **Models/**: The "Skeleton" — C# classes representing database tables.
- **Views/**: The "Face" — HTML/Razor files that define how the website looks.
- **Data/ApplicationDbContext.cs**: The "Bridge" — connects your code to SQL Server.
- **SQL/setup.sql**: The "Foundation" — the script to build your database.
- **wwwroot/css/site.css**: The "Styler" — custom CSS for that premium look.

---

## 💡 Beginner Tips
- **Comments**: Every file has simple comments explaining what the code is doing.
- **Stock Control**: When you "Buy" a book, the `Stock` in the `Books` table automatically goes down.
- **Admin Access**: Find the "Admin" link in the navbar to see the dashboard.
- **Logout**: To switch users, click your name in the navbar and select Logout.

---
*Created for College Project — Easy to read, easy to modify.*
