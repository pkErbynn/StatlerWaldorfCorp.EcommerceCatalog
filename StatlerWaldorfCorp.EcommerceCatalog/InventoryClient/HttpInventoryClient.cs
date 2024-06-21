using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StatlerWaldorfCorp.EcommerceCatalog.Models;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;

namespace StatlerWaldorfCorp.EcommerceCatalog.InventoryClient
{
    public class HttpInventoryClient: IInventoryClient
    {
        private DiscoveryHttpClientHandler discoveryHttpClientHandler;
        private const string STOCKSERVICE_URL_BASE = "http://inventory/api/skustatus/";

        public HttpInventoryClient(IDiscoveryClient discoveryClient)
        {
            this.discoveryHttpClientHandler = new DiscoveryHttpClientHandler(discoveryClient);
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient(this.discoveryHttpClientHandler, false);
        }

        public async Task<StockStatus> GetStockStatusAsync(int sku)
        {
            StockStatus stockStatus = null;

            using (HttpClient client = CreateHttpClient())
            {
                var result = await client.GetStringAsync(STOCKSERVICE_URL_BASE + sku.ToString());
                stockStatus = JsonConvert.DeserializeObject<StockStatus>(result);
            }

            return stockStatus;
        }
    }
}