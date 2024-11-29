namespace ensek_spark.Models;

public class MeterReading
{
    public Guid AccountId { get; set; }
    public DateTime MeterReadingDateTime { get; set; }
    public decimal MeterReadValue { get; set; } // TODO: or int? float?
}
