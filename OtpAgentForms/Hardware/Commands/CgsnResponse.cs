using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for request product serial mumber identification command response
    /// </summary>
    public class CgsnResponse : BaseResponse<CgsnResponse>
    {
        /// <summary>
        /// Modem serial number
        /// </summary>

        public string SerialNumber { get; set; }

        /// <inheritdoc/>
        public override CgsnResponse Parse(string rawResponse)
        {
            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            SerialNumber = response[0];
            return this;
        }
    }
}