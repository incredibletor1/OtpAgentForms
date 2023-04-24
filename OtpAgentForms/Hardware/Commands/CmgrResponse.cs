using OtpAgentForms.Hardware.Commands.Base;
using OtpAgentForms.Models;
using OTPAgent.Helpers;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for read SMS message command response
    /// </summary>
    public sealed class CmgrResponse : BaseResponse<CmgrResponse>
    {
        /// <summary>
        /// List sms messages
        /// </summary>
        public AtSms SMS { get; set; }

        /// <inheritdoc/>
        public override CmgrResponse Parse(string rawResponse)
        {
            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            if(response.Count == 1)
            {
                return this;
            }
            if(response.Count < 6)
            {

            }

            SMS = new AtSms()
            {
                Status = Screen(response[1]),
                SourceNumber = Ucs2.ToString(Screen(response[2])).Replace("+", ""),
                DateReceived = DateTime.Now,
                Message = Ucs2.ToString(Screen(response[7])).Replace("\n", " ")
            };
            return this;
        }
    }
}