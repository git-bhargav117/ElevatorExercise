namespace ElevatorSimulator.Models
{

    /// <summary>
    /// Represents an elevator model that tracks its current floor, direction, and scheduled stops.
    /// </summary>
    /// <remarks>The <see cref="Elevator"/> class models the state and basic operations of an elevator,
    /// including movement between floors and managing a queue of requested stops. It provides properties to access the
    /// elevator's current floor, direction, and timing configuration, as well as methods to add stops and advance the
    /// simulation by one time unit.</remarks>
    public class Elevator
    {
        /// <summary>
        /// Elevator identifier.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Elevator's current floor.
        /// </summary>
        public int CurrentFloor { get; private set; }

        /// <summary>
        /// Elevator's current direction.
        /// </summary>
        public ElevatorDirection Direction { get; private set; } = ElevatorDirection.Idle;

        /// <summary>
        /// Elevator's default move time in seconds between floors.
        /// </summary>
        public int MoveTime { get; } = 10;

        /// <summary>
        /// Elevator's default stop time in seconds at each floor.
        /// </summary>
        public int StopTime { get; } = 10;

        /// <summary>
        /// Elevator's scheduled stops.
        /// </summary>
        //public Queue<int> Stops { get; } = new();
        public SortedSet<int> Stops { get; } = new();

        public Elevator(int id, int initialFloor = 0, ElevatorConfig? elevatorConfig = null)
        {
            Id = id;
            CurrentFloor = initialFloor;

            if (elevatorConfig != null)
            {
                MoveTime = elevatorConfig.SimulationTiming.MoveDurationInSeconds;
                StopTime = elevatorConfig.SimulationTiming.StopDurationInSeconds;
            }
        }

        /// <summary>
        /// Determines whether the elevator is currently idle.
        /// </summary>
        public bool IsIdle => Direction == ElevatorDirection.Idle && Stops.Count == 0;

        /// <summary>
        /// Add new stop to the elevator's queue.
        /// </summary>
        /// <param name="floor"></param>
        public void AddStop(int floor)
        {
            if (!Stops.Contains(floor))
            {
                Stops.Add(floor);
            }

            if (Direction == ElevatorDirection.Idle)
            {
                SetDirection(floor);
            }
        }

        /// <summary>
        /// Simulate the current state of the elevator.
        /// </summary>
        public void Tick()
        {
            if (Stops.Count == 0)
            {
                Direction = ElevatorDirection.Idle;
                return;
            }

            int? nextStop = GetNextStop();

            // No stop in current direction → reverse
            if (nextStop == null)
            {
                Direction = Direction == ElevatorDirection.Up
                    ? ElevatorDirection.Down
                    : ElevatorDirection.Up;

                return;
            }

            if (CurrentFloor == nextStop)
            {
                Stops.Remove(nextStop.Value);
                return;
            }

            if (nextStop > CurrentFloor)
            {
                Direction = ElevatorDirection.Up;
                CurrentFloor++;
            }
            else
            {
                Direction = ElevatorDirection.Down;
                CurrentFloor--;
            }
        }

        private void SetDirection(int targetFloor)
        {
            if (targetFloor < CurrentFloor)
            {
                Direction = ElevatorDirection.Down;
            }
            else if (targetFloor > CurrentFloor)
            {
                Direction = ElevatorDirection.Up;
            }
            else
            {
                Direction = ElevatorDirection.Idle;
            }
        }

        private int? GetNextStop()
        {
            if (Direction == ElevatorDirection.Up)
                return Stops.Where(s => s >= CurrentFloor).OrderBy(s => s).FirstOrDefault();

            if (Direction == ElevatorDirection.Down)
                return Stops.Where(s => s <= CurrentFloor).OrderByDescending(s => s).FirstOrDefault();

            return null;
        }
    }
}