using System.Diagnostics;
using System.Net.Http.Headers;

string baseUrl = "http://localhost:4000";
string uploadMeterReadingsUrl = $"{baseUrl}/meter-reading-uploads";
string getMeterReadingsUrl = $"{baseUrl}/meter-readings";


// Measure latency for POST request
//Console.WriteLine("Measuring POST endpoint latency...");

//string relativePath = @"..\..\..\..\ensek-test-data\Meter_Reading.csv";
//string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativePath);

//using var form = new MultipartFormDataContent();
//using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

//var fileContent = new StreamContent(fileStream);
//fileContent.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
//form.Add(fileContent, "file", Path.GetFileName(filePath));

//var postLatency = await MeasureLatencyAsync(uploadMeterReadingsUrl, HttpMethod.Post, form);
//Console.WriteLine($"POST latency: {postLatency.TotalMilliseconds} ms");


// Measure latency for GET request
Console.WriteLine("Measuring GET endpoint latency...");
var getLatency = await MeasureLatencyAsync(getMeterReadingsUrl, HttpMethod.Get);
Console.WriteLine($"GET latency: {getLatency.TotalMilliseconds} ms");


static async Task<TimeSpan> MeasureLatencyAsync(string url, HttpMethod method, HttpContent? content = null)
{
    using var client = new HttpClient();
    using var request = new HttpRequestMessage(method, url)
    {
        Content = content
    };

    var stopwatch = Stopwatch.StartNew();
    try
    {
        var response = await client.SendAsync(request);
        stopwatch.Stop();

        Console.WriteLine($"Status code: {response.StatusCode}");
    }
    catch (Exception ex)
    {
        stopwatch.Stop();
        Console.WriteLine($"Error: {ex.Message}");
    }

    return stopwatch.Elapsed;
}