using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.EcommerceCatalog.InventoryClient;
using StatlerWaldorfCorp.EcommerceCatalog.Repository;

namespace StatlerWaldorfCorp.EcommerceCatalog.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly IInventoryClient _inventoryClient;

        public ProductsController(IProductRepository productRepository,
            IInventoryClient inventoryClient)
        {
            this._inventoryClient = inventoryClient;
            this._productRepository = productRepository;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return this.Ok(this._productRepository.All());
        }

        [HttpGet("{sku}")]
        public IActionResult GetProduct(int sku)
        {
            var product = new
            {
                Product = this._productRepository.Get(sku),
                // Status = this._inventoryClient.GetStockStatusAsync(sku).Result
                Status = this._inventoryClient.GetStockStatusWithRetryAsync(sku).Result
            };
            return this.Ok(product);
        }
    }
}

// SKU = "stock keeping unit" = unique identifier for product in inventory.