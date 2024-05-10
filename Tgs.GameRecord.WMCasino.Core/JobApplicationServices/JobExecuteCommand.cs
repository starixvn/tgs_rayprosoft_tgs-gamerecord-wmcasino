namespace Tgs.GameRecord.WMCasino.Core.JobApplicationServices;

public record JobExecuteCommand(
	string GameProviderCode,
	string Name,
	Uri ApiUrl,
	string VendorId,
	string Signature);
