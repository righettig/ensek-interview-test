using CsvHelper.Configuration;
using ensek_spark.Models;

namespace ensek_spark_tests;

public class CustomDateTimeConverterTests
{
    private readonly CustomDateTimeConverter _converter;

    public CustomDateTimeConverterTests()
    {
        _converter = new CustomDateTimeConverter();
    }

    #region ConvertFromString

    [Theory]
    [InlineData("25/12/2023 14:30", 2023, 12, 25, 14, 30, 0)]
    [InlineData("01/01/2024 00:00", 2024, 1, 1, 0, 0, 0)]
    public void ConvertFromString_ValidDateTime_ParsesSuccessfully(string input,
                                                                   int year,
                                                                   int month,
                                                                   int day,
                                                                   int hour,
                                                                   int minute,
                                                                   int second)
    {
        // Arrange
        var memberMapData = new MemberMapData(null);

        // Act
        var result = _converter.ConvertFromString(input, null, memberMapData);

        // Assert
        Assert.NotNull(result);
        var dateTime = Assert.IsType<DateTime>(result);
        Assert.Equal(year, dateTime.Year);
        Assert.Equal(month, dateTime.Month);
        Assert.Equal(day, dateTime.Day);
        Assert.Equal(hour, dateTime.Hour);
        Assert.Equal(minute, dateTime.Minute);
        Assert.Equal(second, dateTime.Second);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void ConvertFromString_NullOrEmptyString_ReturnsNull(string input)
    {
        // Arrange
        var memberMapData = new MemberMapData(null);

        // Act
        var result = _converter.ConvertFromString(input, null, memberMapData);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("25-12-2023 14:30")]
    [InlineData("12/25/2023 14:30")]
    [InlineData("InvalidDateTime")]
    [InlineData("25/12/2023")]
    public void ConvertFromString_InvalidDateTime_ThrowsFormatException(string input)
    {
        // Arrange
        var memberMapData = new MemberMapData(null);

        // Act & Assert
        Assert.Throws<FormatException>(() => _converter.ConvertFromString(input, null, memberMapData));
    }

    #endregion

    #region ConvertToString

    [Fact]
    public void ConvertToString_ValidDateTime_ConvertsSuccessfully()
    {
        // Arrange
        var dateTime = new DateTime(2023, 12, 25, 14, 30, 0);
        var memberMapData = new MemberMapData(null);

        // Act
        var result = _converter.ConvertToString(dateTime, null, memberMapData);

        // Assert
        Assert.Equal("25/12/2023 14:30:00", result);
    }

    [Fact]
    public void ConvertToString_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;
        var memberMapData = new MemberMapData(null);

        // Act
        var result = _converter.ConvertToString(value, null, memberMapData);

        // Assert
        Assert.Null(result);
    }

    #endregion
}