using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.SimCard
{
    /// <summary>
    /// DTO for add sim card request
    /// </summary>
    public sealed class AddSimCardRequest : BaseRequest
    {
        /// <summary>
        /// International mobile subscriber identity
        /// </summary>
        public ulong Imsi { get; set; }

        /// <summary>
        /// Provider id
        /// </summary>
        public Guid ProviderId { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string PhoneNumber { get; set; }
    }
}