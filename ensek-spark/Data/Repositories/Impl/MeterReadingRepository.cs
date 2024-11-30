using ensek_spark.Data.Contexts;
using ensek_spark.Data.Repositories.Interfaces;
using ensek_spark.Models;
using Microsoft.EntityFrameworkCore;

namespace ensek_spark.Data.Repositories.Impl;

public class MeterReadingRepository : IMeterReadingRepository
{
    private readonly MeterReadingContext _context;

    public MeterReadingRepository(MeterReadingContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<MeterReading>> GetAllAsync()
    {
        return await _context.MeterReadings.ToListAsync();
    }

    public async Task<MeterReading?> GetByIdAsync(string accountId)
    {
        return await _context.MeterReadings
            .FirstOrDefaultAsync(m => m.AccountId == accountId);
    }

    public async Task UpsertAsync(MeterReading meterReading)
    {
        ArgumentNullException.ThrowIfNull(meterReading);

        // Check if the entity already exists in the context (tracked entities)
        var existingEntity = await GetByIdAsync(meterReading.AccountId);

        if (existingEntity != null)
        {
            // Entity exists, update the existing one
            _context.Entry(existingEntity).CurrentValues.SetValues(meterReading);
        }
        else
        {
            // Entity does not exist, add as a new entity
            await _context.MeterReadings.AddAsync(meterReading);
        }

        // Save changes to the database
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string accountId)
    {
        var meterReading = await GetByIdAsync(accountId);

        if (meterReading == null)
        {
            throw new KeyNotFoundException($"Meter reading with AccountId '{accountId}' not found.");
        }

        _context.MeterReadings.Remove(meterReading);
        await _context.SaveChangesAsync();
    }
}
