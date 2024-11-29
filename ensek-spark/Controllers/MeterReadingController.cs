using ensek_spark.Services;
using Microsoft.AspNetCore.Mvc;

namespace ensek_spark.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeterReadingController(IMeterReadingService meterReadingService,
                                    ILogger<MeterReadingController> logger) : ControllerBase
{
    [HttpPost("/meter-reading-uploads")]
    public async Task<IActionResult> UploadMeterReadings(IFormFile file)
    {
        var (successfulReadings, failedReadings) = await meterReadingService.ProcessMeterReadingsAsync(file);

        return Ok(new
        {
            SuccessfulCount = successfulReadings.Count,
            FailedCount = failedReadings.Count,
            FailedDetails = failedReadings
        });
    }

    [HttpGet("/meter-readings")]
    public string GetMeterReadings()
    {
        return "OK";
    }
}
