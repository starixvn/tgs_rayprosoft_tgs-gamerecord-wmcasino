namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

public record ManualPullRecordCommand(
    Uri ApiUrl,
    string VendorId,
    string Signature,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
