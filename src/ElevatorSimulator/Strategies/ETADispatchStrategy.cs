using ElevatorSimulator.Models;

namespace ElevatorSimulator.Strategies
{
    /// <summary>
    /// Provides an elevator dispatch strategy that selects elevators based on estimated time of arrival (ETA algorithm) to a
    /// requested floor.
    /// </summary>
    public class ETADispatchStrategy : IElevatorDispatchStrategy
    {
        /// <inheritdoc/>
        public Elevator SelectElevator(IEnumerable<Elevator> elevators, ElevatorRequest request)
        {
            return elevators
                .OrderBy(e => CalculateETA(e, request))
                .First();
        }

        /// <inheritdoc/>
        public int CalculateETA(Elevator elevator, ElevatorRequest request)
        {
            // Determine the pickup (waiting) floor
            int pickup = request.WaitingFloor;

            // Capture the elevator's current position
            int current = elevator.CurrentFloor;

            // If the elevator is idle, ETA is direct travel time to pickup
            if (elevator.IsIdle)
            {
                return Math.Abs(current - pickup) * elevator.MoveTime;
            }

            // Calculate direct travel time to the pickup floor
            int travelToPickup = Math.Abs(current - pickup) * elevator.MoveTime;

            // Handle ETA calculation when elevator is moving upward
            if (elevator.Direction == ElevatorDirection.Up)
            {
                // Identify all pending stops in the upward direction
                var upwardStops = elevator.Stops.Where(s => s >= current).ToList();

                // Case: pickup floor lies ahead in the current upward path
                if (pickup >= current)
                {
                    // Count how many stops will occur before reaching pickup
                    int stopsBeforePickup = upwardStops.Count(s => s <= pickup);

                    // ETA = travel time + dwell time for intermediate stops
                    return travelToPickup + stopsBeforePickup * elevator.StopTime;
                }
                else
                {
                    // Determine the highest stop before reversing direction
                    int highestStop = upwardStops.Last();

                    // Time required to finish upward travel
                    int travelToEnd = Math.Abs(current - highestStop) * elevator.MoveTime;

                    // Time required to travel back down to the pickup floor
                    int reverseTravel = Math.Abs(highestStop - pickup) * elevator.MoveTime;

                    // ETA includes finishing upward stops, stop dwell time, and reverse travel
                    return travelToEnd
                           + upwardStops.Count * elevator.StopTime
                           + reverseTravel;
                }
            }

            // Handle ETA calculation when elevator is moving downward
            if (elevator.Direction == ElevatorDirection.Down)
            {
                // Identify all pending stops in the downward direction
                var downwardStops = elevator.Stops.Where(s => s <= current).ToList();

                // Case: pickup floor lies ahead in the current downward path
                if (pickup <= current)
                {
                    // Count how many stops will occur before reaching pickup
                    int stopsBeforePickup = downwardStops.Count(s => s >= pickup);

                    // ETA = travel time + dwell time for intermediate stops
                    return travelToPickup + stopsBeforePickup * elevator.StopTime;
                }
                else
                {
                    // Determine the lowest stop before reversing direction
                    int lowestStop = downwardStops.Last();

                    // Time required to finish downward travel
                    int travelToEnd = Math.Abs(current - lowestStop) * elevator.MoveTime;

                    // Time required to travel back up to the pickup floor
                    int reverseTravel = Math.Abs(lowestStop - pickup) * elevator.MoveTime;

                    // ETA includes finishing downward stops, stop dwell time, and reverse travel
                    return travelToEnd
                           + downwardStops.Count * elevator.StopTime
                           + reverseTravel;
                }
            }

            return travelToPickup;
        }
    }
}