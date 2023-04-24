using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for select TE character set command response
    /// </summary>
    public sealed class CscsResponse : BaseResponse<CscsResponse>
    {        
        /// <inheritdoc/>
        public override CscsResponse Parse(string rawResponse)
        {
            SplitResponse(rawResponse);          
            return this;
        }
    }
}
