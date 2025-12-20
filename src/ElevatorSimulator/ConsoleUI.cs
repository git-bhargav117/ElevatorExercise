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
                    try
                    {
                        _engine.Tick();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Elevator movement failed: {ex.Message}");
                    }

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
                // Initial delay before starting requests
                await Task.Delay(5000, token);

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        _engine.ProcessRandomRequest();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[ERROR] Random request failed: {ex.Message}");
                    }

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