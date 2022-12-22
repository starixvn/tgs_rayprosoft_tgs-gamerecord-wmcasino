namespace Tgs.GameRecord.WMCasinoService;

public record NatsSettingOptions
{
    public string Group { get; init; } = default!;

    public string Subject { get; set; } = default!;
}
