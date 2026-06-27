using Microsoft.EntityFrameworkCore;
using SupermarketSystem.Data;
using SupermarketSystem.DataStructures;
using SupermarketSystem.Models;

namespace SupermarketSystem.Services
{
    // Handles all product operations - connects UI to database and hash table
    public class ProductService
    {
        private readonly SupermarketContext _context;
        private readonly ProductHashTable _hashTable;

        public ProductService(SupermarketContext context)
        {
            _context = context;
            _hashTable = new ProductHashTable();
            LoadHashTable(); // fill hash table when service starts
        }

        // Load all products from DB into hash table at startup
        private void LoadHashTable()
        {
            var products = _context.Products.ToList();
            _hashTable.LoadProducts(products);
        }

        // Get all products with category and supplier details
        public List<Product> GetAllProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .ToList();
        }

        // Barcode lookup using hash table - O(1) average
        public Product? SearchByBarcode(string barcode)
        {
            return _hashTable.Search(barcode);
        }

        // Name search using linear scan through database - O(n)
        public List<Product> SearchByName(string name)
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.Title.ToLower().Contains(name.ToLower()))
                .ToList();
        }

        // Returns products that are at or below their low stock threshold
        public List<Product> GetLowStockProducts()
        {
            return _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.QuantityInStock <= p.LowStockThreshold)
                .ToList();
        }

        // Add a new product with validation checks
        public void AddProduct(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Title))
                throw new Exception("Product title cannot be empty.");

            // Barcodes must be unique across all products
            if (_context.Products.Any(p => p.Barcode == product.Barcode))
                throw new Exception("A product with this barcode already exists.");

            if (product.Price <= 0)
                throw new Exception("Price must be greater than zero.");

            if (product.QuantityInStock < 0)
                throw new Exception("Quantity cannot be negative.");

            _context.Products.Add(product);
            _context.SaveChanges();
            _hashTable.Insert(product.Barcode, product); // keep hash table in sync
        }

        // Update an existing product
        public void UpdateProduct(Product product)
        {
            var existing = _context.Products.Find(product.ProductId);
            if (existing == null)
                throw new Exception("Product not found.");

            // Check barcode is not taken by a different product
            if (existing.Barcode != product.Barcode &&
                _context.Products.Any(p => p.Barcode == product.Barcode))
                throw new Exception("A product with this barcode already exists.");

            // Remove old barcode from hash table before updating
            _hashTable.Delete(existing.Barcode);

            existing.Title = product.Title;
            existing.Barcode = product.Barcode;
            existing.Price = product.Price;
            existing.QuantityInStock = product.QuantityInStock;
            existing.LowStockThreshold = product.LowStockThreshold;
            existing.ExpiryDate = product.ExpiryDate;
            existing.CategoryId = product.CategoryId;
            existing.SupplierId = product.SupplierId;

            _context.SaveChanges();
            _hashTable.Insert(product.Barcode, existing); // re-add with updated barcode
        }

        // Remove a product from database and hash table
        public void DeleteProduct(int productId)
        {
            var product = _context.Products.Find(productId);
            if (product == null)
                throw new Exception("Product not found.");

            _hashTable.Delete(product.Barcode);
            _context.Products.Remove(product);
            _context.SaveChanges();
        }
    }
}