using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.WMCasino.Common;

public record WMCasinoGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	public string VendorId { get; init; } = default!;
	[Required]
	public string Signature { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartTime { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndTime { get; init; } = default!;
}
