using Tgs.GameInterface.WMCasinoService.Protos;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices
{
    public record GameSettleRecordCommand(Guid GameProviderId, string GameProviderCode, IEnumerable<GameInterfaceWMCasinoServiceGamePullRecord> Records);
}
