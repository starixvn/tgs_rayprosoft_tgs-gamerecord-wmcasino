namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

public record SchedulePullRecordCommand(
    Uri ApiUrl,
    string VendorId,
    string Signature,
    DateTimeOffset StartTime,
    DateTimeOffset EndTime);
