using Microsoft.EntityFrameworkCore;
using SupermarketSystem.Data;
using SupermarketSystem.Models;
using SupermarketSystem.Services;

namespace SupermarketSystem.Forms
{
    // Main screen for viewing, searching, adding, editing and deleting products
    public class ProductsForm : Form
    {
        private readonly SupermarketContext _context;
        private readonly ProductService _productService;

        private Label lblTitle;
        private TextBox txtSearch;
        private ComboBox cmbSearchType;
        private Button btnSearch;
        private Button btnClear;
        private DataGridView dgvProducts;
        private Label lblLowStock;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private Label lblStatus;

        public ProductsForm()
        {
            _context = new SupermarketContext();
            _productService = new ProductService(_context);
            SetupUI();
            LoadProducts();
        }

        private void SetupUI()
        {
            this.Text = "Manage Products";
            this.Size = new Size(960, 660);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Page title
            lblTitle = new Label
            {
                Text = "📦 Product Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 15),
                Size = new Size(500, 40)
            };

            // Search controls
            var lblSearch = new Label
            {
                Text = "Search:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 68),
                Size = new Size(60, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtSearch = new TextBox
            {
                Location = new Point(85, 68),
                Size = new Size(260, 25),
                Font = new Font("Segoe UI", 10)
            };

            cmbSearchType = new ComboBox
            {
                Location = new Point(355, 68),
                Size = new Size(130, 25),
                Font = new Font("Segoe UI", 10),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbSearchType.Items.AddRange(new[] { "By Name", "By Barcode" });
            cmbSearchType.SelectedIndex = 0;

            btnSearch = CreateButton("🔍 Search", new Point(495, 66),
                new Size(100, 30), Color.FromArgb(70, 130, 180));

            btnClear = CreateButton("✖ Clear", new Point(605, 66),
                new Size(80, 30), Color.FromArgb(150, 150, 150));

            // Warning label for low stock items
            lblLowStock = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Red,
                Location = new Point(20, 103),
                Size = new Size(800, 20)
            };

            // Product table
            dgvProducts = new DataGridView
            {
                Location = new Point(20, 128),
                Size = new Size(905, 430),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9),
                GridColor = Color.LightGray
            };

            // Style header row
            dgvProducts.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
            dgvProducts.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvProducts.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvProducts.EnableHeadersVisualStyles = false;

            // Bottom action buttons
            btnAdd = CreateButton("➕ Add", new Point(20, 575), new Size(110, 38), Color.FromArgb(60, 179, 113));
            btnEdit = CreateButton("✏ Edit", new Point(140, 575), new Size(110, 38), Color.FromArgb(70, 130, 180));
            btnDelete = CreateButton("🗑 Delete", new Point(260, 575), new Size(110, 38), Color.FromArgb(178, 34, 34));
            btnBack = CreateButton("⬅ Back", new Point(800, 575), new Size(110, 38), Color.FromArgb(100, 100, 100));

            // Status bar for feedback
            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Green,
                Location = new Point(385, 583),
                Size = new Size(400, 20)
            };

            // Wire up events
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnBack.Click += btnBack_Click;

            // Allow pressing Enter in search box
            txtSearch.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter) btnSearch_Click(s, e);
            };

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblSearch, txtSearch, cmbSearchType,
                btnSearch, btnClear, lblLowStock, dgvProducts,
                btnAdd, btnEdit, btnDelete, btnBack, lblStatus
            });
        }

        // Helper to build consistently styled buttons
        private Button CreateButton(string text, Point location, Size size, Color color)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = size,
                Font = new Font("Segoe UI", 9),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        // Load products into the grid - optionally pass a filtered list
        private void LoadProducts(List<Product>? products = null)
        {
            var list = products ?? _productService.GetAllProducts();

            dgvProducts.DataSource = null;
            dgvProducts.Columns.Clear();

            // Project to anonymous type for clean column display
            dgvProducts.DataSource = list.Select(p => new
            {
                ID = p.ProductId,
                Title = p.Title,
                Barcode = p.Barcode,
                Price = p.Price.ToString("£0.00"),
                Stock = p.QuantityInStock,
                Category = p.Category?.Name ?? "N/A",
                Supplier = p.Supplier?.Name ?? "N/A",
                Expiry = p.ExpiryDate.ToString("dd/MM/yyyy"),
                Status = p.IsLowStock ? "⚠ Low Stock" : "✅ OK"
            }).ToList();

            // Highlight low stock rows in red
            foreach (DataGridViewRow row in dgvProducts.Rows)
            {
                if (row.Cells["Status"].Value?.ToString()?.StartsWith("⚠") == true)
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 220, 220);
            }

            // Show low stock warning count
            int lowCount = list.Count(p => p.IsLowStock);
            lblLowStock.Text = lowCount > 0
                ? $"⚠ Warning: {lowCount} product(s) are low in stock!"
                : "";

            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = $"{list.Count} product(s) loaded.";
        }

        private void btnSearch_Click(object? sender, EventArgs e)
        {
            string query = txtSearch.Text.Trim();

            // Show all if search box is empty
            if (string.IsNullOrEmpty(query)) { LoadProducts(); return; }

            if (cmbSearchType.SelectedItem?.ToString() == "By Barcode")
            {
                // Hash table lookup - O(1)
                var product = _productService.SearchByBarcode(query);
                if (product != null)
                {
                    // Attach category and supplier manually for display
                    product.Category = _context.Categories.Find(product.CategoryId);
                    product.Supplier = _context.Suppliers.Find(product.SupplierId);
                    LoadProducts(new List<Product> { product });
                }
                else
                {
                    LoadProducts(new List<Product>());
                    lblStatus.ForeColor = Color.Red;
                    lblStatus.Text = "No product found with that barcode.";
                }
            }
            else
            {
                // Linear search by name - O(n)
                var results = _productService.SearchByName(query);
                LoadProducts(results);
            }
        }

        private void btnClear_Click(object? sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadProducts(); // reset to full product list
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            var form = new ProductAddEditForm(_context);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadProducts();
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Product added successfully!";
            }
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to edit.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)dgvProducts.SelectedRows[0].Cells["ID"].Value;
            var product = _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .FirstOrDefault(p => p.ProductId == id);

            if (product != null)
            {
                var form = new ProductAddEditForm(_context, product);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadProducts();
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Product updated successfully!";
                }
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a product to delete.",
                    "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string title = dgvProducts.SelectedRows[0].Cells["Title"].Value?.ToString() ?? "this product";
            var confirm = MessageBox.Show($"Are you sure you want to delete '{title}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                int id = (int)dgvProducts.SelectedRows[0].Cells["ID"].Value;
                try
                {
                    _productService.DeleteProduct(id);
                    LoadProducts();
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Product deleted successfully!";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Delete Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnBack_Click(object? sender, EventArgs e)
        {
            this.Close(); // return to main menu
        }
    }
}