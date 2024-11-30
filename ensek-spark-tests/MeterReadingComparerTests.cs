using ensek_spark.Models;

namespace ensek_spark_tests;

public class MeterReadingComparerTests
{
    private readonly MeterReadingComparer _comparer;

    public MeterReadingComparerTests()
    {
        _comparer = new MeterReadingComparer();
    }

    #region Equals

    [Fact]
    public void Equals_SameObjects_ReturnsTrue()
    {
        // Arrange
        var reading1 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };
        var reading2 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };

        // Act
        var result = _comparer.Equals(reading1, reading2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Equals_DifferentAccountId_ReturnsFalse()
    {
        // Arrange
        var reading1 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };
        var reading2 = new MeterReading
        {
            AccountId = "124",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };

        // Act
        var result = _comparer.Equals(reading1, reading2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_DifferentMeterReadingDateTime_ReturnsFalse()
    {
        // Arrange
        var reading1 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };
        var reading2 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 2, 12, 0, 0),
            MeterReadValue = 100
        };

        // Act
        var result = _comparer.Equals(reading1, reading2);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void Equals_DifferentMeterReadValue_ReturnsFalse()
    {
        // Arrange
        var reading1 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };
        var reading2 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 101
        };

        // Act
        var result = _comparer.Equals(reading1, reading2);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetHashCode

    [Fact]
    public void GetHashCode_SameObject_ReturnsSameHashCode()
    {
        // Arrange
        var reading1 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };

        // Act
        var hashCode1 = _comparer.GetHashCode(reading1);
        var hashCode2 = _comparer.GetHashCode(reading1);

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentObjectsWithSameData_ReturnsSameHashCode()
    {
        // Arrange
        var reading1 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };
        var reading2 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };

        // Act
        var hashCode1 = _comparer.GetHashCode(reading1);
        var hashCode2 = _comparer.GetHashCode(reading2);

        // Assert
        Assert.Equal(hashCode1, hashCode2);
    }

    [Fact]
    public void GetHashCode_DifferentObjects_ReturnsDifferentHashCodes()
    {
        // Arrange
        var reading1 = new MeterReading
        {
            AccountId = "123",
            MeterReadingDateTime = new DateTime(2024, 1, 1, 12, 0, 0),
            MeterReadValue = 100
        };
        var reading2 = new MeterReading
        {
            AccountId = "124",
            MeterReadingDateTime = new DateTime(2024, 1, 2, 12, 0, 0),
            MeterReadValue = 101
        };

        // Act
        var hashCode1 = _comparer.GetHashCode(reading1);
        var hashCode2 = _comparer.GetHashCode(reading2);

        // Assert
        Assert.NotEqual(hashCode1, hashCode2);
    }

    #endregion
}