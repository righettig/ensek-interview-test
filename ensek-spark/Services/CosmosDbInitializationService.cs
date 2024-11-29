using CsvHelper;
using CsvHelper.Configuration;
using ensek_spark.Models;
using Microsoft.Azure.Cosmos;
using System.Globalization;

namespace ensek_spark.Services;

public class CosmosDbInitializationService(CosmosClient cosmosClient,
                                           ILogger<CosmosDbInitializationService> logger,
                                           string databaseId) : BackgroundService
{
    private readonly CosmosClient _cosmosClient = cosmosClient;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Starting CosmosDB Initialization...");

        Database database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(
            id: databaseId,
            throughput: 400,
            cancellationToken: stoppingToken
        );

        logger.LogInformation($"{databaseId} database created or already exists.");

        await database.CreateContainerIfNotExistsAsync(
            id: "meter-readings",
            partitionKeyPath: "/id",
            cancellationToken: stoppingToken
        );
        logger.LogInformation("Meter readings container created or already exists.");

        await database.CreateContainerIfNotExistsAsync(
            id: "user-accounts",
            partitionKeyPath: "/id",
            cancellationToken: stoppingToken
        );
        logger.LogInformation("Meter readings container created or already exists.");

        // Import user accounts
        var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../ensek-test-data/Test_Accounts.csv");
        if (!File.Exists(filePath))
        {
            logger.LogError($"File not found: {filePath}");
            return;
        }
        await ImportUserAccountsAsync(database, filePath, stoppingToken);
    }

    private async Task ImportUserAccountsAsync(Database database, string filePath, CancellationToken cancellationToken)
    {
        var container = database.GetContainer("user-accounts");

        // Read CSV file
        var users = new List<UserAccount>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true
        };

        using (var reader = new StreamReader(filePath))
        using (var csv = new CsvReader(reader, config))
        {
            users = csv.GetRecords<UserAccount>().ToList();
        }

        // Insert each user into the container
        foreach (var user in users)
        {
            await container.UpsertItemAsync(user, cancellationToken: cancellationToken);
        }

        logger.LogInformation($"{users.Count} user accounts imported into the user-accounts collection.");
    }
}