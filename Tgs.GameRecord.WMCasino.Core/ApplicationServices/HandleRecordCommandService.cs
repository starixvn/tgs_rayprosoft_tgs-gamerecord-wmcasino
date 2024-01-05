using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasino.Core.ApplicationServices;

internal class HandleRecordCommandService(
    IOptions<GameRecordInfoOptions> options,
    IUtensil utensil,
    IGameRecordHandleClient client) : ICommandService<HandleRecordCommand>
{
    private readonly GameRecordInfoOptions _gameRecordInfo = options.Value;

    public ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var playerCode = utensil.WMCasinoUserToPlayerCode(command.Record.User);
        var wagerState = command.Record.Winloss.ToDecimal().GetValueOrDefault(0) <= 0 ? WagerStates.Lose : WagerStates.Win;
        var externalCreatedOn = command.Record.BetTime.ToDateTimeOffset();
        var externalClosedOn = command.Record.Settime.ToDateTimeOffset();
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.Validbet.ToDecimal().GetValueOrDefault(0);
        var turnover = command.Record.Validbet.ToDecimal().GetValueOrDefault(0);
        var playerWinLoss = command.Record.Winloss.ToDecimal().GetValueOrDefault(0);
        var settle = playerWinLoss + validBet;
        var commission = 0.0m;

        return client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            playerCode: playerCode,
            wagerState: wagerState,
            wagerType: WagerTypes.Normal,
            freeGameState: FreeGameStates.None,
            gambleState: GambleStates.None,
            cashoutState: CashoutStates.None,
            resettleState: ResettleStates.None,
            externalRoundId: command.Record.BetId,
            externalWagerId: command.Record.BetId,
            externalGameId: command.Record.Gid.ToString(),
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            externalPayoffOn: externalClosedOn,
            externalResettleOn: DateTimeOffset.UnixEpoch,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            winLoss: playerWinLoss,
            additionalData: JsonNode.Parse(command.Record.AdditionalData)!,
            cancellationToken: cancellationToken);
    }
}
