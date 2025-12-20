using ElevatorSimulator.Models;

namespace ElevatorSimulator.Services
{
    /// <summary>
    /// Provides functionality to generate random elevator requests within a specified range of floors.
    /// </summary>
    public class RequestGenerator
    {
        private readonly int _maxFloor;

        public RequestGenerator(int maxFloor)
        {
            _maxFloor = maxFloor;
        }

        /// <summary>
        /// Generates a random elevator request with valid waiting floors.
        /// </summary>
        public ElevatorRequest GenerateRandomRequest()
        {
            try
            {
                int waitingFloor = Random.Shared.Next(1, _maxFloor + 1);

                ElevatorDirection direction;

                if (waitingFloor == 1)
                {
                    direction = ElevatorDirection.Up;
                }
                else if (waitingFloor == _maxFloor)
                {
                    direction = ElevatorDirection.Down;
                }
                else
                {
                    direction = Random.Shared.Next(0, 2) == 0
                        ? ElevatorDirection.Up
                        : ElevatorDirection.Down;
                }

                int destinationFloor = direction == ElevatorDirection.Up
                                                    ? Random.Shared.Next(waitingFloor + 1, _maxFloor + 1)
                                                    : Random.Shared.Next(1, waitingFloor);

                return new ElevatorRequest(waitingFloor, direction, destinationFloor);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to generate a random elevator request.", ex);
            }
        }
    }
}
