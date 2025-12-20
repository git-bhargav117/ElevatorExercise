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
            try
            {
                // Determine the pickup (waiting) floor
                int pickup = request.WaitingFloor;

                // Determine the destination floor
                int destination = request.DestinationFloor;

                // Capture the elevator's current position
                int current = elevator.CurrentFloor;

                if (pickup == current && destination == 0)
                {
                    return 0;
                }

                // Phase 1: ETA to pickup
                // -----------------------------
                int etaToPickup = 0;

                // Calculate direct travel time to the pickup floor
                int travelToPickup = Math.Abs(current - pickup) * elevator.MoveTime;

                if (elevator.IsIdle)
                {
                    // If the elevator is idle, ETA is direct travel time to pickup
                    etaToPickup = Math.Abs(current - pickup) * elevator.MoveTime;
                }
                else if (elevator.Direction == ElevatorDirection.Up)
                {
                    // Handle ETA calculation when elevator is moving upward

                    // Identify all pending stops in the upward direction
                    var upwardStops = elevator.Stops.Where(s => s >= current).ToList();

                    // Case: pickup floor lies ahead in the current upward path
                    if (pickup >= current)
                    {
                        // Count how many stops will occur before reaching pickup
                        int stopsBeforePickup = upwardStops.Count(s => s <= pickup);

                        // ETA = travel time + dwell time for intermediate stops
                        etaToPickup = travelToPickup + stopsBeforePickup * elevator.StopTime;
                    }
                    else
                    {
                        // Determine the highest stop before reversing direction
                        int highestStop = upwardStops.Count != 0 ? upwardStops.Last() : current;

                        // Time required to finish upward travel
                        int travelToEnd = Math.Abs(current - highestStop) * elevator.MoveTime;

                        // Time required to travel back down to the pickup floor
                        int reverseTravel = Math.Abs(highestStop - pickup) * elevator.MoveTime;

                        // ETA includes finishing upward stops, stop dwell time, and reverse travel
                        etaToPickup = travelToEnd + upwardStops.Count * elevator.StopTime + reverseTravel;
                    }
                }
                else
                {
                    // Handle ETA calculation when elevator is moving downward

                    // Identify all pending stops in the downward direction
                    var downwardStops = elevator.Stops.Where(s => s <= current).ToList();

                    // Case: pickup floor lies ahead in the current downward path
                    if (pickup <= current)
                    {
                        // Count how many stops will occur before reaching pickup
                        int stopsBeforePickup = downwardStops.Count(s => s >= pickup);

                        // ETA = travel time + dwell time for intermediate stops
                        etaToPickup = travelToPickup + stopsBeforePickup * elevator.StopTime;
                    }
                    else
                    {
                        // Determine the lowest stop before reversing direction
                        int lowestStop = downwardStops.Count != 0 ? downwardStops.Last() : current;

                        // Time required to finish downward travel
                        int travelToEnd = Math.Abs(current - lowestStop) * elevator.MoveTime;

                        // Time required to travel back up to the pickup floor
                        int reverseTravel = Math.Abs(lowestStop - pickup) * elevator.MoveTime;

                        // ETA includes finishing downward stops, stop dwell time, and reverse travel
                        etaToPickup = travelToEnd + downwardStops.Count * elevator.StopTime + reverseTravel;
                    }
                }

                // Stop time at pickup floor
                etaToPickup += elevator.StopTime;

                // Phase 2: ETA from Pickup to Destination
                // -----------------------------
                int travelPickupToDestination =
                    Math.Abs(pickup - destination) * elevator.MoveTime;

                // Count stops between pickup and destination
                int stopsBetween =
                    pickup < destination
                        ? elevator.Stops.Count(s => s > pickup && s < destination)
                        : elevator.Stops.Count(s => s < pickup && s > destination);

                int etaToDestination =
                    travelPickupToDestination
                    + stopsBetween * elevator.StopTime;

                // Total ETA
                // -----------------------------
                return etaToPickup + etaToDestination;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calculating ETA for elevator {elevator.Id}: {ex.Message}");
                throw;
            }
        }
    }
}