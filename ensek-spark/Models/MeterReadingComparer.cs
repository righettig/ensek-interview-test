namespace ensek_spark.Models;

public class MeterReadingComparer : IEqualityComparer<MeterReading>
{
    public bool Equals(MeterReading x, MeterReading y)
    {
        return x.AccountId == y.AccountId &&
               x.MeterReadingDateTime == y.MeterReadingDateTime &&
               x.MeterReadValue == y.MeterReadValue;
    }

    public int GetHashCode(MeterReading obj)
    {
        return HashCode.Combine(obj.AccountId, obj.MeterReadingDateTime, obj.MeterReadValue);
    }
}
