namespace Tgs.GameRecord.WMCasinoService.Core.WorkerServices;

public record ScheduleWorkerCommand(
    Uri GameApiUrl,
    string GameProviderVendorId,
    string GameProviderSignature,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
