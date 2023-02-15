using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.UnitTest;

[TestClass]
public class WMCasinoWorkerServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
        var fakeLogger = NullLogger<ScheduleWorkerService>.Instance;
        var fakeGameSettleRecordCommand = Substitute.For<ICommandService<SettlementCommand>>();
        var fakeGameUnsettleRecordCommand = Substitute.For<ICommandService<UnsettledCommand>>();
        var command = new ScheduleWorkerCommand(
            GameApiUrl: new("http://localhost/"),
            GameProviderVendorId: "testVendorId",
            GameProviderSignature: "testSignature",
            StartTime: DateTime.UtcNow.AddMinutes(-5),
            EndTime: DateTime.UtcNow);

        var gameUnsettleRecordReply = new GameInterfaceWMCasinoServiceGameUnsettleRecordReply();
        _ = fakeGameInterfaceWMCasinoServiceGrpc
            .GameUnsettleAsync(
               apiUrl: Arg.Is(command.GameApiUrl),
               vendorId: Arg.Is(command.GameProviderVendorId),
               signature: Arg.Is(command.GameProviderSignature),
               time: Arg.Any<DateTime>()).Returns(gameUnsettleRecordReply);

        var gamePullRecordReply = new GameInterfaceWMCasinoServiceGamePullRecordReply();
        _ = fakeGameInterfaceWMCasinoServiceGrpc
            .GamePullRecordAsync(
               apiUrl: Arg.Is(command.GameApiUrl),
               vendorId: Arg.Is(command.GameProviderVendorId),
               signature: Arg.Is(command.GameProviderSignature),
               startTime: Arg.Any<DateTime>(),
               endTime: Arg.Any<DateTime>()).Returns(gamePullRecordReply);

        var service = new ScheduleWorkerService(
            fakeGameInterfaceWMCasinoServiceGrpc,
            fakeLogger,
            fakeGameSettleRecordCommand,
            fakeGameUnsettleRecordCommand);

        await service.ExecuteAsync(command).ConfigureAwait(false);
    }
}
