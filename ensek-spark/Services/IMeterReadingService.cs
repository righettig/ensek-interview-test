using ensek_spark.Models;

namespace ensek_spark.Services;

public interface IMeterReadingService
{
    Task<(List<MeterReading> successfulReadings, List<string> failedReadings)> ProcessMeterReadingsAsync(IFormFile file);
}

