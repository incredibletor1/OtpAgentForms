namespace OtpAgentForms.Dto.SimCard.Models
{
    /// <summary>
    /// Sim card model
    /// </summary>
    public sealed class SimCard
    {
        /// <summary>
        /// Sim card id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Internationsl mobile subscriber identity
        /// </summary>
        public ulong Imsi { get; set; }

        /// <summary>
        /// Provider id
        /// </summary>
        public Guid ProviderId { get; set; }

        /// <summary>
        /// Provider model
        /// </summary>
        public Provider.Models.Provider Provider { get; set; }

        /// <summary>
        /// Sim card phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Sim card create date
        /// </summary>
        public DateTime DateCreated { get; set; }
    }
}