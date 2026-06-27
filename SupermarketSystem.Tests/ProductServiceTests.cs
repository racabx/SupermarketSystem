using SupermarketSystem.Services;

namespace SupermarketSystem.Tests
{
    // Tests for product CRUD operations, search and validation
    public class ProductServiceTests
    {
        [Fact]
        public void AddProduct_ValidProduct_SavedToDatabase()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            var product = TestDbHelper.MakeProduct("Fresh Milk", "BAR001");

            // Act
            service.AddProduct(product);

            // Assert - one product should now exist
            Assert.Single(context.Products);
        }

        [Fact]
        public void AddProduct_DuplicateBarcode_ThrowsException()
        {
            // Arrange - add first product
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            service.AddProduct(TestDbHelper.MakeProduct("Milk", "DUPE01"));

            // Act & Assert - same barcode should be rejected
            var ex = Assert.Throws<Exception>(() =>
                service.AddProduct(TestDbHelper.MakeProduct("Butter", "DUPE01")));
            Assert.Contains("barcode already exists", ex.Message);
        }

        [Fact]
        public void AddProduct_NegativePrice_ThrowsException()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            var product = TestDbHelper.MakeProduct(price: -5.00m);

            // Act & Assert - negative price must be rejected
            var ex = Assert.Throws<Exception>(() => service.AddProduct(product));
            Assert.Contains("greater than zero", ex.Message);
        }

        [Fact]
        public void AddProduct_EmptyTitle_ThrowsException()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            var product = TestDbHelper.MakeProduct(title: "");

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => service.AddProduct(product));
            Assert.Contains("cannot be empty", ex.Message);
        }

        [Fact]
        public void SearchByName_PartialMatch_ReturnsResults()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            service.AddProduct(TestDbHelper.MakeProduct("Whole Milk", "S001"));
            service.AddProduct(TestDbHelper.MakeProduct("Skimmed Milk", "S002"));
            service.AddProduct(TestDbHelper.MakeProduct("Orange Juice", "S003"));

            // Act - "milk" should match two products
            var results = service.SearchByName("milk");

            // Assert
            Assert.Equal(2, results.Count);
        }

        [Fact]
        public void SearchByBarcode_ExactMatch_ReturnsProduct()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            service.AddProduct(TestDbHelper.MakeProduct("Eggs", "EGG001"));

            // Act - hash table barcode lookup
            var result = service.SearchByBarcode("EGG001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("EGG001", result.Barcode);
        }

        [Fact]
        public void GetLowStockProducts_ReturnsOnlyLowStock()
        {
            // Arrange - one low stock item, one healthy
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            service.AddProduct(TestDbHelper.MakeProduct("Low Item", "LOW01", qty: 2));
            service.AddProduct(TestDbHelper.MakeProduct("Good Item", "GOD01", qty: 50));

            // Act
            var lowStock = service.GetLowStockProducts();

            // Assert - only the item below threshold (5) returned
            Assert.Single(lowStock);
            Assert.Equal("LOW01", lowStock[0].Barcode);
        }

        [Fact]
        public void DeleteProduct_ExistingProduct_RemovedFromDatabase()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var service = new ProductService(context);
            service.AddProduct(TestDbHelper.MakeProduct("Delete Me", "DEL001"));
            int id = context.Products.First().ProductId;

            // Act
            service.DeleteProduct(id);

            // Assert - database should be empty
            Assert.Empty(context.Products);
        }
    }
}