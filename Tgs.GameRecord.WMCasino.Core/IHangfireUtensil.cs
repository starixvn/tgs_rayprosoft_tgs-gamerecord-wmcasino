using Tgs.GameRecord.WMCasino.Core.JobApplicationServices;

namespace Tgs.GameRecord.WMCasino.Core;

public interface IHangfireUtensil
{
	public Task JobCheckAsync();

	public Task JobExecuteAsync(JobExecuteCommand source);

	public Task JobInitAsync();
}
