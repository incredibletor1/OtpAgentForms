using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for cpbw command response
    /// </summary>
    public sealed class CpbwResponse : BaseResponse<CpbwResponse>
    {        
        /// <inheritdoc/>
        public override CpbwResponse Parse(string rawResponse)
        {
            SplitResponse(rawResponse);          
            return this;
        }
    }
}
