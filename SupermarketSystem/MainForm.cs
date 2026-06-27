using SupermarketSystem.Data;

namespace SupermarketSystem
{
    public partial class MainForm : Form
    {
        private readonly SupermarketContext _context;

        // Controls
        private Label lblTitle;
        private Button btnProducts;
        private Button btnSuppliers;
        private Button btnCategories;
        private Button btnSales;
        private Button btnReports;
        private Button btnExit;

        public MainForm()
        {
            InitializeComponent();
            _context = new SupermarketContext();
            SetupUI();
        }

        private void SetupUI()
        {
            // Form settings
            this.Text = "Supermarket Management System";
            this.Size = new Size(500, 550);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.WhiteSmoke;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Title label
            lblTitle = new Label
            {
                Text = "🛒 Supermarket Management System",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(34, 139, 34),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 50),
                Location = new Point(20, 20)
            };

            // Helper to create buttons consistently
            Button CreateButton(string text, Point location, Color color)
            {
                return new Button
                {
                    Text = text,
                    Size = new Size(340, 50),
                    Location = location,
                    Font = new Font("Segoe UI", 12, FontStyle.Regular),
                    BackColor = color,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand
                };
            }

            btnProducts = CreateButton("📦  Manage Products", new Point(80, 100), Color.FromArgb(70, 130, 180));
            btnSuppliers = CreateButton("🏭  Manage Suppliers", new Point(80, 165), Color.FromArgb(70, 130, 180));
            btnCategories = CreateButton("🗂️  Manage Categories", new Point(80, 230), Color.FromArgb(70, 130, 180));
            btnSales = CreateButton("💰  Record a Sale", new Point(80, 295), Color.FromArgb(60, 179, 113));
            btnReports = CreateButton("📊  View Reports", new Point(80, 360), Color.FromArgb(60, 179, 113));
            btnExit = CreateButton("✖   Exit", new Point(80, 440), Color.FromArgb(178, 34, 34));

            // Wire up click events
            btnProducts.Click += btnProducts_Click;
            btnSuppliers.Click += btnSuppliers_Click;
            btnCategories.Click += btnCategories_Click;
            btnSales.Click += btnSales_Click;
            btnReports.Click += btnReports_Click;
            btnExit.Click += btnExit_Click;

            // Add all controls to form
            this.Controls.AddRange(new Control[]
            {
                lblTitle, btnProducts, btnSuppliers,
                btnCategories, btnSales, btnReports, btnExit
            });
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            // Open the products management screen
            var form = new SupermarketSystem.Forms.ProductsForm();
            form.ShowDialog();
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            // Open supplier management screen
            var form = new SupermarketSystem.Forms.SuppliersForm();
            form.ShowDialog();
        }

        private void btnCategories_Click(object sender, EventArgs e)
        {
            // Open category management screen
            var form = new SupermarketSystem.Forms.CategoriesForm();
            form.ShowDialog();
        }

        private void btnSales_Click(object sender, EventArgs e)
        {
            // Open the sales recording screen
            var form = new SupermarketSystem.Forms.SalesForm();
            form.ShowDialog();
        }

        private void btnReports_Click(object sender, EventArgs e)
        {
            // Open the reports screen
            var form = new SupermarketSystem.Forms.ReportsForm();
            form.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}