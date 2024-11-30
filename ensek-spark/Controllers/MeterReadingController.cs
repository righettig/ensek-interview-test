using ensek_spark.Data.Repositories;
using ensek_spark.DTOs;
using ensek_spark.Services;
using Microsoft.AspNetCore.Mvc;

namespace ensek_spark.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeterReadingController(IMeterReadingService meterReadingService,
                                    IMeterReadingRepository meterReadingRepository,
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
    public async Task<IActionResult> GetMeterReadings()
    {
        try
        {
            var result = await meterReadingRepository.GetAllAsync();
            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
