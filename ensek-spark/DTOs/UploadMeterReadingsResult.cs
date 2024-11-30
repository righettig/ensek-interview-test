namespace ensek_spark.DTOs;

public record UploadMeterReadingsResult(int SuccessfulCount, int FailedCount, List<string>? FailedDetails);
