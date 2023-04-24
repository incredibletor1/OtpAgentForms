using OtpAgentForms.Hardware.Commands.Base;

namespace OtpAgentForms.Hardware.Commands
{
    /// <summary>
    /// Dto for cpbr command response
    /// </summary>
    public sealed class CpbrResponse : BaseResponse<CpbrResponse>
    {        

        public string Number { get; set; }
        public string Name { get; set; }

        /// <inheritdoc/>
        public override CpbrResponse Parse(string rawResponse)
        {
            var response = SplitResponse(rawResponse);
            if (Error != null || response.Count == 1)
            {
                return this;
            }
            Number = Screen(response[2]);
            Name = Screen(response[4]);
            return this;
        }
    }
}
