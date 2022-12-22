using Tgs.GameInterface.WMCasinoService.Protos;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

public record GamePullRecordUnsettledCommand(IEnumerable<GameInterfaceWMCasinoServiceGameUnsettleRecord> Records);
