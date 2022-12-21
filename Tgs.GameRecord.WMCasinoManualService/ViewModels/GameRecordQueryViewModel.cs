namespace Tgs.GameRecord.WMCasinoManualService.ViewModels
{
    public record GameRecordQueryViewModel(
        Uri GameApiUrl,
        string GameProviderVendorId,
        string GameProviderSignature,
        DateTimeOffset StartTime,
        DateTimeOffset EndTime);
}
