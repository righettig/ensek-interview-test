using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using System.Globalization;

namespace ensek_spark.Models;

public class CustomDateTimeConverter : ITypeConverter
{
    private static readonly string[] _formats = ["dd/MM/yyyy HH:mm"];

    public object? ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
    {
        if (string.IsNullOrWhiteSpace(text)) 
        {
            return null;
        }
       
        if (DateTime.TryParseExact(text, _formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
        {
            return result;
        }

        throw new FormatException($"Unable to convert '{text}' to a DateTime.");
    }

    public string? ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
    {
        return value?.ToString();
    }
}
