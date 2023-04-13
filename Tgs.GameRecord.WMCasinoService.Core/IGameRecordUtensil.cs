using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.WMCasinoService.Core.Models;

namespace Tgs.GameRecord.WMCasinoService.Core;

public interface IGameRecordUtensil
{
    public ValueTask<RecordInfo> SettlementConvertRecordAsync(
        GamePullRecord source,
        CancellationToken cancellationToken = default);

    public ValueTask<RecordInfo> UnsettlementConvertRecordAsync(
        GameUnsettleRecord source,
        CancellationToken cancellationToken = default);
}
