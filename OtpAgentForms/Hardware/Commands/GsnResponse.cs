using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for request international mobile equipment identity (IMEI) command response
    /// </summary>
    public class GsnResponse : BaseResponse<GsnResponse>
    {
        /// <summary>
        /// Modem imei
        /// </summary>
        public ulong Imei { get; set; }

        /// <inheritdoc/>
        public override GsnResponse Parse(string rawResponse)
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
            Imei = result;
            return this;
        }
    }
}