using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.Sms
{
    /// <summary>
    /// Dto for add sms request
    /// </summary>
    public class AddSmsRequest : BaseRequest
    {
        /// <summary>
        /// Sender number
        /// </summary>
        public string SenderNumber { get; set; }

        /// <summary>
        /// Sms message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Sim card id
        /// </summary>
        public Guid SimCardId { get; set; }

        /// <summary>
        /// Sms date received
        /// (date from smsc)
        /// </summary>
        public DateTime DateReceived { get; set; }
    }
}