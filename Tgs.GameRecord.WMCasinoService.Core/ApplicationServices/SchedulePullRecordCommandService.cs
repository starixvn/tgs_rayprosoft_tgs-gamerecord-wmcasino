using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

internal class SchedulePullRecordCommandService : ICommandService<SchedulePullRecordCommand>
{
    private readonly IGameInterfaceWMCasinoServiceGrpc _client;
    private readonly IGameRecordUtensil _gameRecordUtensil;
    private readonly ILogger<SchedulePullRecordCommandService> _logger;
    private readonly ICommandService<ScheduleHandleRecordCommand> _service;

    public SchedulePullRecordCommandService(
        ILogger<SchedulePullRecordCommandService> logger,
        IGameInterfaceWMCasinoServiceGrpc client,
        IGameRecordUtensil gameRecordUtensil,
        ICommandService<ScheduleHandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordUtensil = gameRecordUtensil ?? throw new ArgumentNullException(nameof(gameRecordUtensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(SchedulePullRecordCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            var gameUnsettleRecordReply = await _client
                .GameUnsettleAsync(
                    apiUrl: command.ApiUrl,
                    vendorId: command.VendorId,
                    signature: command.Signature,
                    time: command.StartTime.UtcDateTime,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            var gameRecords = gameUnsettleRecordReply.Items ?? Enumerable.Empty<GameInterfaceWMCasinoServiceGameUnsettleRecord>();

            _logger.LogDebug("LogOn:{logOn} Message: Unsettlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

            foreach (var gameRecord in gameRecords)
            {
                _logger.LogDebug("LogOn:{logOn} Message: Unsettlement. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

                try
                {
                    var record = await _gameRecordUtensil
                        .UnsettlementConvertRecordAsync(
                            source: gameRecord,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug("LogOn:{logOn} Message: Unsettlement. Record:{record}", DateTime.Now.ToString("o"), record);

                    await _service
                        .ExecuteAsync(
                            command: new(record),
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "遊戲紀錄處理發生例外.({gameRecord})", gameRecord);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Message:此未結算程序發生異常.", DateTime.UtcNow.ToString("O"));
        }

        try
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

            var gameRecords = gamePullRecordReply.Items ?? Enumerable.Empty<GameInterfaceWMCasinoServiceGamePullRecord>();

            _logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecords:{gameRecords}", DateTime.Now.ToString("o"), gameRecords);

            foreach (var gameRecord in gameRecords)
            {
                _logger.LogDebug("LogOn:{logOn} Message: Settlement. GameRecord:{gameRecord}", DateTime.Now.ToString("o"), gameRecord);

                try
                {

                    var record = await _gameRecordUtensil
                        .SettlementConvertRecordAsync(
                            source: gameRecord,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    _logger.LogDebug("LogOn:{logOn} Message: Settlement. Record:{record}", DateTime.Now.ToString("o"), record);

                    await _service
                        .ExecuteAsync(
                            command: new(record),
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "遊戲紀錄處理發生例外.({gameRecord})", gameRecord);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Message:此已結算程序發生異常.", DateTime.UtcNow.ToString("O"));
        }
    }
}
