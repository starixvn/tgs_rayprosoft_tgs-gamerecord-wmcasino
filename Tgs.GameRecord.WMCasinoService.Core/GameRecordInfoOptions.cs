namespace Tgs.GameRecord.WMCasinoService.Core;

public record GameRecordInfoOptions()
{
    public string GameProviderCode { get; set; } = default!;
    public double MaxIntervalMinute { get; set; } = 30d;
    public double MinIntervalMinute { get; set; } = 1d;
    public double IntervalMinute { get; set; } = 5d;
    public double LockMinute { get; set; } = 4d;
    public string MongoDefine { get; set; } = default!;
    public string MongoGlobPattern { get; set; } = default!;
}
