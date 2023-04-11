using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

internal class ManualPullRecordCommandService : ICommandService<ManualPullRecordCommand>
{
    private readonly IGameInterfaceWMCasinoServiceGrpc _client;
    private readonly IGameRecordUtensil _gameRecordUtensil;
    private readonly ILogger<ManualPullRecordCommandService> _logger;
    private readonly ICommandService<ManualHandleRecordCommand> _service;

    public ManualPullRecordCommandService(
        ILogger<ManualPullRecordCommandService> logger,
        IGameInterfaceWMCasinoServiceGrpc client,
        IGameRecordUtensil gameRecordUtensil,
        ICommandService<ManualHandleRecordCommand> service)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _gameRecordUtensil = gameRecordUtensil ?? throw new ArgumentNullException(nameof(gameRecordUtensil));
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask ExecuteAsync(ManualPullRecordCommand command, CancellationToken cancellationToken = default)
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

            foreach (var gameRecord in gameRecords)
            {
                try
                {
                    var info = await _gameRecordUtensil
                        .UnsettlementConvertRecord(
                            source: gameRecord,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    await _service
                        .ExecuteAsync(
                            command: new(info),
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

            foreach (var gameRecord in gameRecords)
            {
                try
                {
                    var info = await _gameRecordUtensil
                        .SettlementConvertRecord(
                            source: gameRecord,
                            cancellationToken: cancellationToken)
                        .ConfigureAwait(false);

                    await _service
                        .ExecuteAsync(
                            command: new(info),
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
