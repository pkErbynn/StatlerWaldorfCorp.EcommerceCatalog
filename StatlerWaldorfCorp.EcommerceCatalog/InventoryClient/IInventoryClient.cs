using StatlerWaldorfCorp.EcommerceCatalog.Models;

namespace StatlerWaldorfCorp.EcommerceCatalog.InventoryClient
{
    public interface IInventoryClient
    {
        Task<StockStatus> GetStockStatusAsync(int sku);
        Task<StockStatus> GetStockStatusWithRetryAsync(int sku);
        
    }
}