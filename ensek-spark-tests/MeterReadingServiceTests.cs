using ensek_spark.Data.Repositories.Interfaces;
using ensek_spark.Models;
using ensek_spark.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Text;

namespace ensek_spark_tests;

public class MeterReadingServiceTests
{
    private readonly Mock<IUserAccountRepository> _userAccountRepositoryMock;
    private readonly Mock<IMeterReadingRepository> _meterReadingRepositoryMock;

    private readonly MeterReadingService _service;

    public MeterReadingServiceTests()
    {
        _userAccountRepositoryMock = new Mock<IUserAccountRepository>();
        _meterReadingRepositoryMock = new Mock<IMeterReadingRepository>();

        _service = new MeterReadingService(_userAccountRepositoryMock.Object, _meterReadingRepositoryMock.Object);
    }

    #region ProcessMeterReadingsAsync

    [Fact]
    public async Task ProcessMeterReadingsAsync_ReturnsFailure_WhenFileIsNull()
    {
        // Arrange
        IFormFile file = null;

        // Act
        var (successfulReadings, failedReadings) = await _service.ProcessMeterReadingsAsync(file);

        // Assert
        Assert.Empty(successfulReadings);
        Assert.Single(failedReadings);
        Assert.Contains("File is missing or empty.", failedReadings);
    }

    [Fact]
    public async Task ProcessMeterReadingsAsync_ReturnsFailure_WhenAccountIdIsInvalid()
    {
        // Arrange
        var fileContent = "AccountId,MeterReadingDateTime,MeterReadValue,\nInvalidId,22/04/2019 09:24,12345,";
        var file = CreateMockFile(fileContent);

        _userAccountRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([]); // No valid accounts

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var (successfulReadings, failedReadings) = await _service.ProcessMeterReadingsAsync(file);

        // Assert
        Assert.Empty(successfulReadings);
        Assert.Single(failedReadings);
        Assert.Contains("Invalid AccountId: InvalidId", failedReadings);
    }

    [Fact]
    public async Task ProcessMeterReadingsAsync_ReturnsFailure_WhenMeterReadValueIsInvalid()
    {
        // Arrange
        var fileContent = "AccountId,MeterReadingDateTime,MeterReadValue,\nValidAccount,22/04/2019 09:24,123,";
        var file = CreateMockFile(fileContent);

        _userAccountRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([new UserAccount { AccountId = "ValidAccount" }]);

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([]);

        // Act
        var (successfulReadings, failedReadings) = await _service.ProcessMeterReadingsAsync(file);

        // Assert
        Assert.Empty(successfulReadings);
        Assert.Single(failedReadings);
        Assert.Contains("Invalid MeterReadValue: 123 for AccountId ValidAccount", failedReadings);
    }

    [Fact]
    public async Task ProcessMeterReadingsAsync_ReturnsFailure_WhenDuplicateEntryExists()
    {
        // Arrange
        var fileContent = "AccountId,MeterReadingDateTime,MeterReadValue,\nValidAccount,22/04/2019 09:24,12345,";
        var file = CreateMockFile(fileContent);

        var existingReading = new MeterReading
        {
            AccountId = "ValidAccount",
            MeterReadValue = 12345,
            MeterReadingDateTime = new DateTime(2019, 4, 22, 9, 24, 0)
        };

        _userAccountRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([new UserAccount { AccountId = "ValidAccount" }]);

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([existingReading]);

        // Act
        var (successfulReadings, failedReadings) = await _service.ProcessMeterReadingsAsync(file);

        // Assert
        Assert.Empty(successfulReadings);
        Assert.Single(failedReadings);
        Assert.Contains("Duplicate entry: AccountId ValidAccount, DateTime 22/04/2019 09:24:00", failedReadings);
    }

    [Fact]
    public async Task ProcessMeterReadingsAsync_ReturnsSuccess_WhenValidRecordIsProvided()
    {
        // Arrange
        var fileContent = "AccountId,MeterReadingDateTime,MeterReadValue,\nValidAccount,22/04/2019 09:24,12345,";
        var file = CreateMockFile(fileContent);

        _userAccountRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([new UserAccount { AccountId = "ValidAccount" }]);

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([]);

        _meterReadingRepositoryMock
            .Setup(repo => repo.UpsertAsync(It.IsAny<MeterReading>()))
            .Returns(Task.CompletedTask);

        // Act
        var (successfulReadings, failedReadings) = await _service.ProcessMeterReadingsAsync(file);

        // Assert
        Assert.Single(successfulReadings);
        Assert.Empty(failedReadings);
    }

    [Fact]
    public async Task ProcessMeterReadingsAsync_ReturnsFailure_WhenReadingIsOlderThanExisting()
    {
        // Arrange
        var fileContent = "AccountId,MeterReadingDateTime,MeterReadValue,\nValidAccount,21/04/2019 09:24,12345,";
        var file = CreateMockFile(fileContent);

        var existingReading = new MeterReading
        {
            AccountId = "ValidAccount",
            MeterReadValue = 12345,
            MeterReadingDateTime = new DateTime(2019, 4, 22, 9, 24, 0) // Existing read is newer
        };

        _userAccountRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([new UserAccount { AccountId = "ValidAccount" }]);

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([existingReading]);

        // Act
        var (successfulReadings, failedReadings) = await _service.ProcessMeterReadingsAsync(file);

        // Assert
        Assert.Empty(successfulReadings);
        Assert.Single(failedReadings);
        Assert.Contains("Reading too old: AccountId ValidAccount, DateTime 21/04/2019 09:24:00 (latest: 22/04/2019 09:24:00)", failedReadings);
    }

    [Fact]
    public async Task ProcessMeterReadingsAsync_AllowsNewerReading()
    {
        // Arrange
        var fileContent = "AccountId,MeterReadingDateTime,MeterReadValue,\nValidAccount,23/04/2019 09:24,12345,";
        var file = CreateMockFile(fileContent);

        var existingReading = new MeterReading
        {
            AccountId = "ValidAccount",
            MeterReadValue = 12345,
            MeterReadingDateTime = new DateTime(2019, 4, 22, 9, 24, 0) // Existing read is older
        };

        _userAccountRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([new UserAccount { AccountId = "ValidAccount" }]);

        _meterReadingRepositoryMock
            .Setup(repo => repo.GetAllAsync())
            .ReturnsAsync([existingReading]);

        _meterReadingRepositoryMock
            .Setup(repo => repo.UpsertAsync(It.IsAny<MeterReading>()))
            .Returns(Task.CompletedTask);

        // Act
        var (successfulReadings, failedReadings) = await _service.ProcessMeterReadingsAsync(file);

        // Assert
        Assert.Single(successfulReadings);
        Assert.Empty(failedReadings);
    }

    #endregion

    private IFormFile CreateMockFile(string content, string fileName = "test.csv")
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(stream.Length);
        return fileMock.Object;
    }
}