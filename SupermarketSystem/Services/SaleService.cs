using Microsoft.EntityFrameworkCore;
using SupermarketSystem.Data;
using SupermarketSystem.Models;

namespace SupermarketSystem.Services
{
    // Handles recording sales and updating stock levels after each transaction
    public class SaleService
    {
        private readonly SupermarketContext _context;

        public SaleService(SupermarketContext context)
        {
            _context = context;
        }

        // Get all products available for sale
        public List<Product> GetAvailableProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.QuantityInStock > 0)
                .OrderBy(p => p.Title)
                .ToList();
        }

        // Record a completed sale and reduce stock for each item sold
        public Sale RecordSale(List<SaleItem> items)
        {
            if (items == null || items.Count == 0)
                throw new Exception("Cannot record an empty sale.");

            // Validate stock availability before saving anything
            foreach (var item in items)
            {
                var product = _context.Products.Find(item.ProductId);
                if (product == null)
                    throw new Exception($"Product ID {item.ProductId} not found.");

                if (item.Quantity <= 0)
                    throw new Exception($"Quantity for '{product.Title}' must be greater than zero.");

                if (item.Quantity > product.QuantityInStock)
                    throw new Exception($"Not enough stock for '{product.Title}'. Available: {product.QuantityInStock}");
            }

            // Build the sale record
            var sale = new Sale
            {
                SaleDate = DateTime.Now,
                TotalAmount = items.Sum(i => i.Quantity * i.UnitPrice),
                SaleItems = items
            };

            // Deduct sold quantities from stock
            foreach (var item in items)
            {
                var product = _context.Products.Find(item.ProductId)!;
                product.QuantityInStock -= item.Quantity;
            }

            _context.Sales.Add(sale);
            _context.SaveChanges();
            return sale;
        }

        // Get all past sales with their items for reporting
        public List<Sale> GetAllSales()
        {
            return _context.Sales
                .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
                .OrderByDescending(s => s.SaleDate)
                .ToList();
        }
    }
}