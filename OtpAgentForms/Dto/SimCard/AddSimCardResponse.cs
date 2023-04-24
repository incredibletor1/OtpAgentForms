using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.SimCard
{
    /// <summary>
    /// DTO for add sim card respone
    /// </summary>
    public sealed class AddSimCardResponse : BaseResponse
    {
        /// <summary>
        /// Sim card model
        /// Null if not created.
        /// </summary>
        public Models.SimCard SimCard { get; set; }
    }
}