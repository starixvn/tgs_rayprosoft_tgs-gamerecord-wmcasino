namespace Tgs.GameRecord.WMCasinoService
{
    public record ManualMessageRequest(
        Uri GameApiUrl,
        string GameProviderVendorId,
        string GameProviderSignature,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
