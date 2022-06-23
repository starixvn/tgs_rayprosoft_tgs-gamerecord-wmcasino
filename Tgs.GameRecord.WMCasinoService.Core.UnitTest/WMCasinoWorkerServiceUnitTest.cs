using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Tgs.GameInterface.WMCasinoService.Client;
using Tgs.GameInterface.WMCasinoService.Protos;
using Tgs.GameRecord.WMCasinoService.Core.ApplicationServices;
using Tgs.GameRecord.WMCasinoService.Core.WorkerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameRecord.WMCasinoService.Core.UnitTest
{
    [TestClass]
    public class WMCasinoWorkerServiceUnitTest
    {
        [TestMethod]
        public async Task Normal_Process()
        {
            var fakeGameInterfaceWMCasinoServiceGrpc = Substitute.For<IGameInterfaceWMCasinoServiceGrpc>();
            var fakeLogger = NullLogger<WMCasinoWorkerService>.Instance;
            var fakeGameSettleRecordCommand = Substitute.For<ICommandService<GameSettleRecordCommand>>();
            var fakeGameUnsettleRecordCommand = Substitute.For<ICommandService<GameUnsettleRecordCommand>>();
            var fakeUtensil = Substitute.For<IUtensil>();
            var gameRecordInfoOptions = new GameRecordInfoOptions()
            {
                GameProviderCode = "WMTF",
                IntervalMinute = 5
            };
            var options = Options.Create(gameRecordInfoOptions);
            var command = new WMCasinoWorkerCommand(
                GameApiUrl: new("http://localhost/"),
                GameProviderVendorId: "testVendorId",
                GameProviderSignature: "testSignature",
                StartTime: DateTime.UtcNow.AddMinutes(-5),
                EndTime: DateTime.UtcNow);

            _ = fakeUtensil.GetGameProviderIdAsync(gameRecordInfoOptions.GameProviderCode).Returns(Guid.NewGuid());

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
                   endTime: Arg.Any<DateTime>(),
                   playerCode: null).Returns(gamePullRecordReply);

            var service = new WMCasinoWorkerService(
                options,
                fakeGameInterfaceWMCasinoServiceGrpc,
                fakeLogger,
                fakeUtensil,
                fakeGameSettleRecordCommand,
                fakeGameUnsettleRecordCommand);

            await service.ExecuteAsync(command).ConfigureAwait(false);
        }
    }
}
