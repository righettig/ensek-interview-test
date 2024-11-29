using ensek_spark.Data;
using ensek_spark.Models;
using Microsoft.EntityFrameworkCore;

namespace ensek_spark.Services;

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

    public async Task AddAsync(MeterReading meterReading)
    {
        ArgumentNullException.ThrowIfNull(meterReading);

        await _context.MeterReadings.AddAsync(meterReading);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MeterReading meterReading)
    {
        ArgumentNullException.ThrowIfNull(meterReading);

        _context.MeterReadings.Update(meterReading);
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
