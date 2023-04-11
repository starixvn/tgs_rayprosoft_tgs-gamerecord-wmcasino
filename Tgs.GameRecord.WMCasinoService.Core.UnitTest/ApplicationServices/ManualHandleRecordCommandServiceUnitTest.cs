using System.Text.Json;
using NSubstitute;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.GameRecord.WMCasinoService.Core.Models;
using Tgs.GameRounds.Work.Manual.Client;
using Tgs.GameRounds.Work.Manual.Common.Protos;

namespace Tgs.GameRecord.WMCasinoService.Core.UnitTest.ApplicationServices;

public class ManualHandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Normal_Process_Settlement_Loss()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkManualClient>();

        var command = new ManualHandleRecordCommand(
            GameRecord: new GameRecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "WMTF",
                ExternalRoundId: "3393785377",
                GameId: 1,
                ExternalGameId: "101",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Closed: true,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-10 23:45:22+08:00"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-04-10 23:45:52+08:00"),
                Bet: 100,
                ValidBet: 100,
                Settle: 0,
                Commission: 0,
                Turnover: 100,
                WinLoss: -100,
                AdditionalData: "{\"BetId\": \"3393785377\"}"
                ));

        var service = new ManualHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Loss),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Settlement_Win()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkManualClient>();

        var command = new ManualHandleRecordCommand(
            GameRecord: new GameRecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "WMTF",
                ExternalRoundId: "3393785377",
                GameId: 1,
                ExternalGameId: "101",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Closed: true,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-10 23:45:22+08:00"),
                ExternalClosedOn: DateTimeOffset.Parse("2023-04-10 23:45:52+08:00"),
                Bet: 100,
                ValidBet: 100,
                Settle: 200,
                Commission: 0,
                Turnover: 100,
                WinLoss: 100,
                AdditionalData: "{\"BetId\": \"3393785377\"}"
                ));

        var service = new ManualHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Closed),
                clientStatus: Arg.Is(RoundClientStatus.Win),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Success_Is_False()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkManualClient>();

        var command = new ManualHandleRecordCommand(
            GameRecord: new GameRecordInfo(
                Success: false,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "WMTF",
                ExternalRoundId: "3393785377",
                GameId: 1,
                ExternalGameId: "101",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Closed: false,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-10 23:45:22+08:00"),
                ExternalClosedOn: null,
                Bet: 100,
                ValidBet: 100,
                Settle: 0,
                Commission: 0,
                Turnover: 0,
                WinLoss: 0,
                AdditionalData: "{\"BetId\": \"3393785377\"}"
                ));

        var service = new ManualHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(0)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Open),
                clientStatus: Arg.Is(RoundClientStatus.Open),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundMissing(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                data: Arg.Is(JsonSerializer.Serialize(command.GameRecord)),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }

    [Fact]
    public async void Normal_Process_Unsettlement()
    {
        var fakeClient = Substitute.For<IGameRoundsWorkManualClient>();

        var command = new ManualHandleRecordCommand(
            GameRecord: new GameRecordInfo(
                Success: true,
                GameProviderId: Guid.NewGuid(),
                GameProviderCode: "WMTF",
                ExternalRoundId: "3393785377",
                GameId: 1,
                ExternalGameId: "101",
                PlayerCode: "4uispltzj9d8",
                WalletId: Guid.NewGuid(),
                CurrencyCode: "PHP",
                BrandId: Guid.NewGuid(),
                GameProviderConfigurationId: Guid.NewGuid(),
                Closed: false,
                ExternalCreatedOn: DateTimeOffset.Parse("2023-04-10 23:45:22+08:00"),
                ExternalClosedOn: null,
                Bet: 100,
                ValidBet: 100,
                Settle: 0,
                Commission: 0,
                Turnover: 0,
                WinLoss: 0,
                AdditionalData: "{\"BetId\": \"3393785377\"}"
                ));

        var service = new ManualHandleRecordCommandService(fakeClient);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        await fakeClient
            .Received(1)
            .RoundBidding(
                gameProviderId: Arg.Is(command.GameRecord.GameProviderId),
                gameProviderCode: Arg.Is(command.GameRecord.GameProviderCode),
                externalRoundId: Arg.Is(command.GameRecord.ExternalRoundId),
                gameId: Arg.Is(command.GameRecord.GameId),
                externalGameId: Arg.Is(command.GameRecord.ExternalGameId),
                playerCode: Arg.Is(command.GameRecord.PlayerCode),
                walletId: Arg.Is(command.GameRecord.WalletId),
                currencyCode: Arg.Is(command.GameRecord.CurrencyCode),
                brandId: Arg.Is(command.GameRecord.BrandId),
                gameProviderConfigurationId: Arg.Is(command.GameRecord.GameProviderConfigurationId),
                status: Arg.Is(RoundStatus.Open),
                clientStatus: Arg.Is(RoundClientStatus.Open),
                externalCreatedOn: Arg.Is(command.GameRecord.ExternalCreatedOn),
                externalClosedOn: Arg.Is(command.GameRecord.ExternalClosedOn),
                bet: Arg.Is(command.GameRecord.Bet),
                validBet: Arg.Is(command.GameRecord.ValidBet),
                settle: Arg.Is(command.GameRecord.Settle),
                commission: Arg.Is(command.GameRecord.Commission),
                turnover: Arg.Is(command.GameRecord.Turnover),
                playerWinLoss: Arg.Is(command.GameRecord.WinLoss),
                additionalData: Arg.Is(command.GameRecord.AdditionalData),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
