using ElevatorSimulator.Strategies;

namespace ElevatorSimulator.Factory
{
    public interface IElevatorDispatchStrategyFactory
    {
        IElevatorDispatchStrategy Create();
    }
}