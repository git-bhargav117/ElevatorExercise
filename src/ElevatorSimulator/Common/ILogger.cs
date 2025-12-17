namespace ElevatorSimulator.common
{
    /// <summary>
    /// Provides a logging interface.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Writes a log entry with the specified message.
        /// </summary>
        /// <param name="message">The message to include in the log entry.</param>
        void Log(string message);
    }
}