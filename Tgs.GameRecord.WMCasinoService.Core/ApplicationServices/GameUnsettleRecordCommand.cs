using Tgs.GameInterface.WMCasinoService.Protos;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices
{
    public record GameUnsettleRecordCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameInterfaceWMCasinoServiceGameUnsettleRecord> Records);
}
