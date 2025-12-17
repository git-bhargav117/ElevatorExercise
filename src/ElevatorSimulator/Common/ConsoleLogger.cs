namespace ElevatorSimulator.common
{
    /// <summary>
    /// Provides a simple logger implementation that writes log messages to the console output.
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <inheritdoc/>
        public void Log(string message)
        {
            Console.WriteLine($"[LOG - {DateTime.Now:HH:mm:ss}] {message}");
        }
    }
}