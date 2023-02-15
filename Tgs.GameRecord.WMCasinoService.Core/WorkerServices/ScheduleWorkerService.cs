using Microsoft.Extensions.Logging;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.WorkerServices;

internal class ScheduleWorkerService : ICommandService<ScheduleWorkerCommand>
{
    private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
    private readonly ILogger<ScheduleWorkerService> _logger;
    private readonly ICommandService<SettlementCommand> _settleService;
    private readonly ICommandService<UnsettledCommand> _unsettleService;

    public ScheduleWorkerService(
        IGameInterfaceWMCasinoServiceGrpc grpc,
        ILogger<ScheduleWorkerService> logger,
        ICommandService<SettlementCommand> settleService,
        ICommandService<UnsettledCommand> unsettleService)
    {
        _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _settleService = settleService ?? throw new ArgumentNullException(nameof(settleService));
        _unsettleService = unsettleService ?? throw new ArgumentNullException(nameof(unsettleService));
    }

    public async ValueTask ExecuteAsync(ScheduleWorkerCommand command, CancellationToken cancellationToken = default)
    {
        try
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
                await _unsettleService.ExecuteAsync(new UnsettledCommand(
                    Records: gameUnsettleRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Message:此未結算程序發生異常.", DateTime.UtcNow.ToString("O"));
        }

        try
        {
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
                await _settleService.ExecuteAsync(new SettlementCommand(
                    Records: gamePullRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "LogOn:{logOn} Message:此已結算程序發生異常.", DateTime.UtcNow.ToString("O"));
        }
    }
}
