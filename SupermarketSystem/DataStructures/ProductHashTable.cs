using SupermarketSystem.Models;

namespace SupermarketSystem.DataStructures
{
    // Custom hash table for fast barcode lookups - no built-in collections used
    public class ProductHashTable
    {
        private const int TableSize = 200; // fixed number of slots
        private string?[] _keys;
        private Product?[] _values;
        private bool[] _deleted; // tracks deleted slots for linear probing

        public ProductHashTable()
        {
            _keys = new string[TableSize];
            _values = new Product[TableSize];
            _deleted = new bool[TableSize];
        }

        // Converts barcode string into an array index
        private int GetHash(string key)
        {
            int hash = 0;
            foreach (char c in key)
                hash = (hash * 31 + c) % TableSize;
            return Math.Abs(hash);
        }

        // Insert product by barcode - O(1) average case
        public void Insert(string barcode, Product product)
        {
            int index = GetHash(barcode);
            int start = index;

            // Linear probing to handle collisions
            while (_keys[index] != null && _keys[index] != barcode && !_deleted[index])
            {
                index = (index + 1) % TableSize;
                if (index == start)
                    throw new Exception("Hash table is full.");
            }

            _keys[index] = barcode;
            _values[index] = product;
            _deleted[index] = false;
        }

        // Search by barcode - O(1) average case
        public Product? Search(string barcode)
        {
            int index = GetHash(barcode);
            int start = index;

            while (_keys[index] != null || _deleted[index])
            {
                if (_keys[index] == barcode && !_deleted[index])
                    return _values[index];

                index = (index + 1) % TableSize;
                if (index == start) break;
            }
            return null; // not found
        }

        // Mark slot as deleted without breaking the probing chain
        public void Delete(string barcode)
        {
            int index = GetHash(barcode);
            int start = index;

            while (_keys[index] != null || _deleted[index])
            {
                if (_keys[index] == barcode && !_deleted[index])
                {
                    _deleted[index] = true;
                    _keys[index] = null;
                    _values[index] = null;
                    return;
                }
                index = (index + 1) % TableSize;
                if (index == start) break;
            }
        }

        // Load all existing products into the table on startup
        public void LoadProducts(List<Product> products)
        {
            foreach (var product in products)
                if (!string.IsNullOrEmpty(product.Barcode))
                    Insert(product.Barcode, product);
        }
    }
}