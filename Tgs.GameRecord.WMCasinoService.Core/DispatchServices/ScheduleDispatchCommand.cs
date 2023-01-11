namespace Tgs.GameRecord.WMCasinoService.Core.DispatchServices;

public record ScheduleDispatchCommand(
    Uri GameApiUrl,
    string GameProviderVendorId,
    string GameProviderSignature);
