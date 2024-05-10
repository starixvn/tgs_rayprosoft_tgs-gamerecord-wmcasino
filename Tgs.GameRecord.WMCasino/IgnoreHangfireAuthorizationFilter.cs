using Hangfire.Dashboard;

namespace Tgs.GameRecord.WMCasino;

public class IgnoreHangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
	public bool Authorize(DashboardContext context) => true;
}
