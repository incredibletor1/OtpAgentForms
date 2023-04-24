using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for at command response
    /// </summary>
    public class AtResponse : BaseResponse<AtResponse>
    {
        /// <inheritdoc/>
        public override AtResponse Parse(string rawResponse)
        {
            SplitResponse(rawResponse);
            return this;
        }
    }
}