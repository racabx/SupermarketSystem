using SupermarketSystem.Models;
using SupermarketSystem.Services;
using System.Net.ServerSentEvents;

namespace SupermarketSystem.Tests
{
    // Tests for sale recording, stock deduction and validation
    public class SaleServiceTests
    {
        [Fact]
        public void RecordSale_ValidItems_SaleIsSaved()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var productService = new ProductService(context);
            var saleService = new SaleService(context);

            productService.AddProduct(TestDbHelper.MakeProduct("Bread", "BRD001", qty: 10));
            int productId = context.Products.First().ProductId;

            var items = new List<SaleItem>
            {
                new SaleItem { ProductId = productId, Quantity = 2, UnitPrice = 1.50m }
            };

            // Act
            var sale = saleService.RecordSale(items);

            // Assert - sale recorded with correct total
            Assert.NotNull(sale);
            Assert.Equal(3.00m, sale.TotalAmount);
        }

        [Fact]
        public void RecordSale_ReducesStockCorrectly()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var productService = new ProductService(context);
            var saleService = new SaleService(context);

            productService.AddProduct(TestDbHelper.MakeProduct("Butter", "BUT001", qty: 20));
            int productId = context.Products.First().ProductId;

            var items = new List<SaleItem>
            {
                new SaleItem { ProductId = productId, Quantity = 5, UnitPrice = 2.00m }
            };

            // Act
            saleService.RecordSale(items);

            // Assert - stock should drop from 20 to 15
            var product = context.Products.Find(productId);
            Assert.Equal(15, product!.QuantityInStock);
        }

        [Fact]
        public void RecordSale_EmptyBasket_ThrowsException()
        {
            // Arrange
            var context = TestDbHelper.CreateInMemoryContext();
            var saleService = new SaleService(context);

            // Act & Assert - empty sale must be rejected
            var ex = Assert.Throws<Exception>(() =>
                saleService.RecordSale(new List<SaleItem>()));
            Assert.Contains("empty sale", ex.Message);
        }

        [Fact]
        public void RecordSale_InsufficientStock_ThrowsException()
        {
            // Arrange - product only has 3 in stock
            var context = TestDbHelper.CreateInMemoryContext();
            var productService = new ProductService(context);
            var saleService = new SaleService(context);

            productService.AddProduct(TestDbHelper.MakeProduct("Rare Item", "RARE1", qty: 3));
            int productId = context.Products.First().ProductId;

            var items = new List<SaleItem>
            {
                new SaleItem { ProductId = productId, Quantity = 10, UnitPrice = 1.00m }
            };

            // Act & Assert - should fail due to insufficient stock
            var ex = Assert.Throws<Exception>(() => saleService.RecordSale(items));
            Assert.Contains("Not enough stock", ex.Message);
        }

        [Fact]
        public void GetAllSales_ReturnsSalesInDescendingOrder()
        {
            // Arrange - record two sales
            var context = TestDbHelper.CreateInMemoryContext();
            var productService = new ProductService(context);
            var saleService = new SaleService(context);

            productService.AddProduct(TestDbHelper.MakeProduct("Item A", "ITM001", qty: 50));
            int productId = context.Products.First().ProductId;

            saleService.RecordSale(new List<SaleItem>
                { new SaleItem { ProductId = productId, Quantity = 1, UnitPrice = 1.00m } });
            saleService.RecordSale(new List<SaleItem>
                { new SaleItem { ProductId = productId, Quantity = 2, UnitPrice = 1.00m } });

            // Act
            var sales = saleService.GetAllSales();

            // Assert - most recent sale appears first
            Assert.Equal(2, sales.Count);
            Assert.True(sales[0].SaleDate >= sales[1].SaleDate);
        }
    }
}