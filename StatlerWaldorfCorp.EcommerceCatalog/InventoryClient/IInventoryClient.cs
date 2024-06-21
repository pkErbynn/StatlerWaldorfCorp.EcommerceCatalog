using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StatlerWaldorfCorp.EcommerceCatalog.Models;

namespace StatlerWaldorfCorp.EcommerceCatalog.InventoryClient
{
    public interface IInventoryClient
    {
        Task<StockStatus> GetStockStatusAsync(int sku);
    }
}