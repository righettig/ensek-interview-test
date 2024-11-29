using Microsoft.AspNetCore.Mvc;

namespace ensek_spark.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeterReadingController(
    ILogger<MeterReadingController> logger) : ControllerBase
{
    [HttpPost("/meter-reading-uploads")]
    public string UploadMeterReadings()
    {
        return "OK";
    }

    [HttpGet("/meter-readings")]
    public string GetMeterReadings()
    {
        return "OK";
    }
}
