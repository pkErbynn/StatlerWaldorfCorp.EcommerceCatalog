using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatlerWaldorfCorp.EcommerceCatalog.Models;

namespace StatlerWaldorfCorp.EcommerceCatalog.Repository
{
    public interface IProductRepository
    {
        ICollection<Product> All();
        Product Get(int sku);
    }
}