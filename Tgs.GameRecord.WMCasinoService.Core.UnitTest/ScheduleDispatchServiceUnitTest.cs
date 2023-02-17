using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameRecord.WMCasinoService.Core;
using Tgs.GameRecord.WMCasinoService.Core.DispatchServices;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.JiliService.Core.UnitTest;

[TestClass]
public class ScheduleDispatchServiceUnitTest
{
    [TestMethod]
    public async Task Locking()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "WMTF",
            IntervalMinutes = 5d,
            IntervalMinutesMax = 30d,
            IntervalMinutesMinimum = 1d,
            LockMinute = 3
        });
        var fakeLogger = NullLogger<ScheduleDispatchService>.Instance;
        var fakeDispatchLock = Substitute.For<IDispatchLock>();
        var fakeCommandService = Substitute.For<ICommandService<ScheduleWorkerCommand>>();

        var command = new ScheduleDispatchCommand(
            GameApiUrl: new("http://localhost/"),
            GameProviderVendorId: "test",
            GameProviderSignature: Guid.NewGuid().ToString("N"));

        var key = $"gamerecord:wmtf:dispatch:schedule:{command.GameProviderVendorId}";
        _ = fakeDispatchLock
            .LockingAsync(
                key: Arg.Is(key),
                command: Arg.Is(command))
            .Returns(true);

        var service = new ScheduleDispatchService(
            fakeLogger,
            gameRecordInfo,
            fakeDispatchLock,
            fakeCommandService);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeDispatchLock
           .Received(1)
           .UnlockAsync(key: key);
    }

    [TestMethod]
    public async Task Unlocked()
    {
        var gameRecordInfo = Options.Create(new GameRecordInfoOptions
        {
            GameProviderCode = "WMTF",
            IntervalMinutes = 5d,
            IntervalMinutesMax = 30d,
            IntervalMinutesMinimum = 1d,
            LockMinute = 3
        });
        var fakeLogger = NullLogger<ScheduleDispatchService>.Instance;
        var fakeDispatchLock = Substitute.For<IDispatchLock>();
        var fakeCommandService = Substitute.For<ICommandService<ScheduleWorkerCommand>>();

        var command = new ScheduleDispatchCommand(
            GameApiUrl: new("http://localhost/"),
            GameProviderVendorId: "test",
            GameProviderSignature: Guid.NewGuid().ToString("N"));

        var key = $"gamerecord:wmtf:dispatch:schedule:{command.GameProviderVendorId}";
        _ = fakeDispatchLock
            .LockingAsync(
                key: Arg.Is(key),
                command: Arg.Is(command))
            .Returns(false);

        var service = new ScheduleDispatchService(
            fakeLogger,
            gameRecordInfo,
            fakeDispatchLock,
            fakeCommandService);

        await service
            .ExecuteAsync(
                command)
            .ConfigureAwait(false);

        _ = fakeDispatchLock
           .Received(0)
           .UnlockAsync(key: key);
    }
}
