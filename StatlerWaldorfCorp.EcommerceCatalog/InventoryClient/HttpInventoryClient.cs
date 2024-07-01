using Newtonsoft.Json;
using StatlerWaldorfCorp.EcommerceCatalog.Models;
using StatlerWaldorfCorp.EcommerceCatalog.Policies;
using Steeltoe.Common.Discovery;
using Steeltoe.Discovery;

namespace StatlerWaldorfCorp.EcommerceCatalog.InventoryClient
{
    public class HttpInventoryClient: IInventoryClient
    {
        private DiscoveryHttpClientHandler discoveryHttpClientHandler;
        private ClientPolicy clientPolicy;
        private const string STOCKSERVICE_URL_BASE = "http://inventory/api/skustatus/";

        public HttpInventoryClient(IDiscoveryClient discoveryClient, ClientPolicy clientPolicy)
        {
            this.discoveryHttpClientHandler = new DiscoveryHttpClientHandler(discoveryClient);
            this.clientPolicy = clientPolicy;
        }

        private HttpClient CreateHttpClient()
        {
            return new HttpClient(this.discoveryHttpClientHandler, false);
        }

        public async Task<StockStatus> GetStockStatusWithRetryAsync(int sku)
        {
            var url = $"{STOCKSERVICE_URL_BASE}{sku}";
            url = "http://localhost:8080/api/skustatus/";
            StockStatus stockStatus = null;

            using (HttpClient client = CreateHttpClient())
            {
                var response = await this.clientPolicy.ExponentialRetryHttp.ExecuteAsync(() => client.GetAsync(url));
                if(response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();   // Read the response content as a string
                    stockStatus = JsonConvert.DeserializeObject<StockStatus>(content);
                }
            }

            return stockStatus;
        }

        public async Task<StockStatus> GetStockStatusAsync(int sku)
        {
            var url = $"{STOCKSERVICE_URL_BASE}{sku}";;
            StockStatus stockStatus = null;

            using (HttpClient client = CreateHttpClient())
            {
                var result = await client.GetStringAsync(url);
                stockStatus = JsonConvert.DeserializeObject<StockStatus>(result);
            }

            return stockStatus;
        }
    }
}