using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.WorkerServices;

internal class GameHistoryWorkerService : ICommandService<GameHistoryWorkerCommand>
{
    private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
    private readonly ILogger<GameHistoryWorkerService> _logger;
    private readonly ICommandService<GamePullRecordSettledCommand> _settleService;
    private readonly ICommandService<GamePullRecordUnsettledCommand> _unsettleService;

    public GameHistoryWorkerService(
        IGameInterfaceWMCasinoServiceGrpc grpc,
        ILogger<GameHistoryWorkerService> logger,
        ICommandService<GamePullRecordSettledCommand> settleService,
        ICommandService<GamePullRecordUnsettledCommand> unsettleService)
    {
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settleService = settleService ?? throw new ArgumentNullException(nameof(settleService));
        _unsettleService = unsettleService ?? throw new ArgumentNullException(nameof(unsettleService));
    }

    public async ValueTask ExecuteAsync(GameHistoryWorkerCommand command, CancellationToken cancellationToken = default)
    {
        var gameUnsettleRecordReply = await _grpc.GameUnsettleAsync(
            apiUrl: command.GameApiUrl,
            vendorId: command.GameProviderVendorId,
            signature: command.GameProviderSignature,
            time: command.StartTime.UtcDateTime,
            cancellationToken: cancellationToken);

        _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
            DateTime.UtcNow.ToString("O"),
            command,
            gameUnsettleRecordReply);

        if (gameUnsettleRecordReply.Items != null && gameUnsettleRecordReply.Items.Any())
        {
            await _unsettleService.ExecuteAsync(new GamePullRecordUnsettledCommand(
                Records: gameUnsettleRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        var gamePullRecordReply = await _grpc.GamePullRecordAsync(
           apiUrl: command.GameApiUrl,
           vendorId: command.GameProviderVendorId,
           signature: command.GameProviderSignature,
           startTime: command.StartTime.UtcDateTime,
           endTime: command.EndTime.UtcDateTime,
           cancellationToken: cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
            DateTime.UtcNow.ToString("O"),
            command,
            gamePullRecordReply);

        if (gamePullRecordReply.Items != null && gamePullRecordReply.Items.Any())
        {
            await _settleService.ExecuteAsync(new GamePullRecordSettledCommand(
                Records: gamePullRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
