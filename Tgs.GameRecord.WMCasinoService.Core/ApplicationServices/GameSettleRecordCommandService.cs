using System.Text.Json;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices
{
    internal class GameSettleRecordCommandService : ICommandService<GameSettleRecordCommand>
    {
        private readonly ICommandService<GameRoundCommand> _service;
        private readonly IUtensil _utensil;

        public GameSettleRecordCommandService(
            ICommandService<GameRoundCommand> service,
            IUtensil utensil)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(GameSettleRecordCommand command, CancellationToken cancellationToken = default)
        {
            foreach (var record in command.Records)
            {
                bool success = true;
                var playerModel = await _utensil.GetPlayerAsync(record.User).ConfigureAwait(false);
                var gameId = await _utensil.GetGameIdAsync(command.GameProviderId, record.Gid.ToString()).ConfigureAwait(false);
                if (playerModel == null || !gameId.HasValue)
                {
                    success = false;
                }

                Guid? gameProviderConfigurationId = default;
                if (playerModel != null)
                {
                    gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, command.GameProviderId).ConfigureAwait(false);
                    if (!gameProviderConfigurationId.HasValue)
                    {
                        success = false;
                    }
                }
                var status = GameRoundStatus.Closed;
                var clientStatus = record.Winloss > 0 ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss;

                var gameRoundCommand = new GameRoundCommand(
                    ExternalRoundId: record.BetId,
                    GameProviderId: command.GameProviderId,
                    GameProviderCode: command.GameProviderCode,
                    PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                    GameId: gameId ?? -1,
                    ExternalGameId: record.Gid.ToString(),
                    CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                    Status: status,
                    GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                    WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                    ExternalCreatedOn: record.BetTime.ToDateTimeOffset(),
                    ExternalClosedOn: record.Settime.ToDateTimeOffset(),
                    BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                    BetAmount: record.Bet,
                    ValidBetAmount: record.Validbet,
                    SettleAmount: record.Winloss + record.Validbet,
                    Commission: 0,
                    Turnover: record.Validbet,
                    PlayerWinLoss: record.Winloss,
                    AdditionalData: JsonSerializer.Serialize(record),
                    ClientStatus: clientStatus,
                    Success: success);

                await _service.ExecuteAsync(gameRoundCommand, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
