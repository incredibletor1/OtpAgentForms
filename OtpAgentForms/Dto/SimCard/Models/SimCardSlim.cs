using System;

namespace OTP.Dto.SimCard.Models
{
    /// <summary>
    /// Sim card slim model
    /// </summary>
    public sealed class SimCardSlim
    {
        /// <summary>
        /// International mobile subscriber identity
        /// </summary>
        public ulong Imsi { get; set; }

        /// <summary>
        /// Sim card phone number
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Sim card provider id
        /// </summary>
        public Guid ProviderId { get; set; }
    }
}