using Microsoft.AspNetCore.Mvc;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.GameRecord.WMCasinoService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Controllers
{
    [ApiController]
    [Route("wmtf")]
    public class WMCasinoTransferController : ControllerBase
    {
        [HttpPost]
        public ValueTask ExecuteAsync(
            [FromServices] ICommandService<WMCasinoWorkerCommand> service,
            GameRecordQueryViewModel model)
            => service.ExecuteAsync(new WMCasinoWorkerCommand(
                GameApiUrl: model.GameApiUrl,
                GameProviderVendorId: model.GameProviderVendorId,
                GameProviderSignature: model.GameProviderSignature,
                StartTime: model.StartTime,
                EndTime: model.EndTime));
    }
}
