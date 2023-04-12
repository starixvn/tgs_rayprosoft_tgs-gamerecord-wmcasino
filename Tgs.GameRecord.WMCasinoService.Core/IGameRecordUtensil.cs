using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.WMCasinoService.Core.Models;

namespace Tgs.GameRecord.WMCasinoService.Core;

public interface IGameRecordUtensil
{
    public ValueTask<RecordInfo> SettlementConvertRecordAsync(
        GameInterfaceWMCasinoServiceGamePullRecord source,
        CancellationToken cancellationToken = default);

    public ValueTask<RecordInfo> UnsettlementConvertRecordAsync(
        GameInterfaceWMCasinoServiceGameUnsettleRecord source,
        CancellationToken cancellationToken = default);
}
