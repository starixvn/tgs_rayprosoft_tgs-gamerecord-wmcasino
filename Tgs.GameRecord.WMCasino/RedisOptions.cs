using Tgs.GameRecord.WMCasino.Core;
namespace Tgs.GameRecord.WMCasino;

public record RedisOptions
{
    public string ConnectionString { get; set; } = "localhost";
}
