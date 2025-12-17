using ElevatorSimulator.Models;

namespace ElevatorSimulator.Engine
{
    /// <summary>
    /// Defines the contract for a simulation engine that processes elevator requests and advances simulation time.
    /// </summary>
    public interface ISimulationEngine
    {
        /// <summary>
        /// Processes a manual elevator request to move from a specified waiting floor to a target floor in the given
        /// direction.
        /// </summary>
        /// <param name="waitingFloor">Pickup floor.</param>
        /// <param name="direction">Requested travel direction.</param>
        void ProcessManualRequest(int waitingFloor, ElevatorDirection direction);

        /// <summary>
        /// Generates and processes a single random request to move from a waiting floor to a target floor in the given
        /// direction.
        /// </summary>
        void ProcessRandomRequest();

        /// <summary>
        /// Advances the elevator one step toward its next scheduled stop,
        /// updating direction and removing the stop when reached.
        /// </summary>
        void Tick();
    }
}
