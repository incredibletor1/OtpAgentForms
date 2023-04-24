namespace Otp.Agent.Settings
{
    /// <summary>
    /// Logging settings
    /// </summary>
    internal sealed class LoggingSettings
    {
        /// <summary>
        /// Show debug logs in console
        /// </summary>
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Show serial port trace
        /// </summary>
        public bool Serial { get; set; } = false;
    }
}