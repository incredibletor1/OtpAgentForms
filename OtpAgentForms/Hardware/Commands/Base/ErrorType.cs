namespace OtpAgentForms.Hardware.Commands.Base
{
    /// <summary>
    /// At command response error type
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Generic error
        /// </summary>
        Generic = 0x01,

        /// <summary>
        /// Devive error
        /// </summary>
        Device = 0x0200,

        /// <summary>
        /// Network error
        /// </summary>
        Network = 0x03
    }
}