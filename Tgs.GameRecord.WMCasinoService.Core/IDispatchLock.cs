using Tgs.GameRecord.WMCasinoService.Core.DispatchServices;

namespace Tgs.GameRecord.WMCasinoService.Core;

public interface IDispatchLock
{
    public Task<bool> LockingAsync(string key, ScheduleDispatchCommand command);

    public Task UnlockAsync(string key);
}
