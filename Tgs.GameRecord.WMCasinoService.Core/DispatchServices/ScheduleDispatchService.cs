using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.DispatchServices;

internal class ScheduleDispatchService : ICommandService<ScheduleDispatchCommand>
{
    private readonly ICommandService<ScheduleWorkerCommand> _commandService;
    private readonly GameRecordInfoOptions _gameRecordInfoOptions;
    private readonly double _intervalMinute;
    private readonly ILogger<ScheduleDispatchService> _logger;
    private readonly IDispatchLock _scheduleDispatch;

    public ScheduleDispatchService(
        ILogger<ScheduleDispatchService> logger,
        IOptions<GameRecordInfoOptions> options,
        IDispatchLock scheduleDispatch,
        ICommandService<ScheduleWorkerCommand> commandService)
    {
        _gameRecordInfoOptions = options.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        var key = $"gamerecord:wmtf:dispatch:schedule:{command.GameProviderVendorId}";

        if (await _scheduleDispatch.LockingAsync(key: key, command: command).ConfigureAwait(false))
        {
            try
            {
                var endTime = DateTimeOffset.ParseExact(DateTimeOffset.UtcNow.ToString(@"yyyy-MM-dd\THH:mm:00K"), @"yyyy-MM-dd\THH:mm:ssK", null, styles: System.Globalization.DateTimeStyles.RoundtripKind);
                var startTime = endTime.AddMinutes(_intervalMinute * -1);

                await _commandService.ExecuteAsync(new ScheduleWorkerCommand(
                     GameApiUrl: command.GameApiUrl,
                     GameProviderVendorId: command.GameProviderVendorId,
                     GameProviderSignature: command.GameProviderSignature,
                     StartTime: startTime,
                     EndTime: endTime), cancellationToken: cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "LogOn:{logOn} Message:An exception occurred on dispatch.(GameProviderVendorId:{gameProviderVendorId})",
                    DateTimeOffset.UtcNow.ToString("O"),
                    command.GameProviderVendorId);
            }
            finally
            {
                await _scheduleDispatch.UnlockAsync(key: key).ConfigureAwait(false);
            }
        }
    }
}
