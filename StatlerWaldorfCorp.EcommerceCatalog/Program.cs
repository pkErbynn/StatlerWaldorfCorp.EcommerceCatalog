using StatlerWaldorfCorp.EcommerceCatalog.InventoryClient;
using StatlerWaldorfCorp.EcommerceCatalog.Policies;
using StatlerWaldorfCorp.EcommerceCatalog.Repository;
using Steeltoe.Discovery.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional:false, reloadOnChange: true);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// builder.Services.AddDiscoveryClient(builder.Configuration);
builder.Services.AddPollyPolicyService();
builder.Services.AddScoped<IInventoryClient, HttpInventoryClient>();
builder.Services.AddScoped<IProductRepository, MemoryProductRepository>();

var app = builder.Build();

app.UseDiscoveryClient();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();