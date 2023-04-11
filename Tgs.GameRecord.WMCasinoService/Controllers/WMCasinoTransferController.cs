using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.GameRecord.WMCasinoService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Controllers;

[Route("[controller]")]
[ApiController]
public class WMCasinoTransferController : ControllerBase
{
    [HttpPost("manual")]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<ManualPullRecordCommand> service,
        PullRecordViewModel source)
        => service.ExecuteAsync(new ManualPullRecordCommand(
            ApiUrl: source.ApiUrl,
            VendorId: source.VendorId,
            Signature: source.Signature,
            StartTime: source.StartTime,
            EndTime: source.EndTime));

    [HttpPost("schedule")]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<SchedulePullRecordCommand> service,
        PullRecordViewModel source)
        => service.ExecuteAsync(new SchedulePullRecordCommand(
            ApiUrl: source.ApiUrl,
            VendorId: source.VendorId,
            Signature: source.Signature,
            StartTime: source.StartTime,
            EndTime: source.EndTime));
}
