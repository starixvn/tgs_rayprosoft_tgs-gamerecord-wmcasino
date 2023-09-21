using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.WMCasino.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasino.Core.UnitTest.ApplicationServices;

public class PullRecordCommandServiceUnitTest
{
    [Fact]
    public async Task Normal_Process()
    {
        var fakeLogger = NullLogger<PullRecordCommandService>.Instance;
        var fakeClient = Substitute.For<IGameInterfaceWMCasinoServiceClient>();
        var fakeService = Substitute.For<ICommandService<HandleRecordCommand>>();

        var command = new PullRecordCommand(
            ApiUrl: new("http://localhost/"),
            VendorId: "hpg02twapi",
            Signature: "89448b444ed2e922ddc3aa0753370238",
            StartTime: DateTimeOffset.UtcNow.AddMinutes(-10),
            EndTime: DateTimeOffset.UtcNow);

        var gamePullRecordReply = new GamePullRecordReply { };
        _ = fakeClient
            .GamePullRecordAsync(
                apiUrl: Arg.Is(command.ApiUrl),
                vendorId: Arg.Is(command.VendorId),
                signature: Arg.Is(command.Signature),
                startTime: Arg.Is(command.StartTime.UtcDateTime),
                endTime: Arg.Is(command.EndTime.UtcDateTime),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(gamePullRecordReply);

        var service = new PullRecordCommandService(
            fakeLogger,
            fakeClient,
            fakeService);

        await service.ExecuteAsync(command: command);
    }
}
