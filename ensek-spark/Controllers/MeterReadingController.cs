using Microsoft.AspNetCore.Mvc;

namespace ensek_spark.Controllers;

[ApiController]
[Route("[controller]")]
public class MeterReadingController(
    ILogger<MeterReadingController> logger) : ControllerBase
{
    [HttpPost("/meter-reading-uploads")]
    public void UploadMeterReadings()
    {

    }

    [HttpGet("/meter-readings")]
    public void GetMeterReadings()
    {

    }
}
