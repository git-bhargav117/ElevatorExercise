using ElevatorSimulator.Models;
using ElevatorSimulator.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ElevatorSimulator.Tests
{
    public class ElevatorDispatchStrategyFactoryTests
    {
        private readonly ElevatorConfig config;

        public ElevatorDispatchStrategyFactoryTests()
        {
            config = new ElevatorConfig
            {
                DispatchStrategy = "ETA",
                Building = new BuildingSettings { Floors = 10, Elevators = 4 },
                SimulationTiming = new TimingSettings { MoveDurationInSeconds = 10, StopDurationInSeconds = 10 }
            };
        }

        [Fact]
        public void Factory_Returns_Eta_Strategy_When_Config_Is_ETA()
        {
            // Arrange  
            var options = Options.Create(config);

            var provider = new ServiceCollection()
                .AddSingleton<ETADispatchStrategy>()
                .BuildServiceProvider();

            var factory = new ElevatorDispatchStrategyFactory(
                provider, options);

            // Act
            var strategy = factory.Create();

            // Assert
            Assert.IsType<ETADispatchStrategy>(strategy);
        }
    }
}
