namespace Tgs.GameRecord.WMCasino.Core.ApplicationServices;

public record PullRecordCommand(
	Uri ApiUrl,
	string VendorId,
	string Signature,
	DateTimeOffset StartTime,
	DateTimeOffset EndTime);
