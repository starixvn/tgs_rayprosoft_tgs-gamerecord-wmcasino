namespace Tgs.GameRecord.WMCasino.Core;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
