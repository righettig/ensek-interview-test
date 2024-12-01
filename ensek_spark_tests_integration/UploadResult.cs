namespace ensek_spark_tests_integration;

public class UploadResult
{
    public int SuccessfulCount { get; set; }
    public int FailedCount { get; set; }
    public required List<string> FailedDetails { get; set; }
}
