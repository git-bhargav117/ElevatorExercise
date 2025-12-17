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

        public ElevatorRequest(int waitingFloor, ElevatorDirection direction)
        {
            Direction = direction;
            WaitingFloor = waitingFloor;
        }
    }
}
