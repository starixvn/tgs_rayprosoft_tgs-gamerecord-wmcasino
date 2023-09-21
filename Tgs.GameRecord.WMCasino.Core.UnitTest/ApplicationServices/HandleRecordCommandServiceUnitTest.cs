using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.Handle.Client;
using Tgs.GameRecord.Handle.Common.Protos;
using Tgs.GameRecord.WMCasino.Core.ApplicationServices;

namespace Tgs.GameRecord.WMCasino.Core.UnitTest.ApplicationServices;

public class HandleRecordCommandServiceUnitTest
{
    [Fact]
    public async void Loss()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "WMTF"
        });
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GamePullRecord
            {
                User = "wmtf_4ukghxddxmgm_Sky8",
                BetId = "3642915116",
                BetTime = DateTimeOffset.Parse("2023-08-17T11:13:17+08:00").ToTimestamp(),
                Bet = 500m,
                Validbet = 500m,
                Water = 0.0m,
                Result = -500m,
                BetCode = "Player",
                Waterbet = 500m,
                Winloss = -500m,
                Ip = "121.202.127.3",
                Gid = "101",
                Event = "113523802",
                EventChild = "36",
                Round = "113523802",
                Subround = "36",
                TableId = "116",
                Commission = "0",
                Settime = DateTimeOffset.Parse("2023-08-17T11:13:37+08:00").ToTimestamp(),
                Reset = "N",
                BetResult = "Player",
                GameResult = "Banker:♥7♣8♥2 Player:♣K♣6",
                Gname = "Baccarat",
                AdditionalData = "{\"user\":\"wmtf_4ukghxddxmgm_Sky8\",\"betId\":\"3642915116\",\"betTime\":\"2023-08-17 11:13:17\",\"beforeCash\":\"1026.0000\",\"bet\":\"500.0000\",\"validbet\":\"500.0000\",\"water\":\"0.0000\",\"result\":\"-500.0000\",\"betCode\":\"Player\",\"waterbet\":\"500.0000\",\"winLoss\":\"-500.0000\",\"ip\":\"121.202.127.3\",\"gid\":\"101\",\"event\":\"113523802\",\"eventChild\":\"36\",\"round\":\"113523802\",\"subround\":\"36\",\"tableId\":\"116\",\"commission\":\"0\",\"settime\":\"2023-08-17 11:13:37\",\"reset\":\"N\",\"betResult\":\"Player\",\"gameResult\":\"Banker:♥7♣8♥2 Player:♣K♣6\",\"gname\":\"Baccarat\"}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeUtensil,
            fakeClient);

        var playerCode = "4ukghxddxmgm";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
            Arg.Is(command.Record.User))
            .Returns(playerCode);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(playerCode),
            wagerState: Arg.Is(WagerStates.Lose),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Record.BetId),
            externalWagerId: Arg.Is(command.Record.BetId),
            externalGameId: Arg.Is(command.Record.Gid.ToString()),
            externalCreatedOn: Arg.Is(command.Record.BetTime.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.Settime.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Record.Settime.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(500m),
            validBet: Arg.Is(500m),
            settle: Arg.Is(0.0m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(500m),
            winLoss: Arg.Is(-500m),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public async void Win()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "WMTF"
        });
        var fakeUtensil = Substitute.For<IUtensil>();
        var fakeClient = Substitute.For<IGameRecordHandleClient>();

        var command = new HandleRecordCommand(
            Record: new GamePullRecord
            {
                User = "wmtf_4uh6v1kvxurl_PB",
                BetId = "3642911555",
                BetTime = DateTimeOffset.Parse("2023-08-17T11:10:57+08:00").ToTimestamp(),
                Bet = 50m,
                Validbet = 50m,
                Water = 0.0m,
                Result = 50m,
                BetCode = "Banker",
                Waterbet = 50m,
                Winloss = 50m,
                Ip = "110.54.191.87",
                Gid = "101",
                Event = "113523806",
                EventChild = "25",
                Round = "113523806",
                Subround = "25",
                TableId = "113",
                Commission = "1",
                Settime = DateTimeOffset.Parse("2023-08-17T11:11:18+08:00").ToTimestamp(),
                Reset = "N",
                BetResult = "Banker",
                GameResult = "Banker:♦A♥7 Player:♠8♦3",
                Gname = "Baccarat",
                AdditionalData = "{\"user\":\"wmtf_4uh6v1kvxurl_PB\",\"betId\":\"3642911555\",\"betTime\":\"2023-08-17 11:10:57\",\"beforeCash\":\"101.5000\",\"bet\":\"50.0000\",\"validbet\":\"50.0000\",\"water\":\"0.0000\",\"result\":\"50.0000\",\"betCode\":\"Banker\",\"waterbet\":\"50.0000\",\"winLoss\":\"50.0000\",\"ip\":\"110.54.191.87\",\"gid\":\"101\",\"event\":\"113523806\",\"eventChild\":\"25\",\"round\":\"113523806\",\"subround\":\"25\",\"tableId\":\"113\",\"commission\":\"1\",\"settime\":\"2023-08-17 11:11:18\",\"reset\":\"N\",\"betResult\":\"Banker\",\"gameResult\":\"Banker:♦A♥7 Player:♠8♦3\",\"gname\":\"Baccarat\"}"
            });

        var service = new HandleRecordCommandService(
            gameRecordInfo,
            fakeUtensil,
            fakeClient);

        var playerCode = "4uh6v1kvxurl";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
            Arg.Is(command.Record.User))
            .Returns(playerCode);

        await service.ExecuteAsync(command);

        await fakeClient
            .Received(1)
            .MessageAsync(
            gameProviderCode: Arg.Is(gameRecordInfo.Value.GameProviderCode),
            playerCode: Arg.Is(playerCode),
            wagerState: Arg.Is(WagerStates.Win),
            wagerType: Arg.Is(WagerTypes.Normal),
            freeGameState: Arg.Is(FreeGameStates.None),
            gambleState: Arg.Is(GambleStates.None),
            cashoutState: Arg.Is(CashoutStates.None),
            resettleState: Arg.Is(ResettleStates.None),
            externalRoundId: Arg.Is(command.Record.BetId),
            externalWagerId: Arg.Is(command.Record.BetId),
            externalGameId: Arg.Is(command.Record.Gid.ToString()),
            externalCreatedOn: Arg.Is(command.Record.BetTime.ToDateTimeOffset()),
            externalClosedOn: Arg.Is(command.Record.Settime.ToDateTimeOffset()),
            externalPayoffOn: Arg.Is(command.Record.Settime.ToDateTimeOffset()),
            externalResettleOn: Arg.Is(DateTimeOffset.UnixEpoch),
            bet: Arg.Is(50m),
            validBet: Arg.Is(50m),
            settle: Arg.Is(100m),
            commission: Arg.Is(0.0m),
            turnover: Arg.Is(50m),
            winLoss: Arg.Is(50m),
            additionalData: Arg.Is(command.Record.AdditionalData),
            cancellationToken: Arg.Any<CancellationToken>());
    }
}
