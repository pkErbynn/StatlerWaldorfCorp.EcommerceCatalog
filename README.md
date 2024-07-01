## Service Discovery

The Steeltoe Discovery Client package integrates Eureka with .NET applications, enabling service registration and discovery.

## High-level Architecture
![Architecture](./System%20Architecture%20Service%20discovery.png)

## Setting Up
Demonstration of creating a resilient and discoverable microservices architecture using .NET 6, Steeltoe, and Eureka. 

This setup allows services to dynamically discover and communicate with each other, enhancing scalability and resilience.

### Setting Up Eureka Server
Extending from the Inventory Service here - https://github.com/pkErbynn/StatlerWaldorfCorp.EcommerceInventory
  - where Eureka server is set up

1. **Configure Eureka in appsettings.json**
```json
{
  "eureka": {
    "client": {
      "serviceUrl": "http://localhost:8761/eureka/",
      "shouldRegisterWithEureka":false

    },
    "instance": {
      "appName": "catalog",
    }
  }
}
```
**Why**: This configuration specifies the Eureka server's URL and sets the application name, allowing Eureka to manage service registration and discovery.

2. **Configure Eureka and Discovery Client in middleware in Program.cs**:

```bash
...
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDiscoveryClient(builder.Configuration);

...
app.UseDiscoveryClient();
...

```
**Why**: This sets up the necessary configurations and middleware for the Catalog Service that uses Eureka for service discovery.

3. **Create a catalog endpoint that queries the inventory service**

Run `dotnet run` and access

```bash
GET https://localhost:7125/api/products/123
GET https://localhost:7125/api/products/{sku} where sku=123
```
This retrieves product details, which will invoke the inventory service, whose url is dynamically discovered via Eureka

**Why**: This endpoint in the Catalog Service queries the Inventory Service for inventory data, demonstrating service-to-service communication using Eureka for discovery.

*Find more documentation [HERE...](./StatlerWaldorfCorp.EcommerceCatalog/doc.md)* 

4. **Addon: Retry Policy with Polly Library**
- HttpClient with Polly: Add a retry policy with WaitAndRetryAsync which retries up to 5 times with an exponential backoff.
- Implementation [here...](./StatlerWaldorfCorp.EcommerceCatalog/Policies/ClientPolicy.cs)

## In Sum...

- Inventory Service: Provides inventory data and registers with Eureka.
- Catalog Service: Queries the Inventory Service using a discovery-enabled HTTP client, registered with Eureka.
