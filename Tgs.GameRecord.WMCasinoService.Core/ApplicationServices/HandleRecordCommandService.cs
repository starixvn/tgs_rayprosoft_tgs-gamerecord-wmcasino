using Microsoft.Extensions.Options;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

internal class HandleRecordCommandService : ICommandService<HandleRecordCommand>
{
    private readonly IGameRecordHandleClient _client;
    private readonly GameRecordInfoOptions _gameRecordInfo;
    private readonly IUtensil _utensil;

    public HandleRecordCommandService(
        IOptions<GameRecordInfoOptions> options,
        IUtensil utensil,
        IGameRecordHandleClient client
        )
    {
        _gameRecordInfo = options.Value;
        _utensil = utensil ?? throw new ArgumentNullException(nameof(utensil));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(HandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        var playerCode = _utensil.WMCasinoUserToPlayerCode(command.Record.User);
        var status = GameRecordStatus.Closed;
        var clientStatus = command.Record.Winloss.ToDecimal().GetValueOrDefault(0) <= 0 ? GameRecordClientStatus.Loss : GameRecordClientStatus.Win;
        var externalCreatedOn = command.Record.BetTime.ToDateTimeOffset();
        var externalClosedOn = command.Record.Settime.ToDateTimeOffset();
        var bet = command.Record.Bet.ToDecimal().GetValueOrDefault(0);
        var validBet = command.Record.Validbet.ToDecimal().GetValueOrDefault(0);
        var turnover = command.Record.Validbet.ToDecimal().GetValueOrDefault(0);
        var playerWinLoss = command.Record.Winloss.ToDecimal().GetValueOrDefault(0);
        var settle = playerWinLoss + validBet;
        var commission = 0.0m;

        await _client
            .MessageAsync(
            gameProviderCode: _gameRecordInfo.GameProviderCode,
            externalRoundId: command.Record.BetId,
            externalGameId: command.Record.Gid.ToString(),
            playerCode: playerCode,
            status: status,
            clientStatus: clientStatus,
            externalCreatedOn: externalCreatedOn,
            externalClosedOn: externalClosedOn,
            bet: bet,
            validBet: validBet,
            settle: settle,
            commission: commission,
            turnover: turnover,
            playerWinLoss: playerWinLoss,
            additionalData: command.Record.AdditionalData,
            cancellationToken: cancellationToken)
        .ConfigureAwait(false);
    }
}
