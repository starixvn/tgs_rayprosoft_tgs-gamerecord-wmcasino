using Tgs.GameRecord.WMCasino.Core.ApplicationServices;

namespace Tgs.GameRecord.WMCasino.Core;

public interface IPullRecordDispatcher
{
	ValueTask PushCommandAsync(PullRecordCommand command, CancellationToken cancellationToken = default);
}
