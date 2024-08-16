using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Tgs.GameRecord.WMCasino.Common;

public record WMCasinoGameRecordMessage
{
	[Required]
	[DataType(DataType.Url)]
	[DefaultValue(typeof(Uri), "https://api.a45.me/")]
	public Uri ApiUrl { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "hpg02twapi")]
	public string VendorId { get; init; } = default!;
	[Required]
	[DefaultValue(typeof(string), "89448b444ed2e922ddc3aa0753370238")]
	public string Signature { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(string), "2024-08-15T13:00:00+08:00")]
	public DateTimeOffset StartTime { get; init; } = default!;
	[Required]
	[DataType(DataType.DateTime)]
	[DefaultValue(typeof(string), "2024-08-15T13:10:00+08:00")]
	public DateTimeOffset EndTime { get; init; } = default!;
}
