using Microsoft.EntityFrameworkCore;
using SupermarketSystem.Data;
using SupermarketSystem.Models;

namespace SupermarketSystem.Tests
{
    // Creates a fresh in-memory database for each test - no SQL Server needed
    public static class TestDbHelper
    {
        public static SupermarketContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<SupermarketContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new SupermarketContext(options);

            // Seed a category and supplier required by product tests
            context.Categories.Add(new Category { CategoryId = 1, Name = "Dairy" });
            context.Suppliers.Add(new Supplier
            {
                SupplierId = 1,
                Name = "FreshFoods Ltd",
                ContactEmail = "contact@freshfoods.com",
                Phone = "01234567890"
            });
            context.SaveChanges();
            return context;
        }

        // Returns a valid product ready for testing
        public static Product MakeProduct(string title = "Test Milk",
            string barcode = "123456", decimal price = 1.50m, int qty = 20)
        {
            return new Product
            {
                Title = title,
                Barcode = barcode,
                Price = price,
                QuantityInStock = qty,
                LowStockThreshold = 5,
                ExpiryDate = DateTime.Today.AddDays(30),
                CategoryId = 1,
                SupplierId = 1
            };
        }
    }
}