# Supermarket Management System
### CST2550 Software Engineering - Reset Coursework
**Name:** Rajab Alasgarov | **Student ID:** M01002269

---

## Overview
Windows desktop program built in C# .NET 10 WinForms to help small supermarkets
replace paperwork records with a digital management system. The system supports
product management, stock control, supplier records, sales tracking, low stock 
alerts and reporting.

---

## Requirements
- Windows 10 or later.
- Visual Studio 2026 — https://visualstudio.microsoft.com
- SQL Server Express 2022 - https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- .NET 10 SDK — https://dotnet.microsoft.com/download

---

## Database Setup

### Option A - Automatic
The program automatically creates and seeds the database on first launch. 
No manual setup required.

### Option B - Manual (via SQL Script)
1. Open **SQL Server Management Studio (SSMS)**
2. Connect to `localhost\SQLEXPRESS`
3. Open the file `DatabaseScript.sql` from the repository.
4. Click **Execute**.

The script will create the `SupermarketDB` database with all tables and sample data.

---

## How To Run
1. Clone the repository: git clone https://github.com/racabx/SupermarketSystem.git
2. Open `SupermarketSystem.slnx` in Visual Studio 2026.
3. Make sure the startup project is set to **SupermarketSystem**.
4. Press **F5** or click the green "Play" button.
5. The app will launch and seed the database automatically on first run.

---

## Connection String
The database connection is configured in `Data/SupermarketContext.cs`:
Server=localhost\SQLEXPRESS;Database=SupermarketDB;Trusted_Connection=True;TrustServerCertificate=True;
If your SQL server instance has a different name, update this line accordingly.

---

## How To Run Tests
1. Open the solution in Visual Studio 2026.
2. Go to **Test -> Run All Tests**.
3. All 18 tests should pass (Green Ticks in Test Explorer).

---

## Project Structure

SupermarketSystem/
|- Data/               # Database context and seed data.
|- DataStructures/     # Custom hash table implementation.
|- Forms/              # All WinForms UI screens.
|- Models/             # Entity classes (Product, Sale, etc.).
|- Services/           # Business logic layer.

SupermarketSystem.Tests/
|- ProductHashTableTests.cs
|- ProductServiceTests.cs
|- SaleServiceTests.cs
|- TestDbHelper.cs

---

## Features
- **Product Management** — Add, Edit, Delete and Search products by name or barcode.
- **Category Management** — Organise products into categories.
- **Supplier Management** — Maintain supplier contact records.
- **Sales Recording** — Basket based sales with automatic stock deduction.
- **Low Stock Alerts** — Visual warnings when stock falls below threshold.
- **Reports** — Four built-in reports: low stock, sales history, products by category, supplier stock.

---

## Video Demonstration
The video demonstration file is included in this repository as Video.mp4
