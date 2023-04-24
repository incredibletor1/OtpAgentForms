using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.Modem
{
    /// <summary>
    /// DTO for add or edit modem request
    /// </summary>
    public sealed class AddEditModemRequest : BaseRequest
    {
        /// <summary>
        /// Modem id
        /// </summary>
        public Guid? Id { get; set; }

        /// <summary>
        /// Serial port name
        /// </summary>
        public string PortName { get; set; }

        /// <summary>
        /// Modem model name
        /// </summary>
        public string Model { get; init; }

        /// <summary>
        /// Modem serial number
        /// </summary>
        public string SerialNumber { get; init; }

        /// <summary>
        /// Sim card id
        /// Null if sim card not installed
        /// </summary>
        public Guid? SimCardId { get; set; }

        /// <summary>
        /// Is sim card inserted
        /// true if inserted
        /// </summary>
        public bool SimInserted { get; set; }

        /// <summary>
        /// Modem imei
        /// </summary>
        public ulong Imei { get; set; }

        /// <summary>
        /// Current rssi level
        /// </summary>
        public int Rssi { get; set; }

        /// <summary>
        /// Error message
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}