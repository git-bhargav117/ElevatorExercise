using ElevatorSimulator.Engine;
using ElevatorSimulator.Models;
using Microsoft.Extensions.Options;

namespace ElevatorSimulator
{
    /// <summary>
    /// Provides a console-based user interface for running and interacting with the elevator car simulation.
    /// </summary>
    /// <remarks>
    /// Author: Bhargav Panchal
    /// </remarks>

    public class ConsoleUI
    {
        private readonly ISimulationEngine _engine;
        private readonly ElevatorConfig _config;

        public ConsoleUI(ISimulationEngine engine, IOptions<ElevatorConfig> options)
        {
            _engine = engine;
            _config = options.Value;
        }

        public void Run()
        {
            var cts = new CancellationTokenSource();
            var token = cts.Token;

            Console.WriteLine($"--------------Elevator Car Simulator-------------------------------");
            Console.WriteLine($"--------------for Building having {_config.Building.Floors} floors and {_config.Building.Elevators} elevators cars --");
            Console.WriteLine("\nPress ESC to stop the simulation.");

            // -----------------------------
            // Elevator movement loop
            // -----------------------------
            Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    _engine.Tick();

                    await Task.Delay(
                        TimeSpan.FromSeconds(_config.SimulationTiming.MoveDurationInSeconds),
                        token);
                }
            }, token);

            // -----------------------------
            // Random request generation loop
            // -----------------------------
            Task.Run(async () =>
            {
                Thread.Sleep(5000); // Initial delay before starting random requests

                while (!token.IsCancellationRequested)
                {
                    _engine.ProcessRandomRequest();

                    await Task.Delay(
                        TimeSpan.FromSeconds(_config.SimulationTiming.RandomRequestIntervalInSeconds),
                        token);
                }
            }, token);


            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                // do nothing
            }

            Console.WriteLine("Stopping simulation...");
            cts.Cancel();

        }
    }
}