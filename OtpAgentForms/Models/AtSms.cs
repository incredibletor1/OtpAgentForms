namespace OtpAgentForms.Models
{
    /// <summary>
    /// At sms model
    /// </summary>
    public sealed class AtSms
    {
        /// <summary>
        /// Sms id number
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Sms status
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Source number
        /// </summary>
        public string SourceNumber { get; set; }

        /// <summary>
        /// Sms date received
        /// </summary>
        public DateTime DateReceived { get; set; }

        /// <summary>
        /// Sms message
        /// </summary>
        public string Message { get; set; }
    }
}