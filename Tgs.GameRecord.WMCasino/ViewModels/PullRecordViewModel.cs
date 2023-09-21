namespace Tgs.GameRecord.WMCasino.ViewModels;

public record PullRecordViewModel
{
    public Uri ApiUrl { get; init; } = default!;
    public string VendorId { get; init; } = default!;
    public string Signature { get; init; } = default!;
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset EndTime { get; init; }
}
