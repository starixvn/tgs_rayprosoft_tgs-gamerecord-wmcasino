using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.WMCasinoService.Core.Models;

namespace Tgs.GameRecord.WMCasinoService.Core;

public interface IGameRecordUtensil
{
    public ValueTask<GameRecordInfo> SettlementConvertRecord(
        GameInterfaceWMCasinoServiceGamePullRecord source,
        CancellationToken cancellationToken = default);

    public ValueTask<GameRecordInfo> UnsettlementConvertRecord(
            GameInterfaceWMCasinoServiceGameUnsettleRecord source,
        CancellationToken cancellationToken = default);
}
