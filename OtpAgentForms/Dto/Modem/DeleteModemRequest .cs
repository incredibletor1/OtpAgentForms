using OtpAgentForms.Dto.Base;

namespace OtpAgentForms.Dto.Modem
{
    /// <summary>
    /// Dto for delete modem request
    /// </summary>
    public sealed class DeleteModemRequest : BaseRequest
    {
        /// <summary>
        /// Modem id 
        /// </summary>
        public Guid Id { get; set; }
    }
}