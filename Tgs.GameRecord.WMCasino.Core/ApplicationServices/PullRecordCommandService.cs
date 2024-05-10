using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasino.Core.ApplicationServices;

internal class PullRecordCommandService(
	ILogger<PullRecordCommandService> logger,
	IGameInterfaceWMCasinoServiceClient client,
	ICommandService<HandleRecordCommand> service) : ICommandService<PullRecordCommand>
{
	public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
	{
		var gamePullRecordReply = await client
			.GamePullRecordAsync(
				apiUrl: command.ApiUrl,
				vendorId: command.VendorId,
				signature: command.Signature,
				startTime: command.StartTime.UtcDateTime,
				endTime: command.EndTime.UtcDateTime,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var gameRecords = gamePullRecordReply.Items ?? Enumerable.Empty<GamePullRecord>();

		logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

		foreach (var gameRecord in gameRecords)
		{
			logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

			try
			{
				await service
					.ExecuteAsync(
					command: new(gameRecord),
					cancellationToken: cancellationToken)
					.ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameRecord);
			}
		}
	}
}
