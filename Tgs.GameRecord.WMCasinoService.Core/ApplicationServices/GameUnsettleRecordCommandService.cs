using System.Text.Json;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices
{
    internal class GameUnsettleRecordCommandService : ICommandService<GameUnsettleRecordCommand>
    {
        private readonly ICommandService<GameRoundCommand> _service;
        private readonly IUtensil _utensil;

        public GameUnsettleRecordCommandService(
            ICommandService<GameRoundCommand> service,
            IUtensil utensil)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        }

        public async ValueTask ExecuteAsync(GameUnsettleRecordCommand command, CancellationToken cancellationToken = default)
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

                var gameRoundCommand = new GameRoundCommand(
                    ExternalRoundId: record.BetId,
                    GameProviderId: command.GameProviderId,
                    GameProviderCode: command.GameProviderCode,
                    PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                    GameId: gameId ?? -1,
                    ExternalGameId: record.Gid.ToString(),
                    CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                    Status: GameRoundStatus.Open,
                    GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                    WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                    ExternalCreatedOn: record.BetTime.ToDateTimeOffset(),
                    ExternalClosedOn: null,
                    BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                    BetAmount: record.Bet,
                    ValidBetAmount: record.Bet,
                    SettleAmount: null,
                    Commission: 0,
                    Turnover: null,
                    PlayerWinLoss: null,
                    AdditionalData: JsonSerializer.Serialize(record),
                    ClientStatus: GameRoundClientStatus.Open,
                    Success: success);

                await _service.ExecuteAsync(gameRoundCommand, cancellationToken).ConfigureAwait(false);
            }
        }
    }
}
