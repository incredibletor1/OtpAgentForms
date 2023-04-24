using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.SimCard
{
    /// <summary>
    /// Dto for get sim card response
    /// </summary>
    public sealed class GetSimCardResponse : BaseResponse
    {
        /// <summary>
        /// Sim card model
        /// </summary>
        public Models.SimCard SimCard { get; set; }
    }
}