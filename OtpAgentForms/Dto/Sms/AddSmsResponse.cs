using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.Sms
{
    /// <summary>
    /// Dto for add sms response
    /// </summary>
    public class AddSmsResponse : BaseResponse
    {
        /// <summary>
        /// Sms id
        /// null if failed
        /// </summary>
        public Guid? Id { get; set; }
    }
}