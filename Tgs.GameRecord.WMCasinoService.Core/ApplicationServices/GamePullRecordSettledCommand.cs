using Tgs.GameInterface.WMCasinoService.Protos;

namespace Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;

public record GamePullRecordSettledCommand(IEnumerable<GameInterfaceWMCasinoServiceGamePullRecord> Records);
