using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for signal quality report command response
    /// </summary>
    public class CsqResponse : BaseResponse<CsqResponse>
    {
        /// <summary>
        /// Modem rssi
        /// </summary>
        public uint Rssi { get; set; }

        /// <inheritdoc/>
        public override CsqResponse Parse(string rawResponse)
        {
            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            if (!uint.TryParse(response[1], out var result))
            {
                Error = ErrorList.NoResponse;
                return this;
            }
            Rssi = result;
            return this;
        }
    }
}