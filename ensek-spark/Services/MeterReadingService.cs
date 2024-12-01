using CsvHelper.Configuration;
using CsvHelper;
using ensek_spark.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using ensek_spark.Data.Repositories.Interfaces;

namespace ensek_spark.Services;

public class MeterReadingService(IUserAccountRepository userAccountRepository,
                                 IMeterReadingRepository meterReadingRepository) : IMeterReadingService
{
    public async Task<(List<MeterReading> successfulReadings, List<string> failedReadings)> ProcessMeterReadingsAsync(IFormFile file)
    {
        var successfulReadings = new List<MeterReading>();
        var failedReadings = new List<string>();

        if (!IsFileValid(file))
        {
            failedReadings.Add("File is missing or empty.");
            return (successfulReadings, failedReadings);
        }

        var userAccounts = await LoadUserAccountsAsync();
        var existingReadings = await LoadExistingReadingsAsync();

        using var reader = new StreamReader(file.OpenReadStream());
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture));

        while (await csv.ReadAsync())
        {
            try
            {
                var record = csv.GetRecord<MeterReading>();

                if (!IsValidMeterReading(record, userAccounts, existingReadings, out var validationError))
                {
                    failedReadings.Add(validationError);
                    continue;
                }

                await meterReadingRepository.UpsertAsync(record);
                successfulReadings.Add(record);
                existingReadings[record.AccountId] = record.MeterReadingDateTime;
            }
            catch (Exception ex)
            {
                failedReadings.Add($"Error parsing record: {ex.Message}");
            }
        }

        return (successfulReadings, failedReadings);
    }

    private static bool IsFileValid(IFormFile file) => file != null && file.Length > 0;

    private async Task<Dictionary<string, UserAccount>> LoadUserAccountsAsync()
    {
        var userAccounts = await userAccountRepository.GetAllAsync();
        return userAccounts.ToDictionary(u => u.AccountId);
    }

    private async Task<Dictionary<string, DateTime>> LoadExistingReadingsAsync()
    {
        var readings = await meterReadingRepository.GetAllAsync();
        return readings
            .GroupBy(r => r.AccountId)
            .ToDictionary(g => g.Key, g => g.Max(r => r.MeterReadingDateTime));
    }

    /* Potential improvement
     * ---------------------
     * This code is easy enough but we have some hardcoded rules for the validation logic.
     * To make the validation logic generic and extensible we can use the strategy pattern and the factory pattern.
     * Create a IValidationRule interface that defines a method to validate a record.
     * Implement different validation rules (e.g., AccountIdValidationRule, MeterReadValueValidationRule, etc.) as separate classes.
     * Create a factory that instantiates each rule (we can use reflection to discover implementation of the IValidationRule.
     * The ProcessMeterReadingsAsync method loops through the rules and applies them.
     */
    private static bool IsValidMeterReading(MeterReading record,
                                            Dictionary<string, UserAccount> userAccounts,
                                            Dictionary<string, DateTime> existingReadings,
                                            out string validationError)
    {
        validationError = string.Empty;

        // Validate AccountId
        if (string.IsNullOrWhiteSpace(record.AccountId) || !userAccounts.ContainsKey(record.AccountId))
        {
            validationError = $"Invalid AccountId: {record.AccountId}";
            return false;
        }

        // ASSUMPTION: based on the acceptance criteria on slide 03 I am assuming that all readings must be EXACTLY 5 digits.
        // if all numerical values up to 5 digits are to be considered valid please use this instead.
        // if (!Regex.IsMatch(record.MeterReadValue.ToString(), @"^\d{1,5}$")) // up to 5 chars

        if (!Regex.IsMatch(record.MeterReadValue.ToString(), @"^\d{5}$"))
        {
            validationError = $"Invalid MeterReadValue: {record.MeterReadValue} for AccountId {record.AccountId}";
            return false;
        }

        // Validate Duplicate Entry
        if (existingReadings.ContainsKey(record.AccountId) &&
            existingReadings[record.AccountId] == record.MeterReadingDateTime)
        {
            validationError = $"Duplicate entry: AccountId {record.AccountId}, DateTime {record.MeterReadingDateTime}";
            return false;
        }

        // Validate that the new reading is not older than the existing reading
        if (existingReadings.TryGetValue(record.AccountId, out var latestReadingDateTime) &&
            record.MeterReadingDateTime < latestReadingDateTime)
        {
            validationError = $"Reading too old: AccountId {record.AccountId}, DateTime {record.MeterReadingDateTime} (latest: {latestReadingDateTime})";
            return false;
        }

        return true;
    }
}
