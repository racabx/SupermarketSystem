using SupermarketSystem.Models;

namespace SupermarketSystem.Data
{
    // Populates the database with sample data for demonstration purposes
    public static class SeedData
    {
        public static void Initialise(SupermarketContext context)
        {
            // Only seed if database is completely empty
            if (context.Categories.Any()) return;

            // --- Categories ---
            var categories = new List<Category>
            {
                new Category { Name = "Dairy" },
                new Category { Name = "Bakery" },
                new Category { Name = "Beverages" },
                new Category { Name = "Snacks" },
                new Category { Name = "Fruits & Vegetables" },
                new Category { Name = "Meat & Fish" },
                new Category { Name = "Frozen Foods" },
                new Category { Name = "Household" }
            };
            context.Categories.AddRange(categories);
            context.SaveChanges();

            // --- Suppliers ---
            var suppliers = new List<Supplier>
            {
                new Supplier
                {
                    Name         = "FreshFoods Ltd",
                    ContactEmail = "orders@freshfoods.com",
                    Phone        = "01234567890"
                },
                new Supplier
                {
                    Name         = "DairyBest Co",
                    ContactEmail = "supply@dairybest.com",
                    Phone        = "01987654321"
                },
                new Supplier
                {
                    Name         = "BakerySupplies UK",
                    ContactEmail = "info@bakerysupplies.co.uk",
                    Phone        = "01765432198"
                },
                new Supplier
                {
                    Name         = "GlobalDrinks Inc",
                    ContactEmail = "trade@globaldrinks.com",
                    Phone        = "01654321987"
                }
            };
            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            // Get saved IDs for linking products
            int dairy = categories[0].CategoryId;
            int bakery = categories[1].CategoryId;
            int beverages = categories[2].CategoryId;
            int snacks = categories[3].CategoryId;
            int fruitsVeg = categories[4].CategoryId;
            int meat = categories[5].CategoryId;
            int frozen = categories[6].CategoryId;
            int household = categories[7].CategoryId;

            int fresh = suppliers[0].SupplierId;
            int dairyBst = suppliers[1].SupplierId;
            int bakeryS = suppliers[2].SupplierId;
            int drinks = suppliers[3].SupplierId;

            // --- Products ---
            var products = new List<Product>
            {
                // Dairy
                new Product { Title = "Whole Milk 2L",       Barcode = "5000112637922", Price = 1.45m, QuantityInStock = 40, LowStockThreshold = 10, ExpiryDate = DateTime.Today.AddDays(7),  CategoryId = dairy,     SupplierId = dairyBst },
                new Product { Title = "Skimmed Milk 1L",     Barcode = "5000112637923", Price = 0.99m, QuantityInStock = 3,  LowStockThreshold = 10, ExpiryDate = DateTime.Today.AddDays(6),  CategoryId = dairy,     SupplierId = dairyBst },
                new Product { Title = "Cheddar Cheese 400g", Barcode = "5010123456781", Price = 2.85m, QuantityInStock = 25, LowStockThreshold = 5,  ExpiryDate = DateTime.Today.AddDays(21), CategoryId = dairy,     SupplierId = dairyBst },
                new Product { Title = "Greek Yoghurt 500g",  Barcode = "5010123456782", Price = 1.65m, QuantityInStock = 4,  LowStockThreshold = 8,  ExpiryDate = DateTime.Today.AddDays(10), CategoryId = dairy,     SupplierId = dairyBst },
                new Product { Title = "Salted Butter 250g",  Barcode = "5010123456783", Price = 1.20m, QuantityInStock = 18, LowStockThreshold = 5,  ExpiryDate = DateTime.Today.AddDays(30), CategoryId = dairy,     SupplierId = dairyBst },

                // Bakery
                new Product { Title = "White Bread 800g",    Barcode = "5010251001218", Price = 1.10m, QuantityInStock = 20, LowStockThreshold = 8,  ExpiryDate = DateTime.Today.AddDays(4),  CategoryId = bakery,    SupplierId = bakeryS  },
                new Product { Title = "Brown Bread 800g",    Barcode = "5010251001219", Price = 1.25m, QuantityInStock = 2,  LowStockThreshold = 8,  ExpiryDate = DateTime.Today.AddDays(4),  CategoryId = bakery,    SupplierId = bakeryS  },
                new Product { Title = "Croissants 4 Pack",   Barcode = "5010251001220", Price = 1.80m, QuantityInStock = 12, LowStockThreshold = 5,  ExpiryDate = DateTime.Today.AddDays(3),  CategoryId = bakery,    SupplierId = bakeryS  },

                // Beverages
                new Product { Title = "Orange Juice 1L",     Barcode = "5449000000996", Price = 1.35m, QuantityInStock = 30, LowStockThreshold = 10, ExpiryDate = DateTime.Today.AddDays(14), CategoryId = beverages, SupplierId = drinks   },
                new Product { Title = "Sparkling Water 6pk", Barcode = "5449000000997", Price = 2.50m, QuantityInStock = 22, LowStockThreshold = 6,  ExpiryDate = DateTime.Today.AddDays(90), CategoryId = beverages, SupplierId = drinks   },
                new Product { Title = "Cola 2L",             Barcode = "5449000000998", Price = 1.75m, QuantityInStock = 3,  LowStockThreshold = 8,  ExpiryDate = DateTime.Today.AddDays(60), CategoryId = beverages, SupplierId = drinks   },
                new Product { Title = "Apple Juice 1L",      Barcode = "5449000000999", Price = 1.20m, QuantityInStock = 17, LowStockThreshold = 6,  ExpiryDate = DateTime.Today.AddDays(20), CategoryId = beverages, SupplierId = drinks   },

                // Snacks
                new Product { Title = "Ready Salted Crisps", Barcode = "5010251312342", Price = 0.89m, QuantityInStock = 45, LowStockThreshold = 10, ExpiryDate = DateTime.Today.AddDays(45), CategoryId = snacks,    SupplierId = fresh    },
                new Product { Title = "Milk Chocolate Bar",  Barcode = "5010251312343", Price = 0.75m, QuantityInStock = 60, LowStockThreshold = 15, ExpiryDate = DateTime.Today.AddDays(90), CategoryId = snacks,    SupplierId = fresh    },

                // Fruits & Vegetables
                new Product { Title = "Bananas Bunch",       Barcode = "5010251312350", Price = 0.65m, QuantityInStock = 2,  LowStockThreshold = 10, ExpiryDate = DateTime.Today.AddDays(5),  CategoryId = fruitsVeg, SupplierId = fresh    },
                new Product { Title = "Broccoli 400g",       Barcode = "5010251312351", Price = 0.79m, QuantityInStock = 15, LowStockThreshold = 5,  ExpiryDate = DateTime.Today.AddDays(6),  CategoryId = fruitsVeg, SupplierId = fresh    },
                new Product { Title = "Cherry Tomatoes 250g",Barcode = "5010251312352", Price = 1.10m, QuantityInStock = 20, LowStockThreshold = 5,  ExpiryDate = DateTime.Today.AddDays(7),  CategoryId = fruitsVeg, SupplierId = fresh    },

                // Meat & Fish
                new Product { Title = "Chicken Breast 500g", Barcode = "5010251312360", Price = 3.50m, QuantityInStock = 4,  LowStockThreshold = 5,  ExpiryDate = DateTime.Today.AddDays(3),  CategoryId = meat,      SupplierId = fresh    },
                new Product { Title = "Salmon Fillet 300g",  Barcode = "5010251312361", Price = 4.25m, QuantityInStock = 8,  LowStockThreshold = 4,  ExpiryDate = DateTime.Today.AddDays(2),  CategoryId = meat,      SupplierId = fresh    },

                // Frozen
                new Product { Title = "Frozen Peas 900g",    Barcode = "5010251312370", Price = 1.45m, QuantityInStock = 25, LowStockThreshold = 6,  ExpiryDate = DateTime.Today.AddDays(180),CategoryId = frozen,    SupplierId = fresh    },
                new Product { Title = "Beef Burgers 4 Pack", Barcode = "5010251312371", Price = 2.99m, QuantityInStock = 3,  LowStockThreshold = 5,  ExpiryDate = DateTime.Today.AddDays(90), CategoryId = frozen,    SupplierId = fresh    },

                // Household
                new Product { Title = "Washing Up Liquid",   Barcode = "5010251312380", Price = 1.05m, QuantityInStock = 30, LowStockThreshold = 8,  ExpiryDate = DateTime.Today.AddYears(2), CategoryId = household, SupplierId = fresh    },
                new Product { Title = "Kitchen Roll 2 Pack", Barcode = "5010251312381", Price = 1.89m, QuantityInStock = 22, LowStockThreshold = 6,  ExpiryDate = DateTime.Today.AddYears(3), CategoryId = household, SupplierId = fresh    },
            };

            context.Products.AddRange(products);
            context.SaveChanges();

            // --- Sample Sales ---
            var sale1 = new Sale
            {
                SaleDate = DateTime.Now.AddDays(-2),
                TotalAmount = 5.29m,
                SaleItems = new List<SaleItem>
                {
                    new SaleItem { ProductId = products[0].ProductId, Quantity = 2, UnitPrice = 1.45m },
                    new SaleItem { ProductId = products[5].ProductId, Quantity = 1, UnitPrice = 1.10m },
                    new SaleItem { ProductId = products[8].ProductId, Quantity = 1, UnitPrice = 1.35m }
                }
            };

            var sale2 = new Sale
            {
                SaleDate = DateTime.Now.AddDays(-1),
                TotalAmount = 7.14m,
                SaleItems = new List<SaleItem>
                {
                    new SaleItem { ProductId = products[2].ProductId, Quantity = 1, UnitPrice = 2.85m },
                    new SaleItem { ProductId = products[12].ProductId, Quantity = 2, UnitPrice = 0.89m },
                    new SaleItem { ProductId = products[9].ProductId, Quantity = 1, UnitPrice = 2.50m }
                }
            };

            var sale3 = new Sale
            {
                SaleDate = DateTime.Now,
                TotalAmount = 9.74m,
                SaleItems = new List<SaleItem>
                {
                    new SaleItem { ProductId = products[16].ProductId, Quantity = 1, UnitPrice = 3.50m },
                    new SaleItem { ProductId = products[17].ProductId, Quantity = 1, UnitPrice = 4.25m },
                    new SaleItem { ProductId = products[14].ProductId, Quantity = 3, UnitPrice = 0.65m }
                }
            };

            context.Sales.AddRange(sale1, sale2, sale3);
            context.SaveChanges();
        }
    }
}