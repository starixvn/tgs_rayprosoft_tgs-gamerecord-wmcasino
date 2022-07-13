namespace Tgs.GameRecord.WMCasinoService.Core.WorkerServices
{
    public record WMCasinoWorkerCommand(
        Uri GameApiUrl,
        string GameProviderVendorId,
        string GameProviderSignature,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
