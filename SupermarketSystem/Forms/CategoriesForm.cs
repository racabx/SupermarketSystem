using SupermarketSystem.Data;
using SupermarketSystem.Models;
using SupermarketSystem.Services;

namespace SupermarketSystem.Forms
{
    // Screen for managing product categories
    public class CategoriesForm : Form
    {
        private readonly SupermarketContext _context;
        private readonly CategoryService _categoryService;

        private Label lblTitle;
        private ListBox lstCategories;
        private TextBox txtCategoryName;
        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnBack;
        private Label lblStatus;

        public CategoriesForm()
        {
            _context = new SupermarketContext();
            _categoryService = new CategoryService(_context);
            SetupUI();
            LoadCategories();
        }

        private void SetupUI()
        {
            this.Text = "Manage Categories";
            this.Size = new Size(480, 560);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            lblTitle = new Label
            {
                Text = "🗂️ Category Management",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                Location = new Point(20, 15),
                Size = new Size(420, 35)
            };

            // Label above the list
            var lblList = new Label
            {
                Text = "Existing Categories:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 60),
                Size = new Size(200, 22)
            };

            // Scrollable list of categories
            lstCategories = new ListBox
            {
                Location = new Point(20, 85),
                Size = new Size(420, 250),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Input for new or edited category name
            var lblName = new Label
            {
                Text = "Category Name:",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(20, 350),
                Size = new Size(130, 22),
                TextAlign = ContentAlignment.MiddleLeft
            };

            txtCategoryName = new TextBox
            {
                Location = new Point(155, 348),
                Size = new Size(285, 25),
                Font = new Font("Segoe UI", 10)
            };

            // Action buttons
            btnAdd = CreateButton("➕ Add", new Point(20, 410), Color.FromArgb(60, 179, 113));
            btnEdit = CreateButton("✏ Edit", new Point(145, 410), Color.FromArgb(70, 130, 180));
            btnDelete = CreateButton("🗑 Delete", new Point(270, 410), Color.FromArgb(178, 34, 34));
            btnBack = CreateButton("⬅ Back", new Point(20, 460), Color.FromArgb(100, 100, 100));

            lblStatus = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Green,
                Location = new Point(145, 468),
                Size = new Size(300, 20)
            };

            btnAdd.Click += btnAdd_Click;
            btnEdit.Click += btnEdit_Click;
            btnDelete.Click += btnDelete_Click;
            btnBack.Click += (s, e) => this.Close();

            // Clicking a list item copies name to the text box for easy editing
            lstCategories.SelectedIndexChanged += (s, e) =>
            {
                if (lstCategories.SelectedItem != null)
                    txtCategoryName.Text = lstCategories.SelectedItem.ToString();
            };

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblList, lstCategories,
                lblName, txtCategoryName,
                btnAdd, btnEdit, btnDelete, btnBack, lblStatus
            });
        }

        private Button CreateButton(string text, Point location, Color color)
        {
            return new Button
            {
                Text = text,
                Location = location,
                Size = new Size(110, 36),
                Font = new Font("Segoe UI", 9),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
        }

        // Refresh the list from the database
        private void LoadCategories()
        {
            lstCategories.Items.Clear();
            var categories = _categoryService.GetAllCategories();
            foreach (var c in categories)
                lstCategories.Items.Add(c.Name);

            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = $"{categories.Count} category/categories loaded.";
        }

        private void btnAdd_Click(object? sender, EventArgs e)
        {
            string name = txtCategoryName.Text.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Please enter a category name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                _categoryService.AddCategory(new Category { Name = name });
                txtCategoryName.Clear();
                LoadCategories();
                lblStatus.ForeColor = Color.Green;
                lblStatus.Text = $"'{name}' added successfully!";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEdit_Click(object? sender, EventArgs e)
        {
            if (lstCategories.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a category to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string newName = txtCategoryName.Text.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show("Please enter a new name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Find the category by its current name to get the ID
                var all = _categoryService.GetAllCategories();
                string oldName = lstCategories.SelectedItem!.ToString()!;
                var category = all.FirstOrDefault(c => c.Name == oldName);

                if (category != null)
                {
                    category.Name = newName;
                    _categoryService.UpdateCategory(category);
                    txtCategoryName.Clear();
                    LoadCategories();
                    lblStatus.ForeColor = Color.Green;
                    lblStatus.Text = "Category updated successfully!";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object? sender, EventArgs e)
        {
            if (lstCategories.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a category to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string name = lstCategories.SelectedItem!.ToString()!;
            var confirm = MessageBox.Show($"Delete category '{name}'?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                try
                {
                    var all = _categoryService.GetAllCategories();
                    var category = all.FirstOrDefault(c => c.Name == name);

                    if (category != null)
                    {
                        _categoryService.DeleteCategory(category.CategoryId);
                        txtCategoryName.Clear();
                        LoadCategories();
                        lblStatus.ForeColor = Color.Green;
                        lblStatus.Text = "Category deleted.";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}