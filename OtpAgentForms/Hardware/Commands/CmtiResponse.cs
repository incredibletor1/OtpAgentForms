using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for new sms response
    /// </summary>
    public class CmtiResponse : BaseResponse<CmtiResponse>
    {
        /// <summary>
        /// Memory type
        /// </summary>
        public string MemType { get; set; }

        /// <summary>
        /// Sms position
        /// </summary>
        public int Position { get; set; }


        /// <inheritdoc/>
        public override CmtiResponse Parse(string rawResponse)
        {
            var response = SplitResponse(rawResponse);
            if (Error != null)
            {
                return this;
            }
            MemType = Screen(response[1]);
            Position = int.Parse(response[2]);
            return this;
        }
    }
}