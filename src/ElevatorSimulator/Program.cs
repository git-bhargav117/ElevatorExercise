using ElevatorSimulator;
using ElevatorSimulator.common;
using ElevatorSimulator.Engine;
using ElevatorSimulator.Factory;
using ElevatorSimulator.Models;
using ElevatorSimulator.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

/// <summary>
/// Registers services and starts the Elevator Simulator console application.
var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ILogger, ConsoleLogger>();
builder.Services.AddSingleton<ISimulationEngine, SimulationEngine>();
builder.Services.AddSingleton<ConsoleUI>();
builder.Services.AddSingleton<ETADispatchStrategy>();

builder.Services.AddSingleton<IElevatorDispatchStrategyFactory,
                      ElevatorDispatchStrategyFactory>();

builder.Services.Configure<ElevatorConfig>(
    builder.Configuration.GetSection("ElevatorConfig"));

var host = builder.Build();

/// Start the console UI
host.Services.GetRequiredService<ConsoleUI>().Run();