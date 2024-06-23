## Service Discovery

The Steeltoe Discovery Client package integrates Eureka with .NET applications, enabling service registration and discovery.

## Setting Up
Demonstration of creating a resilient and discoverable microservices architecture using .NET 6, Steeltoe, and Eureka. 

This setup allows services to dynamically discover and communicate with each other, enhancing scalability and resilience.

### Setting Up Eureka Server
Extending from the Inventory Service here - https://github.com/pkErbynn/StatlerWaldorfCorp.EcommerceInventory


1. Configure Eureka in appsettings.json
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
*Why*: This configuration specifies the Eureka server's URL and sets the application name, allowing Eureka to manage service registration and discovery.

2. Configure Eureka and Discovery Client in middleware in Program.cs:

```bash
...
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDiscoveryClient(builder.Configuration);

...
app.UseDiscoveryClient();
...

```
*Why*: This sets up the necessary configurations and middleware for the Catalog Service that uses Eureka for service discovery.

3. Create a catalog endpoint that queries the inventory service

Run `dotnet run` and access

```bash
GET https://localhost:7125/api/products/123
GET https://localhost:7125/api/products/{sku} where sku=123
```
This retrieves product details, which will invoke the inventory service, whose url is dynamically discovered via Eureka

*Why*: This endpoint in the Catalog Service queries the Inventory Service for inventory data, demonstrating service-to-service communication using Eureka for discovery.



## In Sum...

- Inventory Service: Provides inventory data and registers with Eureka.

- Catalog Service: Queries the Inventory Service using a discovery-enabled HTTP client, registered with Eureka.



---------------------------------
----------------------------------

intro
---
service discovery particularly valuable in dynamic, scalable microservices environments where services need to be robust and adaptable to changes and failures.

how the Catalog Service discovers the Inventory Service at runtime:
---

1. Service Registration:

Both the Inventory Service and Catalog Service register themselves with the Eureka server upon startup.
They send their metadata (such as service name, IP address, port) to Eureka.

2. Eureka Server:

The Eureka server stores the registration details of all services and continuously monitors their health status.

3. Discovery HTTP Client Configuration:

In the Catalog Service, an HTTP client is configured to use Eureka for service discovery.
This is done by adding Steeltoe’s DiscoveryHttpClientHandler to the HTTP client configuration.

4. Service Discovery:

When the Catalog Service needs to call the Inventory Service, it uses the configured HTTP client.
The HTTP client handler (DiscoveryHttpClientHandler) intercepts the request.

5. Querying Eureka:

The DiscoveryHttpClientHandler queries the Eureka server to get the list of instances for the Inventory Service.
Eureka returns the available instances of the Inventory Service.

6. Load Balancing:

The DiscoveryHttpClientHandler selects an instance from the list returned by Eureka, typically using a load-balancing strategy (e.g., round-robin, random).

7. Making the Request:

The HTTP client then makes the request to the selected instance of the Inventory Service.
This process is transparent to the service code, which only needs to use the configured HTTP client.


----
Scenario: Inventory Service API Changes

scenario where the Inventory Service API changes, and the Catalog Service can’t find the Inventory Service. This will help illustrate the dynamics of service discovery and how to handle such situations.

1. Initial Setup:

Both the Inventory Service and Catalog Service are running and registered with Eureka.
The Catalog Service uses the DiscoveryHttpClientHandler to discover and communicate with the Inventory Service.

2. Then Inventory Service API Changes:

Suppose the Inventory Service changes its API endpoint from /api/inventory/{sku} to /api/**items**/{sku}.
If the Catalog Service is not updated to reflect this change, it will continue to send requests to the old endpoint /api/inventory/{sku}.

What Happens?

1. Catalog Service Tries to Discover Inventory Service:
The Catalog Service’s HTTP client queries Eureka to discover the Inventory Service.
Eureka responds with the available instances of the Inventory Service.

2. HTTP Request Fails:
The Catalog Service constructs a request to the old endpoint /api/inventory/{productId}.
Since the Inventory Service has changed its endpoint to /api/items/{productId}, the request fails with a 404 Not Found error.


Eureka continuously monitors the health of registered services. If an instance fails health checks, it is marked as unavailable.

----
Benefits:
Using a service discovery method for runtime URL resolution provides several advantages over statically configuring service URLs in configuration files. Here are the key benefits:

1. Dynamic Scalability and Load Balancing
Dynamic Scalability: Service discovery allows your system to dynamically scale up or down. New instances of a service can be added or removed, and the service registry will automatically update the available instances.
Load Balancing: Service discovery can distribute requests across multiple service instances, providing built-in load balancing. This helps ensure even distribution of traffic and better utilization of resources.

2. Resilience and Fault Tolerance
Automatic Failover: If a service instance goes down, the service registry will automatically mark it as unavailable. Service discovery ensures that only healthy instances are used, improving the resilience of your application.
Retry Mechanisms: Combined with resilience patterns (e.g., Polly), service discovery can automatically retry failed requests on different instances, reducing the impact of transient failures.

3. Simplified Configuration Management
Reduced Configuration Overhead: Instead of manually updating configuration files whenever a service's URL changes, service discovery automates this process. This reduces the chances of configuration errors and simplifies the deployment process.
Environment Agnostic: Service discovery makes it easier to move between different environments (development, staging, production) without changing configuration files. The service registry will handle the correct routing based on the environment.

4. Microservices Architecture Compatibility
Service Interdependence: In a microservices architecture, services often depend on other services. Service discovery provides a flexible and reliable way to manage these dependencies, especially in complex systems.
Easier Service Updates: With service discovery, you can update or deploy new versions of services without downtime. New instances with different URLs can be seamlessly integrated, and old instances can be decommissioned without affecting the overall system.

5. Improved Operational Visibility
Health Monitoring: Service registries like Eureka often include health monitoring and status checking capabilities, providing visibility into the health and status of services.
Service Metrics: You can gather metrics on service usage, performance, and availability, which can help with capacity planning and identifying potential issues.

Comparison with Static Configuration

Fixed URLs: 
Service URLs are hardcoded in configuration files.
Manual Updates: 
Any change in service URLs requires manual updates and redeployment of configuration files.
Limited Scalability: 
Static URLs do not support dynamic scaling or automatic failover.
Potential for Errors: 
Increased risk of misconfigurations due to manual updates.

Summary
Using service discovery offers significant advantages over static configuration, particularly in dynamic and complex microservices environments. It enhances scalability, resilience, and operational efficiency while reducing the risk of configuration errors and simplifying the management of service dependencies. By dynamically resolving service URLs at runtime, service discovery ensures that your applications can adapt to changes and maintain high availability and performance.

----

both approaches (static configuration and service discovery) will return an error if a service is unavailable. However, service discovery offers additional benefits that go beyond simply detecting an unavailable service. Here’s a more detailed comparison:

1. Service Unavailability (Static Configuration vs Service Discovery)
Static Configuration (Manual Intervention):
  - The Catalog Service consumer uses a hardcoded URL to connect to the Inventory Service dependency.
  - Failure Handling: If the Inventory Service dependency at the hardcoded URL is down, the request fails, and an error is returned. Manual intervention is needed to update the URL if the service dependency endpoint changes.


Service Discovery (Automatic Deregistration)
  - Error Handling: Similar to static configuration, if no instances are available, the request will fail, and an error will be returned.
  - If one instance of the Inventory Service is down, the service registry automatically directs the request to another healthy instance. 
  - Unavailable instance is automatically deregistered from the service registry, preventing requests to be sent until it recovers.

2. Failover
Static Config (No Automatic Failover:):
  - Fixed Endpoint: Static configuration points to a fixed endpoint. If that endpoint is down, the application cannot automatically switch to another available instance.
  - Single Point of Failure: There is no built-in mechanism to handle failover, making the application more vulnerable to downtime if the depending service is unavailable.

Service Discovery (Automatic Failover:)
- Dynamic Endpoint Resolution: Service discovery dynamically resolves endpoints. If one instance is down, the request can be redirected to another healthy instance.
- Increased Resilience: This automatic failover capability increases the resilience of your application by ensuring that only healthy instances are used.

3. Load Balancing

Static Config (Lack of Load Balancing)
- No Distribution: Requests are sent to a single, fixed endpoint, which can lead to uneven load distribution and potential bottlenecks.

Service Discovery (Load Balancing)
- Even Distribution: Service discovery can balance requests across multiple instances, leading to better resource utilization and reduced risk of bottlenecks.
- Improved Performance: Load balancing helps maintain consistent performance and availability, especially under high load.

4. Configuration Management:

Static Config (Manual Updatesg)
- Manual Updates: Any change in depending service URL requires manual updates to the configuration file and redeployment of the consumer service.

Service Discovery (Automatic Updates)
- Automatic Updates: Changes in service endpoints are automatically managed by the service registry. There’s no need for manual updates and redeployment when depending service instances change.
- Simplified Deployment: Service discovery simplifies deployment and scaling, as new instances can be added or removed without manual reconfiguration.


Additional Benefits of Service Discovery
These benefit has been harped before. 

1. Health Checks:

Automatic Health Monitoring: Service registries often include health checks to continuously monitor the status of registered services.
Proactive Management: Unhealthy instances are automatically removed from the pool, reducing the likelihood of failed requests.

2. Dynamic Scaling:

Elastic Scaling: Service discovery supports dynamic scaling of services. As new instances are added, they are automatically registered and available for discovery.
Seamless Updates: Services can be updated or replaced without downtime, as the registry ensures that only healthy, updated instances are used.


---------
the benefits of using a discovery service over static configuration in your two services (Inventory and Catalog). Let's walk through an example where the Catalog Service needs to query the Inventory Service.


=== Use Case: Static Configuration

-- Catalog Service

```json
{
  "Inventory": {
    "BaseUrl": "http://localhost:5001/api/skustatus/"
  }
}
```

```c#
var inventoryServiceBaseUrl = Configuration["Inventory:BaseUrl"];
var response = await client.GetStringAsync(inventoryServiceBaseUrl + sku.ToString());
```

Issue: Adding or removing instances requires manual updates to configuration files and redeployment.

Issue: If the Inventory Service goes down, the Catalog Service will continue to send requests to the now unavailable endpoint, resulting in failures.

-- 

=== Use Case: Inventory and Catalog Services with Eureka Discovery Service


Inventory Service Configuration - appsettings.json:
```json
{
  "spring": {
    "application": {
      "name": "inventory"
    }
  },
  "eureka": {
    "client": {
      "serviceUrl": "http://192.168.0.33:8080/eureka/",
      "shouldRegisterWithEureka": true,
      "shouldFetchRegistry": false,
      "validate_certificates": false
    },
    "instance": {
      "appname": "inventory",
      "nonSecurePort": 5001,
      "port": 5001,
      "instanceId": "localhost:inventory:5001"
    }
  },
  "Logging": {
    "IncludeScopes": false,
    "LogLevel": {
        "Default": "Debug",
        "System": "Debug",
        "Microsoft": "Debug",
        "Steeltoe": "Debug"
    }
  }
}
```
When you start a new instance of the Inventory Service, it will automatically register itself with the Eureka server.


Inventory Configuration - Program.cs
```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.Discovery.Client;
using Steeltoe.Common.Http.Discovery;

var builder = WebApplication.CreateBuilder(args);

// Configure Eureka and Discovery Client
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDiscoveryClient(builder.Configuration);

....
builder.Services.AddControllers();

var app = builder.Build();

...
app.UseDiscoveryClient(); // discovery added to middleware

app.Run();

```


Catalog Configuration
appsettings.json

```json
{
  "spring": {
    "application": {
      "name": "catalog"
    }
  },
  "eureka": {
    "client": {
      "serviceUrl": "http://localhost:8080/eureka/",
      "shouldRegisterWithEureka": false
    }
  }
}
```

Benefit: New instances register themselves with Eureka automatically. No need to manually update configuration files.

Example: As new instances of the Inventory Service come online, they register with Eureka, and the Catalog Service automatically discovers them.


Program.cs
```c#
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Steeltoe.Discovery.Client;
using Steeltoe.Common.Http.Discovery;

var builder = WebApplication.CreateBuilder(args);

// Configure Eureka and Discovery Client
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddDiscoveryClient(builder.Configuration);

....
builder.Services.AddControllers();
builder.Services.AddScoped<IInventoryClient, HttpInventoryClient>();

var app = builder.Build();

...
app.UseDiscoveryClient(); // discovery added to middleware

app.Run();

```


Catalog Service's HttpInventoryClient Implementation:

```c#
public class HttpInventoryClient: IInventoryClient
{
    private readonly DiscoveryHttpClientHandler discoveryHttpClientHandler;
    private const string STOCKSERVICE_URL_BASE = "http://inventory/api/skustatus/";   // Use the service descriptor

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

```

Catalog Controller:

```c#
[HttpGet("{sku}")]
public IActionResult GetProduct(int sku)
{
    var product = new
    {
        Product = this._productRepository.Get(sku),
        Status = this._inventoryClient.GetStockStatusAsync(sku).Result  // making call to inventory service
    };
    return this.Ok(product);
}

```


Analogy:

Static configuration is like always going to the same store address even if it is closed, whereas service discovery is like asking a concierge for an open store, ensuring you never reach a closed one.





=== Adding or Modifying Instances with Eureka

USING DYNAMIC SERVICE DISCOVERY

Static configuration:

- Any change in the service URL requires manual updates to the configuration files of all services that depend on it.
- These updates typically necessitate a redeployment of those dependent services to reflect the new configurations.

Example Scenario:
1. Initial Setup: The Catalog Service has a hardcoded URL for the Inventory Service.

```json
{
  "InventoryService": {
    "BaseUrl": "http://localhost:5001/api/skustatus/"
  }
}
```
2. Change in Inventory Service URL: If the Inventory Service URL changes to `http://localhost:5002/api/skustatus/`, you must manually update the Catalog Service configuration:
```json
{
  "InventoryService": {
    "BaseUrl": "http://localhost:5002/api/skustatus/" // port changed
  }
}
```

3. Redeployment: The Catalog Service must be redeployed to apply this configuration change.



USING DYNAMIC SERVICE DISCOVERY

Service discovery using Eureka:

- Service instances automatically register and deregister with Eureka.
- Eureka provides the current list of service instances to clients, allowing them to discover service URLs dynamically at runtime.
- Changes to the service URL or new instances do not require manual updates to the dependent services' configurations or redeployments.

Example scenario

Existing Inventory Service Configuration
```json
{
  "spring": {
    "application": {
      "name": "inventory"
    }
  },
  "eureka": {
    "client": {
      "serviceUrl": "http://192.168.0.33:8080/eureka/",
      ...
    },
    "instance": {
      "appname": "inventory",
      "port": 5001,
      "instanceId": "localhost:inventory:5001"
    }
  },
  ...
}

```

New Instance Configuration
- When a new instance of the Inventory Service starts, it registers itself with Eureka without affecting the running instance.

```json
{
  "spring": {
    "application": {
      "name": "inventory"
    }
  },
  "eureka": {
    "client": {
      "serviceUrl": "http://192.168.0.33:8080/eureka/",
      ...
    },
    "instance": {
      "appname": "inventory",
      "port": 5002,
      "instanceId": "localhost:inventory:5002"  // port changed
    }
  },
  ...
}
```

Catalog Service

- The Catalog Service does not need to be updated manually to recognize the new instance.
- The `DiscoveryHttpClientHandler` will dynamically discover all available instances of the Inventory Service through Eureka.

Catalog Service Discovery Client Setup (**_remains unchanged_**):
```json
{
  "spring": {
    "application": {
      "name": "catalog"
    }
  },
  "eureka": {
    "client": {
      "serviceUrl": "http://localhost:8080/eureka/",
      "shouldRegisterWithEureka": false
    }
  }
}
```


Benefits of Dynamic Service Discovery

- No Manual Configuration Updates:

When a new instance is added or an existing instance is modified, no changes are required in the Catalog Service configuration files. Eureka handles the discovery of new instances automatically.

- Automatic Load Balancing:

Requests from the Catalog Service are automatically distributed among all available instances of the Inventory Service. The DiscoveryHttpClientHandler ensures that traffic is balanced, and if one instance goes down, requests are rerouted to healthy instances.

- High Availability:

By having multiple instances of the Inventory Service, you ensure high availability and fault tolerance. If one instance fails, Eureka will mark it as unavailable, and the Catalog Service will not attempt to route requests to the failed instance.



HOW EUREKA Resolves Service Descriptors

In the Catalog Service's HttpInventoryClient Implementation:
```c#
    private readonly DiscoveryHttpClientHandler discoveryHttpClientHandler;
    private const string STOCKSERVICE_URL_BASE = "http://inventory/api/skustatus/";

    public HttpInventoryClient(IDiscoveryClient discoveryClient)
    {
        this.discoveryHttpClientHandler = new DiscoveryHttpClientHandler(discoveryClient);
    }

```

How It Works
1. Service Registration: The Inventory Service registers itself with Eureka using a unique identifier and its URL (e.g., `http://inventory:5001`).
2. Service Descriptor: The Catalog Service uses `http://inventory/api/` as the base URL for its HTTP client.
3. Discovery Client: The DiscoveryHttpClientHandler intercepts the request to `http://inventory/api/skustatus/{sku}`.
4. Service Resolution:
  - The DiscoveryHttpClientHandler queries the Eureka server for instances of the inventory service.
  - Eureka returns the list of available instances (e.g., http://inventory:5001, http://inventory:5002).
  - The handler replaces inventory in the URL with the actual instance address (e.g., http://inventory:5001/api/skustatus/{sku}).

Analogy
- Think of it as using a short nickname (e.g., "inventory") that automatically gets resolved to the full address (e.g., inventory.mycluster.mycorp.com) whenever you send a message. This way, you don't have to remember or update the full address every time it changes.


SUMMARY

URLs are resolved at runtime, so changes in service instances are automatically handled without manual updates. Requests can be distributed among multiple instances of a service, improving scalability and reliability. If one instance is down, the discovery client can route requests to another healthy instance, enhancing fault tolerance.

By using service discovery with Eureka and Steeltoe’s DiscoveryHttpClientHandler, your microservices can dynamically discover and communicate with each other, ensuring robust and scalable inter-service communication.
