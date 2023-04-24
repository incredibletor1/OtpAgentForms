namespace OtpAgentForms.Dto.Modem.Models
{
    /// <summary>
    /// Modem model
    /// </summary>
    public sealed class Modem
    {
        /// <summary>
        /// Modem id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Modem model name
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Modem serial number
        /// </summary>
        public string SerialNumber { get; set; }

        /// <summary>
        /// Modem imei
        /// </summary>
        public ulong Imei { get; set; }

        /// <summary>
        /// Sim card id
        /// </summary>
        public Guid? SimCardId { get; set; }

        /// <summary>
        /// Current rssi level
        /// </summary>
        public int Rssi { get; set; }

        /// <summary>
        /// Serial port name
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// Is sim card inserted
        /// true if inserted
        /// </summary>
        public bool SimInserted { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Get modem hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(PortName, Rssi, SimCardId, Imei, SerialNumber, Model, Id);
        }
    }
}