namespace Tgs.GameRecord.WMCasinoService.Core.Models;

public record GameRecordInfo(
    bool Success,
    Guid GameProviderId,
    string GameProviderCode,
    string ExternalRoundId,
    int GameId,
    string ExternalGameId,
    string PlayerCode,
    Guid WalletId,
    string CurrencyCode,
    Guid BrandId,
    Guid GameProviderConfigurationId,
    bool Closed,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset? ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal Settle,
    decimal Commission,
    decimal Turnover,
    decimal WinLoss,
    string AdditionalData);
