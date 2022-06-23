using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.WorkerServices
{
    internal class WMCasinoWorkerService : ICommandService<WMCasinoWorkerCommand>
    {
        private readonly GameRecordInfoOptions _gameRecordInfo;
        private readonly IGameInterfaceWMCasinoServiceGrpc _grpc;
        private readonly ILogger<WMCasinoWorkerService> _logger;
        private readonly ICommandService<GameSettleRecordCommand> _settleService;
        private readonly ICommandService<GameUnsettleRecordCommand> _unsettleService;
        private readonly IUtensil _utensil;

        public WMCasinoWorkerService(
            IOptions<GameRecordInfoOptions> options,
            IGameInterfaceWMCasinoServiceGrpc grpc,
            ILogger<WMCasinoWorkerService> logger,
            IUtensil utensil,
            ICommandService<GameSettleRecordCommand> settleService,
            ICommandService<GameUnsettleRecordCommand> unsettleService)
        {
            _gameRecordInfo = options.Value;
            _grpc = grpc ?? throw new ArgumentNullException(nameof(grpc));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
            _settleService = settleService ?? throw new ArgumentNullException(nameof(settleService));
            _unsettleService = unsettleService ?? throw new ArgumentNullException(nameof(unsettleService));
        }

        public async ValueTask ExecuteAsync(WMCasinoWorkerCommand command, CancellationToken cancellationToken = default)
        {
            var gameProviderId = await _utensil.GetGameProviderIdAsync(_gameRecordInfo.GameProviderCode).ConfigureAwait(false);

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
                await _unsettleService.ExecuteAsync(new GameUnsettleRecordCommand(
                    GameProviderId: gameProviderId,
                    GameProviderCode: _gameRecordInfo.GameProviderCode,
                    Records: gameUnsettleRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }

            var gamePullRecordReply = await _grpc.GamePullRecordAsync(
               apiUrl: command.GameApiUrl,
               vendorId: command.GameProviderVendorId,
               signature: command.GameProviderSignature,
               startTime: command.StartTime.UtcDateTime,
               endTime: command.EndTime.UtcDateTime,
               playerCode: null,
               cancellationToken: cancellationToken).ConfigureAwait(false);

            _logger.LogInformation("LogOn:{logOn} | Request:{request} | Response:{response}",
                DateTime.UtcNow.ToString("O"),
                command,
                gamePullRecordReply);

            if (gamePullRecordReply.Items != null && gamePullRecordReply.Items.Any())
            {
                await _settleService.ExecuteAsync(new GameSettleRecordCommand(
                    GameProviderId: gameProviderId,
                    GameProviderCode: _gameRecordInfo.GameProviderCode,
                    Records: gamePullRecordReply.Items), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
