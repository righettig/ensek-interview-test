using ensek_spark.Controllers;
using ensek_spark.Data.Repositories;
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
    private readonly Mock<IMeterReadingRepository> _meterReadingRepositoryMock;
    private readonly Mock<IMeterReadingService> _meterReadingServiceMock;
    private readonly Mock<ILogger<MeterReadingController>> _loggerMock;

    private readonly MeterReadingController _controller;

    public MeterReadingControllerTests()
    {
        _meterReadingRepositoryMock = new Mock<IMeterReadingRepository>();
        _meterReadingServiceMock = new Mock<IMeterReadingService>();
        _loggerMock = new Mock<ILogger<MeterReadingController>>();

        _controller = new MeterReadingController(_meterReadingServiceMock.Object,
                                                 _meterReadingRepositoryMock.Object,
                                                 _loggerMock.Object);
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
    public async Task GetMeterReadings_ReturnsOkResult_WithListOfMeterReadings()
    {
        // Arrange
        var mockMeterReadings = new List<MeterReading>
        {
            new MeterReading { AccountId = "123", MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0), MeterReadValue = 100 },
            new MeterReading { AccountId = "456", MeterReadingDateTime = new DateTime(2024, 1, 2, 12, 0, 0), MeterReadValue = 200 }
        };

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(mockMeterReadings);

        // Act
        var result = await _controller.GetMeterReadings();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualMeterReadings = Assert.IsAssignableFrom<IEnumerable<MeterReading>>(okResult.Value);

        Assert.Equal(2, actualMeterReadings.Count());
    }

    [Fact]
    public async Task GetMeterReadings_ReturnsOkResult_WithEmptyList_WhenNoReadingsExist()
    {
        // Arrange
        var mockMeterReadings = new List<MeterReading>(); // Empty list

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(mockMeterReadings);

        // Act
        var result = await _controller.GetMeterReadings();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var actualMeterReadings = Assert.IsAssignableFrom<IEnumerable<MeterReading>>(okResult.Value);

        Assert.Empty(actualMeterReadings);
    }

    [Fact]
    public async Task GetMeterReadings_RepositoryThrowsException_Returns500Status()
    {
        // Arrange
        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetMeterReadings();

        // Assert
        var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
    }

    #endregion
}
