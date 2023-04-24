namespace OtpAgentForms.Connector
{
    /// <summary>
    /// Serial port service interface
    /// </summary>
    internal interface IModemConnector
    {
        /// <summary>
        /// Modem event
        /// </summary>
        event ModemEventDelegate ModemEvent;

        /// <summary>
        /// Start connector
        /// </summary>
        /// <param name="portName">Serial port name</param>
        /// <param name="baudRate">Serial port baud rate</param>
        bool Start(string portName, int baudRate, Form form);

        /// <summary>
        /// Do request to serial port
        /// </summary>
        /// <param name="request">Request string</param>
        /// <param name="timeout">Request timeout. Default value = 5 sec</param>
        /// <returns>Response string</returns>
        string DoRequest(string request, int timeout = 5);

        /// <summary>
        /// Dispose connector
        /// </summary>
        void Dispose();
    }
}