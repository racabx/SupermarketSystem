using Microsoft.EntityFrameworkCore;
using SupermarketSystem.Data;
using SupermarketSystem.Services;

namespace SupermarketSystem.Forms
{
    // Screen for viewing system reports - low stock, sales, categories, suppliers
    public class ReportsForm : Form
    {
        private readonly SupermarketContext _context;
        private readonly ProductService _productService;
        private readonly SaleService _saleService;

        private TabControl tabReports;
        private Button btnBack;

        public ReportsForm()
        {
            _context = new SupermarketContext();
            _productService = new ProductService(_context);
            _saleService = new SaleService(_context);
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Reports";
            this.Size = new Size(900, 620);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var lblTitle = new Label
            {
                Text = "📊 Reports",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 15),
                Size = new Size(300, 35)
            };

            // Tab control holds all report types as separate tabs
            tabReports = new TabControl
            {
                Location = new Point(20, 60),
                Size = new Size(845, 490),
                Font = new Font("Segoe UI", 9)
            };

            tabReports.TabPages.Add(BuildLowStockTab());
            tabReports.TabPages.Add(BuildSalesTab());
            tabReports.TabPages.Add(BuildCategoryTab());
            tabReports.TabPages.Add(BuildSupplierTab());

            btnBack = new Button
            {
                Text = "⬅ Back",
                Location = new Point(750, 558),
                Size = new Size(115, 36),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(100, 100, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, tabReports, btnBack });
        }

        // Report 1 - Products that have hit or passed their low stock threshold
        private TabPage BuildLowStockTab()
        {
            var tab = new TabPage("⚠ Low Stock");
            var dgv = BuildGrid(Color.FromArgb(178, 34, 34));

            var lowStock = _productService.GetLowStockProducts();
            dgv.DataSource = lowStock.Select(p => new
            {
                ID = p.ProductId,
                Title = p.Title,
                Barcode = p.Barcode,
                Stock = p.QuantityInStock,
                Threshold = p.LowStockThreshold,
                Category = p.Category?.Name ?? "N/A",
                Supplier = p.Supplier?.Name ?? "N/A"
            }).ToList();

            var lblCount = new Label
            {
                Text = $"{lowStock.Count} product(s) are low in stock.",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = lowStock.Count > 0 ? Color.Red : Color.Green,
                Location = new Point(5, 435),
                Size = new Size(500, 22)
            };

            tab.Controls.AddRange(new Control[] { dgv, lblCount });
            return tab;
        }

        // Report 2 - All recorded sales with totals
        private TabPage BuildSalesTab()
        {
            var tab = new TabPage("💰 Sales History");
            var dgv = BuildGrid(Color.FromArgb(60, 179, 113));

            var sales = _saleService.GetAllSales();
            dgv.DataSource = sales.Select(s => new
            {
                SaleID = s.SaleId,
                Date = s.SaleDate.ToString("dd/MM/yyyy HH:mm"),
                Items = s.SaleItems.Count,
                Total = s.TotalAmount.ToString("£0.00")
            }).ToList();

            var lblTotal = new Label
            {
                Text = $"Total revenue: {sales.Sum(s => s.TotalAmount):£0.00}  |  {sales.Count} sale(s) recorded.",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Location = new Point(5, 435),
                Size = new Size(700, 22)
            };

            tab.Controls.AddRange(new Control[] { dgv, lblTotal });
            return tab;
        }

        // Report 3 - Products grouped by category
        private TabPage BuildCategoryTab()
        {
            var tab = new TabPage("🗂️ Products by Category");
            var dgv = BuildGrid(Color.FromArgb(70, 130, 180));

            var products = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .OrderBy(p => p.Category != null ? p.Category.Name : "")
                .ToList();

            dgv.DataSource = products.Select(p => new
            {
                Category = p.Category?.Name ?? "N/A",
                Title = p.Title,
                Price = p.Price.ToString("£0.00"),
                Stock = p.QuantityInStock
            }).ToList();

            var lblCount = new Label
            {
                Text = $"{products.Count} product(s) across all categories.",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Location = new Point(5, 435),
                Size = new Size(500, 22)
            };

            tab.Controls.AddRange(new Control[] { dgv, lblCount });
            return tab;
        }

        // Report 4 - Stock levels grouped by supplier
        private TabPage BuildSupplierTab()
        {
            var tab = new TabPage("🏭 Supplier Stock");
            var dgv = BuildGrid(Color.FromArgb(100, 149, 237));

            var products = _context.Products
                .Include(p => p.Supplier)
                .Include(p => p.Category)
                .OrderBy(p => p.Supplier != null ? p.Supplier.Name : "")
                .ToList();

            dgv.DataSource = products.Select(p => new
            {
                Supplier = p.Supplier?.Name ?? "N/A",
                Title = p.Title,
                Barcode = p.Barcode,
                Price = p.Price.ToString("£0.00"),
                Stock = p.QuantityInStock,
                Status = p.IsLowStock ? "⚠ Low" : "✅ OK"
            }).ToList();

            var lblCount = new Label
            {
                Text = $"{products.Count} product(s) across all suppliers.",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.DimGray,
                Location = new Point(5, 435),
                Size = new Size(500, 22)
            };

            tab.Controls.AddRange(new Control[] { dgv, lblCount });
            return tab;
        }

        // Shared grid style used across all report tabs
        private DataGridView BuildGrid(Color headerColor)
        {
            var dgv = new DataGridView
            {
                Location = new Point(5, 5),
                Size = new Size(830, 425),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor = headerColor;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgv.EnableHeadersVisualStyles = false;
            return dgv;
        }
    }
}