using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ensek_spark_tests_integration;

public class IntegrationTests
{
    #region UploadMeterReadings

    [Fact]
    public async Task UploadMeterReadings_ShouldReturnBadRequest_WhenNoFileIsProvided()
    {
        // Arrange
        string url = "http://localhost:4000/meter-reading-uploads";

        // Act
        using var httpClient = new HttpClient();
        var response = await httpClient.PostAsync(url, null);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UploadMeterReadings_ShouldReturnOk_WhenFileIsValid()
    {
        // Arrange
        string url = "http://localhost:4000/meter-reading-uploads";

        string relativePath = @"..\..\..\..\ensek-test-data\Meter_Reading.csv";
        string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        string filePath = Path.Combine(baseDirectory, relativePath);

        using var httpClient = new HttpClient();
        using var form = new MultipartFormDataContent();

        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var fileContent = new StreamContent(fileStream);

        fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");

        form.Add(fileContent, "file", Path.GetFileName(filePath));

        var expectedData = new UploadResult
        {
            SuccessfulCount = 4,
            FailedCount = 31,
            FailedDetails =
            [
                "Invalid MeterReadValue: 1002 for AccountId 2344",
                "Invalid MeterReadValue: 323 for AccountId 2233",
                "Invalid MeterReadValue: 3440 for AccountId 8766",
                "Invalid MeterReadValue: 1002 for AccountId 2344",
                "Invalid MeterReadValue: 999999 for AccountId 2346",
                "Invalid MeterReadValue: 54 for AccountId 2347",
                "Invalid MeterReadValue: 123 for AccountId 2348",
                "Invalid MeterReadValue: 0 for AccountId 2349",
                "Invalid MeterReadValue: 5684 for AccountId 2350",
                "Invalid MeterReadValue: 455 for AccountId 2352",
                "Invalid MeterReadValue: 1212 for AccountId 2353",
                "Invalid AccountId: 2354",
                "Invalid MeterReadValue: 1 for AccountId 2355",
                "Invalid MeterReadValue: 0 for AccountId 2356",
                "Invalid MeterReadValue: 0 for AccountId 2344",
                "Invalid MeterReadValue: -6575 for AccountId 6776",
                "Invalid MeterReadValue: 0 for AccountId 4534",
                "Invalid MeterReadValue: 9787 for AccountId 1234",
                "Invalid AccountId: 1235",
                "Invalid AccountId: 1236",
                "Invalid AccountId: 1237",
                "Invalid AccountId: 1238",
                "Invalid MeterReadValue: 978 for AccountId 1240",
                "Invalid MeterReadValue: 436 for AccountId 1241",
                "Invalid MeterReadValue: 124 for AccountId 1242",
                "Invalid MeterReadValue: 77 for AccountId 1243",
                "Invalid MeterReadValue: 3478 for AccountId 1244",
                "Invalid MeterReadValue: 676 for AccountId 1245",
                "Invalid MeterReadValue: 3455 for AccountId 1246",
                "Invalid MeterReadValue: 3 for AccountId 1247",
                "Invalid MeterReadValue: 3467 for AccountId 1248"
            ]
        };

        // Act
        var response = await httpClient.PostAsync(url, form);

        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();

        // Deserialize the response JSON
        var actualData = JsonSerializer.Deserialize<UploadResult>(responseData, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(actualData);
        Assert.Equal(expectedData.SuccessfulCount, actualData.SuccessfulCount);
        Assert.Equal(expectedData.FailedCount, actualData.FailedCount);
        Assert.Equal(expectedData.FailedDetails.Count, actualData.FailedDetails.Count);

        for (int i = 0; i < expectedData.FailedDetails.Count; i++)
        {
            Assert.Equal(expectedData.FailedDetails[i], actualData.FailedDetails[i]);
        }
    }

    #endregion

    #region GetMeterReadings

    [Fact]
    public async Task GetMeterReadings_ShouldReturnExpectedData()
    {
        // Arrange
        string url = "http://localhost:4000/meter-readings";

        var expectedData = new List<MeterReading>
        {
            new MeterReading("2345", DateTime.Parse("2019-04-22T12:25:00"), 45522),
            new MeterReading("2351", DateTime.Parse("2019-04-22T12:25:00"), 57579),
            new MeterReading("6776", DateTime.Parse("2019-05-10T09:24:00"), 23566),
            new MeterReading("1239", DateTime.Parse("2019-05-17T09:24:00"), 45345)
        };

        // Act
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();

        var actualData = JsonSerializer.Deserialize<List<MeterReading>>(responseData, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(actualData);
        Assert.Equal(expectedData.Count, actualData.Count);

        for (int i = 0; i < expectedData.Count; i++)
        {
            Assert.Equal(expectedData[i].AccountId, actualData[i].AccountId);
            Assert.Equal(expectedData[i].MeterReadingDateTime, actualData[i].MeterReadingDateTime);
            Assert.Equal(expectedData[i].MeterReadValue, actualData[i].MeterReadValue);
        }
    }

    [Fact]
    public async Task GetMeterReadings_ShouldReturnEmptyList_WhenNoReadingsExist()
    {
        // Arrange
        string url = "http://localhost:4000/meter-readings";

        // Act
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();

        var actualData = JsonSerializer.Deserialize<List<MeterReading>>(responseData, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        // Assert
        Assert.NotNull(actualData);
        Assert.Empty(actualData);
    }

    #endregion
}