using ensek_spark.DTOs;
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
        if (file is null)
        {
            return BadRequest("File is missing or empty.");
        }

        var (successfulReadings, failedReadings) = await meterReadingService.ProcessMeterReadingsAsync(file);

        return Ok(new UploadMeterReadingsResult(successfulReadings.Count, failedReadings.Count, failedReadings));
    }

    [HttpGet("/meter-readings")]
    public string GetMeterReadings()
    {
        return "OK";
    }
}
