using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WMCasino.Core.ApplicationServices;
using Tgs.GameRecord.WMCasino.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasino.Controllers;

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
