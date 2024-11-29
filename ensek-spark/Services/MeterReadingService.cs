using CsvHelper.Configuration;
using CsvHelper;
using ensek_spark.Data.Repositories;
using ensek_spark.Models;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ensek_spark.Services;

public class MeterReadingService(IUserAccountRepository userAccountRepository,
                                 IMeterReadingRepository meterReadingRepository) : IMeterReadingService
{
    public async Task<(List<MeterReading> successfulReadings, List<string> failedReadings)> ProcessMeterReadingsAsync(IFormFile file)
    {
        var successfulReadings = new List<MeterReading>();
        var failedReadings = new List<string>();

        if (file == null || file.Length == 0)
        {
            failedReadings.Add("File is missing or empty.");
            return (successfulReadings, failedReadings);
        }

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        var userAccounts = (await userAccountRepository.GetAllAsync())
            .ToDictionary(u => u.AccountId);

        var existingReadings = (await meterReadingRepository.GetAllAsync())
            .ToHashSet(new MeterReadingComparer());

        while (await csv.ReadAsync())
        {
            try
            {
                var record = csv.GetRecord<MeterReading>();

                // Validate AccountId
                if (string.IsNullOrWhiteSpace(record.AccountId) || !userAccounts.ContainsKey(record.AccountId))
                {
                    failedReadings.Add($"Invalid AccountId: {record.AccountId}");
                    continue;
                }

                // Validate MeterReadValue
                // if (!Regex.IsMatch(record.MeterReadValue.ToString(), @"^\d{1,5}$")) // up to 5 chars
                if (!Regex.IsMatch(record.MeterReadValue.ToString(), @"^\d{5}$")) // exactly 5 chars
                {
                    failedReadings.Add($"Invalid MeterReadValue: {record.MeterReadValue} for AccountId {record.AccountId}");
                    continue;
                }

                // Validate Duplicate Entry
                if (existingReadings.Contains(record))
                {
                    failedReadings.Add($"Duplicate entry: AccountId {record.AccountId}, DateTime {record.MeterReadingDateTime}");
                    continue;
                }

                // Add valid record
                await meterReadingRepository.AddAsync(record);
                successfulReadings.Add(record);
            }
            catch (Exception ex)
            {
                failedReadings.Add($"Error parsing record: {ex.Message}");
            }
        }

        return (successfulReadings, failedReadings);
    }
}

