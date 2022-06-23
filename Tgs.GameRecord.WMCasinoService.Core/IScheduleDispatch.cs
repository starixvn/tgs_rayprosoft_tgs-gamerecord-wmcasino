using Tgs.GameRecord.WMCasinoService.Core.DispatchServices;

namespace Tgs.GameRecord.WMCasinoService.Core
{
    public interface IScheduleDispatch
    {
        public Task<bool> ExistAsync(string key);

        public Task<bool> LockingAsync(string key, ScheduleDispatchCommand command);

        public Task<bool> UnlockAsync(string key);
    }
}
