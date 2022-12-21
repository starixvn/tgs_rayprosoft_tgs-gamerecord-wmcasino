using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Tgs.GameRounds.Common;
using Tgs.GameRounds.Common.MessageModels;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

internal class GamePullRecordSettledCommandService : ICommandService<GamePullRecordSettledCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly ILogger<GamePullRecordSettledCommandService> _logger;
    private readonly IMessageSender _messageSender;
    private readonly IUtensil _utensil;

    public GamePullRecordSettledCommandService(
        ILogger<GamePullRecordSettledCommandService> logger,
        IOptions<GameRecordInfoOptions> options,
        IMessageSender messageSender,
        IUtensil utensil)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _gameRecordInfo = options.Value;
        _messageSender = messageSender ?? throw new ArgumentNullException(nameof(messageSender));
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask ExecuteAsync(GamePullRecordSettledCommand command, CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil.GetGameProviderIdAsync().ConfigureAwait(false);
        foreach (var record in command.Records)
        {
            bool success = true;
            var playerCode = _utensil.WMCasinoUserToPlayerCode(record.User);
            var playerModel = await _utensil.GetPlayerAsync(playerCode).ConfigureAwait(false);
            var gameId = await _utensil.GetGameIdAsync(gameProviderId, record.Gid.ToString()).ConfigureAwait(false);
            if (playerModel == null || !gameId.HasValue)
            {
                success = false;
            }

            Guid? gameProviderConfigurationId = default;
            if (playerModel != null)
            {
                gameProviderConfigurationId = await _utensil.BrandGetGameProviderConfigurationIdAsync(playerModel.BrandId, gameProviderId).ConfigureAwait(false);
                if (!gameProviderConfigurationId.HasValue)
                {
                    success = false;
                }
            }
            var status = GameRoundStatus.Closed;
            var clientStatus = record.Winloss > 0 ? GameRoundClientStatus.Win : GameRoundClientStatus.Loss;

            var data = new GameRecordMessage(
                ExternalRoundId: record.BetId,
                GameProviderId: gameProviderId,
                GameProviderCode: _gameRecordInfo.GameProviderCode,
                GameId: gameId ?? -1,
                ExternalGameId: record.Gid.ToString(),
                PlayerCode: (playerModel == null) ? string.Empty : playerModel.PlayerCode,
                CurrencyCode: (playerModel == null) ? string.Empty : playerModel.DefaultWalletCurrencyCode,
                WalletId: (playerModel == null) ? Guid.Empty : playerModel.DefaultWalletId,
                BrandId: (playerModel == null) ? Guid.Empty : playerModel.BrandId,
                GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
                Status: status,
                ClientStatus: clientStatus,
                ExternalCreatedOn: record.BetTime.ToDateTimeOffset(),
                ExternalClosedOn: record.Settime.ToDateTimeOffset(),
                BetAmount: record.Bet,
                ValidBetAmount: record.Validbet,
                SettleAmount: record.Winloss + record.Validbet,
                Commission: 0,
                Turnover: record.Validbet,
                PlayerWinLoss: record.Winloss,
                AdditionalData: JsonSerializer.Serialize(record),
                Success: success);

            var subject = _gameRecordInfo.MongoGlobPattern;

            _logger.LogInformation("LogOn:{logOn}  Subject:{subject} MessageRequest:{request}", DateTime.Now.ToString("o"), subject, data);

            await _messageSender
                .PublishAsync(
                    subject: subject,
                    data: data.ToBson(),
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
