using System.Text.Json;
using Tgs.GameRounds.Work.Manual.Client;
using Tgs.GameRounds.Work.Manual.Common.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

internal class ManualHandleRecordCommandService : ICommandService<ManualHandleRecordCommand>
{
    private readonly IGameRoundsWorkManualClient _client;

    public ManualHandleRecordCommandService(
        IGameRoundsWorkManualClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask ExecuteAsync(ManualHandleRecordCommand command, CancellationToken cancellationToken = default)
    {
        if (command.GameRecord.Success)
        {
            var status = RoundStatus.Open;
            var clientStatus = RoundClientStatus.Open;
            if (command.GameRecord.Closed)
            {
                status = RoundStatus.Closed;
                clientStatus = command.GameRecord.WinLoss <= 0 ? RoundClientStatus.Loss : RoundClientStatus.Win;
            }

            await _client
            .RoundBidding(
                gameProviderId: command.GameRecord.GameProviderId,
                gameProviderCode: command.GameRecord.GameProviderCode,
                externalRoundId: command.GameRecord.ExternalRoundId,
                gameId: command.GameRecord.GameId,
                externalGameId: command.GameRecord.ExternalGameId,
                playerCode: command.GameRecord.PlayerCode,
                walletId: command.GameRecord.WalletId,
                currencyCode: command.GameRecord.CurrencyCode,
                brandId: command.GameRecord.BrandId,
                gameProviderConfigurationId: command.GameRecord.GameProviderConfigurationId,
                status: status,
                clientStatus: clientStatus,
                externalCreatedOn: command.GameRecord.ExternalCreatedOn,
                externalClosedOn: command.GameRecord.ExternalClosedOn,
                bet: command.GameRecord.Bet,
                validBet: command.GameRecord.ValidBet,
                settle: command.GameRecord.Settle,
                commission: command.GameRecord.Commission,
                turnover: command.GameRecord.Turnover,
                playerWinLoss: command.GameRecord.WinLoss,
                additionalData: command.GameRecord.AdditionalData,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        }
        else
        {
            await _client
            .RoundMissing(
                gameProviderId: command.GameRecord.GameProviderId,
                gameProviderCode: command.GameRecord.GameProviderCode,
                externalRoundId: command.GameRecord.ExternalRoundId,
                gameId: command.GameRecord.GameId,
                externalGameId: command.GameRecord.ExternalGameId,
                playerCode: command.GameRecord.PlayerCode,
                data: JsonSerializer.Serialize(command.GameRecord),
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        }
    }
}
