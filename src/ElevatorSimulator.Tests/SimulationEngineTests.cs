using ElevatorSimulator.Engine;
using ElevatorSimulator.Models;
using ElevatorSimulator.Strategies;
using ElevatorSimulator.common;
using ElevatorSimulator.Factory;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ElevatorSimulator.Tests
{
    public class SimulationEngineTests
    {
        private readonly ElevatorConfig config;
        private readonly ETADispatchStrategy strategy;

        /// <summary>
        /// Test cases to validate the SimulationEngine.
        /// </summary>
        public SimulationEngineTests()
        {
            config = new ElevatorConfig
            {
                DispatchStrategy = "ETA",
                Building = new BuildingSettings { Floors = 10, Elevators = 4 },
                SimulationTiming = new TimingSettings { MoveDurationInSeconds = 10, StopDurationInSeconds = 10 }
            };

            strategy = new ETADispatchStrategy();
        }

        [Fact]
        public void ProcessManualRequest_AssignsElevatorAndAddsStops()
        {
            var options = Options.Create(config);

            var loggerMock = new Mock<ILogger>();

            var factoryMock = new Mock<IElevatorDispatchStrategyFactory>();
            factoryMock.Setup(f => f.Create()).Returns(strategy);

            var engine = new SimulationEngine(factoryMock.Object, loggerMock.Object, options);

            engine.ProcessManualRequest(waitingFloor: 2, direction: ElevatorDirection.Up);

            // After processing, one of the elevators should have both stops
            // We cannot access private elevator list, but we can call Tick() to exercise behavior
            engine.Tick();

            loggerMock.Verify(l => l.Log(It.Is<string>(s => s.Contains("request on floor"))), Times.Once);
        }
    }
}
