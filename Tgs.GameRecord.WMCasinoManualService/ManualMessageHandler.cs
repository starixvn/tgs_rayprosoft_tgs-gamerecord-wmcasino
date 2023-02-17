using System.Text;
using System.Text.Json;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.MessageQueue;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoManualService;

public class ManualMessageHandler : IMessageProcessor
{
    private readonly ICommandService<ScheduleWorkerCommand> _service;

    public ManualMessageHandler(ICommandService<ScheduleWorkerCommand> service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public async ValueTask<ReadOnlyMemory<byte>> HandleAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        var json = Encoding.UTF8.GetString(data.Span);
        var request = JsonSerializer.Deserialize<ManualMessageRequest>(json)!;

        var gameRecordCommand = new ScheduleWorkerCommand(
            GameApiUrl: request.GameApiUrl,
            GameProviderVendorId: request.GameProviderVendorId,
            GameProviderSignature: request.GameProviderSignature,
            StartTime: request.StartTime,
            EndTime: request.EndTime);

        await _service.ExecuteAsync(
            command: gameRecordCommand,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        var response = new ManualMessageResponse(true);
        return Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
    }
}
