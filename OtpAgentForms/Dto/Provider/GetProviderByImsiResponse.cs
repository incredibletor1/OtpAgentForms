using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.Provider
{
    /// <summary>
    /// Dto for get provider by imsi response
    /// </summary>
    public sealed class GetProviderByImsiResponse : BaseResponse
    {
        /// <summary>
        /// Provider
        /// </summary>
        public Models.Provider Provider { get; set; }
    }
}