using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.GameRecord.WMCasinoService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Controllers;

[Route("[controller]")]
[ApiController]
public class WMCasinoTransferController : ControllerBase
{
    [HttpPost()]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<PullRecordCommand> service,
        PullRecordViewModel source)
        => service.ExecuteAsync(new PullRecordCommand(
            ApiUrl: source.ApiUrl,
            VendorId: source.VendorId,
            Signature: source.Signature,
            StartTime: source.StartTime,
            EndTime: source.EndTime));
}
