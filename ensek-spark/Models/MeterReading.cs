using CsvHelper.Configuration.Attributes;
using Newtonsoft.Json;

namespace ensek_spark.Models;

public class MeterReading
{
    [JsonProperty("id")]
    public string AccountId { get; set; }

    [TypeConverter(typeof(CustomDateTimeConverter))]
    public DateTime MeterReadingDateTime { get; set; }
    
    public int MeterReadValue { get; set; }
}
