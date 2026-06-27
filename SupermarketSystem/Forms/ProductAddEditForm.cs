using SupermarketSystem.Data;
using SupermarketSystem.Models;
using SupermarketSystem.Services;

namespace SupermarketSystem.Forms
{
    // Used for both adding a new product and editing an existing one
    public class ProductAddEditForm : Form
    {
        private readonly SupermarketContext _context;
        private readonly ProductService _productService;
        private readonly Product? _existingProduct;

        private TextBox txtTitle;
        private TextBox txtBarcode;
        private TextBox txtPrice;
        private NumericUpDown nudQuantity;
        private NumericUpDown nudThreshold;
        private DateTimePicker dtpExpiry;
        private ComboBox cmbCategory;
        private ComboBox cmbSupplier;
        private Button btnSave;
        private Button btnCancel;

        // Constructor for adding
        public ProductAddEditForm(SupermarketContext context)
        {
            _context = context;
            _productService = new ProductService(context);
            _existingProduct = null;
            SetupUI();
            LoadDropdowns();
        }

        // Constructor for editing - takes existing product to pre-fill fields
        public ProductAddEditForm(SupermarketContext context, Product product)
        {
            _context = context;
            _productService = new ProductService(context);
            _existingProduct = product;
            SetupUI();
            LoadDropdowns();
            PopulateFields();
        }

        private void SetupUI()
        {
            this.Text = _existingProduct == null ? "Add New Product" : "Edit Product";
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var lblTitle = new Label
            {
                Text = _existingProduct == null ? "➕ Add New Product" : "✏ Edit Product",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 15),
                Size = new Size(380, 35)
            };
            this.Controls.Add(lblTitle);

            // Build each label + input pair at increasing Y positions
            int y = 65;

            void AddField(string labelText, Control input)
            {
                var lbl = new Label
                {
                    Text = labelText,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(20, y),
                    Size = new Size(155, 22),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                input.Location = new Point(185, y - 2);
                input.Size = new Size(210, 25);
                input.Font = new Font("Segoe UI", 9);
                this.Controls.Add(lbl);
                this.Controls.Add(input);
                y += 48;
            }

            txtTitle = new TextBox();
            txtBarcode = new TextBox();
            txtPrice = new TextBox();
            nudQuantity = new NumericUpDown { Minimum = 0, Maximum = 99999 };
            nudThreshold = new NumericUpDown { Minimum = 1, Maximum = 1000, Value = 5 };
            dtpExpiry = new DateTimePicker { Format = DateTimePickerFormat.Short, MinDate = DateTime.Today };
            cmbCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            cmbSupplier = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

            AddField("Product Title *", txtTitle);
            AddField("Barcode *", txtBarcode);
            AddField("Price (£) *", txtPrice);
            AddField("Quantity in Stock", nudQuantity);
            AddField("Low Stock Alert At", nudThreshold);
            AddField("Expiry Date", dtpExpiry);
            AddField("Category *", cmbCategory);
            AddField("Supplier *", cmbSupplier);

            // Save and Cancel buttons
            btnSave = new Button
            {
                Text = "💾 Save",
                Location = new Point(80, y + 10),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(60, 179, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnCancel = new Button
            {
                Text = "✖ Cancel",
                Location = new Point(220, y + 10),
                Size = new Size(120, 40),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.FromArgb(178, 34, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };

            btnSave.Click += btnSave_Click;
            this.Controls.AddRange(new Control[] { btnSave, btnCancel });
            this.ClientSize = new Size(425, y + 70);
        }

        // Fill dropdowns from database
        private void LoadDropdowns()
        {
            var categories = _context.Categories.ToList();
            cmbCategory.DataSource = categories;
            cmbCategory.DisplayMember = "Name";
            cmbCategory.ValueMember = "CategoryId";

            var suppliers = _context.Suppliers.ToList();
            cmbSupplier.DataSource = suppliers;
            cmbSupplier.DisplayMember = "Name";
            cmbSupplier.ValueMember = "SupplierId";
        }

        // Pre-fill fields when editing an existing product
        private void PopulateFields()
        {
            if (_existingProduct == null) return;

            txtTitle.Text = _existingProduct.Title;
            txtBarcode.Text = _existingProduct.Barcode;
            txtPrice.Text = _existingProduct.Price.ToString("0.00");
            nudQuantity.Value = _existingProduct.QuantityInStock;
            nudThreshold.Value = _existingProduct.LowStockThreshold;
            dtpExpiry.Value = _existingProduct.ExpiryDate < DateTime.Today
                ? DateTime.Today : _existingProduct.ExpiryDate;
            cmbCategory.SelectedValue = _existingProduct.CategoryId;
            cmbSupplier.SelectedValue = _existingProduct.SupplierId;
        }

        private void btnSave_Click(object? sender, EventArgs e)
        {
            // Validate required fields before saving
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Please enter a product title.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTitle.Focus(); return;
            }

            if (string.IsNullOrWhiteSpace(txtBarcode.Text))
            {
                MessageBox.Show("Please enter a barcode.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBarcode.Focus(); return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Please enter a valid price greater than zero.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPrice.Focus(); return;
            }

            if (cmbCategory.SelectedValue == null || cmbSupplier.SelectedValue == null)
            {
                MessageBox.Show("Please add at least one Category and one Supplier first,\nthen come back to add products.",
                    "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var product = new Product
                {
                    ProductId = _existingProduct?.ProductId ?? 0,
                    Title = txtTitle.Text.Trim(),
                    Barcode = txtBarcode.Text.Trim(),
                    Price = price,
                    QuantityInStock = (int)nudQuantity.Value,
                    LowStockThreshold = (int)nudThreshold.Value,
                    ExpiryDate = dtpExpiry.Value,
                    CategoryId = (int)cmbCategory.SelectedValue,
                    SupplierId = (int)cmbSupplier.SelectedValue
                };

                if (_existingProduct == null)
                    _productService.AddProduct(product);   // new product
                else
                    _productService.UpdateProduct(product); // update existing

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product:\n{ex.Message}", "Save Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}