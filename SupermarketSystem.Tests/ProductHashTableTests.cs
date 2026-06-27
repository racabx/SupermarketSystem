using SupermarketSystem.DataStructures;
using SupermarketSystem.Models;

namespace SupermarketSystem.Tests
{
    // Tests for the custom hash table data structure
    public class ProductHashTableTests
    {
        private Product MakeProduct(string barcode) => new Product
        {
            Title = "Test Item",
            Barcode = barcode,
            Price = 1.00m
        };

        [Fact]
        public void Insert_And_Search_ReturnsCorrectProduct()
        {
            // Arrange
            var table = new ProductHashTable();
            var product = MakeProduct("ABC123");

            // Act
            table.Insert("ABC123", product);
            var result = table.Search("ABC123");

            // Assert - inserted product should be found
            Assert.NotNull(result);
            Assert.Equal("ABC123", result.Barcode);
        }

        [Fact]
        public void Search_NonExistentBarcode_ReturnsNull()
        {
            // Arrange
            var table = new ProductHashTable();

            // Act - search on empty table
            var result = table.Search("NOTHERE");

            // Assert - nothing should be found
            Assert.Null(result);
        }

        [Fact]
        public void Delete_RemovesProduct_SearchReturnsNull()
        {
            // Arrange
            var table = new ProductHashTable();
            table.Insert("DEL999", MakeProduct("DEL999"));

            // Act
            table.Delete("DEL999");
            var result = table.Search("DEL999");

            // Assert - deleted product should not be found
            Assert.Null(result);
        }

        [Fact]
        public void Insert_MultipleProducts_AllFoundCorrectly()
        {
            // Arrange
            var table = new ProductHashTable();
            var barcodes = new[] { "P001", "P002", "P003", "P004", "P005" };

            // Act
            foreach (var b in barcodes)
                table.Insert(b, MakeProduct(b));

            // Assert - every inserted product is retrievable
            foreach (var b in barcodes)
                Assert.NotNull(table.Search(b));
        }

        [Fact]
        public void LoadProducts_BulkLoad_AllSearchable()
        {
            // Arrange - simulate loading products from database on startup
            var table = new ProductHashTable();
            var products = new List<Product>
            {
                MakeProduct("BULK1"),
                MakeProduct("BULK2"),
                MakeProduct("BULK3")
            };

            // Act
            table.LoadProducts(products);

            // Assert
            Assert.NotNull(table.Search("BULK1"));
            Assert.NotNull(table.Search("BULK2"));
            Assert.NotNull(table.Search("BULK3"));
        }
    }
}