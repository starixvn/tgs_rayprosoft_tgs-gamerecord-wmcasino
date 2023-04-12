using System.Text.Json;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.WMCasinoService.Core.Models;

namespace Tgs.GameRecord.WMCasinoService.Core;

internal class GameRecordUtensil : IGameRecordUtensil
{
    private readonly IUtensil _utensil;

    public GameRecordUtensil(
        IUtensil utensil)
    {
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
    }

    public async ValueTask<RecordInfo> SettlementConvertRecord(
        GameInterfaceWMCasinoServiceGamePullRecord source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;

        var playerCode = _utensil.WMCasinoUserToPlayerCode(source.User);
        var player = await _utensil
            .GetPlayerAsync(
                playerCode: playerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: source.Gid.ToString(),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (player == null || !gameId.HasValue)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (player != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: player.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }

        var externalCreatedOn = source.BetTime.ToDateTimeOffset();
        var externalClosedOn = source.Settime.ToDateTimeOffset();
        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = source.Validbet.ToDecimal().GetValueOrDefault(0);
        var turnover = source.Validbet.ToDecimal().GetValueOrDefault(0);
        var winLoss = source.Winloss.ToDecimal().GetValueOrDefault(0);
        var settle = winLoss + validBet;
        var commission = 0;

        return new RecordInfo(
            Success: success,
            GameProviderId: gameProviderId,
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.BetId,
            GameId: gameId ?? -1,
            ExternalGameId: source.Gid.ToString(),
            PlayerCode: (player == null) ? string.Empty : player.PlayerCode,
            WalletId: (player == null) ? Guid.Empty : player.DefaultWalletId,
            CurrencyCode: (player == null) ? string.Empty : player.DefaultWalletCurrencyCode,
            BrandId: (player == null) ? Guid.Empty : player.BrandId,
            GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
            Closed: true,
            ExternalCreatedOn: externalCreatedOn,
            ExternalClosedOn: externalClosedOn,
            Bet: bet,
            ValidBet: validBet,
            Settle: settle,
            Commission: commission,
            Turnover: turnover,
            WinLoss: winLoss,
            AdditionalData: JsonSerializer.Serialize(source));
    }

    public async ValueTask<RecordInfo> UnsettlementConvertRecord(
        GameInterfaceWMCasinoServiceGameUnsettleRecord source,
        CancellationToken cancellationToken = default)
    {
        var gameProviderId = await _utensil
            .GetGameProviderIdAsync(
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var gameProviderCode = _utensil
            .GetGameProviderCode();

        bool success = true;

        var playerCode = _utensil.WMCasinoUserToPlayerCode(source.User);
        var player = await _utensil
            .GetPlayerAsync(
                playerCode: playerCode,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        var gameId = await _utensil
            .GetGameIdAsync(
                gameProviderId: gameProviderId,
                gameExternalId: source.Gid.ToString(),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (player == null || !gameId.HasValue)
        {
            success = false;
        }

        Guid? gameProviderConfigurationId = default;
        if (player != null)
        {
            gameProviderConfigurationId = await _utensil
                .GetBrandGameProviderConfigurationIdAsync(
                    brandId: player.BrandId,
                    gameProviderId: gameProviderId,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            if (!gameProviderConfigurationId.HasValue)
            {
                success = false;
            }
        }

        var externalCreatedOn = source.BetTime.ToDateTimeOffset();

        var bet = source.Bet.ToDecimal().GetValueOrDefault(0);

        return new RecordInfo(
            Success: success,
            GameProviderId: gameProviderId,
            GameProviderCode: gameProviderCode,
            ExternalRoundId: source.BetId,
            GameId: gameId ?? -1,
            ExternalGameId: source.Gid.ToString(),
            PlayerCode: (player == null) ? string.Empty : player.PlayerCode,
            WalletId: (player == null) ? Guid.Empty : player.DefaultWalletId,
            CurrencyCode: (player == null) ? string.Empty : player.DefaultWalletCurrencyCode,
            BrandId: (player == null) ? Guid.Empty : player.BrandId,
            GameProviderConfigurationId: gameProviderConfigurationId ?? Guid.Empty,
            Closed: false,
            ExternalCreatedOn: externalCreatedOn,
            ExternalClosedOn: null,
            Bet: bet,
            ValidBet: bet,
            Settle: 0.0m,
            Commission: 0.0m,
            Turnover: 0.0m,
            WinLoss: 0.0m,
            AdditionalData: JsonSerializer.Serialize(source));
    }
}
