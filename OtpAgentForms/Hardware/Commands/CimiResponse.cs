using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for request international mobile subscriber identity (IMSI) command response
    /// </summary>
    public class CimiResponse : BaseResponse<CimiResponse>
    {
        /// <summary>
        /// Intrnational mobile subscriber identity
        /// </summary>
        public ulong Imsi { get; set; }

        /// <inheritdoc/>
        public override CimiResponse Parse(string rawResponse)
        {
            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            if (!ulong.TryParse(response[0], out var result))
            {
                Error = ErrorList.NoResponse;
                return this;
            }
            Imsi = result;
            return this;
        }
    }
}