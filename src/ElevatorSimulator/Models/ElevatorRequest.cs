namespace ElevatorSimulator.Models
{
    /// <summary>
    /// Represents a request for elevator service, including the floor where the request was made, the desired
    /// destination floor, and the intended direction of travel.
    /// </summary>
    public class ElevatorRequest
    {
        /// <summary>
        /// Waiting floor where the user made the request.
        /// </summary>
        public int WaitingFloor { get; }

        /// <summary>
        /// Direction of the elevator request.
        /// </summary>
        public ElevatorDirection Direction { get; }

        /// <summary>
        /// Elevator destination floor.
        /// </summary>
        public int DestinationFloor { get; }

        public ElevatorRequest(int waitingFloor, ElevatorDirection direction, int destinationFloor)
        {
            Direction = direction;
            WaitingFloor = waitingFloor;
            DestinationFloor = destinationFloor;
        }
    }
}
