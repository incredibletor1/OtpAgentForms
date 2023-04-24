namespace OtpAgentForms.Logging
{
    /// <summary>
    /// Logger interface
    /// </summary>
    internal interface ILogger
    {
        /// <summary>
        /// Write debug log message
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="portName">Serial port name</param>
        void WriteDebug(string message, string portName);

        /// <summary>
        /// Write information log message
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="portName">Serial port name</param>        
        void WriteInformation(string message, string portName = null);

        /// <summary>
        /// Write error log message
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="portName">Serial port name</param>       
        void WriteError(string message, string portName = null);

        /// <summary>
        /// Write serial log message
        /// </summary>
        /// <param name="message">Log message</param>
        /// <param name="portName">Serial port name</param>
        void WriteSerial(string message, string portName = null);
    }
}