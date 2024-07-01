using Polly;
using Polly.Retry;

namespace StatlerWaldorfCorp.EcommerceCatalog.Policies
{
    public class ClientPolicy
    {
        public AsyncRetryPolicy<HttpResponseMessage> ExponentialRetryHttp { get; }
        public ClientPolicy()
        {
            this.ExponentialRetryHttp = Policy.HandleResult<HttpResponseMessage>(response => !response.IsSuccessStatusCode)
                                        .WaitAndRetryAsync(5, retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)));
        }
    }
}