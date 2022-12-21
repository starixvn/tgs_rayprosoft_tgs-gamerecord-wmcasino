namespace Tgs.GameRecord.WMCasinoService.Core.WorkerServices;

public record GameHistoryWorkerCommand(
    Uri GameApiUrl,
    string GameProviderVendorId,
    string GameProviderSignature,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
