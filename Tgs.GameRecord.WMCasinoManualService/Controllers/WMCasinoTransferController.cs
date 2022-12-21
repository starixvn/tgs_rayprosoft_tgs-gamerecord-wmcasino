using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WMCasinoManualService.ViewModels;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoManualService.Controllers;

[ApiController]
[Route("wmtf")]
public class WMCasinoTransferController : ControllerBase
{
    [HttpPost]
    public ValueTask ExecuteAsync(
        [FromServices] ICommandService<GameHistoryWorkerCommand> service,
        GameRecordQueryViewModel model)
        => service.ExecuteAsync(new GameHistoryWorkerCommand(
            GameApiUrl: model.GameApiUrl,
            GameProviderVendorId: model.GameProviderVendorId,
            GameProviderSignature: model.GameProviderSignature,
            StartTime: model.StartTime,
            EndTime: model.EndTime));
}
