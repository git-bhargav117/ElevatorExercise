using ElevatorSimulator.common;
using ElevatorSimulator.Factory;
using ElevatorSimulator.Models;
using ElevatorSimulator.Services;
using ElevatorSimulator.Strategies;
using Microsoft.Extensions.Options;

namespace ElevatorSimulator.Engine
{
    /// <summary>
    /// Provides the core functionality for simulating elevator operations including processing
    /// elevator requests and advancing the simulation state.
    /// </summary>
    public class SimulationEngine : ISimulationEngine
    {
        private readonly List<Elevator> _elevators = new();
        private readonly IElevatorDispatchStrategy _strategy;
        private readonly ILogger _logger;
        private readonly RequestGenerator _generator;
        private readonly ElevatorConfig _config;

        public SimulationEngine(IElevatorDispatchStrategyFactory strategyFactory, ILogger logger, IOptions<ElevatorConfig> options)
        {
            _config = options.Value;
            _strategy = strategyFactory.Create();
            _logger = logger;

            _generator = new RequestGenerator(_config.Building.Floors);

            for (int i = 1; i <= _config.Building.Elevators; i++)
                _elevators.Add(new Elevator(i));
        }

        /// <inheritdoc/>
        public void ProcessManualRequest(int waitingFloor, ElevatorDirection direction)
        {
            HandleRequest(new ElevatorRequest(waitingFloor, direction));
        }

        /// <inheritdoc/>
        public void ProcessRandomRequest()
        {
            var request = _generator.GenerateRandomRequest();
            HandleRequest(request);
        }

        /// <inheritdoc/>
        public void Tick()
        {
            Console.WriteLine("\n--------------Current Elevator Car Status-----------------");
            var stops = new SortedSet<int>();

            foreach (var e in _elevators)
            {
                e.Tick();

                stops = null;

                if (e.Direction == ElevatorDirection.Down)
                {
                    stops = [.. e.Stops.OrderByDescending(f => f)];
                }
                else
                {
                    stops = e.Stops;
                }

                _logger.Log($"Car {e.Id} is on floor: {e.CurrentFloor}, Stops [{string.Join(",", stops)}]");
            }

            Console.Write("\n");
        }

        private void HandleRequest(ElevatorRequest request)
        {
            var selected = _strategy.SelectElevator(_elevators, request);
            _logger.Log($"'{request.Direction}' request on floor {request.WaitingFloor} received");

            selected.AddStop(request.WaitingFloor);
        }
    }
}