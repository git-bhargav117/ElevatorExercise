using ElevatorSimulator.Models;
using ElevatorSimulator.Strategies;
using Xunit;

namespace ElevatorSimulator.Tests
{
    public class ETADispatchStrategyTests
    {
        private readonly ElevatorConfig config;
        private readonly ETADispatchStrategy strategy;

        /// <summary>
        ///  Test cases to validate the ETADispatchStrategy.
        /// </summary>
        public ETADispatchStrategyTests()
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
        public void CalculateETA_IdleElevator_ReturnsDirectTravelTime()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 0, elevatorConfig: config);

            var request = new ElevatorRequest(waitingFloor: 5, direction: ElevatorDirection.Up);

            // Act
            // Expected travel time = |0-5| * 10 = 50
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(50, eta);
        }

        [Fact]
        public void CalculateETA_MovingUp_PickupOnWay_IncludesStopsBeforePickup()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 1, elevatorConfig: config);

            // Add stops that set direction to Up and include stops both before and after pickup
            elevator.AddStop(3);
            elevator.AddStop(5);

            var request = new ElevatorRequest(waitingFloor: 4, direction: ElevatorDirection.Up);

            // Act
            // travel = |1-4|*10 = 30
            // stopsBeforePickup = Count stops <= 4 => {3} => 1 -> stop time 10
            // expected = 30 + 10 = 40
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(40, eta);
        }

        [Fact]
        public void SelectElevator_PicksLowestETA()
        {
            // Arrange
            var e1 = new Elevator(1, initialFloor: 0, elevatorConfig: config);
            var e2 = new Elevator(2, initialFloor: 5, elevatorConfig: config);

            var request = new ElevatorRequest(waitingFloor: 6, direction: ElevatorDirection.Up);

            // Act
            // e1 ETA = |0-6|*10 = 60
            // e2 ETA = |5-6|*10 = 10 -> e2 should be selected
            var selected = strategy.SelectElevator(new[] { e1, e2 }, request);

            // Assert
            Assert.Equal(2, selected.Id);
        }

        [Fact]
        public void CalculateETA_MovingDown_PickupOnWay_IncludesStopsBeforePickup()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 8, elevatorConfig: config);
            // moving down stops
            elevator.AddStop(6);
            elevator.AddStop(3);

            var request = new ElevatorRequest(waitingFloor: 5, direction: ElevatorDirection.Down);

            // Act
            // travel = |8-5| * 10 = 30
            // stopsBeforePickup = Count stops >= 5 => {6} => 1 -> stop time 10
            // expected = 30 + 10 = 40
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(40, eta);
        }

        [Fact]
        public void CalculateETA_NoStopsAndSameFloor_ReturnsZero()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 4, elevatorConfig: config);

            var request = new ElevatorRequest(waitingFloor: 4, direction: ElevatorDirection.Down);

            // Act
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(0, eta);
        }

        [Fact]
        public void SelectElevator_TiePicksFirstInList()
        {
            // Arrange
            // Two elevators equidistant from request
            var e1 = new Elevator(1, initialFloor: 2, elevatorConfig: config);
            var e2 = new Elevator(2, initialFloor: 8, elevatorConfig: config);

            var request = new ElevatorRequest(waitingFloor: 5, direction: ElevatorDirection.Down);

            // Act
            // e1 ETA = |2-5|*10 = 30
            // e2 ETA = |8-5|*10 = 30 -> tie, OrderBy then First should pick e1
            var selected = strategy.SelectElevator(new[] { e1, e2 }, request);

            // Assert
            Assert.Equal(1, selected.Id);
        }

        [Fact]
        public void CalculateETA_NegativeFloorRequest_HandlesGracefully()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 2, elevatorConfig: config);

            // Create a request with invalid negative waiting floor
            var request = new ElevatorRequest(waitingFloor: -1, direction: ElevatorDirection.Down);

            // Act
            // Abs distance = |2 - (-1)| * 10 = 30
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(30, eta);
        }

        [Fact]
        public void SelectElevator_ChoosesIdleOverBusyWhenETAEqual()
        {
            // Arrange
            var idle = new Elevator(1, initialFloor: 4, elevatorConfig: config);
            var busy = new Elevator(2, initialFloor: 4, elevatorConfig: config);

            // Make busy elevator non-idle by adding stops and setting direction
            busy.AddStop(6);

            var request = new ElevatorRequest(waitingFloor: 7, direction: ElevatorDirection.Up);

            // Act
            // Both could produce same numeric travel, but tie-breaker should pick first in list (idle)
            var selected = strategy.SelectElevator(new[] { idle, busy }, request);

            // Assert
            Assert.Equal(1, selected.Id);
        }

        [Fact]
        public void CalculateETA_MovingUp_PickupAbove_FinishesStopsThenReverses()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 1, elevatorConfig: config);
            elevator.AddStop(4);
            elevator.AddStop(6);

            var request = new ElevatorRequest(waitingFloor: 0, direction: ElevatorDirection.Down);

            // Act
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(130, eta);
        }
    }
}
