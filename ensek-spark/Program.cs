using ensek_spark.Data;
using ensek_spark.Services;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cosmosDbConnectionString = builder.Configuration.GetConnectionString("CosmosDb");
var databaseName = builder.Configuration["DatabaseName"];

// avoid SSL certificate issue on connect
var httpClientFactory = () =>
{
    HttpMessageHandler httpMessageHandler = new HttpClientHandler()
    {
        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
    };

    return new HttpClient(httpMessageHandler);
};

var cosmosClientOptions = new CosmosClientOptions()
{
    HttpClientFactory = httpClientFactory,
    ConnectionMode = ConnectionMode.Gateway
};

var cosmosClient = new CosmosClient(cosmosDbConnectionString, cosmosClientOptions);
builder.Services.AddSingleton(cosmosClient);

builder.Services.AddDbContext<MeterReadingContext>(options => options.UseCosmos(cosmosDbConnectionString, databaseName, cosmosOptions =>
{
    cosmosOptions.ConnectionMode(ConnectionMode.Gateway);
    cosmosOptions.HttpClientFactory(httpClientFactory);
}));

builder.Services.AddHostedService(sp =>
{
    var logger = sp.GetRequiredService<ILogger<CosmosDbInitializationService>>();
    return new CosmosDbInitializationService(cosmosClient, logger, databaseName);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
