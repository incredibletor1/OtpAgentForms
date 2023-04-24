namespace OtpAgentForms.Logging
{
    /// <summary>
    /// Log message model
    /// </summary>
    internal class LogMessage
    {
        /// <summary>
        /// Log level
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Port name
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Message timestamp
        /// </summary>
        public DateTime Timestamp { get; init; }

        /// <summary>
        /// Init
        /// </summary>
        public LogMessage()
        {
            Timestamp = DateTime.Now;
        }
    }
}