using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.Provider
{
    /// <summary>
    /// Dto for get provider by imsi request 
    /// </summary>
    public sealed class GetProviderByImsiRequest : BaseRequest
    {
        /// <summary>
        /// IMSI
        /// </summary>
        public ulong Imsi { get; set; }
    }
}