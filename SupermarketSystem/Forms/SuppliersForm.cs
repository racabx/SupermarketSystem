using SupermarketSystem.Data;
using SupermarketSystem.Models;
using SupermarketSystem.Services;

namespace SupermarketSystem.Forms
{
    // Screen for managing suppliers who provide stock to the supermarket
    public class SuppliersForm : Form
    {
        private readonly SupermarketContext _context;
        private readonly SupplierService _supplierService;

        private DataGridView dgvSuppliers;
        private TextBox txtName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private Label lblStatus;

        public SuppliersForm()
        {
            _context = new SupermarketContext();
            _supplierService = new SupplierService(_context);
            SetupUI();
            LoadSuppliers();
        }

        private void SetupUI()
        {
            this.Text = "Manage Suppliers";
            this.Size = new Size(700, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            var lblTitle = new Label
            {
                Text = "🏭 Supplier Management",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 15),
                Size = new Size(400, 35)
            };

            // Supplier table
            dgvSuppliers = new DataGridView
            {
                Location = new Point(20, 60),
                Size = new Size(645, 250),
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

            dgvSuppliers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(70, 130, 180);
            dgvSuppliers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSuppliers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            dgvSuppliers.EnableHeadersVisualStyles = false;

            // Clicking a row fills the input fields below
            dgvSuppliers.SelectionChanged += DgvSuppliers_SelectionChanged;

            // Input fields for add / edit
            int y = 325;
            void AddField(string label, TextBox box)
            {
                var lbl = new Label
                {
                    Text = label,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(20, y),
                    Size = new Size(130, 24),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                box.Location = new Point(160, y);
                box.Size = new Size(505, 25);
                box.Font = new Font("Segoe UI", 9);
                this.Controls.AddRange(new Control[] { lbl, box });
                y += 40;
            }

            txtName = new TextBox();
            txtEmail = new TextBox();
            txtPhone = new TextBox();

            AddField("Supplier Name *", txtName);
            AddField("Contact Email", txtEmail);
            AddField("Phone Number", txtPhone);

            // Action buttons
            btnAdd = CreateButton("➕ Add", new Point(20, y + 10), Color.FromArgb(60, 179, 113));
            btnEdit = CreateButton("✏ Edit", new Point(150, y + 10), Color.FromArgb(70, 130, 180));
            btnDelete = CreateButton("🗑 Delete", new Point(280, y + 10), Color.FromArgb(178, 34, 34));
            btnBack = CreateButton("⬅ Back", new Point(545, y + 10), Color.FromArgb(100, 100, 100));

            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Green,
                Location = new Point(20, y + 58),
                Size = new Size(660, 20)
            };

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnBack.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[]
            {
                lblTitle, dgvSuppliers,
                btnAdd, btnEdit, btnDelete, btnBack, lblStatus
            });
        }

        private Button CreateButton(string text, Point location, Color color)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(115, 36),
                Font = new Font("Segoe UI", 9),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        // Fill input fields when a row is selected
        private void DgvSuppliers_SelectionChanged(object? sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count == 0) return;
            var row = dgvSuppliers.SelectedRows[0];
            txtName.Text = row.Cells["Name"].Value?.ToString() ?? "";
            txtEmail.Text = row.Cells["Email"].Value?.ToString() ?? "";
            txtPhone.Text = row.Cells["Phone"].Value?.ToString() ?? "";
        }

        private void LoadSuppliers()
        {
            var suppliers = _supplierService.GetAllSuppliers();
            dgvSuppliers.DataSource = suppliers.Select(s => new
            {
                ID = s.SupplierId,
                Name = s.Name,
                Email = s.ContactEmail,
                Phone = s.Phone
            }).ToList();

            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = $"{suppliers.Count} supplier(s) loaded.";
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Supplier name is required.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _supplierService.AddSupplier(new Supplier
                {
                    Name = txtName.Text.Trim(),
                    ContactEmail = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim()
                });
                ClearFields();
                LoadSuppliers();
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Supplier added successfully!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int id = (int)dgvSuppliers.SelectedRows[0].Cells["ID"].Value;

            try
            {
                _supplierService.UpdateSupplier(new Supplier
                {
                    SupplierId = id,
                    Name = txtName.Text.Trim(),
                    ContactEmail = txtEmail.Text.Trim(),
                    Phone = txtPhone.Text.Trim()
                });
                ClearFields();
                LoadSuppliers();
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = "Supplier updated successfully!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvSuppliers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a supplier to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string name = dgvSuppliers.SelectedRows[0].Cells["Name"].Value?.ToString() ?? "this supplier";
            var confirm = MessageBox.Show($"Delete supplier '{name}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                int id = (int)dgvSuppliers.SelectedRows[0].Cells["ID"].Value;
                try
                {
                    _supplierService.DeleteSupplier(id);
                    ClearFields();
                    LoadSuppliers();
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Supplier deleted.";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Clear input fields after an action
        private void ClearFields()
        {
            txtName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
        }
    }
}