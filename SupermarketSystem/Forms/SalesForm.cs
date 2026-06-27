using SupermarketSystem.Data;
using SupermarketSystem.Models;
using SupermarketSystem.Services;

namespace SupermarketSystem.Forms
{
    // Screen for recording new sales - staff select products and quantities
    public class SalesForm : Form
    {
        private readonly SupermarketContext _context;
        private readonly SaleService _saleService;

        // Basket holds items being added to the current sale
        private List<SaleItem> _basket = new();

        private ComboBox cmbProducts;
        private NumericUpDown nudQuantity;
        private Button btnAddToBasket;
        private DataGridView dgvBasket;
        private Button btnRemoveItem;
        private Label lblTotal;
        private Button btnCompleteSale;
        private Button btnClearBasket;
        private Button btnBack;
        private Label lblStatus;

        public SalesForm()
        {
            _context = new SupermarketContext();
            _saleService = new SaleService(_context);
            SetupUI();
            LoadProducts();
        }

        private void SetupUI()
        {
            this.Text = "Record a Sale";
            this.Size = new Size(750, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var lblTitle = new Label
            {
                Text = "💰 Record a Sale",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 15),
                Size = new Size(400, 35)
            };

            // Product selector
            var lblProduct = new Label
            {
                Text = "Select Product:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 65),
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            cmbProducts = new ComboBox
            {
                Location = new Point(145, 65),
                Size = new Size(340, 25),
                Font = new Font("Segoe UI", 9),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            // Quantity selector
            var lblQty = new Label
            {
                Text = "Quantity:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(495, 65),
                Size = new Size(70, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            nudQuantity = new NumericUpDown
            {
                Location = new Point(570, 65),
                Size = new Size(70, 25),
                Font = new Font("Segoe UI", 9),
                Minimum = 1,
                Maximum = 9999,
                Value = 1
            };

            btnAddToBasket = new Button
            {
                Text = "➕ Add to Basket",
                Location = new Point(20, 105),
                Size = new Size(160, 36),
                Font = new Font("Segoe UI", 9),
                BackColor = Color.FromArgb(70, 130, 180),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            // Basket grid showing items in current sale
            var lblBasket = new Label
            {
                Text = "Current Sale Basket:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 155),
                Size = new Size(200, 22)
            };

            dgvBasket = new DataGridView
            {
                Location = new Point(20, 180),
                Size = new Size(695, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9)
            };

            dgvBasket.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(60, 179, 113);
            dgvBasket.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvBasket.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvBasket.EnableHeadersVisualStyles = false;

            // Total amount label
            lblTotal = new Label
            {
                Text = "Total: £0.00",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 472),
                Size = new Size(250, 30)
            };

            // Bottom action buttons
            btnRemoveItem = CreateButton("🗑 Remove Item", new Point(20, 515), new Size(150, 38), Color.FromArgb(178, 34, 34));
            btnCompleteSale = CreateButton("✅ Complete Sale", new Point(185, 515), new Size(155, 38), Color.FromArgb(60, 179, 113));
            btnClearBasket = CreateButton("🔄 Clear Basket", new Point(355, 515), new Size(150, 38), Color.FromArgb(150, 150, 150));
            btnBack = CreateButton("⬅ Back", new Point(590, 515), new Size(120, 38), Color.FromArgb(100, 100, 100));

            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Green,
                Location = new Point(20, 562),
                Size = new Size(700, 20)
            };

            btnAddToBasket.Click += btnAddToBasket_Click;
            btnRemoveItem.Click += btnRemoveItem_Click;
            btnCompleteSale.Click += btnCompleteSale_Click;
            btnClearBasket.Click += btnClearBasket_Click;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblProduct, cmbProducts, lblQty, nudQuantity,
                btnAddToBasket, lblBasket, dgvBasket,
                lblTotal, btnRemoveItem, btnCompleteSale,
                btnClearBasket, btnBack, lblStatus
            });
        }

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

        // Populate the product dropdown from database
        private void LoadProducts()
        {
            var products = _saleService.GetAvailableProducts();
            cmbProducts.DataSource = products;
            cmbProducts.DisplayMember = "Title";
            cmbProducts.ValueMember = "ProductId";
        }

        // Refresh the basket grid and recalculate total
        private void RefreshBasket()
        {
            dgvBasket.DataSource = null;
            dgvBasket.DataSource = _basket.Select(i => new
            {
                ProductID = i.ProductId,
                Product = i.Product?.Title ?? "",
                Qty = i.Quantity,
                UnitPrice = i.UnitPrice.ToString("£0.00"),
                Subtotal = (i.Quantity * i.UnitPrice).ToString("£0.00")
            }).ToList();

            // Show running total at bottom
            decimal total = _basket.Sum(i => i.Quantity * i.UnitPrice);
            lblTotal.Text = $"Total: {total:£0.00}";
        }

        private void btnAddToBasket_Click(object? sender, EventArgs e)
        {
            if (cmbProducts.SelectedItem is not Product selected)
            {
                MessageBox.Show("Please select a product.", "No Product",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qty = (int)nudQuantity.Value;

            // If product already in basket, increase quantity instead of duplicating
            var existing = _basket.FirstOrDefault(i => i.ProductId == selected.ProductId);
            if (existing != null)
            {
                existing.Quantity += qty;
            }
            else
            {
                _basket.Add(new SaleItem
                {
                    ProductId = selected.ProductId,
                    Product = selected,
                    Quantity = qty,
                    UnitPrice = selected.Price
                });
            }

            RefreshBasket();
            nudQuantity.Value = 1;
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = $"'{selected.Title}' added to basket.";
        }

        private void btnRemoveItem_Click(object? sender, EventArgs e)
        {
            if (dgvBasket.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to remove.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int productId = (int)dgvBasket.SelectedRows[0].Cells["ProductID"].Value;
            var item = _basket.FirstOrDefault(i => i.ProductId == productId);
            if (item != null) _basket.Remove(item);

            RefreshBasket();
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = "Item removed from basket.";
        }

        private void btnCompleteSale_Click(object? sender, EventArgs e)
        {
            if (_basket.Count == 0)
            {
                MessageBox.Show("Basket is empty. Add products before completing a sale.",
                    "Empty Basket", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal total = _basket.Sum(i => i.Quantity * i.UnitPrice);
            var confirm = MessageBox.Show(
                $"Complete sale for {total:£0.00}?\nThis will update stock levels.",
                "Confirm Sale", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    var sale = _saleService.RecordSale(_basket);
                    MessageBox.Show(
                        $"✅ Sale recorded!\nTotal: {sale.TotalAmount:£0.00}\nDate: {sale.SaleDate:dd/MM/yyyy HH:mm}",
                        "Sale Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reset basket and reload product stock
                    _basket.Clear();
                    LoadProducts();
                    RefreshBasket();
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Sale completed and stock updated.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Sale Failed",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnClearBasket_Click(object? sender, EventArgs e)
        {
            _basket.Clear();
            RefreshBasket();
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = "Basket cleared.";
        }
    }
}