namespace Tgs.GameRecord.WMCasino.Core;

public record GameRecordInfoOptions()
{
	public string GameProviderCode { get; set; } = default!;
	public string GameProviderConfiguration { get; init; } = default!;

	public string HangfirePrefix { get; set; } = "gamerecord-wmtf-hangfire:";
	public string HangfireLocking { get; set; } = "gamerecord-wmtf-locking:";

	public double IntervalMinutes { get; init; } = 5d;

	public double IntervalMinutesMax { get; init; } = 30d;

	public double IntervalMinutesMinimum { get; init; } = 1d;
}
