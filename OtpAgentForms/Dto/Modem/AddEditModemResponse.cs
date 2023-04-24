using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.Modem
{
    /// <summary>
    /// DTO for add or edit modem respone
    /// </summary>
    public sealed class AddEditModemResponse : BaseResponse
    {
        /// <summary>
        /// Modem id
        /// Null if not created or updated.
        /// </summary>
        public Guid? Id { get; set; }
    }
}