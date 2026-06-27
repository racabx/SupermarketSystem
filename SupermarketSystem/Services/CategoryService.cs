using SupermarketSystem.Data;
using SupermarketSystem.Models;

namespace SupermarketSystem.Services
{
    // Handles all category operations - categories group products together
    public class CategoryService
    {
        private readonly SupermarketContext _context;

        public CategoryService(SupermarketContext context)
        {
            _context = context;
        }

        // Get all categories sorted alphabetically
        public List<Category> GetAllCategories()
        {
            return _context.Categories
                .OrderBy(c => c.Name)
                .ToList();
        }

        // Add a new category with basic validation
        public void AddCategory(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                throw new Exception("Category name cannot be empty.");

            // Prevent duplicate category names
            if (_context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower()))
                throw new Exception("A category with this name already exists.");

            _context.Categories.Add(category);
            _context.SaveChanges();
        }

        // Update category name
        public void UpdateCategory(Category category)
        {
            var existing = _context.Categories.Find(category.CategoryId);
            if (existing == null)
                throw new Exception("Category not found.");

            if (_context.Categories.Any(c =>
                c.Name.ToLower() == category.Name.ToLower() &&
                c.CategoryId != category.CategoryId))
                throw new Exception("A category with this name already exists.");

            existing.Name = category.Name;
            _context.SaveChanges();
        }

        // Delete only if no products are linked to this category
        public void DeleteCategory(int categoryId)
        {
            var category = _context.Categories.Find(categoryId);
            if (category == null)
                throw new Exception("Category not found.");

            bool hasProducts = _context.Products.Any(p => p.CategoryId == categoryId);
            if (hasProducts)
                throw new Exception("Cannot delete — products are linked to this category.");

            _context.Categories.Remove(category);
            _context.SaveChanges();
        }
    }
}