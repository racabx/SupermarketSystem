using SupermarketSystem.Data;
using SupermarketSystem.Models;

namespace SupermarketSystem.Services
{
    // Handles all supplier operations - suppliers provide products to the shop
    public class SupplierService
    {
        private readonly SupermarketContext _context;

        public SupplierService(SupermarketContext context)
        {
            _context = context;
        }

        // Return all suppliers sorted by name
        public List<Supplier> GetAllSuppliers()
        {
            return _context.Suppliers
                .OrderBy(s => s.Name)
                .ToList();
        }

        // Add supplier with name, email and phone validation
        public void AddSupplier(Supplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.Name))
                throw new Exception("Supplier name cannot be empty.");

            if (_context.Suppliers.Any(s => s.Name.ToLower() == supplier.Name.ToLower()))
                throw new Exception("A supplier with this name already exists.");

            _context.Suppliers.Add(supplier);
            _context.SaveChanges();
        }

        // Update supplier details
        public void UpdateSupplier(Supplier supplier)
        {
            var existing = _context.Suppliers.Find(supplier.SupplierId);
            if (existing == null)
                throw new Exception("Supplier not found.");

            existing.Name = supplier.Name;
            existing.ContactEmail = supplier.ContactEmail;
            existing.Phone = supplier.Phone;
            _context.SaveChanges();
        }

        // Only delete if no products are linked to this supplier
        public void DeleteSupplier(int supplierId)
        {
            var supplier = _context.Suppliers.Find(supplierId);
            if (supplier == null)
                throw new Exception("Supplier not found.");

            bool hasProducts = _context.Products.Any(p => p.SupplierId == supplierId);
            if (hasProducts)
                throw new Exception("Cannot delete — products are linked to this supplier.");

            _context.Suppliers.Remove(supplier);
            _context.SaveChanges();
        }
    }
}