using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasino.Core.ApplicationServices;

internal class PullRecordCommandService : ICommandService<PullRecordCommand>
{
    private readonly IGameInterfaceWMCasinoServiceClient _client;
    private readonly ILogger<PullRecordCommandService> _logger;
    private readonly ICommandService<HandleRecordCommand> _service;

    public PullRecordCommandService(
        ILogger<PullRecordCommandService> logger,
        IGameInterfaceWMCasinoServiceClient client,
        ICommandService<HandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(PullRecordCommand command, CancellationToken cancellationToken = default)
    {
        var gamePullRecordReply = await _client
            .GamePullRecordAsync(
                apiUrl: command.ApiUrl,
                vendorId: command.VendorId,
                signature: command.Signature,
                startTime: command.StartTime.UtcDateTime,
                endTime: command.EndTime.UtcDateTime,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameRecords = gamePullRecordReply.Items ?? Enumerable.Empty<GamePullRecord>();

        _logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

        foreach (var gameRecord in gameRecords)
        {
            _logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

            try
            {
                await _service
                    .ExecuteAsync(
                    command: new(gameRecord),
                    cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message: An exception occurred in this game record.({gameRecord})", DateTime.Now.ToString("o"), gameRecord);
            }
        }
    }
}
