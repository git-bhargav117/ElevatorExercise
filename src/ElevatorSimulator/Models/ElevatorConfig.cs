namespace ElevatorSimulator.Models
{
    /// <summary>
    /// POCO class representing the configuration settings for the elevator simulation.
    /// </summary>
    public class ElevatorConfig
    {
        public required string DispatchStrategy { get; set; }
        public required BuildingSettings Building { get; set; }
        public required TimingSettings SimulationTiming { get; set; }
    }

    public class BuildingSettings
    {
        public int Floors { get; set; }
        public int Elevators { get; set; }
    }

    public class TimingSettings
    {
        public int MoveDurationInSeconds { get; set; }
        public int StopDurationInSeconds { get; set; }
        public int RandomRequestIntervalInSeconds { get; set; }
    }
}