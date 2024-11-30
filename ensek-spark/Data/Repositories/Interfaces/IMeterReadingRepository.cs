using ensek_spark.Models;

namespace ensek_spark.Data.Repositories.Interfaces;

public interface IMeterReadingRepository
{
    Task<IEnumerable<MeterReading>> GetAllAsync();
    Task<MeterReading?> GetByIdAsync(string accountId);
    Task UpsertAsync(MeterReading meterReading);
    Task DeleteAsync(string accountId);
}
