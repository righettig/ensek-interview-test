using ensek_spark.Controllers;
using ensek_spark.DTOs;
using ensek_spark.Models;
using ensek_spark.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text;

namespace ensek_spark_tests;

public class MeterReadingControllerTests
{
    private readonly Mock<IMeterReadingService> _meterReadingServiceMock;
    private readonly Mock<ILogger<MeterReadingController>> _loggerMock;

    private readonly MeterReadingController _controller;

    public MeterReadingControllerTests()
    {
        _meterReadingServiceMock = new Mock<IMeterReadingService>();
        _loggerMock = new Mock<ILogger<MeterReadingController>>();

        _controller = new MeterReadingController(_meterReadingServiceMock.Object, _loggerMock.Object);
    }

    #region UploadMeterReadings

    [Fact]
    public async Task UploadMeterReadings_ReturnsOkResult_WithValidCounts()
    {
        // Arrange
        var fileMock = new Mock<IFormFile>();
        var fileName = "test.csv";
        var fileContent = "test content";
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(fileContent));

        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);

        var successfulReadings = new List<MeterReading> { new MeterReading() };
        var failedReadings = new List<string> { "Error 1" };

        _meterReadingServiceMock
            .Setup(s => s.ProcessMeterReadingsAsync(It.IsAny<IFormFile>()))
            .ReturnsAsync((successfulReadings, failedReadings));

        // Act
        var result = await _controller.UploadMeterReadings(fileMock.Object);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UploadMeterReadingsResult>(okResult.Value);
        Assert.Equal(1, response.SuccessfulCount);
        Assert.Equal(1, response.FailedCount);
        //Assert.Contains("Error 1", response.FailedDetails);
    }

    [Fact]
    public async Task UploadMeterReadings_ReturnsBadRequest_WhenFileIsNull()
    {
        // Arrange
        IFormFile file = null;

        // Act
        var result = await _controller.UploadMeterReadings(file);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File is missing or empty.", badRequestResult.Value);
    }

    #endregion

    #region GetMeterReadings

    [Fact]
    public void GetMeterReadings_ReturnsOkString()
    {
        // Act
        var result = _controller.GetMeterReadings();

        // Assert
        Assert.Equal("OK", result);
    }

    #endregion
}
