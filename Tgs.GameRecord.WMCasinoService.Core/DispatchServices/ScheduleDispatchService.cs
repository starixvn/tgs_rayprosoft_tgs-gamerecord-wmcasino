using Microsoft.Extensions.Options;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly ICommandService<GameHistoryWorkerCommand> _commandService;
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly double _intervalMinute;
    private readonly IScheduleDispatch _scheduleDispatch;

    public ScheduleDispatchService(
        IOptions<GameRecordInfoOptions> options,
        ICommandService<GameHistoryWorkerCommand> commandService,
        IScheduleDispatch scheduleDispatch
        )
    {
        _gameRecordInfoOptions = options.Value;
        _commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
        _scheduleDispatch = scheduleDispatch ?? throw new ArgumentNullException(nameof(scheduleDispatch));
        _intervalMinute = (_gameRecordInfoOptions.IntervalMinutes > _gameRecordInfoOptions.IntervalMinutesMax)
                        ? _gameRecordInfoOptions.IntervalMinutesMax
                        : (_gameRecordInfoOptions.IntervalMinutes < _gameRecordInfoOptions.IntervalMinutesMinimum)
                        ? _gameRecordInfoOptions.IntervalMinutesMinimum
                        : _gameRecordInfoOptions.IntervalMinutes;
    }

    public async ValueTask ExecuteAsync(ScheduleDispatchCommand command, CancellationToken cancellationToken = default)
    {
        var key = $"GameRecord:WMCasinoService:{_gameRecordInfoOptions.GameProviderCode}:{command.GameProviderVendorId}".ToLower();
        if (!await _scheduleDispatch.ExistAsync(key: key).ConfigureAwait(false))
        {
            if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
            {
                try
                {
                    var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                    var startTime = endTime.AddMinutes(_intervalMinute * -1);

                    await _commandService.ExecuteAsync(new GameHistoryWorkerCommand(
                         GameApiUrl: command.GameApiUrl,
                         GameProviderVendorId: command.GameProviderVendorId,
                         GameProviderSignature: command.GameProviderSignature,
                         StartTime: startTime,
                         EndTime: endTime), cancellationToken: cancellationToken).ConfigureAwait(false);
                }
                finally
                {
                    _ = await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
                }
            }
        }
    }
}
