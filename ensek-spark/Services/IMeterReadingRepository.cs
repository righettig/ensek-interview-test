﻿using ensek_spark.Models;

namespace ensek_spark.Services;

public interface IMeterReadingRepository
{
    Task<IEnumerable<MeterReading>> GetAllAsync();
    Task<MeterReading?> GetByIdAsync(string accountId);
    Task AddAsync(MeterReading meterReading);
    Task UpdateAsync(MeterReading meterReading);
    Task DeleteAsync(string accountId);
}
