using ensek_spark.Models;
using ensek_spark.Services;
using Microsoft.AspNetCore.Mvc;

namespace ensek_spark.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MeterReadingController(IMeterReadingRepository repository,
                                    ILogger<MeterReadingController> logger) : ControllerBase
{
    [HttpPost("/meter-reading-uploads")]
    public string UploadMeterReadings()
    {
        repository.AddAsync(new MeterReading 
        {
            AccountId = "123",
            MeterReadingDateTime = DateTime.Now,
            MeterReadValue = 111
        });

        return "OK";
    }

    [HttpGet("/meter-readings")]
    public string GetMeterReadings()
    {
        return "OK";
    }
}
