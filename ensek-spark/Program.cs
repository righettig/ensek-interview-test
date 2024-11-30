using ensek_spark.Data.Contexts;
using ensek_spark.Data.Repositories.Impl;
using ensek_spark.Data.Repositories.Interfaces;
using ensek_spark.Extensions;
using ensek_spark.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var cosmosDbConnectionString = builder.Configuration.GetConnectionString("CosmosDb");
var databaseName = builder.Configuration["DatabaseName"];

// Configure Cosmos DB client and options
var cosmosClientOptions = new CosmosClientOptions
{
    HttpClientFactory = CreateHttpClientFactory(),
    ConnectionMode = ConnectionMode.Gateway
};

var cosmosClient = new CosmosClient(cosmosDbConnectionString, cosmosClientOptions);
builder.Services.AddSingleton(cosmosClient);

// Register DbContexts
void ConfigureDbContext<TContext>(DbContextOptionsBuilder options) where TContext : DbContext =>
    options.UseCosmos(cosmosDbConnectionString, databaseName, cosmosOptions =>
    {
        cosmosOptions.ConnectionMode(ConnectionMode.Gateway);
        cosmosOptions.HttpClientFactory(CreateHttpClientFactory());
    });

builder.Services.AddDbContext<UserAccountContext>(ConfigureDbContext<UserAccountContext>);
builder.Services.AddDbContext<MeterReadingContext>(ConfigureDbContext<MeterReadingContext>);

// Register hosted services
builder.Services.AddHostedService(sp =>
{
    var logger = sp.GetRequiredService<ILogger<CosmosDbInitializationService>>();
    return new CosmosDbInitializationService(cosmosClient, logger, databaseName);
});

// Register application services
builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
builder.Services.AddScoped<IMeterReadingRepository, MeterReadingRepository>();
builder.Services.AddScoped<IMeterReadingService, MeterReadingService>();

// Add MVC and Swagger services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(["http://localhost:5000"]);

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseCors("AllowLocalApps");
app.MapControllers();
app.Run();

// Helper methods
static Func<HttpClient> CreateHttpClientFactory() => () =>
{
    var httpMessageHandler = new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };
    return new HttpClient(httpMessageHandler);
};
