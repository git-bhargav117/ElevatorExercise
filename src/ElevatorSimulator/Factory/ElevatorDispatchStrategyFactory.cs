using ElevatorSimulator.Factory;
using ElevatorSimulator.Models;
using ElevatorSimulator.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

/// <summary>
/// Provides a factory for creating elevator dispatch strategy instances based on system configuration.
/// </summary>
public class ElevatorDispatchStrategyFactory
    : IElevatorDispatchStrategyFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ElevatorConfig _config;

    public ElevatorDispatchStrategyFactory(
        IServiceProvider serviceProvider,
        IOptions<ElevatorConfig> options)
    {
        _serviceProvider = serviceProvider;
        _config = options.Value;
    }

    /// <summary>
    /// Creates an instance of the elevator dispatch strategy configured for the system.
    /// </summary>
    public IElevatorDispatchStrategy Create()
    {
        return _config.DispatchStrategy.ToUpper() switch
        {
            "ETA" => _serviceProvider.GetRequiredService<ETADispatchStrategy>(),

            _ => throw new InvalidOperationException(
                $"Unknown dispatch strategy: {_config.DispatchStrategy}")
        };
    }
}
