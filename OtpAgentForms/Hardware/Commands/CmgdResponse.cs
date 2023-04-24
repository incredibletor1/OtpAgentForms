using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for delete SMS message command response
    /// </summary>
    public sealed class CmgdResponse : BaseResponse<CmgdResponse>
    {        
        /// <inheritdoc/>
        public override CmgdResponse Parse(string rawResponse)
        {
            SplitResponse(rawResponse);          
            return this;
        }
    }
}