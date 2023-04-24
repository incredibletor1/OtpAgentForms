using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for request TA model identification command response
    /// </summary>
    public class GmmResponse : BaseResponse<GmmResponse>
    {
        /// <summary>
        /// Modem model name
        /// </summary>
        public string Model { get; set; }

        /// <inheritdoc/>
        public override GmmResponse Parse(string rawResponse)
        {
            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            Model = response[0];
            return this;
        }
    }
}