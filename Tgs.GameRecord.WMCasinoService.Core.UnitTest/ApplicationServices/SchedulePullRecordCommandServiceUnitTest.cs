using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.UnitTest.ApplicationServices;

public class SchedulePullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<SchedulePullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakeGameRecordUtensil = Substitute.For<IGameRecordUtensil>();
        var fakeService = Substitute.For<ICommandService<ScheduleHandleRecordCommand>>();

        var command = new SchedulePullRecordCommand(
            ApiUrl: new("http://localhost/"),
            VendorId: "hpg02twapi",
            Signature: "89448b444ed2e922ddc3aa0753370238",
            StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndTime: DateTimeOffset.UtcNow);

        var service = new SchedulePullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeGameRecordUtensil,
            fakeService);

        await service
            .ExecuteAsync(
                command: command)
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GameUnsettleAsync(
                apiUrl: Arg.Is(command.ApiUrl),
                vendorId: Arg.Is(command.VendorId),
                signature: Arg.Is(command.Signature),
                time: Arg.Is(command.StartTime.UtcDateTime),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);

        _ = await fakeClient
            .Received(1)
            .GamePullRecordAsync(
                apiUrl: Arg.Is(command.ApiUrl),
                vendorId: Arg.Is(command.VendorId),
                signature: Arg.Is(command.Signature),
                startTime: Arg.Is(command.StartTime.UtcDateTime),
                endTime: Arg.Is(command.EndTime.UtcDateTime),
                cancellationToken: Arg.Any<CancellationToken>())
            .ConfigureAwait(false);
    }
}
