namespace Tgs.GameRecord.WMCasinoService
{
    public record ScheduleMessageModel(
        Uri GameApiUrl,
        string GameProviderVendorId,
        string GameProviderSignature);
}
