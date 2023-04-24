using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.SimCard
{
    /// <summary>
    /// Dto for get sim card request
    /// </summary>
    public sealed class GetSimCardRequest : BaseRequest
    {
        /// <summary>
        /// International mobile subscriber identity
        /// </summary>
        public ulong Imsi { get; set; }
    }
}