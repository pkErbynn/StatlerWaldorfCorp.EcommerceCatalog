using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatlerWaldorfCorp.EcommerceCatalog.Models;

namespace StatlerWaldorfCorp.EcommerceCatalog.Repository
{
    public class MemoryProductRepository: IProductRepository
    {
        private Dictionary<int, Product> products;

        public MemoryProductRepository()
        {
            products = new Dictionary<int, Product>();

            products.Add(123, new Product
            {
                SKU = 123,
                Name = "The Magic 123"
            });

            products.Add(456, new Product
            {
                SKU = 456,
                Name = "Supervac"
            });
        }
        public ICollection<Product> All()
        {
            return products.Values;
        }

        public Product Get(int sku)
        {
            return products[sku];
        }
    }
}