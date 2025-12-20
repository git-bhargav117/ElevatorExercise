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

            var request = new ElevatorRequest(waitingFloor: 5, direction: ElevatorDirection.Up, destinationFloor: 8);

            // Act
            // Expected time to pickup = |0-5| * 10 = 50
            // Expected stop time at pickup = 10
            // Expected time to destination = |5-8| * 10 = 30
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(90, eta);
        }

        [Fact]
        public void CalculateETA_MovingUp_PickupOnWay_IncludesStopsBeforePickup()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 1, elevatorConfig: config);

            // Add stops that set direction to Up and include stops both before and after pickup
            elevator.AddStop(3);
            elevator.AddStop(5);

            var request = new ElevatorRequest(waitingFloor: 4, direction: ElevatorDirection.Up, destinationFloor: 7);

            // Act
            // Expected time to pickup = |1-4|*10 = 30
            // Expected stop time at pickup = 10
            // StopsBeforePickup = Count stops <= 4 => {3} => 1 -> stop time 10
            // StopedAfterPickup = Count stops > 4 => {5} => 1 -> stop time 10
            // Expected time to destination = |4-7|*10 = 30
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(90, eta);
        }

        [Fact]
        public void SelectElevator_PicksLowestETA()
        {
            // Arrange
            var e1 = new Elevator(1, initialFloor: 0, elevatorConfig: config);
            var e2 = new Elevator(2, initialFloor: 5, elevatorConfig: config);

            var request = new ElevatorRequest(waitingFloor: 6, direction: ElevatorDirection.Up, destinationFloor: 9);

            // Act
            // With updated ETA logic:
            // e1 ETA = |0-6|*10 (travel to pickup) + 10 (stop at pickup) + |6-9|*10 (travel to destination) = 60 + 10 + 30 = 100
            // e2 ETA = |5-6|*10 + 10 + |6-9|*10 = 10 + 10 + 30 = 50 -> e2 should be selected
            var eta1 = strategy.CalculateETA(e1, request);
            var eta2 = strategy.CalculateETA(e2, request);

            Assert.Equal(100, eta1);
            Assert.Equal(50, eta2);

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

            var request = new ElevatorRequest(waitingFloor: 5, direction: ElevatorDirection.Down, destinationFloor: 1);

            // Act  
            // Expected time to pickup = |8-5|*10 = 30
            // Expected stop time at pickup = 10
            // StopsBeforePickup = Count stops >= 5 => {6} => 1 -> stop time 10
            // StopsAfterPickup = Count stops < 5 => {3} => 1 -> stop time 10
            // Expected time to destination = |5-1|*10 = 40
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(100, eta);
        }

        [Fact]
        public void CalculateETA_NoStopsAndSameFloor_ReturnsZero()
        {
            // Arrange
            var elevator = new Elevator(1, initialFloor: 4, elevatorConfig: config);

            var request = new ElevatorRequest(waitingFloor: 4, direction: ElevatorDirection.Down, destinationFloor: 0);

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

            var request = new ElevatorRequest(waitingFloor: 5, direction: ElevatorDirection.Down, destinationFloor: 0);

            // Act
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
            var request = new ElevatorRequest(waitingFloor: -1, direction: ElevatorDirection.Down, destinationFloor: 0);

            // Act
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(50, eta);
        }

        [Fact]
        public void SelectElevator_ChoosesIdleOverBusyWhenETAEqual()
        {
            // Arrange
            var idle = new Elevator(1, initialFloor: 4, elevatorConfig: config);
            var busy = new Elevator(2, initialFloor: 4, elevatorConfig: config);

            // Make busy elevator non-idle by adding stops and setting direction
            busy.AddStop(6);

            var request = new ElevatorRequest(waitingFloor: 7, direction: ElevatorDirection.Up, destinationFloor: 9);

            // Act
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

            var request = new ElevatorRequest(waitingFloor: 0, direction: ElevatorDirection.Up, destinationFloor: 2);

            // Act
            var eta = strategy.CalculateETA(elevator, request);

            // Assert
            Assert.Equal(160, eta);
        }
    }
}
