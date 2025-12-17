using ElevatorSimulator.Models;

namespace ElevatorSimulator.Strategies
{
    /// <summary>
    /// Defines a strategy for selecting an elevator to handle a request.
    /// </summary>
    public interface IElevatorDispatchStrategy
    {
        /// <summary>
        /// Selects the most suitable elevator from a collection based on the specified request.
        /// </summary>
        Elevator SelectElevator(IEnumerable<Elevator> elevators, ElevatorRequest request);

        /// <summary>
        /// Calculates the estimated time of arrival (ETA) for an elevator to reach the requested floor based on its
        /// current state and the specified request.
        /// </summary>
        int CalculateETA(Elevator elevator, ElevatorRequest request);
    }
}