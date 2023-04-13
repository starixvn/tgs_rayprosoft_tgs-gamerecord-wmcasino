using Google.Protobuf.WellKnownTypes;
using NSubstitute;
using Tgs.GameCommand.Common;
using Tgs.GameInterface.WMCasinoService.Protos;

namespace Tgs.GameRecord.WMCasinoService.Core.UnitTest;

public class GameRecordUtensilUnitTest
{
    [Fact]
    public async Task Settlement_Brand_Game_Provider_Setting_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GamePullRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Validbet = 100,
            Water = 0,
            Result = -100,
            BetCode = "Banker",
            Waterbet = 100,
            Winloss = -100,
            Ip = "136.158.24.8",
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            Settime = DateTimeOffset.Parse("2023-04-13T15:58:25+08:00").ToTimestamp(),
            Reset = "N",
            BetResult = "Banker",
            GameResult = "Banker:♣2♥5 Player:♥Q♠9",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"beforeCash\":\"701.1500\",\"bet\":\"100.0000\",\"validbet\":\"100.0000\",\"water\":\"0.0000\",\"result\":\"-100.0000\",\"betCode\":\"Banker\",\"waterbet\":\"100.0000\",\"winLoss\":\"-100.0000\",\"ip\":\"136.158.24.8\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"settime\":\"2023-04-13 15:58:25\",\"reset\":\"N\",\"betResult\":\"Banker\",\"gameResult\":\"Banker:\\\\u26663\\\\u2663A\\\\u26632 Player:\\\\u2660K\\\\u2666A\\\\u26666\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: "4uispltzj9d8",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(playerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(Guid.Empty, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.Settime.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0) + source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Settle);
    }

    [Fact]
    public async Task Settlement_Game_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GamePullRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Validbet = 100,
            Water = 0,
            Result = -100,
            BetCode = "Banker",
            Waterbet = 100,
            Winloss = -100,
            Ip = "136.158.24.8",
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            Settime = DateTimeOffset.Parse("2023-04-13T15:58:25+08:00").ToTimestamp(),
            Reset = "N",
            BetResult = "Banker",
            GameResult = "Banker:♣2♥5 Player:♥Q♠9",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"beforeCash\":\"701.1500\",\"bet\":\"100.0000\",\"validbet\":\"100.0000\",\"water\":\"0.0000\",\"result\":\"-100.0000\",\"betCode\":\"Banker\",\"waterbet\":\"100.0000\",\"winLoss\":\"-100.0000\",\"ip\":\"136.158.24.8\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"settime\":\"2023-04-13 15:58:25\",\"reset\":\"N\",\"betResult\":\"Banker\",\"gameResult\":\"Banker:\\\\u26663\\\\u2663A\\\\u26632 Player:\\\\u2660K\\\\u2666A\\\\u26666\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: "4uispltzj9d8",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<int?>(null));

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(-1, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(playerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.Settime.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0) + source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Settlement_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GamePullRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Validbet = 100,
            Water = 0,
            Result = -100,
            BetCode = "Banker",
            Waterbet = 100,
            Winloss = -100,
            Ip = "136.158.24.8",
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            Settime = DateTimeOffset.Parse("2023-04-13T15:58:25+08:00").ToTimestamp(),
            Reset = "N",
            BetResult = "Banker",
            GameResult = "Banker:♣2♥5 Player:♥Q♠9",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"beforeCash\":\"701.1500\",\"bet\":\"100.0000\",\"validbet\":\"100.0000\",\"water\":\"0.0000\",\"result\":\"-100.0000\",\"betCode\":\"Banker\",\"waterbet\":\"100.0000\",\"winLoss\":\"-100.0000\",\"ip\":\"136.158.24.8\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"settime\":\"2023-04-13 15:58:25\",\"reset\":\"N\",\"betResult\":\"Banker\",\"gameResult\":\"Banker:\\\\u26663\\\\u2663A\\\\u26632 Player:\\\\u2660K\\\\u2666A\\\\u26666\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: "4uispltzj9d8",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(playerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.Settime.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0) + source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Settlement_Player_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GamePullRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Validbet = 100,
            Water = 0,
            Result = -100,
            BetCode = "Banker",
            Waterbet = 100,
            Winloss = -100,
            Ip = "136.158.24.8",
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            Settime = DateTimeOffset.Parse("2023-04-13T15:58:25+08:00").ToTimestamp(),
            Reset = "N",
            BetResult = "Banker",
            GameResult = "Banker:♣2♥5 Player:♥Q♠9",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"beforeCash\":\"701.1500\",\"bet\":\"100.0000\",\"validbet\":\"100.0000\",\"water\":\"0.0000\",\"result\":\"-100.0000\",\"betCode\":\"Banker\",\"waterbet\":\"100.0000\",\"winLoss\":\"-100.0000\",\"ip\":\"136.158.24.8\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"settime\":\"2023-04-13 15:58:25\",\"reset\":\"N\",\"betResult\":\"Banker\",\"gameResult\":\"Banker:\\\\u26663\\\\u2663A\\\\u26632 Player:\\\\u2660K\\\\u2666A\\\\u26666\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<PlayerInfo?>(null));

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .SettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(string.Empty, actual.PlayerCode);
        Assert.Equal(Guid.Empty, actual.WalletId);
        Assert.Equal(string.Empty, actual.CurrencyCode);
        Assert.Equal(Guid.Empty, actual.BrandId);
        Assert.Equal(Guid.Empty, actual.GameProviderConfigurationId);
        Assert.True(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Equal(source.Settime.ToDateTimeOffset(), actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Turnover);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0), actual.WinLoss);
        Assert.Equal(source.Winloss.ToDecimal().GetValueOrDefault(0) + source.Validbet.ToDecimal().GetValueOrDefault(0), actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Unsettlement_Brand_Game_Provider_Setting_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameUnsettleRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            BetResult = "Banker",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"bet\":\"100.0000\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"betResult\":\"Banker\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: "4uispltzj9d8",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<Guid?>(null));

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .UnsettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(playerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(Guid.Empty, actual.GameProviderConfigurationId);
        Assert.False(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Null(actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(0, actual.Turnover);
        Assert.Equal(0, actual.WinLoss);
        Assert.Equal(0, actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Unsettlement_Normal_Process()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameUnsettleRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            BetResult = "Banker",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"bet\":\"100.0000\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"betResult\":\"Banker\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: "4uispltzj9d8",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .UnsettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.True(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(playerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.False(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Null(actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(0, actual.Turnover);
        Assert.Equal(0, actual.WinLoss);
        Assert.Equal(0, actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Unsettlement_Player_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameUnsettleRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            BetResult = "Banker",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"bet\":\"100.0000\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"betResult\":\"Banker\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<PlayerInfo?>(null));

        var gameId = 1;
        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .UnsettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(gameId, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(string.Empty, actual.PlayerCode);
        Assert.Equal(Guid.Empty, actual.WalletId);
        Assert.Equal(string.Empty, actual.CurrencyCode);
        Assert.Equal(Guid.Empty, actual.BrandId);
        Assert.Equal(Guid.Empty, actual.GameProviderConfigurationId);
        Assert.False(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Null(actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(0, actual.Turnover);
        Assert.Equal(0, actual.WinLoss);
        Assert.Equal(0, actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }

    [Fact]
    public async Task Unsettlementt_Game_Does_Not_Exist()
    {
        var fakeUtensil = Substitute.For<IUtensil>();

        var source = new GameUnsettleRecord
        {
            User = "wmtf_4uispltzj9d8_PB2",
            BetId = "3399781680",
            BetTime = DateTimeOffset.Parse("2023-04-13T15:57:27+08:00").ToTimestamp(),
            Bet = 100,
            Gid = "101",
            Event = "113387295",
            EventChild = "22",
            Round = "113387295",
            Subround = "22",
            TableId = "112",
            Commission = "0",
            BetResult = "Banker",
            Gname = "Baccarat",
            AdditionalData = "{\"user\":\"wmtf_4uispltzj9d8_PB2\",\"betId\":\"3399781680\",\"betTime\":\"2023-04-13 15:57:27\",\"bet\":\"100.0000\",\"gid\":\"101\",\"event\":\"113387295\",\"eventChild\":\"22\",\"round\":\"113387295\",\"subround\":\"22\",\"tableId\":\"112\",\"commission\":\"0\",\"betResult\":\"Banker\",\"gname\":\"Baccarat\"}"
        };

        var gameProviderId = Guid.NewGuid();
        _ = fakeUtensil
            .GetGameProviderIdAsync(
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderId);

        var gameProviderCode = "WMTF";
        _ = fakeUtensil
            .GetGameProviderCode()
            .Returns(gameProviderCode);

        var playerCode = "4uispltzj9d8";
        _ = fakeUtensil
            .WMCasinoUserToPlayerCode(
                Arg.Is(source.User))
            .Returns(playerCode);

        var player = new PlayerInfo(
            ResellerId: Guid.NewGuid(),
            ResellerCode: "TGS",
            LicenseeId: Guid.NewGuid(),
            LicenseeCode: "TGS",
            BrandId: Guid.NewGuid(),
            BrandCode: "PB2",
            PlayerCode: "4uispltzj9d8",
            Username: "test",
            DefaultWalletId: Guid.NewGuid(),
            DefaultWalletCurrencyCode: "PHP"
            );
        _ = fakeUtensil
            .GetPlayerAsync(
                playerCode: Arg.Is(playerCode),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(player);

        _ = fakeUtensil
            .GetGameIdAsync(
                gameProviderId: Arg.Is(gameProviderId),
                gameExternalId: Arg.Is(source.Gid.ToString()),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(ValueTask.FromResult<int?>(null));

        var gameProviderConfigurationId = Guid.NewGuid();
        _ = fakeUtensil
            .GetBrandGameProviderConfigurationIdAsync(
                brandId: Arg.Is(player.BrandId),
                gameProviderId: Arg.Is(gameProviderId),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gameProviderConfigurationId);

        var gameRecordUtensil = new GameRecordUtensil(fakeUtensil);

        var actual = await gameRecordUtensil
             .UnsettlementConvertRecordAsync(
                 source: source)
             .ConfigureAwait(false);

        Assert.NotNull(actual);
        Assert.False(actual.Success);
        Assert.Equal(gameProviderId, actual.GameProviderId);
        Assert.Equal(gameProviderCode, actual.GameProviderCode);
        Assert.Equal(source.BetId, actual.ExternalRoundId);
        Assert.Equal(-1, actual.GameId);
        Assert.Equal(source.Gid.ToString(), actual.ExternalGameId);
        Assert.Equal(playerCode, actual.PlayerCode);
        Assert.Equal(player.DefaultWalletId, actual.WalletId);
        Assert.Equal(player.DefaultWalletCurrencyCode, actual.CurrencyCode);
        Assert.Equal(player.BrandId, actual.BrandId);
        Assert.Equal(gameProviderConfigurationId, actual.GameProviderConfigurationId);
        Assert.False(actual.Closed);
        Assert.Equal(source.BetTime.ToDateTimeOffset(), actual.ExternalCreatedOn);
        Assert.Null(actual.ExternalClosedOn);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.Bet);
        Assert.Equal(source.Bet.ToDecimal().GetValueOrDefault(0), actual.ValidBet);
        Assert.Equal(0, actual.Turnover);
        Assert.Equal(0, actual.WinLoss);
        Assert.Equal(0, actual.Settle);
        Assert.Equal(source.AdditionalData, actual.AdditionalData);
    }
}
